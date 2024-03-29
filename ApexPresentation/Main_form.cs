﻿#define real_time
//#define bypass_opc_init

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TimeLine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ApexPresentation.TYPES;

namespace ApexPresentation
{
    public partial class Main_form : Form
    {
        public Main_form()
        {
            InitializeComponent();
        }

        static Sql_class sql_obj = new Sql_class();
#if !bypass_opc_init
        static OPC_class opc_obj_Counter = new OPC_class("OPCRingsCounterName", true);
        static OPC_class opc_obj_Apex_OnCarrier = new OPC_class("OPCRingsOnCarrierName", false);
#endif
        static Timer Tick1sec = new Timer();
        static Timer Tick5sec = new Timer();
        static Timer Tick60sec = new Timer();
        private static Settings Settings1 = new Settings();
        /// <summary>
        /// for zeroing rings counter
        /// </summary>
        static DateTime previous_time = new DateTime();


        static string actual_order_number = "";
        static string previous_order_number = "";
        static int ScheduledQty = 0;
        static int ActualQty = 0;
        static int StartOrderConter = 0;
        static bool ShowLabel12 = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            #region Indication

            toolStripStatusLabel1.Text = "REAL TIME";
#if !real_time
            toolStripStatusLabel1.ForeColor = Color.Red;
#endif
#if real_time
            toolStripStatusLabel1.ForeColor = Color.Green;
#endif

            toolStripStatusLabel2.Text = "OPC Enagaged";
#if bypass_opc_init
            toolStripStatusLabel2.ForeColor = Color.Red;
#endif
#if !bypass_opc_init
            toolStripStatusLabel2.ForeColor = Color.Green;
#endif
            #endregion


            DateTime BStartTime = new DateTime(2015, 04, 24, 00, 00, 00);
            
            label9.Text = sql_obj.GetWCName();
#if !real_time
            dateTimePicker1.Value = BStartTime;
#endif
#if real_time
            if (System.DateTime.Now.Hour<9)
                dateTimePicker1.Value = System.DateTime.Now.Date-TimeSpan.FromDays(1);
            if (System.DateTime.Now.Hour >=8)
                dateTimePicker1.Value = System.DateTime.Now.Date;
#endif
            //check shift
            if (get_CURR().Hour >= 8 && get_CURR().Hour <20) 
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;
            
            Tick1sec.Interval = 1000;
            Tick1sec.Tick += Tick1sec_Tick;
            Tick1sec.Start();

            Tick1sec.Interval = 5000;
            Tick1sec.Tick += Tick5sec_Tick;
            Tick1sec.Start();

            Tick60sec.Interval = 60000;
            Tick60sec.Tick += Tick60sec_Tick;
            Tick60sec.Start();
            GlobalPresenter();
            label5.Text = sql_obj.GetCurrentStatus();

            Color temp_color_000 = sql_obj.GetCurrentStatusColor();
            label5.BackColor = temp_color_000;
            if ((((int)temp_color_000.R + (int)temp_color_000.G + (int)temp_color_000.B))/3>=170)
                label5.ForeColor = Color.Black;
            else
                label5.ForeColor = Color.White;


            //history browser
            tableLayoutPanel2.RowStyles[2].Height = 0;
            try
            {
                if (File.Exists("settings.xml"))
                {
                    XmlSerializer XmlSerializer1 = new XmlSerializer(typeof(Settings));
                    TextReader reader1 = new StreamReader("settings.xml");
                    Settings1 = (Settings)XmlSerializer1.Deserialize(reader1);
                    reader1.Dispose();

                    showHistoryBrowserToolStripMenuItem.Checked = Settings1.GENERALShowHistoryBrowser;
                    tableLayoutPanel2.RowStyles[2].Height = (Settings1.GENERALShowHistoryBrowser) ? 35 : 0;
                }
            }
            catch
            {

            }

