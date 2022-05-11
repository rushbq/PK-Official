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

public partial class html_PackFiles : SecurityCheckDealer
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Param_thisID))
                {
                    this.pl_warning.Visible = true;
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
    /// 顯示資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT Pic.Pic_OrgFile, Pic.Pic_File");
                SBSql.Append("  , RTRIM(Prod.Model_No) AS ID, (Prod.Model_Name_{0}) AS Label".FormatThis(fn_Language.Param_Lang));
                SBSql.Append(" FROM Prod_Item Prod WITH (NOLOCK)");
                SBSql.Append("  INNER JOIN ProdPic_Group Pic WITH (NOLOCK) ON Prod.Model_No = Pic.Model_No");
                SBSql.Append(" WHERE (Prod.Model_No = @DataID) AND (Pic.Pic_Class = @PicClass)");
                SBSql.Append(" ORDER BY Pic.Sort");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("PicClass", Param_PicClass);

                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        this.lt_ID.Text = DT.Rows[0]["ID"].ToString();
                        this.lt_Label.Text = DT.Rows[0]["Label"].ToString();
                    }

                    //DataBind
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
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
                string ModelNo = DataBinder.Eval(dataItem.DataItem, "ID").ToString();
                string myFile = DataBinder.Eval(dataItem.DataItem, "Pic_File").ToString();
                string myDispName = DataBinder.Eval(dataItem.DataItem, "Pic_OrgFile").ToString();

                //取得控制項
                Literal lt_Files = (Literal)e.Item.FindControl("lt_Files");

                //下載資料夾
                string folder = "ProductPic/{0}/{1}/".FormatThis(ModelNo, Param_PicClass);
           
                //下載路徑
                lt_Files.Text = GetUrl(folder, myFile, myDispName);

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 取得下載路徑
    /// </summary>
    /// <param name="fileFolder"></param>
    /// <param name="fileName"></param>
    /// <param name="downloadName"></param>
    /// <returns></returns>
    private string GetUrl(string fileFolder, string fileName, string downloadName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }
        else
        {
            string url =
                "{0}myHandler/Ashx_DealerDownload.ashx?f={1}&r={2}&d={3}&t={4}".FormatThis(
                          Application["WebUrl"]
                          , HttpUtility.UrlEncode(fileFolder)
                          , HttpUtility.UrlEncode(fileName)
                          , HttpUtility.UrlEncode(downloadName)
                          , Token
                      );

            return "<a href=\"{0}\" class=\"btn btn-default\"><i class=\"fa fa-cloud-download\"></i></a>".FormatThis(url);
        }
    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Request["DataID"];

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Pic Class
    /// </summary>
    private string _Param_PicClass;
    public string Param_PicClass
    {
        get
        {
            String DataID = Request["c"];

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Param_PicClass = value;
        }
    }

    /// <summary>
    /// 依不同身份產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get
        {
            return fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);
        }
        set
        {
            this._Token = value;
        }
    }
    #endregion
}