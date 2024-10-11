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
        public static Animation HorizSlide { get { return new Animation { SlideCoeff = new PointF(1, 0) }; } }
        public static Animation VertSlide { get { return new Animation { SlideCoeff = new PointF(0, 1) }; } }
        public static Animation Scale { get { return new Animation { ScaleCoeff = new PointF(1, 1) }; } }
        public static Animation ScaleAndRotate { get { return new Animation { ScaleCoeff = new PointF(1, 1), RotateCoeff = 0.5f, RotateLimit = 0.2f, Padding = new Padding(30, 30, 30, 30) }; } }
        public static Animation HorizSlideAndRotate { get { return new Animation { SlideCoeff = new PointF(1, 0), RotateCoeff = 0.3f, RotateLimit = 0.2f, Padding = new Padding(50, 50, 50, 50) }; } }
        public static Animation ScaleAndHorizSlide { get { return new Animation { ScaleCoeff = new PointF(1, 1), SlideCoeff = new PointF(1, 0), Padding = new Padding(30, 0, 0, 0) }; } }
        public static Animation Transparent { get { return new Animation { TransparencyCoeff = 1 }; } }
        public static Animation Leaf { get { return new Animation { LeafCoeff = 1 }; } }
        public static Animation Mosaic { get { return new Animation { MosaicCoeff = new PointF(100f, 100f), MosaicSize = 20, Padding = new Padding(30, 30, 30, 30) }; } }
        public static Animation Particles { get { return new Animation { MosaicCoeff = new PointF(200, 200), MosaicSize = 1, MosaicShift = new PointF(0, 0.5f), Padding = new Padding(100, 50, 100, 150), TimeCoeff = 2 }; } }


    }
}
