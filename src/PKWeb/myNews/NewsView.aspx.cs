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

public partial class myNews_NewsView : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //取得資料
                LookupData_News();

                //** 次標題 **
                this.Page.Title = meta_Title;

                //隱藏主頁的meta
                PlaceHolder myMeta = (PlaceHolder)Master.FindControl("ph_MetaInfo");
                myMeta.Visible = false;
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// News 資料顯示
    /// </summary>
    private void LookupData_News()
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
                SBSql.AppendLine(" SELECT Base.News_Title, Base.News_Desc, Base.News_PubDate, Base.Group_ID, Base.News_ID, Base.News_Pic ");
                SBSql.AppendLine("  , Sub.Block_Desc, Sub.Block_Pic ");
                SBSql.AppendLine(" FROM News Base ");
                SBSql.AppendLine("  INNER JOIN News_Block Sub ON Base.News_ID = Sub.News_ID ");
                SBSql.AppendLine(" WHERE (Base.News_ID = @DataID) ");
                SBSql.AppendLine(" ORDER BY Sub.Sort ASC, Sub.Block_ID");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    if (DT.Rows.Count > 0)
                    {
                        //填入表頭資料(after databind)
                        Literal lt_Title = (Literal)this.lvDataList.FindControl("lt_Title");
                        lt_Title.Text = DT.Rows[0]["News_Title"].ToString();

                        Literal lt_Date = (Literal)this.lvDataList.FindControl("lt_Date");
                        lt_Date.Text = DT.Rows[0]["News_PubDate"].ToString().ToDateString("yyyy/MM/dd");

                        this.lt_BackUrl.Text = "<a href=\"{0}\">Back</a>".FormatThis(
                                Application["WebUrl"] + "News/" + DT.Rows[0]["News_PubDate"].ToString().ToDateString("yyyy")
                            );

                        //Meta資訊
                        meta_Title = "{0} | {1}".FormatThis(DT.Rows[0]["News_Title"].ToString(), Application["WebName"].ToString());
                        meta_Desc = DT.Rows[0]["News_Desc"].ToString().Left(100);//HttpUtility.HtmlDecode(DT.Rows[0]["Block_Desc"].ToString()).Left(100);
                        meta_Url = "{0}News/View/{1}".FormatThis(
                            Application["WebUrl"].ToString()
                            , Cryptograph.MD5Encrypt(DT.Rows[0]["News_ID"].ToString(), Application["DesKey"].ToString())
                            );
                        meta_Image = "{0}News/{1}/{2}".FormatThis(
                            Application["File_WebUrl"].ToString() + Param_FileWebFolder
                            , DT.Rows[0]["Group_ID"].ToString()
                            , DT.Rows[0]["News_Pic"].ToString()
                            );

                    }

                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - News");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetPic = DataBinder.Eval(e.Item.DataItem, "Block_Pic").ToString();
            string GetID = DataBinder.Eval(e.Item.DataItem, "News_ID").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();

            if (!string.IsNullOrEmpty(GetPic))
            {

                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                //顯示Html
                lt_Pic.Text = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"img-responsive lazy\" alt=\"News\" />".FormatThis(
                        fn_stringFormat.ashx_Pic("{0}News/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
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


    public string meta_Title
    {
        get;
        set;
    }

    public string meta_Desc
    {
        get;
        set;
    }

    public string meta_Url
    {
        get;
        set;
    }

    public string meta_Image
    {
        get;
        set;
    }
    #endregion
}