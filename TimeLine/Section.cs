using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLine
{
    public class Section 
    {
        public byte colorRed;
        public byte colorGreen;
        public byte colorBlue;


        public DateTime StartTime;

        public Section(byte in_colorRed, byte in_colorGreen, byte in_colorBlue, DateTime in_StartTime)
        {
            this.colorRed = in_colorRed;
            this.colorGreen = in_colorGreen;
            this.colorBlue = in_colorBlue;

            this.StartTime = in_StartTime;

        }
    }
}
