using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TimeLine
{
    public partial class TimeLine : UserControl
    {
        public TimeLine()
        {
            InitializeComponent();
        }

        

        private void TimeLine_Paint(object sender, PaintEventArgs e)
        {
            //global settings and vars
            Color color1 = new Color();
            Pen pen;
            System.Drawing.Font font_004 = new System.Drawing.Font("Arial", 9);
            SolidBrush brush_004 = new SolidBrush(Color.Black);
            String tempString = "";

            this.LeftMargin = 5;
            this.RightMargin = 5;
            this.TimeLineHeight = 50;
            this.TimeLineX1 = this.LeftMargin;
            this.TimeLineY1 = 0;
            this.TimeLineX2 = this.Width - this.RightMargin;
            this.TimeLineY2 = this.TimeLineHeight;
            this.TimeLineWidth = this.TimeLineX2 - this.TimeLineX1;

            

            //drawing base
            color1 = Color.FromArgb(this.BaseColor_R, this.BaseColor_G, this.BaseColor_B);
            pen = new Pen(color1);
            pen.Width = this.TimeLineHeight;
            e.Graphics.DrawLine(pen, this.LeftMargin, this.TimeLineHeight / 2, this.Width - this.RightMargin,  this.TimeLineHeight / 2);
            
            //000. calculating sum of Times
            double SumOfTimesInData=System.Convert.ToDouble(this.EndTime.Subtract(this.StartTime).TotalSeconds);

            // drawing periods and metrics from Data
            #region //drawing per hour metric (for each hour, undepend Data list)
            /*
            double TotalHoursBtwStartAndEnd;
            double TotalSecondsBtwStartAndEnd;
            TimeSpan delta;
            DateTime NearEntireDateTime;
            int StartXcoord;
            int OneHourWidth;
             * */
            #endregion
            for (int i=0;i<this.Data.Count;i++)
            {
                //drawing color periods
                color1 = Color.FromArgb(this.Data[i].colorRed, this.Data[i].colorGreen, this.Data[i].colorBlue);
                pen = new Pen(color1);
                pen.Width = this.TimeLineHeight;
                if (i!=this.Data.Count-1)
                e.Graphics.DrawLine(pen,
                    this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                    System.Convert.ToInt16(this.TimeLineHeight / 2),
                    this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i + 1].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                    System.Convert.ToInt16(this.TimeLineHeight / 2));
                else
                    e.Graphics.DrawLine(pen,
                    this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                    System.Convert.ToInt16(this.TimeLineHeight / 2),
                    this.TimeLineX1 + System.Convert.ToInt16((((this.EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                    System.Convert.ToInt16(this.TimeLineHeight / 2));

                //drawing metrics
                if (i < this.Data.Count)
                {
                    color1 = Color.FromArgb(0, 0, 0);
                    pen = new Pen(color1);
                    pen.Width = 1;
                    e.Graphics.DrawLine(pen, 
                        this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                        this.TimeLineY2+1,
                        this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                        this.TimeLineY2+6);
                }

               


                //drawing times
                tempString = (this.Data[i].StartTime.Hour < 10) ? "0" + this.Data[i].StartTime.Hour.ToString() : this.Data[i].StartTime.Hour.ToString();
                tempString += ":";
                tempString += (this.Data[i].StartTime.Minute < 10) ? "0" + this.Data[i].StartTime.Minute.ToString() : this.Data[i].StartTime.Minute.ToString();

                //print first time from Data
                if (i == 0)
                {
                    if (System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) > 20 && i < this.Data.Count)
                    {
                        e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 13, 55);
                    }
                }
                //print last time from Data
                if (i == this.Data.Count-1)
                {
                    if (System.Convert.ToInt16((((this.EndTime - this.Data[i].StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) > 15 && i < this.Data.Count)
                    {
                        e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 13, 55);
                    }
                }
                //print intermediate times from Data
                if (i>0 && i < this.Data.Count - 1)
                {
                    if (System.Convert.ToInt16((((this.Data[i].StartTime - this.Data[i - 1].StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) > 36 && i < this.Data.Count)
                    {
                        e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 12, 55);
                    }
                }
            }

            // print start and end time
            tempString = this.StartTime.ToString();
            e.Graphics.DrawString(tempString, font_004, brush_004, this.TimeLineX1, this.TimeLineY2+20);

            tempString = this.EndTime.ToString();
            e.Graphics.DrawString(tempString, font_004, brush_004, this.TimeLineX2-117, this.TimeLineY2 + 20);

            //draw title time rectangle and lines
            e.Graphics.DrawLine(pen, this.TimeLineX1, this.TimeLineY2, this.TimeLineX1, this.TimeLineY2 + 20);
            e.Graphics.DrawLine(pen, this.TimeLineX2, this.TimeLineY2, this.TimeLineX2, this.TimeLineY2 + 20);

            e.Graphics.DrawRectangle(pen, this.TimeLineX1, this.TimeLineY2 + 20, 117, 14);
            e.Graphics.DrawRectangle(pen, this.TimeLineX2-117, this.TimeLineY2 + 20, 117, 14);


            //drawing black border rectangle
            color1 = Color.FromArgb(0, 0, 0);
            pen = new Pen(color1);
            pen.Width = 1;
            e.Graphics.DrawRectangle(pen, this.LeftMargin, 0, this.Width - (this.RightMargin+this.LeftMargin), this.TimeLineHeight);


        }

        private void TimeLine_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void TimeLine_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void TimeLine_Load(object sender, EventArgs e)
        {
           
        }


    }
}
