using ExtensionMethods;
using LogRecord;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

/// <summary>
/// 自動重新導向
/// </summary>
/// <remarks>
/// http://www.prokits.com.tw/Redirect.aspx?ActType=l&Data=en-US&rt=http://www.prokits.com.tw/WhereToBuy/
/// ActType:l=語系, r=區域
/// Data:l->en-US..., r->1,2,3
/// rt:指定網址(不設定則為首頁)
/// </remarks>
public partial class Redirect : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //判斷類型
                switch (Req_ActType.ToLower())
                {
                    case "l":
                        changeLang();

                        break;

                    case "r":
                        changeArea();

                        break;

                    case "ltor":
                        //自動依語系切換區域
                        changeLang();
                        changeArea();
                        break;

                    case "log":
                        //記錄
                        fn_Log.writeLog(
                           fn_Param.MemberID
                           , fn_stringFormat.Set_FilterHtml(Request.QueryString["mu"].ToString())
                           , "4001"
                           , "前往舊版經銷商頁面,Menu ID = {0}".FormatThis(fn_stringFormat.Set_FilterHtml(Request.QueryString["mu"].ToString()))
                           );

                        break;

                    case "buy":
                        //導購記錄
                        //http://url/Redirect.aspx?ActType=buy&id={品號}&data={區域}&rt={導購網址}
                        string id = Request.QueryString["id"].ToString();
                        fn_Log.writeLog(
                           fn_Param.MemberID
                           , fn_stringFormat.Set_FilterHtml(id)
                           , "5001"
                           , "#{0}#的使用者按下{1}的導購,網址={2}".FormatThis(Req_Data, fn_stringFormat.Set_FilterHtml(id), Req_ReturnUrl)
                           );

                        break;
                }

                //導向至指定Url
                Response.Redirect(Req_ReturnUrl);
                return;

            }
            catch (Exception)
            {
                //Response.Write(ex.Message.ToString());
                throw;
            }
        }
    }


    /// <summary>
    /// 切換語系
    /// </summary>
    private void changeLang()
    {
        if (string.IsNullOrEmpty(Req_ActType) == false)
        {
            //新增語系Cookies
            Response.Cookies.Remove("PKWeb_Lang");
            Response.Cookies.Add(new HttpCookie("PKWeb_Lang", Req_Data));
            Response.Cookies["PKWeb_Lang"].Expires = DateTime.Now.AddYears(1);
        }
    }

    /// <summary>
    /// 切換區域
    /// </summary>
    private void changeArea()
    {
        if (string.IsNullOrEmpty(Req_ActType) == false)
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                cmd.Parameters.Clear();

                // SQL查詢組成
                SBSql.AppendLine("SELECT AreaCode, AreaName");
                SBSql.AppendLine(" FROM Param_Area WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (LOWER(LangCode) = LOWER(@Lang)) AND (AreaCode = @AreaCode) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);

                // 判斷是否為同時變更語系及區域
                if (Req_ActType.ToLower().Equals("ltor"))
                {
                    //判斷傳入語系, 切換所屬區域
                    string myArea;
                    switch (Req_Data.ToLower())
                    {
                        case "zh-tw":
                            myArea = "2";
                            break;

                        case "zh-cn":
                            myArea = "3";
                            break;

                        default:
                            myArea = "1";
                            break;
                    }

                    cmd.Parameters.AddWithValue("AreaCode", myArea);
                }
                else
                {
                    cmd.Parameters.AddWithValue("AreaCode", Req_Data);
                }

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        string myArea = DT.Rows[0]["AreaCode"].ToString();

                        //新增區域Cookies(區域編號)
                        Response.Cookies.Remove("PKWeb_Area");
                        Response.Cookies.Add(new HttpCookie("PKWeb_Area", myArea));
                        Response.Cookies["PKWeb_Area"].Expires = DateTime.Now.AddYears(1);

                    }
                }
            }
        }
    }

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - ActType
    /// </summary>
    private string _Req_ActType;
    public string Req_ActType
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["ActType"]) ? "" : Request.QueryString["ActType"].ToString();
        }
        set
        {
            this._Req_ActType = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Data
    /// </summary>
    private string _Req_Data;
    public string Req_Data
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["Data"]) ? "" : Request.QueryString["Data"].ToString();
        }
        set
        {
            this._Req_Data = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 指定Url
    /// </summary>
    private string _Req_ReturnUrl;
    public string Req_ReturnUrl
    {
        get
        {
            string ReturnUrl = string.IsNullOrEmpty(Request.QueryString["rt"]) ? "" : HttpUtility.HtmlDecode(Request.QueryString["rt"].ToString());

            if (!ReturnUrl.IsUrl())
            {
                ReturnUrl = Application["WebUrl"].ToString();
            }

            return ReturnUrl;
        }
        set
        {
            this._Req_ReturnUrl = value;
        }
    }

    #endregion
}