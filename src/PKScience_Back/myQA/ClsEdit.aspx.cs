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

public partial class ClsEdit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("610", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "600", "610"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 狀態(預設Y)
                if (fn_CustomUI.Get_PubDisp(this.rbl_Display, "Y", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //帶出資料
                LookupData();

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
                SBSql.AppendLine(" SELECT Base.Class_ID, Base.Class_Name, Base.Display, Base.Sort");
                SBSql.AppendLine("  , Lang.LangCode, Lang.LangName ");
                SBSql.AppendLine(" FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("     LEFT JOIN FAQ_Class Base ON Lang.LangCode = Base.LangCode AND Base.Class_ID = @DataID");
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
                        //取得資料
                        string Class_ID = DT.Rows[0]["Class_ID"].ToString();
                        

                        //Flag設定 & 欄位顯示/隱藏
                        if (!string.IsNullOrEmpty(Class_ID))
                        {
                            this.hf_flag.Value = "Edit";

                            this.lb_DataID.Text = Class_ID;
                            this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                            this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();
                        }
                        else
                        {
                            this.hf_flag.Value = "Add";

                            this.lb_DataID.Text = "NEW";
                            this.rbl_Display.SelectedValue = "N";
                            this.tb_Sort.Text = "999";
                        }

                        //帶出語系資料
                        this.lvDataList.DataSource = DT.DefaultView;
                        this.lvDataList.DataBind();
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
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
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

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Class_ID) ,0) + 1 FROM FAQ_Class ");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" SELECT @NewID AS NewID");

            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                NewID = Convert.ToInt32(DT.Rows[0]["NewID"]);
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 各語系新增
            for (int row = 0; row < this.lvDataList.Items.Count; row++)
            {
                //[取得參數] 
                string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                string lvParam_Name = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Class_Name")).Text;

                SBSql.AppendLine(" INSERT INTO FAQ_Class( ");
                SBSql.AppendLine("  LangCode, Class_ID, Class_Name, Display, Sort");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @LangCode_{0}, @Class_ID, @Class_Name_{0}, @Display, @Sort".FormatThis(row));
                SBSql.AppendLine(" );");

                cmd.Parameters.AddWithValue("LangCode_" + row, lvParam_ID);
                cmd.Parameters.AddWithValue("Class_Name_" + row, lvParam_Name);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Class_ID", NewID);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}QAClass/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(NewID.ToString(), fn_Param.DesKey)
                        );

            //導向本頁
            Response.Redirect(thisUrl);
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
          

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 各語系新增

            for (int row = 0; row < this.lvDataList.Items.Count; row++)
            {
                //[取得參數] 
                string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                string lvParam_Name = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Class_Name")).Text;

                SBSql.AppendLine(" UPDATE FAQ_Class");
                SBSql.AppendLine(" SET Display = @Display, Sort = @Sort, Class_Name = @Class_Name_{0}".FormatThis(row));
                SBSql.AppendLine(" WHERE (Class_ID = @Class_ID) AND (LangCode = @LangCode_{0}); ".FormatThis(row));

                cmd.Parameters.AddWithValue("LangCode_" + row, lvParam_ID);
                cmd.Parameters.AddWithValue("Class_Name_" + row, lvParam_Name);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Class_ID", Param_thisID);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);

            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
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

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, fn_Param.DesKey);
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
            return "{0}QAClass/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, fn_Param.DesKey))
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
                Url = "{0}QAClass/Search/".FormatThis(Application["WebUrl"]);
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