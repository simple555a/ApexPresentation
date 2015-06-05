#define real_time

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
        static OPC_class opc_obj = new OPC_class();
        static Timer global_clock = new Timer();
        static Timer refresh_form_timer = new Timer(); 

        private void Form1_Load(object sender, EventArgs e)
        {

            DateTime BStartTime = new DateTime(2015, 04, 24, 00, 00, 00);
            
            label1.Text = sql_obj.GetOperatorName();
#if !real_time
            dateTimePicker1.Value = BStartTime;
#endif
#if real_time
            dateTimePicker1.Value = System.DateTime.Now.Date;
#endif
            //check shift
            if (get_CURR().Hour >= 8 && get_CURR().Hour <20) 
                radioButton1.Checked = true; 
            else
                radioButton2.Checked = true;

            global_clock.Interval = 1000;
            global_clock.Tick += global_clock_Tick;
            global_clock.Start();

            refresh_form_timer.Interval = 60000;
            refresh_form_timer.Tick += refresh_form_timer_Tick;
            refresh_form_timer.Start();
            GlobalPresenter();
            label5.Text = sql_obj.GetCurrentStatus();
            label5.BackColor = sql_obj.GetCurrentStatusColor();

            this.Text += " (serpikov.sergey@gmail.com)";

            //OPC
            //opc_obj.
        }

        void refresh_form_timer_Tick(object sender, EventArgs e)
        {
            GlobalPresenter();
        }

        void global_clock_Tick(object sender, EventArgs e)
        {
            String hours = (System.DateTime.Now.TimeOfDay.Hours < 10) ? "0" + System.DateTime.Now.TimeOfDay.Hours.ToString() : System.DateTime.Now.TimeOfDay.Hours.ToString();
            String minutes = (System.DateTime.Now.TimeOfDay.Minutes < 10) ? "0" + System.DateTime.Now.TimeOfDay.Minutes.ToString() : System.DateTime.Now.TimeOfDay.Minutes.ToString();
            String seconds = (System.DateTime.Now.TimeOfDay.Seconds < 10) ? "0" + System.DateTime.Now.TimeOfDay.Seconds.ToString() : System.DateTime.Now.TimeOfDay.Seconds.ToString();


            label2.Text = hours + ":" + minutes + ":" + seconds;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionsForm ConnectionsForm1 = new ConnectionsForm();
            ConnectionsForm1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //for (int i = 0; i < 1000; i++)
            GlobalPresenter();
        }

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

                if (!temp_is_last)
                    in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, a1[a1.Length - 1].StartTime, T2, temp_is_last);
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
                in_control.Rows[i].Cells[0].Value = a1[i].MachineCode;
                in_control.Rows[i].Cells[1].Style.BackColor = a1[i].Color;
                in_control.Rows[i].Cells[2].Value = TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Hours.ToString() +"h " 
                    + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Minutes.ToString() + "min " 
                    + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].SummaryTime)).Seconds.ToString() + "sec ";
                in_control.Rows[i].Cells[3].Value = a1[i].Status;
                in_control.Rows[i].Cells[4].Value = a1[i].Count;
                in_control.Rows[i].Cells[5].Value = TimeSpan.FromSeconds(Convert.ToDouble(a1[i].ExceededTime)).Hours.ToString() + 
                    "h " + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].ExceededTime)).Minutes.ToString() + 
                    "min " + TimeSpan.FromSeconds(Convert.ToDouble(a1[i].ExceededTime)).Seconds.ToString() + "sec ";
                if (a1[i].ExceededTime != "0") in_control.Rows[i].Cells[5].Style.BackColor = Color.Red; else in_control.Rows[i].Cells[5].Style.BackColor = Color.GreenYellow;
            }

            
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
            label1.Text = sql_obj.GetOperatorName();

            TimeLinePresenter(timeLine1, dateTimePicker1.Value);
            DataGridPresenter(dataGridView1, dateTimePicker1.Value);

            label5.Text = sql_obj.GetCurrentStatus();
            label5.BackColor = sql_obj.GetCurrentStatusColor();
        }
       

        
    }
}
