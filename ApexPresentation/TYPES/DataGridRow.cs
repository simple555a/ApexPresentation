using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ApexPresentation.TYPES
{
    class DataGridRow: IDisposable
    {
        public String MachineCode;
        public Color Color;
        public String SummaryTime;
        public String Status;

        public DataGridRow(String MachineCode, Color Color, String SummaryTime, String Status)
        {
            this.MachineCode = MachineCode;
            this.Color = Color;
            this.SummaryTime = SummaryTime;
            this.Status = Status;
        }

        public DataGridRow() { }

        public void Dispose()
        { 
            GC.SuppressFinalize(this);           
        }
                
    }
}
