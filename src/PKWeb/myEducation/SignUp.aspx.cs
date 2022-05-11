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
using ExtensionUI;
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
                this.Page.Title = Resources.resPublic.title_學生期保固註冊;

                //填入語系文字
                this.tb_Email.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的電子郵件地址").ToString());
                this.tb_Password.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的密碼").ToString());
                this.tb_CfmPassword.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的確認密碼").ToString());
                this.tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_驗證碼").ToString());
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";
                this.btn_Submit.Text = this.GetLocalResourceObject("txt_傳送").ToString();
                this.tb_SchoolName.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_填入您的學校").ToString());
                this.tb_SchoolDept.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_填入您的科系").ToString());

                //[取得/檢查參數] - 縣市
                if (fn_CustomUI.Get_City(this.ddl_RegionCode, "", Req_Code, true, out ErrMsg) == false)
                {
                    this.ddl_RegionCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //帶入預設資料
                string sDate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
                string eDate = DateTime.Now.AddYears(1).ToString().ToDateString("yyyy/MM/dd");
                this.tb_RegDate.Text = sDate;
                this.show_sDate.Text = sDate;
                this.tb_WarrantyDate.Text = eDate;
                this.show_eDate.Text = eDate;

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
            if (!CheckAccount(GetEmail))
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

            if (false == CreateAccount(GetEmail, GetPwd, TokenID, TimeoutTS))
            {
                //跳轉至訊息頁(註冊失敗=1)
                Response.Redirect("{0}Notification/1".FormatThis(Application["WebUrl"].ToString()));
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
                    Response.Redirect("{0}Notification/1".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                #endregion


                //跳轉至訊息頁(註冊成功, 未驗證=2)
                Response.Redirect("{0}Notification/2".FormatThis(Application["WebUrl"].ToString()));
                return;

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 會員資料 - 判斷帳號是否可以使用
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    private bool CheckAccount(string account)
    {
        try
        {
            //判斷EMail格式
            if (!Regex.IsMatch(account, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                return false;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Mem_ID, Mem_Account ");
                sbSQL.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Mem_Account = @account) ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("account", account);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //可使用
                        return true;
                    }
                    else
                    {
                        //不可使用
                        return false;
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
    /// 會員資料 -建立帳號
    /// </summary>
    /// <param name="account">email</param>
    /// <param name="pwd">password</param>
    /// <param name="tokenID">token</param>
    /// <param name="ts">timestamp</param>
    /// <returns></returns>
    private bool CreateAccount(string account, string pwd, string tokenID, Int64 ts)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @NewID AS INT, @WID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(Mem_ID) ,0) + 1 FROM Member_Data ");
                SBSql.AppendLine(" );");
                SBSql.AppendLine(" SET @WID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(WID) ,0) + 1 FROM Member_Warranty ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增會員資料
                SBSql.AppendLine(" INSERT INTO Member_Data( ");
                SBSql.AppendLine("  Mem_ID, Mem_Account, Mem_Pwd");
                SBSql.AppendLine("  , LastName, FirstName, Mobile, Country_Code");
                SBSql.AppendLine("  , IsWrite, Display, Create_Time");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @Mem_Account, @Mem_Pwd");
                SBSql.AppendLine("  , @LastName, @FirstName, @Mobile, @Country_Code");
                SBSql.AppendLine("  , 'Y', 'S', GETDATE()");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增驗證資料
                SBSql.AppendLine(" INSERT INTO Member_Token( ");
                SBSql.AppendLine("  Mem_ID, TokenID, TimeoutTS, ActType");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @TokenID, @TimeoutTS, 1");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增保固資料
                SBSql.AppendLine(" INSERT INTO Member_Warranty( ");
                SBSql.AppendLine("  WID, Mem_ID, TID, RegDate, WarrDate");
                SBSql.AppendLine("  , Sch_Name, Sch_Dept");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @WID, @NewID, @SchoolID, @RegDate, @WarrDate");
                SBSql.AppendLine("  , @Sch_Name, @Sch_Dept");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_Account", account.ToLower());
                cmd.Parameters.AddWithValue("Mem_Pwd", Cryptograph.MD5(pwd).ToLower());
                cmd.Parameters.AddWithValue("LastName", this.tb_LastName.Text.Left(50));
                cmd.Parameters.AddWithValue("FirstName", this.tb_FirstName.Text.Left(50));
                cmd.Parameters.AddWithValue("Mobile", this.tb_Mobile.Text.Left(30));
                cmd.Parameters.AddWithValue("Country_Code", Req_Code.ToUpper());
                cmd.Parameters.AddWithValue("SchoolID", this.tb_DataValue.Text);
                cmd.Parameters.AddWithValue("RegDate", this.tb_RegDate.Text);
                cmd.Parameters.AddWithValue("WarrDate", this.tb_WarrantyDate.Text);
                cmd.Parameters.AddWithValue("TokenID", tokenID);
                cmd.Parameters.AddWithValue("TimeoutTS", ts);
                //其他科系
                if (this.tb_DataValue.Text.Equals("-1"))
                {
                    cmd.Parameters.AddWithValue("Sch_Name", this.tb_SchoolName.Text);
                    cmd.Parameters.AddWithValue("Sch_Dept", this.tb_SchoolDept.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("Sch_Name", "");
                    cmd.Parameters.AddWithValue("Sch_Dept", "");
                }

                return dbConn.ExecuteSql(cmd, out ErrMsg);

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 建立帳號");
        }
    }

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 國家代碼
    /// </summary>
    private string _Req_Code;
    public string Req_Code
    {
        get
        {
            String DataID = Convert.ToString(Page.RouteData.Values["code"]);
            return fn_stringFormat.Set_FilterHtml(DataID).Left(2);
        }
        set
        {
            this._Req_Code = value;
        }
    }
    #endregion
}