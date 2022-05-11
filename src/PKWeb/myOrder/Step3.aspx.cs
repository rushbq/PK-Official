using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using eOrder.Models;
using MailMethods;
using PKLib_Method.Methods;

public partial class myOrder_Step3 : SecurityCheckDealer
{
    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //檢查ID是否為空
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    Response.Redirect(Application["WebUrl"] + "EO/List");
                    return;
                }

                //取得資料
                LookupData();

            }


        }
        catch (Exception)
        {

            throw;
        }

    }

    #region -- 資料讀取 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);
        search.Add((int)mySearch.CustID, fn_Param.Get_CustID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                TotalPrice = fld.TotalPrice,
                Staus = fld.Status

            }).FirstOrDefault();


        //Check Data
        if (query == null)
        {
            //No data ...redirect
            Response.Redirect(Application["WebUrl"] + "EO/List");
            return;
        }


        //檢查訂單是否已完成
        if (query.Staus.Equals(4))
        {
            Response.Redirect(Application["WebUrl"] + "EO/Step1");
            return;
        }

        //載入單身資料
        LookupDetailData();


        //----- 資料整理:填入資料 -----
        this.lt_TraceID.Text = query.TraceID;
        this.lt_TotalPrice.Text = fn_stringFormat.Money_Format(query.TotalPrice.ToString());

        //運費提示
        this.ph_Freight.Visible = query.TotalPrice < 10000;

        query = null;

    }

    private void LookupDetailData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDetailList(Req_DataID);

        //----- 資料繫結 -----
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();


        //填入表頭的多語系文字
        ((Literal)this.lvDataList.FindControl("lt_Header1")).Text = this.GetLocalResourceObject("txt_Header1").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header2")).Text = this.GetLocalResourceObject("txt_Header2").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header3")).Text = this.GetLocalResourceObject("txt_Header3").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header4")).Text = this.GetLocalResourceObject("txt_Header4").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header5")).Text = this.GetLocalResourceObject("txt_Header5").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header6")).Text = this.GetLocalResourceObject("txt_Header6").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header7")).Text = this.GetLocalResourceObject("txt_Header7").ToString();
    }

    #endregion


    #region -- 按鈕事件 --


    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        string traceID = this.lt_TraceID.Text;

        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();

        //建立基本資料參數, 用途:資料篩選條件
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID
        };


        //產生快照PDF
        if (!Upload_SnapShotPdf(Req_DataID, traceID, out ErrMsg))
        {
            //[Log]
            string Msg = "快照產生失敗(Step3)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_errMessage.Visible = true;
            return;
        }


        //匯入EDI
        if (!_data.Create_EDI(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "EDI匯入失敗(Step3)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_errMessage.Visible = true;
            return;
        }

        //更新狀態
        if (!_data.Update_Status(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "狀態更新失敗(Step3)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_errMessage.Visible = true;
            return;
        }

        //清空暫存
        if (!_data.Delete_Temp(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "暫存檔清空失敗(Step3)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_errMessage.Visible = true;
            return;
        }

        //MAIL通知相關人員
        if (!MailInform(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "發送訂單通知信失敗(Step3)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);
        }


        //OK
        Response.Redirect(Application["WebUrl"] + "EO/Step4/" + baseData.Data_ID.ToString());

    }


    /// <summary>
    /// 產生PDF快照
    /// </summary>
    /// <param name="dataID"></param>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private bool Upload_SnapShotPdf(string dataID, string traceID, out string ErrMsg)
    {
        try
        {
            //[Step1] 取得Step3 HTML
            string url = "{0}EO/PDFHtml/{1}".FormatThis(Application["WebUrl"], dataID);

            //[Step2] 設定API產生PDF的網址
            //string pdfUrl = "http://localhost/API/PDF/f507450f1d94dec7ac6a268654eca5f1/";
            string pdfUrl = "{0}PDF/{1}/".FormatThis(Application["API_WebUrl"]
                , System.Web.Configuration.WebConfigurationManager.AppSettings["API_TokenID"]);

            //[Step2] 設定參數,準備丟給API
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //要轉換的路徑
            dic.Add("u", url);
            //產生檔名
            dic.Add("f", "view.pdf");

            //[Step3] 使用API產生PDF,將PDF轉成byte
            byte[] pdfByte = CustomExtension.WebRequestByte_byPOST(pdfUrl, dic, null);

            //[Step3] 使用byte方式上傳至FTP
            bool isOK = _ftp.FTP_doUploadWithByte(pdfByte, UploadFolder(traceID), traceID + ".pdf");

            ErrMsg = "";

            return isOK;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }

    }


    /// <summary>
    /// 寄通知信給 該客戶所屬業務人員的部門
    /// </summary>
    /// <param name="dataID"></param>
    /// <returns></returns>
    private bool MailInform(string dataID, out string ErrMsg)
    {
        try
        {
            /*
            * 取得基本資料
            */
            //----- 宣告:資料參數 -----
            eOrderingRepository _baseData = new eOrderingRepository();
            Dictionary<int, string> search = new Dictionary<int, string>();

            //----- 原始資料:條件篩選 -----
            search.Add((int)mySearch.DataID, dataID);

            //----- 原始資料:取得資料 -----
            var query = _baseData.GetDataList(search).Take(1)
                .Select(fld => new
                {
                    TraceID = fld.TraceID,
                    CustID = fld.CustID,
                    CustName = fld.CustName,
                    UpdateTime = fld.Update_Time

                }).FirstOrDefault();
            if (query == null)
            {
                ErrMsg = "[Email發送]基本資料未取得";
                return false;
            }


            //[發送郵件]
            #region - 寄EMail -
            //[設定參數] - 建立者(20字)
            fn_Mail.Create_Who = "PKWeb-System";

            //[設定參數] - 來源程式/功能
            fn_Mail.FromFunc = "線上下單,下單通知";

            //[設定參數] - 寄件人
            fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

            //[設定參數] - 寄件人顯示名稱
            fn_Mail.SenderName = "Pro'sKit eOrder Inform";


            //[設定參數] - 收件人        
            List<string> emailTo = new List<string>();
            eOrderingRepository _mailData = new eOrderingRepository();

            //該客戶所屬業務人員的部門EMAIL
            //var mailList = _mailData.GetMailList(query.CustID);
            //foreach (var mailto in mailList)
            //{
            //    emailTo.Add(mailto.MailAddress);
            //}

            //發給指定對象(2020/7/20 #99-20200720-0003)
            //20201225:移除szsales
            emailTo.Add("SH_Sales@mail.prokits.com.tw");
            emailTo.Add("SH_OPTeam@mail.prokits.com.tw");
            emailTo.Add("opteam@mail.prokits.com.tw");

            //固定傳送MAIL:系統收件箱
            emailTo.Add(fn_Param.SysMail_Inform);

            //Set reciever
            fn_Mail.Reciever = emailTo;


            //[設定參數] - 轉寄人群組
            fn_Mail.CC = null;

            //[設定參數] - 密件轉寄人群組
            fn_Mail.BCC = null;


            #region *** 設定主旨 / 內容 ***
            /* 取得基本資料, 設定主旨 / 內容 */

            //追蹤編號/單號
            string traceID = query.TraceID;
            //PDF快照路徑
            string pdfUrl = "{0}{1}{2}".FormatThis(Application["File_WebUrl"], UploadFolder(traceID), traceID + ".pdf");

            //[設定參數] - 郵件主旨(客户代码+中文简称+订单+日期)
            fn_Mail.Subject = "[官網線上下單]{0}{1},{2} - {3}".FormatThis(
                 query.CustID
                 , query.CustName
                 , traceID
                 , query.UpdateTime.ToDateString("yyyy/MM/dd")
                );


            //[設定參數] - 郵件內容
            StringBuilder mailBody = new StringBuilder();
            mailBody.Append("<h4>下單時間：{0}</h4><h4>平台單號：{1}</h4><p><a href=\"{2}\" target=\"_blank\">查看下單內容</a></p>".FormatThis(
                query.UpdateTime,
                traceID,
                pdfUrl
                ));

            fn_Mail.MailBody = mailBody;


            #endregion

            //[設定參數] - 指定檔案 - 路徑
            fn_Mail.FilePath = "";

            //[設定參數] - 指定檔案 - 檔名
            fn_Mail.FileName = "";

            //發送郵件
            fn_Mail.SendMail();

            //[判斷參數] - 寄件是否成功
            if (!fn_Mail.MessageCode.Equals(200))
            {
                ErrMsg = "Mail發送失敗";
                return false;
            }
            else
            {
                ErrMsg = "";
                return true;
            }
            #endregion
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }

    }

    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Convert.ToString(Page.RouteData.Values["DataID"]);

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}EO/Step3/{1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    /// <summary>
    /// 上傳目錄(+TraceID)
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder(string traceID)
    {
        return "{0}OrderFiles/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceID);
    }

    #endregion

}