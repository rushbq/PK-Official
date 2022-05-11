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
using ExtensionUI;

public partial class mySupport_DWList : System.Web.UI.Page
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

                //取得資料
                LookupData_Class();
                LookupData();

                ////** 設定程式編號(目前頁面所在功能位置) **
                //if (false == setProgIDs.setID(this.Master, "1"))
                //{
                //    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                //}
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 顯示大分類
    /// </summary>
    private void LookupData_Class()
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
                    //Clear Dropdown Menu
                    this.ddl_Class.Items.Clear();

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        this.ddl_Class.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString(), DT.Rows[row]["Class_ID"].ToString()));

                    }

                    //Selected Index
                    this.ddl_Class.SelectedValue = Req_ClassID;
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

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

                //判斷是否有指定品號
                if (!string.IsNullOrEmpty(Req_ModelNo))
                {
                    //[SQL] - 資料查詢 (品名)
                    SBSql.AppendLine(" SELECT RTRIM(Prod.Model_No) AS ModelNo, Prod.Model_Name_{0} AS ModelName ".FormatThis(fn_Language.Param_Lang));
                    SBSql.AppendLine(" FROM Prod_Item Prod WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Prod.Model_No = @Model_No) ");

                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Model_No", Req_ModelNo);
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            Response.Redirect(Application["WebUrl"].ToString());
                            return;
                        }

                        //填入品名品號
                        this.lt_ModelName.Text = DT.Rows[0]["ModelName"].ToString();
                        this.lt_ModelNo.Text = DT.Rows[0]["ModelNo"].ToString();
                        this.ph_ModelInfo.Visible = true;
                    }
                }

                //清除參數
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 資料查詢 (下載資料)
                SBSql.Append(" SELECT");
                SBSql.Append("   Base.File_ID, Base.FileName AS DwFile, Base.DisplayName AS DwFileName");
                SBSql.Append("   , FType.Class_Name AS FileTypeName, Lang.Class_Name AS LangName");
                SBSql.Append("   , ISNULL(Rel.Model_No , 'ALL') AS ModelNo");
                SBSql.Append("   , FTarget.Class_Name AS TargetName");
                SBSql.Append("   , Cls.Class_Name, Cls.Class_ID");
                SBSql.Append(" FROM PKEF.dbo.File_List Base");
                SBSql.Append("   INNER JOIN PKEF.dbo.File_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = @LangCode AND Cls.Web_Display = 'Y'");
                SBSql.Append("   INNER JOIN PKEF.dbo.File_LangType Lang ON Base.LangType_ID = Lang.Class_ID AND Lang.Display = 'Y'");
                SBSql.Append("   INNER JOIN PKEF.dbo.File_Type FType ON Base.FileType_ID = FType.Class_ID AND LOWER(FType.LangCode) = @LangCode AND FType.Display = 'Y' AND FType.Up_ClassID = @ClassID");
                SBSql.Append("   INNER JOIN PKEF.dbo.File_Target FTarget ON Base.Target = FTarget.Class_ID AND FTarget.Display = 'Y'");
                SBSql.Append("   INNER JOIN PKEF.dbo.File_Rel_ModelNo Rel ON Base.File_ID = Rel.File_ID");
                SBSql.Append("    AND Rel.Model_No IN (");
                SBSql.Append("        SELECT Model_No FROM Prod WHERE (Prod.Display = 'Y')");
                SBSql.Append("    )");
                SBSql.Append(" WHERE (Cls.Class_ID = @ClassID)");

                #region >> 判斷登入身份 <<
                //資料Target-- 1(everyone) / 2(All dealers) / 3(dealer group) / 4(one dealer) / 5(member)
                //登入身份-- 0(everyone)/1(member)/2(dealer)
                string LoginType = fn_Extensions.Get_MemberType(fn_Param.MemberType);
                switch (LoginType)
                {
                    case "1":
                        //一般會員
                        SBSql.Append(" AND (Base.Target IN (1,5)) ");

                        break;

                    case "2":
                        //經銷商
                        SBSql.Append(" AND ((Base.Target IN (1,2,3,4))");
                        SBSql.Append(" OR (");
                        SBSql.Append("  Base.File_ID IN (");
                        SBSql.Append("      SELECT Rel.File_ID");
                        SBSql.Append("      FROM PKEF.dbo.File_CustList CustList");
                        SBSql.Append("       INNER JOIN PKEF.dbo.File_Rel_CustGroup Rel ON CustList.Group_ID = Rel.Group_ID");
                        SBSql.Append("      WHERE (CustList.Cust_ERPID = @CustID))");
                        SBSql.Append("   OR (");
                        SBSql.Append("  Base.File_ID IN (");
                        SBSql.Append("      SELECT Rel.File_ID");
                        SBSql.Append("      FROM PKEF.dbo.File_Rel_Cust Rel");
                        SBSql.Append("      WHERE (Rel.Cust_ERPID = @CustID))");
                        SBSql.Append("    )");
                        SBSql.Append(" ))");

                        cmd.Parameters.AddWithValue("CustID", fn_Member.GetDealerID(fn_Param.MemberID));

                        break;

                    default:
                        //一般人
                        SBSql.Append(" AND (Base.Target IN (1))");
                        break;
                }

                #endregion

                #region >> 查詢條件 <<
                //[查詢條件] - 指定品號
                if (!string.IsNullOrEmpty(Req_ModelNo))
                {
                    SBSql.Append(" AND (Base.File_ID IN (");
                    SBSql.Append("   SELECT File_ID");
                    SBSql.Append("   FROM PKEF.dbo.File_Rel_ModelNo");
                    SBSql.Append("    WHERE Model_No = @ModelNo");
                    SBSql.Append("   ))");

                    cmd.Parameters.AddWithValue("ModelNo", Req_ModelNo);
                }

                //[查詢條件] - 關鍵字
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    SBSql.Append(" AND (");
                    //-檔案顯示名
                    SBSql.Append("     (Base.DisplayName LIKE '%' + @Keyword + '%')");

                    //-品號/品名
                    SBSql.Append("     OR (Base.File_ID IN (");
                    SBSql.Append("         SELECT File_ID FROM PKEF.dbo.File_Rel_ModelNo myRel");
                    SBSql.Append("          INNER JOIN ProductCenter.dbo.Prod_Item Prod ON myRel.Model_No = Prod.Model_No");
                    SBSql.Append("         WHERE (");
                    SBSql.Append("             (Prod.Model_No LIKE '%' + @Keyword + '%')");
                    SBSql.Append("             OR (Prod.Model_Name_en_US LIKE '%' + @Keyword + '%')");
                    SBSql.Append("             OR (Prod.Model_Name_zh_CN LIKE '%' + @Keyword + '%')");
                    SBSql.Append("             OR (Prod.Model_Name_zh_TW LIKE '%' + @Keyword + '%')");
                    SBSql.Append("         )");
                    SBSql.Append("      ))");

                    //-產品Tag
                    SBSql.Append("     OR (Base.File_ID IN (");
                    SBSql.Append("         SELECT File_ID FROM PKEF.dbo.File_Rel_ModelNo myRel");
                    SBSql.Append("          INNER JOIN Prod ON myRel.Model_No = Prod.Model_No");
                    SBSql.Append("          INNER JOIN Prod_Rel_Tags RelTag ON RelTag.Model_No = Prod.Model_No");
                    SBSql.Append("          INNER JOIN Prod_Tags Tags ON RelTag.Tag_ID = Tags.Tag_ID");
                    SBSql.Append("         WHERE (Tags.Tag_Name LIKE '%' + @Keyword + '%')");
                    SBSql.Append("     ))");
                    SBSql.Append(" )");

                    cmd.Parameters.AddWithValue("Keyword", Req_Keyword);
                }
                #endregion


                SBSql.AppendLine(" ORDER BY FType.Sort");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("ClassID", fn_Extensions.IsNumeric(Req_ClassID) ? Convert.ToInt16(Req_ClassID) : 0);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.ph_noData.Visible = true;
                        this.ph_Data.Visible = false;
                        return;
                    }

                    //宣告
                    StringBuilder html = new StringBuilder();
                    //取得token
                    string token = fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);


                    //[取得迴圈] - lv1 --------
                    int row = 0;

                    var query_M = DT.AsEnumerable()
                        .GroupBy(M => new
                        {
                            FileTypeName = M.Field<string>("FileTypeName")
                        })
                        .Select(el => new
                        {
                            FileTypeName = el.Key.FileTypeName
                        });


                    #region >> Tab 選項 <<

                    //[Html] - Navi Header
                    html.Append("<ul class=\"nav nav-tabs\" id=\"myTabs\" role=\"tablist\">");

                    foreach (var m in query_M)
                    {
                        row++;

                        html.Append("<li class=\"{1}\"><a href=\"#tab-{2}\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"true\">{0}</a></li>"
                            .FormatThis(
                             m.FileTypeName
                             , (row == 1) ? "active" : ""
                             , row
                            ));
                    }

                    html.Append("</ul>");

                    #endregion


                    #region >> Tab 內容 <<

                    //reset row
                    row = 0;
                    //[Html] - Body
                    html.Append("<div class=\"tab-content\">");

                    foreach (var m in query_M)
                    {
                        row++;

                        html.Append("<div class=\"tab-pane fade {1}\" role=\"tabpanel\" id=\"tab-{0}\">".FormatThis(
                            row
                            , (row == 1) ? "active in" : ""));
                       

                        //[取得類別迴圈] - lv2 --------
                        var query_GP = DT.AsEnumerable()
                            .Where(el => el.Field<string>("FileTypeName").Equals(m.FileTypeName))
                            .GroupBy(gp => new
                            {
                                File_ID = gp.Field<Guid>("File_ID"),
                                DwFile = gp.Field<string>("DwFile"),
                                DwFileName = gp.Field<string>("DwFileName"),
                                LangName = gp.Field<string>("LangName"),
                                Class_ID = gp.Field<Int16>("Class_ID")
                            })
                            .Select(el => new
                            {
                                File_ID = el.Key.File_ID,
                                DwFile = el.Key.DwFile,
                                DwFileName = el.Key.DwFileName,
                                LangName = el.Key.LangName,
                                Class_ID = el.Key.Class_ID
                            });

                        foreach (var gp in query_GP)
                        {
                            //[Html] - 檔名&下載路徑
                            /*
                             *  [FTP下載路徑參數]
                             *  f = 資料夾
                             *  r = 實際檔名
                             *  d = 另存新檔檔名
                             *  t = Token
                             */

                            html.Append("<div class=\"tab-card row\">");

                            //[Html] - Name
                            html.Append("<h3 class=\"tab-card-title tab-card-title col-md-6\">{0} ({1})</h3>".FormatThis(
                                gp.DwFileName, gp.LangName
                                ));

                            //[取得該類別的資料] - lv3 --------
                            var query = DT.AsEnumerable()
                                .Where(el => el.Field<Guid>("File_ID").Equals(gp.File_ID))
                                .Select(el => new
                                {
                                    ModelNo = el.Field<string>("ModelNo")
                                });

                            html.Append("<span class=\"col-md-4\">");
                            //[Html] - 適用型號
                            foreach (var item in query)
                            {
                                html.Append("<h5 class=\"id-label\">{0}</h5>&nbsp;".FormatThis(item.ModelNo));
                            }
                            html.Append("</span>");


                            //[Html] - Download Button
                            html.Append("<a href=\"{0}\" class=\"button col-md-1\">{1}</a>".FormatThis(
                                    "{0}myHandler/Ashx_FtpFileDownload.ashx?f={1}&r={2}&d={3}&t={4}".FormatThis(
                                        Application["WebUrl"]
                                        , Cryptograph.MD5Encrypt("ProdFiles/{0}".FormatThis(gp.Class_ID), DesKey)
                                        , Cryptograph.MD5Encrypt(gp.DwFile, DesKey)
                                        , HttpUtility.UrlEncode(gp.DwFileName)
                                        , token
                                    )
                                    , this.GetLocalResourceObject("txt_下載").ToString()
                                ));

                            html.AppendLine("</div>");
                        }

                        //--------------

                        html.Append("</div>");
                    }

                    html.Append("</div>");

                    #endregion


                    //輸出Html
                    this.lt_DataList.Text = html.ToString();
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    protected void ddl_Class_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = "{0}Download/{1}/{2}/{3}".FormatThis(
            Application["WebUrl"]
            , this.ddl_Class.SelectedValue
            , string.IsNullOrEmpty(Req_ModelNo) ? "ALL" : HttpUtility.UrlEncode(Req_ModelNo)
            , string.IsNullOrEmpty(Req_Keyword) ? "" : HttpUtility.UrlEncode(Req_Keyword)
            );

        Response.Redirect(url);
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_ClassID;
    public string Req_ClassID
    {
        get
        {
            String myData = Page.RouteData.Values["myClass"] == null ? "1" : Page.RouteData.Values["myClass"].ToString();

            return myData;
        }
        set
        {
            this._Req_ClassID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - ModelNo
    /// </summary>
    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String myData = Page.RouteData.Values["myModel"] == null ? "ALL" : Convert.ToString(Page.RouteData.Values["myModel"]);
            return (myData.ToUpper().Equals("ALL")) ? "" : myData;
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String myData = Page.RouteData.Values["myData"] == null ? "" : Convert.ToString(Page.RouteData.Values["myData"]);
            return string.IsNullOrEmpty(myData) ? "" : myData;
        }
        set
        {
            this._Req_Keyword = value;
        }
    }

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
    #endregion

}