            LabelsCenterPositioning(groupBox1);
            LabelsCenterPositioning(groupBox2);
            LabelsCenterPositioning(groupBox3);
            
            this.Text += " v1.3.8";

            //OPC
#if !bypass_opc_init
            opc_obj_Counter.Value = sql_obj.GetRingsCounter();
            label4.Text = "Per shift: " + opc_obj_Counter.Value.ToString();
            //opc_obj.SetActiveLabel(label4);
#endif
            
            previous_time = get_CURR();
        }



        void Tick1sec_Tick(object sender, EventArgs e)
        {
            String year = System.DateTime.Now.Year.ToString();
            String month = System.DateTime.Now.ToString("MMMM");
            String day = System.DateTime.Now.Day.ToString();
            String hours = (System.DateTime.Now.TimeOfDay.Hours < 10) ? "0" + System.DateTime.Now.TimeOfDay.Hours.ToString() : System.DateTime.Now.TimeOfDay.Hours.ToString();
            String minutes = (System.DateTime.Now.TimeOfDay.Minutes < 10) ? "0" + System.DateTime.Now.TimeOfDay.Minutes.ToString() : System.DateTime.Now.TimeOfDay.Minutes.ToString();
            String seconds = (System.DateTime.Now.TimeOfDay.Seconds < 10) ? "0" + System.DateTime.Now.TimeOfDay.Seconds.ToString() : System.DateTime.Now.TimeOfDay.Seconds.ToString();


            label2.Text = year + " " + month + " " +day + " \n" + hours + ":" + minutes + ":" + seconds;
#if !bypass_opc_init
            opc_obj_Counter.AskValue();
            opc_obj_Apex_OnCarrier.AskValue();
            label4.Text = "Per Shift: " + opc_obj_Counter.Value.ToString();
            label14.Text = "On Carrier: " + opc_obj_Apex_OnCarrier.Value.ToString();
#endif
        }

        private void Tick5sec_Tick(object sender, EventArgs e)
        {
            //actual_order_number = sql_obj.GetActualOrderNumber();
            toolStripStatusLabel5.Text = "";

            //if (actual_order_number!="" && actual_order_number!=previous_order_number)
            {
                previous_order_number = actual_order_number;
                ScheduledQty = sql_obj.GetScheduledQty();
                ActualQty = sql_obj.GetActualQty();
                //StartOrderConter = opc_obj.CounterOfRings;
            }

            //if (actual_order_number != "" && actual_order_number == previous_order_number)
            {
                //ActualQty = opc_obj.CounterOfRings - StartOrderConter;
                toolStripStatusLabel5.Text = "(" + ScheduledQty.ToString() + "/" + ActualQty.ToString() + ")";
                if (ActualQty==10)
                {
                    toolStripStatusLabel6.Text = "(=1)";
                    ShowLabel12 = true;
                }
                if (ActualQty == ScheduledQty/ 2 && ActualQty != 0)
                {
                    toolStripStatusLabel6.Text = "(=2)";
                    ShowLabel12 = true;
                }
                if (ActualQty == ScheduledQty && ActualQty!=0)
                {
                    toolStripStatusLabel6.Text = "(=3)";
                    ShowLabel12 = true;
                }
            }

            label13.Text = "Current Order: " + sql_obj.GetScheduledQty().ToString();
            
        }

