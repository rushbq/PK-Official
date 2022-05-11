﻿using System;
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

public partial class myStat_ProdClass : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("610", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "600", "610"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
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

                SBSql.Append(" SELECT RTRIM(Tbl.Class_ID) Class_ID, RTRIM(Tbl.Class_Name) Class_Name");
                SBSql.Append("  , SUM(Tbl.myTotal_NotMem) myTotal_NotMem, SUM(Tbl.myTotal_IsMem) myTotal_IsMem");
                SBSql.Append(" FROM (");
                SBSql.Append("     SELECT Cls.Class_ID, (Cls.Class_Name_zh_TW) AS Class_Name");
                SBSql.Append("      , COUNT(Base.LogID) AS myTotal_NotMem, 0 AS myTotal_IsMem");
                SBSql.Append("     FROM Log_Event Base WITH(NOLOCK)");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Base.EventDesc = Prod.Model_No");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Class Cls WITH(NOLOCK) ON Prod.Class_ID = Cls.Class_ID");
                SBSql.Append("     WHERE (Base.EventID = '1002') AND (Base.Platform = '前台') AND (Base.Who <> '路人')");

                #region "..查詢條件.."

                //[查詢條件] - 日期區間, sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    SBSql.Append(" AND (Base.EventTime>= @SDate)");
                    cmd.Parameters.AddWithValue("SDate", Req_sDate);
                }

                //[查詢條件] - 日期區間, eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    SBSql.Append(" AND (Base.EventTime<= @EDate)");
                    cmd.Parameters.AddWithValue("EDate", Req_eDate + " 23:59");
                }

                #endregion

                SBSql.Append("     GROUP BY Cls.Class_ID, Cls.Class_Name_zh_TW");
                SBSql.Append("      UNION ALL");
                SBSql.Append("     SELECT Cls.Class_ID, (Cls.Class_Name_zh_TW) AS Class_Name");
                SBSql.Append("      , 0 AS myTotal_NotMem, COUNT(Base.LogID) AS myTotal_IsMem");
                SBSql.Append("     FROM Log_Event Base WITH(NOLOCK)");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Base.EventDesc = Prod.Model_No");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Class Cls WITH(NOLOCK) ON Prod.Class_ID = Cls.Class_ID");
                SBSql.Append("     WHERE (Base.EventID = '1002') AND (Base.Platform = '前台') AND (Base.Who = '路人')");

                #region "..查詢條件.."

                //[查詢條件] - 日期區間, sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    SBSql.Append(" AND (Base.EventTime>= @SDate)");
                }

                //[查詢條件] - 日期區間, eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    SBSql.Append(" AND (Base.EventTime<= @EDate)");
                }

                #endregion

                SBSql.Append("     GROUP BY Cls.Class_ID, Cls.Class_Name_zh_TW");
                SBSql.Append(" ) AS Tbl");
                SBSql.Append(" GROUP BY Tbl.Class_ID, Tbl.Class_Name");
                SBSql.Append(" ORDER BY Tbl.Class_ID, Tbl.Class_Name");

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
            SBUrl.Append("{0}Stat/ProdClass/?srh=1".FormatThis(Application["WebUrl"]));

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

                SBSql.Append(" SELECT RTRIM(Tbl.Class_ID) AS '類別編號', RTRIM(Tbl.Class_Name) AS '類別名稱'");
                SBSql.Append("  , SUM(Tbl.myTotal_IsMem) '會員', SUM(Tbl.myTotal_NotMem) AS '非會員'");
                SBSql.Append(" FROM (");
                SBSql.Append("     SELECT Cls.Class_ID, (Cls.Class_Name_zh_TW) AS Class_Name");
                SBSql.Append("      , COUNT(Base.LogID) AS myTotal_NotMem, 0 AS myTotal_IsMem");
                SBSql.Append("     FROM Log_Event Base WITH(NOLOCK)");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Base.EventDesc = Prod.Model_No");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Class Cls WITH(NOLOCK) ON Prod.Class_ID = Cls.Class_ID");
                SBSql.Append("     WHERE (Base.EventID = '1002') AND (Base.Platform = '前台') AND (Base.Who <> '路人')");

                #region "..查詢條件.."

                //[查詢條件] - 日期區間, sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    SBSql.Append(" AND (Base.EventTime>= @SDate)");
                    cmd.Parameters.AddWithValue("SDate", Req_sDate);
                }

                //[查詢條件] - 日期區間, eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    SBSql.Append(" AND (Base.EventTime<= @EDate)");
                    cmd.Parameters.AddWithValue("EDate", Req_eDate + " 23:59");
                }

                #endregion

                SBSql.Append("     GROUP BY Cls.Class_ID, Cls.Class_Name_zh_TW");
                SBSql.Append("      UNION ALL");
                SBSql.Append("     SELECT Cls.Class_ID, (Cls.Class_Name_zh_TW) AS Class_Name");
                SBSql.Append("      , 0 AS myTotal_NotMem, COUNT(Base.LogID) AS myTotal_IsMem");
                SBSql.Append("     FROM Log_Event Base WITH(NOLOCK)");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Base.EventDesc = Prod.Model_No");
                SBSql.Append("      INNER JOIN [ProductCenter].dbo.Prod_Class Cls WITH(NOLOCK) ON Prod.Class_ID = Cls.Class_ID");
                SBSql.Append("     WHERE (Base.EventID = '1002') AND (Base.Platform = '前台') AND (Base.Who = '路人')");

                #region "..查詢條件.."

                //[查詢條件] - 日期區間, sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    SBSql.Append(" AND (Base.EventTime>= @SDate)");
                }

                //[查詢條件] - 日期區間, eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    SBSql.Append(" AND (Base.EventTime<= @EDate)");
                }

                #endregion

                SBSql.Append("     GROUP BY Cls.Class_ID, Cls.Class_Name_zh_TW");
                SBSql.Append(" ) AS Tbl");
                SBSql.Append(" GROUP BY Tbl.Class_ID, Tbl.Class_Name");
                SBSql.Append(" ORDER BY Tbl.Class_ID, Tbl.Class_Name");


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
                        , "{0}-產品類別瀏覽統計資料.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
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


    #endregion
}