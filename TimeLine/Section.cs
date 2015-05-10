using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine
{
    public class Section 
    {
        public byte colorRed;
        public byte colorGreen;
        public byte colorBlue;

        public bool is_last;
        public DateTime StartTime;
        public DateTime EndTime;

        public Section(byte in_colorRed, byte in_colorGreen, byte in_colorBlue, DateTime in_StartTime, DateTime in_EndTime, bool is_last)
        {
            this.colorRed = in_colorRed;
            this.colorGreen = in_colorGreen;
            this.colorBlue = in_colorBlue;

            this.is_last = is_last;

            this.StartTime = in_StartTime;
            this.EndTime = in_EndTime;

        }

        public Section() { }
    }
}
