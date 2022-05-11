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

public partial class PV_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("430", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "400", "430"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
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
            this.ViewState["Page_Url"] = Application["WebUrl"] + "PVList/Search";
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

            SBSql.AppendLine("  SELECT p.*");
            SBSql.AppendLine("  , ROW_NUMBER() OVER (ORDER BY p.Model_No) AS RowRank");
            SBSql.AppendLine("  FROM (");
            SBSql.AppendLine("  	SELECT");
            SBSql.AppendLine("  	Rel.Model_No");
            SBSql.AppendLine("  	, Base.LangCode, Base.PV_Uri AS VideoUrl");
            SBSql.AppendLine("  	, gp.Group_Name, gp.Group_ID");
            SBSql.AppendLine("  	FROM PV Base");
            SBSql.AppendLine("  	 INNER JOIN PV_Group gp ON Base.Group_ID = gp.Group_ID");
            SBSql.AppendLine("  	 INNER JOIN PV_Group_Rel_ModelNo Rel ON Base.Group_ID = Rel.Group_ID");
            SBSql.AppendLine("      WHERE (gp.Display = 'Y')");

            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Rel.Model_No LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                Params.Add("Keyword=" + Server.UrlEncode(Req_Keyword));
            }

            #endregion

            SBSql.AppendLine("  	GROUP BY Rel.Model_No, Base.LangCode, PV_Uri, gp.Group_Name, gp.Group_ID");
            SBSql.AppendLine("  ) t");
            SBSql.AppendLine("  PIVOT (");
            SBSql.AppendLine("   MAX(VideoUrl)");
            SBSql.AppendLine("   FOR LangCode IN ([zh-tw],[zh-cn],[en-us])");
            SBSql.AppendLine("  ) p");

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
            SBSql.AppendLine("  SELECT COUNT(*) AS TOTAL_CNT");
            SBSql.AppendLine("  FROM (");
            SBSql.AppendLine("  	SELECT");
            SBSql.AppendLine("  	Rel.Model_No");
            SBSql.AppendLine("  	, Base.LangCode, Base.PV_Uri AS VideoUrl");
            SBSql.AppendLine("  	, gp.Group_Name, gp.Group_ID");
            SBSql.AppendLine("  	FROM PV Base");
            SBSql.AppendLine("  	 INNER JOIN PV_Group gp ON Base.Group_ID = gp.Group_ID");
            SBSql.AppendLine("  	 INNER JOIN PV_Group_Rel_ModelNo Rel ON Base.Group_ID = Rel.Group_ID");
            SBSql.AppendLine("      WHERE (gp.Display = 'Y')");

            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Rel.Model_No LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }

            #endregion

            SBSql.AppendLine("  	GROUP BY Rel.Model_No, Base.LangCode, PV_Uri, gp.Group_Name, gp.Group_ID");
            SBSql.AppendLine("  ) t");
            SBSql.AppendLine("  PIVOT (");
            SBSql.AppendLine("   MAX(VideoUrl)");
            SBSql.AppendLine("   FOR LangCode IN ([zh-tw],[zh-cn],[en-us])");
            SBSql.AppendLine("  ) p");

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
            SBUrl.Append("{0}PVList/Search/?s=1".FormatThis(Application["WebUrl"]));

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

                SBSql.AppendLine(" SELECT TBL.* ");
                SBSql.AppendLine(" FROM ( ");

                SBSql.AppendLine("  SELECT p.*");
                SBSql.AppendLine("  FROM (");
                SBSql.AppendLine("  	SELECT");
                SBSql.AppendLine("  	Rel.Model_No");
                SBSql.AppendLine("  	, Base.LangCode, Base.PV_Uri AS VideoUrl");
                SBSql.AppendLine("  	, gp.Group_Name, gp.Group_ID");
                SBSql.AppendLine("  	FROM PV Base");
                SBSql.AppendLine("  	 INNER JOIN PV_Group gp ON Base.Group_ID = gp.Group_ID");
                SBSql.AppendLine("  	 INNER JOIN PV_Group_Rel_ModelNo Rel ON Base.Group_ID = Rel.Group_ID");
                SBSql.AppendLine("      WHERE (gp.Display = 'Y')");

                #region "..查詢條件.."

                //[查詢條件] - 關鍵字
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    SBSql.Append(" AND ( ");
                    SBSql.Append("  (Rel.Model_No LIKE '%' + @Keyword + '%') ");
                    SBSql.Append(" ) ");
                    cmd.Parameters.AddWithValue("Keyword", Req_Keyword);
                }

                #endregion

                SBSql.AppendLine("  	GROUP BY Rel.Model_No, Base.LangCode, PV_Uri, gp.Group_Name, gp.Group_ID");
                SBSql.AppendLine("  ) t");
                SBSql.AppendLine("  PIVOT (");
                SBSql.AppendLine("   MAX(VideoUrl)");
                SBSql.AppendLine("   FOR LangCode IN ([zh-tw],[zh-cn],[en-us])");
                SBSql.AppendLine("  ) p");

                SBSql.AppendLine(" ) AS TBL ");


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
                        , "{0}-產品影片清單.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
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