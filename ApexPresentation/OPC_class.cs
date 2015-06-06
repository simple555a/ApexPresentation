using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace ApexPresentation
{
    class OPC_class
    {
        #region Constructors
        public OPC_class()
        {
            this.Initialized = false;
            InitializeOPC();
        }
        public OPC_class(String in_URL)
        {
            try
            {
                this.Initialized = false;

                // 1st: Create a server object and connect
                url = new Opc.URL(in_URL);
                server = new Opc.Da.Server(fact, null);

                //2nd: Connect to the created server
                server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));

                //if no exeption
                this.URL = in_URL;
                this.Initialized = true;
            }
            catch
            {
                MessageBox.Show("Bad OPC connection. Review connection string");
                this.Initialized = false;
            }
        }
        #endregion

        #region Properties
            public bool Initialized;
            private String URL;

            #region Variables for OPC client

            private Opc.URL url;
            private Opc.Da.Server server;
            private OpcCom.Factory fact = new OpcCom.Factory();

            #endregion

        #endregion

        #region Metods

            #region InitializeOPC()
                private void InitializeOPC()
                {
                    try
                    {
                        if (File.Exists("settings.xml"))
                        {
                            XmlSerializer XmlSerializer1 = new XmlSerializer(typeof(Settings));
                            TextReader reader1 = new StreamReader("settings.xml");
                            Settings Settings1 = (Settings)XmlSerializer1.Deserialize(reader1);
                            reader1.Dispose();

                            // 1st: Create a server object and connect to the RSLinx OPC Server
                            url = new Opc.URL(Settings1.OPCConnectionString);
                            server = new Opc.Da.Server(fact, null);
                            //2nd: Connect to the created server
                            server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));

                            this.Initialized = true;
                        }
                        else
                        {
                            MessageBox.Show("OPC settings is empty. See Settings - > Connection...");
                            this.Initialized = false;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Bad OPC connection. Review connection string");
                        this.Initialized = false;
                    }
                }
            #endregion
            #region public void 
            public void RefreshLabelControl(Label in_control,int in_value)
                {
                    in_control.Text = in_value.ToString();
                }

            #endregion

        #endregion


    }
}
