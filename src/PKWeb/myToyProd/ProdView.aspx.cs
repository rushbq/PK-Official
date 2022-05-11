using System;
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
using LogRecord;

public partial class myProd_ProdView : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //取得資料
                LookupData();

                //隱藏主頁的meta
                PlaceHolder myMeta = (PlaceHolder)Master.FindControl("ph_MetaInfo");
                myMeta.Visible = false;

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

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                //產品基本資料
                SBSql.AppendLine(" SELECT TOP 1 Rel.Class_ID, RTRIM(myData.Model_No) AS ModelNo, RTRIM(myData.Model_Name_{0}) AS ModelName".FormatThis(fn_Language.Param_Lang));
                //產品客製資訊
                SBSql.AppendLine(" , myInfo.Info1 AS InfoFullDesc, myInfo.Info2 AS InfoFeature, myInfo.Info3 AS InfoApp");
                SBSql.AppendLine(" , myInfo.Info4 AS InfoSpec, myInfo.Info5 AS InfoDesc, myInfo.Info9 AS InfoDescSeo");

                //是否為新品
                SBSql.AppendLine(" , (CASE WHEN (DATEDIFF(DAY, GP.StartTime, GETDATE()) <= 365) AND (GP.IsNew = 'Y') THEN 'Y' ELSE 'N' END) AS IsNewItem");
                SBSql.AppendLine(" , (CASE WHEN (GP.IsNew = 'Z') THEN 'Z' ELSE 'N' END) AS IsRecItem");

                //是否已停售
                SBSql.AppendLine(" , (CASE WHEN GETDATE() > myData.Stop_Offer_Date THEN 'Y' ELSE 'N' END) AS IsStop");
                //產品主圖(判斷圖片中心 2->1->3->4->5->7->8->9)
                SBSql.AppendLine(" , (SELECT TOP 1 (ISNULL(Pic02,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'') ");
                SBSql.AppendLine("    + '|' + ISNULL(Pic05,'') + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')) AS PicGroup");
                SBSql.AppendLine("     FROM [ProductCenter].dbo.ProdPic_Photo WITH (NOLOCK) WHERE (ProdPic_Photo.Model_No = myData.Model_No)");
                SBSql.AppendLine("   ) AS PhotoGroup ");

                //產品示意圖
                SBSql.AppendLine(" , (SELECT TOP 1 Pic_File ");
                SBSql.AppendLine("    FROM [ProductCenter].dbo.ProdPic_Group ");
                SBSql.AppendLine("    WHERE (ProdPic_Group.Pic_Class = 13) AND (ProdPic_Group.Model_No = myData.Model_No) ");
                SBSql.AppendLine("    ORDER BY ProdPic_Group.Sort ASC, ProdPic_Group.Create_Time DESC, ProdPic_Group.Update_Time DESC ");
                SBSql.AppendLine("   ) AS SpecPic ");

                //取得該品號在各區域是否有上架資料
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM Prod_Rel_Area WHERE (Model_No = GP.Model_No) AND (AreaCode = 1)) AreaGlobal");
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM Prod_Rel_Area WHERE (Model_No = GP.Model_No) AND (AreaCode = 2)) AreaTW");
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM Prod_Rel_Area WHERE (Model_No = GP.Model_No) AND (AreaCode = 3)) AreaCN");

                //取得該品號在各區域是否有開賣資料
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM Prod_Rel_SellArea WHERE (Model_No = GP.Model_No) AND (AreaCode = 1)) SellGlobal");
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM Prod_Rel_SellArea WHERE (Model_No = GP.Model_No) AND (AreaCode = 2)) SellTW");
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM Prod_Rel_SellArea WHERE (Model_No = GP.Model_No) AND (AreaCode = 3)) SellCN");

                //判斷是否有下載
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM [PKEF].[dbo].File_Rel_ModelNo WHERE (Model_No = GP.Model_No)) AS CntDwFile");

                //判斷是否有FAQ
                SBSql.AppendLine(" , (SELECT COUNT(*)");
                SBSql.AppendLine("  FROM FAQ_Group inGP");
                SBSql.AppendLine("  INNER JOIN FAQ inBase ON inGP.Group_ID = inBase.Group_ID");
                SBSql.AppendLine("  INNER JOIN FAQ_Rel_ModelNo Rel ON inGP.Group_ID = Rel.Group_ID");
                SBSql.AppendLine("  WHERE (UPPER(inBase.LangCode) = 'ZH-TW') AND (Rel.Model_No = GP.Model_No)) AS CntFAQ");

                //判斷是否有商城輔圖
                SBSql.AppendLine(" , (SELECT COUNT(*) FROM [ProductCenter].dbo.Prod_MallPic WHERE (Model_No = GP.Model_No) AND (UPPER(LangCode) = UPPER(@Lang))) AS CntMallPic");

                SBSql.AppendLine(" FROM Prod GP ");
                SBSql.AppendLine("   INNER JOIN [ProductCenter].dbo.Prod_Item myData WITH (NOLOCK) ON GP.Model_No = myData.Model_No");
                SBSql.AppendLine("   INNER JOIN [ProductCenter].dbo.ProdToy_Class_Rel_ModelNo Rel ON myData.Model_No = Rel.Model_No");
                SBSql.AppendLine("   LEFT JOIN [ProductCenter].dbo.Prod_Info myInfo WITH (NOLOCK) ON GP.Model_No = myInfo.Model_No AND myInfo.Lang = @Lang");
                SBSql.AppendLine(" WHERE (GP.Display = 'Y') ");
                SBSql.AppendLine("   AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("   AND (myData.Model_No = @DataID)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //沒資料,返回列表頁
                    if (DT.Rows.Count == 0)
                    {
                        Response.StatusCode = 404;
                        Response.Redirect("{0}RobotKits".FormatThis(Application["WebUrl"].ToString()));
                        return;
                    }

                    //填入資料
                    Model_No = DT.Rows[0]["ModelNo"].ToString();
                    Model_Name = DT.Rows[0]["ModelName"].ToString();
                    string ModelName = DT.Rows[0]["ModelName"].ToString();
                    string PhotoGroup = DT.Rows[0]["PhotoGroup"].ToString();


                    //-- Meta資訊 --
                    //重設標題
                    meta_Title = "Pro'sKit {0}{1} | {2}".FormatThis(Model_Name, Model_No, Resources.resPublic.title_All);
                    meta_Desc = DT.Rows[0]["InfoDesc"].ToString().Left(100);
                    meta_Url = "{0}RobotKit/{1}/".FormatThis(
                        Application["WebUrl"].ToString()
                        , Model_No
                        );
                    meta_Image = GetData_MainPic(PhotoGroup, Model_No);

                    //關鍵字
                    meta_Keyword = GetData_Tags(Model_No);
                    meta_DescSeo = DT.Rows[0]["InfoDescSeo"].ToString();

                    //Navi代入類別名稱
                    lt_navbar.Text = "<li><a href=\"{1}RobotKits/{2}\">{0}</a></li><li>{3}</li>".FormatThis(
                        fn_CustomUI.Get_ProdToyClassName(DT.Rows[0]["Class_ID"].ToString(), fn_Language.Param_Lang)
                        , Application["WebUrl"].ToString()
                        , DT.Rows[0]["Class_ID"].ToString()
                        , Model_No
                        );

                    this.lt_ModelNo.Text = Model_No; //品號
                    this.lt_ModelName.Text = ModelName; //品名
                    this.lt_ProdInfo.Text = DT.Rows[0]["InfoDesc"].ToString().Replace("\n", "<br/>"); //產品簡述

                    //區域判斷
                    AreaGlobal = Convert.ToInt16(DT.Rows[0]["AreaGlobal"]);
                    AreaTW = Convert.ToInt16(DT.Rows[0]["AreaTW"]);
                    AreaCN = Convert.ToInt16(DT.Rows[0]["AreaCN"]);
                    this.lt_Area.Text = GetArea_Icons(AreaGlobal, AreaTW, AreaCN);

                    //開賣判斷
                    Int16 SellGlobal = Convert.ToInt16(DT.Rows[0]["SellGlobal"]);
                    Int16 SellTW = Convert.ToInt16(DT.Rows[0]["SellTW"]);
                    Int16 SellCN = Convert.ToInt16(DT.Rows[0]["SellCN"]);

                    //規格符號 + 認證符號
                    this.lt_Icons.Text = GetData_SpecIcons() + GetData_CertIcons();

                    //產品圖集(slide)
                    this.lt_SlidePic.Text = GetData_AllPic(PhotoGroup, Model_No);

                    //標籤判斷
                    string IsNewItem = DT.Rows[0]["IsNewItem"].ToString();
                    string IsRecItem = DT.Rows[0]["IsRecItem"].ToString();
                    string IsStop = DT.Rows[0]["IsStop"].ToString();

                    //是否為新品
                    this.ph_Label_New.Visible = IsNewItem.Equals("Y");
                    ph_Label_Rec.Visible = IsRecItem.Equals("Z");

                    //是否停售
                    this.ph_Label_Stop.Visible = IsStop.Equals("Y");


                    //產品影片(#1)
                    string showVideo_big = GetData_Video("big", "content");
                    string showVideo_small = GetData_Video("small", "content");
                    if (!string.IsNullOrEmpty(showVideo_big))
                    {
                        this.lt_Videos.Text = showVideo_big;
                        this.lt_Videos_S.Text = showVideo_small;

                        tab_info1.Visible = true;
                        data_info1.Visible = true;

                    }

                    //產品特色(#2)
                    string InfoFeature = DT.Rows[0]["InfoFeature"].ToString();
                    if (!string.IsNullOrEmpty(InfoFeature))
                    {
                        this.lt_Feature.Text = InfoFeature;

                        this.tab_info2.Visible = true;
                        this.data_info2.Visible = true;

                    }

                    //應用(#3)
                    string InfoApp = DT.Rows[0]["InfoApp"].ToString();
                    if (!string.IsNullOrEmpty(InfoApp))
                    {
                        this.lt_Application.Text = InfoApp;

                        this.tab_info3.Visible = true;
                        this.data_info3.Visible = true;

                    }

                    //產品規格(#4)
                    string InfoSpec = DT.Rows[0]["InfoSpec"].ToString();
                    if (!string.IsNullOrEmpty(InfoSpec))
                    {
                        this.lt_SpecInfo.Text = InfoSpec;

                        this.tab_info4.Visible = true;
                        this.data_info4.Visible = true;

                    }

                    //尺寸示意圖
                    this.lt_SpecPic.Text = string.IsNullOrEmpty(DT.Rows[0]["SpecPic"].ToString())
                        ? ""
                        : "<img src=\"{0}\" class=\"img-responsive\" alt=\"\" />".FormatThis(
                            fn_stringFormat.ashx_Pic("ProductPic/{0}/{1}/{2}".FormatThis(Model_No, "13", DT.Rows[0]["SpecPic"].ToString()))
                         );

                    //FAQ
                    int _cntFAQ = Convert.ToInt32(DT.Rows[0]["CntFAQ"]);
                    ph_FAQ.Visible = !_cntFAQ.Equals(0);


                    //一般下載
                    int _dwFile = Convert.ToInt32(DT.Rows[0]["CntDwFile"]);
                    ph_download.Visible = !_dwFile.Equals(0);

                    //商城輔圖下載
                    int _mallPic = Convert.ToInt32(DT.Rows[0]["CntMallPic"]);
                    ph_mallPic.Visible = !_mallPic.Equals(0);

                    //圖片集下載
                    if (string.IsNullOrEmpty(fn_Param.MemberID))
                    {
                        this.ph_gallery.Visible = false;
                    }


                    /*
                      * 立即購買網址
                      * type=direct:直接導購, 開新視窗
                      * type=frame:有多選項, 開小視窗提供選擇
                      * 停售商品 = Y, Contact us表單
                      */

                    //未開賣訊息
                    bool _lockMsg = false;
                    switch (Req_CountryCode.ToUpper())
                    {
                        case "TW":
                            _lockMsg = SellTW > 0;
                            break;

                        case "CN":
                            _lockMsg = SellCN > 0;
                            break;

                        default:
                            _lockMsg = SellGlobal > 0;
                            break;
                    }

                    if (_lockMsg)
                    {
                        //Show未開賣訊息
                        lt_BuyUrl.Text = "<a class=\"btn btn-more\" data-target=\"#myUnsell\" data-toggle=\"modal\" data-id=\"{0}\">{1}</a>".FormatThis(
                                Model_No
                                , this.GetLocalResourceObject("txt_查看詳情").ToString());
                    }
                    else
                    {
                        //停售判斷
                        if (IsStop.Equals("Y"))
                        {
                            //停售商品顯示與我們聯絡
                            //lt_BuyUrl.Text = "<a class=\"btn btn-more doContact\" data-target=\"#myModalContact\" data-toggle=\"modal\" data-id=\"{0}\" style=\"margin-bottom:10px;\">此商品售完，請聯絡我們</a>".FormatThis(
                            //    Model_No);
                        }
                        else
                        {
                            string buyType = Req_BuyUrl[0];
                            string buyUrl = Req_BuyUrl[1];
                            if (!buyType.Equals("none"))
                            {
                                string btnBuyUrl = fn_Param.Get_BuyRedirectUrl(buyType, buyUrl, Req_CountryCode, Model_No, ModelName);

                                lt_BuyUrl.Text = "<a href=\"{0}\" class=\"btn btn-buy {3}\" {1} style=\"margin-bottom:10px;\">{2}</a>".FormatThis(
                                               btnBuyUrl
                                               , buyType.Equals("frame") ? "data-toggle=\"modal\" data-target=\"#remoteModal-{0}\" data-id=\"{0}\" data-name=\"{1}\"".FormatThis(Model_No, HttpUtility.UrlEncode(ModelName)) : "target=\"_blank\""
                                               , this.GetLocalResourceObject("txt_立即購買").ToString()
                                               , buyType.Equals("frame") ? "doRemoteUrl" : ""
                                           );
                            }

                        }
                    }


                    #region -- Log記錄 --
                    fn_Log.writeLog(
                           fn_Param.MemberID
                           , Model_No
                           , "1002"
                           , "產品明細頁查看{0}".FormatThis(Model_No)
                           );
                    #endregion

                }

            }
        }
        catch (Exception)
        {
            throw;
            //throw new Exception("系統發生錯誤 - 基本資料");
        }
    }


    /// <summary>
    /// 回傳各區域是否有此品號
    /// </summary>
    /// <param name="area1">Global</param>
    /// <param name="area2">TW</param>
    /// <param name="area3">CN</param>
    /// <returns></returns>
    private string GetArea_Icons(Int16 area1, Int16 area2, Int16 area3)
    {
        StringBuilder html = new StringBuilder();

        html.Append("{0}".FormatThis(area1 > 0 ? "<li class=\"area-item\">Global</li>" : ""));
        html.Append("{0}".FormatThis(area2 > 0 ? "<li class=\"area-item\">Taiwan</li>" : ""));
        html.Append("{0}".FormatThis(area3 > 0 ? "<li class=\"area-item\">China</li>" : ""));

        return html.ToString();
    }


    /// <summary>
    /// 取得認證符號
    /// </summary>
    /// <returns></returns>
    private string GetData_CertIcons()
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 資料查詢
            SBSql.AppendLine(" SELECT Icon_Pics.Pic_File ");
            SBSql.AppendLine(" FROM [ProductCenter].dbo.Prod_Certification Base WITH (NOLOCK) ");
            SBSql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Certification_Detail Sub WITH (NOLOCK) ON Base.Cert_ID = Sub.Cert_ID ");
            SBSql.AppendLine("  INNER JOIN [ProductCenter].dbo.Icon_Rel_Certification Rel WITH (NOLOCK) ON Rel.Cert_ID = Sub.Cert_ID AND Rel.Detail_ID = Sub.Detail_ID ");
            SBSql.AppendLine("  INNER JOIN [ProductCenter].dbo.Icon_Pics WITH (NOLOCK) ON Rel.Pic_ID = Icon_Pics.Pic_ID ");
            SBSql.AppendLine("  INNER JOIN [ProductCenter].dbo.Icon WITH (NOLOCK) ON Icon_Pics.Icon_ID = Icon.Icon_ID ");
            SBSql.AppendLine(" WHERE (Base.Model_No = @Model_No) AND (Icon.Display = 'Y') AND (Sub.Cert_ValidDate >= GETDATE() OR Sub.Cert_ValidDate IS NULL)");
            SBSql.AppendLine(" AND (Icon_Pics.Pic_ID IN ( ");
            SBSql.AppendLine("  SELECT Pic_ID FROM Prod_Rel_CertIcon ");
            SBSql.AppendLine("  WHERE (Model_No = @DataID) ");
            SBSql.AppendLine(" )) ");
            SBSql.AppendLine(" GROUP BY Icon_Pics.Pic_File");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);
            cmd.Parameters.AddWithValue("Model_No", Model_No);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                //組合html
                StringBuilder html = new StringBuilder();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    html.Append("<li><img src=\"{0}\" class=\"img-responsive\" height=\"30\" style=\"height:30px\" alt=\"Cert\" /></li>".FormatThis(
                        fn_stringFormat.ashx_Pic("Icons/{0}".FormatThis(DT.Rows[row]["Pic_File"].ToString()))
                        ));

                }

                return html.ToString();
            }

        }
    }


    /// <summary>
    /// 取得規格符號
    /// </summary>
    /// <returns></returns>
    private string GetData_SpecIcons()
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 資料查詢
            SBSql.AppendLine(" SELECT Icon_Pics.Pic_File ");
            SBSql.AppendLine("  FROM Icon WITH (NOLOCK) ");
            SBSql.AppendLine("      INNER JOIN Icon_Pics WITH (NOLOCK) ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
            SBSql.AppendLine("      INNER JOIN Icon_Rel_PKWeb Rel WITH (NOLOCK) ON Icon_Pics.Pic_ID = Rel.Pic_ID AND Rel.Model_No = @Model_No ");
            SBSql.AppendLine(" WHERE (Icon.Icon_Type = 'Product') AND (Display = 'Y') ");
            SBSql.AppendLine(" ORDER BY Icon.Sort, Icon.Icon_ID ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Model_No", Model_No);
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                //組合html
                StringBuilder html = new StringBuilder();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    html.Append("<li><img src=\"{0}\" class=\"img-responsive\" alt=\"Spec\" /></li>".FormatThis(
                        fn_stringFormat.ashx_Pic("Icons/{0}".FormatThis(DT.Rows[row]["Pic_File"].ToString()))
                        ));
                }

                return html.ToString();
            }
        }
    }


    /// <summary>
    /// 取得產品主圖
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <returns></returns>
    private string GetData_MainPic(string PhotoGroup, string Model_No)
    {
        //判斷參數
        if (string.IsNullOrEmpty(Model_No))
        {
            return "";
        }

        //拆解圖片值 "|"
        string Photo = "";
        string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");
        for (int row = 0; row < strAry.Length; row++)
        {
            if (false == string.IsNullOrEmpty(strAry[row].ToString()))
            {
                Photo = strAry[row].ToString();
                break;
            }
        }

        //判斷是否有圖片
        if (string.IsNullOrEmpty(Photo))
        {
            return "{0}images/NoPic.png".FormatThis(Application["WebUrl"]);
        }
        else
        {
            return "{3}ProductPic/{0}/{1}/{2}".FormatThis(Model_No, "1", Photo, Application["File_WebUrl"]);
        }
    }


    /// <summary>
    /// 取得圖片集
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <param name="type">Slide or PicList</param>
    /// <remarks>
    /// 圖片要給直接路徑
    /// </remarks>
    private string GetData_AllPic(string PhotoGroup, string Model_No)
    {
        //判斷參數
        if (string.IsNullOrEmpty(Model_No))
        {
            return "";
        }


        //拆解圖片值 "|"
        StringBuilder Photo = new StringBuilder();
        string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");

        Photo.Append("<ul class=\"slides\">");

        //加入影片 
        Photo.Append(GetData_Video("normal", "slider"));


        for (int row = 0; row < strAry.Length; row++)
        {
            if (false == string.IsNullOrEmpty(strAry[row].ToString()))
            {
                string picUrl = "{0}ProductPic/{1}/{2}/{3}".FormatThis(Application["File_WebUrl"].ToString(), Model_No, "1", strAry[row].ToString());

                //組合圖片集
                Photo.Append("<li data-thumb=\"{0}\"><a class=\"venobox\" data-gall=\"myGallery\" href=\"{0}\"><img src=\"{0}\" alt=\"\" /></a></li>".FormatThis(
                        picUrl));
            }
        }

        Photo.Append("</ul>");

        return Photo.ToString();
    }


    /// <summary>
    /// 取得產品影片
    /// </summary>
    /// <returns></returns>
    private string GetData_Video(string size, string pos)
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 資料查詢
            SBSql.AppendLine(" SELECT GP.Group_ID");
            SBSql.AppendLine("  , myData.PV_ID, myData.PV_Title, myData.PV_PubDate, myData.PV_Pic, myData.PV_UriSrc, myData.PV_Uri");
            SBSql.AppendLine(" FROM PV_Group GP");
            SBSql.AppendLine("  INNER JOIN PV_Area Area ON GP.Group_ID = Area.Group_ID");
            SBSql.AppendLine("  INNER JOIN PV myData ON GP.Group_ID = myData.Group_ID");
            SBSql.AppendLine(" WHERE (GP.Display = 'Y')");
            SBSql.AppendLine("  AND (Area.AreaCode = @Area) AND (LOWER(myData.LangCode) = LOWER(@Lang))");
            SBSql.AppendLine("  AND GP.Group_ID IN (");
            SBSql.AppendLine("   SELECT Group_ID FROM PV_Group_Rel_ModelNo WHERE (UPPER(Model_No) = UPPER(@Model_No))");
            SBSql.AppendLine("  )");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Area", fn_Area.PKWeb_Area);
            cmd.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);
            cmd.Parameters.AddWithValue("Model_No", Model_No);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                //組合html
                StringBuilder html = new StringBuilder();

                if (pos.Equals("content"))
                {
                    //內容區
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        string url = DT.Rows[row]["PV_Uri"].ToString();
                        if (size.ToLower().Equals("big"))
                        {
                            html.Append("<iframe src=\"{0}\" frameborder=\"0\" allowfullscreen></iframe>".FormatThis(url));
                        }
                        else
                        {
                            html.Append("<embed src=\"{0}\" frameborder=\"0\" allowfullscreen=\"true\"  webkitallowfullscreen=\"true\"></embed>".FormatThis(url));
                        }

                    }
                }
                else
                {
                    //產品圖片List
                    string picUrl = fn_stringFormat.ashx_Pic("{0}PV/{1}/{2}".FormatThis(
                        System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]
                        , DT.Rows[0]["Group_ID"].ToString()
                        , DT.Rows[0]["PV_Pic"].ToString()));

                    string videoUrl = DT.Rows[0]["PV_Uri"].ToString();

                    //Html
                    html.Append("<li class='video' data-thumb=\"#picurl#\">");
                    html.Append("<a class=\"venobox\" data-gall=\"myGallery\" data-vbtype=\"iframe\" href=\"#videourl#\">");
                    html.Append("<img src=\"#picurl#\" />");
                    html.Append("</a></li>");

                    html.Replace("#picurl#", picUrl);
                    html.Replace("#videourl#", videoUrl);
                }


                return html.ToString();
            }

        }
    }


    /// <summary>
    /// 判斷編號是否正確
    /// </summary>
    /// <param name="ProdID">產品編號</param>
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
    /// 取得產品關鍵字
    /// </summary>
    /// <returns></returns>
    private string GetData_Tags(string dataID)
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 資料查詢
            SBSql.AppendLine("SELECT Tags.Tag_Name TagName");
            SBSql.AppendLine(" FROM Prod GP ");
            SBSql.AppendLine("     INNER JOIN Prod_Rel_Tags RelTag ON GP.Model_No = RelTag.Model_No ");
            SBSql.AppendLine("     INNER JOIN Prod_Tags Tags ON RelTag.Tag_ID = Tags.Tag_ID ");
            SBSql.AppendLine("WHERE (GP.Model_No = @Model_No) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Model_No", dataID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                //Get tags
                var tags = string.Join(",", DT.AsEnumerable()
                    .Select(x => x["TagName"].ToString())
                    .ToArray());


                return tags;
            }

        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 購買網址
    /// </summary>
    public string[] Req_BuyUrl
    {
        get
        {
            return fn_Param.Get_BuyUrl(Req_CountryCode);
            //return fn_Param.Get_BuyUrl("CN");
        }
        set
        {
            this._Req_BuyUrl = value;
        }
    }
    private string[] _Req_BuyUrl;


    /// <summary>
    /// 國家區碼
    /// </summary>
    public string Req_CountryCode
    {
        get
        {
            return fn_Param.GetCountryCode_fromCookie();
        }
        set
        {
            this._Req_CountryCode = value;
        }
    }
    private string _Req_CountryCode;

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Convert.ToString(Page.RouteData.Values["DataID"]);

            return DataID;
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 設定品號
    /// </summary>
    private string _Model_No;
    public string Model_No
    {
        get;
        set;
    }

    private string _Model_Name;
    public string Model_Name
    {
        get;
        set;
    }

    private short _AreaGlobal;
    public short AreaGlobal
    {
        get;
        set;
    }

    private short _AreaTW;
    public short AreaTW
    {
        get;
        set;
    }

    private short _AreaCN;
    public short AreaCN
    {
        get;
        set;
    }

    /// <summary>
    /// [參數] - Ref路徑
    /// </summary>
    private string _Param_FileWebUrl;
    public string Param_FileWebUrl
    {
        get
        {
            return Application["File_WebUrl"].ToString();
        }
        set
        {
            this._Param_FileWebUrl = value;
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

    /// <summary>
    /// 依不同身份產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get
        {
            return fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);
        }
        set
        {
            this._Token = value;
        }
    }
    #endregion

    #region -- Meta --

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

    public string meta_DescSeo
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

    public string meta_Keyword
    {
        get;
        set;
    }
    #endregion
}