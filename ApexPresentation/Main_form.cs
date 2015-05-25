//#define real_time

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



namespace ApexPresentation
{
    public partial class Main_form : Form
    {
        public Main_form()
        {
            InitializeComponent();

        }

        static Sql_class sql_obj = new Sql_class();

        private void Form1_Load(object sender, EventArgs e)
        {

            DateTime BStartTime = new DateTime(2015, 04, 24, 00, 00, 00);
            
            label1.Text = sql_obj.GetOperatorName();

            dateTimePicker1.Value = BStartTime;
#if real_time
            dateTimePicker1.Value = System.DateTime.Now.Date;
#endif
            

            TimeLinePresenter(timeLine1, dateTimePicker1.Value);
            DataGridShow(dataGridView1);
            label5.Text = sql_obj.GetCurrentStatus();
            label5.BackColor = sql_obj.GetCurrentStatusColor();

            this.Text += " (serpikov.sergey@gmail.com)";
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionsForm ConnectionsForm1 = new ConnectionsForm();
            ConnectionsForm1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TimeLinePresenter(timeLine1,dateTimePicker1.Value);
        }
 
        private void TimeLinePresenter(TimeLine.TimeLine in_control,DateTime in_StartTime)
        {
            TimeSpan t1 = new TimeSpan(8, 0, 0);
            TimeSpan t2 = new TimeSpan(12, 0, 0);

            DateTime T1, T2, CURR;
            Section[] a1;

            CURR = new DateTime(2015, 04, 24, 19, 39, 00);
#if real_time
            CURR = DateTime.Now;
#endif

            if (radioButton1.Checked)
            {
                T1 = in_StartTime.Date + t1;
                T2 = in_StartTime.Date + t1 + t2;
            }
            else
            {
                T1 = in_StartTime.Date + t1 + t2;
                T2 = in_StartTime.Date + t1 + t2 + t2;
            }


            a1 = sql_obj.GetTimeLineData(T1, T2, CURR);

            in_control.SetEmpty();

            //for DESC sql order
            //if (a1.Length != 0)
            //{
            //    in_control.AddBasePeriod(T1, T2, false);
            //    //not empty left
            //    if (a1[a1.Length - 1].StartTime < T1 && a1[a1.Length - 1].EndTime != DateTime.MaxValue)
            //        in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, T1, a1[a1.Length - 1].EndTime, false);
            //    //empty left
            //    if (a1[a1.Length - 1].StartTime >= T1)
            //        in_control.AddPeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, a1[a1.Length - 1].StartTime, a1[a1.Length - 1].EndTime, false);
            //    for (int i = a1.Length - 2; i > 0; i--)
            //    {
            //        in_control.AddPeriod(a1[i].colorRed, a1[i].colorGreen, a1[i].colorBlue, a1[i].StartTime, a1[i].EndTime, false);
            //    }
            //    bool temp_is_last = false;
            //    //for last time
            //    if (a1[0].EndTime == DateTime.MaxValue) temp_is_last = true;

            //    if (temp_is_last && T2 < CURR && a1[0].StartTime>T1)
            //        in_control.AddPeriod(a1[0].colorRed, a1[0].colorGreen, a1[0].colorBlue, a1[0].StartTime, T2, temp_is_last);
            //    if (temp_is_last && T2 < CURR && a1[0].StartTime<=T1)
            //        in_control.AddPeriod(a1[0].colorRed, a1[0].colorGreen, a1[0].colorBlue, T1, T2, temp_is_last);
            //    if (temp_is_last && T1 < CURR && CURR < T2 && a1[0].StartTime < T1)
            //        in_control.AddPeriod(a1[0].colorRed, a1[0].colorGreen, a1[0].colorBlue, T1, CURR, temp_is_last);
            //    if (temp_is_last && T1 < CURR && CURR < T2 && a1[0].StartTime >= T1)
            //        in_control.AddPeriod(a1[0].colorRed, a1[0].colorGreen, a1[0].colorBlue, a1[0].StartTime, CURR, temp_is_last);
            //}

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
            }
            if (a1.Length == 0)
            {
                in_control.AddBasePeriod(T1, T2, true);
            }

            in_control.Refresh();
        }

        private void DataGridShow(DataGridView in_control)
        {
            //in_control.Rows[0].Cells[0].Value="1";
            DataGridViewRow r0 = new DataGridViewRow();
            DataGridViewCell c0;
            
            
        }

        private void DataGridPresenter()
        {

        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            TimeLinePresenter(timeLine1, dateTimePicker1.Value);
        }

        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
            TimeLinePresenter(timeLine1, dateTimePicker1.Value);
        }

        private void radioButton2_MouseClick(object sender, MouseEventArgs e)
        {
            TimeLinePresenter(timeLine1, dateTimePicker1.Value);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        
    }
}
