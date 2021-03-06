using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;

public partial class myWarrMember_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("530", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "500", "530"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - dateType
                if (!string.IsNullOrEmpty(Req_dateType))
                {
                    this.ddl_dateType.SelectedValue = Req_dateType;
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
                //if (!string.IsNullOrEmpty(Req_Keyword))
                //{
                //    this.tb_Keyword.Text = Req_Keyword;
                //}

                //[帶出資料]
                LookupDataList();

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 副程式 - 取得資料列表
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT Sub.RID AS ID, Sub.InvoiceNo AS Invoice");
                SBSql.Append("  , ISNULL(Ct.Country_Name, '未知') AS Country_Name");
                SBSql.Append("  , Base.FirstName AS FirstName, Base.LastName AS LastName, Base.Mem_Account AS Email");
                SBSql.Append("  , Sub.RegDate AS RegDate, Sub.BuyDate AS BuyDate");
                SBSql.Append(" FROM Member_Data Base");
                SBSql.Append("  INNER JOIN Register_Prod Sub ON Base.Mem_ID = Sub.Mem_ID");
                SBSql.Append("  LEFT JOIN Geocode_CountryName Ct ON Base.Country_Code = Ct.Country_Code AND LOWER(Ct.LangCode) = 'zh-tw'");
                SBSql.Append(" WHERE (1=1) ");

                #region "..查詢條件.."

                //[查詢條件] - 關鍵字
                //if (!string.IsNullOrEmpty(Req_Keyword))
                //{
                //    SBSql.Append(" AND (");
                //    SBSql.Append("   (Base.Subject LIKE '%' + @Keyword + '%')");
                //    SBSql.Append(" )");

                //    cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                //}

                //[查詢條件] - 日期區間, sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    switch (Req_dateType)
                    {
                        case "1":
                            SBSql.Append(" AND (Sub.RegDate>= @SDate)");
                            break;

                        default:
                            SBSql.Append(" AND (Sub.BuyDate >= @SDate)");
                            break;
                    }
                    cmd.Parameters.AddWithValue("SDate", Req_sDate);
                }

                //[查詢條件] - 日期區間, eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    switch (Req_dateType)
                    {
                        case "1":
                            SBSql.Append(" AND (Sub.RegDate<= @EDate)");
                            break;

                        default:
                            SBSql.Append(" AND (Sub.BuyDate <= @EDate)");
                            break;
                    }
                    cmd.Parameters.AddWithValue("EDate", Req_eDate + " 23:59");
                }

                #endregion

                SBSql.Append(" ORDER BY Sub.RegDate DESC, Sub.BuyDate");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {

            throw;
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
            SBUrl.Append("{0}WarrMember/Search/?srh=1".FormatThis(Application["WebUrl"]));

            //[查詢條件] - dateType
            if (!string.IsNullOrEmpty(this.ddl_dateType.SelectedValue))
            {
                SBUrl.Append("&dateType=" + Server.UrlEncode(this.ddl_dateType.SelectedValue));
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

            ////[查詢條件] - 關鍵字
            //if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            //{
            //    SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            //}

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 查詢", "");
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

                SBSql.Append(" SELECT Sub.InvoiceNo AS '發票號碼'");
                SBSql.Append("  , ISNULL(Ct.Country_Name, '未知') AS '國家'");
                SBSql.Append("  , Base.FirstName AS '姓', Base.LastName AS '名', Base.Mem_Account AS 'Email'");
                SBSql.Append("  , Sub.RegDate AS '註冊日期', Sub.BuyDate AS '購買日期'");
                SBSql.Append(" FROM Member_Data Base");
                SBSql.Append("  INNER JOIN Register_Prod Sub ON Base.Mem_ID = Sub.Mem_ID");
                SBSql.Append("  LEFT JOIN Geocode_CountryName Ct ON Base.Country_Code = Ct.Country_Code AND LOWER(Ct.LangCode) = 'zh-tw'");
                SBSql.Append(" WHERE (1=1) ");

                #region "..查詢條件.."

                //[查詢條件] - 關鍵字
                //if (!string.IsNullOrEmpty(Req_Keyword))
                //{
                //    SBSql.Append(" AND (");
                //    SBSql.Append("   (Base.Subject LIKE '%' + @Keyword + '%')");
                //    SBSql.Append(" )");

                //    cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                //}

                //[查詢條件] - 日期區間, sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    switch (Req_dateType)
                    {
                        case "1":
                            SBSql.Append(" AND (Sub.RegDate>= @SDate)");
                            break;

                        default:
                            SBSql.Append(" AND (Sub.BuyDate >= @SDate)");
                            break;
                    }
                    cmd.Parameters.AddWithValue("SDate", Req_sDate);
                }

                //[查詢條件] - 日期區間, eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    switch (Req_dateType)
                    {
                        case "1":
                            SBSql.Append(" AND (Sub.RegDate<= @EDate)");
                            break;

                        default:
                            SBSql.Append(" AND (Sub.BuyDate <= @EDate)");
                            break;
                    }
                    cmd.Parameters.AddWithValue("EDate", Req_eDate + " 23:59");
                }

                #endregion

                SBSql.Append(" ORDER BY Sub.RegDate DESC, Sub.BuyDate");

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
                        , "{0}-產品保固名單.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
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
    /// 取得傳遞參數 - Keyword
    /// </summary>
    //private string _Req_Keyword;
    //public string Req_Keyword
    //{
    //    get
    //    {
    //        String Keyword = Request.QueryString["Keyword"];
    //        return (fn_Extensions.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
    //    }
    //    set
    //    {
    //        this._Req_Keyword = value;
    //    }
    //}

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
    /// 取得傳遞參數 - dateType
    /// </summary>
    private string _Req_dateType;
    public string Req_dateType
    {
        get
        {
            String data = Request.QueryString["dateType"];
            return (fn_Extensions.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(data).Trim() : "";
        }
        set
        {
            this._Req_dateType = value;
        }
    }

    #endregion
}