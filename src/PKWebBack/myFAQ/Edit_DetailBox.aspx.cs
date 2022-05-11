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


public partial class Edit_DetailBox : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("230", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("權限不足", "script:parent.$.fancybox.close();");
                    return;
                }

                //[必要參數判斷]
                if (string.IsNullOrEmpty(Param_parentID) || string.IsNullOrEmpty(Param_thisID))
                {
                    fn_Extensions.JsAlert("錯誤的操作，請重新開啟", "script:parent.$.fancybox.close();");
                    return;
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

                //[SQL] - 資料查詢
                SBSql.Append(" SELECT Base.FAQ_ID, Base.Block_ID");
                SBSql.Append(" , Base.Block_Title, Base.Block_Desc");
                SBSql.Append(" FROM FAQ_Block Base ");
                SBSql.Append(" WHERE (Base.FAQ_ID = @ParentID) AND (Base.Block_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ParentID", Param_parentID);
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "");
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Block_Title.Text = DT.Rows[0]["Block_Title"].ToString();
                        this.tb_Block_Desc.Text = HttpUtility.HtmlDecode(DT.Rows[0]["Block_Desc"].ToString());

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料顯示");
        }
    }

    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_BlockSave_Click(object sender, EventArgs e)
    {
        try
        {

            #region "..資料儲存.."
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                string getTitle = string.IsNullOrEmpty(this.tb_Block_Title.Text) ? "" : this.tb_Block_Title.Text;
                string getContent = string.IsNullOrEmpty(this.tb_Block_Desc.Text) ? "" : this.tb_Block_Desc.Text;

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料更新
                SBSql.Append(" UPDATE FAQ_Block ");
                SBSql.Append(" SET Block_Title = @Block_Title, Block_Desc = @Block_Content");
                SBSql.Append(" WHERE (FAQ_ID = @ParentID) AND (Block_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("ParentID", Param_parentID);
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("Block_Title", getTitle);
                cmd.Parameters.AddWithValue("Block_Content", HttpUtility.HtmlEncode(getContent));
               
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                    return;
                }

                //導向主頁
                fn_Extensions.JsAlert("", "script:parent.$.fancybox.close();");

            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 區塊設定存檔", "");
            return;
        }

    }


    #endregion -- 資料編輯 End --

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 單頭資料編號
    /// </summary>
    private string _Param_parentID;
    public string Param_parentID
    {
        get
        {
            String DataID = Page.RouteData.Values["ParentID"].ToString();

            return DataID;
        }
        set
        {
            this._Param_parentID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
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
            return "{0}FAQ/Edit/DetailBox/{1}/{2}".FormatThis(
                 Application["WebUrl"]
                 , Param_parentID
                 , Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString())
             );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }
    #endregion

    
}