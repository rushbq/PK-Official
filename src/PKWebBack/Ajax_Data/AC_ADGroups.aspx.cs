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
/// 列出AD群組
/// </summary>
public partial class AC_ADGroups : SecurityCheck
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
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

                    SBSql.AppendLine("SELECT TOP 100 Guid AS id, Display_Name + '(' + Account_Name + ')' AS label ");
                    SBSql.AppendLine(" FROM User_Group WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Display = 'Y') ");
                    SBSql.AppendLine(" AND ( ");
                    SBSql.AppendLine("      (Account_Name LIKE '%' + @Keyword + '%') ");
                    SBSql.AppendLine("      OR (Display_Name LIKE '%' + @Keyword + '%') ");
                    SBSql.AppendLine(" ) ");
                    SBSql.AppendLine(" ORDER BY Sort, Display_Name ");

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
        catch (Exception)
        {
            Response.Write("");
        }
    }
}