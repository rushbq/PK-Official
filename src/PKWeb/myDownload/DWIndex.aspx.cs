using System;
using System.Collections;
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

public partial class QAIndex : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_下載中心;

                //載入資料
                LookupData();

                //placeholder
                tb_Keyword.Attributes.Add("placeholder", this.GetLocalResourceObject("txt_關鍵字查詢").ToString());

                //判斷是否為經銷商 - hidden on 20180704
                //this.ph_DealerDW.Visible = fn_Param.MemberType.Equals("1");
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢 (分類資料)
                SBSql.Append(" SELECT Class_ID, Class_Name");
                SBSql.Append(" FROM File_Class");
                SBSql.Append(" WHERE (LOWER(LangCode) = LOWER(@LangCode)) AND (Web_Display = 'Y')");
                SBSql.AppendLine(" ORDER BY Sort");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKEF, out ErrMsg))
                {

                    StringBuilder html = new StringBuilder();


                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        html.Append("<a class=\"blok-link\" href=\"{0}\">".FormatThis(
                           "{0}Download/{1}".FormatThis(Application["WebUrl"], DT.Rows[row]["Class_ID"])));

                        html.Append("    <div class=\"col-md-4\">");
                        html.Append("        <div class=\"thumbnail\">");
                        html.Append("            <div class=\"block\">");
                        html.Append("                <div class=\"media-left\"><i class=\"fa fa-clipboard fa-3x fa-fw text-gray\"></i></div>");
                        html.Append("                <div class=\"media-body\"><h5 class=\"text-gray\">{0}</h5></div>".FormatThis(DT.Rows[row]["Class_Name"]));
                        html.Append("            </div>");
                        html.Append("        </div>");
                        html.Append("    </div>");
                        html.Append("</a>");
                    }

                    //顯示Html
                    this.lt_ClassItems.Text = html.ToString();
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 舊版經銷商資料網址
    /// </summary>
    /// <param name="menuID"></param>
    /// <returns></returns>
    public string historyUrl(int menuID)
    {
        string getMenuUri = "{0}Redirect.aspx?ActType=log&mu={1}&rt={2}".FormatThis(
                 Application["WebUrl"]
                 , menuID
                 , HttpUtility.UrlEncode(
                     "http://w3.prokits.com.tw/CrossPage/CheckLogin.asp?l={0}&m={1}&dealerID={2}&code={3}".FormatThis(
                         fn_Language.PKWeb_Lang
                         , menuID
                         , Param_DealerID
                         , Param_DealerMD5
                     )
                 )
             );

        return getMenuUri;
    }
    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("{0}Download/1/ALL/".FormatThis(Application["WebUrl"]));

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append(Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion


    #region -- 參數設定 --

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

    #endregion
}