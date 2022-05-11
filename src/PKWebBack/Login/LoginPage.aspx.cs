using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class LoginPage : System.Web.UI.Page
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if ((Request.Cookies["PK_Remember"] != null && Request.Cookies["PK_UserSID"] != null))
                {
                    this.cb_remember.Checked = true;
                    this.tb_UserID.Text = Request.Cookies["PK_UserSID"].Value.ToString();
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 按鈕事件 --
    /// <summary>
    /// 登入
    /// </summary>
    protected void lbtn_Login_Click(object sender, EventArgs e)
    {
        //判斷是否使用內部AD驗證
        string IsAD = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_IsUse"];
        if (IsAD.ToUpper().Equals("Y"))
        {
            #region ***AD帳戶驗證***
            //[設定參數] - LDAP路徑
            string adPath = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_Path"];
            //[設定參數] - 網域名稱
            string domainName = "prokits";

            LdapAuthentication adAuth = new LdapAuthentication(adPath);

            //[取得參數] - 使用者帳密
            string UserName = fn_stringFormat.Set_FilterString(this.tb_UserID.Text.Trim());
            string UserPwd = fn_stringFormat.Set_FilterString(this.tb_UserPwd.Text.Trim());

            //[AD驗證]
            if (true == adAuth.IsAuthenticated(domainName, UserName, UserPwd))
            {
                //取得SID
                string SID = adAuth.GetSID;
                if (string.IsNullOrEmpty(SID))
                {
                    //SID,抓不到,導向失敗頁
                    Response.Redirect("{0}ServiceLoginFail/1002/".FormatThis(Application["WebUrl"]));
                    return;
                }

                //[暫存參數] - 新增Cookie, 存入SID(設定過期時間為 4 小時)
                Response.Cookies.Add(new HttpCookie("PK_UserSID", SID));
                Response.Cookies["PK_UserSID"].Expires = DateTime.Now.AddHours(4);

                //登入成功，導向首頁
                Response.Redirect(Application["WebUrl"].ToString());
                return;
            }
            else
            {
                //帳密有誤
                Response.Redirect("{0}ServiceLoginFail/999/".FormatThis(Application["WebUrl"]));
                return;
            }
            #endregion
        }
        else
        {
            #region ***一般帳戶驗證***

            //[取得參數] - 使用者帳密
            string UserName = fn_stringFormat.Set_FilterString(this.tb_UserID.Text.Trim());
            string UserPwd = fn_stringFormat.Set_FilterString(this.tb_UserPwd.Text.Trim());

            if (false == Check_Account(UserName, UserPwd, out ErrMsg))
            {
                //登入失敗
                Response.Redirect("{0}ServiceLoginFail/{1}/".FormatThis(Application["WebUrl"], ErrMsg));
                return;
            }
            else
            {
                //登入成功，導向首頁
                Response.Redirect(Application["WebUrl"].ToString());
                return;
            }

            #endregion
        }
    }

    /// <summary>
    /// 驗證帳號, 一般用戶
    /// </summary>
    /// <param name="UserID">帳號</param>
    /// <param name="UserPwd">密碼</param>
    /// <returns></returns>
    private bool Check_Account(string UserID, string UserPwd, out string ErrMsg)
    {
        try
        {
            //[參數宣告] - SqlCommand
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 帳號查詢
                SBSql.AppendLine(" SELECT Prof.Guid, Prof.Account_Name, Prof.Display_Name, Prof.Display, Rel.Group_Guid ");
                SBSql.AppendLine(" FROM User_Profile Prof ");
                SBSql.AppendLine("  INNER JOIN User_Group_Rel_Profile Rel ON Prof.Guid = Rel.Profile_Guid ");
                SBSql.AppendLine("  INNER JOIN User_Group GP ON Rel.Group_Guid = GP.Guid ");
                SBSql.AppendLine(" WHERE (Prof.Account_Name = @UserID) AND (Prof.Account_Pwd = @UserPwd) ");
                SBSql.AppendLine("  AND (GP.Display = 'Y')");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UserID", UserID);
                cmd.Parameters.AddWithValue("UserPwd", Cryptograph.MD5(UserPwd).ToUpper());
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //判斷是否有資料
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "2001";
                        return false;
                    }
                    //判斷是否被停用
                    if (DT.Rows[0]["Display"].Equals("N"))
                    {
                        //登入失敗 - 帳號已停用
                        ErrMsg = "2002";
                        return false;
                    }

                    //[登入成功，儲存Session]
                    //取得登入名稱
                    HttpContext.Current.Session["Login_UserName"] = DT.Rows[0]["Display_Name"];
                    //取得登入帳號
                    HttpContext.Current.Session["Login_UserID"] = DT.Rows[0]["Account_Name"];
                    //取得AD GUID
                    HttpContext.Current.Session["Login_GUID"] = DT.Rows[0]["Guid"];

                    //取得登入者所屬群組(ArrayList)
                    ArrayList aryGroup = new ArrayList();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        aryGroup.Add(DT.Rows[row]["Group_Guid"].ToString());
                    }
                    HttpContext.Current.Session["Login_UserGroups"] = aryGroup;

                    //判斷是否勾選"記住我"
                    if (this.cb_remember.Checked)
                    {
                        //[暫存參數] - 新增Cookie, 存入帳號(設定過期時間為 7 天)
                        Response.Cookies.Add(new HttpCookie("PK_UserSID", Session["Login_UserID"].ToString()));
                        Response.Cookies["PK_UserSID"].Expires = DateTime.Now.AddDays(7);

                        Response.Cookies.Add(new HttpCookie("PK_Remember", "Y"));
                        Response.Cookies["PK_Remember"].Expires = DateTime.Now.AddDays(7);

                    }
                    else
                    {
                        //清除Cookie
                        HttpCookie myCookie = new HttpCookie("PK_UserSID");
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);

                        HttpCookie myCookie_Remb = new HttpCookie("PK_Remember");
                        myCookie_Remb.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie_Remb);
                    }

                    return true;
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("登入失敗");
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}ServiceLogin/".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    #endregion
}