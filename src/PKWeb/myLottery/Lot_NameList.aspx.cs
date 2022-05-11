using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

public partial class Lot_NameList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_抽獎活動;

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //取得資料
                LookupDataList(Req_PageIdx);

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
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
            this.ViewState["Page_Url"] = "{0}Winner/{1}".FormatThis(
                    Application["WebUrl"]
                    , Req_EncID
                );
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
            SBSql.AppendLine("    SELECT Base.EventCode, Base.Lot_Name, Rel.IsConfirm, Rel.Mem_ID");
            SBSql.AppendLine("     , myMem.FirstName, myMem.LastName, myMem.Mem_Account");
            SBSql.AppendLine("     , Sub.Prize_Model AS id, Sub.Prize_Name AS label");
            SBSql.AppendLine("     , Sub.Lot_ID, Sub.Lot_PID");
            SBSql.AppendLine("     , ROW_NUMBER() OVER (ORDER BY Rel.IsConfirm ASC, Sub.Lot_PID ASC) AS RowRank ");
            SBSql.AppendLine("    FROM Lottery Base ");
            SBSql.AppendLine("      INNER JOIN Lottery_Prize Sub ON Base.Lot_ID = Sub.Lot_ID ");
            SBSql.AppendLine("      INNER JOIN Lottery_Rel_Member Rel ON Sub.Lot_ID = Rel.Lot_ID AND Sub.Lot_PID = Rel.Lot_PID ");
            SBSql.AppendLine("      INNER JOIN Member_Data myMem ON Rel.Mem_ID = myMem.Mem_ID ");
            SBSql.AppendLine("    WHERE (Base.Lot_ID = @DataID) ");

            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (myMem.FirstName LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myMem.LastName LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myMem.Mem_Account LIKE '%' + @Keyword + '%')");
                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                Params.Add("k=" + Server.UrlEncode(Req_Keyword));
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
            cmd.Parameters.AddWithValue("DataID", Req_DataID);

            #endregion

            #region - [SQL] 計算筆數 -
            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Lottery Base ");
            SBSql.AppendLine("   INNER JOIN Lottery_Prize Sub ON Base.Lot_ID = Sub.Lot_ID ");
            SBSql.AppendLine("   INNER JOIN Lottery_Rel_Member Rel ON Sub.Lot_ID = Rel.Lot_ID AND Sub.Lot_PID = Rel.Lot_PID ");
            SBSql.AppendLine("   INNER JOIN Member_Data myMem ON Rel.Mem_ID = myMem.Mem_ID ");
            SBSql.AppendLine(" WHERE (Base.Lot_ID = @DataID) ");

            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (myMem.FirstName LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myMem.LastName LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myMem.Mem_Account LIKE '%' + @Keyword + '%')");
                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }

            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            cmdTotalCnt.Parameters.AddWithValue("DataID", Req_DataID);

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
                    this.lt_Pager_top.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);

                    Literal lt_Pager_footer = (Literal)this.lvDataList.FindControl("lt_Pager_footer");
                    lt_Pager_footer.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);

                    //帶出活動名稱
                    this.lt_subTitle.Text = DT.Rows[0]["Lot_Name"].ToString();
                }

                //[頁數判斷] - 目前頁數大於總頁數, 則導向第一頁
                if (pageIndex > TotalRow && TotalRow > 0)
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
                string getConfirm = "E";

                //判斷按了哪個鈕
                switch (e.CommandName)
                {
                    case "doConfirm":
                        getConfirm = "Y";

                        break;

                    case "doUndo":
                        getConfirm = "N";
                        break;
                }

                //更新狀態
                if (!getConfirm.Equals("E"))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        StringBuilder SBSql = new StringBuilder();

                        //[取得參數] - 編號
                        string Get_Lot_PID = ((HiddenField)e.Item.FindControl("hf_Lot_PID")).Value;
                        string Get_Mem_ID = ((HiddenField)e.Item.FindControl("hf_Mem_ID")).Value;
                        
                        //[SQL] - 刪除資料
                        SBSql.AppendLine(" UPDATE Lottery_Rel_Member SET IsConfirm = @IsConfirm ");
                        SBSql.AppendLine(" WHERE (Lot_ID = @ParentID) AND (Lot_PID = @Lot_PID) AND (Mem_ID = @Mem_ID)");
                        cmd.CommandText = SBSql.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("ParentID", Req_DataID);
                        cmd.Parameters.AddWithValue("Lot_PID", Get_Lot_PID);
                        cmd.Parameters.AddWithValue("Mem_ID", Get_Mem_ID);
                        cmd.Parameters.AddWithValue("IsConfirm", getConfirm);
                        if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                        {
                            fn_Extensions.JsAlert("資料處理發生錯誤，請聯絡系統人員！", "");
                        }
                        else
                        {
                            //頁面跳至本頁
                            Response.Redirect(this.ViewState["Page_Url"].ToString());
                        }
                    }
                }
                
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料
            string Get_IsConfirm = DataBinder.Eval(e.Item.DataItem, "IsConfirm").ToString();
            //取得控制項
            LinkButton lbtn_Confirm = (LinkButton)e.Item.FindControl("lbtn_Confirm");
            LinkButton lbtn_Undo = (LinkButton)e.Item.FindControl("lbtn_Undo");

            //判斷是否已領取
            if (Get_IsConfirm.Equals("Y"))
            {
                lbtn_Confirm.Visible = false;
                lbtn_Undo.Visible = true;
            }
            else
            {
                lbtn_Confirm.Visible = true;
                lbtn_Undo.Visible = false;
            }

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
            SBUrl.Append("{0}Winner/{1}/".FormatThis(
                    Application["WebUrl"]
                    , Req_EncID
                ));

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("?k=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
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

    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["k"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 加密過編號
    /// </summary>
    private string _Req_EncID;
    public string Req_EncID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"] == null ? "" : Page.RouteData.Values["DataID"].ToString();

            return DataID;
        }
        set
        {
            this._Req_EncID = value;
        }
    }

    /// <summary>
    /// 解密編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            return Cryptograph.MD5Decrypt(Req_EncID, DesKey);
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
    {
        get
        {
            return Application["DesKey"].ToString();
        }
        set
        {
            this._DesKey = value;
        }
    }

    #endregion
}