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
            Color color2 = new Color();
            Pen pen1, pen2;
            System.Drawing.Font font_004 = new System.Drawing.Font("Arial", 9);
            System.Drawing.Font font_005 = new System.Drawing.Font("Arial", 30);
            SolidBrush brush_004 = new SolidBrush(Color.Black);
            TextureBrush brush_001 = new TextureBrush(Properties.Resources.chess_texture);
            String tempString = "";

            this.LeftMargin = 0;
            this.RightMargin = 1;
            this.TimeLineHeight = 50;
            this.TimeLineX1 = this.LeftMargin;
            this.TimeLineY1 = 0;
            this.TimeLineX2 = this.Width - this.RightMargin;
            this.TimeLineY2 = this.TimeLineHeight;
            this.TimeLineWidth = this.TimeLineX2 - this.TimeLineX1;

            #region print start and end time
            tempString = this.StartTime.ToString();
            e.Graphics.DrawString(tempString, font_004, brush_004, this.TimeLineX1, this.TimeLineY2 + 20);

            tempString = this.EndTime.ToString();
            e.Graphics.DrawString(tempString, font_004, brush_004, this.TimeLineX2 - 117, this.TimeLineY2 + 20);

            //draw title time rectangle and lines
            color1 = Color.FromArgb(0, 0, 0);
            pen1 = new Pen(color1);
            pen1.Width = 1;
            e.Graphics.DrawLine(pen1, this.TimeLineX1, this.TimeLineY2, this.TimeLineX1, this.TimeLineY2 + 20);
            e.Graphics.DrawLine(pen1, this.TimeLineX2, this.TimeLineY2, this.TimeLineX2, this.TimeLineY2 + 20);

            e.Graphics.DrawRectangle(pen1, this.TimeLineX1, this.TimeLineY2 + 20, 117, 14);
            e.Graphics.DrawRectangle(pen1, this.TimeLineX2 - 117, this.TimeLineY2 + 20, 117, 14);
            #endregion

            #region drawing chess field

            pen2 = new Pen(brush_001);
            pen2.Width = this.TimeLineHeight;
            e.Graphics.DrawLine(pen2, this.TimeLineX1, (this.TimeLineY1 + this.TimeLineHeight) / 2, this.TimeLineX2, (this.TimeLineY1 + this.TimeLineHeight) / 2);

            #endregion

            #region drawing black border rectangle
            color1 = Color.FromArgb(0, 0, 0);
            pen1 = new Pen(color1);
            pen1.Width = 1;
            e.Graphics.DrawRectangle(pen1, this.LeftMargin, 0, this.Width - (this.RightMargin + this.LeftMargin), this.TimeLineHeight);
            #endregion

            #region drawing per hour metric (for each hour, undepend Data list)
            int total_hours = (this.EndTime - this.StartTime).Hours;
            for (int i = 1; i < total_hours; i++)
            {
                color1 = Color.FromArgb(0, 0, 0);
                pen1 = new Pen(color1);
                pen1.Width = 1;
                e.Graphics.DrawLine(pen1,
                    this.TimeLineX1 + System.Convert.ToInt16(((i) * this.TimeLineWidth) / total_hours),
                    this.TimeLineY2 + 1,
                    this.TimeLineX1 + System.Convert.ToInt16(((i) * this.TimeLineWidth) / total_hours),
                    this.TimeLineY2 + 5);

                tempString = (((this.StartTime.Hour + i) % 24) < 10) ? "0" + ((this.StartTime.Hour + i) % 24).ToString() : ((this.StartTime.Hour + i) % 24).ToString();
                tempString += ":00";

                e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16(((i) * this.TimeLineWidth) / total_hours) - 17, 55);
            }
            #endregion

            #region  if (!this.SetEmpty_property)

            if (!this.SetEmpty_property)
            {
                
                //000. calculating sum of Times
                double SumOfTimesInData = System.Convert.ToDouble(this.EndTime.Subtract(this.StartTime).TotalSeconds);

                #region drawing triangle
                if (this.Data.Count!=0                                         
                    && this.Data[this.Data.Count - 1].is_last == true 
                    && this.Data[this.Data.Count - 1].EndTime < this.EndTime)
                        {
                            color1 = Color.FromArgb(0, 0, 0);
                            pen1 = new Pen(color1);
                            pen1.Width = 1;
                            Point[] points = new Point[3];
                            points[0].X = this.TimeLineX1 + System.Convert.ToInt16((((this.Data[this.Data.Count - 1].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) + 0;
                            points[0].Y = this.TimeLineY2 + 1;
                            points[1].X = this.TimeLineX1 + System.Convert.ToInt16((((this.Data[this.Data.Count - 1].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) + 3;
                            points[1].Y = this.TimeLineY2 + 4;
                            points[2].X = this.TimeLineX1 + System.Convert.ToInt16((((this.Data[this.Data.Count - 1].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 3;
                            points[2].Y = this.TimeLineY2 + 4;
                            e.Graphics.DrawPolygon(pen1, points);
                        }

                        #endregion

                // drawing periods and metrics from Data
                for (int i = 0; i < this.Data.Count; i++)
                {


                    #region drawing color periods
                    color1 = Color.FromArgb(this.Data[i].colorRed, this.Data[i].colorGreen, this.Data[i].colorBlue);
                    pen1 = new Pen(color1);
                    pen1.Width = this.TimeLineHeight;
                        
                    if (i<this.Data.Count-1)
                        e.Graphics.DrawLine(pen1,
                            this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                            System.Convert.ToInt16(this.TimeLineHeight / 2),
                            this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i + 1].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                            System.Convert.ToInt16(this.TimeLineHeight / 2));
                    if (i == this.Data.Count - 1 && this.Data[i].is_last != true)
                        e.Graphics.DrawLine(pen1,
                        this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                        System.Convert.ToInt16(this.TimeLineHeight / 2),
                        this.TimeLineX1 + System.Convert.ToInt16((((this.EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                        System.Convert.ToInt16(this.TimeLineHeight / 2)); 
                    if (i == this.Data.Count - 1 && this.Data[i].is_last == true)
                        e.Graphics.DrawLine(pen1,
                        this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                        System.Convert.ToInt16(this.TimeLineHeight / 2),
                        this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                        System.Convert.ToInt16(this.TimeLineHeight / 2));
                        
                    #endregion

                    #region drawing metrics
                    if (i < this.Data.Count )
                    {
                        color1 = Color.FromArgb(0, 0, 0);
                        pen1 = new Pen(color1);
                        pen1.Width = 1;
                        e.Graphics.DrawLine(pen1,
                            this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                            this.TimeLineY2 + 0,
                            this.TimeLineX1 + System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData),
                            this.TimeLineY2 + 1);
                    }
                    #endregion

                    #region drawing times

                    //tempString = (this.Data[i].StartTime.Hour < 10) ? "0" + this.Data[i].StartTime.Hour.ToString() : this.Data[i].StartTime.Hour.ToString();
                    //tempString += ":";
                    //tempString += (this.Data[i].StartTime.Minute < 10) ? "0" + this.Data[i].StartTime.Minute.ToString() : this.Data[i].StartTime.Minute.ToString();



                    ////print first time from Data
                    //if (i == 0)
                    //{
                    //    if (System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) > 20 && i < this.Data.Count)
                    //    {
                    //        e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 13, 55);
                    //    }
                    //}
                    ////print last time from Data
                    //if (i == this.Data.Count - 1)
                    //{
                    //    if (System.Convert.ToInt16((((this.EndTime - this.Data[i].StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) > 15 && i < this.Data.Count)
                    //    {
                    //        e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 13, 55);
                    //    }
                    //}
                    ////print intermediate times from Data
                    //if (i > 0 && i < this.Data.Count - 1)
                    //{
                    //    if (System.Convert.ToInt16((((this.Data[i].StartTime - this.Data[i - 1].StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) > 36 && i < this.Data.Count)
                    //    {
                    //        e.Graphics.DrawString(tempString, font_004, brush_004, System.Convert.ToInt16((((this.Data[i].StartTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 12, 55);
                    //    }
                    //}

                    #endregion
                }

                #region drawing triangle
                if (this.Data.Count!=0 && this.Data[0].is_last == true && this.Data[0].EndTime < this.EndTime)
                {
                    color1 = Color.FromArgb(0, 0, 0);
                    pen1 = new Pen(color1);
                    pen1.Width = 1;
                    Point[] points = new Point[3];
                    points[0].X = this.TimeLineX1 + System.Convert.ToInt16((((this.Data[0].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) + 0;
                    points[0].Y = this.TimeLineY2 + 1;
                    points[1].X = this.TimeLineX1 + System.Convert.ToInt16((((this.Data[0].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) + 3;
                    points[1].Y = this.TimeLineY2 + 4;
                    points[2].X = this.TimeLineX1 + System.Convert.ToInt16((((this.Data[0].EndTime - this.StartTime).TotalSeconds) * this.TimeLineWidth) / SumOfTimesInData) - 3;
                    points[2].Y = this.TimeLineY2 + 4;
                    e.Graphics.DrawPolygon(pen1, points);
                }

                    #endregion

                //drawing black border rectangle
                color1 = Color.FromArgb(0, 0, 0);
                pen1 = new Pen(color1);
                pen1.Width = 1;
                e.Graphics.DrawRectangle(pen1, this.LeftMargin, 0, this.Width - (this.RightMargin + this.LeftMargin), this.TimeLineHeight);
            }
            #endregion

            #region  if (this.SetEmpty_property)
            if (this.SetEmpty_property)
            {
                

                tempString = "There is no data :(";
                e.Graphics.DrawString(tempString, font_005, brush_004, this.TimeLineX1 + 10, this.TimeLineY1+3);
            }
            #endregion

        }

        private void TimeLine_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }



        private void TimeLine_Load(object sender, EventArgs e)
        {
           
        }


    }
}
