using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    public class DecorationControl : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Timer tm;


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


    }
}
