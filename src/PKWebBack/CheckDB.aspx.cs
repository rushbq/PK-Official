using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class CheckDB : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string ErrMsg = "";
        //using (SqlCommand cmd = new SqlCommand())
        //{
        //    //宣告
        //    StringBuilder SBSql = new StringBuilder();

        //    //[SQL] - 清除cmd參數
        //    cmd.Parameters.Clear();

        //    SBSql.AppendLine(" SELECT COUNT(*) AS Cnt FROM Program ");

        //    cmd.CommandText = SBSql.ToString();
        //    //cmd.Parameters.AddWithValue("UserGUID", Session["Login_GUID"].ToString());
        //    using (DataTable DT = LookupDT(cmd, DBS.EFLocal, out ErrMsg))
        //    {
        //        Response.Write(ErrMsg);
        //        //Response.Write(DT.Rows.Count);
        //    }
        //}

        string path = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"Data_File\Authorization\User_Group.xml";


        //if (false == System.IO.File.Exists(path)) {
        //    Response.Write("不存在");
        //    return;
        //};
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            using (StreamReader sw = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                Response.Write(sw.ReadToEnd());
            }
        }
     
    }

    public enum DBS
    {
        EFLocal = 1,
        PKSYS = 2,
        Product = 3
    }

    /// <summary>
    /// 連線字串
    /// </summary>
    /// <param name="dbs">資料庫別</param>
    /// <returns></returns>
    private static string ConnString(DBS dbs)
    {
        switch ((int)dbs)
        {
            case 2:
                return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon_PKSYS"];

            case 3:
                return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon_Product"];

            default:
                return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon"];
        }
    }

    public static DataTable LookupDT(SqlCommand cmd, DBS dbs, out string errMsg)
    {
        SqlConnection connSql = new SqlConnection(ConnString(dbs));
        try
        {
            connSql.Open();
            cmd.Connection = connSql;

            //建立DataAdapter
            SqlDataAdapter dataAdapterSql = new SqlDataAdapter();
            dataAdapterSql.SelectCommand = cmd;

            //取得DataTable
            DataTable DTSql = new DataTable();
            dataAdapterSql.Fill(DTSql);
            connSql.Close();
            errMsg = "";

            return DTSql;

        }
        catch (Exception ex)
        {
            errMsg = ex.Message.ToString();
            return null;

        }
        finally
        {
            cmd.Dispose();
            connSql.Close();
            connSql.Dispose();
        }
    }
}