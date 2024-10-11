using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    public class DecorationControl : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Timer tm;
        private bool isSnapshotNow = false;


        public DecorationType DecorationType { get; set; }
        public Control DecoratedControl { get; set; }
        public Padding Paddings { get; set; }
        public Bitmap CtrlBmp { get; set; }
        public byte[] CtrlPixels { get; set; }
        public int CtrlStride { get; set; }
        public Bitmap Frame { get; set; }
        public float CurrentTime { get; set; }

        public DecorationControl(DecorationType type, Control decoratedControl)
        {
            this.DecorationType = type;
            this.DecoratedControl = decoratedControl;

            decoratedControl.VisibleChanged += new EventHandler(control_VisibleChanged);
            decoratedControl.ParentChanged += new EventHandler(control_VisibleChanged);
            decoratedControl.LocationChanged += new EventHandler(control_VisibleChanged);

            decoratedControl.Paint += new PaintEventHandler(decoratedControl_Paint);

            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            InitPadding();

            tm = new System.Windows.Forms.Timer();
            tm.Interval = 100;
            tm.Tick += new EventHandler(tm_Tick);
            tm.Enabled = true;
        }

        private void InitPadding()
        {
            switch (DecorationType)
            {
                case Max_Calculator.Foreign.GAnimate.DecorationType.BottomMirror:
                    Padding = new Padding(0, 0, 0, 20);
                    break;
            }
        }

        private void tm_Tick(object sender, EventArgs e)
        {
            switch (DecorationType)
            {
                case Max_Calculator.Foreign.GAnimate.DecorationType.BottomMirror:
                case Max_Calculator.Foreign.GAnimate.DecorationType.Custom:
                    Invalidate();
                    break;
            }
        }

        private void decoratedControl_Paint(object sender, PaintEventArgs e)
        {
            if (!isSnapshotNow)
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            CtrlBmp = GetForeground(DecoratedControl);
            CtrlPixels = GetPixels(CtrlBmp);

            if (Frame != null)
                Frame.Dispose();
            Frame = OnNonLinearTransfromNeeded();

            if (Frame != null)
            {
                e.Graphics.DrawImage(Frame, Point.Empty);
            }
        }

        private void control_VisibleChanged(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            this.Parent = DecoratedControl.Parent;
            this.Visible = DecoratedControl.Visible;
            this.Location = new Point(DecoratedControl.Left - Padding.Left, DecoratedControl.Top - Padding.Top);


            if (Parent != null)
            {
                var i = Parent.Controls.GetChildIndex(DecoratedControl);
                Parent.Controls.SetChildIndex(this, i + 1);
            }

            var newSize = new Size(DecoratedControl.Width + Padding.Left + Padding.Right, DecoratedControl.Height + Padding.Top + Padding.Bottom);
            if (newSize != Size)
            {
                this.Size = newSize;
            }
        }

        protected virtual Bitmap GetForeground(Control ctrl)
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            if (!ctrl.IsDisposed)
            {
                isSnapshotNow = true;
                ctrl.DrawToBitmap(bmp, new Rectangle(Padding.Left, Padding.Top, ctrl.Width, ctrl.Height));
                isSnapshotNow = false;
            }
            return bmp;
        }

        byte[] GetPixels(Bitmap bmp)
        {
            const int bytesPerPixel = 4;
            PixelFormat pxf = PixelFormat.Format32bppArgb;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, pxf);
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];
            Marshal.Copy(ptr, argbValues, 0, numBytes);
            bmp.UnlockBits(bmpData);
            return argbValues;
        }



    }
}
