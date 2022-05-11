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

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 匯出PriceList Excel
    /// 為顯示圖片, 使用html格式, 匯出格式僅限xls.
    /// </summary>
    protected void btn_PriceList_Click(object sender, EventArgs e)
    {
        StringBuilder html = new StringBuilder();

        //查詢StoreProcedure (myPrc_GetCustFullPrice_OverSales)
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "myPrc_GetCustFullPrice_OverSales";
            cmd.Parameters.AddWithValue("CustID", Get_CustID);
            cmd.Parameters.AddWithValue("ProdClass", string.IsNullOrEmpty(Get_ProdCls) ? DBNull.Value : (Object)Get_ProdCls);
            cmd.CommandTimeout = 90;

            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
            {
                if (DT == null)
                {
                    fn_Extensions.JsAlert("No Data", "");
                    return;
                }

                if (DT.Rows.Count == 0)
                {
                    fn_Extensions.JsAlert("Fail", "");
                    return;
                }

                //取得Datatable, 篩選欄位
                var query =
                from el in DT.AsEnumerable()
                orderby el.Field<string>("Model_No")
                select new
                {
                    Stop_Offer = el.Field<string>("Stop_Offer"),
                    Item_NO = el.Field<string>("Model_No"),
                    Class = el.Field<string>("ClassName_{0}".FormatThis(fn_Language.Param_Lang)),
                    Description = el.Field<string>("Model_Name_{0}".FormatThis(fn_Language.Param_Lang)),
                    Currency = el.Field<string>("Currency"),
                    Unit_Price = el.Field<double?>("myPrice"),
                    Unit = el.Field<string>("Unit"),
                    Quote_Date = el.Field<string>("QuoteDate"),
                    MOQ = el.Field<int?>("MOQ"),
                    VOL = el.Field<string>("Vol"),
                    Page = el.Field<string>("Page"),
                    Qty_Inner = el.Field<int?>("InnerBox_Qty"),
                    NW = el.Field<double?>("InnerBox_NW"),
                    GW = el.Field<double?>("InnerBox_GW"),
                    CUFT = el.Field<double?>("InnerBox_Cuft"),
                    BarCode = el.Field<string>("BarCode"),
                    Packing = el.Field<string>("Packing_{0}".FormatThis(fn_Language.Param_Lang)),
                    Ship_From = el.Field<string>("Ship_From"),
                    Term = el.Field<string>("TransTermValue"),
                    ListPic = el.Field<string>("ListPic")
                };


                /*
                 Html Table
                */
                //header
                html.Append("<table border=\"1\" class=\"grid\">");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append(" <th>Image</th>");
                html.Append(" <th>Stop Offer</th>");
                html.Append(" <th>Item NO</th>");
                html.Append(" <th>Class</th>");
                html.Append(" <th>Description</th>");
                html.Append(" <th>Currency</th>");
                html.Append(" <th>Unit Price</th>");
                html.Append(" <th>Unit</th>");
                html.Append(" <th>Quote Date</th>");
                html.Append(" <th>MOQ</th>");
                html.Append(" <th>VOL</th>");
                html.Append(" <th>Page</th>");
                html.Append(" <th>Qty Inner</th>");
                html.Append(" <th>NW</th>");
                html.Append(" <th>GW</th>");
                html.Append(" <th>CUFT</th>");
                html.Append(" <th>BarCode</th>");
                html.Append(" <th>Packing</th>");
                html.Append(" <th>Ship From</th>");
                html.Append(" <th>Term</th>");
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");

                foreach (var item in query)
                {
                    //*** 填入Html ***
                    html.Append("<tr>");
                    //產品圖
                    html.Append("<td width=\"110\" {1}>{0}</td>".FormatThis(
                        GetImgUrl(item.Item_NO, item.ListPic)
                        , !string.IsNullOrWhiteSpace(item.ListPic) ? "style=\"height:115px !important;\"" : ""));

                    html.Append("<td>{0}</td>".FormatThis(item.Stop_Offer));
                    html.Append("<td>{0}</td>".FormatThis(item.Item_NO));
                    html.Append("<td>{0}</td>".FormatThis(item.Class));
                    html.Append("<td>{0}</td>".FormatThis(item.Description));
                    html.Append("<td>{0}</td>".FormatThis(item.Currency));
                    html.Append("<td>{0}</td>".FormatThis(item.Unit_Price));
                    html.Append("<td>{0}</td>".FormatThis(item.Unit));
                    html.Append("<td>{0}</td>".FormatThis(item.Quote_Date));
                    html.Append("<td>{0}</td>".FormatThis(item.MOQ));
                    html.Append("<td>{0}</td>".FormatThis(item.VOL));
                    html.Append("<td>{0}</td>".FormatThis(item.Page));
                    html.Append("<td>{0}</td>".FormatThis(item.Qty_Inner));
                    html.Append("<td>{0}</td>".FormatThis(item.NW));
                    html.Append("<td>{0}</td>".FormatThis(item.GW));
                    html.Append("<td>{0}</td>".FormatThis(item.CUFT));
                    html.Append("<td style=\"mso-number-format:\\@\">{0}</td>".FormatThis(item.BarCode));
                    html.Append("<td>{0}</td>".FormatThis(item.Packing));
                    html.Append("<td>{0}</td>".FormatThis(item.Ship_From));
                    html.Append("<td>{0}</td>".FormatThis(item.Term));

                    html.Append("</tr>");
                }

                html.Append("</tbody>");
                html.Append("</table>");

                ////Linq轉DataTable
                //DataTable myDT = fn_CustomUI.LINQToDataTable(query);

                ////匯出Excel
                //fn_CustomUI.ExportExcel(myDT
                //    , "{0}-PriceList.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
                //    );
            }
        }


        /*
        output Excel
        */
        HttpResponse resp = System.Web.HttpContext.Current.Response;
        resp.Charset = "utf-8";
        resp.Clear();
        string filename = "PriceList_{0}".FormatThis(DateTime.Now.ToString("yyyyMMddHHmm"));
        resp.AppendHeader("Content-Disposition", "attachment;filename=" + filename + ".xls");
        resp.ContentEncoding = System.Text.Encoding.UTF8;
        resp.ContentType = "application/ms-excel";

        resp.Write("<html>");
        string style = "<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=utf-8\"/>" + "<style> .grid{color: #222222; text-align:center; } .grid thead th{font: 10pt \"Microsoft JhengHei\", \"Microsoft YaHei\"; color: #ffffff; font-weight: bold; background-color: #1565c0; height:30px; text-align:center;} .grid td{font: 10pt \"Microsoft JhengHei\", \"Microsoft YaHei\"; height:25px;} .red-text{color:#f44336} </style>";
        resp.Write(style);
        resp.Write("<body>");
        resp.Write(html.ToString());
        resp.Write("</body></html>");

        resp.Flush();
        //resp.End(); //--不可用,會有thread錯誤
        HttpContext.Current.ApplicationInstance.CompleteRequest();

    }


    /// <summary>
    /// Get Image
    /// </summary>
    /// <param name="_modelNo"></param>
    /// <param name="_ID">file name</param>
    /// <returns></returns>
    private string GetImgUrl(string _modelNo, string _ID)
    {
        if (string.IsNullOrWhiteSpace(_ID))
        {
            return "";
        }

        //Img url
        string _url = fn_Param.RefUrl;
        //產品中心圖片
        _url += "ProductPic/" + _modelNo + "/1/" + _ID;

        return "<img src=\"{0}\" width=\"80\" height=\"80\">".FormatThis(_url);
    }

    #endregion

    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 客戶編號
    /// </summary>
    public string Get_CustID
    {
        get
        {
            String DataID = fn_Member.GetDealerID(fn_Param.MemberID);

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Get_CustID = value;
        }
    }
    private string _Get_CustID;


    /// <summary>
    /// 取得參數 - 產品類別
    /// </summary>
    public string Get_ProdCls
    {
        get
        {
            String data = Request.QueryString["Cls"] == "-1" ? "" : Request.QueryString["Cls"].ToString();

            return data;
        }
        set
        {
            this._Get_ProdCls = value;
        }
    }
    private string _Get_ProdCls;
    #endregion
}