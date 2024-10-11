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

        public float TransparencyCoeff { get; set; }
        public float LeafCoeff { get; set; }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(PointFConverter))]
        public PointF MosaicShift { get; set; }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(PointFConverter))]
        public PointF MosaicCoeff { get; set; }

        public int MosaicSize { get; set; }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(PointFConverter))]
        public PointF BlindCoeff { get; set; }

        public float TimeCoeff { get; set; }
        public float MinTime { get; set; }
        public float MaxTime { get; set; }
        public Padding Padding { get; set; }
        public bool AnimateOnlyDifferences { get; set; }


        public bool IsNonLinearTransformNeeded
        {
            get
            {
                if (this.BlindCoeff == PointF.Empty)
                    if (this.MosaicCoeff == PointF.Empty || this.MosaicSize == 0)
                        if (this.TransparencyCoeff == 0f)
                            if (this.LeafCoeff == 0f)
                                return false;

                return true;
            }
        }

        public Animation()
        {
            this.MinTime = 0f;
            this.MaxTime = 1f;
            this.AnimateOnlyDifferences = true;
        }

        public Animation Clone()
        {
            return (Animation)MemberwiseClone();
        }

        public static WFAnimations.Animation Rotate { get { return new Animation { RotateCoeff = 1f, TransparencyCoeff = 1, Padding = new Padding(50, 50, 50, 50) }; } }





    }
}
