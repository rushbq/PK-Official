using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;

public partial class Member_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("510", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "500", "510"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 洲別
                if (fn_CustomUI.Get_Region(this.ddl_AreaCode, Req_AreaCode, true, out ErrMsg) == false)
                {
                    this.ddl_AreaCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - CountryCode
                if (!string.IsNullOrEmpty(Req_CountryCode))
                {
                    this.tb_DataValue.Text = Req_CountryCode;
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //[帶出區域Tab]
                this.lt_Tabs.Text = Get_Tab();

                //[帶出資料]
                LookupDataList(Req_PageIdx);

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="pageIndex">目前頁數</param>
    private void LookupDataList(int pageIndex)
    {
        string ErrMsg;

        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        try
        {
            //[參數宣告] - 設定本頁Url(末端無須加 "/")
            this.ViewState["Page_Url"] = Application["WebUrl"] + "Member/Search";
            ArrayList Params = new ArrayList();

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 10;  //每頁筆數
            int TotalRow = 0;  //總筆數
            int BgItem = (pageIndex - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數 

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            #region - [SQL] 資料顯示 -
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Base.Mem_ID, Base.Mem_Account, Base.Mem_Type, Base.Company, Base.LastName, Base.FirstName");
            SBSql.AppendLine("      , Base.Display, Base.IsWrite, Base.DealerCheck, Base.Create_Time");
            SBSql.AppendLine("      , ISNULL(Ct.Country_Name, '未知') AS Country_Name");
            SBSql.AppendLine("      , ('(' + Base.DealerID + ') ' + Cust.MA002) AS CustName");
            SBSql.AppendLine("      , (SELECT COUNT(*) FROM Member_SocialToken WHERE Mem_ID = Base.Mem_ID) AS SocialCnt");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.Display DESC, Base.Create_Time DESC) AS RowRank ");
            SBSql.AppendLine("    FROM Member_Data Base ");
            SBSql.AppendLine("      LEFT JOIN Geocode_CountryName Ct ON Base.Country_Code = Ct.Country_Code AND LOWER(Ct.LangCode) = 'zh-tw'");
            SBSql.AppendLine("      LEFT JOIN PKSYS.dbo.Customer Cust ON Base.DealerID = Cust.MA001 AND DBC = DBS");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "..查詢條件.."

            //Dealer ID
            if (!string.IsNullOrWhiteSpace(Req_DealerID))
            {
                SBSql.Append("  AND (Base.DealerID = @DealerID) ");
                cmd.Parameters.AddWithValue("DealerID", Req_DealerID);

                Params.Add("dealerid=" + Server.UrlEncode(Req_DealerID));
            }

            //[查詢條件] - 會員Type
            if (string.IsNullOrEmpty(Req_Tab))
            {
                //無條件則帶一般會員
                SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck IN ('N','R'))");

                Params.Add("Tab=1");
            }
            else
            {
                switch (Req_Tab)
                {
                    case "1":
                        SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck IN ('N','R')) ");
                        break;

                    case "2":
                        SBSql.Append(" AND (Base.Mem_Type = 1) ");
                        break;

                    default:
                        SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck = 'S') ");
                        break;
                }

                Params.Add("Tab=" + Server.UrlEncode(Req_Tab));
            }

            //[查詢條件] - 國家
            if (!string.IsNullOrEmpty(Req_CountryCode))
            {
                SBSql.Append("  AND (Base.Country_Code = @Country_Code) ");
                cmd.Parameters.AddWithValue("Country_Code", Req_CountryCode);

                Params.Add("CountryCode=" + Server.UrlEncode(Req_CountryCode));
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Mem_Account LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Base.LastName LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Base.FirstName LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Base.Company LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (RTRIM(Cust.MA001) LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (UPPER(Cust.MA002) LIKE UPPER('%' + @Keyword + '%')) ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                Params.Add("Keyword=" + Server.UrlEncode(Req_Keyword));
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            #endregion

            #region - [SQL] 計算筆數 -
            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Member_Data Base ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "..查詢條件.."

            //Dealer ID
            if (!string.IsNullOrWhiteSpace(Req_DealerID))
            {
                SBSql.Append("  AND (Base.DealerID = @DealerID) ");
                cmdTotalCnt.Parameters.AddWithValue("DealerID", Req_DealerID);
            }

            //[查詢條件] - 會員Type
            if (string.IsNullOrEmpty(Req_Tab))
            {
                //無條件則帶一般會員
                SBSql.Append(" AND (Base.Mem_Type = 0) ");
            }
            else
            {
                switch (Req_Tab)
                {
                    case "1":
                        SBSql.Append(" AND (Base.Mem_Type = 0) ");
                        break;

                    case "2":
                        SBSql.Append(" AND (Base.Mem_Type = 1) ");
                        break;

                    default:
                        SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck = 'S') ");
                        break;
                }
            }

            //[查詢條件] - 國家
            if (!string.IsNullOrEmpty(Req_CountryCode))
            {
                SBSql.Append("  AND (Base.Country_Code = @Country_Code) ");
                cmdTotalCnt.Parameters.AddWithValue("Country_Code", Req_CountryCode);
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Mem_Account LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Base.LastName LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Base.FirstName LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Base.Company LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            #endregion

            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalRow, out ErrMsg))
            {
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();

                if (DT.Rows.Count > 0)
                {
                    //顯示分頁, 需在DataBind之後
                    Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                    lt_Pager.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);

                }

                //[頁數判斷] - 目前頁數大於總頁數, 則導向第一頁
                //計算總頁數
                int TotalPage = (TotalRow / PageSize);
                if (TotalRow % PageSize > 0)
                {
                    TotalPage++;
                }
                if (pageIndex > TotalPage && TotalPage > 0)
                {
                    Response.Redirect("{0}/{1}/{2}".FormatThis(
                            this.ViewState["Page_Url"]
                            , 1
                            , "?" + string.Join("&", Params.ToArray())));
                }
                else
                {
                    //重新整理頁面Url
                    this.ViewState["Page_Url"] = "{0}/{1}/{2}".FormatThis(
                        this.ViewState["Page_Url"]
                        , pageIndex
                        , "?" + string.Join("&", Params.ToArray()));

                    //暫存頁面Url, 給其他頁使用
                    Session["BackListUrl"] = this.ViewState["Page_Url"];
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 讀取資料");
        }
        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
               
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - ItemCommand");
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料, 判斷狀態
                string GetDisp = DataBinder.Eval(dataItem.DataItem, "Display").ToString();

                #region 上架狀態
                //Get value
                var queryVal = fn_CustomUI.Get_ActiveDisp(true)
                  .Where(el => el.ID.Equals(GetDisp.ToUpper()))
                  .First();

                //取得控制項, 顯示狀態
                Label lb_Status = (Label)e.Item.FindControl("lb_Status");
                lb_Status.Text = queryVal.Name;
                //判斷狀態, 改變顏色
                switch (queryVal.ID.ToUpper())
                {
                    case "Y":
                        lb_Status.CssClass = "label label-success";
                        break;

                    case "N":
                        lb_Status.CssClass = "label label-default";
                        break;

                    default:
                        lb_Status.CssClass = "label label-info";
                        break;
                }
                #endregion

                //判斷是否使用社群帳號登入
                int GetSocialCnt = Convert.ToInt16(DataBinder.Eval(dataItem.DataItem, "SocialCnt"));
                if (GetSocialCnt > 0)
                {
                    Literal lt_Social = (Literal)e.Item.FindControl("lt_Social");
                    lt_Social.Text = "&nbsp;<i class=\"fa fa-child fa-lg text-info\" title=\"使用社群帳號登入\"></i>&nbsp;";
                }

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }

    /// <summary>
    /// 取得Tab
    /// </summary>
    /// <returns></returns>
    private string Get_Tab()
    {
        try
        {
            //宣告
            StringBuilder html = new StringBuilder();
            Dictionary<int, string> dicTab = new Dictionary<int, string>();
            dicTab.Add(1, "一般會員");
            dicTab.Add(2, "經銷商");
            dicTab.Add(3, "待審核的經銷商");

            //輸出Html
            html.AppendLine("<ul class=\"nav nav-tabs\">");

            foreach (KeyValuePair<int, string> kvp in dicTab)
            {
                string GetID = kvp.Key.ToString();
                string GetLabel = kvp.Value;

                html.AppendLine("<li class=\"{2}\"><a href=\"{0}\">{1}</a></li>".FormatThis(
                        Application["WebUrl"] + "Member/Search/?srh=1&Tab=" + GetID
                        , GetLabel
                        , Req_Tab.Equals(GetID) ? "active" : ""
                    ));

            }

            html.AppendLine("</ul>");

            return html.ToString();

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 取得Tab");
        }
    }
    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("{0}Member/Search/?srh=1".FormatThis(Application["WebUrl"]));

            if (!string.IsNullOrWhiteSpace(Req_DealerID))
            {
                SBUrl.Append("&dealerid=" + Server.UrlEncode(Req_DealerID));
            }

            //[查詢條件] - AreaCode
            if (this.ddl_AreaCode.SelectedIndex > 0)
            {
                SBUrl.Append("&AreaCode=" + Server.UrlEncode(this.ddl_AreaCode.SelectedValue));
            }

            //[查詢條件] - CountryCode
            if (!string.IsNullOrEmpty(this.tb_DataValue.Text))
            {
                SBUrl.Append("&CountryCode=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_DataValue.Text)));
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

            //[查詢條件] - Tab
            SBUrl.Append("&Tab=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(Req_Tab)));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
        }
    }


    protected void btn_Excel_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT");
                SBSql.Append("  Base.Mem_Account 'Email', Base.FirstName '姓', Base.LastName '名'");
                SBSql.Append("  , Base.Birthday '生日', Base.Address '地址', Base.Tel '電話', Base.Mobile '行動電話'");
                SBSql.Append("  , (CASE Base.Sex WHEN 1 THEN '男' WHEN 2 THEN '女' ELSE '' END) '性別'");
                SBSql.Append("  , (CASE Base.Mem_Type WHEN 0 THEN '一般使用者' WHEN 1 THEN '經銷商' ELSE '' END) AS '身份別'");
                SBSql.Append("  , ISNULL(Ct.Country_Name, '未知') AS '國家'");
                SBSql.Append("  , ('(' + Base.DealerID + ') ' + Cust.MA002) AS '客戶名'");
                SBSql.Append(" FROM Member_Data Base ");
                SBSql.Append("  LEFT JOIN Geocode_CountryName Ct ON Base.Country_Code = Ct.Country_Code AND LOWER(Ct.LangCode) = 'zh-tw'");
                SBSql.Append("  LEFT JOIN PKSYS.dbo.Customer Cust ON Base.DealerID = Cust.MA001 AND DBC = DBS");
                SBSql.Append(" WHERE (1 = 1) ");

                #region "..查詢條件.."

                //[查詢條件] - 會員Type
                if (string.IsNullOrEmpty(Req_Tab))
                {
                    //無條件則帶一般會員
                    SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck IN ('N','R'))");
                }
                else
                {
                    switch (Req_Tab)
                    {
                        case "1":
                            SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck IN ('N','R')) ");
                            break;

                        case "2":
                            SBSql.Append(" AND (Base.Mem_Type = 1) ");
                            break;

                        default:
                            SBSql.Append(" AND (Base.Mem_Type = 0) AND (Base.DealerCheck = 'S') ");
                            break;
                    }
                }

                //[查詢條件] - 國家
                if (!string.IsNullOrEmpty(Req_CountryCode))
                {
                    SBSql.Append("  AND (Base.Country_Code = @Country_Code) ");
                    cmd.Parameters.AddWithValue("Country_Code", Req_CountryCode);
                }

                //[查詢條件] - 關鍵字
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    SBSql.Append(" AND ( ");
                    SBSql.Append("  (Base.Mem_Account LIKE '%' + @Keyword + '%') ");
                    SBSql.Append("  OR (Base.LastName LIKE '%' + @Keyword + '%') ");
                    SBSql.Append("  OR (Base.FirstName LIKE '%' + @Keyword + '%') ");
                    SBSql.Append("  OR (Base.Company LIKE '%' + @Keyword + '%') ");
                    SBSql.Append(" ) ");
                    cmd.Parameters.AddWithValue("Keyword", Req_Keyword);
                }

                #endregion

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料", "");
                        return;
                    }

                    //匯出Excel
                    fn_CustomUI.ExportExcel(
                        DT
                        , "{0}-會員資料.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
                        , false);
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - Excel", "");
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    private int _Req_PageIdx;
    public int Req_PageIdx
    {
        get
        {
            int PageID = Convert.ToInt32(Page.RouteData.Values["PageID"]);
            return PageID;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - AreaCode
    /// </summary>
    private string _Req_AreaCode;
    public string Req_AreaCode
    {
        get
        {
            String Disp = Request.QueryString["AreaCode"];
            return (fn_Extensions.String_資料長度Byte(Disp, "1", "4", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Disp).Trim() : "";
        }
        set
        {
            this._Req_AreaCode = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - CountryCode
    /// </summary>
    private string _Req_CountryCode;
    public string Req_CountryCode
    {
        get
        {
            String CountryCode = Request.QueryString["CountryCode"];
            return (fn_Extensions.String_資料長度Byte(CountryCode, "1", "10", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(CountryCode).Trim() : "";
        }
        set
        {
            this._Req_CountryCode = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Tab
    /// </summary>
    private string _Req_Tab;
    public string Req_Tab
    {
        get
        {
            String ReqData = Request.QueryString["Tab"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "1", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "1";
        }
        set
        {
            this._Req_Tab = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["Keyword"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }


    private string _Req_DealerID;
    public string Req_DealerID
    {
        get
        {
            String ReqData = Request.QueryString["dealerid"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "20", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "";
        }
        set
        {
            this._Req_DealerID = value;
        }
    }

    #endregion
}