using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

/*
 * 科玩產品
 */
public partial class myToyProd_ProdList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_科學玩具;

                this.btn_Search.Text = Resources.resPublic.btn_查詢;
                this.tb_Keyword.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_關鍵字").ToString());
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";
                tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("txt_Verify").ToString());

                //[取得/檢查參數] - 產品類別(TOY)
                if (fn_CustomUI.Get_ProdToyClass(this.ddl_ProdClass, Req_ClassID, fn_Language.Param_Lang, true, this.GetLocalResourceObject("tip_所有類別").ToString(), out ErrMsg) == false)
                {
                    this.ddl_ProdClass.Items.Insert(0, new ListItem("empty item", ""));
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //取得資料
                LookupDataList(Req_PageIdx);


            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="pageIndex">目前頁數</param>
    private void LookupDataList(int pageIndex)
    {
        string ErrMsg;

        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        try
        {
            //[參數宣告] - 設定本頁Url(末端無須加 "/")
            this.ViewState["Page_Url"] = "{0}RobotKits/{1}".FormatThis(
                    Application["WebUrl"]
                    , (string.IsNullOrEmpty(Req_ClassID)) ? "ALL" : Req_ClassID
                );
            ArrayList Params = new ArrayList();

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 12;  //每頁筆數
            int TotalRow = 0;  //總筆數
            int BgItem = (pageIndex - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數 

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

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

            SBSql.AppendLine("    FROM Prod GP ");
            SBSql.AppendLine("      INNER JOIN [ProductCenter].dbo.Prod_Item myData WITH (NOLOCK) ON GP.Model_No = myData.Model_No");
            //SBSql.AppendLine("      INNER JOIN [ProductCenter].dbo.ProdToy_Class_Rel_ModelNo Rel ON myData.Model_No = Rel.Model_No");
            SBSql.AppendLine(" WHERE (GP.Display = 'Y') ");
            SBSql.AppendLine("   AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");

            #region "..查詢條件.."

            //Toy關聯
            SBSql.AppendLine(" AND (myData.Model_No IN (");
            SBSql.AppendLine("  SELECT Rel.Model_No");
            SBSql.AppendLine("  FROM [ProductCenter].dbo.ProdToy_Class_Rel_ModelNo Rel");
            //[查詢條件] - 產品類別(TOY)
            if (!string.IsNullOrEmpty(Req_ClassID) && (!Req_ClassID.ToUpper().Equals("ALL")))
            {
                SBSql.Append(" WHERE (Rel.Class_ID = @Class_ID)");
                cmd.Parameters.AddWithValue("Class_ID", Req_ClassID);

            }
            SBSql.AppendLine(" ))");


            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(myData.Model_No) LIKE '%' + UPPER(@Keyword) + '%')");
                SBSql.Append("  OR (myData.Model_Name_{0} LIKE '%' + @Keyword + '%')".FormatThis(fn_Language.Param_Lang));

                //--tag
                SBSql.Append(" OR (GP.Model_No IN (");
                SBSql.Append("  SELECT RelTag.Model_No");
                SBSql.Append("  FROM Prod_Rel_Tags RelTag");
                SBSql.Append("   INNER JOIN Prod_Tags Tags ON RelTag.Tag_ID = Tags.Tag_ID ");
                SBSql.Append("  WHERE (UPPER(Tags.Tag_Name) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  ))");

                //--info2
                SBSql.Append(" OR (GP.Model_No IN (");
                SBSql.Append("  SELECT Model_No");
                SBSql.Append("  FROM [ProductCenter].dbo.Prod_Info");
                SBSql.Append("  WHERE (UPPER(Lang) = UPPER(@Lang)) AND (Info2 LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  ))");

                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);
                cmd.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);

                Params.Add("k=" + Server.UrlEncode(Req_Keyword));
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            #endregion


            #region - [SQL] 計算筆數 -

            SBSql.Clear();

            SBSql.AppendLine(" SELECT COUNT(*) FROM (");
            SBSql.AppendLine("     SELECT GP.Model_No");
            SBSql.AppendLine("     FROM Prod GP");
            SBSql.AppendLine("     INNER JOIN [ProductCenter].dbo.Prod_Item myData WITH (NOLOCK) ON GP.Model_No = myData.Model_No");
            SBSql.AppendLine(" WHERE (GP.Display = 'Y') ");
            SBSql.AppendLine("   AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");

            #region "..查詢條件.."

            //Toy關聯
            SBSql.AppendLine(" AND (myData.Model_No IN (");
            SBSql.AppendLine("  SELECT Rel.Model_No");
            SBSql.AppendLine("  FROM [ProductCenter].dbo.ProdToy_Class_Rel_ModelNo Rel");
            //[查詢條件] - 產品類別(TOY)
            if (!string.IsNullOrEmpty(Req_ClassID) && (!Req_ClassID.ToUpper().Equals("ALL")))
            {
                SBSql.Append(" WHERE (Rel.Class_ID = @Class_ID)");
                cmdTotalCnt.Parameters.AddWithValue("Class_ID", Req_ClassID);

            }
            SBSql.AppendLine(" ))");

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(myData.Model_No) LIKE '%' + UPPER(@Keyword) + '%')");
                SBSql.Append("  OR (myData.Model_Name_{0} LIKE '%' + @Keyword + '%')".FormatThis(fn_Language.Param_Lang));

                //--tag
                SBSql.Append(" OR (GP.Model_No IN (");
                SBSql.Append("  SELECT RelTag.Model_No");
                SBSql.Append("  FROM Prod_Rel_Tags RelTag");
                SBSql.Append("   INNER JOIN Prod_Tags Tags ON RelTag.Tag_ID = Tags.Tag_ID ");
                SBSql.Append("  WHERE (UPPER(Tags.Tag_Name) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  ))");

                //--info2
                SBSql.Append(" OR (GP.Model_No IN (");
                SBSql.Append("  SELECT Model_No");
                SBSql.Append("  FROM [ProductCenter].dbo.Prod_Info");
                SBSql.Append("  WHERE (UPPER(Lang) = UPPER(@Lang)) AND (Info2 LIKE '%' + @Keyword + '%') ");
                SBSql.Append("  ))");

                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
                cmdTotalCnt.Parameters.AddWithValue("Lang", fn_Language.PKWeb_Lang);
            }

            #endregion
            
            SBSql.AppendLine("   GROUP BY GP.Model_No");
            SBSql.AppendLine(") AS TbCnt");

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();

            #endregion

            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalRow, out ErrMsg))
            {
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();

                if (DT.Rows.Count > 0)
                {
                    //顯示分頁, 需在DataBind之後
                    this.lt_Pager_top.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);

                    Literal lt_Pager_footer = (Literal)this.lvDataList.FindControl("lt_Pager_footer");
                    lt_Pager_footer.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);
                }

                //[頁數判斷] - 目前頁數大於總頁數, 則導向第一頁
                if (pageIndex > TotalRow && TotalRow > 0)
                {
                    Response.Redirect("{0}/{1}/{2}".FormatThis(
                            this.ViewState["Page_Url"]
                            , 1
                            , "?" + string.Join("&", Params.ToArray())));
                }
                else
                {
                    //重新整理頁面Url
                    this.ViewState["Page_Url"] = "{0}/{1}/{2}".FormatThis(
                        this.ViewState["Page_Url"]
                        , pageIndex
                        , "?" + string.Join("&", Params.ToArray()));

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
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
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
            //實際檔案資料夾路徑(圖片中心的縮圖)
            string fileRealPath = string.Format("ProductPic/{0}/{1}/{2}"
                , Model_No
                , "1"
                , "500x500_{0}".FormatThis(Photo));

            //下載路徑
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
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("{0}RobotKits/{1}/".FormatThis(
                    Application["WebUrl"]
                    , (this.ddl_ProdClass.SelectedIndex > 0) ? this.ddl_ProdClass.SelectedValue : "ALL"
                ));

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("?k=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
        }
    }


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
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    private int _Req_PageIdx;
    public int Req_PageIdx
    {
        get
        {
            int PageID = Convert.ToInt32(Page.RouteData.Values["PageID"]);
            return PageID;
        }
        set
        {
            this._Req_PageIdx = value;
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
            String Keyword = Request.QueryString["k"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - 分類
    /// </summary>
    private string _Req_ClassID;
    public string Req_ClassID
    {
        get
        {
            String DataID = Convert.ToString(Page.RouteData.Values["ClassID"]);

            if (string.IsNullOrEmpty(DataID))
            {
                return "ALL";
            }
            else
            {
                //取得類別名稱
                ClassName = fn_CustomUI.Get_ProdClassName(DataID, fn_Language.Param_Lang);

                return fn_stringFormat.Set_FilterHtml(DataID);
            }
        }
        set
        {
            this._Req_ClassID = value;
        }
    }

    public string ClassName
    {
        get;
        set;
    }
    #endregion

}