using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using ClosedXML.Excel;
using CustomController;
using ExtensionMethods;

namespace ExtensionUI
{
    /// <summary>
    /// 自訂常用的UI
    /// 換網站時注意DB名前置詞
    /// </summary>
    public class fn_CustomUI
    {
        #region -- 產生選單 --

        /// <summary>
        /// 取得年份 (for DropDrownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="numBFnow">啟始年(增減值)</param>
        /// <param name="numAFnow">結束年(增減值)</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        /// <remarks>
        /// 起始年為 今年 +- numBFnow
        /// 迄止年為 今年 +- numAFnow
        /// </remarks>
        public static bool Get_Years(DropDownList setMenu, string inputValue, bool showRoot, int numBFnow, int numAFnow, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                if (numBFnow > numAFnow)
                {
                    numBFnow = numAFnow;
                }

                //計算起迄年份
                int bgYear = DateTime.Now.AddYears(numBFnow).Year;
                int edYear = DateTime.Now.AddYears(numAFnow).Year;
                for (int intY = bgYear; intY <= edYear; intY++)
                {
                    setMenu.Items.Add(new ListItem(intY.ToString(), intY.ToString()));
                }

                //判斷是否要顯示索引文字
                if (showRoot)
                {
                    setMenu.Items.Insert(0, new ListItem("-- Year --", ""));
                }

                //判斷是否有已選取的項目
                if (false == string.IsNullOrEmpty(inputValue))
                {
                    setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得月份 (for DropDrownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Get_Months(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //固定為12個月, 若有增加或減少請跟我說....
                for (int intM = 1; intM <= 12; intM++)
                {
                    setMenu.Items.Add(new ListItem(intM.ToString(), intM.ToString()));
                }

                //判斷是否要顯示索引文字
                if (showRoot)
                {
                    setMenu.Items.Insert(0, new ListItem("-- Month --", ""));
                }

                //判斷是否有已選取的項目
                if (false == string.IsNullOrEmpty(inputValue))
                {
                    setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得產品分類 (for DropDownList) - Tools
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="lang">語系</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        /// <remarks>
        /// DB = ProductCenter
        /// </remarks>
        public static bool Get_ProdClass(DropDownList setMenu, string inputValue, string lang, bool showRoot, string rootName, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT RTRIM(Cls.Class_ID) AS ID, Cls.Class_Name_{0} AS Label ".FormatThis(lang));
                    SBSql.AppendLine(" FROM Prod_Class Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (LEFT(RTRIM(Cls.Class_ID),1) = '2') AND (Cls.Display = 'Y') AND (Cls.Display_PKWeb = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Cls.Sort, Cls.Class_ID");
                    cmd.CommandText = SBSql.ToString();
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- {0} --".FormatThis(
                                string.IsNullOrEmpty(rootName) ? "ALL" : rootName
                                ), ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得產品分類名稱-Tools
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public static string Get_ProdClassName(string inputValue, string lang)
        {
            string ErrMsg;

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Cls.Class_Name_{0} AS Label ".FormatThis(lang));
                    SBSql.AppendLine(" FROM Prod_Class Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y') AND (Cls.Display_PKWeb = 'Y') AND (Cls.Class_ID = @Class_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Class_ID", inputValue);
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return "";
                        }
                        else
                        {
                            return DT.Rows[0]["Label"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }


        /// <summary>
        /// 取得產品分類 (for DropDownList) - Toy
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="lang">語系</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        /// <remarks>
        /// DB = ProductCenter
        /// </remarks>
        public static bool Get_ProdToyClass(DropDownList setMenu, string inputValue, string lang, bool showRoot, string rootName, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT RTRIM(Cls.Class_ID) AS ID, Cls.Class_Name_{0} AS Label ".FormatThis(lang));
                    SBSql.AppendLine(" FROM ProdToy_Class Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y') AND (Cls.Display_PKWeb = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Cls.Sort, Cls.Class_ID");
                    cmd.CommandText = SBSql.ToString();
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- {0} --".FormatThis(
                                string.IsNullOrEmpty(rootName) ? "ALL" : rootName
                                ), ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得產品分類名稱-Toy
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public static string Get_ProdToyClassName(string inputValue, string lang)
        {
            string ErrMsg;

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Cls.Class_Name_{0} AS Label ".FormatThis(lang));
                    SBSql.AppendLine(" FROM ProdToy_Class Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y') AND (Cls.Display_PKWeb = 'Y') AND (Cls.Class_ID = @Class_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Class_ID", inputValue);
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return "";
                        }
                        else
                        {
                            return DT.Rows[0]["Label"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 取得洲別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        /// <remarks>
        /// DB = PKWeb
        /// </remarks>
        public static bool Get_Region(DropDownList setMenu, string inputValue, string lang, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Sub.AreaCode AS ID, Sub.AreaName AS Label ");
                    SBSql.AppendLine(" FROM Geocode_AreaCode Base WITH (NOLOCK) ");
                    SBSql.AppendLine(" INNER JOIN Geocode_AreaName Sub WITH (NOLOCK) ON Base.AreaCode = Sub.AreaCode ");
                    SBSql.AppendLine(" WHERE (LOWER(Sub.LangCode) = LOWER(@Lang)) ");
                    SBSql.AppendLine(" ORDER BY Sub.AreaCode");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Lang", lang);
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- Region --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得城市/地區
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        /// <remarks>
        /// DB = PKWeb
        /// </remarks>
        public static bool Get_City(DropDownList setMenu, string inputValue, string code, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();            

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Region_Code AS ID, Region_Name_{0} AS Label".FormatThis(fn_Language.Param_Lang));
                    SBSql.AppendLine(" FROM Geocode_Region WITH (NOLOCK)");
                    SBSql.AppendLine(" WHERE (LOWER(Country_Code) = LOWER(@Country_Code))");
                    SBSql.AppendLine(" ORDER BY Sort, Country_Code");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Country_Code", code);
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            //hide
                            setMenu.Visible = false;
                        }
                        else
                        {
                            //新增選單項目
                            for (int row = 0; row < DT.Rows.Count; row++)
                            {
                                setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                             , DT.Rows[row]["ID"].ToString()));
                            }
                            //判斷是否有已選取的項目
                            if (false == string.IsNullOrEmpty(inputValue))
                            {
                                setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                            }
                            //判斷是否要顯示索引文字
                            if (showRoot)
                            {
                                setMenu.Items.Insert(0, new ListItem("-- Select --", ""));
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得Inquiry問題分類 (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="checkID"></param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        /// <remarks>
        /// DB = ProductCenter
        /// </remarks>
        public static bool Get_InquiryClass(DropDownList setMenu, string inputValue, string lang, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Cls.Class_ID AS ID, Cls.Class_Name AS Label ");
                    SBSql.AppendLine(" FROM Inquiry_Class Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y') AND (Cls.LangCode = @Lang) ");
                    SBSql.AppendLine(" ORDER BY Cls.Sort, Cls.Class_ID");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Lang", lang);
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- Select --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }
        #endregion


        #region -- 產生分頁 --
        public static string PageControl(int TotalRow, int PageSize, int CurrentPageIdx, int PageRoll, string PageUrl, ArrayList Params, bool IsRouting)
        {
            return PageControl(TotalRow, PageSize, CurrentPageIdx, PageRoll, PageUrl, Params, IsRouting, false);
        }
        /// <summary>
        /// 自訂分頁
        /// </summary>
        /// <param name="TotalRow">總筆數</param>
        /// <param name="PageSize">每頁顯示筆數</param>
        /// <param name="CurrentPageIdx">目前的索引頁</param>
        /// <param name="PageRoll">要顯示幾個頁碼</param>
        /// <param name="PageUrl">Url</param>
        /// <param name="Params">參數</param>
        /// <param name="IsRouting">是否使用Routing</param>
        /// <param name="RoutingVal">Params是否使用Routing(接在分頁後)</param>
        /// <returns>string</returns>
        public static string PageControl(int TotalRow, int PageSize, int CurrentPageIdx, int PageRoll, string PageUrl, ArrayList Params, bool IsRouting, bool RoutingVal)
        {
            //[參數宣告]
            int cntBgNum, cntEdNum;     //計算開始數, 計算終止數
            int PageBg, PageEd;     //設定頁數(開始), 設定頁數(結束)

            //[參數設定] - 計算總頁數, TotalPage
            int TotalPage = (TotalRow / PageSize);
            if (TotalRow % PageSize > 0)
            {
                TotalPage++;
            }
            //[參數設定] - 判斷Request Page, 若目前Page < 1, Page設為 1
            if (CurrentPageIdx < 1)
            {
                CurrentPageIdx = 1;
            }
            //[參數設定] - 判斷Request Page, 若目前Page > 總頁數TotalPage, Page 設為 TotalPage
            if (CurrentPageIdx > TotalPage)
            {
                CurrentPageIdx = TotalPage;
            }
            //[參數設定] - 開始資料列/結束資料列 (分頁資訊)
            int FirstItem = (CurrentPageIdx - 1) * PageSize + 1;
            int LastItem = FirstItem + (PageSize - 1);
            if (LastItem > TotalRow)
            {
                LastItem = TotalRow;
            }
            //string PageInfo = string.Format("Showing {0} to {1} of {2} entries", FirstItem, LastItem, TotalRow);

            //[分頁設定] - 計算開始頁/結束頁
            cntBgNum = CurrentPageIdx - ((PageRoll + 5) / 5);
            cntEdNum = CurrentPageIdx + ((PageRoll + 5) / 5);

            //[分頁設定] - 設定開始值/結束值
            PageBg = cntBgNum;
            PageEd = cntEdNum;

            //判斷開始值 是否小於 1
            if (PageBg < 1)
            {
                PageBg = 1;
                PageEd = (cntEdNum - cntBgNum) + 1;
            }
            //判斷結束值 是否大於 總頁數
            if (PageEd > TotalPage)
            {
                if (cntBgNum > 1)
                {
                    PageBg = cntBgNum - (cntEdNum - TotalPage);
                    if (PageBg == 0) PageBg = 1;
                }
                PageEd = TotalPage;
            }

            //----- 分頁Html -----
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"row\">");
            //sb.AppendLine("<div class=\"col-md-4 styleGraylight\">{0}</div>".FormatThis(PageInfo));
            sb.AppendLine("<div class=\"col-md-12 text-right\">");
            sb.AppendLine("<ul class=\"pagination\">");

            string fixParams = "";
            //判斷參數串
            if (Params != null && Params.Count > 0)
            {
                if (RoutingVal)
                {
                    fixParams = string.Join("/", Params.ToArray());
                }
                else
                {
                    if (IsRouting)
                        fixParams = "?" + string.Join("&", Params.ToArray());
                    else
                        fixParams = string.Join("&", Params.ToArray());
                }

            }

            //[分頁按鈕] - 第一頁 & 上一頁
            if (CurrentPageIdx > 1)
            {
                sb.Append("<li>");
                /*第一頁
                sb.AppendFormat("<a href=\"{0}{1}{2}\"><span>&lt;&lt;</span></a> ", PageUrl
                    , (IsRouting) ? "/{0}/".FormatThis(1) : "?page=1"
                    , fixParams);
                 */

                sb.AppendFormat("<a href=\"{0}{1}{2}\"><span>{3}</span></a> ", PageUrl
                    , (IsRouting) ? "/{0}/".FormatThis(CurrentPageIdx - 1) : "?page={0}".FormatThis(CurrentPageIdx - 1)
                    , fixParams
                    , "← ");

                sb.Append("</li>");
            }
            else
            {
                sb.AppendLine("<li class=\"disabled\"><a>← </a></li>");
            }

            //[分頁按鈕] - 數字頁碼
            for (int row = PageBg; row <= PageEd; row++)
            {
                if (row == CurrentPageIdx)
                {
                    sb.Append("<li class=\"active\">");
                    sb.AppendFormat("<a>{0}</a>", row);
                    sb.Append("</li>");
                }
                else
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<a href=\"{0}{1}{2}\">{3}</a> ", PageUrl
                    , (IsRouting) ? "/{0}/".FormatThis(row) : "?page={0}".FormatThis(row)
                    , fixParams
                    , row);
                    sb.Append("</li>");
                }
            }

            //[分頁按鈕] - 最後一頁 & 下一頁
            if (CurrentPageIdx < TotalPage)
            {
                sb.Append("<li>");
                sb.AppendFormat("<a href=\"{0}{1}{2}\"><span>{3}</span></a> ", PageUrl
                    , (IsRouting) ? "/{0}/".FormatThis(CurrentPageIdx + 1) : "?page={0}".FormatThis(CurrentPageIdx + 1)
                    , fixParams
                    , " →");

                /*最後一頁
                  sb.AppendFormat("<a href=\"{0}{1}{2}\"><span>{3}</span></a> ", PageUrl
                      , (IsRouting) ? "/{0}/".FormatThis(TotalPage) : "?page={0}".FormatThis(TotalPage)
                      , fixParams
                      , "&gt;&gt;");
               */

                sb.Append("</li>");
            }
            else
            {
                sb.AppendLine("<li class=\"disabled\"><a> →</a></li>");
            }


            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");


            //[輸出Html]
            return sb.ToString();
        }

        #endregion


        #region -- 其他功能 --
        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="fileName"></param>
        /// <remarks>
        /// 使用元件:ClosedXML
        /// </remarks>
        public static void ExportExcel(DataTable DT, string fileName)
        {
            //宣告
            XLWorkbook wbook = new XLWorkbook();

            //加入WorkSheet
            var ws = wbook.Worksheets.Add(DT, "PKDataList");

            //鎖定工作表, 並設定密碼
            ws.Protect("iLoveProkits25")    //Set Password
                .SetFormatCells(true)   // Cell Formatting
                .SetInsertColumns() // Inserting Columns
                .SetDeleteColumns() // Deleting Columns
                .SetDeleteRows();   // Deleting Rows

            //Disable autofilter
            ws.Tables.FirstOrDefault().ShowAutoFilter = false;

            //Http Response & Request
            var resp = HttpContext.Current.Response;
            var req = HttpContext.Current.Request;
            HttpResponse httpResponse = resp;
            httpResponse.Clear();
            // 編碼
            httpResponse.ContentEncoding = Encoding.UTF8;
            // 設定網頁ContentType
            httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // 匯出檔名
            var browser = req.Browser.Browser;
            var exportFileName = browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : HttpUtility.UrlEncode(fileName, Encoding.UTF8);

            resp.AddHeader(
                "Content-Disposition",
                string.Format("attachment;filename={0}", exportFileName));

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wbook.SaveAs(memoryStream);
                memoryStream.WriteTo(httpResponse.OutputStream);
                memoryStream.Close();
            }

            httpResponse.End();
        }

        /// <summary>
        /// Linq查詢結果轉Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// 此方法僅可接受IEnumerable<T>泛型物件
        /// DataTable dt = LINQToDataTable(query);
        /// </remarks>
        public static DataTable LINQToDataTable<T>(IEnumerable<T> query)
        {
            //宣告一個datatable
            DataTable tbl = new DataTable();
            //宣告一個propertyinfo為陣列的物件，此物件需要import reflection才可以使用
            //使用 ParameterInfo 的執行個體來取得有關參數的資料型別、預設值等資訊

            PropertyInfo[] props = null;
            //使用型別為T的item物件跑query的內容
            foreach (T item in query)
            {
                if (props == null) //尚未初始化
                {
                    //宣告一型別為T的t物件接收item.GetType()所回傳的物件
                    Type t = item.GetType();
                    //props接收t.GetProperties();所回傳型別為props的陣列物件
                    props = t.GetProperties();
                    //使用propertyinfo物件針對propertyinfo陣列的物件跑迴圈
                    foreach (PropertyInfo pi in props)
                    {
                        //將pi.PropertyType所回傳的物件指給型別為Type的coltype物件
                        Type colType = pi.PropertyType;
                        //針對Nullable<>特別處理
                        if (colType.IsGenericType
                            && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            colType = colType.GetGenericArguments()[0];
                        //建立欄位
                        tbl.Columns.Add(pi.Name, colType);
                    }
                }
                //宣告一個datarow物件
                DataRow row = tbl.NewRow();
                //同樣利用PropertyInfo跑迴圈取得props的內容，並將內容放進剛所宣告的datarow中
                //接著在將該datarow加到datatable (tb1) 當中
                foreach (PropertyInfo pi in props)
                    row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
                tbl.Rows.Add(row);
            }
            //回傳tb1的datatable物件
            return tbl;
        }
        #endregion
    }
}
