using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class myController_Ascx_Adv : System.Web.UI.UserControl
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //載入Adv橫幅資料
                LookupData_Adv();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// Adv橫幅資料顯示
    /// </summary>
    private void LookupData_Adv()
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
                SBSql.AppendLine(" SELECT myData.Adv_ID, myData.Group_ID, myData.Adv_Title, myData.Adv_Pic, myData.Adv_Uri, GP.Adv_Target ");
                SBSql.AppendLine(" FROM Adv_Group GP ");
                SBSql.AppendLine(" INNER JOIN Adv_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine(" INNER JOIN Adv myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("  AND (GP.Adv_Position = 1)");
                SBSql.AppendLine(" ORDER BY GP.Sort ASC, GP.EndTime DESC");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.ph_Adv.Visible = false;
                        return;
                    }
                    //宣告
                    StringBuilder html_target = new StringBuilder();
                    StringBuilder html_item = new StringBuilder();
                    int idx = 0;

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //取得參數
                        string GetGroupID = DT.Rows[row]["Group_ID"].ToString();
                        string GetPic = DT.Rows[row]["Adv_Pic"].ToString();
                        string GetUri = DT.Rows[row]["Adv_Uri"].ToString();
                        string GetTarget = DT.Rows[row]["Adv_Target"].ToString();

                        if (!string.IsNullOrEmpty(GetPic))
                        {
                            string ShowPic = "{0}Adv/{1}/{2}".FormatThis(Application["File_WebUrl"] + Param_FileWebFolder, GetGroupID, GetPic);
                            //目前顯示的li小圓點
                            html_target.Append("<li data-target=\"#banner-pc\" data-slide-to=\"{0}\" class=\"{1}\"></li>".FormatThis(
                                    idx
                                    , idx.Equals(0) ? "active" : ""
                                ));
                          
                            //廣告圖片
                            html_item.Append("<div class=\"item {0}\">".FormatThis(idx.Equals(0) ? "active" : ""));
                            if (string.IsNullOrEmpty(GetUri))
                            {
                                html_item.Append("<img src=\"{0}\" />".FormatThis(ShowPic));
                            }
                            else
                            {
                                html_item.Append("<a href=\"{1}\" target=\"{2}\"><img src=\"{0}\" /></a>".FormatThis(
                                    ShowPic
                                    , GetUri
                                    , GetTarget));
                            }
                            html_item.Append("</div>");

                            idx++;
                        }
                    }

                    //顯示Html
                    this.lt_AdvTarget.Text = html_target.ToString();
                    this.lt_AdvItems.Text = html_item.ToString();

                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Adv");
        }
    }

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 檔案Web根目錄路徑
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
    /// [參數] - 檔案Web資料夾
    /// </summary>
    private string _Param_FileWebFolder;
    public string Param_FileWebFolder
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"];
        }
        set
        {
            this._Param_FileWebFolder = value;
        }
    }

    #endregion
}