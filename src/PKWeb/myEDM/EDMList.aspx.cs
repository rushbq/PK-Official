using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class myEDM_EDMList : SecurityCheckDealer
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.home_經銷商專區;

                //所屬程式名稱
                this.Ascx_Navi1.Param_CurrID = Req_ProgID;

                //Request - Subject
                if (!string.IsNullOrWhiteSpace(Req_Subject))
                {
                    tb_Subject.Text = Req_Subject;
                }

                //取得年份
                LookupData_NewsYear();

                //取得資料
                LookupData();

                /* 判斷舊版經銷商資料連結
                 * 只有代號 = 301/302/303 有資料(特刊/停產/設變)
                 */
                string[] strAry = { "301", "302", "303" };
                foreach (var item in strAry)
                {
                    if (item.Contains(Req_ProgID))
                    {
                        this.ph_history.Visible = true;
                        break;
                    }
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 取得年份
    /// </summary>
    private void LookupData_NewsYear()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder Html = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT YEAR(Base.SendTime) AS myYear");
                SBSql.AppendLine(" FROM EDM_List Base");
                SBSql.AppendLine(" WHERE (Base.InProcess = 'Y')");
                SBSql.AppendLine("  AND (LOWER(Base.LangCode) = LOWER(@LangCode)) AND (Base.Template_ClassID = @ClassID)");
                SBSql.AppendLine(" GROUP BY YEAR(Base.SendTime)");
                SBSql.AppendLine(" ORDER BY 1 DESC ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("ClassID", Req_ClassID);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EDM, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //沒有資料, 帶入今年
                        // 下拉選單
                        string thisYear = Convert.ToString(DateTime.Now.Year);

                        // 項目連結
                        Html.AppendLine("<li><a href=\"{0}EDM/{1}/{2}/{3}\">{3}</a></li>".FormatThis(
                                Application["WebUrl"]
                                , Req_ClassID
                                , Req_ProgID
                                , thisYear
                            ));
                    }
                    else
                    {
                        // 下拉選單
                        this.ddl_Year.DataValueField = "myYear";
                        this.ddl_Year.DataTextField = "myYear";
                        this.ddl_Year.DataSource = DT.DefaultView;
                        this.ddl_Year.DataBind();
                        this.ddl_Year.SelectedValue = Req_Year;

                        // 項目連結
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            Html.AppendLine("<li><a href=\"{0}EDM/{1}/{2}/{3}\">{3}</a></li>".FormatThis(
                               Application["WebUrl"]
                                , Req_ClassID
                                , Req_ProgID
                                , DT.Rows[row]["myYear"].ToString()
                           ));
                        }
                    }

                    //填入年份資料
                    this.lt_listYear.Text = Html.ToString();

                    this.ddl_Year.Items.Insert(0, new ListItem(GetLocalResourceObject("txt_年份").ToString() + ":" + Req_Year, Req_Year));
                }


            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Menu");
        }
    }
    
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupData()
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
                SBSql.AppendLine(" SELECT Base.EDM_ID AS ID, Base.Subject AS Label, Base.SendTime AS Time");
                SBSql.AppendLine(" ,(");
                SBSql.AppendLine("     SELECT TOP 1 Block_Pic");
                SBSql.AppendLine("     FROM EDM_ListBlock");
                SBSql.AppendLine("     WHERE (Block_Pic <> '') AND (EDM_ID = Base.EDM_ID)");
                SBSql.AppendLine("     ORDER BY Sort, Block_ID");
                SBSql.AppendLine(" ) AS Img");
                SBSql.AppendLine(" FROM EDM_List Base");
                SBSql.AppendLine(" WHERE (Base.InProcess = 'Y')");
                SBSql.AppendLine("  AND (LOWER(Base.LangCode) = LOWER(@LangCode)) AND (Base.Template_ClassID = @ClassID)");
                SBSql.AppendLine("  AND (YEAR(Base.SendTime) = @Req_Year)");

                if (!string.IsNullOrWhiteSpace(Req_Subject))
                {
                    SBSql.AppendLine(" AND (Base.Subject LIKE '%' + @keyword + '%')");

                    cmd.Parameters.AddWithValue("keyword", Req_Subject);
                }

                SBSql.AppendLine(" ORDER BY Base.SendTime DESC ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("ClassID", Req_ClassID);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("Req_Year", Req_Year);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EDM, out ErrMsg))
                {
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Data");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetPic = DataBinder.Eval(e.Item.DataItem, "Img").ToString();
            string GetID = DataBinder.Eval(e.Item.DataItem, "ID").ToString();

            if (!string.IsNullOrEmpty(GetPic))
            {
                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                StringBuilder html = new StringBuilder();

                html.AppendLine("<div class=\"myImg pull-left hidden-xs\">");
                html.AppendLine("<a href=\"{0}\" target=\"_blank\"><img data-original=\"{1}\" src=\"{2}js/lazyload/grey.gif\" class=\"img-responsive fixImg lazy\" alt=\"\" width=\"165\" /></a>".FormatThis(
                        "{0}PKEDM/EDM_Html/{1}/public.html".FormatThis(Application["File_WebUrl"], GetID)
                        , "{0}PKEDM/EDM_Img/{1}/{2}".FormatThis(Application["File_WebUrl"], GetID, GetPic)
                        , Application["WebUrl"]
                    ));
                html.AppendLine("</div>");

                //顯示Html
                lt_Pic.Text = html.ToString();
            }
        }
    }

    //Search
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        string subject = tb_Subject.Text;
        Response.Redirect("{0}EDM/{1}/{2}/{3}?sj={4}".FormatThis(
               Application["WebUrl"]
               , Req_ClassID
               , Req_ProgID
               , this.ddl_Year.SelectedValue
               , Server.UrlEncode(subject)
           ));
    }

    /// <summary>
    /// 歷史記錄Url
    /// </summary>
    /// <returns></returns>
    public string historyUrl()
    {
        string getMenuUri = "{0}Redirect.aspx?ActType=log&mu={1}&rt={2}".FormatThis(
                 Application["WebUrl"]
                 , Req_ProgID
                 , HttpUtility.UrlEncode(
                     "http://w3.prokits.com.tw/CrossPage/CheckLogin.asp?l={0}&m={1}&dealerID={2}&code={3}".FormatThis(
                         fn_Language.PKWeb_Lang
                         , Req_ProgID
                         , Param_DealerID
                         , Param_DealerMD5
                     )
                 )
             );

        return getMenuUri;
    }
    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 主旨
    /// </summary>
    private string _Req_Subject;
    public string Req_Subject
    {
        get
        {
            string getData = Request.QueryString["sj"] == null ? "" : Request.QueryString["sj"].ToString();
            return getData;
        }
        set
        {
            this._Req_Subject = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - 年份
    /// </summary>
    private string _Req_Year;
    public string Req_Year
    {
        get
        {
            string getData = Page.RouteData.Values["Year"].ToString();
            return getData;
        }
        set
        {
            this._Req_Year = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 類別
    /// </summary>
    private string _Req_ClassID;
    public string Req_ClassID
    {
        get
        {
            string getData = Page.RouteData.Values["ClassID"].ToString();
            return getData;
        }
        set
        {
            this._Req_ClassID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 程式編號
    /// </summary>
    private string _Req_ProgID;
    public string Req_ProgID
    {
        get
        {
            string getData = Page.RouteData.Values["ProgID"].ToString();
            return getData;
        }
        set
        {
            this._Req_ProgID = value;
        }
    }

    /// <summary>
    /// 經銷商代號
    /// </summary>
    private string _Param_DealerID;
    public string Param_DealerID
    {
        get
        {
            return fn_Member.GetDealerID(fn_Param.MemberID);
        }
        set
        {
            this._Param_DealerID = value;
        }
    }

    private string _Param_DealerMD5;
    public string Param_DealerMD5
    {
        get
        {

            string dateNow = DateTime.Today.ToShortDateString().ToDateString("yyyyMMdd");
            string md5 = Cryptograph.MD5(Param_DealerID + dateNow);

            return md5;
        }
        set
        {
            this._Param_DealerMD5 = value;
        }
    }
    #endregion

}