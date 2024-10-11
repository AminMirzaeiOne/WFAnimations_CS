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


    }
}
