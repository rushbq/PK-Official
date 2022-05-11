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

public partial class myNews_PromoList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_促銷活動;

                //取得資料
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
                SBSql.AppendLine(" SELECT myData.Promo_ID, myData.Group_ID, myData.Promo_Title, myData.Promo_Pic_S, myData.Promo_Pic_B");
                SBSql.AppendLine(" FROM Promo_Group GP ");
                SBSql.AppendLine("  INNER JOIN Promo_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine("  INNER JOIN Promo myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine(" ORDER BY GP.Sort ASC");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Promo");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetPic_S = DataBinder.Eval(e.Item.DataItem, "Promo_Pic_S").ToString();
            string GetPic_B = DataBinder.Eval(e.Item.DataItem, "Promo_Pic_B").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();

            if (!string.IsNullOrEmpty(GetPic_S))
            {
                //取得控制項
                Literal lt_Pic_S = (Literal)e.Item.FindControl("lt_Pic_S");

                //顯示Html
                lt_Pic_S.Text = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"fixImg lazy\" alt=\"\" width=\"70\" />".FormatThis(
                        fn_stringFormat.ashx_Pic("{0}Promo/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic_S))
                        , Application["WebUrl"]
                    );
            }
            if (!string.IsNullOrEmpty(GetPic_B))
            {
                //取得控制項
                Literal lt_Pic_B = (Literal)e.Item.FindControl("lt_Pic_B");

                //顯示Html
                lt_Pic_B.Text = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"img-responsive fixImg lazy\" alt=\"\" />".FormatThis(
                        fn_stringFormat.ashx_Pic("{0}Promo/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic_B))
                        , Application["WebUrl"]
                    );
            }
        }
    }
    #endregion

    #region -- 參數設定 --
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