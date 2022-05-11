using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ExtensionMethods;
using MailMethods;
using Newtonsoft.Json.Linq;

public partial class Login : System.Web.UI.Page
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_會員登入;

                //填入語系文字
                this.tb_Email.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的電子郵件地址").ToString());
                this.tb_Password.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的密碼").ToString());

                //判斷是否有記住帳號
                if ((!string.IsNullOrEmpty(fn_Param.MemberAcct) && fn_Param.RememberMe.Equals("Y")))
                {
                    this.cb_Remember.Checked = true;
                    this.tb_Email.Text = fn_Param.MemberAcct;
                }

                //已登入會員
                if (!string.IsNullOrEmpty(fn_Param.MemberID))
                {
                    //導向指定網址
                    Response.Redirect(GetUrl());
                }

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 登入按鈕
    /// </summary>
    protected void btn_Login_Click(object sender, EventArgs e)
    {
        try
        {
            /*
             * 1) 判斷帳號密碼是否正確
             * 2) 判斷是否已啟用帳號(通過驗證)
             *   - 未通過:刪除舊Token,新增Token, 重新發送驗證信
             *   - 已通過:產生會員Token, 導回首頁
             */
            //取得輸入參數
            string GetEmail = this.tb_Email.Text;
            string GetPwd = fn_stringFormat.Set_FilterHtml(this.tb_Password.Text);

            //宣告變數
            Int64 MemberID;
            string MemberAcct;
            string MemberName;
            string IsWrite;
            Int16 MemberType;

            //[判斷帳號是否存在]
            //判斷帳號, 取得回覆代碼
            Int16 GetStatus = CheckAccount(GetEmail, GetPwd, out MemberID, out MemberAcct, out MemberName, out IsWrite, out MemberType);

            //確認回覆代碼, 執行相關措施
            string MD5Pwd = Cryptograph.MD5(GetPwd);
            CheckStatus(GetStatus, GetEmail, MemberID, MemberAcct, MemberName, IsWrite, MemberType, MD5Pwd);

        }
        catch (Exception)
        {

            throw;
        }
    }



    /// <summary>
    /// 判斷帳號是否存在 - 登入
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    private Int16 CheckAccount(string account, string pwd, out Int64 memberID, out string memberAcct, out string memberName, out string isWrite
        , out Int16 memberType)
    {
        try
        {
            //初始化
            memberID = 0;
            memberAcct = "";
            memberName = "";
            isWrite = "N";
            memberType = -1;

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
                sbSQL.AppendLine(" SELECT Mem_ID, Mem_Account, Display, IsWrite, Mem_Type, Mem_Pwd ");
                sbSQL.AppendLine("  , (ISNULL(LastName, SUBSTRING(Mem_Account, 1, CHARINDEX('@', Mem_Account, 1)-1))) AS Member_Name ");
                sbSQL.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Mem_Account = @account)");

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
                        //取得會員資料欄位
                        memberID = Convert.ToInt64(DT.Rows[0]["Mem_ID"]);
                        memberAcct = DT.Rows[0]["Mem_Account"].ToString();
                        memberName = DT.Rows[0]["Member_Name"].ToString();
                        isWrite = DT.Rows[0]["IsWrite"].ToString();
                        memberType = Convert.ToInt16(DT.Rows[0]["Mem_Type"]);

                        //判斷密碼是否正確
                        string memberPwd = DT.Rows[0]["Mem_Pwd"].ToString();
                        if (!Cryptograph.MD5(pwd).Equals(memberPwd))
                        {
                            return 99;
                        }

                        //帳號存在, 判斷是否已啟用
                        string GetDisp = DT.Rows[0]["Display"].ToString();
                        switch (GetDisp)
                        {
                            case "S":
                                //未啟用
                                return 10;

                            case "N":
                                //已停用
                                return 0;

                            default:
                                //已啟用
                                return 200;

                        }

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
    /// 處理回覆代碼 - 登入
    /// </summary>
    /// <param name="statusCode">0=帳號停用/帳號不存在, 10=帳號未啟用, 200=成功</param>
    /// <param name="GetEmail"></param>
    /// <param name="MemberID"></param>
    /// <param name="MemberAcct"></param>
    /// <param name="MemberName"></param>
    /// <param name="IsWrite"></param>
    /// <param name="MemberType"></param>
    private void CheckStatus(Int16 statusCode, string GetEmail, long MemberID, string MemberAcct, string MemberName
        , string IsWrite, Int16 MemberType, string pwd)
    {
        int getID;
        //設定ts, 3分後到期
        Int64 validTime = Cryptograph.GetCurrentTime() + 3 * 60;
        string _rtUrl = GoUrl("6?code={0}&err=".FormatThis(HttpUtility.UrlEncode(Req_Code)));

        switch (statusCode)
        {
            case 0:
                //判斷是否為良興會員(Y->詢問是否轉換, N->原流程)
                getID = Get_EcLifeMemberID(GetEmail, pwd, out ErrMsg);
                if (!getID.Equals(0))
                {
                    //導向
                    Response.Redirect("{0}Transfer/{1}/?ts={2}".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(getID.ToString(), Application["DesKey"].ToString())
                        , Cryptograph.MD5Encrypt(validTime.ToString(), Application["DesKey"].ToString())
                        ));
                    return;
                }

                //登入失敗
                Response.Redirect(_rtUrl + statusCode.ToString() + ErrMsg);
                break;

            case 10:
                //未啟用, [重新寄送驗證信]
                if (ReSendMail(GetEmail, MemberID))
                {
                    //重發驗證信通知
                    Response.Redirect(GoUrl("7"));
                }
                else
                {
                    //啟用信發送失敗

                    //判斷是否為良興會員(Y->詢問是否轉換, N->原流程)
                    getID = Get_EcLifeMemberID(GetEmail, pwd, out ErrMsg);
                    if (!getID.Equals(0))
                    {
                        Response.Redirect("{0}Transfer/{1}/?ts={2}".FormatThis(
                            Application["WebUrl"]
                            , Cryptograph.MD5Encrypt(getID.ToString(), Application["DesKey"].ToString())
                            , Cryptograph.MD5Encrypt(validTime.ToString(), Application["DesKey"].ToString())
                            ));
                        return;
                    }

                    //登入失敗
                    Response.Redirect(_rtUrl + statusCode.ToString() + ErrMsg);
                }
                break;

            case 99:
                //密碼錯誤
                Response.Redirect(_rtUrl + statusCode.ToString());

                break;

            case 200:
                //- 取得目前TS -
                Int64 CurrTS = Cryptograph.GetCurrentTime();
                //- 取得TokenID -
                string TokenID = fn_Extensions.GetTokenID(CurrTS);
                //- 取得到期時間戳記(365 days = 365*24 = 8760 hr) -
                Int64 TimeoutTS = fn_Extensions.GetTimeoutTS(CurrTS, 8760);

                //更新Token資料庫
                if (!UpdateAccountToken(MemberID, TokenID, TimeoutTS, 3))
                {
                    //登入失敗
                    Response.Redirect(_rtUrl + statusCode.ToString());
                }


                #region -- Cookie處理 --

                //先清除Cookie
                HttpCookie myCookie = new HttpCookie("PKWeb_MemberInfo");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);

                //產生Cookie
                HttpCookie cMemberInfo = new HttpCookie("PKWeb_MemberInfo");

                //設定多值
                cMemberInfo.Values.Add("MemberID", Cryptograph.MD5Encrypt(MemberID.ToString(), Application["DesKey"].ToString()));    //會員編號
                cMemberInfo.Values.Add("MemberAcct", Cryptograph.MD5Encrypt(MemberAcct, Application["DesKey"].ToString()));   //會員帳號
                cMemberInfo.Values.Add("MemberID_ChgPwd", MemberID.ToString());    //變更密碼使用
                cMemberInfo.Values.Add("MemberName", Cryptograph.MD5Encrypt(MemberName, Application["DesKey"].ToString()));   //會員姓名
                cMemberInfo.Values.Add("MemberType", MemberType.ToString());   //會員型態
                cMemberInfo.Values.Add("MemberIsWrite", IsWrite.ToString());   //是否已填資料
                cMemberInfo.Values.Add("Token", TokenID);   //會員Token

                //判斷是否要記住帳號
                if (this.cb_Remember.Checked)
                {
                    cMemberInfo.Values.Add("RememberMe", "Y");  //記住我
                    //設定到期日(2週=336hr)
                    cMemberInfo.Expires = DateTime.Now.AddHours(336);
                }
                else
                {
                    cMemberInfo.Values.Add("RememberMe", "N");  //記住我(N)
                    //不設定到期日, 基本上瀏覽器關閉就會消失
                }

                //寫到用戶端
                Response.Cookies.Add(cMemberInfo);

                #endregion


                //導向指定網址
                Response.Redirect(GetUrl());
                break;

        }
    }


    /// <summary>
    /// 判斷是否為良興會員
    /// 1.先執行良興會員API,儲存會員資料
    /// 2.查詢EcLife_MemberData
    /// </summary>
    /// <param name="email"></param>
    /// <param name="inputPwd"></param>
    /// <returns></returns>
    private int Get_EcLifeMemberID(string email, string inputPwd, out string ErrMsg)
    {
        //觸發會員同步API
        if (false == Sync_EcLifeMember(out ErrMsg))
        {
            //失敗時發送Email
            #region - 寄EMail -

            //[設定參數] - 建立者(20字)
            fn_Mail.Create_Who = "PKWeb-System";

            //[設定參數] - 來源程式/功能
            fn_Mail.FromFunc = "官網-前台, EcLife會員同步";

            //[設定參數] - 寄件人
            fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

            //[設定參數] - 寄件人顯示名稱
            fn_Mail.SenderName = "Pro'sKit EcLife會員同步";

            //[設定參數] - 收件人
            List<string> emailTo = new List<string>();
            emailTo.Add("mis@mail.prokits.com.tw");

            fn_Mail.Reciever = emailTo;

            //[設定參數] - 轉寄人群組
            fn_Mail.CC = null;

            //[設定參數] - 密件轉寄人群組
            fn_Mail.BCC = null;

            //[設定參數] - 郵件主旨
            fn_Mail.Subject = "EcLife會員同步失敗，請檢查程式或良興API(PKWeb)";

            //[設定參數] - 郵件內容
            StringBuilder mailBody = new StringBuilder();
            //內容主體
            mailBody.Append("<h2>EcLife會員同步失敗，請檢查官網會員登入程式</h2><br>from PKWeb....Login.aspx<hr>function=Sync_EcLifeMember<hr><h4>[詳細說明]</h4>{0}<hr>{1}"
                .FormatThis(ErrMsg, email));

            fn_Mail.MailBody = mailBody;

            //[設定參數] - 指定檔案 - 路徑
            fn_Mail.FilePath = "";

            //[設定參數] - 指定檔案 - 檔名
            fn_Mail.FileName = "";

            //發送郵件
            fn_Mail.SendMail();

            #endregion
        }

        //資料查詢
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT cno, UPPER(password) AS password FROM EcLife_MemberData WITH(NOLOCK)");
            SBSql.AppendLine(" WHERE (UPPER(Email) = UPPER(@Email)) AND (Sync_Status = 'N')");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Email", email);

            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    ErrMsg = "204";
                    return 0;
                }
                else
                {
                    int memberID = Convert.ToInt32(DT.Rows[0]["cno"]);
                    string checkPwd = DT.Rows[0]["password"].ToString().ToUpper();
                    if (checkPwd.Equals(inputPwd.ToUpper()))
                    {
                        return memberID;
                    }
                    else
                    {
                        ErrMsg = "401";
                        return 0;
                    }
                }
            }
        }
    }


    /// <summary>
    /// 觸發良興會員同步
    /// 將良興會員資料存到EcLife_MemberData
    /// </summary>
    /// <returns></returns>
    private bool Sync_EcLifeMember(out string ErrMsg)
    {
        try
        {
            //Key & Url
            string apiKey = fn_Param.EcLife_ApiKey;
            string url = "https://api-proskit.eclife.com.tw/v1/getMember/";
            string token = fn_Param.EcLife_Token;

            //Post Data
            Dictionary<string, string> postHeaders = new Dictionary<string, string>();
            Dictionary<string, string> postParams = new Dictionary<string, string>();

            //查詢2個月內的良興會員
            postHeaders.Add("token", token);
            postParams.Add("sdate", DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd"));
            postParams.Add("edate", DateTime.Now.ToString("yyyy/MM/dd"));

            //Get Api Response
            string result = fn_Extensions.WebRequest_POST(false, url, postParams, postHeaders);

            //解析Json
            JObject jData;

            try
            {
                jData = JObject.Parse(result);
            }
            catch (Exception ex)
            {
                //回傳格式有誤
                ErrMsg = "良興API回傳Json格式錯誤," + ex.Message.ToString();
                return false;
            }


            //Get Count
            string sDate = jData["data"]["sdate"].ToString();
            string eDate = jData["data"]["edate"].ToString();
            string MemberCnt = jData["data"]["count"].ToString();

            if (MemberCnt.Equals("0"))
            {
                //查無資料, 不做動作
                ErrMsg = "";
                return true;
            }

            //decode MemberData
            string MemberData = Cryptograph.Decrypt3DES(jData["data"]["members"].ToString(), apiKey);

            //將指定內容轉化為JArray
            JArray aryObj = JArray.Parse(MemberData);

            //Get Data Columns
            var members = aryObj
                .Select(i => new
                {
                    cno = i["cno"].ToString(),
                    ModifyDate = i["ModifyDate"].ToString(),
                    RegisterDate = i["RegisterDate"].ToString(),
                    Name = i["Name"].ToString(),
                    Email = i["Email"].ToString(),
                    Mobile = i["Mobile"].ToString(),
                    Sex = i["Sex"].ToString(),
                    password = i["password"].ToString(),
                    Birthday = i["Birthday"].ToString(),
                    LiveCityName = i["LiveCityName"].ToString(),
                    LiveStateName = i["LiveStateName"].ToString(),
                    LiveState = i["LiveState"].ToString(),
                    LiveAddress = i["LiveAddress"].ToString()
                });


            #region -- Insert MemberData --

            try
            {
                //Insert Data to EcLife_MemberData
                StringBuilder sql = new StringBuilder();

                using (SqlCommand cmd = new SqlCommand())
                {
                    foreach (var member in members)
                    {
                        sql.AppendLine(string.Format(" IF (SELECT COUNT(*) FROM EcLife_MemberData WHERE cno = {0}) = 0", member.cno));

                        //---- Insert ----
                        sql.AppendLine(" BEGIN ");
                        sql.Append(" INSERT INTO EcLife_MemberData(");
                        sql.Append("  cno, ModifyDate, RegisterDate, Name, Email, Mobile, Sex");
                        sql.Append("  , password, Birthday, LiveCityName, LiveStateName, LiveState, LiveAddress");
                        sql.Append(" ) VALUES(");
                        sql.Append(string.Format("  {0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}'",
                                member.cno
                                , string.IsNullOrEmpty(member.ModifyDate) || member.ModifyDate.Equals("null") ? "" : member.ModifyDate
                                , string.IsNullOrEmpty(member.RegisterDate) ? "" : member.RegisterDate
                                , string.IsNullOrEmpty(member.Name) || member.Name.Equals("null") ? "" : member.Name
                                , member.Email
                                , string.IsNullOrEmpty(member.Mobile) || member.Mobile.Equals("null") ? "" : member.Mobile
                                , member.Sex
                            ));
                        sql.Append(string.Format("  , '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'",
                                member.password
                                , string.IsNullOrEmpty(member.Birthday) || member.Birthday.Equals("null") ? "" : member.Birthday
                                , string.IsNullOrEmpty(member.LiveCityName) || member.LiveCityName.Equals("null") ? "" : member.LiveCityName
                                , string.IsNullOrEmpty(member.LiveStateName) || member.LiveStateName.Equals("null") ? "" : member.LiveStateName
                                , string.IsNullOrEmpty(member.LiveState) || member.LiveState.Equals("null") ? "" : member.LiveState
                                , string.IsNullOrEmpty(member.LiveAddress) || member.LiveAddress.Equals("null") ? "" : member.LiveAddress
                            ));
                        sql.Append(" );");
                        sql.AppendLine(" END ");

                        sql.AppendLine(" ELSE ");

                        //---- Update ----
                        sql.AppendLine(" BEGIN ");
                        sql.Append(" UPDATE EcLife_MemberData SET ");
                        sql.Append(string.Format("  ModifyDate = '{0}', RegisterDate = '{1}', Name = '{2}', Email = '{3}', Mobile = '{4}', Sex = '{5}'",
                                string.IsNullOrEmpty(member.ModifyDate) || member.ModifyDate.Equals("null") ? "" : member.ModifyDate
                                , string.IsNullOrEmpty(member.RegisterDate) || member.RegisterDate.Equals("null") ? "" : member.RegisterDate
                                , string.IsNullOrEmpty(member.Name) || member.Name.Equals("null") ? "" : member.Name
                                , member.Email
                                , string.IsNullOrEmpty(member.Mobile) || member.Mobile.Equals("null") ? "" : member.Mobile
                                , member.Sex
                            ));
                        sql.Append(string.Format("  , password = '{0}', Birthday = '{1}', LiveCityName = '{2}', LiveStateName = '{3}', LiveState = '{4}', LiveAddress = '{5}'",
                                member.password
                                , string.IsNullOrEmpty(member.Birthday) || member.Birthday.Equals("null") ? "" : member.Birthday
                                , string.IsNullOrEmpty(member.LiveCityName) || member.LiveCityName.Equals("null") ? "" : member.LiveCityName
                                , string.IsNullOrEmpty(member.LiveStateName) || member.LiveStateName.Equals("null") ? "" : member.LiveStateName
                                , string.IsNullOrEmpty(member.LiveState) || member.LiveState.Equals("null") ? "" : member.LiveState
                                , string.IsNullOrEmpty(member.LiveAddress) || member.LiveAddress.Equals("null") ? "" : member.LiveAddress
                            ));
                        sql.Append(" , Update_Time = GETDATE()");
                        sql.Append(string.Format(" WHERE (cno = {0}) ", member.cno));
                        sql.AppendLine(" END ");

                    }

                    //執行SQL
                    cmd.CommandText = sql.ToString();
                    if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                ErrMsg = "SQL更新時出錯, {0}...詳細資料:<br><br>{1}".FormatThis(ex.Message.ToString(), result);
                return false;
            }
            #endregion


        }
        catch (Exception ex)
        {
            ErrMsg = "未預期的錯誤發生..." + ex.Message.ToString();
            return false;
        }

    }

    /// <summary>
    /// 重新寄送驗證信 - 帳號未啟用
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
            string ValidUrl = "{0}Register/1/{1}".FormatThis(
                    Application["WebUrl"].ToString()
                    , TokenID
                );

            //更新Token - 註冊驗證
            if (!UpdateAccountToken(MemberID, TokenID, TimeoutTS, 1))
            {
                //token 更新失敗
                return false;
            }

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
    /// 更新Member Token - 註冊驗證=1 / 登入使用=3
    /// </summary>
    /// <param name="memberID">會員ID</param>
    /// <param name="tokenID">TokenID</param>
    /// <param name="ts">到期時間戳記</param>
    /// <returns></returns>
    private bool UpdateAccountToken(Int64 memberID, string tokenID, Int64 ts, Int16 actType)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //清除參數
            cmd.Parameters.Clear();

            //[SQL] - 刪除舊驗證資料
            SBSql.AppendLine(" DELETE FROM Member_Token ");
            SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID) AND (ActType = @ActType); ");

            //[SQL] - 資料處理, 新增驗證資料
            SBSql.AppendLine(" INSERT INTO Member_Token( ");
            SBSql.AppendLine("  Mem_ID, TokenID, TimeoutTS, ActType");
            SBSql.AppendLine(" ) VALUES (");
            SBSql.AppendLine("  @Mem_ID, @TokenID, @TimeoutTS, @ActType");
            SBSql.AppendLine(" );");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Mem_ID", memberID);
            cmd.Parameters.AddWithValue("TokenID", tokenID);
            cmd.Parameters.AddWithValue("TimeoutTS", ts);
            cmd.Parameters.AddWithValue("ActType", actType);

            return dbConn.ExecuteSql(cmd, out ErrMsg);

        }
    }



    /// <summary>
    /// 取得要回傳的網址
    /// </summary>
    /// <returns></returns>
    private string GetUrl()
    {
        //判斷是否已填寫基本資料
        //成立 = 未填寫資料 & 非活動流程
        if (fn_Param.IsWrite.Equals("N") && string.IsNullOrEmpty(Req_Code))
        {
            return "{0}MemberData".FormatThis(Application["WebUrl"].ToString());
        }

        //判斷是否有活動代碼
        if (!string.IsNullOrEmpty(Req_Code))
        {
            //導至抽獎活動頁
            return "{0}Lottery/{1}".FormatThis(Application["WebUrl"].ToString(), HttpUtility.UrlEncode(Req_Code));
        }
        //判斷是否有上一頁網址
        else if (!string.IsNullOrEmpty(Req_LastUrl))
        {
            return Req_LastUrl;
        }
        else
        {
            return Application["WebUrl"].ToString();
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


    #region -- 參數設定 --

    /// <summary>
    /// 取得傳遞參數 - 上一頁網址
    /// </summary>
    private string _Req_LastUrl;
    public string Req_LastUrl
    {
        get
        {
            //上一頁網址不存在= 帶首頁Url, 存在= 解密
            String url = string.IsNullOrEmpty(Request.QueryString["u"])
                ? Application["WebUrl"].ToString()
                : Cryptograph.MD5Decrypt(Request.QueryString["u"].ToString(), Application["DesKey"].ToString());

            //判斷是否為正確的網址
            if (false == fn_Extensions.IsUrl(url)) url = Application["WebUrl"].ToString();

            return url;
        }
        set
        {
            this._Req_LastUrl = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - 活動code
    /// </summary>
    private string _Req_Code;
    public string Req_Code
    {
        get
        {
            string GetCode = fn_stringFormat.Set_FilterHtml(Request.QueryString["code"]);

            return GetCode;
        }
        set
        {
            this._Req_Code = value;
        }
    }

    #endregion

}