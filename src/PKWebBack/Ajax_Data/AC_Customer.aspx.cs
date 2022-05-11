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
/// Tags
/// </summary>
public partial class AC_Customer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
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

                SBSql.AppendLine("SELECT TOP 100 RTRIM(MA001) AS id, RTRIM(MA002) AS label ");
                SBSql.AppendLine(" FROM Customer WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (DBS = DBC) AND ( ");
                SBSql.AppendLine("      (MA001 LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("   OR (MA002 LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("   OR (MA003 LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("   OR (MA009 LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine(" ) ");
                SBSql.AppendLine(" ORDER BY MA001 ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));

                //[SQL] - 取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
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

    }
}