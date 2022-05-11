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

public partial class Ajax_Data_Frame_GoBuy : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //取得資料
                Lookup_Data();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }


    private void Lookup_Data()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();
        string ErrMsg;

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT Country_Code, Url, ImgUrl");
            sql.AppendLine(" FROM Shop_Redirect WITH(NOLOCK)");
            sql.AppendLine(" WHERE (UPPER(Country_Code) = UPPER(@Country_Code))");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Country_Code", Req_Area);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                this.lvData.DataSource = DT.DefaultView;
                this.lvData.DataBind();
            }
        }

    }


    private string _Req_Area;
    public string Req_Area
    {
        get
        {
            String data = Request["area"];

            return string.IsNullOrEmpty(data) ? "" : fn_stringFormat.Set_FilterHtml(data.Left(2));
        }
        set
        {
            this._Req_Area = value;
        }
    }

    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String data = Request["id"];

            return string.IsNullOrEmpty(data) ? "" : fn_stringFormat.Set_FilterHtml(data.Left(50));
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }


    private string _Req_ModelName;
    public string Req_ModelName
    {
        get
        {
            String data = Request["name"];

            return string.IsNullOrEmpty(data) ? "" : fn_stringFormat.Set_FilterHtml(data.Left(100));
        }
        set
        {
            this._Req_ModelName = value;
        }
    }

    private string _CDNUrl;
    public string CDNUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"];
        }
        set
        {
            this._CDNUrl = value;
        }
    }
}