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

public partial class _Default : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
               


            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料取得 --
    /// <summary>
    /// 資料顯示, 自訂報表查詢
    /// </summary>
    private void LookupData_CU()
    {
        try
        {
            //[取得資料] - 取得資料
            //using (SqlCommand cmd = new SqlCommand())
            //{
            //    //宣告
            //    StringBuilder SBSql = new StringBuilder();
            //    StringBuilder html = new StringBuilder();

            //    //清除參數
            //    cmd.Parameters.Clear();

            //    //[SQL] - 資料查詢
            //    SBSql.AppendLine(" SELECT TOP 5 CUID, CU_Name, CU_Desc");
            //    SBSql.AppendLine(" FROM CU_Base WITH (NOLOCK) ");
            //    SBSql.AppendLine(" WHERE (Create_Who = @Create_Who) AND (onHome = 'Y')");
            //    SBSql.AppendLine(" ORDER BY Sort ASC, Create_Time DESC");
            //    cmd.CommandText = SBSql.ToString();
            //    cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            //    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            //    {
            //        if (DT.Rows.Count == 0)
            //        {
            //            html.Append("<a class=\"list-group-item\"><p class=\"list-group-item-text\">尚未設定</p></a>");
            //        }
            //        else
            //        {
            //            //組合Html
            //            for (int row = 0; row < DT.Rows.Count; row++)
            //            {
            //                html.Append("<a href=\"{0}\" class=\"list-group-item\" target=\"_blank\">".FormatThis(
            //                        "{0}myReport/Rpt_CU_Process.aspx?CUID={1}".FormatThis(
            //                            Application["WebUrl"], Cryptograph.MD5Encrypt(DT.Rows[row]["CUID"].ToString(), Application["DesKey"].ToString())
            //                        )
            //                    ));
            //                html.Append("<h4 class=\"list-group-item-heading\"><strong>{0}</strong></h4><p class=\"list-group-item-text\">{1}</p>".FormatThis(
            //                    DT.Rows[row]["CU_Name"].ToString()
            //                    , DT.Rows[row]["CU_Desc"].ToString()
            //                    ));
            //                html.Append("</a>");
            //            }
            //        }
            //    }

            //    //輸出Html
            //    this.lt_CU_List.Text = html.ToString();
            //}
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 自訂報表查詢");
        }
    }
    #endregion
}