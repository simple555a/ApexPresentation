﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;
using TimeLine;
using ApexPresentation.TYPES;

namespace ApexPresentation
{
    class Sql_class
    {
        #region 1. Constructor

        public  Sql_class()
        {
            this.Initialized = false;
            InitializeSQL();
        }
        public  Sql_class(String ServerName, String ExemplarName)
        {
            this.Initialized = false;

            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + ServerName + "\\" + ExemplarName + ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True");
                con.Open();
                //if ok - fill connection string field
                this.ConnectionString = "Data Source=" + ServerName + "\\" + ExemplarName + ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True";

                this.Initialized = true;
            }
            catch
            {
                MessageBox.Show("Bad connection. Review server name or exemplar name or sql server");
                this.Initialized = false;
            }
        }

        #endregion


        #region 2. Properties

        private String ConnectionString;
        public bool Initialized;

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

                    this.Initialized = true;
                }
                else
                {
                    MessageBox.Show("Settings is empty. See Settings - > Connection...");
                    this.Initialized = false;
                }


            }
            catch
            {
                MessageBox.Show("Bad connection. Review server name or exemplar name or sql server");
                this.Initialized = false;
            }
        }
        public String GetOperatorName()
        {
            if (!this.Initialized) return "***************";

            //TODO: OPL.wc_name = 'BAA21' - need changed
            String SQLQuery = @"SELECT 
                            OPL.[user_name]
                            ,AU.[first_name]
                            ,AU.[last_name]
                            ,OPL.[wc_name]
                            ,OPL.[station_name]
                            ,OPL.[login_type]
                            ,OPL.[login_time]
                            ,OPL.[logout_time]
                            ,OPL.[message_type]
                            FROM 
                            [SFI_local_PC_SQL].[dbo].[sfi_SLCOperatorLogin] AS OPL 
                            INNER JOIN 
                            [SLC_rsActive].[dbo].[APP_USER] AS AU 
                            ON OPL.[user_name] = AU.[user_name]
                            WHERE OPL.login_time > (GETDATE() - '08:00:00.000') and OPL.wc_name = 'BAA21'";

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
        public Section[] GetTimeLineData(DateTime in_StartTime, DateTime in_EndTime, DateTime in_CURR)
        {
            Section[] NULL_return = new Section[0];
            if (!this.Initialized) return NULL_return;

            Section[] a1;

            String SQLQuery = @"SELECT DISTINCT
                                [MachineState]
                                ,COLORS.[ColorValue]
                                ,[StartTime]
                                ,[EndTime]
                                FROM [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStateHistory]
                                INNER JOIN
                                [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
                                ON [MachineState]=COLORS.StatusCode
                                WHERE 
                                [StartTime]>=CONVERT(DATETIME,'" + in_StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120) AND [StartTime]<CONVERT(DATETIME,'" + in_EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120)" +
                                "OR [EndTime]>=CONVERT(DATETIME,'" + in_StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120) AND [StartTime]<CONVERT(DATETIME,'" + in_EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120)" +
                                "OR [StartTime]<CONVERT(DATETIME,'" + in_StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120) AND CONVERT(DATETIME,'" + in_StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120)<CONVERT(DATETIME,'" + in_CURR.ToString("yyyy-MM-dd HH:mm:ss") + "',120) AND [EndTime] IS NULL " +
                                "ORDER BY [StartTime] asc";


            String SQLQuery_getCount = @"SELECT DISTINCT
                                    [MachineState]
                                    ,COLORS.[ColorValue]
                                    ,[StartTime]
                                    FROM [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStateHistory]
                                    INNER JOIN
                                    [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
                                    ON [MachineState]=COLORS.StatusCode
                                    WHERE 
                                    [StartTime] BETWEEN CONVERT(DATETIME,'" + in_StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120) AND CONVERT(DATETIME,'" + in_EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "',120)" +
                                    "ORDER BY [StartTime] asc";

            //get count
            Int32 RecordsCount=0;
            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RecordsCount++;
                        }
                    }
                }
            }
             
            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        a1 = new Section[RecordsCount];
                        for (int i = 0; i < RecordsCount; i++)
                        {
                            reader.Read();
                            a1[i] = new Section();
                                a1[i].StartTime = reader.GetDateTime(2);

                            try
                            {
                                a1[i].EndTime = reader.GetDateTime(3);
                            }
                            catch
                            {
                                a1[i].EndTime = DateTime.MaxValue;
                            }
                            a1[i].colorBlue = Convert.ToByte(reader.GetInt64(1) >> 16);
                            a1[i].colorGreen = Convert.ToByte((reader.GetInt64(1) >> 8) & 255);
                            a1[i].colorRed = Convert.ToByte((reader.GetInt64(1)  & 255));
                            
                        }
                    }
                }
            }

            return a1;
        }
        public string GetCurrentStatus()
        {
            if (!this.Initialized) return "***************";

            string return_string="";

            String SQLQuery = @"SELECT DISTINCT
                                [StatusDescription],
                                [Language],
                                [EndTime] 
                                FROM [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStateHistory]
                                INNER JOIN
                                [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
                                ON [MachineState]=COLORS.StatusCode
                                WHERE 
                                [EndTime] IS NULL 
                                AND ([Language]='ru-RU' OR [Language]='en-US')";

            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                            reader.Read();
                            return_string += reader.GetString(0);
                            reader.Read();
                            return_string += " - "+reader.GetString(0);
                            

                    }
                }
            }
            

            return return_string;
        }
        public Color GetCurrentStatusColor()
        {
            if (!this.Initialized) return Color.White;

            Byte colorBlue,
                colorGreen,
                colorRed;

            String SQLQuery = @"SELECT DISTINCT
                                [ColorValue]
                                FROM [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStateHistory]
                                INNER JOIN
                                [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
                                ON [MachineState]=COLORS.StatusCode
                                WHERE 
                                [EndTime] IS NULL 
                                AND ([Language]='ru-RU' OR [Language]='en-US')";

            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        colorBlue = Convert.ToByte(reader.GetInt64(0) >> 16);
                        colorGreen = Convert.ToByte((reader.GetInt64(0) >> 8) & 255);
                        colorRed = Convert.ToByte((reader.GetInt64(0) & 255));


                    }
                }
            }


            return Color.FromArgb(colorRed,colorGreen,colorBlue);
        }
        public DataGridRow[] GetTableStatistic()
        {
            DataGridRow[] return_value;
            return_value = new DataGridRow[1];
            

            String SQLQuery = @"DECLARE @TB1 table(MachineState int, ColorValue int, StartTime datetime, EndTime Datetime);

INSERT INTO @TB1 
SELECT DISTINCT
                                [MachineState]
                                ,COLORS.[ColorValue] 
                                ,[StartTime]
                                ,[EndTime]
                              
                                FROM [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStateHistory]
                                INNER JOIN
                                [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
                                ON [MachineState]=COLORS.StatusCode 
                                WHERE 
                                [StartTime]>=CONVERT(DATETIME,'2015-04-24 08:00:00',120) AND [StartTime]<CONVERT(DATETIME,'2015-04-24 20:00:00',120)
                                OR [EndTime]>=CONVERT(DATETIME,'2015-04-24 08:00:00',120) AND [StartTime]<CONVERT(DATETIME,'2015-04-24 20:00:00',120)
                                OR [StartTime]<CONVERT(DATETIME,'2015-04-24 08:00:00',120) AND CONVERT(DATETIME,'2015-04-24 08:00:00',120)<CONVERT(DATETIME,'2015-04-24 19:39:00',120) AND [EndTime] IS NULL
                                ORDER BY [StartTime] asc
                                
DECLARE @TB2 table(MachineState int, DateDifference int);

INSERT INTO @TB2  
SELECT 
					[MachineState]
					 ,SUM(DATEDIFF(MINUTE,[StartTime],ISNULL([EndTime],CONVERT(DATETIME,'2015-04-24 19:39:00',120)))) AS DateDifference
					FROM @TB1
					GROUP BY  [MachineState]
					
SELECT DISTINCT
		[MachineState]
		,COLORS.ColorValue
		,[DateDifference]
		FROM @TB2 
		LEFT JOIN
                                [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
                                ON [MachineState]=COLORS.StatusCode                        

";

            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                        }
                    }
                }
            }

            return return_value;
        }

        #endregion


    }   

}
