using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using VideoData.Controllers;

public partial class myVideo_VideoList : System.Web.UI.Page
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
                LookupDataList(this.lvDataList_Cls1, 1);
                LookupDataList(this.lvDataList_Cls2, 2);

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
    /// <param name="ClassID"></param>
    private void LookupDataList(ListView lv, int clsID)
    {
        //----- 宣告:資料參數 -----
        VideoRepository _data = new VideoRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        //[取得/檢查參數] - ClassID
        search.Add((int)mySearch.ClassID, clsID.ToString());


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetVideos(search, fn_Language.Web_Lang, 0);


        //----- 資料整理:繫結 ----- 
        lv.DataSource = query;
        lv.DataBind();


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