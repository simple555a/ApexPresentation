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
        public OPC_class(String in_URL,  String in_RingsCounterName)
        {
            try
            {
                this.Initialized = false;

                // 1st: Create a server object and connect
                url = new Opc.URL(in_URL);
                server = new Opc.Da.Server(fact, null);

                //2nd: Connect to the created server
                server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));

                //3rd Create a group if items            
                groupState = new Opc.Da.SubscriptionState();
                groupState.Name = "Group999";
                groupState.UpdateRate = 1000;// this isthe time between every reads from OPC server
                groupState.Active = true;//this must be true if you the group has to read value
                groupRead = (Opc.Da.Subscription)server.CreateSubscription(groupState);
                //groupRead.DataChanged += groupRead_DataChanged;

                items[0] = new Opc.Da.Item();
                items[0].ItemName = in_RingsCounterName;
                items = groupRead.AddItems(items);

                Opc.Da.ItemValueResult[] values = groupRead.Read(items);
                MessageBox.Show("Readed value is " + values[0].Value.ToString());

                //if no exeption
                this.URL = in_URL;
                this.Initialized = true;
            }
            catch
            {
                this.Initialized = false;
            }
        }
        #endregion

        #region Properties
            public bool Initialized;
            public Int32 CounterOfRings;
            private String URL;
            private Label ActiveLabel;

            #region Variables for OPC client

            private Opc.URL url;
            private Opc.Da.Server server;
            private OpcCom.Factory fact = new OpcCom.Factory();
            private Opc.Da.Subscription groupRead;
            private Opc.Da.SubscriptionState groupState;
            private Opc.Da.Item[] items = new Opc.Da.Item[1];

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
                            this.CounterOfRings = 0;

                            XmlSerializer XmlSerializer1 = new XmlSerializer(typeof(Settings));
                            TextReader reader1 = new StreamReader("settings.xml");
                            Settings Settings1 = (Settings)XmlSerializer1.Deserialize(reader1);
                            reader1.Dispose();

                            if (Settings1.OPCInitialized == true)
                            {
                                // 1st: Create a server object and connect to the RSLinx OPC Server
                                url = new Opc.URL(Settings1.OPCConnectionString);
                                server = new Opc.Da.Server(fact, null);
                                //2nd: Connect to the created server
                                server.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                                //3rd Create a group if items            
                                groupState = new Opc.Da.SubscriptionState();
                                groupState.Name = "Group999";
                                groupState.UpdateRate = 1000;// this isthe time between every reads from OPC server
                                groupState.Active = true;//this must be true if you the group has to read value
                                groupRead = (Opc.Da.Subscription)server.CreateSubscription(groupState);
                                groupRead.DataChanged += groupRead_DataChanged;

                                items[0] = new Opc.Da.Item();
                                items[0].ItemName = Settings1.OPCRingsCounterName;
                                items = groupRead.AddItems(items);

                                Opc.Da.ItemValueResult[] values = groupRead.Read(items);
                                //MessageBox.Show("Readed value is " + values[0].Value.ToString());
                                this.Initialized = true;
                            }
                            if (Settings1.OPCInitialized != true)
                            {
                                MessageBox.Show("OPC connection is not tested. See Settings - > Connection...");
                                this.Initialized = false;
                            }
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

                void groupRead_DataChanged(object subscriptionHandle, object requestHandle, Opc.Da.ItemValueResult[] values)
                {
                    //Convert.ToInt32(values[0].Value);
                    this.ActiveLabel.Text = (this.CounterOfRings != Convert.ToInt32(values[0].Value)) ? (this.CounterOfRings + 2).ToString() : this.CounterOfRings.ToString();
                    this.CounterOfRings+=2;
                    //this.CounterOfRings = Convert.ToInt32(values[0].Value);
                }
            #endregion
            #region void SetActiveLabel(Label in_control)
            public void SetActiveLabel(Label in_control)
            {
                this.ActiveLabel = in_control;
            }

        #endregion

        #endregion


    }
}
