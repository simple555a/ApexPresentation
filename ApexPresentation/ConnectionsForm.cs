using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ApexPresentation
{
    public partial class ConnectionsForm : Form
    {
        public ConnectionsForm()
        {
            InitializeComponent();
        }

        private static Settings Settings1 = new Settings();

        private void Connections_form_Load(object sender, EventArgs e)
        {
            label5.Text = "";
            label6.Text = "";
            if (File.Exists("settings.xml"))
            {
                XmlSerializer XmlSerializer1 = new XmlSerializer(typeof(Settings));
                TextReader reader1 = new StreamReader("settings.xml");
                Settings1 = (Settings)XmlSerializer1.Deserialize(reader1);
                reader1.Dispose();

                this.textBox1.Text = Settings1.SQLConnectionString;
                this.textBox3.Text = Settings1.OPCConnectionString;
                this.textBox2.Text = Settings1.OPCGroupName;
                this.textBox4.Text = Settings1.OPCRingsCounterName;
                 
            }
        }

        //save and close
        private void button1_Click(object sender, EventArgs e)
        {
            Settings1.SQLConnectionString = this.textBox1.Text;
            Settings1.OPCConnectionString = this.textBox3.Text;
            Settings1.OPCGroupName = this.textBox2.Text;
            Settings1.OPCRingsCounterName = this.textBox4.Text;
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter("settings.xml");
            serializer.Serialize(writer, Settings1);
            writer.Dispose();

            this.Dispose();
        }

        //test SQL connection
        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            this.button2.Text = "Testing...";
            Sql_class sql_obj = new Sql_class(this.textBox1.Text);
            Settings1.SQLInitialized = sql_obj.Initialized;
            label5.Text = (Settings1.SQLInitialized) ? "Ok" : "Not OK";
            this.button2.Enabled = true;
            this.button2.Text = "Test connection";
            
        }

        //test OPC connection
        private void button3_Click(object sender, EventArgs e)
        {
            this.button3.Enabled = false;
            this.button3.Text = "Testing...";
            OPC_class opc_obj = new OPC_class(textBox3.Text,textBox2.Text,textBox4.Text);
            Settings1.OPCInitialized = opc_obj.Initialized;
            label6.Text = (Settings1.OPCInitialized) ? "Ok" : "Not OK";
            this.button3.Enabled = true;
            this.button3.Text = "Test connection";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
