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
        public String Count;
        public String ExceededTime;

        public DataGridRow(String in_MachineCode, Color in_Color, String in_SummaryTime, String in_Status, String in_Count, String in_ExceededTime)
        {
            this.MachineCode = in_MachineCode;
            this.Color = in_Color;
            this.SummaryTime = in_SummaryTime;
            this.Status = in_Status;
            this.Count = in_Count;
            this.ExceededTime = in_ExceededTime;
        }

        public DataGridRow() { }

        public void Dispose()
        { 
            GC.SuppressFinalize(this);           
        }
                
    }
}
