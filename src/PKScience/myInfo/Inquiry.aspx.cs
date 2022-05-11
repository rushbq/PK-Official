using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MailMethods;
using PKLib_Method.Methods;

public partial class myInfo_Inquiry : System.Web.UI.Page
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //驗證碼
                this.img_Verify.ImageUrl = fn_Param.WebUrl + "myHandler/Ashx_CreateValidImg.ashx";

                //[取得/檢查參數] - 洲別
                if (Get_Region(this.ddl_AreaCode, "", fn_Language.Web_Lang, true, out ErrMsg) == false)
                {
                    this.ddl_AreaCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 問題分類
                if (Get_InquiryClass(this.ddl_ClassID, "", fn_Language.Web_Lang, false, out ErrMsg) == false)
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


    protected void lbtn_Submit_Click(object sender, EventArgs e)
    {
        try
        {
            //檢查驗證碼
            string ImgCheckCode = Request.Cookies["ImgCheckCode"].Value;
            if (!this.tb_VerifyCode.Text.ToUpper().Equals(ImgCheckCode.ToUpper()))
            {
                this.tb_VerifyCode.Text = "";
                this.pl_Valid.Visible = true;
                this.pl_Require.Visible = false;
                return;
            }


            //檢查必填
            string firstName = this.tb_FirstName.Text.Trim();
            string lastName = this.tb_LastName.Text.Trim();
            string country = this.tb_CountryValue.Text;
            string tel = this.tb_Tel.Text.Trim();
            string email = this.tb_Email.Text.Trim();
            string msg = this.tb_Message.Text;
            string classID = this.ddl_ClassID.SelectedValue;

            if (CheckNull(firstName) || CheckNull(lastName) || CheckNull(country) || CheckNull(tel)
                || CheckNull(email) || CheckNull(msg))
            {
                this.tb_VerifyCode.Text = "";
                this.pl_Valid.Visible = false;
                this.pl_Require.Visible = true;
                return;
            }


            //--- 資料處理 ---
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
                SBSql.AppendLine("  InquiryID, Class_ID, Message");
                SBSql.AppendLine("  , FirstName, LastName, CountryCode, Tel, Email");
                SBSql.AppendLine("  , Status, Create_Time, TraceID");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @Class_ID, @Message");
                SBSql.AppendLine("  , @FirstName, @LastName, @CountryCode, @Tel, @Email");
                SBSql.AppendLine("  , 1, GETDATE(), @TraceID");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Class_ID", classID);
                cmd.Parameters.AddWithValue("Message", msg);
                cmd.Parameters.AddWithValue("FirstName", firstName);
                cmd.Parameters.AddWithValue("LastName", lastName);
                cmd.Parameters.AddWithValue("CountryCode", country);
                cmd.Parameters.AddWithValue("Tel", tel);
                cmd.Parameters.AddWithValue("Email", email);

                cmd.Parameters.AddWithValue("TraceID", TraceID);
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    //失敗
                    showMsgBox("Fail!", "");
                    return;
                }

                //[發送通知信]
                #region - 寄EMail -
                //[設定參數] - 建立者(20字)
                fn_Mail.Create_Who = "PKScience";

                //[設定參數] - 來源程式/功能
                fn_Mail.FromFunc = "玩具網站, Inquiry";

                //[設定參數] - 寄件人
                fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

                //[設定參數] - 寄件人顯示名稱
                fn_Mail.SenderName = "Pro'sKit Science Kits";

                //[設定參數] - 收件人
                fn_Mail.Reciever = emailReceiver();

                //[設定參數] - 轉寄人群組
                fn_Mail.CC = null;

                //[設定參數] - 密件轉寄人群組
                fn_Mail.BCC = null;

                //[設定參數] - 郵件主旨
                fn_Mail.Subject = this.GetLocalResourceObject("mail_郵件主旨").ToString() + " - 追蹤編號:{0}".FormatThis(TraceID);

                //[設定參數] - 郵件內容
                #region 郵件內容

                StringBuilder mailBody = new StringBuilder();
                //內容主體
                mailBody.Append(this.GetLocalResourceObject("mail_郵件內容").ToString());

                //回覆連結網址
                mailBody.Replace("#LinkUrl#", "http://pkef.prokits.com.tw?t=sc-inquiry&dataID={0}".FormatThis(TraceID));
                //發出時間
                mailBody.Replace("#CurrTime#", DateTime.Now.ToString().ToDateString("yyyy-MM-dd HH:mm"));
                //追蹤編號
                mailBody.Replace("#TraceID#", TraceID);

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
                    showMsgBox("Thank you!", fn_Param.WebUrl);
                    return;
                }
                else
                {
                    //寄信發生錯誤, 需要檢查
                    showMsgBox("Fail!", "");
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
    /// 顯示Alert視窗
    /// </summary>
    /// <param name="msg">alert訊息(不可空白)</param>
    /// <param name="redirectUrl">導向網址(可空白)</param>
    private void showMsgBox(string msg, string redirectUrl)
    {
        //script
        string myScript = "<script>alert('{0}');{1}</script>".FormatThis(
            msg
            , string.IsNullOrEmpty(redirectUrl) ? "" : "location.href='{0}';".FormatThis(redirectUrl));


        ClientScript.RegisterStartupScript(GetType(), "message", myScript);
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
            SBSql.AppendLine(" WHERE (Display = 'Y')");
            cmd.CommandText = SBSql.ToString();

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


    private bool CheckNull(string inputVal)
    {
        return string.IsNullOrEmpty(inputVal);
    }

    /// <summary>
    /// 取得洲別
    /// </summary>
    /// <param name="setMenu">控制項</param>
    /// <param name="inputValue">輸入值</param>
    /// <param name="showRoot">是否顯示索引文字</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns></returns>
    /// <remarks>
    /// DB = PKWeb
    /// </remarks>
    private bool Get_Region(DropDownList setMenu, string inputValue, string lang, bool showRoot, out string ErrMsg)
    {
        //清除參數
        ErrMsg = "";
        setMenu.Items.Clear();

        try
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                // ↓↓ SQL查詢組成 ↓↓
                SBSql.AppendLine(" SELECT Sub.AreaCode AS ID, Sub.AreaName AS Label ");
                SBSql.AppendLine(" FROM Geocode_AreaCode Base WITH (NOLOCK) ");
                SBSql.AppendLine(" INNER JOIN Geocode_AreaName Sub WITH (NOLOCK) ON Base.AreaCode = Sub.AreaCode ");
                SBSql.AppendLine(" WHERE (LOWER(Sub.LangCode) = LOWER(@Lang)) ");
                SBSql.AppendLine(" ORDER BY Sub.AreaCode");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Lang", lang);
                // ↑↑ SQL查詢組成 ↑↑

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                     , DT.Rows[row]["ID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (false == string.IsNullOrEmpty(inputValue))
                    {
                        setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                    }
                    //判斷是否要顯示索引文字
                    if (showRoot)
                    {
                        setMenu.Items.Insert(0, new ListItem("-- {0} --".FormatThis(this.GetLocalResourceObject("fld_地區").ToString()), ""));
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }


    /// <summary>
    /// 取得Inquiry問題分類 (for DropDownList)
    /// </summary>
    /// <param name="setMenu">控制項</param>
    /// <param name="inputValue">輸入值</param>
    /// <param name="checkID"></param>
    /// <param name="showRoot">是否顯示索引文字</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns></returns>
    /// <remarks>
    /// DB = PKScience
    /// </remarks>
    private bool Get_InquiryClass(DropDownList setMenu, string inputValue, string lang, bool showRoot, out string ErrMsg)
    {
        //清除參數
        ErrMsg = "";
        setMenu.Items.Clear();

        try
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                // ↓↓ SQL查詢組成 ↓↓
                SBSql.AppendLine(" SELECT Cls.Class_ID AS ID, Cls.Class_Name AS Label ");
                SBSql.AppendLine(" FROM Inquiry_Class Cls WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Cls.Display = 'Y') AND (Cls.LangCode = @Lang) ");
                SBSql.AppendLine(" ORDER BY Cls.Sort, Cls.Class_ID");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Lang", lang);
                // ↑↑ SQL查詢組成 ↑↑

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                     , DT.Rows[row]["ID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (false == string.IsNullOrEmpty(inputValue))
                    {
                        setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                    }
                    //判斷是否要顯示索引文字
                    if (showRoot)
                    {
                        setMenu.Items.Insert(0, new ListItem("-- {0} --".FormatThis(this.GetLocalResourceObject("fld_問題類別").ToString()), ""));
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 語系
    /// </summary>
    public string Req_Lang
    {
        get
        {
            string myData = Page.RouteData.Values["lang"].ToString();

            //若為auto, 就去抓cookie
            return myData.Equals("auto") ? fn_Language.Get_Lang(Request.Cookies["PKScience_Lang"].Value) : myData;
        }
        set
        {
            this._Req_Lang = value;
        }
    }
    private string _Req_Lang;



    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    /// <remarks>
    /// url/{cls}/
    /// </remarks>
    //public string PageUrl
    //{
    //    get
    //    {
    //        return "{0}{1}/Products/{2}".FormatThis(webUrl, Req_Lang, Req_Cls);
    //    }
    //    set
    //    {
    //        this._PageUrl = value;
    //    }
    //}
    //private string _PageUrl;


    #endregion

}