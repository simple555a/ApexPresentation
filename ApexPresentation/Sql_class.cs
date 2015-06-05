using System;
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
        public  Sql_class(String in_ConnectionString)
        {
            this.Initialized = false;

            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + in_ConnectionString +  ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True");
                con.Open();
                //if ok - fill connection string field
                this.ConnectionString = "Data Source=" + in_ConnectionString +  ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True";

                this.Initialized = true;
            }
            catch
            {
                MessageBox.Show("Bad SQL connection. Review connection string");
                this.Initialized = false;
            }
        }

        #endregion
        
        #region 2. Properties

        private String ConnectionString;
        public bool Initialized;

        #endregion

        #region 3. Metods

            #region public void InitializeSQL()
            private void InitializeSQL()
            {
                try
                {
                    if (File.Exists("settings.xml"))
                    {
                        XmlSerializer XmlSerializer1 = new XmlSerializer(typeof(Settings));
                        TextReader reader1 = new StreamReader("settings.xml");
                        Settings Settings1 = (Settings)XmlSerializer1.Deserialize(reader1);
                        reader1.Dispose();

                        SqlConnection con = new SqlConnection("Data Source=" + Settings1.SQLConnectionString +  ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True");
                        con.Open();
                        //if ok - fill connection string field
                        this.ConnectionString = "Data Source=" + Settings1.SQLConnectionString +  ";Initial Catalog=SFI_local_PC_SQL;Integrated Security=True";

                        this.Initialized = true;
                    }
                    else
                    {
                        MessageBox.Show("SQL settings is empty. See Settings - > Connection...");
                        this.Initialized = false;
                    }


                }
                catch
                {
                    MessageBox.Show("Bad SQL connection. Review connection string");
                    this.Initialized = false;
                }
            }
            #endregion
            #region public String GetOperatorName()
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
            #endregion
            #region public Section[] GetTimeLineData(DateTime in_StartTime, DateTime in_EndTime, DateTime in_CURR)
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
            #endregion
            #region public string GetCurrentStatus()
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
            #endregion
            #region public Color GetCurrentStatusColor()
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
            #endregion
            #region public List<DataGridRow> GetTableStatistic(DateTime in_StartTime, DateTime in_EndTime, DateTime in_CURR)
            public List<DataGridRow> GetTableStatistic(DateTime in_StartTime, DateTime in_EndTime, DateTime in_CURR)
            {
                

                List<DataGridRow> return_value = new List<DataGridRow>();
                if (!this.Initialized) return return_value;

                String SQLQuery = @"DECLARE @gStartTime DATETIME;
DECLARE @gEndTime DATETIME;
DECLARE @gCURR DATETIME;

SET @gStartTime=CONVERT(DATETIME,'" + in_StartTime.ToString("yyyy-MM-dd HH:mm:ss") + @"',120)
SET @gEndTime=CONVERT(DATETIME,'" + in_EndTime.ToString("yyyy-MM-dd HH:mm:ss") + @"',120)
SET @gCURR=CONVERT(DATETIME,'" + in_CURR.ToString("yyyy-MM-dd HH:mm:ss") + @"',120)

DECLARE @TB1 table(MachineState int, ColorValue int, StartTime datetime, EndTime Datetime);
INSERT INTO @TB1 
SELECT DISTINCT
    [MachineState]
    ,COLORS.[ColorValue] 
    ,[StartTime]
    ,[EndTime]
FROM 
    [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStateHistory]
INNER JOIN
    [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates] AS COLORS
ON 
    [MachineState]=COLORS.StatusCode 
WHERE 
    [StartTime]>=@gStartTime AND [StartTime]<@gEndTime
    OR [EndTime]>=@gStartTime AND [StartTime]<@gEndTime
    OR [StartTime]<@gStartTime AND @gStartTime<@gCURR AND ISNULL([EndTime],@gCURR)>@gStartTime
ORDER BY [StartTime] asc


DECLARE @TB2 table(MachineState int, DateDifference int);
INSERT INTO @TB2  
SELECT 
	[MachineState]
	,SUM(DATEDIFF(
                SECOND
                ,CAST(CASE
                            WHEN [StartTime]<@gStartTime
                            THEN @gStartTime
                            ELSE [StartTime]
                        END as DATETIME)
                ,CAST(CASE
                            WHEN ISNULL([EndTime],@gCURR)>@gEndTime
                            THEN @gEndTime
                            ELSE ISNULL([EndTime],@gCURR)
                        END as DATETIME)
                )) AS DateDifference 
FROM @TB1
GROUP BY  [MachineState]


DECLARE @TB3 table(MachineState int, StartTime datetime);
INSERT INTO @TB3
SELECT 
    [MachineState]
    ,[StartTime]
FROM @TB1
 

DECLARE @TB4 table(MachineState int, StartTime datetime);
INSERT INTO @TB4
SELECT 
    [MachineState]
    ,min([StartTime]) 
FROM @TB3
GROUP BY [MachineState]

DECLARE @excessed_times_table table (MachineState int, ApprovedTime int)
INSERT INTO 
	@excessed_times_table
VALUES      
	(213,15);

INSERT INTO 
	@excessed_times_table
VALUES   
	(511,30);

INSERT INTO 
	@excessed_times_table
VALUES   
	(531,30);

INSERT INTO 
	@excessed_times_table
VALUES    
	(540,10);

INSERT INTO 
	@excessed_times_table
VALUES     
	(711,5);

INSERT INTO 
	@excessed_times_table
VALUES     
	(721,10);

INSERT INTO 
	@excessed_times_table
VALUES    
	(823,15);   
									 


DECLARE @correct_tb1 table (MachineState int, StartTime datetime, EndTime Datetime)
INSERT INTO @correct_tb1
SELECT 
	[@TB1].[MachineState]
	,CAST(
		CASE
			WHEN [StartTime]<@gStartTime 
			THEN @gStartTime 
			ELSE [StartTime]
		END AS DATETIME
		)
	,CAST(CASE
                            WHEN ISNULL([EndTime],@gCURR)>@gEndTime
                            THEN @gEndTime
                            ELSE ISNULL([EndTime],@gCURR)
                        END as DATETIME
            )											
FROM @TB1
									    
DECLARE @excessed_times table (MachineState int, ExceededTimeSumValue int)
INSERT INTO @excessed_times            
select 
	[@correct_tb1].[MachineState]
	,SUM(CAST(
		CASE
			WHEN (DATEDIFF(SECOND,[StartTime],[EndTime])-ISNULL([@excessed_times_table].[ApprovedTime],DATEDIFF(SECOND,[StartTime],[EndTime]))*60)>0
			THEN (DATEDIFF(SECOND,[StartTime],[EndTime])-ISNULL([@excessed_times_table].[ApprovedTime],DATEDIFF(SECOND,[StartTime],[EndTime]))*60)
			ELSE 0
		END AS INT))
from @correct_tb1
LEFT JOIN 
	@excessed_times_table
	ON [@correct_tb1].[MachineState]=[@excessed_times_table].[MachineState]
group by  [@correct_tb1].[MachineState]

DECLARE @status_count table(MachineState int, _count int)
INSERT INTO @status_count            
select 
	[MachineState]
	,COUNT([MachineState])
from @TB1
GROUP BY [MachineState]

select distinct
    [@TB4].MachineState
    ,[SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates].ColorValue
    ,[SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates].StatusDescription
    ,[@TB2].DateDifference
    ,[StartTime]
    ,[@status_count].[_count]
    ,[@excessed_times].[ExceededTimeSumValue]
FROM @TB4
INNER JOIN
    @TB2
    ON [@TB4].[MachineState]=[@TB2].MachineState
INNER JOIN
	[SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates]
	ON [@TB4].[MachineState]=[SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates].StatusCode
INNER JOIN
	@excessed_times
	ON [@TB4].[MachineState]=[@excessed_times].[MachineState]
INNER JOIN
	@status_count
	ON [@TB4].[MachineState]=[@status_count].[MachineState]
WHERE [SFI_local_PC_SQL].[dbo].[tbl_slc_MachineStates].[Language]='en-US'
ORDER BY [StartTime]";

                using (SqlConnection con = new SqlConnection(this.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                using (DataGridRow a1 = new DataGridRow())
                                {
                                    a1.MachineCode = reader.GetInt32(0).ToString();
                                    a1.Color = Color.FromArgb(Convert.ToByte((reader.GetInt64(1) & 255)), Convert.ToByte((reader.GetInt64(1) >> 8) & 255), Convert.ToByte(reader.GetInt64(1) >> 16));
                                    a1.Status = reader.GetString(2);
                                    a1.SummaryTime = reader.GetInt32(3).ToString();
                                    a1.Count = reader.GetInt32(5).ToString();
                                    a1.ExceededTime = reader.GetInt32(6).ToString();
                                    return_value.Add(a1);
                                }
                            }
                        }
                    }
                }

                return return_value;
            }
            #endregion

        #endregion


    }   

}
