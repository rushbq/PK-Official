﻿using ExtensionMethods;
using ExtensionUI;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Prod_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("410", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "400", "410"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 上架狀態
                if (fn_CustomUI.Get_NewsDisp(this.ddl_Display, Req_Display, true, out ErrMsg) == false)
                {
                    this.ddl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 產品類別
                if (fn_CustomUI.Get_ProdClass(this.ddl_ProdClass, Req_ProdClass, true, out ErrMsg) == false)
                {
                    this.ddl_ProdClass.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 已停售未下架
                if (!string.IsNullOrEmpty(Req_StopOffer))
                {
                    if (Req_StopOffer.Equals("Y"))
                    {
                        this.cb_StopOffer.Checked = true;
                    }
                    else
                    {
                        this.cb_StopOffer.Checked = false;
                    }
                }

                //[取得/檢查參數] - sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    this.show_sDate.Text = Req_sDate;
                    this.tb_StartDate.Text = Req_sDate;
                }

                //[取得/檢查參數] - eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    this.show_eDate.Text = Req_eDate;
                    this.tb_EndDate.Text = Req_eDate;
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //[帶出區域Tab]
                this.lt_Tabs.Text = GetArea_Tab();

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
            this.ViewState["Page_Url"] = Application["WebUrl"] + "Prod/Search";
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
            SBSql.AppendLine("      Base.Prod_ID, Base.Model_No, Base.StartTime, Base.EndTime, Base.Display, Base.IsNew, Base.Sort ");
            SBSql.AppendLine("      , Sub.Model_Name_zh_TW AS Model_Name, Sub.Catelog_Vol, Sub.Page");
            SBSql.AppendLine("      , (CASE WHEN GETDATE() > Sub.Stop_Offer_Date THEN 'Y' ELSE 'N' END) AS IsStop");
            SBSql.AppendLine("      , (CASE WHEN DATEDIFF(DAY, Base.StartTime, GETDATE()) > 365 THEN 'Y' ELSE 'N' END) AS IsExpired");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.IsNew DESC, Base.Display DESC, Base.Model_No) AS RowRank ");
            SBSql.AppendLine("    FROM Prod Base ");
            SBSql.AppendLine("      INNER JOIN [ProductCenter].dbo.Prod_Item Sub ON Base.Model_No = Sub.Model_No");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "..查詢條件.."
            //[查詢條件] - 上架狀態
            if (!string.IsNullOrEmpty(Req_Display))
            {
                SBSql.Append(" AND (Base.Display = @Display) ");
                cmd.Parameters.AddWithValue("Display", Req_Display);

                Params.Add("Display=" + Server.UrlEncode(Req_Display));
            }

            //[查詢條件] - 產品類別
            if (!string.IsNullOrEmpty(Req_ProdClass))
            {
                SBSql.Append(" AND (Base.Model_No IN(");
                SBSql.Append("  SELECT Model_No FROM [ProductCenter].dbo.Prod_Item");
                SBSql.Append("  WHERE (Class_ID = @ProdClass)");
                SBSql.Append("))");

                cmd.Parameters.AddWithValue("ProdClass", Req_ProdClass);

                Params.Add("ProdClass=" + Server.UrlEncode(Req_ProdClass));
            }

            //[查詢條件] - 已停售未下架
            if (Req_StopOffer.Equals("Y"))
            {
                SBSql.Append(" AND (Base.Display = 'Y') AND (Base.Model_No IN(");
                SBSql.Append("  SELECT Model_No FROM [ProductCenter].dbo.Prod_Item");
                SBSql.Append("  WHERE (GETDATE() > Stop_Offer_Date)");
                SBSql.Append("))");


                Params.Add("StopOffer=" + Server.UrlEncode(Req_StopOffer));
            }

            //[查詢條件] - 上架區域
            if (string.IsNullOrEmpty(Req_Area))
            {
                //無條件則帶全球
                SBSql.Append(" AND (Base.Model_No IN (SELECT Model_No FROM Prod_Rel_Area WHERE AreaCode = 1)) ");

                Params.Add("Area=1");
            }
            else
            {
                SBSql.Append(" AND (Base.Model_No IN (SELECT Model_No FROM Prod_Rel_Area WHERE AreaCode = @Area)) ");
                cmd.Parameters.AddWithValue("Area", Req_Area);

                Params.Add("Area=" + Server.UrlEncode(Req_Area));
            }

            //[查詢條件] - sDate
            if (!string.IsNullOrEmpty(Req_sDate))
            {
                SBSql.Append(" AND (Base.StartTime >= @StartTime) ");
                cmd.Parameters.AddWithValue("StartTime", Req_sDate);

                Params.Add("sDate=" + Server.UrlEncode(Req_sDate));
            }

            //[查詢條件] - eDate
            if (!string.IsNullOrEmpty(Req_eDate))
            {
                SBSql.Append(" AND (Base.EndTime <= @EndTime) ");
                cmd.Parameters.AddWithValue("EndTime", Req_eDate);

                Params.Add("eDate=" + Server.UrlEncode(Req_eDate));
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Model_No LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Sub.Catelog_Vol LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Sub.Page LIKE '%' + @Keyword + '%') ");
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
            SBSql.AppendLine(" FROM Prod Base ");
            SBSql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Sub ON Base.Model_No = Sub.Model_No");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "..查詢條件.."
            //[查詢條件] - 上架狀態
            if (!string.IsNullOrEmpty(Req_Display))
            {
                SBSql.Append(" AND (Base.Display = @Display) ");
                cmdTotalCnt.Parameters.AddWithValue("Display", Req_Display);
            }

            //[查詢條件] - 產品類別
            if (!string.IsNullOrEmpty(Req_ProdClass))
            {
                SBSql.Append(" AND (Base.Model_No IN(");
                SBSql.Append("  SELECT Model_No FROM [ProductCenter].dbo.Prod_Item");
                SBSql.Append("  WHERE (Class_ID = @ProdClass)");
                SBSql.Append("))");

                cmdTotalCnt.Parameters.AddWithValue("ProdClass", Req_ProdClass);
            }

            //[查詢條件] - 已停售未下架
            if (Req_StopOffer.Equals("Y"))
            {
                SBSql.Append(" AND (Base.Display = 'Y') AND (Base.Model_No IN(");
                SBSql.Append("  SELECT Model_No FROM [ProductCenter].dbo.Prod_Item");
                SBSql.Append("  WHERE (GETDATE() > Stop_Offer_Date)");
                SBSql.Append("))");
            }

            //[查詢條件] - 上架區域
            if (string.IsNullOrEmpty(Req_Area))
            {
                //無條件則帶全球
                SBSql.Append(" AND (Base.Model_No IN (SELECT Model_No FROM Prod_Rel_Area WHERE AreaCode = 1)) ");
            }
            else
            {
                SBSql.Append(" AND (Base.Model_No IN (SELECT Model_No FROM Prod_Rel_Area WHERE AreaCode = @Area)) ");
                cmdTotalCnt.Parameters.AddWithValue("Area", Req_Area);
            }

            //[查詢條件] - sDate
            if (!string.IsNullOrEmpty(Req_sDate))
            {
                SBSql.Append(" AND (Base.StartTime >= @StartTime) ");
                cmdTotalCnt.Parameters.AddWithValue("StartTime", Req_sDate);
            }

            //[查詢條件] - eDate
            if (!string.IsNullOrEmpty(Req_eDate))
            {
                SBSql.Append(" AND (Base.EndTime <= @EndTime) ");
                cmdTotalCnt.Parameters.AddWithValue("EndTime", Req_eDate);
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Model_No LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Sub.Catelog_Vol LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  OR (Sub.Page LIKE '%' + @Keyword + '%') ");
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

                //Page
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
                ////取得Key值
                //string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                //using (SqlCommand cmd = new SqlCommand())
                //{
                //    //刪除資料
                //    StringBuilder SBSql = new StringBuilder();
                //    SBSql.AppendLine(" DELETE FROM Prod_Rel_Tags WHERE (Model_No = @Param_ID); ");
                //    SBSql.AppendLine(" DELETE FROM Prod_Rel_CertIcon WHERE (Model_No = @Param_ID); ");
                //    SBSql.AppendLine(" DELETE FROM Prod WHERE (Prod_ID = @Param_ID); ");

                //    cmd.CommandText = SBSql.ToString();
                //    cmd.Parameters.Clear();
                //    cmd.Parameters.AddWithValue("Param_ID", Get_DataID);
                //    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                //    {
                //        fn_Extensions.JsAlert("資料處理失敗", "");
                //        return;
                //    }
                //    else
                //    {
                //        //導向列表頁
                //        fn_Extensions.JsAlert("", this.ViewState["Page_Url"].ToString());
                //    }
                //}
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
                string GetIsNew = DataBinder.Eval(dataItem.DataItem, "IsNew").ToString();
                string GetIsStop = DataBinder.Eval(dataItem.DataItem, "IsStop").ToString();
                string GetIsExpired = DataBinder.Eval(dataItem.DataItem, "IsExpired").ToString();

                #region 上架狀態
                //Get value
                var queryVal = fn_CustomUI.Get_NewsDisp(true)
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

                #region 新品
                //判斷是否為新品(Y)/推薦(S)
                Label lb_IsNew = (Label)e.Item.FindControl("lb_IsNew");
                switch (GetIsNew)
                {
                    case "Y":
                        lb_IsNew.Text = "新品專區";
                        lb_IsNew.Visible = true;
                        lb_IsNew.CssClass = "label label-danger";
                        break;

                    case "Z":
                        lb_IsNew.Text = "熱銷推薦";
                        lb_IsNew.Visible = true;
                        lb_IsNew.CssClass = "label label-info";
                        break;

                    default:
                        lb_IsNew.Visible = false;
                        break;
                }
                

                //超過一年的新品
                if (GetIsExpired.Equals("Y") && GetIsNew.Equals("Y"))
                {
                    lb_IsNew.Text = "過期新品(1年)";
                    lb_IsNew.CssClass = "label label-warning";
                }
                #endregion

                #region 停售
                //判斷是否已停售
                Label lb_IsStop = (Label)e.Item.FindControl("lb_IsStop");
                if (GetIsStop.Equals("Y"))
                {
                    lb_IsStop.Visible = true;
                }
                else
                {
                    lb_IsStop.Visible = false;
                }
                #endregion
            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }

    /// <summary>
    /// 取得區域Tab
    /// </summary>
    /// <returns></returns>
    private string GetArea_Tab()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT AreaCode AS ID, AreaName AS Label ");
                SBSql.AppendLine(" FROM Param_Area WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (LangCode = 'zh-tw') AND (Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort ");
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    StringBuilder html = new StringBuilder();

                    html.AppendLine("<ul class=\"nav nav-tabs\">");

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        string GetID = DT.Rows[row]["ID"].ToString();
                        string GetLabel = DT.Rows[row]["Label"].ToString();

                        html.AppendLine("<li class=\"{2}\"><a href=\"{0}\">{1}</a></li>".FormatThis(
                                Application["WebUrl"] + "Prod/Search/?srh=1&Area=" + GetID
                                , GetLabel
                                , Req_Area.Equals(GetID) ? "active" : ""
                            ));
                    }

                    html.AppendLine("</ul>");

                    return html.ToString();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 取得區域");
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
            SBUrl.Append("{0}Prod/Search/?srh=1".FormatThis(Application["WebUrl"]));

            //[查詢條件] - Display
            if (this.ddl_Display.SelectedIndex > 0)
            {
                SBUrl.Append("&Display=" + Server.UrlEncode(this.ddl_Display.SelectedValue));
            }

            //[查詢條件] - ProdClass
            if (this.ddl_ProdClass.SelectedIndex > 0)
            {
                SBUrl.Append("&ProdClass=" + Server.UrlEncode(this.ddl_ProdClass.SelectedValue));
            }

            //[查詢條件] - StopOffer
            if (this.cb_StopOffer.Checked)
            {
                SBUrl.Append("&StopOffer=Y");
            }

            //[查詢條件] - sDate
            if (!string.IsNullOrEmpty(this.tb_StartDate.Text))
            {
                SBUrl.Append("&sDate=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_StartDate.Text)));
            }

            //[查詢條件] - eDate
            if (!string.IsNullOrEmpty(this.tb_EndDate.Text))
            {
                SBUrl.Append("&eDate=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_EndDate.Text)));
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

            //[查詢條件] - 區域
            SBUrl.Append("&Area=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(Req_Area)));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 排序歸零
    /// </summary>
    protected void btn_doUpd1_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"UPDATE Prod SET Sort = 999";

                cmd.CommandText = sql;
                if (!dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("排序歸零,更新失敗", "");
                    return;
                }
            }

            Response.Redirect(this.ViewState["Page_Url"].ToString());
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 排序歸零");
        }
    }

    /// <summary>
    /// 新品歸零
    /// </summary>
    protected void btn_doUpd2_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"UPDATE Prod SET IsNew = 'N' WHERE (IsNew = 'Y')";

                cmd.CommandText = sql;
                if (!dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("新品歸零,更新失敗", "");
                    return;
                }
            }

            Response.Redirect(this.ViewState["Page_Url"].ToString());
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 新品歸零");
        }
    }

    /// <summary>
    /// 推薦歸零
    /// </summary>
    protected void btn_doUpd3_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"UPDATE Prod SET IsNew = 'N' WHERE (IsNew = 'Z')";

                cmd.CommandText = sql;
                if (!dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("推薦歸零,更新失敗", "");
                    return;
                }
            }

            Response.Redirect(this.ViewState["Page_Url"].ToString());
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 推薦歸零");
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
    /// 取得傳遞參數 - Area
    /// </summary>
    private string _Req_Area;
    public string Req_Area
    {
        get
        {
            String ReqData = Request.QueryString["Area"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "1", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "1";
        }
        set
        {
            this._Req_Area = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Display
    /// </summary>
    private string _Req_Display;
    public string Req_Display
    {
        get
        {
            String Disp = Request.QueryString["Display"];
            return (fn_Extensions.String_資料長度Byte(Disp, "1", "4", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Disp).Trim() : "";
        }
        set
        {
            this._Req_Display = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - sDate
    /// </summary>
    private string _Req_sDate;
    public string Req_sDate
    {
        get
        {
            String sDate = Request.QueryString["sDate"];
            return (fn_Extensions.String_資料長度Byte(sDate, "1", "10", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(sDate).Trim() : "";
        }
        set
        {
            this._Req_sDate = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - eDate
    /// </summary>
    private string _Req_eDate;
    public string Req_eDate
    {
        get
        {
            String eDate = Request.QueryString["eDate"];
            return (fn_Extensions.String_資料長度Byte(eDate, "1", "10", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(eDate).Trim() : "";
        }
        set
        {
            this._Req_eDate = value;
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
    /// 取得傳遞參數 - ProdClass
    /// </summary>
    private string _Req_ProdClass;
    public string Req_ProdClass
    {
        get
        {
            String ReqData = Request.QueryString["ProdClass"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "4", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "";
        }
        set
        {
            this._Req_ProdClass = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - StopOffer
    /// </summary>
    private string _Req_StopOffer;
    public string Req_StopOffer
    {
        get
        {
            String ReqData = Request.QueryString["StopOffer"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "1", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "";
        }
        set
        {
            this._Req_StopOffer = value;
        }
    }
    #endregion
    
}