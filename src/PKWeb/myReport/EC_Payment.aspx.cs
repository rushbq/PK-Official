using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using ExtensionMethods;
using Microsoft.Reporting.WebForms;
using ReportConn;

public partial class EC_Payment : SecurityCheckDealer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //驗證, 目前鎖定EC才能看
            if (string.IsNullOrEmpty(Request.QueryString["VID"]))
            {
                this.lb_Msg.Text = "fail:401";
                this.lb_Msg.Visible = true;
                return;
            }
            if (!ValidCode.Equals(Request.QueryString["VID"].ToString().ToLower()))
            {
                this.lb_Msg.Text = "fail:401";
                this.lb_Msg.Visible = true;
                return;
            }

            // 設定參數
            ReportParameter[] _params = new ReportParameter[0];
          

            // 帶出報表(Viewer名稱, 參數集合)
            SetReportViewerAuth(RptData, _params
                , "/OpenJun/30-AbroadDealer/"
                , "Rpt_Payment_2019"
                , "N"
                , "N"
                , "Y");

        }
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


    #region -- 參數設定 --

    /// <summary>
    /// 產生MD5驗証碼
    /// EC的客戶編號 + 自訂key
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get { return Cryptograph.MD5("1180401" + Application["DesKey"]); }
        private set { this._ValidCode = value; }
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

}