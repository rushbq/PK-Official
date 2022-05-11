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
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;

public partial class Dealer_Edit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("220", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "200", "220"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 洲別
                if (fn_CustomUI.Get_Region(this.ddl_AreaCode, "", true, out ErrMsg) == false)
                {
                    this.ddl_AreaCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 狀態
                if (fn_CustomUI.Get_PubDisp(this.rbl_Display, "Y", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[參數判斷] - 判斷是否有資料編號
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData();
                    this.ph_myBlock.Visible = true;
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
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得/檢查參數] - 系統編號
            if (string.IsNullOrEmpty(Param_thisID))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Page_SearchUrl);
                return;
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Base.* ");
                SBSql.AppendLine("  , AreaName.AreaCode, CountryName.Country_Code ");
                SBSql.AppendLine("  , Tel.Tel_ID, Tel.Tel");
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = Base.Create_Who)) AS Create_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = Base.Update_Who)) AS Update_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine(" FROM Dealer Base ");
                SBSql.AppendLine("  INNER JOIN Geocode_AreaName AreaName ON Base.AreaCode = AreaName.AreaCode AND LOWER(AreaName.LangCode) = LOWER('zh-tw') ");
                SBSql.AppendLine("  INNER JOIN Geocode_CountryName CountryName ON Base.Country_Code = CountryName.Country_Code AND LOWER(CountryName.LangCode) = LOWER('zh-tw') ");
                SBSql.AppendLine("  LEFT JOIN Dealer_Tel Tel ON Base.Dealer_ID = Tel.Dealer_ID");
                SBSql.AppendLine(" WHERE (Base.Dealer_ID = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_SearchUrl);
                        return;
                    }
                    else
                    {
                        string _country = DT.Rows[0]["Country_Code"].ToString();
                        string _city = DT.Rows[0]["Region_Code"].ToString();

                        //[填入資料]
                        this.lb_DataID.Text = DT.Rows[0]["Dealer_ID"].ToString();
                        this.ddl_AreaCode.SelectedValue = DT.Rows[0]["AreaCode"].ToString();

                        getCountry(_country);
                        getCity(_city);

                        this.tb_Dealer_Name.Text = DT.Rows[0]["Dealer_Name"].ToString();
                        this.tb_Dealer_Location.Text = DT.Rows[0]["Dealer_Location"].ToString();
                        this.tb_Dealer_Lat.Text = DT.Rows[0]["Dealer_Lat"].ToString();
                        this.tb_Dealer_Lng.Text = DT.Rows[0]["Dealer_Lng"].ToString();
                        this.tb_Dealer_Contact.Text = DT.Rows[0]["Dealer_Contact"].ToString();
                        this.tb_Dealer_Fax.Text = DT.Rows[0]["Dealer_Fax"].ToString();
                        this.tb_Dealer_Email.Text = DT.Rows[0]["Dealer_Email"].ToString();
                        this.tb_Dealer_Website.Text = DT.Rows[0]["Dealer_Website"].ToString();
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();
                        this.tb_Remark.Text = DT.Rows[0]["Remark"].ToString();

                        //維護資訊
                        this.lt_CreateInfo.Text = "<p class=\"form-control-static help-block\">Created on <code>{0}</code> by <code>{1}</code></p>"
                            .FormatThis(
                                DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm")
                                , DT.Rows[0]["Create_Name"].ToString()
                            );
                        if (!string.IsNullOrEmpty(DT.Rows[0]["Update_Time"].ToString()))
                        {
                            this.lt_UpdateInfo.Text = "<p class=\"form-control-static help-block\">Last updated on <code>{0}</code> by <code>{1}</code></p>"
                                .FormatThis(
                                    DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm")
                                    , DT.Rows[0]["Update_Name"].ToString()
                                );
                        }

                        //電話
                        if (!string.IsNullOrEmpty(DT.Rows[0]["Tel_ID"].ToString()))
                        {
                            //填入動態項目 Html
                            StringBuilder itemHtml = new StringBuilder();
                            for (int row = 0; row < DT.Rows.Count; row++)
                            {
                                //取得參數
                                string TagID = DT.Rows[row]["Tel_ID"].ToString();
                                string TagName = DT.Rows[row]["Tel"].ToString();

                                //組合Html
                                itemHtml.AppendLine("<li id=\"li_{0}\" style=\"padding-top:5px;\">".FormatThis(row));
                                itemHtml.Append("<input type=\"hidden\" class=\"item_ID\" value=\"{0}\" />".FormatThis(TagID));
                                itemHtml.Append("<input type=\"hidden\" class=\"item_Name\" value=\"{0}\" />".FormatThis(TagName));
                                itemHtml.Append("<a href=\"javascript:Delete_Item('{0}');\" class=\"btn btn-success\">{1}&nbsp;<i class=\"fa fa-trash\"></i></a>"
                                    .FormatThis(row, TagName));
                                itemHtml.AppendLine("</li>");
                            }

                            this.lt_myItems.Text = itemHtml.ToString();
                        }

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.ph_Delete.Visible = true;

                        //帶出圖片集
                        LookupData_Block();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }

    /// <summary>
    /// 圖片集資料顯示
    /// </summary>
    private void LookupData_Block()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Pic_ID, Pic_File, Sort ");
                SBSql.AppendLine(" FROM Dealer_Photos ");
                SBSql.AppendLine(" WHERE (Dealer_ID = @DataID) ");
                SBSql.AppendLine(" ORDER BY Sort, Pic_ID ASC ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.btn_SaveBlock1.Visible = false;
                        this.btn_SaveBlock2.Visible = false;
                    }

                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 圖片集！", "");
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[取得參數] - 編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
                    //[取得參數] - 檔案名稱
                    string GetThisFile = ((HiddenField)e.Item.FindControl("hf_OldFile")).Value;

                    //[SQL] - 刪除資料
                    SBSql.AppendLine(" DELETE FROM Dealer_Photos WHERE (Dealer_ID = @DataID) AND (Pic_ID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("DataID", Param_thisID);
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("刪除失敗！", "");
                    }
                    else
                    {
                        //刪除檔案
                        IOManage.DelFile(Param_FileFolder, GetThisFile);

                        //頁面跳至本頁
                        Response.Redirect(Page_CurrentUrl);
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得參數資料
                string myFile = DataBinder.Eval(dataItem.DataItem, "Pic_File").ToString();

                //取得控制項
                Literal lt_FileThumb = (Literal)e.Item.FindControl("lt_FileThumb");
                Literal lt_dwUrl = (Literal)e.Item.FindControl("lt_dwUrl");

                if (!string.IsNullOrEmpty(myFile))
                {
                    //顯示縮圖
                    lt_FileThumb.Text = "<img src=\"{0}Scripts/lazyload/grey.gif\" data-original=\"{1}\" width=\"180\" alt=\"{2}\" class=\"lazy\" />"
                        .FormatThis(
                            Application["WebUrl"]
                            , Param_WebFolder + myFile
                            , myFile);

                    lt_dwUrl.Text = "<a href=\"{0}\" class=\"btn btn-info zoomPic\" data-gall=\"myGallery\" title=\"\"><i class=\"fa fa-eye\"></i></a>"
                        .FormatThis(Param_WebFolder + myFile);
                }

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }


    protected void ddl_AreaCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        //取得國家
        getCountry("");
    }

    protected void ddl_Country_SelectedIndexChanged(object sender, EventArgs e)
    {
        //取得城市
        getCity("");
    }

    private void getCountry(string value)
    {
        if (!fn_CustomUI.Get_Country(ddl_Country, value, ddl_AreaCode.SelectedValue, true, out ErrMsg))
        {
            this.ddl_Country.Items.Insert(0, new ListItem("選單產生失敗", ""));
        }
    }

    private void getCity(string value)
    {
        if (!fn_CustomUI.Get_City(ddl_City, value, ddl_Country.SelectedValue, true, out ErrMsg))
        {
            this.ddl_City.Items.Insert(0, new ListItem("選單產生失敗", ""));
        }
    }

    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Dealer_Name.Text, "1", "150", out ErrMsg) == false)
            {
                SBAlert.Append("「經銷商名稱」請輸入1 ~ 75個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data();
                    break;

                case "EDIT":
                    Edit_Data();
                    break;

                default:
                    throw new Exception("走錯路囉!");
            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            int NewID;

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Dealer_ID) ,0) + 1 FROM Dealer ");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" SELECT @NewID AS NewID");

            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                NewID = Convert.ToInt32(DT.Rows[0]["NewID"]);
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO Dealer( ");
            SBSql.AppendLine("  Dealer_ID, AreaCode, Country_Code, Region_Code, Dealer_Name");
            SBSql.AppendLine("  , Dealer_Location, Dealer_Lat, Dealer_Lng, Dealer_Fax, Dealer_Email, Dealer_Website, Dealer_Contact");
            SBSql.AppendLine("  , Remark, Display, Sort");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @NewID, @AreaCode, @Country_Code, @Region_Code, @Dealer_Name");
            SBSql.AppendLine("  , @Dealer_Location, @Dealer_Lat, @Dealer_Lng, @Dealer_Fax, @Dealer_Email, @Dealer_Website, @Dealer_Contact");
            SBSql.AppendLine("  , @Remark, @Display, @Sort");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" );");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("AreaCode", this.ddl_AreaCode.SelectedValue);
            cmd.Parameters.AddWithValue("Country_Code", this.ddl_Country.SelectedValue);
            cmd.Parameters.AddWithValue("Region_Code", this.ddl_City.SelectedValue);
            cmd.Parameters.AddWithValue("Dealer_Name", this.tb_Dealer_Name.Text);
            cmd.Parameters.AddWithValue("Dealer_Location", this.tb_Dealer_Location.Text);
            cmd.Parameters.AddWithValue("Dealer_Lat", this.tb_Dealer_Lat.Text);
            cmd.Parameters.AddWithValue("Dealer_Lng", this.tb_Dealer_Lng.Text);
            cmd.Parameters.AddWithValue("Dealer_Fax", this.tb_Dealer_Fax.Text);
            cmd.Parameters.AddWithValue("Dealer_Email", this.tb_Dealer_Email.Text);
            cmd.Parameters.AddWithValue("Dealer_Website", this.tb_Dealer_Website.Text);
            cmd.Parameters.AddWithValue("Dealer_Contact", this.tb_Dealer_Contact.Text);
            cmd.Parameters.AddWithValue("Remark", this.tb_Remark.Text);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}Dealer/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(NewID.ToString(), Application["DesKey"].ToString())
                        );

            //關聯設定
            if (false == Set_Tel(NewID.ToString()))
            {
                fn_Extensions.JsAlert("電話資料新增失敗！", thisUrl);
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(thisUrl);
            }
        }

    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Dealer ");
            SBSql.AppendLine(" SET ");
            SBSql.AppendLine("  AreaCode = @AreaCode, Country_Code = @Country_Code, Region_Code = @Region_Code, Dealer_Name = @Dealer_Name");
            SBSql.AppendLine("  , Dealer_Location = @Dealer_Location, Dealer_Lat = @Dealer_Lat, Dealer_Lng = @Dealer_Lng");
            SBSql.AppendLine("  , Dealer_Fax = @Dealer_Fax, Dealer_Email = @Dealer_Email, Dealer_Website = @Dealer_Website, Dealer_Contact = @Dealer_Contact");
            SBSql.AppendLine("  , Remark = @Remark, Display = @Display, Sort = @Sort");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Dealer_ID = @DataID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("AreaCode", this.ddl_AreaCode.SelectedValue);
            cmd.Parameters.AddWithValue("Country_Code", this.ddl_Country.SelectedValue);
            cmd.Parameters.AddWithValue("Region_Code", this.ddl_City.SelectedValue);
            cmd.Parameters.AddWithValue("Dealer_Name", this.tb_Dealer_Name.Text);
            cmd.Parameters.AddWithValue("Dealer_Location", this.tb_Dealer_Location.Text);
            cmd.Parameters.AddWithValue("Dealer_Lat", this.tb_Dealer_Lat.Text);
            cmd.Parameters.AddWithValue("Dealer_Lng", this.tb_Dealer_Lng.Text);
            cmd.Parameters.AddWithValue("Dealer_Fax", this.tb_Dealer_Fax.Text);
            cmd.Parameters.AddWithValue("Dealer_Email", this.tb_Dealer_Email.Text);
            cmd.Parameters.AddWithValue("Dealer_Website", this.tb_Dealer_Website.Text);
            cmd.Parameters.AddWithValue("Dealer_Contact", this.tb_Dealer_Contact.Text);
            cmd.Parameters.AddWithValue("Remark", this.tb_Remark.Text);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //關聯設定
            if (false == Set_Tel(Param_thisID))
            {
                fn_Extensions.JsAlert("電話資料新增失敗！", Page_CurrentUrl);
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(Page_CurrentUrl);
            }
        }
    }

    /// <summary>
    /// 資料刪除
    /// </summary>
    protected void lbtn_Delete_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" DELETE FROM Dealer_Tel WHERE (Dealer_ID = @DataID); ");
                SBSql.AppendLine(" DELETE FROM Dealer_Photos WHERE (Dealer_ID = @DataID); ");
                SBSql.AppendLine(" DELETE FROM Dealer WHERE (Dealer_ID = @DataID); ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //刪除整個Folder
                    string fileUrl = @"{0}Support\Dealer\{1}\".FormatThis(Application["File_DiskUrl"], Param_thisID);
                    IOManage.DelFolder(fileUrl);

                    //導向列表
                    Response.Redirect(Page_SearchUrl);
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 資料刪除", "");
            return;
        }

    }

    /// <summary>
    /// 關聯設定
    /// </summary>
    /// <param name="DataID">單頭資料編號</param>
    /// <returns></returns>
    private bool Set_Tel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.tb_All_itemID.Text;
        string Get_Names = this.tb_All_itemName.Text;

        //判斷是否為空
        if (string.IsNullOrEmpty(Get_IDs) || string.IsNullOrEmpty(Get_Names))
        {
            return true;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(Get_IDs, @"\|{2}");
        string[] strAry_Name = Regex.Split(Get_Names, @"\|{2}");

        //宣告暫存清單
        List<DyTempParam> ITempList = new List<DyTempParam>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new DyTempParam(strAry_ID[row], strAry_Name[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID,
                        Name = el.tmp_Name
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID,
                        Name = gp.Key.Name
                    };

        //處理Tag資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            SBSql.AppendLine(" DELETE FROM Dealer_Tel WHERE (Dealer_ID = @DataID) ");
            SBSql.AppendLine(" Declare @New_ID AS INT ");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                //新增關聯

                SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Tel_ID), 0) + 1 FROM Dealer_Tel); ");
                SBSql.AppendLine(" INSERT INTO Dealer_Tel( ");
                SBSql.AppendLine("  Dealer_ID, Tel_ID, Tel");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @New_ID, @Input_Name_{0} ".FormatThis(row));
                SBSql.AppendLine(" ); ");

                cmd.Parameters.AddWithValue("Input_Name_" + row, item.Name);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", DataID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                Response.Write(ErrMsg);
                return false;
            }
            else
            {
                return true;
            }

        }

    }
    #endregion -- 資料編輯 End --

    #region -- 資料編輯(區塊) Start --

    /// <summary>
    /// 區塊設定存檔
    /// </summary>
    protected void btn_BlockSave_Click(object sender, EventArgs e)
    {
        try
        {
            #region --檔案處理--
            //副檔名檢查參數
            int errExt = 0;

            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();
            HttpFileCollection hfc = Request.Files;
            for (int i = 0; i <= hfc.Count - 1; i++)
            {
                HttpPostedFile hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpf);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        ITempList.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hpf, ""));
                    }
                    else
                    {
                        errExt++;
                    }
                }
            }

            //未符合檔案規格的警示訊息
            if (errExt > 0)
            {
                fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //--- 開始新增資料 ---
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 資料新增
                SBSql.AppendLine(" Declare @New_ID AS INT ");
                for (int row = 0; row < ITempList.Count; row++)
                {
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM Dealer_Photos) ");
                    SBSql.AppendLine(" INSERT INTO Dealer_Photos( ");
                    SBSql.AppendLine("  Pic_ID, Dealer_ID, Pic_File, Sort");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @DataID, @FileNewName_{0}, 999".FormatThis(row));
                    SBSql.AppendLine(" ); ");
                    cmd.Parameters.AddWithValue("FileNewName_" + row, ITempList[row].Param_Pic);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("圖片集上傳失敗！", Page_CurrentUrl);
                    return;
                }

                //[IO] - 儲存檔案
                for (int row = 0; row < ITempList.Count; row++)
                {
                    HttpPostedFile hpf = ITempList[row].Param_hpf;
                    if (hpf.ContentLength > 0)
                    {
                        IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, Param_Width, Param_Height);
                    }
                }

                //導向本頁
                Response.Redirect(Page_CurrentUrl);

            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 圖片集", "");
            return;
        }

    }

    /// <summary>
    /// 儲存版面排序
    /// </summary>
    protected void btn_SaveSort_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("請先上傳圖片資料", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                for (int row = 0; row < lvDataList.Items.Count; row++)
                {
                    //[取得參數] - 編號
                    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Sort")).Text;

                    SBSql.AppendLine(" UPDATE Dealer_Photos SET Sort = @lvParam_Sort_{0}".FormatThis(row));
                    SBSql.AppendLine(" WHERE (Dealer_ID = @Dealer_ID) AND (Pic_ID = @lvParam_ID_{0}) ".FormatThis(row));

                    cmd.Parameters.AddWithValue("lvParam_ID_" + row, lvParam_ID);
                    cmd.Parameters.AddWithValue("lvParam_Sort_" + row, lvParam_Sort);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Dealer_ID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("儲存版面失敗！", "");
                }
                else
                {
                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存版面排序", "");
            return;
        }
    }

    #endregion -- 資料編輯(區塊) End --

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Param_thisID = value;
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
            return "{0}Dealer/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString()))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}Dealer/Search/".FormatThis(Application["WebUrl"]);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }


    /// <summary>
    /// [參數] - 檔案資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null
                ? this._Param_FileFolder
                : @"{0}Support\Dealer\{1}\".FormatThis(Application["File_DiskUrl"], Param_thisID);
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 檔案Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null
                ? this._Param_WebFolder
                : @"{0}Support/Dealer/{1}/".FormatThis(Application["File_WebUrl"], Param_thisID);
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// 座標 - Lat
    /// </summary>
    private double _Param_Lat;
    public double Param_Lat
    {
        get
        {
            return string.IsNullOrEmpty(this.tb_Dealer_Lat.Text) ? 24.9839922 : Convert.ToDouble(this.tb_Dealer_Lat.Text);
        }
        set
        {
            this._Param_Lat = value;
        }
    }

    /// <summary>
    /// 座標 - Lng
    /// </summary>
    private double _Param_Lng;
    public double Param_Lng
    {
        get
        {
            return string.IsNullOrEmpty(this.tb_Dealer_Lat.Text) ? 121.53484649999996 : Convert.ToDouble(this.tb_Dealer_Lng.Text);
        }
        set
        {
            this._Param_Lng = value;
        }
    }
    #endregion

    #region -- 上傳參數 --

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 1280;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 1024;
        }
        set
        {
            this._Param_Height = value;
        }
    }


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
        }

        /// <summary>
        /// [參數] - 圖片類別
        /// </summary>
        private string _Param_FileKind;
        public string Param_FileKind
        {
            get { return this._Param_FileKind; }
            set { this._Param_FileKind = value; }
        }

        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Pic">系統檔名</param>
        /// <param name="Param_OrgPic">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf, string Param_FileKind)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
            this._Param_FileKind = Param_FileKind;
        }

    }
    #endregion

    #region -- 暫存參數 --
    /// <summary>
    /// 暫存參數
    /// </summary>
    public class DyTempParam
    {
        /// <summary>
        /// [參數] - 編號
        /// </summary>
        private string _tmp_ID;
        public string tmp_ID
        {
            get { return this._tmp_ID; }
            set { this._tmp_ID = value; }
        }

        /// <summary>
        /// [參數] - 名稱
        /// </summary>
        private string _tmp_Name;
        public string tmp_Name
        {
            get { return this._tmp_Name; }
            set { this._tmp_Name = value; }
        }


        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        /// <param name="tmp_Name">名稱</param>
        public DyTempParam(string tmp_ID, string tmp_Name)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Name = tmp_Name;
        }
    }
    #endregion

}