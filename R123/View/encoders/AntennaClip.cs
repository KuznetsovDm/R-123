using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace R123.View
{
    public class AntennaClip : Encoder
    {
        private Image image;
        private double displayAngle;
        public AntennaClip(Image image) : base(image, 360)
        {
            this.image = image;

            Options.Encoders.AthenaDisplay.ValueChanged += AthenaDisplay_ValueChanged;
        }
        //public 

        private void AthenaDisplay_ValueChanged()
        {
            
        }
    }
}
