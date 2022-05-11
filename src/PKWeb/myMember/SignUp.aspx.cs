using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using MailMethods;

public partial class SignUp : System.Web.UI.Page
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_會員註冊;

                //填入語系文字
                this.tb_Email.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的電子郵件地址").ToString());
                this.tb_Password.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的密碼").ToString());
                this.tb_CfmPassword.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的確認密碼").ToString());
                this.tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_驗證碼").ToString());
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";
                this.btn_Submit.Text = this.GetLocalResourceObject("txt_傳送").ToString();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btn_Submit_Click(object sender, EventArgs e)
    {
        try
        {
            //取得輸入參數
            string GetEmail = this.tb_Email.Text;
            string GetPwd = this.tb_Password.Text;


            //[檢查驗證碼]
            string ImgCheckCode = Request.Cookies["ImgCheckCode"].Value;
            if (!this.tb_VerifyCode.Text.ToUpper().Equals(ImgCheckCode))
            {
                this.tb_VerifyCode.Text = "";
                fn_Extensions.JsAlert("{0} {1}".FormatThis(
                        this.GetLocalResourceObject("txt_驗證碼").ToString()
                        , this.GetLocalResourceObject("tip_error").ToString()
                        )
                    , "");
                return;
            }
         

            //[檢查Email是否已使用]
            if (!fn_Member.CheckAccount(GetEmail))
            {
                fn_Extensions.JsAlert("{0}".FormatThis(
                           this.GetLocalResourceObject("tip_帳號不可使用").ToString()
                           )
                       , "");
                return;
            }


            //[建立帳號]
            //- 取得目前TS -
            Int64 CurrTS = Cryptograph.GetCurrentTime();
            //- 取得TokenID -
            string TokenID = fn_Extensions.GetTokenID(CurrTS);
            //- 取得到期時間戳記(24hr) -
            Int64 TimeoutTS = fn_Extensions.GetTimeoutTS(CurrTS, 24);

            //- 驗證網址 -
            string ValidUrl = "{0}Register/1/{1}".FormatThis(
                    Application["WebUrl"].ToString()
                    , TokenID
                );

            if (false == fn_Member.CreateAccount(GetEmail, GetPwd, TokenID, TimeoutTS))
            {
                //跳轉至訊息頁(註冊失敗=1)
                Response.Redirect("{0}Notification_Box/1".FormatThis(Application["WebUrl"].ToString()));
                return;
            }
            else
            {
                //[發送郵件]
                #region - 寄EMail -
                //[設定參數] - 建立者(20字)
                fn_Mail.Create_Who = "PKWeb-System";

                //[設定參數] - 來源程式/功能
                fn_Mail.FromFunc = "官網-前台, 帳號啟用信";

                //[設定參數] - 寄件人
                fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

                //[設定參數] - 寄件人顯示名稱
                fn_Mail.SenderName = "Pro'sKit";

                //[設定參數] - 收件人
                List<string> emailTo = new List<string>();
                emailTo.Add(GetEmail);

                fn_Mail.Reciever = emailTo;

                //[設定參數] - 轉寄人群組
                fn_Mail.CC = null;

                //[設定參數] - 密件轉寄人群組
                fn_Mail.BCC = null;

                //[設定參數] - 郵件主旨
                fn_Mail.Subject = this.GetLocalResourceObject("mail_郵件主旨").ToString();

                //[設定參數] - 郵件內容
                #region 郵件內容

                StringBuilder mailBody = new StringBuilder();
                //內容主體
                mailBody.Append(this.GetLocalResourceObject("mail_郵件內容").ToString());

                //驗證網址
                mailBody.Replace("#ValidUrl#", ValidUrl);

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
                    Response.Redirect("{0}Notification_Box/3".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                #endregion


                //跳轉至訊息頁(註冊成功, 未驗證=2)
                Response.Redirect("{0}Notification_Box/2".FormatThis(Application["WebUrl"].ToString()));
                return;

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

   
}