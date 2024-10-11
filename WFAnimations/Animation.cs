using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WFAnimations
{
    /// <summary>
    /// Settings of animation
    /// </summary>
    public class Animation
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(PointFConverter))]
        public PointF SlideCoeff { get; set; }

        public float RotateCoeff { get; set; }
        public float RotateLimit { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(PointFConverter))]
        public PointF ScaleCoeff { get; set; }


    }
}
