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

public partial class Expo_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("120", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "100", "120"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 上架狀態
                if (fn_CustomUI.Get_NewsDisp(this.ddl_Display, Req_Display, true, out ErrMsg) == false)
                {
                    this.ddl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 區域
                if (fn_CustomUI.Get_Area(this.cbl_Area, new string[] { "1" }, out ErrMsg) == false)
                {
                    this.cbl_Area.Items.Insert(0, new ListItem("選單產生失敗", ""));
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
            this.ViewState["Page_Url"] = Application["WebUrl"] + "Expo/Search";
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
            SBSql.AppendLine("      Base.Group_ID, Base.Group_Name, Base.StartTime, Base.EndTime, Base.Display, Base.onIndex ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.onIndex DESC, Base.Display DESC, Base.Group_ID DESC) AS RowRank ");
            SBSql.AppendLine("    FROM Expo_Group Base ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "..查詢條件.."
            //[查詢條件] - 上架狀態
            if (!string.IsNullOrEmpty(Req_Display))
            {
                SBSql.Append(" AND (Base.Display = @Display) ");
                cmd.Parameters.AddWithValue("Display", Req_Display);

                Params.Add("Display=" + Server.UrlEncode(Req_Display));
            }

            //[查詢條件] - 區域
            if (Req_Area != null)
            {
                //轉成參數串
                List<string> AreaCode = Req_Area.ToList<string>();
               
                SBSql.Append(" AND (Base.Group_ID IN (SELECT Area.Group_ID FROM Expo_Area Area WHERE (Area.AreaCode IN ({0})))) ".FormatThis(
                        fn_Extensions.GetSQLParam(AreaCode, "Area")
                    ));

                for (int row = 0; row < AreaCode.Count; row++)
                {
                    cmd.Parameters.AddWithValue("Area{0}".FormatThis(row), AreaCode[row].ToString());
                }

                //轉成Query字串
                string strArea = string.Join(",", Req_Area);
                Params.Add("Area=" + Server.UrlEncode(strArea));
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
                SBSql.Append("  (Base.Group_Name LIKE '%' + @Keyword + '%') ");
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
            SBSql.AppendLine(" FROM Expo_Group Base ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "..查詢條件.."
            //[查詢條件] - 上架狀態
            if (!string.IsNullOrEmpty(Req_Display))
            {
                SBSql.Append(" AND (Base.Display = @Display) ");
                cmdTotalCnt.Parameters.AddWithValue("Display", Req_Display);
            }

            //[查詢條件] - 區域
            if (Req_Area != null)
            {
                //轉成參數串
                List<string> AreaCode = Req_Area.ToList<string>();

                SBSql.Append(" AND (Base.Group_ID IN (SELECT Area.Group_ID FROM Expo_Area Area WHERE (Area.AreaCode IN ({0})))) ".FormatThis(
                        fn_Extensions.GetSQLParam(AreaCode, "Area")
                    ));

                for (int row = 0; row < AreaCode.Count; row++)
                {
                    cmdTotalCnt.Parameters.AddWithValue("Area{0}".FormatThis(row), AreaCode[row].ToString());
                }
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
                SBSql.Append("  (Base.Group_Name LIKE '%' + @Keyword + '%') ");
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
                //取得Key值
                string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                using (SqlCommand cmd = new SqlCommand())
                {
                    //刪除資料
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" DELETE FROM Expo_Photos WHERE (Expo_ID IN (SELECT Expo_ID FROM Expo WHERE (Group_ID = @Param_ID))); ");
                    SBSql.AppendLine(" DELETE FROM Expo WHERE (Group_ID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM Expo_Area WHERE (Group_ID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM Expo_Group WHERE (Group_ID = @Param_ID); ");
                 
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
                        //刪除整個Folder
                        string fileUrl = @"{0}Expo\{1}\".FormatThis(Application["File_DiskUrl"], Get_DataID);
                        IOManage.DelFolder(fileUrl);

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
                string GetonIndex = DataBinder.Eval(dataItem.DataItem, "onIndex").ToString();

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

                    case "S":
                        lb_Status.CssClass = "label label-info";
                        break;
                }

                //判斷是否在首頁顯示
                Label lb_onIndex = (Label)e.Item.FindControl("lb_onIndex");
                if (GetonIndex.Equals("Y"))
                {
                    lb_onIndex.Visible = true;
                }
                else
                {
                    lb_onIndex.Visible = false;
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
            SBUrl.Append("{0}Expo/Search/?srh=1".FormatThis(Application["WebUrl"]));

            //[查詢條件] - Area
            var GetAreaValues = from ListItem item in this.cbl_Area.Items where item.Selected select item.Value;
            if (GetAreaValues.Count() > 0)
            {
                var delimitedString = GetAreaValues.Aggregate((x, y) => x + "," + y);

                SBUrl.Append("&Area=" + Server.UrlEncode(delimitedString));
            }

            //[查詢條件] - Display
            if (this.ddl_Display.SelectedIndex > 0)
            {
                SBUrl.Append("&Display=" + Server.UrlEncode(this.ddl_Display.SelectedValue));
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
    /// 取得傳遞參數 - Area
    /// </summary>
    private string[] _Req_Area;
    public string[] Req_Area
    {
        get
        {
            String Area = Request.QueryString["Area"];
            string[] strAry;

            return !string.IsNullOrEmpty(Area) ? strAry = Regex.Split(Area, @"\,{1}") : strAry = null;
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

    #endregion
}