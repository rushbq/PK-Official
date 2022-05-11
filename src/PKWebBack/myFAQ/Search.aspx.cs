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

public partial class FAQ_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("230", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "200", "230"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                ////[取得/檢查參數] - 區域
                //if (fn_CustomUI.Get_Area(this.cbl_Area, new string[] { "1" }, out ErrMsg) == false)
                //{
                //    this.cbl_Area.Items.Insert(0, new ListItem("選單產生失敗", ""));
                //}

                //[取得/檢查參數] - 類別
                if (fn_CustomUI.Get_FAQClass(this.ddl_FAQClass, Req_FAQClass, true, out ErrMsg) == false)
                {
                    this.ddl_FAQClass.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 名稱關鍵字
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //[取得/檢查參數] - 品號關鍵字
                if (!string.IsNullOrEmpty(Req_ModelNo))
                {
                    this.tb_ModelNo.Text = Req_ModelNo;
                }

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
            this.ViewState["Page_Url"] = Application["WebUrl"] + "FAQ/Search";
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
            SBSql.AppendLine("      Base.Group_ID, Base.Group_Name, Base.Display, Cls.Class_Name, Base.Sort");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.Sort ASC, Base.Display DESC, Base.Group_ID DESC) AS RowRank ");
            SBSql.AppendLine("    FROM FAQ_Group Base ");
            SBSql.AppendLine("      INNER JOIN FAQ_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = 'zh-tw' ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "..查詢條件.."
            ////[查詢條件] - 區域
            //if (Req_Area != null)
            //{
            //    //轉成參數串
            //    List<string> AreaCode = Req_Area.ToList<string>();

            //    SBSql.Append(" AND (Base.Group_ID IN (SELECT Area.Group_ID FROM FAQ_Area Area WHERE (Area.AreaCode IN ({0})))) ".FormatThis(
            //            fn_Extensions.GetSQLParam(AreaCode, "Area")
            //        ));

            //    for (int row = 0; row < AreaCode.Count; row++)
            //    {
            //        cmd.Parameters.AddWithValue("Area{0}".FormatThis(row), AreaCode[row].ToString());
            //    }

            //    //轉成Query字串
            //    string strArea = string.Join(",", Req_Area);
            //    Params.Add("Area=" + Server.UrlEncode(strArea));
            //}

            //[查詢條件] - 名稱關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Group_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                Params.Add("Keyword=" + Server.UrlEncode(Req_Keyword));
            }

            //[查詢條件] - 產品關鍵字
            if (!string.IsNullOrEmpty(Req_ModelNo))
            {
                SBSql.Append(" AND (Base.Group_ID IN (");
                SBSql.Append("   SELECT Group_ID FROM FAQ_Rel_ModelNo WHERE (UPPER(Model_No) LIKE '%' + UPPER(@ModelNo) + '%')");
                SBSql.Append(" )) ");
                cmd.Parameters.AddWithValue("ModelNo", Req_ModelNo);

                Params.Add("ModelNo=" + Server.UrlEncode(Req_ModelNo));
            }

            //[查詢條件] - FAQ類別
            if (!string.IsNullOrEmpty(Req_FAQClass))
            {
                SBSql.Append(" AND (Base.Class_ID = @FAQClass)");

                cmd.Parameters.AddWithValue("FAQClass", Req_FAQClass);

                Params.Add("FAQClass=" + Server.UrlEncode(Req_FAQClass));
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
            SBSql.AppendLine(" FROM FAQ_Group Base ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "..查詢條件.."
            ////[查詢條件] - 區域
            //if (Req_Area != null)
            //{
            //    //轉成參數串
            //    List<string> AreaCode = Req_Area.ToList<string>();

            //    SBSql.Append(" AND (Base.Group_ID IN (SELECT Area.Group_ID FROM FAQ_Area Area WHERE (Area.AreaCode IN ({0})))) ".FormatThis(
            //            fn_Extensions.GetSQLParam(AreaCode, "Area")
            //        ));

            //    for (int row = 0; row < AreaCode.Count; row++)
            //    {
            //        cmdTotalCnt.Parameters.AddWithValue("Area{0}".FormatThis(row), AreaCode[row].ToString());
            //    }
            //}

            //[查詢條件] - 名稱關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Group_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }

            //[查詢條件] - 產品關鍵字
            if (!string.IsNullOrEmpty(Req_ModelNo))
            {
                SBSql.Append(" AND (Base.Group_ID IN (");
                SBSql.Append("   SELECT Group_ID FROM FAQ_Rel_ModelNo WHERE (UPPER(Model_No) LIKE '%' + UPPER(@ModelNo) + '%')");
                SBSql.Append(" )) ");
                cmdTotalCnt.Parameters.AddWithValue("ModelNo", Req_ModelNo);
            }

            //[查詢條件] - FAQ類別
            if (!string.IsNullOrEmpty(Req_FAQClass))
            {
                SBSql.Append(" AND (Base.Class_ID = @FAQClass)");

                cmdTotalCnt.Parameters.AddWithValue("FAQClass", Req_FAQClass);
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
                //取得Key值
                string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                using (SqlCommand cmd = new SqlCommand())
                {
                    //刪除資料
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" DELETE FROM FAQ_FeedBack WHERE (FAQ_ID IN (SELECT FAQ_ID FROM FAQ WHERE (Group_ID = @Param_ID)))");
                    SBSql.AppendLine(" DELETE FROM FAQ_Block WHERE (FAQ_ID IN (SELECT FAQ_ID FROM FAQ WHERE (Group_ID = @Param_ID)))");
                    SBSql.AppendLine(" DELETE FROM FAQ_Rel_ModelNo WHERE (Group_ID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM FAQ WHERE (Group_ID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM FAQ_Area WHERE (Group_ID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM FAQ_Group WHERE (Group_ID = @Param_ID); ");

                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", Get_DataID);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料處理失敗", "");
                        return;
                    }
                    else
                    {
                        //導向列表頁
                        fn_Extensions.JsAlert("", this.ViewState["Page_Url"].ToString());
                    }
                }
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

                //Get value
                var queryVal = fn_CustomUI.Get_PubDisp(true)
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
                }

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
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
            SBUrl.Append("{0}FAQ/Search/?srh=1".FormatThis(Application["WebUrl"]));

            ////[查詢條件] - Area
            //var GetAreaValues = from ListItem item in this.cbl_Area.Items where item.Selected select item.Value;
            //if (GetAreaValues.Count() > 0)
            //{
            //    var delimitedString = GetAreaValues.Aggregate((x, y) => x + "," + y);

            //    SBUrl.Append("&Area=" + Server.UrlEncode(delimitedString));
            //}

            //[查詢條件] - 名稱關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

            //[查詢條件] - 產品關鍵字
            if (!string.IsNullOrEmpty(this.tb_ModelNo.Text))
            {
                SBUrl.Append("&ModelNo=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_ModelNo.Text)));
            }

            //[查詢條件] - FAQ Class
            if (this.ddl_FAQClass.SelectedIndex > 0)
            {
                SBUrl.Append("&FAQClass=" + Server.UrlEncode(this.ddl_FAQClass.SelectedValue));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
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

    ///// <summary>
    ///// 取得傳遞參數 - Area
    ///// </summary>
    //private string[] _Req_Area;
    //public string[] Req_Area
    //{
    //    get
    //    {
    //        String Area = Request.QueryString["Area"];
    //        string[] strAry;

    //        return !string.IsNullOrEmpty(Area) ? strAry = Regex.Split(Area, @"\,{1}") : strAry = null;
    //    }
    //    set
    //    {
    //        this._Req_Area = value;
    //    }
    //}

    /// <summary>
    /// 取得傳遞參數 - ModelNo
    /// </summary>
    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String Keyword = Request.QueryString["ModelNo"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_ModelNo = value;
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

    /// <summary>
    /// 取得傳遞參數 - FAQ Class
    /// </summary>
    private string _Req_FAQClass;
    public string Req_FAQClass
    {
        get
        {
            String ReqData = Request.QueryString["FAQClass"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "10", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "";
        }
        set
        {
            this._Req_FAQClass = value;
        }
    }


    #endregion
}