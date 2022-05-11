using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using FAQData.Controllers;
using FAQData.Models;
using PKLib_Method.Methods;

public partial class myQA_Index : System.Web.UI.Page
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //Get Data
                LookupDataList();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupDataList()
    {
        //----- 宣告:資料參數 -----
        FAQRepository _data = new FAQRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();
        StringBuilder html = new StringBuilder();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetFAQ(search, fn_Language.Web_Lang);


        //----- 整理資料:類別 -----
        var group = query
            .OrderBy(o => o.ClassSort)
            .Select(i => new
            {
                id = i.ClassID,
                name = i.ClassName
            })
            .Distinct();


        int row = 0, idx = 0;
        foreach (var item in group)
        {
            //計數:判斷列數
            row++;
            //計數:判斷總筆數
            idx++;


            //第一筆時(加入表頭)
            if (row == 1)
            {
                html.AppendLine("<div class=\"row\">");
            }


            //加入此類別的內容
            html.AppendLine(Get_Detail(item.id, item.name, query));


            //第三筆時(加入表尾)
            if (row == 3)
            {
                html.AppendLine("</div>");

                row = 0;
            }
            //非第三筆,且為總筆數最後一筆時(加入表尾)
            else if (idx == group.Count())
            {
                html.AppendLine("</div>");
            }

        }

        //Output Html
        this.lt_Content.Text = html.ToString();
    }


    /// <summary>
    /// 取得各類別的內容
    /// </summary>
    /// <param name="classID">類別id</param>
    /// <param name="className">類別name</param>
    /// <param name="fulldata">原始資料</param>
    /// <returns></returns>
    private string Get_Detail(int classID, string className, IQueryable<FAQ> fulldata)
    {
        if (fulldata == null)
        {
            return "";
        }

        StringBuilder html = new StringBuilder();

        //解析資料
        var data = fulldata
            .Where(f => f.ClassID.Equals(classID))
            .Select(i => new
            {
                ID = i.ID,
                Title = i.Title
            });


        html.Append("<div class=\"col s12 m12 l4\">");
        html.Append("<div class=\"section\">");
        html.Append(" <h2>{0}</h2>".FormatThis(className));
        html.Append(" <ul class=\"question\">");

        //Item List
        foreach (var item in data)
        {
            html.Append(" <li><a href=\"{0}{1}/FAQ/{2}\">{3}</a></li>".FormatThis(
                webUrl
                , Req_Lang
                , item.ID
                , item.Title));
        }

        html.Append(" </ul>");
        html.Append("</div>");
        html.Append("</div>");


        //return html
        return html.ToString();
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 語系
    /// </summary>
    public string Req_Lang
    {
        get
        {
            string myData = Page.RouteData.Values["lang"].ToString();

            //若為auto, 就去抓cookie
            return myData.Equals("auto") ? fn_Language.Get_Lang(Request.Cookies["PKScience_Lang"].Value) : myData;
        }
        set
        {
            this._Req_Lang = value;
        }
    }
    private string _Req_Lang;


    #endregion
}