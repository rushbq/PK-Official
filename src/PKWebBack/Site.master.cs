using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class Site : MasterPage, IProgID
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // 下面的程式碼有助於防禦 XSRF 攻擊
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // 使用 Cookie 中的 Anti-XSRF 權杖
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // 產生新的防 XSRF 權杖並儲存到 cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 設定 Anti-XSRF 權杖
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // 驗證 Anti-XSRF 權杖
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Anti-XSRF 權杖驗證失敗。");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string ErrMsg;

            //取得左側選單
            this.lt_Menu.Text = CreateMenu(out ErrMsg);
        }
    }

    #region -- 資料讀取 --
    /// <summary>
    /// [建立選單] - 第一層
    /// </summary>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private string CreateMenu(out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder SBHtml = new StringBuilder();

                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();

                //[SQL] - 判斷(是否有使用者權限)
                SBSql.AppendLine("DECLARE @RowCnt AS INT ");
                SBSql.AppendLine("SET @RowCnt = ( ");
                SBSql.AppendLine("	SELECT COUNT(*) ");
                SBSql.AppendLine("	FROM Program INNER JOIN User_Profile_Rel_Program UserRel ON Program.Prog_ID = UserRel.Prog_ID ");
                SBSql.AppendLine("	WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') AND (Program.MenuDisplay = 'Y') AND (UserRel.Guid = @UserGUID) ");
                SBSql.AppendLine(")  ");
                SBSql.AppendLine("IF @RowCnt <> 0 ");
                //[SQL] - 顯示權限(使用者)
                SBSql.AppendLine("	BEGIN ");
                SBSql.AppendLine("	    SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.CssStyle, Program.Child_Cnt ");
                //[SQL] - 判斷&顯示(目前語系)
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine("		FROM Program INNER JOIN User_Profile_Rel_Program UserRel ON Program.Prog_ID = UserRel.Prog_ID ");
                SBSql.AppendLine("		WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') AND (Program.MenuDisplay = 'Y') AND (UserRel.Guid = @UserGUID) ");
                SBSql.AppendLine("      ORDER BY Program.Sort, Program.Prog_ID ");
                SBSql.AppendLine("	END ");
                SBSql.AppendLine("ELSE ");
                //[SQL] - 顯示權限(群組)
                SBSql.AppendLine("	BEGIN ");
                SBSql.AppendLine("   SELECT TbGroup.* FROM (");
                SBSql.AppendLine("	    SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.CssStyle, Program.Child_Cnt ");
                //[SQL] - 判斷&顯示(目前語系)
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine("		FROM Program INNER JOIN User_Group_Rel_Program GroupRel ON Program.Prog_ID = GroupRel.Prog_ID ");
                SBSql.AppendLine("		WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') AND (Program.MenuDisplay = 'Y') ");

                #region >>群組參數組合<<
                //[取得參數] - 所屬群組
                ArrayList Groups = (ArrayList)Session["Login_UserGroups"];
                //[SQL] - 暫存參數
                string tempParam = "";
                for (int i = 0; i < Groups.Count; i++)
                {
                    if (string.IsNullOrEmpty(tempParam) == false) { tempParam += ","; }
                    tempParam += "@ParamTmp" + i;
                }
                //[SQL] - 代入暫存參數
                SBSql.AppendLine(" AND (GroupRel.Guid IN (" + tempParam + "))");
                for (int i = 0; i < Groups.Count; i++)
                    cmd.Parameters.AddWithValue("ParamTmp" + i, Groups[i]);
                #endregion

                SBSql.AppendLine("   ) AS TbGroup");
                SBSql.AppendLine("   GROUP BY Prog_ID, Prog_Link, Sort, CssStyle, Prog_Name, Child_Cnt ");
                SBSql.AppendLine("   ORDER BY Sort, Prog_ID ");
                SBSql.AppendLine("	END ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("UserGUID", Session["Login_GUID"].ToString());
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //取得參數
                        int Child_Cnt = Convert.ToInt16(DT.Rows[row]["Child_Cnt"]);
                        string CssStyle = DT.Rows[row]["CssStyle"].ToString();
                        string ProgID = DT.Rows[row]["Prog_ID"].ToString();
                        string ProgName = DT.Rows[row]["Prog_Name"].ToString();
                        string ProgLink = DT.Rows[row]["Prog_Link"].ToString();

                        //-- 組合Html --
                        // 判斷是否有子項目, 變更css樣式
                        SBHtml.AppendLine("<li class=\"{0} {1}\">".FormatThis(
                                (Child_Cnt == 0) ? "" : "sub-menu"
                                , Prog_UpID.Equals(ProgID) ? "active" : ""
                            ));

                        // 判斷是否有連結, 並變更css樣式
                        SBHtml.Append("<a href=\"{0}\"><span class=\"{1}\"></span>&nbsp;<span>{2}</span>{3}</a>".FormatThis(
                                (string.IsNullOrEmpty(ProgLink)) ? "javascript:;" : (Application["WebUrl"] + ProgLink)
                                , CssStyle
                                , ProgName
                                , (Child_Cnt == 0) ? "" : "<span class=\"arrow\"></span>"
                            ));

                        //判斷是否有子選單
                        if (Child_Cnt > 0)
                        {
                            SBHtml.AppendLine(CreateSubMenu(ProgID, out ErrMsg));
                        }
                        SBHtml.AppendLine("</li>");

                    }
                }

                return SBHtml.ToString();
            }


        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return "<li><a><span class=\"glyphicon glyphicon-exclamation-sign\"></span>&nbsp;無法取得選單<br/>{0}</a></li>".FormatThis(ErrMsg);
        }
    }

    /// <summary>
    /// [建立選單] = 第二層
    /// </summary>
    /// <param name="Up_ID">上層編號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns></returns>
    private string CreateSubMenu(string Up_ID, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder SBHtml = new StringBuilder();

                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();

                //[SQL] - 判斷(是否有使用者權限)
                SBSql.AppendLine("DECLARE @RowCnt AS INT ");
                SBSql.AppendLine("SET @RowCnt = ( ");
                SBSql.AppendLine("	SELECT COUNT(*) ");
                SBSql.AppendLine("	FROM Program INNER JOIN User_Profile_Rel_Program UserRel ON Program.Prog_ID = UserRel.Prog_ID ");
                SBSql.AppendLine("	WHERE (Program.Display = 'Y') AND (Program.MenuDisplay = 'Y') AND (Program.Lv = 2) ");
                SBSql.AppendLine("    AND (Program.Up_Id = @Param_UpID) AND (UserRel.Guid = @UserGUID)");
                SBSql.AppendLine(")  ");
                SBSql.AppendLine("IF @RowCnt <> 0 ");
                //[SQL] - 顯示權限(使用者)
                SBSql.AppendLine("	BEGIN ");
                SBSql.AppendLine("	    SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.CssStyle ");
                //[SQL] - 判斷&顯示(目前語系)
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine("		FROM Program INNER JOIN User_Profile_Rel_Program UserRel ON Program.Prog_ID = UserRel.Prog_ID ");
                SBSql.AppendLine("	    WHERE (Program.Display = 'Y') AND (Program.MenuDisplay = 'Y') AND (Program.Lv = 2) ");
                SBSql.AppendLine("        AND (Program.Up_Id = @Param_UpID) AND (UserRel.Guid = @UserGUID)");
                SBSql.AppendLine("      ORDER BY Program.Sort, Program.Prog_ID ");
                SBSql.AppendLine("	END ");
                SBSql.AppendLine("ELSE ");
                //[SQL] - 顯示權限(群組)
                SBSql.AppendLine("	BEGIN ");
                SBSql.AppendLine("   SELECT TbGroup.* FROM (");
                SBSql.AppendLine("	    SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.CssStyle ");
                //[SQL] - 判斷&顯示(目前語系)
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine("		FROM Program INNER JOIN User_Group_Rel_Program GroupRel ON Program.Prog_ID = GroupRel.Prog_ID ");
                SBSql.AppendLine("		WHERE (Program.Up_Id = @Param_UpID) AND (Program.Display = 'Y') AND (Program.MenuDisplay = 'Y') AND (Program.Lv = 2) ");

                #region >> 群組參數組合 <<
                //[取得參數] - 所屬群組
                ArrayList Groups = (ArrayList)Session["Login_UserGroups"];
                //[SQL] - 暫存參數
                string tempParam = "";
                for (int i = 0; i < Groups.Count; i++)
                {
                    if (string.IsNullOrEmpty(tempParam) == false) { tempParam += ","; }
                    tempParam += "@ParamTmp" + i;
                }
                //[SQL] - 代入暫存參數
                SBSql.AppendLine(" AND (GroupRel.Guid IN (" + tempParam + "))");
                for (int i = 0; i < Groups.Count; i++)
                {
                    cmd.Parameters.AddWithValue("ParamTmp" + i, Groups[i]);
                }
                #endregion

                SBSql.AppendLine("   ) AS TbGroup");
                SBSql.AppendLine("   GROUP BY Prog_ID, Prog_Link, Sort, CssStyle, Prog_Name ");
                SBSql.AppendLine("   ORDER BY Sort, Prog_ID ");
                SBSql.AppendLine("	END ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("UserGUID", Session["Login_GUID"].ToString());
                cmd.Parameters.AddWithValue("Param_UpID", Up_ID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        SBHtml.AppendLine("<ul class=\"sub\">");

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //取得參數
                            string ProgID = DT.Rows[row]["Prog_ID"].ToString();
                            string ProgName = DT.Rows[row]["Prog_Name"].ToString();
                            string ProgLink = DT.Rows[row]["Prog_Link"].ToString();

                            SBHtml.AppendLine("<li class=\"{0}\"><a href=\"{1}\">{2}</a></li>".FormatThis(
                                    Prog_SubID.Equals(ProgID) ? "active" : ""
                                    , Application["WebUrl"] + ProgLink
                                    , ProgName
                                ));
                        }

                        SBHtml.AppendLine("</ul>");
                    }
                }

                return SBHtml.ToString();
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return "<li><a><span class=\"glyphicon glyphicon-exclamation-sign\"></span>&nbsp;無法取得子選單</a></li>";
        }
    }
    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// 登出
    /// </summary>
    protected void lbtn_Logout_Click(object sender, EventArgs e)
    {
        //判斷是否使用內部AD驗證
        string IsAD = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_IsUse"];
        if (IsAD.ToUpper().Equals("Y"))
        {
            //清除Cookie
            if (Request.Cookies["PK_UserSID"] != null)
            {
                HttpCookie myCookie = new HttpCookie("PK_UserSID");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
        }

        //清除Session
        Session.Clear();
        Session.Abandon();

        //導向登入頁
        Response.Redirect("{0}ServiceLogin/".FormatThis(Application["WebUrl"]));
    }
    #endregion

    /// <summary>
    /// 瀏覽器Title
    /// </summary>
    private string _Param_WebTitle;
    public string Param_WebTitle
    {
        get
        {
            if (string.IsNullOrEmpty(Page.Title))
            {
                return Application["WebName"].ToString();
            }
            else
            {
                return "{0} | {1}".FormatThis(Page.Title, Application["WebName"].ToString());
            }
        }
        set
        {
            this._Param_WebTitle = value;
        }
    }

    #region Imaster 設定
    /// <summary>
    /// ContentPage 回傳程式編號, 用以判斷選單是否為active
    /// </summary>
    /// <param name="UpID"></param>
    /// <param name="SubID"></param>
    public void setProgID(string UpID, string SubID)
    {
        Prog_UpID = UpID;
        Prog_SubID = SubID;
    }

    /// <summary>
    /// 共用參數, 第一層選單編號
    /// </summary>
    private string _Prog_UpID;
    public string Prog_UpID
    {
        get
        {
            return this._Prog_UpID != null ? this._Prog_UpID : "";
        }
        set
        {
            this._Prog_UpID = value;
        }
    }

    /// <summary>
    /// 共用參數, 第二層選單編號
    /// </summary>
    private string _Prog_SubID;
    public string Prog_SubID
    {
        get
        {
            return this._Prog_SubID != null ? this._Prog_SubID : "";
        }
        set
        {
            this._Prog_SubID = value;
        }
    }
    #endregion

}