using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Newtonsoft.Json;

/// <summary>
/// 產品中心品號
/// </summary>
public partial class AC_ModelNo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[檢查參數] - 查詢關鍵字
                string keywordString = "";
                if (null != Request["q"])
                {
                    keywordString = fn_stringFormat.Set_FilterHtml(Request["q"].Trim());
                }

                string ErrMsg;

                using (SqlCommand cmd = new SqlCommand())
                {
                    //[SQL] - 資料查詢
                    StringBuilder SBSql = new StringBuilder();

                    SBSql.AppendLine(" SELECT TOP 100 RTRIM(PItem.Model_No) AS id, RTRIM(PItem.Model_No) AS label ");
                    SBSql.AppendLine("     , Cls.Class_ID AS categoryID, Cls.Class_Name_zh_TW AS category  ");
                    SBSql.AppendLine(" FROM Prod_Item PItem WITH (NOLOCK) ");
                    SBSql.AppendLine("     INNER JOIN Prod_Class Cls WITH (NOLOCK) ON PItem.Class_ID = Cls.Class_ID ");
                    SBSql.AppendLine(" WHERE (PItem.Model_No <> '') ");
                    SBSql.AppendLine("   AND ( ");
                    SBSql.AppendLine("       (UPPER(PItem.Model_No) LIKE '%' + UPPER(@Keyword) + '%') ");
                    SBSql.AppendLine("   ) ");
                    SBSql.AppendLine(" ORDER BY categoryID, label ");

                    //[SQL] - Command
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));

                    //[SQL] - 取得資料
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            Response.Write("");
                        }
                        else
                        {
                            Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}