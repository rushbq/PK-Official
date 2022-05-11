using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using ProductData.Models;
using PKLib_Method.Methods;

namespace ProductData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        ClassID = 3,
        IsHot = 4
    }


    public class ProductRepository
    {
        public string ErrMsg;


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <param name="lang">語系</param>
        /// <param name="topCnt">只顯示前n筆</param>
        /// <returns></returns>
        public IQueryable<Product> GetProducts(Dictionary<int, string> search, string lang, decimal topCnt)
        {
            //----- 宣告 -----
            List<Product> Products = new List<Product>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, lang, topCnt))
            {
                if (DT == null)
                {
                    return Products.AsQueryable();
                }

                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var prod = new Product
                    {
                        ID = item.Field<int>("Prod_ID"),
                        ShopUrl = item.Field<string>("ShopUrl"),
                        IsNew = item.Field<string>("IsNew"),
                        ModelNo = item.Field<string>("ModelNo"),
                        ModelName = item.Field<string>("ModelName"),
                        ListPic = "{0}ProductPic/{1}/1/{2}".FormatThis(
                            fn_Param.FileUrl
                            , item.Field<string>("ModelNo")
                            , item.Field<string>("ListPic")),
                        ListDesc = item.Field<string>("ListDesc"),
                        ShareTitle = item.Field<string>("ShareTitle"),
                        ShareDesc = item.Field<string>("ShareDesc"),
                        PicGroup = item.Field<string>("PicGroup"),
                        TypeID = item.Field<int>("Class_ID"),
                        TypeName = item.Field<string>("TypeName"),
                        Url1 = item.Field<string>("Url1"),
                        Url2 = item.Field<string>("Url2"),
                        FullDesc = GetPKWeb_Video(item.Field<string>("ModelNo"), lang) + item.Field<string>("FullDesc"),
                        ShortDesc = item.Field<string>("ShortDesc"),
                        SeoDesc = item.Field<string>("SeoDesc")
                    };

                    //將項目加入至集合
                    Products.Add(prod);

                }

            }

            //回傳集合
            return Products.AsQueryable();

        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<Product> GetOne(string queryID, string lang)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)mySearch.DataID, queryID);

            return GetProducts(search, lang, 1);

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
                sql.AppendLine("  Base.Prod_ID, Base.ShopUrl, Base.Class_ID, ClsType.Class_Name AS TypeName");
                sql.AppendLine("  , (CASE WHEN DATEDIFF(DAY, Base.StartTime, GETDATE()) <= 60 THEN 'Y' ELSE 'N' END) AS IsNew");
                sql.AppendLine("  , RTRIM(Prod.Model_No) ModelNo, ISNULL(UPPER(Prod.Model_Name_{0}), '') AS ModelName".FormatThis(lang.ToUpper().Replace("-", "_")));
                sql.AppendLine("  , Info.Info2 AS FullDesc, ISNULL(Info.Info5, '') AS ShortDesc, Info.Info6 AS ListDesc");
                sql.AppendLine("  , Info.Info7 AS ShareTitle, Info.Info8 AS ShareDesc, Info.Info9 AS SeoDesc");
                sql.AppendLine("  , Photo.Pic11 AS ListPic");
                sql.AppendLine("  , ISNULL(ScInfo.Url_Manual, '') AS Url1, ISNULL(ScInfo.Url_Video, '') AS Url2");
                //產品主圖
                sql.AppendLine("  ,(");
                sql.AppendLine("   ISNULL(Pic02,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'')+ '|' + ISNULL(Pic05,'') ");
                sql.AppendLine("   + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')");
                sql.AppendLine("  ) AS PicGroup");

                sql.AppendLine(" FROM Prod Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Prod_Class ClsType WITH(NOLOCK) ON Base.Class_ID = ClsType.Class_ID AND UPPER(ClsType.LangCode) = UPPER(@Lang)");
                sql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Base.Model_No = Prod.Model_No");
                sql.AppendLine("  LEFT JOIN [ProductCenter].dbo.Prod_Info Info WITH(NOLOCK) ON Prod.Model_No = Info.Model_No AND UPPER(Info.Lang) = UPPER(@Lang)");
                sql.AppendLine("  LEFT JOIN [ProductCenter].dbo.ProdPic_Photo Photo WITH(NOLOCK) ON Photo.Model_No = Prod.Model_No");
                sql.AppendLine("  LEFT JOIN Prod_Info ScInfo WITH(NOLOCK) ON Base.Prod_ID = ScInfo.Prod_ID AND UPPER(ScInfo.LangCode) = UPPER(@Lang)");
                sql.AppendLine(" WHERE (Base.Display = 'Y') AND (GETDATE() >= Base.StartTime) AND (GETDATE() <= Base.EndTime)");


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

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (RTRIM(Prod.Model_No) LIKE '%' + @Keyword + '%')");
                                    sql.Append("    OR (UPPER(Prod.Model_Name_zh_TW) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Prod.Model_Name_zh_CN) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Prod.Model_Name_en_US) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");
                                }

                                break;

                            case (int)mySearch.ClassID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Class_ID = @ClassID)");
                                }

                                break;


                            case (int)mySearch.IsHot:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.IsHot = 'Y')");
                                }

                                break;
                        }
                    }
                }


                //指定顯示前n筆, 依開始時間排序
                if (topCnt > 0)
                {
                    sql.AppendLine(" ORDER BY Base.StartTime DESC");
                }
                else
                {
                    sql.AppendLine(" ORDER BY Base.Sort ASC, Base.StartTime DESC");
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


        /// <summary>
        /// 取得產品關鍵字
        /// </summary>
        /// <param name="modelNo"></param>
        /// <returns></returns>
        public IQueryable<Tags> GetTags(string modelNo)
        {
            //----- 宣告 -----
            List<Tags> dataList = new List<Tags>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Tags.Tag_ID, Tags.Tag_Name, Rel.Model_No");
                sql.AppendLine(" FROM Prod_Tags Tags WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN Prod_Rel_Tags Rel WITH(NOLOCK) ON Tags.Tag_ID = Rel.Tag_ID");
                sql.AppendLine(" WHERE (UPPER(Rel.Model_No) = UPPER(@ModelNo))");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Tags
                        {
                            ModelNo = item.Field<string>("Model_No"),
                            TagID = item.Field<int>("Tag_ID"),
                            TagName = item.Field<string>("Tag_Name")
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        /// <summary>
        /// 取得說明書下載
        /// </summary>
        /// <param name="modelNo"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public IQueryable<Manual> GetManuals(string modelNo, string lang)
        {
            //----- 宣告 -----
            List<Manual> dataList = new List<Manual>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("    Base.FileName AS DwFile, Base.DisplayName AS DwFileName");
                sql.AppendLine("    , Lang.Class_Name AS LangName, Cls.Class_ID");
                sql.AppendLine("  FROM PKEF.dbo.File_List Base");
                sql.AppendLine("    INNER JOIN PKEF.dbo.File_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = LOWER(@LangCode) AND Cls.Web_Display = 'Y'");
                sql.AppendLine("    INNER JOIN PKEF.dbo.File_LangType Lang ON Base.LangType_ID = Lang.Class_ID AND Lang.Display = 'Y'");
                sql.AppendLine("    INNER JOIN PKEF.dbo.File_Type FType ON Base.FileType_ID = FType.Class_ID AND LOWER(FType.LangCode) = LOWER(@LangCode) AND FType.Display = 'Y' AND FType.Up_ClassID = Cls.Class_ID");
                sql.AppendLine("    INNER JOIN PKEF.dbo.File_Target FTarget ON Base.Target = FTarget.Class_ID AND FTarget.Display = 'Y'");
                sql.AppendLine("    INNER JOIN PKEF.dbo.File_Rel_ModelNo Rel ON Base.File_ID = Rel.File_ID");
                //--類別:說明書/對象:所有人
                sql.AppendLine("  WHERE (Cls.Class_ID = 1) AND (FType.Class_ID = 1) AND (Base.Target IN (1)) AND (Rel.Model_No = @ModelNo)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);
                cmd.Parameters.AddWithValue("LangCode", lang);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Manual
                        {
                            DwFile = item.Field<string>("DwFile"),
                            DwFileName = item.Field<string>("DwFileName"),
                            LangName = item.Field<string>("LangName"),
                            ClassID = Convert.ToInt16(item.Field<Int16>("Class_ID"))
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        /// <summary>
        /// 取得官網影片連結字串
        /// </summary>
        /// <param name="modelNo">品號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public string GetPKWeb_Video(string modelNo, string lang)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.PV_Uri AS Url");
                sql.AppendLine(" FROM PV_Group GP WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN PV_Group_Rel_ModelNo Rel WITH(NOLOCK) ON GP.Group_ID = Rel.Group_ID");
                sql.AppendLine("    INNER JOIN PV Base WITH(NOLOCK) ON GP.Group_ID = Base.Group_ID");
                sql.AppendLine(" WHERE (Base.LangCode = @lang) AND (Rel.Model_No = @modelNo)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("modelNo", modelNo);
                cmd.Parameters.AddWithValue("lang", lang);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return "<div class=\"video-container\"><iframe src=\"{0}\" frameborder=\"0\" allowfullscreen></iframe></div>".FormatThis(
                                DT.Rows[0]["Url"].ToString()
                            );
                    }
                }

            }
        }
    }
}
