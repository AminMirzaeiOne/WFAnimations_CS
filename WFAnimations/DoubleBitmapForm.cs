using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    public partial class DoubleBitmapForm : Form, IFakeControl
    {
        Bitmap bgBmp;
        Bitmap frame;

        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;


        public DoubleBitmapForm()
        {
            InitializeComponent();
            Visible = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            TopMost = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }
    }
}
