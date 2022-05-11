using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using ExtensionMethods;

public partial class Site_Clean : System.Web.UI.MasterPage, IProgID
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;
    public string ErrMsg;

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
            try
            {
                //取得所有選單資料
                DataTable DataAll = DT_Menu();

                //顯示Top選單
                this.lt_TopMenu.Text = ShowMenu_Top(DataAll);

                //顯示Footer選單
                this.lt_FooterMenu.Text = ShowMenu_Footer(DataAll);

                //登入/登出鈕
                this.lbtn_login.Text = "<i class=\"fa fa-user fa-lg\"></i>&nbsp;{0}".FormatThis(Resources.resPublic.home_會員登入);
                this.lbtn_logout.Text = "<i class=\"fa fa-sign-out fa-fw\"></i>&nbsp;{0}".FormatThis(Resources.resPublic.home_會員登出);

                //顯示登入/登出鈕
                if (string.IsNullOrEmpty(fn_Param.MemberID))
                {
                    this.lbtn_login.Visible = true;
                    this.lbtn_logout.Visible = false;
                    this.ph_Member.Visible = false;
                }
                else
                {
                    this.lbtn_login.Visible = false;
                    this.lbtn_logout.Visible = true;
                    this.ph_Member.Visible = true;
                }

                //Keyword Search
                this.tb_Keyword1.Attributes.Add("placeholder", Resources.resPublic.tip_關鍵字);
                this.tb_Keyword2.Attributes.Add("placeholder", Resources.resPublic.tip_關鍵字);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// 取得選單資料
    /// </summary>
    /// <returns></returns>
    private DataTable DT_Menu()
    {
        try
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                cmd.Parameters.Clear();

                // SQL查詢組成
                SBSql.AppendLine("SELECT Menu_ID, LangCode, Menu_Block, Menu_Name, Menu_Uri, Parent_ID, IsRoot, Place, NewOpen, NewData");
                SBSql.AppendLine(" FROM Navi_Menu WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (LOWER(LangCode) = LOWER(@Lang)) ");
                SBSql.AppendLine(" ORDER BY Sort ASC, IsRoot DESC ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);

                // SQL查詢執行
                return dbConn.LookupDT(cmd, out ErrMsg);
            }

        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 取得TOP選單
    /// </summary>
    /// <param name="DataSrc">資料來源</param>
    /// <returns></returns>    
    private string ShowMenu_Top(DataTable DataSrc)
    {
        //宣告
        StringBuilder html = new StringBuilder();

        //取得資料 - 所有資料(Place = 'both', 'top')
        var DataQuery = from el in DataSrc.AsEnumerable()
                        where el.Field<string>("Place").ToLower().Equals("both") || el.Field<string>("Place").ToLower().Equals("top")
                        select el;

        //Html組成
        html.AppendLine("<ul class=\"nav navbar-nav\">");

        html.AppendLine("<li id=\"nav-home\" class=\"nav-home-hide\"><a href=\"{0}\"><i class=\"fa fa-home fa-lg\"></i></a></li>".FormatThis(Application["WebUrl"]));

        //顯示資料 - 第一層
        var MainQuery = from el in DataQuery
                        where el.Field<string>("IsRoot").Equals("Y")
                        select new
                        {
                            Menu_Block = el.Field<string>("Menu_Block"),
                            Menu_Name = el.Field<string>("Menu_Name").ToUpper(),
                            Menu_Uri = el.Field<string>("Menu_Uri"),
                            NewOpen = el.Field<string>("NewOpen"),
                            NewData = el.Field<string>("NewData"),
                            Menu_ID = el.Field<int>("Menu_ID"),
                            Parent_ID = el.Field<int>("Parent_ID")
                        };
        foreach (var MainItem in MainQuery)
        {
            bool showRootMenu = true;

            /*
             * 判斷身份 與 選單類型
             * 若類型為經銷商, 但身份不是經銷商, 則第一層選單不顯示
             */
            if ((MainItem.Menu_Block.ToUpper().Equals("DEALER"))
                && !fn_Param.MemberType.Equals("1"))
            {
                showRootMenu = false;
            }

            if (showRootMenu)
            {
                html.AppendLine("<li class=\"dropdown\">");
                html.AppendLine("<a href=\"{0}\" target=\"{2}\" {3}>{1}</a>".FormatThis(
                    MainItem.NewOpen.Equals("Y") ? MainItem.Menu_Uri : Application["WebUrl"] + MainItem.Menu_Uri,
                        MainItem.Menu_Name,
                        MainItem.NewOpen.Equals("Y") ? "_blank" : "_self",
                        MainItem.Menu_Uri.Equals("#") ? "class=\"dropdown-toggle\" data-toggle=\"dropdown\"" : ""
                    ));

                //判斷是否要另外取得資料
                if (string.IsNullOrEmpty(MainItem.NewData))
                {
                    //顯示資料 - 第二層 (固定式選單)
                    var SubQuery = from el in DataQuery
                                   where el.Field<string>("IsRoot").Equals("N")
                                    && el.Field<int>("Parent_ID").Equals(MainItem.Menu_ID)
                                   select new
                                   {
                                       Menu_Name = el.Field<string>("Menu_Name"),
                                       Menu_Uri = el.Field<string>("Menu_Uri"),
                                       Menu_ID = el.Field<int>("Menu_ID"),
                                       NewOpen = el.Field<string>("NewOpen"),
                                       LangCode = el.Field<string>("LangCode"),
                                       Menu_Block = el.Field<string>("Menu_Block")
                                   };


                    #region >> 第2層選單 <<
                    if (SubQuery.Count() > 0)
                    {
                        html.AppendLine("<ul class=\"dropdown-menu\" role=\"menu\">");
                    }

                    foreach (var SubItem in SubQuery)
                    {
                        //選單Url
                        string thisMenuUri = SubItem.Menu_Uri;

                        #region >> 經銷商專區選單 <<

                        //判斷是否為經銷商功能, 若是則對網址動手腳
                        if (SubItem.Menu_Block.ToUpper().Equals("DEALER"))
                        {
                            /*
                             * 傳遞參數:l=語系, m=選單編號, dealerID=經銷商ERP代號, code=MD5(dealerID + 今天的日期)
                             * 此為簡易驗證, 故連結在一天內皆有效
                             * 
                             */
                            /*
                            if (SubItem.Menu_Uri.Equals("#auto"))
                            {
                                thisMenuUri = "{0}Redirect.aspx?ActType=log&mu={1}&rt={2}".FormatThis(
                                        Application["WebUrl"]
                                        , SubItem.Menu_ID
                                        , HttpUtility.UrlEncode(
                                            "http://w3.prokits.com.tw/CrossPage/CheckLogin.asp?l={0}&m={1}&dealerID={2}&code={3}".FormatThis(
                                                SubItem.LangCode
                                                , SubItem.Menu_ID
                                                , Param_DealerID
                                                , Param_DealerMD5
                                            )
                                        )
                                    );
                            }
                            else
                            {
                                thisMenuUri = SubItem.Menu_Uri;
                            }
                             * */
                        }

                        #endregion

                        html.AppendLine("<li><a href=\"{0}\" target=\"{2}\">{1}</a></li>".FormatThis(
                                SubItem.NewOpen.Equals("Y") ? thisMenuUri : Application["WebUrl"] + thisMenuUri,
                                SubItem.Menu_Name,
                                SubItem.NewOpen.Equals("Y") ? "_blank" : "_self"
                            ));
                    }

                    if (SubQuery.Count() > 0)
                    {
                        html.AppendLine("</ul>");
                    }
                    #endregion

                }

                html.AppendLine("</li>");
            }
        }

        html.AppendLine("</ul>");


        return html.ToString();

    }

    /// <summary>
    /// 取得Footer選單
    /// </summary>
    /// <param name="DataSrc">資料來源</param>
    /// <returns></returns>
    private string ShowMenu_Footer(DataTable DataSrc)
    {
        //宣告
        StringBuilder html = new StringBuilder();

        //取得資料 - 所有資料(Place = 'both', 'footer')
        var DataQuery = from el in DataSrc.AsEnumerable()
                        where el.Field<string>("Place").ToLower().Equals("both") || el.Field<string>("Place").ToLower().Equals("footer")
                        select el;

        //顯示資料 - 第一層
        var MainQuery = from el in DataQuery
                        where el.Field<string>("IsRoot").Equals("Y")
                        select new
                        {
                            Menu_Block = el.Field<string>("Menu_Block"),
                            Menu_Name = el.Field<string>("Menu_Name"),
                            Menu_Uri = el.Field<string>("Menu_Uri")
                        };
        foreach (var MainItem in MainQuery)
        {
            html.AppendLine("<li>");
            html.AppendLine("<h3>{0}</h3>".FormatThis(MainItem.Menu_Name));

            //顯示資料 - 第二層
            var SubQuery = from el in DataQuery
                           where el.Field<string>("IsRoot").Equals("N")
                            && el.Field<string>("Menu_Block").Equals(MainItem.Menu_Block)
                           select new
                           {
                               Menu_Name = el.Field<string>("Menu_Name"),
                               Menu_Uri = el.Field<string>("Menu_Uri")
                           };

            if (SubQuery.Count() > 0)
            {
                html.AppendLine("<ul>");
            }

            foreach (var SubItem in SubQuery)
            {
                html.AppendLine("<li><a href=\"{0}\">{1}</a></li>".FormatThis(
                        Application["WebUrl"] + SubItem.Menu_Uri,
                        SubItem.Menu_Name
                    ));

            }

            if (SubQuery.Count() > 0)
            {
                html.AppendLine("</ul>");
            }

            html.AppendLine("</li>");
        }


        return html.ToString();

    }


    #region -- 按鈕功能 Start --

    /// <summary>
    /// 登入
    /// </summary>
    protected void lbtn_login_Click(object sender, EventArgs e)
    {
        Response.Redirect("{0}Login/".FormatThis(Application["WebUrl"]));
    }

    /// <summary>
    /// 登出
    /// </summary>
    protected void lbtn_logout_Click(object sender, EventArgs e)
    {
        //清除Session
        Session.Clear();
        Session.Abandon();

        //清除Cookie
        HttpCookie myCookie = new HttpCookie("PKWeb_MemberInfo");
        myCookie.Expires = DateTime.Now.AddDays(-1d);
        Response.Cookies.Add(myCookie);

        //重新導向
        Response.Redirect(Application["WebUrl"].ToString());
    }

    /// <summary>
    /// Search
    /// </summary>
    protected void btn_Search1_Click(object sender, EventArgs e)
    {
        string k = fn_stringFormat.Set_FilterHtml(this.tb_Keyword1.Text);
        Response.Redirect("{0}Products/?k={1}".FormatThis(
                Application["WebUrl"]
                , Server.UrlEncode(k)
            ));
    }
    protected void btn_Search2_Click(object sender, EventArgs e)
    {
        string k = fn_stringFormat.Set_FilterHtml(this.tb_Keyword2.Text);
        Response.Redirect("{0}Products/?k={1}".FormatThis(
                Application["WebUrl"]
                , Server.UrlEncode(k)
            ));
    }

    #endregion -- 按鈕功能 End --

    #region -- 參數設定 Start --
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

    /// <summary>
    /// 經銷商網站網址
    /// </summary>
    private string _Dealer_Url;
    public string Dealer_Url
    {
        get
        {
            switch (fn_Language.Param_Lang.ToLower())
            {
                case "zh_tw":
                    return "http://w3.prokits.com.tw/tw/Distributor_center/Login.asp";

                case "zh_cn":
                    return "http://w3.prokits.com.tw/cn/Distributor_center/Login.asp";

                default:
                    return "http://w3.prokits.com.tw/en/Distributor_center/Login.asp";
            }

        }
        set
        {
            this._Dealer_Url = value;
        }
    }

    /// <summary>
    /// Facebook網址
    /// </summary>
    private string _Facebook_Url;
    public string Facebook_Url
    {
        get
        {
            switch (fn_Language.Param_Lang.ToLower())
            {
                case "zh_tw":
                    return "https://www.facebook.com/pages/ProsKit-in-Taiwan/193925007355955";

                case "zh_cn":
                    return "https://www.facebook.com/pages/ProsKit-in-China/294294700592807";

                default:
                    return "https://www.facebook.com/pages/ProsKit-Global/139913052778640";
            }

        }
        set
        {
            this._Facebook_Url = value;
        }
    }

    private string _CDN_Url;
    public string CDN_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"]; ;

        }
        set
        {
            this._CDN_Url = value;
        }
    }


    /// <summary>
    /// 經銷商代號
    /// </summary>
    private string _Param_DealerID;
    public string Param_DealerID
    {
        get
        {
            return fn_Member.GetDealerID(fn_Param.MemberID);
        }
        set
        {
            this._Param_DealerID = value;
        }
    }

    private string _Param_DealerMD5;
    public string Param_DealerMD5
    {
        get
        {

            string dateNow = DateTime.Today.ToShortDateString().ToDateString("yyyyMMdd");
            string md5 = Cryptograph.MD5(Param_DealerID + dateNow);

            return md5;
        }
        set
        {
            this._Param_DealerMD5 = value;
        }
    }

    #endregion -- 參數設定 End --



    #region Imaster 設定
    /// <summary>
    /// ContentPage 回傳程式編號, 用以判斷選單是否為active
    /// </summary>
    /// <param name="UpID"></param>
    public void setProgID(string UpID)
    {
        Prog_UpID = UpID;
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
    #endregion
}