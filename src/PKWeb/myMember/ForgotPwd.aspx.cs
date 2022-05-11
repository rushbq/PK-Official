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

public partial class ForgotPwd : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_忘記密碼;

                //填入語系文字
                this.tb_Email.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的電子郵件地址").ToString());
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
            //檢查驗證碼
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

            //取得輸入參數
            string GetEmail = this.tb_Email.Text;

            //宣告
            Int64 MemberID;
            Session.Remove("MemberID");

            //[判斷帳號是否存在]
            Int16 GetStatus = CheckAccount(GetEmail, out MemberID);
            switch (GetStatus)
            {
                case 0:
                    //失敗
                    Response.Redirect(GoUrl("999"));
                    break;

                default:
                    //[寄送驗證信]
                    if (ReSendMail(GetEmail, MemberID))
                    {
                        //驗證信通知
                        Response.Redirect(GoUrl("7"));
                    }
                    else
                    {
                        //失敗
                        Response.Redirect(GoUrl("999"));
                    }
                    break;
            }




        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 會員資料 - 判斷帳號是否存在
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    private Int16 CheckAccount(string account, out Int64 memberID)
    {
        try
        {
            //初始化
            memberID = 0;

            //判斷EMail格式
            if (!Regex.IsMatch(account, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                return 0;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Mem_ID ");
                sbSQL.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Mem_Account = @account) AND (Display <> 'N') ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("account", account);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //帳號不存在
                        return 0;
                    }
                    else
                    {
                        //取得會員ID
                        memberID = Convert.ToInt64(DT.Rows[0]["Mem_ID"]);

                        return 200;

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
    /// 寄送驗證信
    /// </summary>
    /// <param name="GetEmail"></param>
    /// <returns></returns>
    private bool ReSendMail(string GetEmail, Int64 MemberID)
    {
        try
        {
            //- 取得目前TS -
            Int64 CurrTS = Cryptograph.GetCurrentTime();
            //- 取得TokenID -
            string TokenID = fn_Extensions.GetTokenID(CurrTS);
            //- 取得到期時間戳記(24hr) -
            Int64 TimeoutTS = fn_Extensions.GetTimeoutTS(CurrTS, 24);

            //- 驗證網址 -
            string ValidUrl = "{0}Register/2/{1}".FormatThis(
                    Application["WebUrl"].ToString()
                    , TokenID
                );

            //[更新Member_Token]
            if (!UpdateAccount(MemberID, TokenID, TimeoutTS))
            {
                return false;
            }

            //[發送郵件]
            #region - 寄EMail -
            //[設定參數] - 建立者(20字)
            fn_Mail.Create_Who = "PKWeb-System";

            //[設定參數] - 來源程式/功能
            fn_Mail.FromFunc = "官網-前台, 忘記密碼信";

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
                return false;
            }
            else
            {
                return true;
            }
            #endregion

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 更新Member_Token
    /// </summary>
    /// <param name="memberID">會員ID</param>
    /// <param name="tokenID">TokenID</param>
    /// <param name="ts">到期時間戳記</param>
    /// <returns></returns>
    private bool UpdateAccount(Int64 memberID, string tokenID, Int64 ts)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //清除參數
            cmd.Parameters.Clear();

            //[SQL] - 刪除舊驗證資料
            SBSql.AppendLine(" DELETE FROM Member_Token ");
            SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID) AND (ActType = 2); ");

            //[SQL] - 資料處理, 新增驗證資料
            SBSql.AppendLine(" INSERT INTO Member_Token( ");
            SBSql.AppendLine("  Mem_ID, TokenID, TimeoutTS, ActType");
            SBSql.AppendLine(" ) VALUES (");
            SBSql.AppendLine("  @Mem_ID, @TokenID, @TimeoutTS, 2");
            SBSql.AppendLine(" );");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Mem_ID", memberID);
            cmd.Parameters.AddWithValue("TokenID", tokenID);
            cmd.Parameters.AddWithValue("TimeoutTS", ts);

            return dbConn.ExecuteSql(cmd, out ErrMsg);

        }
    }

    /// <summary>
    /// 訊息頁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GoUrl(string id)
    {
        return "{0}Notification/{1}".FormatThis(
            Application["WebUrl"].ToString()
            , id
            );
    }
}