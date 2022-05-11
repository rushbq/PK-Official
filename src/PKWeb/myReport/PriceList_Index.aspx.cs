using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

public partial class PriceList : SecurityCheckDealer
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.home_經銷商專區;

                //取得分類
                LookupData_Cate();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    private void LookupData_Cate()
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
                SBSql.AppendLine(" SELECT RTRIM(Class_ID) AS Class_ID, Class_Name_{0} AS Class_Name ".FormatThis(fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM Prod_Class WITH(NOLOCK) ");
                SBSql.AppendLine(" WHERE (LEFT(RTRIM(Class_ID),1) = '2') AND (Display = 'Y') AND (Display_PKWeb = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort, Class_ID ");
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                {
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Categories");
        }

    }


}