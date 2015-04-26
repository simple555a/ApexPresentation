﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ApexPresentation
{
    class Sql_class
    {
        #region 1. Properties

        private String ConnectionString;

        #endregion


        #region 2. Metods

        public bool SetConnection()
        {
            SqlConnection con = new SqlConnection("Data Source=SERZH\\SQLEXPRESS;Initial Catalog=SFI_local_PC_SQL");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"SELECT" + 
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
            "[SFI_local_PC_SQL].[dbo].[sfi_SLCOperatorLogin] AS OPL" + 
            "INNER JOIN " + 
            "[SLC_rsActive].[dbo].[APP_USER] AS AU" + 
            "ON OPL.[user_name] = AU.[user_name]" + 
            "WHERE OPL.login_time > (GETDATE() - '08:00:00.000') and OPL.wc_name = 'BAA21'";
            cmd.Connection = con;
            con.Open(); //if needed!
            //rest of youe code...

            //on the end close connection and command:
            cmd.Dispose();
            con.Dispose();

            //this.ConnectionString = ConnectionString;
            return true;
        }


        #endregion
    }
}
