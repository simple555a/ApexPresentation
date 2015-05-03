using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //load connection string
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime BStartTime = new DateTime(2015, 04, 12, 8, 00, 00);
            DateTime BEndTime = new DateTime(2015, 04, 13, 20, 00, 00);
            
            DateTime Period2Start = new DateTime(2015, 04, 12, 9, 00, 00);
            DateTime Period3Start = new DateTime(2015, 04, 12, 9, 30, 00);
            DateTime Period4Start = new DateTime(2015, 04, 12, 18, 00, 00);
            DateTime Period5Start = new DateTime(2015, 04, 12, 19, 48, 00);

            timeLine1.AddBasePeriod(255, 200, 255, BStartTime, BEndTime);
            timeLine1.AddPeriod(222, 255, 0, Period2Start);
            timeLine1.AddPeriod(255, 0, 255, Period3Start);
            timeLine1.AddPeriod(25, 100,70, Period4Start);
            timeLine1.AddPeriod(255, 255, 255, Period5Start);

            Sql_class sql_obj = new Sql_class();
            label1.Text = sql_obj.GetOperatorName();

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

        }

    }
}
