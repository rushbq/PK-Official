using ExtensionMethods;
using ExtensionUI;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class myTagEvent_TagSearch : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_Tag;

                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";
                tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("txt_Verify").ToString());

                //取得資料
                LookupDataList();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --
    /// <summary>
    /// 取得資料列表
    /// </summary>
    private void LookupDataList()
    {
        string ErrMsg;

        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        try
        {
            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            #region - [SQL] 資料顯示 -
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("    RTRIM(myData.Model_No) AS ModelNo, RTRIM(myData.Model_Name_{0}) AS ModelName".FormatThis(fn_Language.Param_Lang));
            //是否為新品
            SBSql.AppendLine("    , (CASE WHEN (DATEDIFF(DAY, GP.StartTime, GETDATE()) <= 365) AND (GP.IsNew = 'Y') THEN 'Y' ELSE 'N' END) AS IsNewItem");
            SBSql.AppendLine("    , (CASE WHEN (GP.IsNew = 'Z') THEN 'Z' ELSE 'N' END) AS IsRecItem");
            //是否已停售
            SBSql.AppendLine("    , (CASE WHEN GETDATE() > myData.Stop_Offer_Date THEN 'Y' ELSE 'N' END) AS IsStop");
            //圖片(判斷圖片中心 2->1->3->4->5->7->8->9)
            SBSql.AppendLine("      , (SELECT TOP 1 (ISNULL(Pic02,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'') ");
            SBSql.AppendLine("          + '|' + ISNULL(Pic05,'') + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')) AS PicGroup");
            SBSql.AppendLine("          FROM [ProductCenter].dbo.ProdPic_Photo WITH (NOLOCK) WHERE (ProdPic_Photo.Model_No = myData.Model_No)");
            SBSql.AppendLine("      ) AS PhotoGroup ");

            SBSql.AppendLine("    , ROW_NUMBER() OVER (ORDER BY (CASE WHEN GETDATE() > myData.Stop_Offer_Date THEN 'Y' ELSE 'N' END), GP.IsNew DESC, GP.Sort ASC, GP.EndTime DESC) AS RowRank ");
            //取得該品號在各區域是否有上架資料
            SBSql.AppendLine("    , (SELECT COUNT(*) FROM Prod_Rel_Area WHERE (Model_No = GP.Model_No) AND (AreaCode = 1)) AreaGlobal");
            SBSql.AppendLine("    , (SELECT COUNT(*) FROM Prod_Rel_Area WHERE (Model_No = GP.Model_No) AND (AreaCode = 2)) AreaTW");
            SBSql.AppendLine("    , (SELECT COUNT(*) FROM Prod_Rel_Area WHERE (Model_No = GP.Model_No) AND (AreaCode = 3)) AreaCN");

            //取得該品號在各區域是否有開賣資料
            SBSql.AppendLine("    , (SELECT COUNT(*) FROM Prod_Rel_SellArea WHERE (Model_No = GP.Model_No) AND (AreaCode = 1)) SellGlobal");
            SBSql.AppendLine("    , (SELECT COUNT(*) FROM Prod_Rel_SellArea WHERE (Model_No = GP.Model_No) AND (AreaCode = 2)) SellTW");
            SBSql.AppendLine("    , (SELECT COUNT(*) FROM Prod_Rel_SellArea WHERE (Model_No = GP.Model_No) AND (AreaCode = 3)) SellCN");

            //Tag Banner Pic
            SBSql.AppendLine("    , ISNULL((SELECT TOP 1 Tag_Pic FROM Prod_Tags WHERE (UPPER(Tag_Name) = UPPER(@TagName))), '') AS TagPic");

            SBSql.AppendLine("    FROM Prod GP ");
            SBSql.AppendLine("      INNER JOIN [ProductCenter].dbo.Prod_Item myData WITH (NOLOCK) ON GP.Model_No = myData.Model_No ");
            SBSql.AppendLine(" WHERE (GP.Display = 'Y') ");
            SBSql.AppendLine("   AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");

            //--tag
            SBSql.AppendLine(" AND (GP.Model_No IN (");
            SBSql.AppendLine("  SELECT RelTag.Model_No");
            SBSql.AppendLine("  FROM Prod_Rel_Tags RelTag");
            SBSql.AppendLine("   INNER JOIN Prod_Tags Tags ON RelTag.Tag_ID = Tags.Tag_ID ");
            SBSql.AppendLine("  WHERE (UPPER(RelTag.LangCode) = UPPER(@Lang)) AND (UPPER(Tags.Tag_Name) = UPPER(@TagName)) ");
            SBSql.AppendLine("  ))");

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" ORDER BY RowRank");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("TagName", Req_TagName);
            cmd.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);

            #endregion

            //[SQL] - 取得資料            
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();

                if (DT.Rows.Count > 0)
                {
                    string TagPic = DT.Rows[0]["TagPic"].ToString();
                    lt_HeaderContent1.Text = String.IsNullOrWhiteSpace(TagPic) ? ""
                        : "<img src=\"{0}\" class=\"img-responsive\" alt=\"banner\" style=\"margin-bottom: 20px;\" />"
                        .FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"] + "Tag/" + TagPic);
                }

            }
        }
        catch (Exception)
        {
            throw;
        }

        finally
        {
            if (cmd != null)
                cmd.Dispose();
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料
            string Get_IsNewItem = DataBinder.Eval(e.Item.DataItem, "IsNewItem").ToString();
            string Get_IsRecItem = DataBinder.Eval(e.Item.DataItem, "IsRecItem").ToString();
            string Get_IsStop = DataBinder.Eval(e.Item.DataItem, "IsStop").ToString();
            string Get_ModelNo = DataBinder.Eval(e.Item.DataItem, "ModelNo").ToString();
            string Get_ModelName = DataBinder.Eval(e.Item.DataItem, "ModelName").ToString();

            //區域判斷
            Int16 AreaGlobal = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "AreaGlobal"));
            Int16 AreaTW = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "AreaTW"));
            Int16 AreaCN = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "AreaCN"));

            //開賣判斷
            Int16 SellGlobal = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "SellGlobal"));
            Int16 SellTW = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "SellTW"));
            Int16 SellCN = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "SellCN"));

            //判斷是否為新品
            if (Get_IsNewItem.Equals("Y"))
            {
                PlaceHolder ph_NewItem = (PlaceHolder)e.Item.FindControl("ph_NewItem");
                ph_NewItem.Visible = true;
            }

            //判斷是否為推薦
            if (Get_IsRecItem.Equals("Z"))
            {
                PlaceHolder ph_RecmItem = (PlaceHolder)e.Item.FindControl("ph_RecmItem");
                ph_RecmItem.Visible = true;
            }

            //判斷是否已停售
            if (Get_IsStop.Equals("Y"))
            {
                PlaceHolder ph_Stop = (PlaceHolder)e.Item.FindControl("ph_Stop");
                ph_Stop.Visible = true;
            }

            //填入上架區域
            Literal lt_Area = (Literal)e.Item.FindControl("lt_Area");
            lt_Area.Text = GetArea_Icons(AreaGlobal, AreaTW, AreaCN);


            /*
             * 立即購買網址
             * type=direct:直接導購, 開新視窗
             * type=frame:有多選項, 開小視窗提供選擇
             * 停售商品 = Y, Contact us表單
             * frame ex: /Ajax_Data/Frame_GoBuy.aspx?area=CN&id=1PK-3179&name=%e6%92%ac%e6%a3%92%e5%b7%a5%e5%85%b75%e6%94%af%e7%b5%84
             */
            Literal lt_BuyUrl = (Literal)e.Item.FindControl("lt_BuyUrl");

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
                        Get_ModelNo
                        , this.GetLocalResourceObject("txt_查看詳情").ToString());
            }
            else
            {
                //停售判斷
                if (Get_IsStop.Equals("Y"))
                {
                    //停售商品顯示與我們聯絡
                    lt_BuyUrl.Text = "<a class=\"btn btn-more doContact\" data-target=\"#myModalContact\" data-toggle=\"modal\" data-id=\"{0}\">{1}</a>".FormatThis(
                        Get_ModelNo
                        , this.GetLocalResourceObject("txt_停售說明").ToString());
                }
                else
                {
                    /*
                     * 偵測ip, 取得對應的區域URL
                     */
                    string buyType = Req_BuyUrl[0];
                    string buyUrl = Req_BuyUrl[1];
                    if (!buyType.Equals("none"))
                    {
                        string btnBuyUrl = fn_Param.Get_BuyRedirectUrl(buyType, buyUrl, Req_CountryCode, Get_ModelNo, Get_ModelName);

                        lt_BuyUrl.Text = "<a href=\"{0}\" class=\"btn btn-buy {3}\" {1}>{2}</a>".FormatThis(
                                btnBuyUrl
                                , buyType.Equals("frame") ? "data-toggle=\"modal\" data-target=\"#remoteModal-{0}\" data-id=\"{0}\" data-name=\"{1}\"".FormatThis(Get_ModelNo, HttpUtility.UrlEncode(Get_ModelName)) : "target=\"_blank\""
                                , this.GetLocalResourceObject("txt_立即購買").ToString()
                                , buyType.Equals("frame") ? "doRemoteUrl" : ""
                            );
                    }
                }
            }

        }
    }


    /// <summary>
    /// 取得產品圖
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <returns></returns>
    protected string Get_Pic(string PhotoGroup, string Model_No)
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
            return "<img data-original=\"{0}images/NoPic.png\" src=\"{0}js/lazyload/grey.gif\" class=\"lazy img-responsive\" alt=\"\" />".FormatThis(
                Application["WebUrl"]);
        }
        else
        {
            //實際檔案資料夾路徑
            string fileRealPath = string.Format("ProductPic/{0}/{1}/{2}"
                , Model_No
                , "1"
                , "500x500_{0}".FormatThis(Photo));

            //下載路徑 
            //string downloadPath = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"lazy img-responsive\" alt=\"\" />".FormatThis(
            //        fn_stringFormat.ashx_Pic(fileRealPath)
            //        , Application["WebUrl"]
            //    );
            string downloadPath = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"lazy img-responsive\" alt=\"\" />".FormatThis(
                   Application["File_WebUrl"] + fileRealPath
                    , Application["WebUrl"]
                );

            return downloadPath;
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


    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 商品詢問
    /// </summary>
    protected void btn_SendContact_Click(object sender, EventArgs e)
    {
        try
        {
            string _model = hf_ModelNo.Value;
            string _name = tb_Name.Text.Trim();
            string _email = tb_Email.Text.Trim();
            string _message = "【停售商品詢問】 " + _model + " \n\n" + tb_Message.Text.Trim();

            //[檢查驗證碼]
            string ImgCheckCode = Request.Cookies["ImgCheckCode"].Value;
            if (!this.tb_VerifyCode.Text.ToUpper().Equals(ImgCheckCode))
            {
                this.tb_VerifyCode.Text = "";
                fn_Extensions.JsAlert("{0} {1}".FormatThis(
                        this.GetLocalResourceObject("txt_Verify").ToString()
                        , this.GetLocalResourceObject("tip_error").ToString()
                        )
                    , "");
                return;
            }

            //[寫入資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                string TraceID = Cryptograph.GetCurrentTime().ToString();

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(InquiryID) ,0) + 1 FROM Inquiry ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增留言資料
                SBSql.AppendLine(" INSERT INTO Inquiry( ");
                SBSql.AppendLine("  InquiryID, Mem_ID, Class_ID, Message");
                SBSql.AppendLine("  , Status, Create_Time, TraceID, AreaCode");
                SBSql.AppendLine("  , MsgEmail, MsgWho");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @NewID, @Mem_ID, @Class_ID, @Message");
                SBSql.AppendLine("  , 1, GETDATE(), @TraceID, @AreaCode");
                SBSql.AppendLine("  , @MsgEmail, @MsgWho");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Class_ID", 7); //停售商品詢問
                cmd.Parameters.AddWithValue("Mem_ID", 0);
                cmd.Parameters.AddWithValue("Message", _message);
                cmd.Parameters.AddWithValue("TraceID", TraceID);
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("MsgEmail", _email);
                cmd.Parameters.AddWithValue("MsgWho", _name);
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    //失敗
                    fn_Extensions.JsAlert("Oops!", this.ViewState["Page_Url"].ToString());
                    return;
                }
            }

            //OK
            fn_Extensions.JsAlert("Thank you!", this.ViewState["Page_Url"].ToString());
        }
        catch (Exception)
        {

            throw;
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
    /// 取得傳遞參數 - Tag
    /// </summary>
    private string _Req_TagName;
    public string Req_TagName
    {
        get
        {
            var tagName = Page.RouteData.Values["tagName"];
            String DataID = (tagName == null ? "" : tagName.ToString());

            return DataID;
        }
        set
        {
            this._Req_TagName = value;
        }
    }

    #endregion

}