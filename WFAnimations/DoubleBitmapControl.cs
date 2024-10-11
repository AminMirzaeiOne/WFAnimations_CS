using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    public class DoubleBitmapControl : System.Windows.Forms.Control, IFakeControl
    {
        Bitmap bgBmp;
        Bitmap frame;

        Bitmap IFakeControl.BgBmp { get { return this.bgBmp; } set { this.bgBmp = value; } }
        Bitmap IFakeControl.Frame { get { return this.frame; } set { this.frame = value; } }


        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;
        public event EventHandler<PaintEventArgs> FramePainted;


    }
}
