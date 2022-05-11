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
using Microsoft.Reporting.WebForms;
using ReportConn;

public partial class ReportShow : SecurityCheckDealer
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

                //取得資料
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
    /// 資料顯示, ReportCenter
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

                //[SQL] - 資料查詢
                SBSql.Append(" SELECT Base.Prog_ID AS ID");
                SBSql.Append("  , Base.Prog_Name_{0} AS Label".FormatThis(fn_Language.Param_Lang));
                SBSql.Append("  , Rpt.Rpt_Folder, Rpt.Rpt_Name, Rpt.Export_PDF, Rpt.Export_Excel, Rpt.IsPrint");
                SBSql.Append(" FROM Program Base");
                SBSql.Append("  INNER JOIN PKSYS.dbo.Customer_Report Rel ON Base.Prog_ID = Rel.Prog_ID");
                SBSql.Append("  INNER JOIN Rpt_Base Rpt ON Base.Prog_ID = Rpt.Prog_ID");
                SBSql.Append(" WHERE (Rel.Cust_ERPID = @CustID) AND (Base.Display = 'Y') AND (Base.Prog_ID = @DataID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                cmd.Parameters.AddWithValue("CustID", CustID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Report, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("No Data..", Application["WebUrl"].ToString());

                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.lt_Title.Text = DT.Rows[0]["Label"].ToString();

                        //報表資料處理
                        DataProcess(DT.Rows[0]["Rpt_Folder"].ToString()
                           , DT.Rows[0]["Rpt_Name"].ToString()
                           , DT.Rows[0]["Export_PDF"].ToString()
                           , DT.Rows[0]["Export_Excel"].ToString()
                           , DT.Rows[0]["IsPrint"].ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 報表資料處理
    /// </summary>
    /// <param name="myFolder">Report路徑</param>
    /// <param name="myReportName">Report名稱</param>
    /// <param name="myPDF">可否匯出PDF</param>
    /// <param name="myExcel">可否匯出Excel</param>
    /// <param name="myPrint">可否列印(限IE)</param>
    private void DataProcess(string myFolder, string myReportName, string myPDF, string myExcel, string myPrint)
    {

        #region ** Filter參數處理 **
        //暫存參數
        List<TempParam> ITempList = new List<TempParam>();


        ////銷售類別
        //if (!string.IsNullOrEmpty(Request["values_Class"]))
        //{
        //    ITempList.Add(new TempParam("para_Class", Request["values_Class"].ToString()));
        //}

        //客戶
        if (!string.IsNullOrEmpty(CustID))
        {
            ITempList.Add(new TempParam("para_Customer", CustID));
        }


        //年份區間 (DropDownList)
        if (!string.IsNullOrEmpty(Request["values_sYear"]))
        {
            ITempList.Add(new TempParam("para_sYear", Request["values_sYear"].ToString()));
        }
        if (!string.IsNullOrEmpty(Request["values_eYear"]))
        {
            ITempList.Add(new TempParam("para_eYear", Request["values_eYear"].ToString()));
        }

        #endregion


        // 設定參數
        ReportParameter[] _params = new ReportParameter[ITempList.Count];
        for (int row = 0; row < ITempList.Count; row++)
        {
            _params[row] = new ReportParameter(ITempList[row].Param_Name, ITempList[row].Param_Value);
        }

        // 帶出報表(Viewer名稱, 參數集合)
        SetReportViewerAuth(RptData, _params
            , myFolder
            , myReportName
            , myPDF
            , myExcel
            , myPrint);

    }

    /// <summary>
    /// 報表設定
    /// </summary>
    /// <param name="sender">ReportViewer控制項名稱</param>
    /// <param name="_params">報表參數集合</param>
    /// <param name="myFolder">Report路徑</param>
    /// <param name="myReportName">Report名稱</param>
    /// <param name="myPDF">可否匯出PDF</param>
    /// <param name="myExcel">可否匯出Excel</param>
    /// <param name="myPrint">可否列印(限IE)</param>
    public void SetReportViewerAuth(ReportViewer sender, ReportParameter[] _params
        , string myFolder, string myReportName, string myPDF, string myExcel, string myPrint)
    {
        //設定ReportViewer處理模式
        sender.ProcessingMode = ProcessingMode.Remote;

        //** 設定報表屬性 **
        var rpt_with1 = sender.ServerReport;

        // 報表資料夾路徑 (ex:/LockJun/00-Chief/Rpt_Payment)
        rpt_with1.ReportPath = myFolder + myReportName;

        // 報表參數
        rpt_with1.SetParameters(_params);

        /*
         * 若已在web.config設定 ReportViewerServerConnection,
         * 則不要設定 ServerReport.Timeout、ServerReport.ReportServerUrl、ServerReport.ReportServerCredentials、ServerReport.Cookies 或 ServerReport.Headers 屬性
         */
        ////取得報表伺服器路徑
        //string strReportsServer = System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServerUrl"];
        ////指定Uri
        //Uri reportUri = new Uri(strReportsServer);

        // 報表Url (ex:http://pkrpcenter.prokits.com.tw/Report/)
        //rpt_with1.ReportServerUrl = reportUri;
        //取得認證
        //IReportServerCredentials mycred = new MyReportServerConn();
        //rpt_with1.ReportServerCredentials = mycred;


        //關閉報表預設查詢條件
        sender.ShowParameterPrompts = false;
        sender.Visible = true;
        sender.ZoomPercent = 100;
        sender.ShowZoomControl = true;

        //關閉內建的匯出鈕, 用自訂匯出按鈕
        sender.ShowExportControls = false;

        //判斷是否有列印權限(僅限IE)
        sender.ShowPrintButton = (myPrint.Equals("Y") ? true : false);

    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 客戶編號
    /// </summary>
    private string _CustID;
    public string CustID
    {
        get
        {
            return fn_Member.GetDealerID(fn_Param.MemberID);
        }
        set
        {
            this._CustID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return string.IsNullOrEmpty(DataID) ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    /// <summary>
    /// 上一頁連結
    /// </summary>
    private string _BackUrl;
    public string BackUrl
    {
        get
        {
            return "{0}Report".FormatThis(Application["WebUrl"].ToString());
        }
        set
        {
            this._BackUrl = value;
        }
    }
    #endregion

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 名稱
        /// </summary>
        private string _Param_Name;
        public string Param_Name
        {
            get { return this._Param_Name; }
            set { this._Param_Name = value; }
        }

        /// <summary>
        /// [參數] - 值
        /// </summary>
        private string _Param_Value;
        public string Param_Value
        {
            get { return this._Param_Value; }
            set { this._Param_Value = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Name">名稱</param>
        /// <param name="Param_Value">值</param>
        public TempParam(string Param_Name, string Param_Value)
        {
            this._Param_Name = Param_Name;
            this._Param_Value = Param_Value;
        }
    }
}