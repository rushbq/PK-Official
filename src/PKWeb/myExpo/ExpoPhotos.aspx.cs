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

public partial class myExpo_ExpoPhotos : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_展覽活動;

                //取得資料
                LookupData_Expo();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// Expo 資料顯示
    /// </summary>
    private void LookupData_Expo()
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
                SBSql.AppendLine(" SELECT Base.Expo_Title, Base.Expo_PubDate, Base.Group_ID, Base.Expo_ID ");
                SBSql.AppendLine("  , Sub.Pic_File AS Expo_Pic ");
                SBSql.AppendLine(" FROM Expo Base ");
                SBSql.AppendLine("  INNER JOIN Expo_Photos Sub ON Base.Expo_ID = Sub.Expo_ID ");
                SBSql.AppendLine(" WHERE (Base.Expo_ID = @DataID) ");
                SBSql.AppendLine(" ORDER BY Sub.Sort ASC, Sub.Pic_ID");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    if (DT.Rows.Count > 0)
                    {
                        //填入資料
                        this.lt_Title.Text = DT.Rows[0]["Expo_Title"].ToString();
                        this.lt_BackUrl.Text = "<a href=\"{0}\">Back</a>".FormatThis(
                                Application["WebUrl"] + "Expo/" + DT.Rows[0]["Expo_PubDate"].ToString().ToDateString("yyyy")
                            );

                    }

                }

            }
        }
        catch (Exception)
        {
            throw;
            throw new Exception("系統發生錯誤 - Expo");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetPic = DataBinder.Eval(e.Item.DataItem, "Expo_Pic").ToString();
            string GetID = DataBinder.Eval(e.Item.DataItem, "Expo_ID").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();

            if (!string.IsNullOrEmpty(GetPic))
            {
                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                //顯示Html
                lt_Pic.Text = "<a class=\"zoomPic\" data-gall=\"myGallery\" title=\"\" href=\"{0}\"><img class=\"lazy\" src=\"{1}js/lazyload/grey.gif\" data-original=\"{0}\" width=\"200\" alt=\"\" /></a>"
                    .FormatThis(
                        fn_stringFormat.ashx_Pic("{0}Expo/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
                        , Application["WebUrl"]
                    );
            }

        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return string.IsNullOrEmpty(DataID) ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Req_DataID = value;
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