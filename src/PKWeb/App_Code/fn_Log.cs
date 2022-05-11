using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;

namespace LogRecord
{
    /// <summary>
    /// Log記錄
    /// </summary>
    public class fn_Log
    {

        /// <summary>
        /// 寫入Log
        /// </summary>
        /// <param name="currUser">目前user</param>
        /// <param name="logAction">在幹嘛</param>
        /// <param name="logEvtCode">事件代號</param>
        /// <param name="logWhere">在哪兒</param>
        public static void writeLog(string currUser, string logAction, string logEvtCode, string logWhere)
        {
            if (string.IsNullOrEmpty(currUser))
            {
                currUser = "路人";
            }
            writeLog(currUser, logAction, logEvtCode, logWhere
                , string.IsNullOrEmpty(fn_Extensions.GetIP()) ? "local" : fn_Extensions.GetIP()
                , "");

        }
        public static void writeLog(string currUser, string logAction, string logEvtCode, string logWhere, string IP, string TraceID)
        {
            //Log簡述
            string logTitle = string.Format("{0}", logAction);

            //Log詳述(在幹嘛 (什麼人), 時間, 地點)
            string logDesc = string.Format("{0} ({1}), 於 {2}, 在 {3}"
                    , logAction
                    , currUser
                    , DateTime.Now.ToString().ToDateString("yyyy-MM-dd HH:mm:ss")
                    , logWhere
                );

            //[寫入Log]
            string ErrMsg;
            if (false == Log_Event("前台", logEvtCode, logTitle, logDesc, currUser, IP, TraceID, out ErrMsg))
            {
                throw new Exception("fail:" + ErrMsg);
            }
        }

        /// <summary>
        /// 寫入Log
        /// </summary>
        /// <param name="Platform">來源(前台/後台)</param>
        /// <param name="EventID">事件代號</param>
        /// <param name="EventDesc">事件簡述(50字內)</param>
        /// <param name="EventDetail">事件詳述</param>
        /// <param name="CreateWho">建立者</param>
        /// <param name="IP">IP</param>
        /// <param name="TraceID">追蹤編號</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// EventID 參照 Log_EventCode
        /// </remarks>
        public static bool Log_Event(string Platform, string EventID, string EventDesc, string EventDetail, string CreateWho
            , string IP, string TraceID, out string ErrMsg)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[SQL] - 寫入Log
                    SBSql.AppendLine(" INSERT INTO Log_Event( ");
                    SBSql.AppendLine("  LogID, Who, FromIP, Platform, EventTime, EventID, EventDesc, EventDetail, TraceID ");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  NEWID(), @Who, @FromIP, @Platform, GETDATE(), @EventID, @EventDesc, @EventDetail, @TraceID ");
                    SBSql.AppendLine(" )");
                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Who", CreateWho.Left(50));
                    cmd.Parameters.AddWithValue("FromIP", IP);
                    cmd.Parameters.AddWithValue("Platform", Platform);
                    cmd.Parameters.AddWithValue("EventID", EventID);
                    cmd.Parameters.AddWithValue("EventDesc", EventDesc.Left(100));
                    cmd.Parameters.AddWithValue("EventDetail", EventDetail);
                    cmd.Parameters.AddWithValue("TraceID", TraceID);

                    //[執行SQL]
                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }

        }

    }
}
