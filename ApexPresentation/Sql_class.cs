using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using TimeLine;

namespace ApexPresentation
{
    class Sql_class
    {
        #region 1. Constructor
        /// <summary>
        /// Create Sql_class object.
        /// </summary>
        /// <param name="SQLServerName"></param>
        /// <param name="SQLExemplarName"></param>
        /// <returns> True - connection tested and ok, false - bad connection</returns>
        public Sql_class()
        {
            InitializeSQL();
        }


        #endregion


        #region 2. Properties

        private String ConnectionString;

        #endregion


        #region 3. Metods


        public void InitializeSQL()
        {
            try
            {
                if (File.Exists("settings.xml"))
                {
                    XmlSerializer XmlSerializer1 = new XmlSerializer(typeof(Settings));
                    TextReader reader1 = new StreamReader("settings.xml");
                    Settings Settings1 = (Settings)XmlSerializer1.Deserialize(reader1);
                    reader1.Dispose();

                    SqlConnection con = new SqlConnection("Data Source=" + Settings1.SQLServerName + "\\" + Settings1.SQLExemplarName + ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True");
                    con.Open();
                    //if ok - fill connection string field
                    this.ConnectionString = "Data Source=" + Settings1.SQLServerName + "\\" + Settings1.SQLExemplarName + ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True";

                }
                else
                {
                    MessageBox.Show("Settings is empty. See Settings - > Connection...");
                }


            }
            catch
            {
                throw new ArgumentException("Bad connection. Review server name or exemplar name or sql server");
            }
        }
        public String GetOperatorName()
        {
            InitializeSQL();

            String SQLQuery = @"SELECT " +
                            "OPL.[user_name]" +
                            ",AU.[first_name]" +
                            ",AU.[last_name]" +
                            ",OPL.[wc_name]" +
                            ",OPL.[station_name]" +
                            ",OPL.[login_type]" +
                            ",OPL.[login_time]" +
                            ",OPL.[logout_time]" +
                            ",OPL.[message_type]" +
                            "FROM " +
                            "[SFI_local_PC_SQL].[dbo].[sfi_SLCOperatorLogin] AS OPL " +
                            "INNER JOIN " +
                            "[SLC_rsActive].[dbo].[APP_USER] AS AU " +
                            "ON OPL.[user_name] = AU.[user_name]" +
                            "WHERE OPL.login_time > (GETDATE() - '08:00:00.000') and OPL.wc_name = 'BAA21'";

                using (SqlConnection con = new SqlConnection(this.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(SQLQuery,con))
                    {
                        using (SqlDataReader reader= cmd.ExecuteReader())
                            
                        while (reader.Read())
                        {
                            return reader.GetString(1) + " " + reader.GetString(2);
                        }
                        return "Nobody";
                    }
                }
        }
        public void GetTimeLineData()
        {
            
        }

        #endregion


    }   
}
