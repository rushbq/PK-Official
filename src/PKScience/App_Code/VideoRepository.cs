using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Method.Methods;
using VideoData.Models;

namespace VideoData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        IsIndex = 2,
        ClassID = 3
    }


    public class VideoRepository
    {
        public string ErrMsg;


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <param name="lang">語系</param>
        /// <param name="topCnt">只顯示前n筆</param>
        /// <returns></returns>
        public IQueryable<Video> GetVideos(Dictionary<int, string> search, string lang, decimal topCnt)
        {
            //----- 宣告 -----
            List<Video> Videos = new List<Video>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, lang, topCnt))
            {
                if (DT == null)
                {
                    return Videos.AsQueryable();
                }

                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var prod = new Video
                    {
                        ID = item.Field<int>("Data_ID"),
                        Url = item.Field<string>("Data_Url"),
                        ModelNo = item.Field<string>("ModelNo"),
                        ModelName = item.Field<string>("ModelName"),
                        ListPic = "{0}{1}Video/{2}/{3}".FormatThis(
                            fn_Param.FileUrl
                            , fn_Param.FileFolder
                            , item.Field<int>("Group_ID")
                            , item.Field<string>("ListPic"))
                    };

                    //將項目加入至集合
                    Videos.Add(prod);

                }

            }

            //回傳集合
            return Videos.AsQueryable();

        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<Video> GetOne(string queryID, string lang)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)mySearch.DataID, queryID);

            return GetVideos(search, lang, 1);

        }


        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢</param>
        /// <param name="lang">語系</param>
        /// <param name="topCnt">只顯示前n筆</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search, string lang, decimal topCnt)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT {0} ".FormatThis(topCnt > 0 ? "TOP " + topCnt : ""));
                sql.AppendLine("  Base.Data_ID, Base.Data_Url, GP.Class_ID, GP.Group_ID");
                sql.AppendLine("  , Base.Data_ListPic AS ListPic");
                sql.AppendLine("  , RTRIM(Prod.Model_No) ModelNo, ISNULL(UPPER(Prod.Model_Name_{0}), '') AS ModelName".FormatThis(lang.ToUpper().Replace("-", "_")));
                sql.AppendLine(" FROM Movies_Group GP WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Movies Base WITH(NOLOCK) ON Base.Group_ID = GP.Group_ID AND UPPER(Base.LangCode) = UPPER(@Lang)");
                sql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON GP.Model_No = Prod.Model_No");
                sql.AppendLine(" WHERE (GP.Display = 'Y') AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");


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
                                    sql.Append(" AND (RTRIM(Prod.Model_No) = @DataID)");
                                }

                                break;

                            case (int)mySearch.IsIndex:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (GETDATE() >= GP.ActStartDate) AND (GETDATE() <= GP.ActEndDate)");
                                }

                                break;

                            case (int)mySearch.ClassID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (GP.Class_ID = @ClassID)");
                                }

                                break;

                        }
                    }
                }


                //指定顯示前n筆, 依開始時間排序
                if (topCnt > 0)
                {
                    sql.AppendLine(" ORDER BY GP.ActStartDate ASC, GP.Sort ASC");
                }
                else
                {
                    sql.AppendLine(" ORDER BY GP.Sort ASC, GP.StartTime DESC");
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Lang", lang);

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

                            case (int)mySearch.ClassID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    cmd.Parameters.AddWithValue("ClassID", item.Value);
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
