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

    }
}
