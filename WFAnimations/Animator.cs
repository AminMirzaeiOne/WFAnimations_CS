using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    /// <summary>
    /// Animation manager
    /// </summary>
    [ProvideProperty("Decoration", typeof(System.Windows.Forms.Control))]
    public class Animator : Component, IExtenderProvider
    {
        IContainer components = null;
        protected List<QueueItem> queue = new List<QueueItem>();
        private Thread thread;
        System.Windows.Forms.Timer timer;
        private AnimationType animationType;
        Control invokerControl;
        int counter;



        /// <summary>
        /// Occurs when animation of the control is completed
        /// </summary>
        public event EventHandler<AnimationCompletedEventArg> AnimationCompleted;

        /// <summary>
        /// Ocuurs when all animations are completed
        /// </summary>
        public event EventHandler AllAnimationsCompleted;

        /// <summary>
        /// Occurs when needed transform matrix
        /// </summary>
        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;

        /// <summary>
        /// Occurs when needed non-linear transformation
        /// </summary>
        public event EventHandler<NonLinearTransfromNeededEventArg> NonLinearTransfromNeeded;

        /// <summary>
        /// Occurs when user click on the animated control
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseDown;

        /// <summary>
        /// Occurs when frame of animation is painting
        /// </summary>
        public event EventHandler<PaintEventArgs> FramePainted;

        /// <summary>
        /// Max time of animation (ms)
        /// </summary>
        [DefaultValue(1500)]
        public int MaxAnimationTime { get; set; }

        /// <summary>
        /// Default animation
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Animation DefaultAnimation { get; set; }

        /// <summary>
        /// Cursor of animated control
        /// </summary>
        [DefaultValue(typeof(Cursor), "Default")]
        public Cursor Cursor { get; set; }

        /// <summary>
        /// Are all animations completed?
        /// </summary>
        public bool IsCompleted
        {
            get { lock (queue) return queue.Count == 0; }
        }

        /// <summary>
        /// Interval between frames (ms)
        /// </summary>
        [DefaultValue(10)]
        public int Interval
        {
            get;
            set;
        }

        public bool Upside { get; set; } = false;

        /// <summary>
        /// Type of built-in animation
        /// </summary>
        public AnimationType AnimationType
        {
            get { return animationType; }
            set { animationType = value; InitDefaultAnimation(animationType); }
        }

        public Animator()
        {
            Init();
        }

        public Animator(IContainer container)
        {
            container.Add(this);
            Init();
        }

        protected virtual void Init()
        {
            AnimationType = Max_Calculator.Foreign.GAnimate.AnimationType.VertSlide;
            DefaultAnimation = new Animation();
            MaxAnimationTime = 1500;
            TimeStep = 0.02f;
            Interval = 10;

            Disposed += new EventHandler(Animator_Disposed);

            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1;
            timer.Start();
        }

        private void Start()
        {
            //main working thread
            thread = new Thread(Work);
            thread.IsBackground = true;
            thread.Name = "Animator thread";
            thread.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            //create invoker in main UI therad
            invokerControl = new Control();
            invokerControl.CreateControl();
            //
            Start();
        }

        void Animator_Disposed(object sender, EventArgs e)
        {
            ClearQueue();
            if (thread != null)
                thread.Abort();
        }

        void Work()
        {
            while (true)
            {
                Thread.Sleep(Interval);
                try
                {
                    var count = 0;
                    var completed = new List<QueueItem>();
                    var actived = new List<QueueItem>();

                    //find completed
                    lock (queue)
                    {
                        count = queue.Count;
                        var wasActive = false;

                        foreach (var item in queue)
                        {
                            if (item.IsActive) wasActive = true;

                            if (item.controller != null && item.controller.IsCompleted)
                                completed.Add(item);
                            else
                            {
                                if (item.IsActive)
                                {
                                    if ((DateTime.Now - item.ActivateTime).TotalMilliseconds > MaxAnimationTime)
                                        completed.Add(item);
                                    else
                                        actived.Add(item);
                                }
                            }
                        }
                        //start next animation
                        if (!wasActive)
                            foreach (var item in queue)
                                if (!item.IsActive)
                                {
                                    actived.Add(item);
                                    item.IsActive = true;
                                    break;
                                }
                    }

                    //completed
                    foreach (var item in completed)
                        OnCompleted(item);

                    //build next frame of DoubleBitmap
                    foreach (var item in actived)
                        try
                        {
                            var item2 = item;
                            //build next frame of DoubleBitmap
                            //item.control.BeginInvoke(new MethodInvoker(() => DoAnimation(item2)));
                            invokerControl.BeginInvoke(new MethodInvoker(() => DoAnimation(item2)));
                        }
                        catch
                        {
                            //we can not start animation, remove from queue
                            OnCompleted(item);
                        }

                    if (count == 0)
                    {
                        if (completed.Count > 0)
                            OnAllAnimationsCompleted();
                        CheckRequests();
                    }
                }
                catch
                {
                    //form was closed
                }
            }
        }

        /// <summary>
        /// Check result state of controls
        /// </summary>
        private void CheckRequests()
        {
            var toRemove = new List<QueueItem>();

            lock (requests)
            {
                var dict = new Dictionary<Control, QueueItem>();
                foreach (var item in requests)
                    if (item.control != null)
                    {
                        if (dict.ContainsKey(item.control))
                            toRemove.Add(dict[item.control]);
                        dict[item.control] = item;
                    }
                    else
                        toRemove.Add(item);

                foreach (var item in dict.Values)
                {
                    if (item.control != null && !IsStateOK(item.control, item.mode))
                    {
                        if (invokerControl != null)
                            RepairState(item.control, item.mode);
                    }
                    else
                        toRemove.Add(item);
                }

                foreach (var item in toRemove)
                    requests.Remove(item);
            }
        }

        bool IsStateOK(Control control, AnimateMode mode)
        {
            switch (mode)
            {
                case AnimateMode.Hide: return !control.Visible;
                case AnimateMode.Show: return control.Visible;
            }

            return true;
        }

        void RepairState(Control control, AnimateMode mode)
        {
            invokerControl.Invoke(new MethodInvoker(() =>
            {
                try
                {
                    switch (mode)
                    {
                        case AnimateMode.Hide:
                            control.Visible = false;
                            break;
                        case AnimateMode.Show:
                            control.Visible = true;
                            break;
                    }
                }
                catch
                {
                    //form was closed
                }
            }));
        }

        private void DoAnimation(QueueItem item)
        {
            lock (item)
            {
                try
                {
                    if (item.controller == null)
                    {
                        item.controller = CreateDoubleBitmap(item.control, item.mode, item.animation,
                                                             item.clipRectangle);
                    }
                    if (item.controller.IsCompleted)
                        return;
                    item.controller.BuildNextFrame();
                }
                catch
                {
                    if (item.controller != null)
                        item.controller.Dispose();
                    OnCompleted(item);
                }
            }
        }

        private void InitDefaultAnimation(AnimationType animationType)
        {
            switch (animationType)
            {
                case AnimationType.Custom: break;
                case AnimationType.Rotate: DefaultAnimation = Animation.Rotate; break;
                case AnimationType.HorizSlide: DefaultAnimation = Animation.HorizSlide; break;
                case AnimationType.VertSlide: DefaultAnimation = Animation.VertSlide; break;
                case AnimationType.Scale: DefaultAnimation = Animation.Scale; break;
                case AnimationType.ScaleAndRotate: DefaultAnimation = Animation.ScaleAndRotate; break;
                case AnimationType.HorizSlideAndRotate: DefaultAnimation = Animation.HorizSlideAndRotate; break;
                case AnimationType.ScaleAndHorizSlide: DefaultAnimation = Animation.ScaleAndHorizSlide; break;
                case AnimationType.Transparent: DefaultAnimation = Animation.Transparent; break;
                case AnimationType.Leaf: DefaultAnimation = Animation.Leaf; break;
                case AnimationType.Mosaic: DefaultAnimation = Animation.Mosaic; break;
                case AnimationType.Particles: DefaultAnimation = Animation.Particles; break;
                case AnimationType.VertBlind: DefaultAnimation = Animation.VertBlind; break;
                case AnimationType.HorizBlind: DefaultAnimation = Animation.HorizBlind; break;
            }
        }

        /// <summary>
        /// Time step
        /// </summary>
        [DefaultValue(0.02f)]
        public float TimeStep { get; set; }

        /// <summary>
        /// Shows the control. As result the control will be shown with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void Show(Control control, bool parallel = false, Animation animation = null)
        {
            AddToQueue(control, AnimateMode.Show, parallel, animation);
        }

        /// <summary>
        /// Shows the control and waits while animation will be completed. As result the control will be shown with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void ShowSync(Control control, bool parallel = false, Animation animation = null)
        {
            Show(control, parallel, animation);
            WaitAnimation(control);
        }

        /// <summary>
        /// Hides the control. As result the control will be hidden with animation.
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="parallel">Allows to animate it same time as other animations</param>
        /// <param name="animation">Personal animation</param>
        public void Hide(Control control, bool parallel = false, Animation animation = null)
        {
            this.AddToQueue(control, AnimateMode.Hide, parallel, animation);
        }



    }
}
