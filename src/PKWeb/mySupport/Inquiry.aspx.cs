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

public partial class Inquiry : SecurityCheck
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //判斷是否已回填資料
                if (IsWriteData() == false)
                {
                    string url = "{0}MemberData".FormatThis(Application["WebUrl"].ToString());
                    string msg = this.GetLocalResourceObject("msg_基本資料檢查").ToString();
                    fn_Extensions.JsAlert(msg, url);
                    return;
                }

                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_技術諮詢;

                //填入語系文字
                this.tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_驗證碼").ToString());
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";

                //[取得/檢查參數] - 問題分類
                if (fn_CustomUI.Get_InquiryClass(this.ddl_ClassID, "", fn_Language.PKWeb_Lang, true, out ErrMsg) == false)
                {
                    this.ddl_ClassID.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

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
            //再次檢查是否登入中
            if (string.IsNullOrEmpty(fn_Param.MemberID))
            {
                //導向登入頁
                Response.Redirect("{0}Login?u={1}".FormatThis(
                    Application["WebUrl"].ToString()
                    , Cryptograph.MD5Encrypt(Application["WebUrl"].ToString() + "Inquiry", Application["DesKey"].ToString())
                    ));
                return;
            }

            //取得輸入參數
            string GetClassID = this.ddl_ClassID.SelectedValue;
            string GetMessage = fn_stringFormat.Set_FilterString(this.tb_Message.Text).Left(500);

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

            //[寫入資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                string TraceID = Cryptograph.GetCurrentTime().ToString();

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(InquiryID) ,0) + 1 FROM Inquiry ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增留言資料
                SBSql.AppendLine(" INSERT INTO Inquiry( ");
                SBSql.AppendLine("  InquiryID, Mem_ID, Class_ID, Message");
                SBSql.AppendLine("  , Status, Create_Time, TraceID, AreaCode");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @Mem_ID, @Class_ID, @Message");
                SBSql.AppendLine("  , 1, GETDATE(), @TraceID, @AreaCode");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Class_ID", GetClassID);
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                cmd.Parameters.AddWithValue("Message", GetMessage);
                cmd.Parameters.AddWithValue("TraceID", TraceID);
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    //失敗
                    Response.Redirect("{0}ContactNoti/2".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }

                //[發送通知信]
                #region - 寄EMail -
                //[設定參數] - 建立者(20字)
                fn_Mail.Create_Who = "PKWeb-System";

                //[設定參數] - 來源程式/功能
                fn_Mail.FromFunc = "技術支援, Inquiry";

                //[設定參數] - 寄件人
                fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

                //[設定參數] - 寄件人顯示名稱
                fn_Mail.SenderName = "Pro'sKit";

                //[設定參數] - 收件人
                fn_Mail.Reciever = emailReceiver();

                //[設定參數] - 轉寄人群組
                fn_Mail.CC = null;

                //[設定參數] - 密件轉寄人群組
                fn_Mail.BCC = null;

                //[設定參數] - 郵件主旨
                fn_Mail.Subject = this.GetLocalResourceObject("mail_郵件主旨").ToString() + " ,#{0}".FormatThis(TraceID);

                //[設定參數] - 郵件內容
                #region 郵件內容

                StringBuilder mailBody = new StringBuilder();
                //內容主體
                mailBody.Append(this.GetLocalResourceObject("mail_郵件內容").ToString());

                //回覆連結網址
                mailBody.Replace("#LinkUrl#", "http://pkef.prokits.com.tw?t=inquiry&dataID={0}".FormatThis(TraceID));
                //發出時間
                mailBody.Replace("#CurrTime#", DateTime.Now.ToString().ToDateString("yyyy-MM-dd HH:mm"));
                //追蹤編號
                mailBody.Replace("#TraceID#", TraceID);
                //客戶資料
                mailBody.Replace("#CustData#", GetCustData(GetMessage));

                fn_Mail.MailBody = mailBody;

                #endregion

                //[設定參數] - 指定檔案 - 路徑
                fn_Mail.FilePath = "";

                //[設定參數] - 指定檔案 - 檔名
                fn_Mail.FileName = "";

                //發送郵件
                fn_Mail.SendMail();

                //[判斷參數] - 寄信是否成功
                if (fn_Mail.MessageCode.Equals(200))
                {
                    //成功
                    Response.Redirect("{0}ContactNoti/1".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                else
                {
                    //寄信發生錯誤, 需要檢查
                    Response.Redirect("{0}ContactNoti".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                #endregion

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 取得收信人
    /// </summary>
    /// <returns></returns>
    private List<string> emailReceiver()
    {
        //[取得資料]
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT MailAddress ");
            SBSql.AppendLine(" FROM Inquiry_Receiver WITH (NOLOCK) ");
            SBSql.AppendLine(" WHERE (AreaCode = @AreaCode) AND (Display = 'Y')");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);

            // SQL查詢執行
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<string> GetEmail = new List<string>();

                //若無資料則塞mis@mail.prokits.com.tw
                if (DT.Rows.Count == 0)
                {
                    GetEmail.Add("mis@mail.prokits.com.tw");
                }
                else
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                    }
                }

                return GetEmail;

            }
        }
    }

    /// <summary>
    /// 取得會員資料
    /// </summary>
    /// <param name="msg">留言訊息</param>
    /// <returns></returns>
    private string GetCustData(string msg)
    {
        //[取得資料]
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT MD.Mem_Account AS MemberMail, MD.Company, MD.LastName, MD.FirstName, MD.Sex");
            SBSql.AppendLine("   , MD.Birthday, GC.Country_Name, MD.Address, MD.Tel, MD.Mobile");
            SBSql.AppendLine(" FROM Member_Data MD WITH (NOLOCK) ");
            SBSql.AppendLine("   LEFT JOIN Geocode_CountryName GC WITH (NOLOCK) ON MD.Country_Code = GC.Country_Code AND GC.LangCode = 'zh-tw'");
            SBSql.AppendLine(" WHERE (MD.Mem_ID = @Mem_ID)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);

            // SQL查詢執行
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                StringBuilder html = new StringBuilder();

                //header
                html.Append("<h4>相關資料與訊息</h4>");

                if (DT.Rows.Count == 0)
                {
                    //找不到會員資料, 只回傳留言訊息
                    html.Append("留言訊息:{0}".FormatThis(msg.Replace("\r", "<br/>")));

                    return html.ToString();
                }

                //會員資料 + 留言訊息
                string tdBg = "background-color: #777; font-weight: bold; color: #fff;";

                html.Append("<table width=\"100%\" border=\"1\" cellpadding=\"8\" style=\"border-collapse: collapse;\">");
                html.Append("<tr><td style=\"{0}\">Email</td><td colspan=\"3\">{1}</td></tr>".FormatThis(tdBg, DT.Rows[0]["MemberMail"].ToString()));
                html.Append("<tr><td style=\"{0}\">公司</td><td colspan=\"3\">{1}</td></tr>".FormatThis(tdBg, DT.Rows[0]["Company"].ToString()));
                html.Append("<tr><td width=\"15%\" style=\"{0}\">國家</td><td width=\"35%\">{1}</td><td width=\"15%\" style=\"{0}\">性別</td><td width=\"35%\">{2}</td></tr>".FormatThis(
                        tdBg
                        , DT.Rows[0]["Country_Name"].ToString()
                        , fn_Desc.PubAll.Sex(DT.Rows[0]["Sex"].ToString())
                    ));
                html.Append("<tr><td style=\"{0}\">姓</td><td>{1}</td><td style=\"{0}\">名</td><td>{2}</td></tr>".FormatThis(
                        tdBg
                        , DT.Rows[0]["FirstName"].ToString()
                        , DT.Rows[0]["LastName"].ToString()
                    ));
                html.Append("<tr><td style=\"{0}\">電話</td><td>{1}</td><td style=\"{0}\">手機</td><td>{2}</td></tr>".FormatThis(
                        tdBg
                        , DT.Rows[0]["Tel"].ToString()
                        , DT.Rows[0]["Mobile"].ToString()
                    ));

                html.Append("<tr><td style=\"{0}\">留言訊息</td><td colspan=\"3\">{1}</td></tr>".FormatThis(tdBg, msg.Replace("\r", "<br/>")));
                html.Append("</table>");

                return html.ToString();
            }
        }
    }

    /// <summary>
    /// 判斷是否已回填資料
    /// </summary>
    /// <returns></returns>
    private bool IsWriteData()
    {
        //[取得資料]
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT IsWrite, Tel, Mobile ");
            SBSql.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
            SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);

            // SQL查詢執行
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows[0]["IsWrite"].ToString().Equals("N"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

}