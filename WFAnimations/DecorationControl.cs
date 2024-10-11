using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    public class DecorationControl : System.Windows.Forms.UserControl
    {
        public DecorationType DecorationType { get; set; }
        public Control DecoratedControl { get; set; }


    }
}
