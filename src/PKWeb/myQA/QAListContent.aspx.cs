using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

public partial class QAListContent : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_常見問題;


                //取得資料
                LookupDataList();

                //** 設定程式編號(目前頁面所在功能位置) **
                if (false == setProgIDs.setID(this.Master, "200"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 副程式 - 取得資料列表
    /// </summary>
    private void LookupDataList()
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
                SBSql.AppendLine(" SELECT myData.FAQ_Title, myData.FAQ_ID");
                SBSql.AppendLine("    , RTRIM(ProdData.Model_No) AS ModelNo, RTRIM(ProdData.Model_Name_{0}) AS ModelName".FormatThis(fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM FAQ_Group GP WITH(NOLOCK)");
                //SBSql.AppendLine("     INNER JOIN FAQ_Area Area WITH(NOLOCK) ON GP.Group_ID = Area.Group_ID");
                SBSql.AppendLine("     INNER JOIN FAQ myData WITH(NOLOCK) ON GP.Group_ID = myData.Group_ID");
                SBSql.AppendLine("     INNER JOIN FAQ_Rel_ModelNo Rel WITH(NOLOCK) ON GP.Group_ID = Rel.Group_ID");
                SBSql.AppendLine("     INNER JOIN [ProductCenter].dbo.Prod_Item ProdData WITH (NOLOCK) ON Rel.Model_No = ProdData.Model_No");
                SBSql.AppendLine(" WHERE (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("     AND (UPPER(Rel.Model_No) = UPPER(@ModelNo))");
                //(Area.AreaCode = @AreaCode) AND 

                //判斷會員身份
                if (fn_Param.MemberType.Equals("1"))
                {
                    //經銷商身份
                    SBSql.Append(" AND (GP.Target IN (0,1))");
                }
                else
                {
                    //其他
                    SBSql.Append(" AND (GP.Target = 0)");
                }

                SBSql.AppendLine(" ORDER BY GP.Sort ASC, GP.Group_ID DESC");
                cmd.CommandText = SBSql.ToString();
                //cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("ModelNo", Req_ModelNo);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();


                    if (DT.Rows.Count > 0)
                    {
                        //填入產品資訊Html
                        string Model_No = DT.Rows[0]["ModelNo"].ToString();
                        string ModelName = DT.Rows[0]["ModelName"].ToString();
                        this.lt_ModelNo.Text = Model_No;
                        this.lt_ModelName.Text = ModelName;
                        this.lt_ModelPic.Text = "<img src=\"{0}\" alt=\"{1}\" />".FormatThis(
                             "{0}myProd/{1}/".FormatThis(Application["API_WebUrl"], Server.UrlEncode(Model_No))
                            , ModelName
                            );
                    }
                }
            }

        }
        catch (Exception)
        {
            throw;
            throw new Exception("系統發生錯誤 - 讀取資料");
        }
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 上一層Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String myData = Convert.ToString(Page.RouteData.Values["myData"]);
            return (myData.ToUpper().Equals("ALL")) ? "" : myData;
        }
        set
        {
            this._Req_Keyword = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 品號
    /// </summary>
    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String myData = Convert.ToString(Page.RouteData.Values["myModel"]);
            return (fn_Extensions.String_資料長度Byte(myData, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(myData).Trim() : "";
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
    {
        get
        {
            return Application["DesKey"].ToString();
        }
        set
        {
            this._DesKey = value;
        }
    }

    #endregion
}