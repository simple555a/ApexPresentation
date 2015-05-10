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
            DateTime BStartTime = new DateTime(2015, 04, 24, 18, 00, 00);
            DateTime BEndTime = new DateTime(2015, 04, 25, 00, 00, 00);
            
            DateTime Period2Start = new DateTime(2015, 04, 12, 9, 00, 00);
            DateTime Period3Start = new DateTime(2015, 04, 12, 9, 30, 00);
            DateTime Period4Start = new DateTime(2015, 04, 12, 18, 00, 00);
            DateTime Period5Start = new DateTime(2015, 04, 12, 19, 48, 00);


            label1.Text = sql_obj.GetOperatorName();

            Section[] a1 = sql_obj.GetTimeLineData(BStartTime, BEndTime);

            TimeLineShow(timeLine1, BStartTime, BEndTime, a1);

            

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
            TimeSpan t1 = new TimeSpan(8, 0, 0);
            TimeSpan t2 = new TimeSpan(12, 0, 0);
            DateTime T1, T2;
            Section[] a1;

            if (radioButton1.Checked)
            {
                T1 = dateTimePicker1.Value + t1;
                T2 = dateTimePicker1.Value + t1 + t2;
            }
            else
            {
                T1 = dateTimePicker1.Value + t1 + t2;
                T2 = dateTimePicker1.Value + t1 + t2 + t2;
            }

            a1 = sql_obj.GetTimeLineData(T1, T2);

            TimeLineShow(timeLine1, T1, T2, a1);
        }
 
        private void TimeLineShow(TimeLine.TimeLine in_control, DateTime in_StartTime, DateTime in_EndTime, Section[] in_sections)
        {
            in_control.SetEmpty();
            if (in_sections.Length != 0)
            {
                timeLine1.AddBasePeriod(in_StartTime, in_EndTime);
                timeLine1.AddPeriod(in_sections[in_sections.Length - 1].colorRed, in_sections[in_sections.Length - 1].colorGreen, in_sections[in_sections.Length - 1].colorBlue, in_StartTime, in_sections[in_sections.Length - 1].StartTime, false);
                for (int i = in_sections.Length - 2; i > 0; i--)
                {
                    timeLine1.AddPeriod(in_sections[i].colorRed, in_sections[i].colorGreen, in_sections[i].colorBlue, in_sections[i].StartTime, in_sections[i].EndTime, false);
                }
                bool temp_is_last = false;
                if (in_sections[0].EndTime == DateTime.MinValue) temp_is_last = true;
                timeLine1.AddPeriod(in_sections[0].colorRed, in_sections[0].colorGreen, in_sections[0].colorBlue, in_sections[0].StartTime, /*in_sections[0].StartTime.AddHours(1)*/in_EndTime, temp_is_last);
            }
            in_control.Refresh();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
