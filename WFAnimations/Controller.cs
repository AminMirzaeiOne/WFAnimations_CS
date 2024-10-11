using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    /// <summary>
    /// DoubleBitmap displays animation
    /// </summary>
    public class Controller
    {
        protected Bitmap ctrlBmp;
        Point[] buffer;
        byte[] pixelsBuffer;
        protected Rectangle CustomClipRect;
        AnimateMode mode;
        Animation animation;



        protected Bitmap BgBmp { get { return (DoubleBitmap as IFakeControl).BgBmp; } set { (DoubleBitmap as IFakeControl).BgBmp = value; } }
        public Bitmap Frame { get { return (DoubleBitmap as IFakeControl).Frame; } set { (DoubleBitmap as IFakeControl).Frame = value; } }


        public float CurrentTime { get; private set; }
        protected float TimeStep { get; private set; }

        public bool Upside { get; set; } = false;

        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;
        public event EventHandler<NonLinearTransfromNeededEventArg> NonLinearTransfromNeeded;
        public event EventHandler<PaintEventArgs> FramePainting;
        public event EventHandler<PaintEventArgs> FramePainted;
        public event EventHandler<MouseEventArgs> MouseDown;

        public Control DoubleBitmap { get; private set; }
        public Control AnimatedControl { get; set; }


        public void Dispose()
        {
            if (ctrlBmp != null)
                BgBmp.Dispose();
            if (ctrlBmp != null)
                ctrlBmp.Dispose();
            if (Frame != null)
                Frame.Dispose();
            AnimatedControl = null;

            Hide();
        }

        public void Hide()
        {
            if (DoubleBitmap != null)
                try
                {
                    DoubleBitmap.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (DoubleBitmap.Visible) DoubleBitmap.Hide();
                        DoubleBitmap.Parent = null;
                        //DoubleBitmap.Dispose();
                    }));
                }
                catch { }
        }

        protected virtual Rectangle GetBounds()
        {
            return new Rectangle(
                AnimatedControl.Left - animation.Padding.Left,
                AnimatedControl.Top - animation.Padding.Top,
                AnimatedControl.Size.Width + animation.Padding.Left + animation.Padding.Right,
                AnimatedControl.Size.Height + animation.Padding.Top + animation.Padding.Bottom);
        }

        protected virtual Rectangle ControlRectToMyRect(Rectangle rect)
        {
            return new Rectangle(
                animation.Padding.Left + rect.Left,
                animation.Padding.Top + rect.Top,
                rect.Width + animation.Padding.Left + animation.Padding.Right,
                rect.Height + animation.Padding.Top + animation.Padding.Bottom);
        }

        public Controller(Control control, AnimateMode mode, Animation animation, float timeStep, Rectangle controlClipRect)
        {
            if (control is Form)
                DoubleBitmap = new DoubleBitmapForm();
            else
                DoubleBitmap = new DoubleBitmapControl();

            (DoubleBitmap as IFakeControl).FramePainting += OnFramePainting;
            (DoubleBitmap as IFakeControl).FramePainted += OnFramePainting;
            (DoubleBitmap as IFakeControl).TransfromNeeded += OnTransfromNeeded;
            DoubleBitmap.MouseDown += OnMouseDown;

            this.animation = animation;
            this.AnimatedControl = control;
            this.mode = mode;

            this.CustomClipRect = controlClipRect;

            if (mode == AnimateMode.Show || mode == AnimateMode.BeginUpdate)
                timeStep = -timeStep;

            this.TimeStep = timeStep * (animation.TimeCoeff == 0f ? 1f : animation.TimeCoeff);
            if (this.TimeStep == 0f)
                timeStep = 0.01f;

            try
            {
                switch (mode)
                {
                    case AnimateMode.Hide:
                        {
                            BgBmp = GetBackground(control);
                            (DoubleBitmap as IFakeControl).InitParent(control, animation.Padding);
                            ctrlBmp = GetForeground(control);
                            DoubleBitmap.Visible = true;
                            control.Visible = false;
                        }
                        break;

                    case AnimateMode.Show:
                        {
                            BgBmp = GetBackground(control);
                            (DoubleBitmap as IFakeControl).InitParent(control, animation.Padding);
                            DoubleBitmap.Visible = true;
                            DoubleBitmap.Refresh();
                            control.Visible = true;
                            ctrlBmp = GetForeground(control);
                        }
                        break;

                    case AnimateMode.BeginUpdate:
                    case AnimateMode.Update:
                        {
                            (DoubleBitmap as IFakeControl).InitParent(control, animation.Padding);
                            BgBmp = GetBackground(control, true);
                            DoubleBitmap.Visible = true;

                        }
                        break;
                }
            }
            catch
            {
                Dispose();
            }
#if debug
            BgBmp.Save("c:\\bgBmp.png");
            if (ctrlBmp != null)
                ctrlBmp.Save("c:\\ctrlBmp.png");
#endif

            CurrentTime = timeStep > 0 ? animation.MinTime : animation.MaxTime;
        }

        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(this, e);
        }

        protected virtual void OnFramePainting(object sender, PaintEventArgs e)
        {
            var oldFrame = Frame;
            Frame = null;

            if (mode == AnimateMode.BeginUpdate)
                return;

            Frame = OnNonLinearTransfromNeeded();

            if (oldFrame != Frame && oldFrame != null)
                oldFrame.Dispose();

            var time = CurrentTime + TimeStep;
            if (time > animation.MaxTime) time = animation.MaxTime;
            if (time < animation.MinTime) time = animation.MinTime;
            CurrentTime = time;

            if (FramePainting != null)
                FramePainting(this, e);
        }

        protected virtual void OnFramePainted(object sender, PaintEventArgs e)
        {
            if (FramePainted != null)
                FramePainted(this, e);
        }

        protected virtual Bitmap GetBackground(Control ctrl, bool includeForeground = false, bool clip = false)
        {
            if (ctrl is Form)
                return GetScreenBackground(ctrl, includeForeground, clip);

            var bounds = GetBounds();
            var w = bounds.Width;
            var h = bounds.Height;
            if (w == 0) w = 1;
            if (h == 1) h = 1;
            Bitmap bmp = new Bitmap(w, h);


            var clientRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            PaintEventArgs ea = new PaintEventArgs(Graphics.FromImage(bmp), clientRect);
            if (clip)
            {
                if (CustomClipRect == default(Rectangle))
                    ea.Graphics.SetClip(new Rectangle(0, 0, w, h));
                else
                    ea.Graphics.SetClip(CustomClipRect);
            }

            if (ctrl.Parent != null)
            {
                for (int i = ctrl.Parent.Controls.Count - 1; i >= 0; i--)
                {
                    var c = ctrl.Parent.Controls[i];
                    if (c == ctrl && !includeForeground) break;
                    if (c.Visible && !c.IsDisposed)
                        if (c.Bounds.IntersectsWith(bounds))
                        {
                            using (Bitmap cb = new Bitmap(c.Width, c.Height))
                            {
                                c.DrawToBitmap(cb, new Rectangle(0, 0, c.Width, c.Height));
                                /*if (c == ctrl)
                                    ea.Graphics.SetClip(clipRect);*/
                                ea.Graphics.DrawImage(cb, c.Left - bounds.Left, c.Top - bounds.Top, c.Width, c.Height);
                            }
                        }
                    if (c == ctrl) break;
                }
            }


            ea.Graphics.Dispose();

            return bmp;
        }


    }
}
