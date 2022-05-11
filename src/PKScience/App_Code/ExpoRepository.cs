using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using ExpoData.Models;
using PKLib_Method.Methods;

namespace ExpoData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        onIndex = 2
    }

    public class ExpoRepository
    {
        public string ErrMsg;


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="type">查詢類別</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<Expo> GetExpos(Dictionary<int, string> search, string lang)
        {
            //----- 宣告 -----
            List<Expo> Expos = new List<Expo>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, lang))
            {
                if (DT == null)
                {
                    return Expos.AsQueryable();
                }

                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var Expo = new Expo
                    {
                        ID = item.Field<int>("Expo_ID"),
                        Title = item.Field<string>("Expo_Title"),
                        SubTitle = item.Field<string>("Expo_SubTitle"),
                        Desc = item.Field<string>("Expo_Desc"),
                        Url = string.IsNullOrEmpty(item.Field<string>("Expo_Url")) ? "#!" : item.Field<string>("Expo_Url"),
                        ListPic = "{0}{1}Activity/{2}/{3}".FormatThis(
                            fn_Param.FileUrl,
                            fn_Param.FileFolder,
                            item.Field<int>("Group_ID"),
                            item.Field<string>("Expo_ListPic")),
                        BigPic = "{0}{1}Activity/{2}/{3}".FormatThis(
                            fn_Param.FileUrl,
                            fn_Param.FileFolder,
                            item.Field<int>("Group_ID"),
                            item.Field<string>("Expo_BigPic")),
                        StartTime = item.Field<DateTime?>("ActStartDate").ToString().ToDateString("yyyy/MM/dd"),
                        EndTime = item.Field<DateTime?>("ActEndDate").ToString().ToDateString("yyyy/MM/dd")
                    };

                    //將項目加入至集合
                    Expos.Add(Expo);

                }

            }

            //回傳集合
            return Expos.AsQueryable();

        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<Expo> GetOne(string queryID, string lang)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)mySearch.DataID, queryID);

            return GetExpos(search, lang);

        }


        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search, string lang)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT GP.Group_ID, GP.ActStartDate, GP.ActEndDate, myData.Expo_ID, myData.Expo_Title, myData.Expo_SubTitle");
                sql.AppendLine("    , myData.Expo_Desc, myData.Expo_Url, myData.Expo_ListPic, myData.Expo_BigPic");
                sql.AppendLine(" FROM Expo_Group GP WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN Expo myData WITH(NOLOCK) ON GP.Group_ID = myData.Group_ID");
                sql.AppendLine(" WHERE (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                sql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");


                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (myData.Expo_ID = @DataID)");
                                }

                                break;


                            case (int)mySearch.onIndex:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (GP.onIndex = 'Y')");
                                }

                                break;
                        }
                    }
                }


                sql.AppendLine(" ORDER BY GP.Sort ASC, GP.EndTime DESC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("LangCode", lang);

                //----- SQL Filter -----
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;
                        }
                    }
                }


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }
    }
}
