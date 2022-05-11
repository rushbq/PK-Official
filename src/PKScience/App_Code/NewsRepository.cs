using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using NewsData.Models;
using PKLib_Method.Methods;

namespace NewsData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        onIndex = 2
    }

    public class NewsRepository
    {
        public string ErrMsg;


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="type">查詢類別</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<News> GetNews(Dictionary<int, string> search, string lang)
        {
            //----- 宣告 -----
            List<News> DataList = new List<News>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, lang))
            {
                if (DT == null)
                {
                    return DataList.AsQueryable();
                }

                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var Item = new News
                    {
                        ID = item.Field<int>("News_ID"),
                        Title = item.Field<string>("News_Title"),
                        Desc = item.Field<string>("News_Desc"),
                        ListPic = "{0}{1}News/{2}/{3}".FormatThis(
                            fn_Param.FileUrl,
                            fn_Param.FileFolder,
                            item.Field<int>("Group_ID"),
                            item.Field<string>("News_Pic")),
                        StartTime = item.Field<DateTime?>("StartTime").ToString().ToDateString("yyyy/MM/dd"),
                        EndTime = item.Field<DateTime?>("EndTime").ToString().ToDateString("yyyy/MM/dd")
                    };

                    //將項目加入至集合
                    DataList.Add(Item);

                }

            }

            //回傳集合
            return DataList.AsQueryable();

        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<News> GetOne(string queryID, string lang)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)mySearch.DataID, queryID);

            return GetNews(search, lang);

        }


        /// <summary>
        /// 取得關聯資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <returns></returns>
        public IQueryable<NewsDetail> GetDetail(string queryID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            List<NewsDetail> DataList = new List<NewsDetail>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT GP.Group_ID, Base.News_Title, Base.News_Desc, Base.News_Pic");
                sql.AppendLine("  , myData.News_ID, myData.Block_ID, myData.Block_Desc, myData.Block_Pic");
                sql.AppendLine(" FROM News_Group GP WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN News Base WITH(NOLOCK) ON GP.Group_ID = Base.Group_ID");
                sql.AppendLine("  INNER JOIN News_Block myData WITH(NOLOCK) ON Base.News_ID = myData.News_ID");
                sql.AppendLine(" WHERE (Base.News_ID = @DataID)");
                sql.AppendLine(" ORDER BY myData.Sort ASC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", queryID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT == null)
                    {
                        return DataList.AsQueryable();
                    }

                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var Item = new NewsDetail
                        {
                            ID = item.Field<int>("Block_ID"),
                            TopTitle = item.Field<string>("News_Title"),
                            TopDesc = item.Field<string>("News_Desc"),
                            ListPic = "{0}{1}News/{2}/{3}".FormatThis(
                                fn_Param.FileUrl,
                                fn_Param.FileFolder,
                                item.Field<int>("Group_ID"),
                                item.Field<string>("News_Pic")),
                            Desc = item.Field<string>("Block_Desc"),
                            Pic = string.IsNullOrEmpty(item.Field<string>("Block_Pic")) ? "" :
                                "{0}{1}News/{2}/{3}".FormatThis(
                                    fn_Param.FileUrl,
                                    fn_Param.FileFolder,
                                    item.Field<int>("Group_ID"),
                                    item.Field<string>("Block_Pic"))
                        };

                        //將項目加入至集合
                        DataList.Add(Item);

                    }
                }
            }


            //回傳集合
            return DataList.AsQueryable();
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
                sql.AppendLine(" SELECT GP.Group_ID, GP.StartTime, GP.EndTime, myData.News_ID, myData.News_Title");
                sql.AppendLine("    , myData.News_Desc, myData.News_Pic");
                sql.AppendLine(" FROM News_Group GP WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN News myData WITH(NOLOCK) ON GP.Group_ID = myData.Group_ID");
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
                                    sql.Append(" AND (myData.News_ID = @DataID)");
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
