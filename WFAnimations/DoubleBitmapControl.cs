using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAnimations
{
    public class DoubleBitmapControl : System.Windows.Forms.Control, IFakeControl
    {
        Bitmap bgBmp;
        Bitmap frame;
    }
}
