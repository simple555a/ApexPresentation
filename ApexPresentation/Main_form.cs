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
            DateTime BStartTime = new DateTime(2015, 04, 23, 0, 00, 00);
            DateTime BEndTime = new DateTime(2015, 04, 23, 8, 00, 00);
            
            DateTime Period2Start = new DateTime(2015, 04, 12, 9, 00, 00);
            DateTime Period3Start = new DateTime(2015, 04, 12, 9, 30, 00);
            DateTime Period4Start = new DateTime(2015, 04, 12, 18, 00, 00);
            DateTime Period5Start = new DateTime(2015, 04, 12, 19, 48, 00);


            label1.Text = sql_obj.GetOperatorName();

            //Section[] a1 = sql_obj.GetTimeLineData(BStartTime, BEndTime);
            //if (a1.Length != 0)
            //{
            //    timeLine1.AddBasePeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, BStartTime, BEndTime);
            //    for (int i = a1.Length - 2; i >= 0; i--)
            //    {
            //        timeLine1.AddPeriod(a1[i].colorRed, a1[i].colorGreen, a1[i].colorBlue, a1[i].StartTime);
            //    }
            //}
            //timeLine1.SetEmpty();

            

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
            timeLine1.SetEmpty();
            if (a1.Length != 0)
            {
                timeLine1.AddBasePeriod(a1[a1.Length - 1].colorRed, a1[a1.Length - 1].colorGreen, a1[a1.Length - 1].colorBlue, T1, T2);
                for (int i = a1.Length - 2; i >= 0; i--)
                {
                    timeLine1.AddPeriod(a1[i].colorRed, a1[i].colorGreen, a1[i].colorBlue, a1[i].StartTime);
                }
            }
            timeLine1.Refresh();
        } 

    }
}
