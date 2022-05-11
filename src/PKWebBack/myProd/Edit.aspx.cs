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

public partial class Prod_Edit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("410", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "400", "410"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 上架狀態(預設S)
                if (fn_CustomUI.Get_NewsDisp(this.rbl_Display, "S", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }


                //[取得/檢查參數] - 上架區域
                if (fn_CustomUI.Get_Area(this.cbl_Area, null, out ErrMsg) == false)
                {
                    this.cbl_Area.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                if (fn_CustomUI.Get_Area(this.cbl_NoSellArea, null, out ErrMsg) == false)
                {
                    this.cbl_NoSellArea.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[參數判斷] - 判斷是否有資料編號
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData();
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
                SBSql.AppendLine(" SELECT GP.*, Tags.Tag_ID, Tags.Tag_Name ");
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Create_Who)) AS Create_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Update_Who)) AS Update_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , Sub.Catelog_Vol, Sub.Page");
                SBSql.AppendLine("  FROM Prod GP ");
                SBSql.AppendLine("      LEFT JOIN Prod_Rel_Tags RelTag ON GP.Model_No = RelTag.Model_No ");
                SBSql.AppendLine("      LEFT JOIN Prod_Tags Tags ON RelTag.Tag_ID = Tags.Tag_ID ");
                SBSql.AppendLine("      LEFT JOIN [ProductCenter].dbo.Prod_Item Sub ON GP.Model_No = Sub.Model_No");
                SBSql.AppendLine(" WHERE (GP.Prod_ID = @DataID) ");
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
                        //[填入資料]
                        string Model_No = DT.Rows[0]["Model_No"].ToString();
                        this.lb_DataID.Text = DT.Rows[0]["Prod_ID"].ToString();
                        //this.rbl_Area.SelectedValue = DT.Rows[0]["AreaCode"].ToString();
                        this.tb_Model_No.Text = Model_No;
                        this.hf_ModelNo.Value = Model_No;
                        this.hf_myItemVal.Value = DT.Rows[0]["Model_No"].ToString();
                        this.show_sDate.Text = DT.Rows[0]["StartTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.show_eDate.Text = DT.Rows[0]["EndTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_StartDate.Text = DT.Rows[0]["StartTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_EndDate.Text = DT.Rows[0]["EndTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.rbl_IsNew.SelectedValue = DT.Rows[0]["IsNew"].ToString();
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();
                        this.lb_Vol.Text = DT.Rows[0]["Catelog_Vol"].ToString();
                        this.lb_Page.Text = DT.Rows[0]["Page"].ToString();

                        //取得上架區域
                        List<string> queryArea = GetRel_AreaCode(Model_No);
                        CheckBoxList cbl = this.cbl_Area;
                        foreach (var area in queryArea)
                        {
                            for (int col = 0; col < cbl.Items.Count; col++)
                            {
                                if (cbl.Items[col].Value.Equals(area.ToString()))
                                {
                                    cbl.Items[col].Selected = true;
                                }
                            }
                        }

                        //取得未開賣區域
                        List<string> queryNoSellArea = GetRel_NoSellAreaCode(Model_No);
                        CheckBoxList cblUnSell = this.cbl_NoSellArea;
                        foreach (var area in queryNoSellArea)
                        {
                            for (int col = 0; col < cblUnSell.Items.Count; col++)
                            {
                                if (cblUnSell.Items[col].Value.Equals(area.ToString()))
                                {
                                    cblUnSell.Items[col].Selected = true;
                                }
                            }
                        }

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

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.ph_Delete.Visible = true;
                        this.tb_Model_No.ReadOnly = true;
                        this.btn_GetIcon.Visible = false;

                        //填入Tag Html
                        if (!string.IsNullOrEmpty(DT.Rows[0]["Tag_ID"].ToString()))
                        {
                            StringBuilder itemHtml = new StringBuilder();

                            for (int row = 0; row < DT.Rows.Count; row++)
                            {
                                //取得參數
                                string TagID = DT.Rows[row]["Tag_ID"].ToString();
                                string TagName = DT.Rows[row]["Tag_Name"].ToString();

                                //組合Html
                                itemHtml.AppendLine("<li id=\"li_{0}\" style=\"padding-top:5px;\">".FormatThis(row));
                                itemHtml.Append("<input type=\"hidden\" class=\"item_ID\" value=\"{0}\" />".FormatThis(TagID));
                                itemHtml.Append("<input type=\"hidden\" class=\"item_Name\" value=\"{0}\" />".FormatThis(TagName));
                                itemHtml.Append("<a href=\"javascript:Delete_Item('{0}');\" class=\"btn btn-success\">{1}&nbsp;<span class=\"glyphicon glyphicon-trash\"></span></a>"
                                    .FormatThis(row, TagName));
                                itemHtml.AppendLine("</li>");
                            }

                            this.lt_myItems.Text = itemHtml.ToString();
                        }

                        //帶出關聯資料, 認證符號
                        LookupData_Detail(this.tb_Model_No.Text);
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 顯示關聯資料 - 認證符號
    /// </summary>
    private void LookupData_Detail(string ModelNo)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                CheckBoxList cbl = this.cbl_CertIcon;
                cbl.Items.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Icon_Pics.Pic_ID AS ID, Icon_Pics.Pic_File AS Label ");
                SBSql.AppendLine(" , (CASE WHEN RelWeb.Pic_ID IS NULL THEN 'N' ELSE 'Y' END) AS IsChecked");
                SBSql.AppendLine(" FROM Prod_Certification Base WITH (NOLOCK) ");
                SBSql.AppendLine("  INNER JOIN Prod_Certification_Detail Sub WITH (NOLOCK) ON Base.Cert_ID = Sub.Cert_ID ");
                SBSql.AppendLine("  INNER JOIN Icon_Rel_Certification Rel WITH (NOLOCK) ON Rel.Cert_ID = Sub.Cert_ID AND Rel.Detail_ID = Sub.Detail_ID ");
                SBSql.AppendLine("  INNER JOIN Icon_Pics WITH (NOLOCK) ON Rel.Pic_ID = Icon_Pics.Pic_ID ");
                SBSql.AppendLine("  INNER JOIN Icon WITH (NOLOCK) ON Icon_Pics.Icon_ID = Icon.Icon_ID ");
                SBSql.AppendLine("  LEFT JOIN PKWeb.dbo.Prod_Rel_CertIcon RelWeb WITH (NOLOCK) ON Base.Model_No = RelWeb.Model_No AND RelWeb.Pic_ID = Icon_Pics.Pic_ID");
                SBSql.AppendLine(" WHERE (Base.Model_No = @Model_No) AND (Icon.Display = 'Y') ");
                SBSql.AppendLine(" GROUP BY Icon_Pics.Pic_ID, Icon_Pics.Pic_File, RelWeb.Pic_ID ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Model_No", ModelNo);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        cbl.Items.Insert(0, new ListItem("無相關資料顯示", "", false));
                        return;
                    }

                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        cbl.Items.Add(new ListItem("<img src=\"http://ref.prokits.com.tw/Icons/{0}\" height=\"30px\" style=\"height:30px;\" alt=\"{0}\">".FormatThis(DT.Rows[row]["Label"].ToString())
                                     , DT.Rows[row]["ID"].ToString()
                                     ));
                    }

                    //新增時，預設全勾選
                    if (this.hf_flag.Value.ToUpper().Equals("ADD"))
                    {
                        for (int col = 0; col < cbl.Items.Count; col++)
                        {
                            cbl.Items[col].Selected = true;
                        }
                    }
                   
                    //判斷是否有已選取的項目
                    var query = DT.AsEnumerable()
                        .Where(filter => filter.Field<string>("IsChecked").Equals("Y"))
                        .Select(fld => new
                        {
                            ID = fld.Field<int>("ID")
                        });

                    foreach (var item in query)
                    {
                        for (int col = 0; col < cbl.Items.Count; col++)
                        {
                            if (cbl.Items[col].Value.Equals(item.ID.ToString()))
                            {
                                cbl.Items[col].Selected = true;
                            }
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message.ToString());
        }
    }


    /// <summary>
    /// 取得已關聯區域
    /// </summary>
    /// <param name="modelNo"></param>
    /// <returns></returns>
    List<string> GetRel_AreaCode(string modelNo)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT AreaCode");
            SBSql.AppendLine(" FROM Prod_Rel_Area");
            SBSql.AppendLine(" WHERE (Model_No = @Model_No)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Model_No", modelNo);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<string> areaCode = new List<string>();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    areaCode.Add(DT.Rows[row]["AreaCode"].ToString());
                }

                return areaCode;
            }
        }
    }



    /// <summary>
    /// 取得已關聯區域-No Sell
    /// </summary>
    /// <param name="modelNo"></param>
    /// <returns></returns>
    List<string> GetRel_NoSellAreaCode(string modelNo)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT AreaCode");
            SBSql.AppendLine(" FROM Prod_Rel_SellArea");
            SBSql.AppendLine(" WHERE (Model_No = @Model_No)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Model_No", modelNo);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<string> areaCode = new List<string>();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    areaCode.Add(DT.Rows[row]["AreaCode"].ToString());
                }

                return areaCode;
            }
        }
    }


    /// <summary>
    /// 按鈕 - 顯示認證符號
    /// </summary>
    protected void btn_GetIcon_Click(object sender, EventArgs e)
    {
        //判斷空白
        if (string.IsNullOrEmpty(this.hf_myItemVal.Value))
        {
            fn_Extensions.JsAlert("「品號」輸入不正確", "");
            return;
        }

        //帶出資料
        LookupData_Detail(this.tb_Model_No.Text);
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
            if (fn_Extensions.String_資料長度Byte(this.hf_myItemVal.Value, "1", "40", out ErrMsg) == false)
            {
                SBAlert.Append("「品號」輸入不正確\\n");
            }

            //日期區間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            if (getSTime >= getETime)
            {
                SBAlert.Append("「上架日期」\\n{0} ~ {1}\\n不覺得哪裡怪怪的嗎?\\n".FormatThis(getSTime, getETime));
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
                    //判斷重複
                    if (CheckInUse(this.hf_myItemVal.Value))
                    {
                        fn_Extensions.JsAlert("資料重複新增,請確認品號是否正確\\n", "");
                        return;
                    }

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
    /// 判斷重複新增 (品號)
    /// </summary>
    /// <param name="modelNo">品號</param>
    /// <returns>
    /// true = 重複, 不可通過
    /// </returns>
    private bool CheckInUse(string modelNo)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT * FROM Prod WHERE (Model_No = @Model_No) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Model_No", modelNo);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }

        }
        catch (Exception)
        {
            throw;
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
            SBSql.AppendLine("  SELECT ISNULL(MAX(Prod_ID) ,0) + 1 FROM Prod ");
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
            SBSql.AppendLine(" INSERT INTO Prod( ");
            SBSql.AppendLine("  Prod_ID, Model_No, StartTime, EndTime, Display, IsNew, Sort");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @NewID, @Model_No, @StartTime, @EndTime, @Display, @IsNew, @Sort");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" );");

            //上架區域
            var GetArea = from ListItem item in this.cbl_Area.Items where item.Selected select item.Value;
            if (GetArea.Count() > 0)
            {
                int row = 0;

                foreach (var AreaCode in GetArea)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Prod_Rel_Area( ");
                    SBSql.AppendLine("  Model_No, AreaCode");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Model_No, @AreaCode_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AreaCode_{0}".FormatThis(row), AreaCode);
                }
            }

            //未開賣區域
            var GetNoSellArea = from ListItem item in this.cbl_NoSellArea.Items where item.Selected select item.Value;
            if (GetNoSellArea.Count() > 0)
            {
                int row = 0;

                foreach (var AreaCode in GetNoSellArea)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Prod_Rel_SellArea( ");
                    SBSql.AppendLine("  Model_No, AreaCode");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Model_No, @UnSellAreaCode_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("UnSellAreaCode_{0}".FormatThis(row), AreaCode);
                }
            }

            //時間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("StartTime", getSTime);
            cmd.Parameters.AddWithValue("EndTime", getETime);
            cmd.Parameters.AddWithValue("IsNew", this.rbl_IsNew.SelectedValue);

            //認證符號
            var GetIcon = from ListItem item in this.cbl_CertIcon.Items where item.Selected select item.Value;
            if (GetIcon.Count() > 0)
            {
                int row = 0;

                foreach (var Icon in GetIcon)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Prod_Rel_CertIcon( ");
                    SBSql.AppendLine("  Model_No, Pic_ID");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Model_No, @Pic_ID_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("Pic_ID_{0}".FormatThis(row), Icon);
                }
            }


            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("Model_No", this.hf_myItemVal.Value.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}Prod/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(NewID.ToString(), Application["DesKey"].ToString())
                        );

            //Tag 關聯設定
            if (false == Set_TagRel(NewID.ToString()))
            {
                fn_Extensions.JsAlert("關鍵字資料新增失敗！", thisUrl);
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
            SBSql.AppendLine(" UPDATE Prod ");
            SBSql.AppendLine(" SET Model_No = @Model_No ");
            SBSql.AppendLine("  , StartTime = @StartTime, EndTime = @EndTime");
            SBSql.AppendLine("  , Display = @Display, IsNew = @IsNew, Sort = @Sort");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Prod_ID = @DataID) ");

            //先清除資料
            SBSql.AppendLine(" DELETE FROM Prod_Rel_Area WHERE (Model_No = @Model_No);");
            //上架區域
            var GetArea = from ListItem item in this.cbl_Area.Items where item.Selected select item.Value;
            if (GetArea.Count() > 0)
            {
                int row = 0;

                foreach (var AreaCode in GetArea)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Prod_Rel_Area( ");
                    SBSql.AppendLine("  Model_No, AreaCode");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Model_No, @AreaCode_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AreaCode_{0}".FormatThis(row), AreaCode);
                }
            }


            //先清除資料
            SBSql.AppendLine(" DELETE FROM Prod_Rel_SellArea WHERE (Model_No = @Model_No);");
            //未開賣區域
            var GetNoSellArea = from ListItem item in this.cbl_NoSellArea.Items where item.Selected select item.Value;
            if (GetNoSellArea.Count() > 0)
            {
                int row = 0;

                foreach (var AreaCode in GetNoSellArea)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Prod_Rel_SellArea( ");
                    SBSql.AppendLine("  Model_No, AreaCode");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Model_No, @UnSellAreaCode_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("UnSellAreaCode_{0}".FormatThis(row), AreaCode);
                }
            }


            //時間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("StartTime", getSTime);
            cmd.Parameters.AddWithValue("EndTime", getETime);
            cmd.Parameters.AddWithValue("IsNew", this.rbl_IsNew.SelectedValue);


            //認證符號
            var GetIcon = from ListItem item in this.cbl_CertIcon.Items where item.Selected select item.Value;
            if (GetIcon.Count() > 0)
            {
                //先清除資料
                SBSql.AppendLine(" DELETE FROM Prod_Rel_CertIcon WHERE (Model_No = @Model_No);");

                int row = 0;

                foreach (var Icon in GetIcon)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Prod_Rel_CertIcon( ");
                    SBSql.AppendLine("  Model_No, Pic_ID");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Model_No, @Pic_ID_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("Pic_ID_{0}".FormatThis(row), Icon);
                }
            }


            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Model_No", this.hf_myItemVal.Value.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //Tag 關聯設定
            if (false == Set_TagRel(Param_thisID))
            {
                fn_Extensions.JsAlert("關鍵字資料新增失敗！", Page_CurrentUrl);
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
    /// 關聯設定
    /// </summary>
    /// <param name="DataID">單頭資料編號</param>
    /// <returns></returns>
    private bool Set_TagRel(string DataID)
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
        List<TempParam> ITempList = new List<TempParam>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam(strAry_ID[row], strAry_Name[row]));
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

            SBSql.AppendLine(" DELETE FROM Prod_Rel_Tags WHERE (Model_No = @ModelNo) ");
            SBSql.AppendLine(" Declare @New_ID AS INT ");

            int row = 0;
            foreach (var item in query)
            {
                /*
                 * 若ID = 0, 則新增資料(Prod_Tags) & 設定關聯(Prod_Rel_Tags)
                 * ID <> 0, 設定關聯
                 */
                row++;

                //判斷編號
                if (item.ID.Equals("0"))
                {
                    //判斷名稱是否已存在
                    SBSql.AppendLine("IF NOT EXISTS (SELECT * FROM Prod_Tags WHERE (Tag_Name = @Tag_Name_{0}))".FormatThis(row));
                    SBSql.AppendLine(" BEGIN ");

                    //新增Tag
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Tag_ID), 0) + 1 FROM Prod_Tags) ");
                    SBSql.AppendLine(" INSERT INTO Prod_Tags( ");
                    SBSql.AppendLine("  Tag_ID, Tag_Name");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Tag_Name_{0}".FormatThis(row));
                    SBSql.AppendLine(" ); ");

                    //新增關聯
                    SBSql.AppendLine(" INSERT INTO Prod_Rel_Tags( ");
                    SBSql.AppendLine("  Tag_ID, Model_No");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @ModelNo");
                    SBSql.AppendLine(" ); ");

                    SBSql.AppendLine(" END ");

                    cmd.Parameters.AddWithValue("Tag_Name_" + row, item.Name);
                }
                else
                {
                    //新增其他Tag關聯
                    SBSql.AppendLine(" INSERT INTO Prod_Rel_Tags( ");
                    SBSql.AppendLine("  Tag_ID, Model_No");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Tag_ID_{0}, @ModelNo".FormatThis(row));
                    SBSql.AppendLine(" ); ");

                    cmd.Parameters.AddWithValue("Tag_ID_" + row, item.ID);
                }
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            //cmd.Parameters.AddWithValue("DataID", DataID);
            cmd.Parameters.AddWithValue("ModelNo", this.hf_ModelNo.Value);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }

    /// <summary>
    /// 資料刪除
    /// </summary>
    //protected void lbtn_Delete_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        using (SqlCommand cmd = new SqlCommand())
    //        {
    //            StringBuilder SBSql = new StringBuilder();

    //            //[SQL] - 資料更新 
    //            SBSql.AppendLine(" DELETE FROM Prod_Rel_Tags WHERE (Model_No = @ModelNo); ");
    //            SBSql.AppendLine(" DELETE FROM Prod_Rel_CertIcon WHERE (Model_No = @Model_No); ");
    //            SBSql.AppendLine(" DELETE FROM Prod WHERE (Prod_ID = @DataID); ");

    //            //[SQL] - Command
    //            cmd.CommandText = SBSql.ToString();
    //            cmd.Parameters.Clear();
    //            cmd.Parameters.AddWithValue("DataID", Param_thisID);
    //            cmd.Parameters.AddWithValue("ModelNo", this.hf_ModelNo.Value);
    //            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
    //            {
    //                fn_Extensions.JsAlert("刪除失敗！", Page_CurrentUrl);
    //                return;
    //            }
    //            else
    //            {
    //                //導向列表
    //                Response.Redirect(Page_SearchUrl);
    //            }

    //        }

    //    }
    //    catch (Exception)
    //    {
    //        fn_Extensions.JsAlert("系統發生錯誤 - 資料刪除", "");
    //        return;
    //    }

    //}


    #endregion -- 資料編輯 End --

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
            return "{0}Prod/Edit/{1}/".FormatThis(
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
                Url = "{0}Prod/Search/".FormatThis(Application["WebUrl"]);
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

    #endregion

    #region -- 暫存參數 --
    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
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
        public TempParam(string tmp_ID, string tmp_Name)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Name = tmp_Name;
        }
    }
    #endregion


}