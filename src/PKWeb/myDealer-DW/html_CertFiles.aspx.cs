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

public partial class html_CertFiles : SecurityCheckDealer
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Param_thisID))
                {
                    this.pl_warning.Visible = true;
                    return;
                }

                //帶出資料
                LookupData();
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    #region -- 資料顯示 --
    /// <summary>
    /// 顯示資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT Tbl.Model_No AS ID, Tbl.Model_Name AS Label, Tbl.Cert_Type AS ClassName");
                SBSql.Append("  , Tbl.CertFile, Tbl.TestReport");
                SBSql.Append("  , Tbl.SelfCE_{0} AS SelfCE, Tbl.SelfCheck_{0} AS SelfCheck".FormatThis(fn_Language.Param_Lang));
                SBSql.Append(" FROM(");
                SBSql.Append("     SELECT");
                SBSql.Append("      RTRIM(Prod.Model_No) AS Model_No, (Prod.Model_Name_{0}) AS Model_Name".FormatThis(fn_Language.Param_Lang));
                SBSql.Append("      , Dtl.Cert_Type");
                SBSql.Append("      , Dtl.Cert_File AS CertFile");
                SBSql.Append("      , Dtl.Cert_File_Report AS TestReport");
                SBSql.Append("      , Dtl.Cert_File_CE AS SelfCE_zh_TW, Dtl.Cert_File_CE_en_US AS SelfCE_en_US, Dtl.Cert_File_CE_zh_CN AS SelfCE_zh_CN");
                SBSql.Append("      , Dtl.Cert_File_Check AS SelfCheck_zh_TW, Dtl.Cert_File_Check_en_US AS SelfCheck_en_US, Dtl.Cert_File_Check_zh_CN AS SelfCheck_zh_CN");
                SBSql.Append("     FROM Prod_Certification Base WITH (NOLOCK)");
                SBSql.Append("      INNER JOIN Prod_Certification_Detail Dtl WITH (NOLOCK) ON Base.Cert_ID = Dtl.Cert_ID");
                SBSql.Append("      INNER JOIN Prod_Item Prod WITH (NOLOCK) ON Base.Model_No = Prod.Model_No");
                SBSql.Append("     WHERE (Dtl.IsWebDW = 'Y') AND (Prod.Model_No = @DataID)");
                SBSql.Append(" ) AS Tbl");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);

                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        this.lt_ID.Text = DT.Rows[0]["ID"].ToString();
                        this.lt_Label.Text = DT.Rows[0]["Label"].ToString();
                    }

                    //DataBind
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

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得參數資料
                string ModelNo = DataBinder.Eval(dataItem.DataItem, "ID").ToString();
                string CertFile = DataBinder.Eval(dataItem.DataItem, "CertFile").ToString();
                string TestReport = DataBinder.Eval(dataItem.DataItem, "TestReport").ToString();
                string SelfCE = DataBinder.Eval(dataItem.DataItem, "SelfCE").ToString();
                string SelfCheck = DataBinder.Eval(dataItem.DataItem, "SelfCheck").ToString();

                //取得控制項
                Literal lt_CEFile = (Literal)e.Item.FindControl("lt_CEFile");
                Literal lt_TestReport = (Literal)e.Item.FindControl("lt_TestReport");
                Literal lt_SelfCE = (Literal)e.Item.FindControl("lt_SelfCE");
                Literal lt_SelfCheck = (Literal)e.Item.FindControl("lt_SelfCheck");

                //下載資料夾
                string folder = "Certification/{0}/".FormatThis(ModelNo);

                //下載路徑
                lt_CEFile.Text = GetUrl(folder, CertFile, "{0}_{1}".FormatThis(ModelNo, CertFile));
                lt_TestReport.Text = GetUrl(folder, TestReport, "{0}_{1}".FormatThis(ModelNo, TestReport));
                lt_SelfCE.Text = GetUrl(folder, SelfCE, "{0}_{1}".FormatThis(ModelNo, SelfCE));
                lt_SelfCheck.Text = GetUrl(folder, SelfCheck, "{0}_{1}".FormatThis(ModelNo, SelfCheck));

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 取得下載路徑
    /// </summary>
    /// <param name="fileFolder"></param>
    /// <param name="fileName"></param>
    /// <param name="downloadName"></param>
    /// <returns></returns>
    private string GetUrl(string fileFolder, string fileName, string downloadName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }
        else
        {
            string url =
                "{0}myHandler/Ashx_DealerDownload.ashx?f={1}&r={2}&d={3}&t={4}".FormatThis(
                          Application["WebUrl"]
                          , HttpUtility.UrlEncode(fileFolder)
                          , HttpUtility.UrlEncode(fileName)
                          , HttpUtility.UrlEncode(downloadName)
                          , Token
                      );

            return "<a href=\"{0}\" class=\"btn btn-default\"><i class=\"fa fa-cloud-download\"></i></a>".FormatThis(url);
        }
    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Request["DataID"];

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Param_thisID = value;
        }
    }


    /// <summary>
    /// 依不同身份產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get
        {
            return fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);
        }
        set
        {
            this._Token = value;
        }
    }
    #endregion
}