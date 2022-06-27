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

public partial class Country_Edit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("450", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "400", "450"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 功能區塊
                if (fn_CustomUI.Get_ProdArea(this.ddl_ProdArea, Request["Area"], true, out ErrMsg) == false)
                {
                    this.ddl_ProdArea.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 類型:工具/玩具
                if (fn_CustomUI.Get_ProdType(this.ddl_ProdType, "", true, out ErrMsg) == false)
                {
                    this.ddl_ProdType.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 產品類別
                if (fn_CustomUI.Get_ProdAllClass(this.ddl_ProdClass, "", true, out ErrMsg) == false)
                {
                    this.ddl_ProdClass.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //帶出資料
                LookupData();

                ddl_ProdClass.Attributes.Add("disabled", "disabled");
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
                string sql = @"
	                SELECT Base.SeqNo, Base.Subject, Base.Content1
	                , Base.LangCode
	                , Base.功能區塊
	                , Base.產品類型
	                , Base.關聯編號
	                , ROW_NUMBER() OVER (ORDER BY Base.產品類型, Base.關聯編號) AS RowRank
	                FROM [產品宣傳] Base
	                 INNER JOIN [產品宣傳參數] cls1 ON Base.功能區塊 = cls1.Class_MenuID AND cls1.Class_Type = 'A'
	                 INNER JOIN [產品宣傳參數] cls2 ON Base.產品類型 = cls2.Class_MenuID AND cls2.Class_Type = 'B'
	                WHERE (Base.SeqNo = @DataID)";

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ph_DetailMsg.Visible = true;
                        ph_Detail.Visible = false;
                    }
                    else
                    {
                        ph_DetailMsg.Visible = false;
                        ph_Detail.Visible = true;

                        ddl_ProdArea.SelectedValue = DT.Rows[0]["功能區塊"].ToString();
                        ddl_ProdType.SelectedValue = DT.Rows[0]["產品類型"].ToString();
                        ddl_ProdClass.SelectedValue = DT.Rows[0]["關聯編號"].ToString();
                        rbl_Lang.SelectedValue = DT.Rows[0]["LangCode"].ToString();
                        tb_Subject.Text = DT.Rows[0]["Subject"].ToString();
                        tb_Content1.Text = DT.Rows[0]["Content1"].ToString();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }


    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --

    protected void btn_BaseSave_Click(object sender, EventArgs e)
    {
        //取得設定欄位
        string p功能區塊 = ddl_ProdArea.SelectedValue;
        string p產品類型 = ddl_ProdType.SelectedValue;
        string p關聯編號 = ddl_ProdClass.SelectedValue;
        string p語系 = rbl_Lang.SelectedValue;


        try
        {
            string sql = @"
                    DECLARE @DataID AS INT, @IsNew AS CHAR(1)

                    IF (
                     SELECT COUNT(*)
                     FROM [產品宣傳]
                     WHERE (功能區塊 = @p功能區塊) AND (產品類型 = @p產品類型) AND (關聯編號 = @p關聯編號) AND (LangCode = @p語系)
                    ) = 0
	                    BEGIN
	                     SET @IsNew = 'Y';
	                     SET @DataID = (
	                      SELECT ISNULL(MAX(SeqNo), 0) + 1
	                      FROM [產品宣傳]
	                     )
	                     INSERT INTO [產品宣傳] (
		                     SeqNo, 功能區塊, 產品類型, 關聯編號
		                    , LangCode, Subject
		                    , Create_Who, Create_Time
	                     ) VALUES (
		                     @DataID, @p功能區塊, @p產品類型, @p關聯編號
		                    , @p語系, '新的設定,請修改名稱'
		                    , @Who, GETDATE()
	                     );
	                    END
                    ELSE
	                    BEGIN
	                     SET @IsNew = 'N';
	                     SET @DataID = (
	                      SELECT SeqNo
	                      FROM [產品宣傳]
	                      WHERE (功能區塊 = @p功能區塊) AND (產品類型 = @p產品類型) AND (關聯編號 = @p關聯編號) AND (LangCode = @p語系)
	                     );
	                    END

                    SELECT @DataID AS DataID, @IsNew AS IsNew
                    ";
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("p功能區塊", p功能區塊);
                cmd.Parameters.AddWithValue("p產品類型", p產品類型);
                cmd.Parameters.AddWithValue("p關聯編號", string.IsNullOrWhiteSpace(p關聯編號) ? "0" : p關聯編號);
                cmd.Parameters.AddWithValue("p語系", p語系);
                cmd.Parameters.AddWithValue("Who", fn_Param.UserGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT == null)
                    {
                        fn_Extensions.JsAlert("broken", Page_CurrentUrl);
                        return;
                    }

                    Int64 dataID = Convert.ToInt64(DT.Rows[0]["DataID"]);
                    string isNew = DT.Rows[0]["IsNew"].ToString();

                    //更新本頁Url
                    string thisUrl = "{0}Prod/BannerEdit/{1}/".FormatThis(
                            Application["WebUrl"]
                            , dataID
                            );
                    Response.Redirect(thisUrl);

                }

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - Base Setting", "");
            return;
        }

    }



    protected void btn_DetailSave_Click(object sender, EventArgs e)
    {
        string subject = tb_Subject.Text;
        string content1 = tb_Content1.Text;

        #region "..欄位檢查.."
        StringBuilder SBAlert = new StringBuilder();

        if (fn_Extensions.String_資料長度Byte(subject, "1", "140", out ErrMsg) == false)
        {
            SBAlert.Append("「識別名稱」請輸入1 ~ 70個字\\n");
        }

        //[JS] - 判斷是否有警示訊息
        if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
        {
            fn_Extensions.JsAlert(SBAlert.ToString(), "");
            return;
        }
        #endregion


        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = @"UPDATE 產品宣傳
                        SET Subject = @Subject, Content1 = @Content1
                        WHERE (SeqNo = @SeqNo)";

            //[SQL] - Command
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("Subject", subject);
            cmd.Parameters.AddWithValue("Content1", content1);
            cmd.Parameters.AddWithValue("SeqNo", Param_thisID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }
    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            ////宣告
            //StringBuilder SBSql = new StringBuilder();
            //string pic1 = this.hf_OldFile.Value;

            ////取得圖片參數
            //var queryPic = from el in ITempList
            //               select new
            //               {
            //                   NewPic = el.Param_Pic,
            //                   PicKind = el.Param_FileKind
            //               };
            //foreach (var item in queryPic)
            //{
            //    if (item.PicKind.Equals("P1"))
            //    {
            //        pic1 = item.NewPic;
            //    }
            //}


            ////[SQL] - 清除參數設定
            //cmd.Parameters.Clear();

            ////[SQL] - 資料更新
            //SBSql.AppendLine(" UPDATE Geocode_CountryCode ");
            //SBSql.AppendLine(" SET AreaCode = @AreaCode ");
            //SBSql.AppendLine("  , Country_Flag = @Country_Flag, Display = @Display");
            //SBSql.AppendLine(" WHERE (Country_Code = @Country_Code) ");

            ////[SQL] - 各語系新增
            //SBSql.AppendLine(" DELETE FROM Geocode_CountryName WHERE (Country_Code = @Country_Code); ");

            //for (int row = 0; row < this.lvDataList.Items.Count; row++)
            //{
            //    //[取得參數] 
            //    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
            //    string lvParam_Name = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Country_Name")).Text;

            //    SBSql.AppendLine(" INSERT INTO Geocode_CountryName( ");
            //    SBSql.AppendLine("  LangCode, Country_Code, Country_Name");
            //    SBSql.AppendLine(" ) VALUES ( ");
            //    SBSql.AppendLine("  @LangCode_{0}, @Country_Code, @Country_Name_{0}".FormatThis(row));
            //    SBSql.AppendLine(" );");

            //    cmd.Parameters.AddWithValue("LangCode_" + row, lvParam_ID);
            //    cmd.Parameters.AddWithValue("Country_Name_" + row, lvParam_Name);
            //}

            ////[SQL] - Command
            //cmd.CommandText = SBSql.ToString();
            //cmd.Parameters.AddWithValue("AreaCode", this.ddl_AreaCode.SelectedValue);
            //cmd.Parameters.AddWithValue("Country_Flag", pic1);
            //cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            //cmd.Parameters.AddWithValue("Country_Code", Param_thisID);
            //if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            //{
            //    fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
            //    return;
            //}

            ////導向本頁
            //Response.Redirect(Page_CurrentUrl);
        }
    }


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

            return DataID;
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
            return "{0}Prod/BannerEdit/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "" : Param_thisID
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
                Url = "{0}Prod/BannerSearch/".FormatThis(Application["WebUrl"]);
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

}