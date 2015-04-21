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

namespace ApexPresentation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime BStartTime = new DateTime(2015, 04, 12, 8, 00, 00);
            DateTime BEndTime = new DateTime(2015, 04, 13, 20, 00, 00);
            
            DateTime Period2Start = new DateTime(2015, 04, 12, 10, 00, 00);
            DateTime Period3Start = new DateTime(2015, 04, 12, 10, 30, 00);
            DateTime Period4Start = new DateTime(2015, 04, 12, 15, 00, 00);
            DateTime Period5Start = new DateTime(2015, 04, 12, 19, 00, 00);

            timeLine1.AddBasePeriod(255, 40, 40, BStartTime, BEndTime);
            timeLine1.AddPeriod(222, 255, 0, Period2Start);
            timeLine1.AddPeriod(255, 0, 255, Period3Start);
            timeLine1.AddPeriod(25, 100,70, Period4Start);
            timeLine1.AddPeriod(25, 255, 200, Period5Start);

            
            //timeLine1.toolTip1.SetToolTip(timeLine1, "123");
            //toolTip1.AutomaticDelay = 1000;

        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

    }
}
