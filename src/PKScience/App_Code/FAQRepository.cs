using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using FAQData.Models;
using PKLib_Method.Methods;

namespace FAQData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        ClassID = 3
    }

    public class FAQRepository
    {
        public string ErrMsg;


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="type">查詢類別</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<FAQ> GetFAQ(Dictionary<int, string> search, string lang)
        {
            //----- 宣告 -----
            List<FAQ> DataList = new List<FAQ>();

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
                    var Item = new FAQ
                    {
                        ID = item.Field<int>("FAQ_ID"),
                        Title = item.Field<string>("FAQ_Title"),
                        ClassID = item.Field<int>("Class_ID"),
                        ClassName = item.Field<string>("Class_Name"),
                        ClassSort = item.Field<Int16>("Sort")
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
        public IQueryable<FAQ> GetOne(string queryID, string lang)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)mySearch.DataID, queryID);

            return GetFAQ(search, lang);

        }


        /// <summary>
        /// 取得關聯資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<FAQDetail> GetDetail(string queryID, string lang)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            List<FAQDetail> DataList = new List<FAQDetail>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT GP.Group_ID, Base.FAQ_Title, Cls.Class_ID, Cls.Class_Name");
                sql.AppendLine("  , myData.FAQ_ID, myData.Block_ID, myData.Block_Desc, myData.Block_Pic");
                sql.AppendLine(" FROM FAQ_Group GP WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN FAQ Base WITH(NOLOCK) ON GP.Group_ID = Base.Group_ID");
                sql.AppendLine("  INNER JOIN FAQ_Block myData WITH(NOLOCK) ON Base.FAQ_ID = myData.FAQ_ID");
                sql.AppendLine("  INNER JOIN FAQ_Class Cls WITH(NOLOCK) ON GP.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = LOWER(@LangCode)");
                sql.AppendLine(" WHERE (Base.FAQ_ID = @DataID)");
                sql.AppendLine(" ORDER BY myData.Sort ASC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", queryID);
                cmd.Parameters.AddWithValue("LangCode", lang);


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
                        var Item = new FAQDetail
                        {
                            GroupID = item.Field<int>("Group_ID"),
                            ID = item.Field<int>("Block_ID"),
                            TopTitle = item.Field<string>("FAQ_Title"),
                            TopClass = item.Field<string>("Class_Name"),
                            TopClassID = item.Field<int>("Class_ID"),
                            Desc = item.Field<string>("Block_Desc"),
                            Pic = string.IsNullOrEmpty(item.Field<string>("Block_Pic")) ? "" :
                                "{0}{1}FAQ/{2}/{3}".FormatThis(
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
        /// 取得關聯產品
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<FAQProd> GetProds(string queryID, string lang)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            List<FAQProd> DataList = new List<FAQProd>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(Prod.Model_No) ModelNo, ISNULL(UPPER(Prod.Model_Name_{0}), '') AS ModelName".FormatThis(lang.ToUpper().Replace("-", "_")));
                sql.AppendLine(" FROM Prod Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN FAQ_Rel_ModelNo Rel WITH(NOLOCK) ON Rel.Model_No = Base.Model_No");
                sql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Base.Model_No = Prod.Model_No");
                sql.AppendLine(" WHERE (Base.Display = 'Y') AND (GETDATE() >= Base.StartTime) AND (GETDATE() <= Base.EndTime)");
                sql.AppendLine("  AND (Rel.Group_ID = @DataID)");

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
                        var Item = new FAQProd
                        {
                            ModelNo = item.Field<string>("ModelNo"),
                            ModelName = item.Field<string>("ModelName")
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
                sql.AppendLine(" SELECT GP.Class_ID, Cls.Class_Name, myData.FAQ_ID, myData.FAQ_Title, Cls.Sort");
                sql.AppendLine(" FROM FAQ_Group GP WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN FAQ myData WITH(NOLOCK) ON GP.Group_ID = myData.Group_ID");
                sql.AppendLine("    INNER JOIN FAQ_Class Cls WITH(NOLOCK) ON GP.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = LOWER(myData.LangCode)");
                sql.AppendLine(" WHERE (GP.Display = 'Y') AND (Cls.Display = 'Y')");
                sql.AppendLine("  AND (LOWER(myData.LangCode) = LOWER(@LangCode))");


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
                                    sql.Append(" AND (myData.FAQ_ID = @DataID)");
                                }

                                break;


                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (Cls.Class_Name LIKE '%' + @Keyword + '%')");
                                    sql.Append("    OR (UPPER(myData.FAQ_Title) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (GP.Group_ID IN (");
                                    sql.Append("      SELECT Rel.Group_ID");
                                    sql.Append("      FROM FAQ_Rel_ModelNo Rel");
                                    sql.Append("      WHERE (Rel.Model_No LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    ))");
                                    sql.Append(" )");
                                }

                                break;


                            case (int)mySearch.ClassID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Cls.Class_ID = @ClassID)");
                                }

                                break;
                        }
                    }
                }


                sql.AppendLine(" ORDER BY GP.Sort ASC, GP.Create_Time DESC");


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


                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
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