        void Tick60sec_Tick(object sender, EventArgs e)
        {
            //set current data in controls
            if (System.DateTime.Now.Hour < 8)
                dateTimePicker1.Value = System.DateTime.Now.Date - TimeSpan.FromDays(1);
            if (System.DateTime.Now.Hour >= 8)
                dateTimePicker1.Value = System.DateTime.Now.Date;
            if (get_CURR().Hour >= 8 && get_CURR().Hour < 20)
            {
                //MessageBox.Show("Day");
                radioButton1.Checked = true;
            }
            if (get_CURR().Hour >= 20 && get_CURR().Hour < 24 || get_CURR().Hour >= 0 && get_CURR().Hour < 8)
            {
                //MessageBox.Show("Night");
                radioButton2.Checked = true;
            }
            //set average cycle time
#if !bypass_opc_init
            label6.Text = GetAverageCycleTime(opc_obj_Counter.Value).ToString();
            //reset "rings counter" and "average cycle time" each shift change
            if (previous_time.Hour == 7 && get_CURR().Hour == 8 || previous_time.Hour == 19 && get_CURR().Hour == 20)
                opc_obj_Counter.Value = 0;
            previous_time = get_CURR();
#endif
            //show QDA message 
            if (ShowLabel12 == false)
            {
                label12.SendToBack();
            }
            if (ShowLabel12 == true)
            {
                label12.BringToFront();
                ShowLabel12 = false;
            }
            

            GlobalPresenter();
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionForm ConnectionsForm1 = new ConnectionForm();
            ConnectionsForm1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalPresenter();
        }

        /// <summary>
        /// Get EStart shift time
        /// </summary>
        /// <param name="in_StartTime">date of shift</param>
        /// <returns></returns>
        private DateTime get_T1(DateTime in_StartTime)
        {
            TimeSpan t1 = new TimeSpan(8, 0, 0);
            TimeSpan t2 = new TimeSpan(12, 0, 0);
            DateTime T1;

            if (radioButton1.Checked)
            {
                T1 = in_StartTime.Date + t1;
            }
            else
            {
                T1 = in_StartTime.Date + t1 + t2;
            }

            return T1;
        }

        /// <summary>
        /// Get End shift time
        /// </summary>
        /// <param name="in_StartTime">date of shift</param>
        /// <returns></returns>
        private DateTime get_T2(DateTime in_StartTime)
        {
            TimeSpan t1 = new TimeSpan(8, 0, 0);
            TimeSpan t2 = new TimeSpan(12, 0, 0);
            DateTime T2;

            if (radioButton1.Checked)
            {
                T2 = in_StartTime.Date + t1 + t2;
            }
            else
            {
                T2 = in_StartTime.Date + t1 + t2 + t2;
            }

            return T2;
        }

        private DateTime get_CURR()
        {
            DateTime CURR;
#if !real_time
            CURR = new DateTime(2015, 04, 24, 19, 39, 00);
#endif
#if real_time
            CURR = DateTime.Now;
#endif
            return CURR;
        }

