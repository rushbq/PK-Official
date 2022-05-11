using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ExtensionMethods;

/// <summary>
/// 會員功能
/// </summary>
public class fn_Member
{
    protected static string ErrMsg;

    /// <summary>
    /// 會員資料 - 判斷帳號是否可以使用
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static bool CheckAccount(string account)
    {
        try
        {
            if (string.IsNullOrEmpty(account))
            {
                return false;
            }

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
    /// 判斷是否使用社群登入
    /// </summary>
    /// <param name="socialID"></param>
    /// <returns></returns>
    public static bool CheckSocialAccount(string socialID)
    {
        try
        {
            if (string.IsNullOrEmpty(socialID))
            {
                return false;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Mem_ID, Social_ID ");
                sbSQL.AppendLine(" FROM Member_SocialToken WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Social_ID = @Social_ID) ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Social_ID", socialID);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //非社群登入
                        return false;
                    }
                    else
                    {
                        //社群登入
                        return true;
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
    public static bool CreateAccount(string account, string pwd, string tokenID, Int64 ts)
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
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(Mem_ID) ,0) + 1 FROM Member_Data ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增會員資料
                SBSql.AppendLine(" INSERT INTO Member_Data( ");
                SBSql.AppendLine("  Mem_ID, Mem_Account, Mem_Pwd");
                SBSql.AppendLine("  , Display, Create_Time");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @Mem_Account, @Mem_Pwd");
                SBSql.AppendLine("  , 'S', GETDATE()");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增驗證資料
                SBSql.AppendLine(" INSERT INTO Member_Token( ");
                SBSql.AppendLine("  Mem_ID, TokenID, TimeoutTS, ActType");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @TokenID, @TimeoutTS, 1");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_Account", account.ToLower());
                cmd.Parameters.AddWithValue("Mem_Pwd", Cryptograph.MD5(pwd).ToLower());
                cmd.Parameters.AddWithValue("TokenID", tokenID);
                cmd.Parameters.AddWithValue("TimeoutTS", ts);

                return dbConn.ExecuteSql(cmd, out ErrMsg);

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 建立帳號");
        }
    }

    public static bool AccountProcess(string userId, string email, string first_name, string last_name
        , string locale, string token, string platform, string type)
    {
        int MemID;
        switch (type.ToUpper())
        {
            case "CREATE":
                //建立帳號, 取得新編號
                MemID = CreateAccount_With_Social(userId, email, first_name, last_name, locale, token, platform);
                if (MemID.Equals(0))
                {
                    return false;
                }

                //執行登入
                AutoLogin(MemID.ToString(), last_name, email);

                return true;

            case "UPDATE":
                //更新帳號, 取得新編號
                MemID = UpdateAccount_With_Social(userId, email, first_name, last_name, locale, token);
                if (MemID.Equals(0))
                {
                    return false;
                }

                //執行登入
                AutoLogin(MemID.ToString(), last_name, email);

                return true;

            default:
                return false;
        }



    }

    /// <summary>
    /// 會員資料 -建立帳號(來自社群登入)
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <param name="first_name"></param>
    /// <param name="last_name"></param>
    /// <param name="locale"></param>
    /// <param name="token"></param>
    /// <param name="platform"></param>
    /// <returns></returns>
    private static int CreateAccount_With_Social(string userId, string email, string first_name, string last_name
        , string locale, string token, string platform)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return 0;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                int NewID;

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(Mem_ID) ,0) + 1 FROM Member_Data ");
                SBSql.AppendLine(" );");
                SBSql.AppendLine(" SELECT @NewID AS NewID");

                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    NewID = Convert.ToInt32(DT.Rows[0]["NewID"]);
                }


                //--- 開始新增資料 ---
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 資料處理, 新增會員資料
                SBSql.AppendLine(" INSERT INTO Member_Data( ");
                SBSql.AppendLine("  Mem_ID, Mem_Account, Mem_Pwd");
                SBSql.AppendLine("  , Country_Code, LastName, FirstName");
                SBSql.AppendLine("  , Display, Create_Time");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @Mem_ID, @Mem_Account, @Mem_Pwd");
                SBSql.AppendLine("  , @Country_Code, @LastName, @FirstName");
                SBSql.AppendLine("  , 'Y', GETDATE()");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增社群資料
                SBSql.AppendLine(" INSERT INTO Member_SocialToken( ");
                SBSql.AppendLine("  Mem_ID, Social_ID, Platform, Token");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @Mem_ID, @Social_ID, @Platform, @Token");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", NewID);
                cmd.Parameters.AddWithValue("Mem_Account", email.ToLower());
                cmd.Parameters.AddWithValue("Mem_Pwd", Cryptograph.MD5("iLoveProkits").ToLower());
                cmd.Parameters.AddWithValue("Country_Code", string.IsNullOrEmpty(locale) ? "" : locale.Right(2).ToUpper());
                cmd.Parameters.AddWithValue("LastName", last_name);
                cmd.Parameters.AddWithValue("FirstName", first_name);
                cmd.Parameters.AddWithValue("Social_ID", userId);
                cmd.Parameters.AddWithValue("Platform", platform);
                cmd.Parameters.AddWithValue("Token", token);

                if (dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    return NewID;
                }
                else
                {
                    return 0;
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 建立帳號");
        }
    }


    private static int UpdateAccount_With_Social(string userId, string email, string first_name, string last_name
     , string locale, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return 0;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                int GetMemID;

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @GetMemID AS INT ");
                SBSql.AppendLine(" SET @GetMemID = (");
                SBSql.AppendLine("  SELECT Mem_ID FROM Member_SocialToken WHERE (Social_ID = @Social_ID)");
                SBSql.AppendLine(" );");
                SBSql.AppendLine(" SELECT @GetMemID AS GetMemID");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Social_ID", userId);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    GetMemID = Convert.ToInt32(DT.Rows[0]["GetMemID"]);
                }


                //--- 開始新增資料 ---
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 資料處理, 修改會員資料
                SBSql.AppendLine(" UPDATE Member_Data SET ");
                SBSql.AppendLine("  LastName = @LastName, FirstName = @FirstName");
                SBSql.AppendLine("  , Country_Code = @Country_Code, Update_Time = GETDATE()");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID);");

                //[SQL] - 資料處理, 修改社群資料
                SBSql.AppendLine(" UPDATE Member_SocialToken SET Token = @Token");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID) AND (Social_ID = @Social_ID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", GetMemID);
                cmd.Parameters.AddWithValue("Country_Code", locale.Right(2).ToUpper());
                cmd.Parameters.AddWithValue("LastName", last_name);
                cmd.Parameters.AddWithValue("FirstName", first_name);
                cmd.Parameters.AddWithValue("Social_ID", userId);
                cmd.Parameters.AddWithValue("Token", token);

                if (dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    return GetMemID;
                }
                else
                {
                    return 0;
                }

            }
        }
        catch (Exception)
        {
            //throw new Exception("系統發生錯誤 - 更新帳號");
            throw;
        }
    }


    /// <summary>
    /// 自動登入
    /// </summary>
    /// <param name="memberID">會員編號</param>
    /// <param name="memberName">會員名</param>
    private static void AutoLogin(string memberID, string memberName, string email)
    {
        //- 取得目前TS -
        Int64 CurrTS = Cryptograph.GetCurrentTime();
        //- 取得TokenID -
        string TokenID = fn_Extensions.GetTokenID(CurrTS);
        //- 取得到期時間戳記(2週=336hr) -
        Int64 TimeoutTS = fn_Extensions.GetTimeoutTS(CurrTS, 336);


        //先清除Cookie
        HttpCookie myCookie = new HttpCookie("PKWeb_MemberInfo");
        myCookie.Expires = DateTime.Now.AddDays(-1d);
        HttpContext.Current.Response.Cookies.Add(myCookie);

        //產生Cookie
        HttpCookie cMemberInfo = new HttpCookie("PKWeb_MemberInfo");

        string desKey = HttpContext.Current.Application["DesKey"].ToString();

        //設定多值
        cMemberInfo.Values.Add("MemberID", Cryptograph.MD5Encrypt(memberID, desKey));    //會員編號
        cMemberInfo.Values.Add("MemberAcct", Cryptograph.MD5Encrypt(email, desKey));   //會員帳號
        cMemberInfo.Values.Add("MemberID_ChgPwd", "");    //變更密碼使用
        cMemberInfo.Values.Add("MemberName", Cryptograph.MD5Encrypt(memberName, desKey));   //會員姓名
        cMemberInfo.Values.Add("MemberType", "0");   //會員型態-一般會員
        cMemberInfo.Values.Add("RememberMe", "Y");  //記住我
        cMemberInfo.Values.Add("MemberIsWrite", "N");   //是否已填資料
        cMemberInfo.Values.Add("Token", TokenID);   //Member Token

        //設定到期日(3個月)
        cMemberInfo.Expires = DateTime.Now.AddMonths(3);

        //寫到用戶端
        HttpContext.Current.Response.Cookies.Add(cMemberInfo);


        //更新Token資料庫
        if (!UpdateAccountToken(Convert.ToInt64(memberID), TokenID, TimeoutTS, 3))
        {
            //登入失敗
            //Response.Redirect(GoUrl("6?code=" + HttpUtility.UrlEncode(Req_Code)));
        }

    }


    /// <summary>
    /// 更新Member Token - 註冊驗證=1 / 登入使用=3
    /// </summary>
    /// <param name="memberID">會員ID</param>
    /// <param name="tokenID">TokenID</param>
    /// <param name="ts">到期時間戳記</param>
    /// <returns></returns>
    private static bool UpdateAccountToken(Int64 memberID, string tokenID, Int64 ts, Int16 actType)
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
    /// 取得經銷商編號
    /// </summary>
    /// <param name="memberID"></param>
    /// <returns></returns>
    public static string GetDealerID(string memberID)
    {
        try
        {
            if (string.IsNullOrEmpty(memberID))
            {
                return "";
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT DealerID ");
                SBSql.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Mem_Type = 1) AND (Mem_ID = @Mem_ID) ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", memberID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return DT.Rows[0]["DealerID"].ToString();
                    }
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 取得經銷商編號");
        }
    }


    /// <summary>
    /// 取得會員帳號
    /// </summary>
    /// <param name="memberID"></param>
    /// <returns></returns>
    public static string GetMemberAccount(string memberID)
    {
        try
        {
            if (string.IsNullOrEmpty(memberID))
            {
                return "";
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Mem_Account ");
                SBSql.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (Mem_ID = @Mem_ID) ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", memberID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return DT.Rows[0]["Mem_Account"].ToString();
                    }
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 取得經銷商編號");
        }
    }
}