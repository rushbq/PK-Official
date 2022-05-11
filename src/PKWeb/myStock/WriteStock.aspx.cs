using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class myStock_WriteStock : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //Check Token is null
                if (string.IsNullOrWhiteSpace(Req_Token))
                {
                    fn_Extensions.JsAlert("Bad Request.", Application["WebUrl"].ToString());
                    return;
                }

                //載入基本資料
                LookupBase();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --

    private void LookupBase()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Parent_ID, SupID, SupName, IsWrite, StockShow, Token");
                SBSql.AppendLine(" FROM SupInvCheck_Supplier");
                SBSql.AppendLine(" WHERE (UPPER(Token) = UPPER(@token))");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("token", Req_Token);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKEF, out ErrMsg))
                {
                    //check null
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料.", Application["WebUrl"].ToString());
                        return;
                    }

                    //取得參數值
                    string _parentID = DT.Rows[0]["Parent_ID"].ToString();
                    string _supID = DT.Rows[0]["SupID"].ToString();
                    string _supName = DT.Rows[0]["SupName"].ToString();
                    string _isWrite = DT.Rows[0]["IsWrite"].ToString();
                    string _stockShow = DT.Rows[0]["StockShow"].ToString();
                    string _token = DT.Rows[0]["Token"].ToString();

                    //判斷是否已填寫
                    if (_isWrite.Equals("Y"))
                    {
                        Response.Redirect("{0}StockOK?msg={1}".FormatThis(
                            Application["WebUrl"].ToString()
                            , Server.UrlEncode("庫存盤點已填寫完畢,感謝您的配合")));
                    }

                    //填寫基本資料
                    hf_Parent_ID.Value = _parentID;
                    lt_SupName.Text = _supName;
                    hf_Token.Value = _token;

                    //載入單身資料
                    LookupDataList(_parentID, _supID, _stockShow);
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("Oops..");
        }
    }


    /// <summary>
    /// 取得單身資料-品號
    /// </summary>
    /// <param name="_parentID"></param>
    /// <param name="_supID"></param>
    /// <param name="_stockShow"></param>
    private void LookupDataList(string _parentID, string _supID, string _stockShow)
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Base.Data_ID, Base.ModelNo, Base.InputQty1, Base.InputQty2");
                SBSql.AppendLine(" , CAST(Stock.MC007 AS INT) AS StockNum");
                SBSql.AppendLine(" , Prod.MB002 AS ModelName");
                SBSql.AppendLine(" FROM SupInvCheck_Model Base");
                SBSql.AppendLine("  INNER JOIN [prokit2].dbo.INVMB Prod ON Base.ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = Prod.MB001");
                SBSql.AppendLine("  LEFT JOIN [prokit2].dbo.INVMC Stock ON Base.SupID COLLATE Chinese_Taiwan_Stroke_BIN = LEFT(Stock.MC003, 6)");
                SBSql.AppendLine("   AND Base.ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = Stock.MC001 AND Stock.MC002 = '04'");
                SBSql.AppendLine(" WHERE (Parent_ID = @Parent_ID) AND (SupID = @SupID)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Parent_ID", _parentID);
                cmd.Parameters.AddWithValue("SupID", _supID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKEF, out ErrMsg))
                {
                    //Bind Data
                    lvDataList.DataSource = DT.DefaultView;
                    lvDataList.DataBind();

                    //寶工庫存顯示否
                    if (_stockShow.Equals("N"))
                    {
                        //hide header
                        ph_stockHeader.Visible = false;

                        if (DT.Rows.Count > 0)
                        {
                            //Find controller
                            ListView lst = lvDataList;
                            for (int row = 0; row < lst.Items.Count; row++)
                            {
                                ((PlaceHolder)lst.Items[row].FindControl("ph_stockBody")).Visible = false;
                            }
                        }
                    }
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("Oops..");
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ////取得資料
            //string Get_IsNewItem = DataBinder.Eval(e.Item.DataItem, "IsNewItem").ToString();


        }
    }


    #endregion


    #region -- 按鈕事件 --

    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            //Declare
            ListView _data = lvDataList;
            List<SupDataItem> dataList = new List<SupDataItem>(); //資料容器

            for (int row = 0; row < _data.Items.Count; row++)
            {
                string id = ((HiddenField)_data.Items[row].FindControl("hf_DataID")).Value;
                string qty1 = ((TextBox)_data.Items[row].FindControl("tb_InputQty1")).Text;
                string qty2 = ((TextBox)_data.Items[row].FindControl("tb_InputQty2")).Text;

                var dataItem = new SupDataItem
                {
                    DataID = Convert.ToInt32(id),
                    Qty1 = string.IsNullOrWhiteSpace(qty1) ? 0 : Convert.ToInt32(qty1),
                    Qty2 = string.IsNullOrWhiteSpace(qty2) ? 0 : Convert.ToInt32(qty2)
                };

                //將項目加入至集合
                dataList.Add(dataItem);
            }

            //Check input
            int dataCnt = dataList.Count;
            if (dataCnt == 0)
            {
                fn_Extensions.JsAlert("未填寫資料", thisPage);
                return;
            }

            //Update
            if (!doUpdate(dataList, out ErrMsg))
            {
                fn_Extensions.JsAlert("存檔失敗", "");
                return;
            }

            //Redirect OK page
            Response.Redirect(Application["WebUrl"].ToString() + "StockOK");

        }
        catch (Exception)
        {

            throw;
        }

    }

    /// <summary>
    /// 更新填寫數量
    /// </summary>
    /// <param name="inst"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    public bool doUpdate(List<SupDataItem> inst, out string ErrMsg)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            for (int row = 0; row < inst.Count; row++)
            {
                sql.AppendLine(" UPDATE SupInvCheck_Model");
                sql.AppendLine(" SET InputQty1 = {0}, InputQty2 = {1}".FormatThis(inst[row].Qty1, inst[row].Qty2));
                sql.AppendLine(" WHERE (Parent_ID = @Parent_ID) AND (Data_ID = {0});".FormatThis(inst[row].DataID));
            }

            //Update Status
            sql.AppendLine(" UPDATE SupInvCheck_Supplier");
            sql.AppendLine(" SET IsWrite = 'Y', WriteTime = GETDATE()");
            sql.AppendLine(" WHERE (Token = @Token)");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.CommandTimeout = 180;  //單位:秒
            cmd.Parameters.AddWithValue("Parent_ID", hf_Parent_ID.Value);
            cmd.Parameters.AddWithValue("Token", hf_Token.Value);

            //Execute
            return dbConn.ExecuteSql(cmd, dbConn.DBS.PKEF, out ErrMsg);
        }

    }

    #endregion


    #region -- 參數設定 --


    /// <summary>
    /// 取得傳遞參數 - Token
    /// </summary>
    private string _Req_Token;
    public string Req_Token
    {
        get
        {
            String DataID = Page.RouteData.Values["token"].ToString();

            return DataID;
        }
        set
        {
            _Req_Token = value;
        }
    }


    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}/Stock/{1}".FormatThis(Application["WebUrl"].ToString(), Req_Token);
        }
        set
        {
            _thisPage = value;
        }
    }


    public class SupDataItem
    {
        public int DataID { get; set; }
        public int Qty1 { get; set; }
        public int Qty2 { get; set; }
    }
    #endregion

}