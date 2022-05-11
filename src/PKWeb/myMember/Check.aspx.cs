using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

/// <summary>
/// 會員認證信驗證頁
/// </summary>
public partial class Check : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //檢查參數是否空白
                if (string.IsNullOrEmpty(Req_Token) || string.IsNullOrEmpty(Req_Mode))
                {
                    Response.Redirect(GoUrl("1"));
                    return;

                }

                //宣告
                Int64 GetUserID;

                //判斷類型
                switch (Req_Mode)
                {
                    case "1":
                        //1:會員註冊
                        GetUserID = CheckUser();

                        if (GetUserID.Equals(0))
                        {
                            //驗證失敗
                            Response.Redirect(GoUrl("99"));
                        }
                        else
                        {
                            //啟用會員
                            if (ActivateUser(GetUserID))
                            {
                                //成功, 觸發良興同步
                                string email = fn_Member.GetMemberAccount(GetUserID.ToString());
                                fn_Param.SetEclifeMemberStatus("", "", email);

                                //導至訊息頁
                                Response.Redirect(GoUrl("5"));
                            }
                            else
                            {
                                //失敗
                                Response.Redirect(GoUrl("1"));
                            }
                        }

                        break;


                    case "2":
                        //2:變更密碼
                        GetUserID = CheckUser();

                        if (GetUserID.Equals(0))
                        {
                            //驗證失敗
                            Response.Redirect(GoUrl("99"));
                        }
                        else
                        {
                            //把會員編號暫存至Cookie, 提供給變更密碼功能使用
                            #region -- Cookie處理 --

                            //產生Cookie
                            HttpCookie cMemberInfo = new HttpCookie("PKWeb_MemberInfo");

                            //設定多值
                            cMemberInfo.Values.Add("MemberID_ChgPwd", GetUserID.ToString());    //變更密碼使用

                            //寫到用戶端
                            Response.Cookies.Add(cMemberInfo);

                            #endregion


                            //前往密碼變更頁
                            Response.Redirect("{0}ChangePwd".FormatThis(Application["WebUrl"]));
                        }


                        break;


                    default:
                        Response.Redirect(GoUrl("1"));
                        break;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// 會員註冊 - 驗證
    /// </summary>
    /// <returns></returns>
    private Int64 CheckUser()
    {
        try
        {
            /*
             * 1) 判斷TS是否到期
             * 2) 取得會員ID
             */
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" DECLARE @MemID AS Bigint ");
                sbSQL.AppendLine(" SET @MemID = ( ");
                sbSQL.AppendLine("     SELECT Mem_ID ");
                sbSQL.AppendLine("     FROM Member_Token ");
                sbSQL.AppendLine("     WHERE (TokenID = @TokenID) AND (ActType = @ActType) AND (IsUse = 'N') AND (@CurrTS <= TimeoutTS) ");
                sbSQL.AppendLine(" ) ");
                sbSQL.AppendLine(" IF @MemID IS NOT NULL ");
                sbSQL.AppendLine("  BEGIN ");
                sbSQL.AppendLine("    UPDATE Member_Token ");
                sbSQL.AppendLine("    SET IsUse = 'Y' ");
                sbSQL.AppendLine("    WHERE (Mem_ID = @MemID) AND (ActType = @ActType) ");
                sbSQL.AppendLine("  END ");

                sbSQL.AppendLine(" SELECT @MemID AS MemberID ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TokenID", Req_Token);
                cmd.Parameters.AddWithValue("ActType", Req_Mode);
                cmd.Parameters.AddWithValue("CurrTS", Cryptograph.GetCurrentTime());

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows[0]["MemberID"] == DBNull.Value)
                    {
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt64(DT.Rows[0]["MemberID"]);
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
    /// 會員註冊 - 啟用帳號
    /// </summary>
    /// <returns></returns>
    private bool ActivateUser(Int64 memID)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" UPDATE Member_Data ");
                sbSQL.AppendLine(" SET Display = 'Y' ");
                sbSQL.AppendLine(" WHERE (Mem_ID = @Mem_ID) ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Mem_ID", memID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }
        catch (Exception)
        {

            throw;
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
    /// 取得傳遞參數 - Token ID
    /// </summary>
    private string _Req_Token;
    public string Req_Token
    {
        get
        {
            String GetReq = Page.RouteData.Values["token"].ToString();

            return string.IsNullOrEmpty(GetReq) ? "" : fn_stringFormat.Set_FilterHtml(GetReq);
        }
        set
        {
            this._Req_Token = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - 檢查類型
    /// </summary>
    private string _Req_Mode;
    public string Req_Mode
    {
        get
        {
            String GetReq = Page.RouteData.Values["mode"].ToString();

            return string.IsNullOrEmpty(GetReq) ? "" : fn_stringFormat.Set_FilterHtml(GetReq);
        }
        set
        {
            this._Req_Mode = value;
        }
    }

    #endregion
}