        private void TimeLinePresenter(TimeLine.TimeLine in_control,DateTime in_StartTime)
        {
            Section[] a1;

            DateTime T1 = get_T1(in_StartTime);
            DateTime T2 = get_T2(in_StartTime);
            DateTime CURR = get_CURR();           

            
            a1 = sql_obj.GetTimeLineData(T1, T2, CURR);

            in_control.SetEmpty();

            //for ASC sql order
            if (a1.Length != 0)
            {
                in_control.AddBasePeriod(T1, T2, false);
                //not empty left
                if (a1[0].StartTime < T1 && a1[0].EndTime != DateTime.MaxValue)
                    in_control.AddPeriod(a1[0].colorRed, a1[0].colorGreen, a1[0].colorBlue, T1, a1[0].EndTime, false);
                //empty left
                if (a1[0].StartTime >= T1)
                    in_control.AddPeriod(a1[0].colorRed, a1[0].colorGreen, a1[0].colorBlue, a1[0].StartTime, a1[0].EndTime, false);
                for (int i = 1; i < a1.Length-1; i++)
                {
                    in_control.AddPeriod(a1[i].colorRed, a1[i].colorGreen, a1[i].colorBlue, a1[i].StartTime, a1[i].EndTime, false);
                }
                bool temp_is_last = false;
                //for last time
                if (a1[a1.Length - 1].EndTime == DateTime.MaxValue) temp_is_last = true;

                if (temp_is_last && T2 < CURR && a1[a1.Length - 1].StartTime > T1)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, a1[a1.Length - 1].StartTime, T2, temp_is_last);
                if (temp_is_last && T2 < CURR && a1[a1.Length - 1].StartTime <= T1)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, T1, T2, temp_is_last);
                if (temp_is_last && T1 < CURR && CURR < T2 && a1[a1.Length - 1].StartTime < T1)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, T1, CURR, temp_is_last);
                if (temp_is_last && T1 < CURR && CURR < T2 && a1[a1.Length - 1].StartTime >= T1)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, a1[a1.Length - 1].StartTime, CURR, temp_is_last);

                if (!temp_is_last && a1[a1.Length - 1].StartTime >= T1)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, a1[a1.Length - 1].StartTime, T2, temp_is_last);
                if (!temp_is_last && a1[a1.Length - 1].StartTime < T1)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, T1, T2, temp_is_last);
            }
            if (a1.Length == 0)
            {
                in_control.AddBasePeriod(T1, T2, true);
            }

            in_control.Refresh();
        }

        

        private void DataGridPresenter(DataGridView in_control, DateTime in_StartTime)
        {
            DateTime T1 = get_T1(in_StartTime);
            DateTime T2 = get_T2(in_StartTime);
            DateTime CURR = get_CURR();


            List<DataGridRow> a1 = sql_obj.GetTableStatistic(T1, T2, CURR);
            in_control.AllowUserToAddRows = false;
            in_control.Rows.Clear();
            for (int i = 0; i < a1.Count; i++)
            {
                in_control.Rows.Add();
                in_control.Rows[i].Height = 50;
                in_control.Rows[i].Cells[0].Value = a1[i].MachineCode;
                in_control.Rows[i].Cells[1].Style.BackColor = a1[i].Color;

                String hours = (TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Hours < 10) 
                    ? "0" + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Hours.ToString() + "h " 
                    : TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Hours.ToString() + "h ";
                String minutes = (TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Minutes < 10)
                    ? "0" + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Minutes.ToString() + "min "
                    : TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Minutes.ToString() + "min ";
                String seconds = (TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Seconds < 10)
                    ? "0" + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Seconds.ToString() + "sec"
                    : TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Seconds.ToString() + "sec";
                in_control.Rows[i].Cells[2].Value = hours+minutes+seconds;
                in_control.Rows[i].Cells[3].Value = a1[i].Status;
                in_control.Rows[i].Cells[4].Value = a1[i].Count;
                in_control.Rows[i].Cells[5].Value = TimeSpan.FromSeconds(Convert.ToDouble(a1[i].ExceededTime)).Hours.ToString() + 
                    "h " + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].ExceededTime)).Minutes.ToString() + 
                    "min " + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].ExceededTime)).Seconds.ToString() + "sec ";
                if (a1[i].ExceededTime != "0") in_control.Rows[i].Cells[5].Style.BackColor = Color.Red; else in_control.Rows[i].Cells[5].Style.BackColor = Color.GreenYellow;
            }

            
        }

        public int GetAverageCycleTime(int in_DoneRingsCount)
        {
            if (in_DoneRingsCount == 0) return 0;
            if (get_CURR().Hour >= 8 && get_CURR().Hour < 20)
            {
                return Convert.ToInt32(((TimeSpan.FromHours(get_CURR().Hour) + TimeSpan.FromMinutes(get_CURR().Minute) + TimeSpan.FromSeconds(get_CURR().Second) - TimeSpan.FromHours(8))).TotalSeconds / in_DoneRingsCount);
            }
            if (get_CURR().Hour >= 20 && get_CURR().Hour < 24)
            {
                return Convert.ToInt32(((TimeSpan.FromHours(get_CURR().Hour) + TimeSpan.FromMinutes(get_CURR().Minute) + TimeSpan.FromSeconds(get_CURR().Second) - TimeSpan.FromHours(20))).TotalSeconds / in_DoneRingsCount);
            }
            if (get_CURR().Hour >= 0 && get_CURR().Hour < 8)
            {
                return Convert.ToInt32(((TimeSpan.FromHours(get_CURR().Hour) + TimeSpan.FromMinutes(get_CURR().Minute) + TimeSpan.FromSeconds(get_CURR().Second) + TimeSpan.FromHours(4))).TotalSeconds / in_DoneRingsCount);
            }
            return 0;
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GlobalPresenter();
        }

        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
            GlobalPresenter();
        }

        private void radioButton2_MouseClick(object sender, MouseEventArgs e)
        {
            GlobalPresenter();
        }

        private void GlobalPresenter()
        {
            label1.Text =  sql_obj.GetOperatorName() ;
            //MessageBox.Show("GLPR");
            TimeLinePresenter(timeLine1, dateTimePicker1.Value);
            DataGridPresenter(dataGridView1, dateTimePicker1.Value);

            //MessageBox.Show(sql_obj.GetBalastedTimes(get_T1(dateTimePicker1.Value), get_T2(dateTimePicker1.Value), get_CURR()).TotalMinutes.ToString()+"/"+ (get_CURR() - get_T1(dateTimePicker1.Value)).TotalMinutes.ToString());
            //Eficiency
            //1.real time
            if (get_T1(dateTimePicker1.Value) <= get_CURR() && get_CURR() < get_T2(dateTimePicker1.Value))
                label8.Text = (
                                Math.Round(
                                            (1 - (sql_obj.GetBalastedTimes(get_T1(dateTimePicker1.Value), get_T2(dateTimePicker1.Value), get_CURR()).TotalSeconds / (get_CURR() - get_T1(dateTimePicker1.Value)).TotalSeconds)) * 100, 2
                                        )
                            ).ToString()+"%";
            //2.history
            if (get_T2(dateTimePicker1.Value) <= get_CURR())
                label8.Text = (
                                Math.Round(
                                            (1 - (sql_obj.GetBalastedTimes(get_T1(dateTimePicker1.Value), get_T2(dateTimePicker1.Value), get_CURR()).TotalSeconds / 43200)) * 100, 2
                                        )
                            ).ToString() + "%";
            label5.Text = sql_obj.GetCurrentStatus();
            label5.BackColor = sql_obj.GetCurrentStatusColor();
        }

        private void showHistoryBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHistoryBrowserToolStripMenuItem.Checked = !showHistoryBrowserToolStripMenuItem.Checked;

            Settings1.GENERALShowHistoryBrowser = showHistoryBrowserToolStripMenuItem.Checked;
            tableLayoutPanel2.RowStyles[2].Height = (Settings1.GENERALShowHistoryBrowser) ? 35 : 0;

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter("settings.xml");
            serializer.Serialize(writer, Settings1);
            writer.Dispose();

        }

        private void showHistoryBrowserToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm AboutForm1 = new AboutForm();
            AboutForm1.Show();
        }

        private void LabelsCenterPositioning(GroupBox in_GroupBox)
        {
            for (int i = 0; i < in_GroupBox.Controls.Count; i++)
            {
                
            }
             
            int component_count = 0;
            foreach (Control ctrlChild in in_GroupBox.Controls)
            {
                ctrlChild.Location = new Point(in_GroupBox.Size.Width / 2 - ctrlChild.Size.Width / 2, in_GroupBox.Size.Height / 2 - (ctrlChild.Size.Height * in_GroupBox.Controls.Count) / 2 + ctrlChild.Size.Height * component_count + 7);
                component_count++;
            }
        }

        private void Main_form_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void Main_form_Paint(object sender, PaintEventArgs e)
        {

            LabelsCenterPositioning(groupBox1);
            LabelsCenterPositioning(groupBox2);
            LabelsCenterPositioning(groupBox3);
        }
        
    }
}
