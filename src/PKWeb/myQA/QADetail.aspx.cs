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

public partial class QADetail : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //取得資料 - 答案
                LookupDataList();

                //取得快速連結 - 標題
                LookupDataList_Caption();

                //隱藏主頁的meta
                PlaceHolder myMeta = (PlaceHolder)Master.FindControl("ph_MetaInfo");
                myMeta.Visible = false;

                //** 次標題 **
                this.Page.Title = meta_Title;

                this.btn_Yes.Text = this.GetLocalResourceObject("btn_Yes").ToString();
                this.btn_SendMsg.Text = this.GetLocalResourceObject("btn_Submit").ToString();

                //** 設定程式編號(目前頁面所在功能位置) **
                if (false == setProgIDs.setID(this.Master, "200"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 副程式 - 取得答案列表
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT myData.FAQ_Title, myBlock.Block_ID, myBlock.Block_Title, myBlock.Block_Desc");
                SBSql.AppendLine(" , (CASE ROW_NUMBER() OVER(ORDER BY myBlock.Sort, myBlock.Block_ID)");
                SBSql.AppendLine("     WHEN 1 THEN 'inActive'");
                SBSql.AppendLine("     ELSE ''");
                SBSql.AppendLine("     END) AS dtDefault");
                SBSql.AppendLine(" FROM FAQ myData");
                SBSql.AppendLine("  INNER JOIN FAQ_Group GP ON myData.Group_ID = GP.Group_ID");
                SBSql.AppendLine("  INNER JOIN FAQ_Block myBlock ON myData.FAQ_ID = myBlock.FAQ_ID");
                SBSql.AppendLine(" WHERE (myData.FAQ_ID = @DataID)");

                //判斷會員身份
                if (fn_Param.MemberType.Equals("1"))
                {
                    //經銷商身份
                    SBSql.Append(" AND (GP.Target IN (0,1))");
                }
                else
                {
                    //其他
                    SBSql.Append(" AND (GP.Target = 0)");
                }

                SBSql.AppendLine(" ORDER BY myBlock.Sort, myBlock.Block_ID");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        Response.Redirect("{0}QA/Content/{1}".FormatThis(Application["WebUrl"].ToString(), Server.UrlEncode(Req_ModelNo)));
                        return;
                    }

                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    //填入主標題
                    string FAQTitle = DT.Rows[0]["FAQ_Title"].ToString();
                    this.lt_QATitle.Text = FAQTitle;

                    //Meta資訊
                    meta_Title = "{0} | {1}".FormatThis(Req_ModelNo, Resources.resPublic.title_常見問題);
                    meta_Desc = "{0} | {1}：{2}".FormatThis(Req_ModelNo, Resources.resPublic.title_常見問題, FAQTitle);
                    meta_Url = "{0}QA/View/{1}/{2}/".FormatThis(
                        Application["WebUrl"].ToString()
                        , Server.UrlEncode(Req_ModelNo)
                        , Cryptograph.MD5Encrypt(Req_DataID, DesKey)
                        );
                    meta_Image = "{0}/myProd/{1}/".FormatThis(
                        Application["API_WebUrl"]
                        , Server.UrlEncode(Req_ModelNo)
                        );

                }
            }

        }
        catch (Exception)
        {
            throw;
            throw new Exception("系統發生錯誤 - 讀取資料");
        }
    }


    /// <summary>
    /// 副程式 - 取得標題列表
    /// </summary>
    private void LookupDataList_Caption()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder html = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT myData.FAQ_Title, myData.FAQ_ID");
                SBSql.AppendLine("  , RTRIM(Prod.Model_No) AS ModelNo, RTRIM(Prod.Model_Name_{0}) AS ModelName".FormatThis(fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM FAQ_Group GP WITH(NOLOCK)");
                //SBSql.AppendLine(" 	INNER JOIN FAQ_Area Area WITH(NOLOCK) ON GP.Group_ID = Area.Group_ID");
                SBSql.AppendLine(" 	INNER JOIN FAQ myData WITH(NOLOCK) ON GP.Group_ID = myData.Group_ID");
                SBSql.AppendLine(" 	INNER JOIN FAQ_Rel_ModelNo Rel WITH(NOLOCK) ON GP.Group_ID = Rel.Group_ID");
                SBSql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH(NOLOCK) ON Rel.Model_No = Prod.Model_No");
                SBSql.AppendLine(" WHERE (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("     AND (UPPER(Rel.Model_No) = UPPER(@ModelNo))");
                // (Area.AreaCode = @AreaCode) AND 

                //判斷會員身份
                if (fn_Param.MemberType.Equals("1"))
                {
                    //經銷商身份
                    SBSql.Append(" AND (GP.Target IN (0,1))");
                }
                else
                {
                    //其他
                    SBSql.Append(" AND (GP.Target = 0)");
                }

                cmd.CommandText = SBSql.ToString();
                //cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("ModelNo", Req_ModelNo);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        //填入產品資訊Html
                        string Model_No = DT.Rows[0]["ModelNo"].ToString();
                        string ModelName = DT.Rows[0]["ModelName"].ToString();
                        this.lt_ModelNo.Text = Model_No;
                        this.lt_ModelName.Text = ModelName;
                        this.lt_ModelPic.Text = "<img src=\"{0}\" alt=\"{1}\" />".FormatThis(
                             "{0}myProd/{1}/".FormatThis(Application["API_WebUrl"], Server.UrlEncode(Model_No))
                            , ModelName
                            );

                        //組合Html
                        html.Append("<ul>");

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            string myID = DT.Rows[row]["FAQ_ID"].ToString();
                            string myCap = DT.Rows[row]["FAQ_Title"].ToString();
                            string myUri = "{0}QA/View/{1}/{2}".FormatThis(
                                    Application["WebUrl"]
                                    , Server.UrlEncode(Req_ModelNo)
                                    , Cryptograph.MD5Encrypt(myID, DesKey)
                                );

                            html.Append("<li {0}><a href=\"{1}\">{2}</a></li>".FormatThis(
                                (Req_DataID.Equals(myID)) ? "class=\"current\"" : ""
                                , myUri
                                , myCap
                                ));
                        }

                        html.Append("</ul>");

                        //輸出Html
                        this.lt_CapList.Text = html.ToString();
                    }

                }
            }

        }
        catch (Exception)
        {
            throw;
            throw new Exception("系統發生錯誤 - 讀取標題");
        }
    }
    /// <summary>
    /// 判斷編號是否正確
    /// </summary>
    /// <param name="DataID">資料編號</param>
    /// <returns></returns>
    private bool Check_ID(string DataID)
    {
        try
        {
            //判斷是否空白
            if (string.IsNullOrEmpty(DataID))
            {
                return false;
            }

            //判斷編碼是否正確
            String RealID = Cryptograph.MD5Decrypt(fn_stringFormat.Set_FilterHtml(DataID).Trim(), DesKey);
            if (string.IsNullOrEmpty(RealID))
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 判斷目前資料所在位置,並切換css
    /// </summary>
    /// <param name="act">是否為active</param>
    /// <param name="actType">類型</param>
    /// <param name="actCss">css 名稱</param>
    /// <returns></returns>
    public string Get_CurrentClass(string act, string actType, string actCss)
    {
        switch (actType.ToLower())
        {
            case "header":
                return (act.ToLower().Equals("inactive")) ? "" : actCss;

            case "content":
                return (act.ToLower().Equals("inactive")) ? actCss : "";

            default:
                return "";
        }

    }
    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// FeedBack - Yes
    /// </summary>
    protected void btn_Yes_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(FB_ID) ,0) + 1 FROM FAQ_FeedBack ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增資料
                SBSql.AppendLine(" INSERT INTO FAQ_FeedBack( ");
                SBSql.AppendLine("  FAQ_ID, FB_ID, FB_Choice, FB_Desc");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @DataID, @NewID, 'Y', ''");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    Response.Redirect(Page_CurrentUrl);
                    return;
                }
            }

            //Hide filed
            this.fb_choice.Visible = false;
            this.fb_OK.Visible = true;
            this.ph_feedTitle.Visible = false;

        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// FeedBack - Send Message
    /// </summary>
    protected void btn_SendMsg_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(FB_ID) ,0) + 1 FROM FAQ_FeedBack ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增資料
                SBSql.AppendLine(" INSERT INTO FAQ_FeedBack( ");
                SBSql.AppendLine("  FAQ_ID, FB_ID, FB_Choice, FB_Desc");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @DataID, @NewID, 'N', @FB_Desc");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                cmd.Parameters.AddWithValue("FB_Desc", fn_stringFormat.Set_FilterHtml(this.tb_feedMsg.Text));
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    Response.Redirect(Page_CurrentUrl);
                    return;
                }
            }

            //Hide filed
            this.fb_choice.Visible = false;
            this.fb_OK.Visible = true;
            this.ph_feedTitle.Visible = false;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Convert.ToString(Page.RouteData.Values["DataID"]);
            if (false == Check_ID(DataID))
            {
                fn_Extensions.JsAlert("", "script:history.back(-1);");
                return "";
            }
            else
            {
                String RealID = Cryptograph.MD5Decrypt(fn_stringFormat.Set_FilterHtml(DataID).Trim(), DesKey);
                return RealID;
            }
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 品號
    /// </summary>
    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String myData = Convert.ToString(Page.RouteData.Values["myModel"]);
            return (fn_Extensions.String_資料長度Byte(myData, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(myData).Trim() : "";
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}QA/View/{1}/{2}".FormatThis(
                Application["WebUrl"]
                , HttpUtility.UrlEncode(Req_ModelNo)
                , HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Req_DataID, DesKey))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    ///// <summary>
    ///// 取得產品網址,使用API
    ///// </summary>
    //private string _ProdUrl;
    //public string ProdUrl
    //{
    //    get
    //    {
    //        string url = "http://go.prokits.com.tw/";

    //        switch (fn_Area.PKWeb_Area)
    //        {
    //            case "2":
    //                url += "ProdB/";
    //                break;

    //            case "3":
    //                url += "ProdC/";
    //                break;

    //            default:
    //                url += "ProdA/";
    //                break;
    //        }

    //        return url + Server.UrlEncode(Req_ModelNo) + "/";
    //    }
    //    set
    //    {
    //        this._ProdUrl = value;
    //    }
    //}

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
    {
        get
        {
            return Application["DesKey"].ToString();
        }
        set
        {
            this._DesKey = value;
        }
    }


    public string meta_Title
    {
        get;
        set;
    }

    public string meta_Desc
    {
        get;
        set;
    }

    public string meta_Url
    {
        get;
        set;
    }

    public string meta_Image
    {
        get;
        set;
    }
    #endregion
}