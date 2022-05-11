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

public partial class mySupport_DealerList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_銷售據點;

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
                //Check China asshole ip
                string _countryCode = fn_Param.GetCountryCode_fromCookie();

                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder html = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Ext.AreaName, Sub.Country_Code, Sub.Country_Name, Base.Country_Flag");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Base.AreaCode ORDER BY Base.AreaCode, Base.Country_Code ASC) AS 'GP_Rank'");
                SBSql.AppendLine(" FROM Geocode_CountryCode Base WITH (NOLOCK) ");
                SBSql.AppendLine("  INNER JOIN Geocode_AreaName Ext WITH (NOLOCK) ON Base.AreaCode = Ext.AreaCode AND LOWER(Ext.LangCode) = LOWER(@LangCode) ");
                SBSql.AppendLine("  INNER JOIN Geocode_CountryName Sub WITH (NOLOCK) ON Base.Country_Code = Sub.Country_Code ");
                SBSql.AppendLine(" WHERE (LOWER(Sub.LangCode) = LOWER(@LangCode)) AND (Base.Display = 'Y') ");

                //特殊條件:IP = China
                if (_countryCode.Equals("CN"))
                {
                    SBSql.Append(" AND (Base.Country_Code NOT IN ('TW','HK','CN'))");

                    //show html block
                    ph_asshole.Visible = true;
                }

                SBSql.AppendLine(" ORDER BY Base.AreaCode, Base.Country_Code");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //null
                    if (DT.Rows.Count == 0)
                    {
                        Response.Redirect(Application["WebUrl"].ToString());
                        return;
                    }

                    //填入Html
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        int GP_Rank = Convert.ToInt16(DT.Rows[row]["GP_Rank"]);
                        //顯示表頭
                        if (GP_Rank == 1)
                        {
                            if (row > 1) html.Append("<div class=\"row\"></div>");
                            //洲別
                            html.Append("<div class=\"page-header\"><h3>{0}</h3></div>".FormatThis(DT.Rows[row]["AreaName"].ToString()));
                        }

                        //顯示中間內容
                        html.AppendLine("<div class=\"col-md-2 col-sm-3 col-xs-6 country\"><a href=\"{0}\">{1}<span>{2}</span></a></div>".FormatThis(
                                "{0}WhereToBuy/{1}".FormatThis(
                                        Application["WebUrl"]
                                        , DT.Rows[row]["Country_Code"]
                                    )
                                , "<img data-original=\"{1}\" src=\"{0}js/lazyload/grey.gif\" class=\"lazy\" alt=\"Shops\" width=\"40\" />".FormatThis(
                                        Application["WebUrl"]
                                        , fn_stringFormat.ashx_Pic("{0}Support/Flag/{1}".FormatThis(Param_FileWebFolder, DT.Rows[row]["Country_Flag"].ToString()))
                                    )
                                , DT.Rows[row]["Country_Name"].ToString()
                            ));

                    }

                    this.lt_Content.Text = html.ToString();

                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - WhereToBuy");
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