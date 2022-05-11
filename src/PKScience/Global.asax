<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>


<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // 在應用程式啟動時執行的程式碼
        // 載入Routing設定
        RegisterRoutes(RouteTable.Routes);


    }

    void Application_End(object sender, EventArgs e)
    {
        //  在應用程式關閉時執行的程式碼

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 在發生未處理的錯誤時執行的程式碼

    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        #region -- 語系判斷 --

        System.Globalization.CultureInfo currentInfo;

        //[判斷參數] - 判斷Cookie是否存在
        HttpCookie cLang = Request.Cookies["PKScience_Lang"];
        if ((cLang != null))
        {
            //依Cookie選擇，變換語言別
            switch (cLang.Value.ToString().ToUpper())
            {
                case "EN-US":
                    currentInfo = new System.Globalization.CultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;

                case "ZH-CN":
                    currentInfo = new System.Globalization.CultureInfo("zh-CN");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;

                default:
                    currentInfo = new System.Globalization.CultureInfo("zh-TW");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;
            }

        }
        else
        {
            //Cookie不存在, 新增預設語系(依瀏覽器預設)
            string defCName = System.Globalization.CultureInfo.CurrentCulture.Name;

            //判斷瀏覽器預設的語系
            switch (defCName.ToUpper())
            {
                case "EN-US":
                    defCName = "en-US";
                    break;

                case "ZH-CN":
                    defCName = "zh-CN";
                    break;

                default:
                    defCName = "zh-TW";
                    break;
            }


            Response.Cookies.Add(new HttpCookie("PKScience_Lang", defCName));
            Response.Cookies["PKScience_Lang"].Expires = DateTime.Now.AddYears(1);
            currentInfo = new System.Globalization.CultureInfo(defCName);
            System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;

        }

        #endregion

    }

    /// <summary>
    /// Routing設定
    /// </summary>
    /// <param name="routes">URL路徑</param>
    public static void RegisterRoutes(RouteCollection routes)
    {
        #region -- 定義不處理UrlRouting的規則 --
        routes.Ignore("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
        routes.Ignore("{*allcss}", new { allcss = @".*\.css(/.*)?" });
        routes.Ignore("{*alljpg}", new { alljpg = @".*\.jpg(/.*)?" });
        routes.Ignore("{*alljs}", new { alljs = @".*\.js(/.*)?" });
        routes.Add(new Route("{resource}.css/{*pathInfo}", new StopRoutingHandler()));
        routes.Add(new Route("{resource}.js/{*pathInfo}", new StopRoutingHandler()));
        #endregion

        //[首頁]
        routes.MapPageRoute("HomeRoute", "{lang}", "~/default.aspx", false,
             new RouteValueDictionary {
                    { "lang", "auto" }});

        //關於我們
        routes.MapPageRoute("Story", "{lang}/Story", "~/myInfo/Story.aspx", false,
             new RouteValueDictionary {
                    { "lang", "auto" }});
        routes.MapPageRoute("Patent", "{lang}/Patent", "~/myInfo/Patent.aspx", false,
            new RouteValueDictionary {
                    { "lang", "auto" }});
        routes.MapPageRoute("PrivacyPolicy", "{lang}/PrivacyPolicy", "~/myInfo/PrivacyPolicy.aspx", false,
            new RouteValueDictionary {
                    { "lang", "auto" }});
        routes.MapPageRoute("PrivacyCookie", "{lang}/PrivacyCookies", "~/myInfo/Cookie.aspx", false,
            new RouteValueDictionary {
                    { "lang", "auto" }});
        routes.MapPageRoute("UserAgreement", "{lang}/UserAgreement", "~/myInfo/UserAgreement.aspx", false,
            new RouteValueDictionary {
                    { "lang", "auto" }});
        routes.MapPageRoute("ContactUs", "{lang}/ContactUs", "~/myInfo/Inquiry.aspx", false,
            new RouteValueDictionary {
                    { "lang", "auto" }});

        //Where to buy
        routes.MapPageRoute("OnlineShop", "{lang}/OnlineShop", "~/myShop/OnlineShop.aspx", false,
             new RouteValueDictionary {
                    { "lang", "auto" }});
        routes.MapPageRoute("PhysicalShop", "{lang}/Shop", "~/myShop/PhysicalShop.aspx", false,
            new RouteValueDictionary {
                    { "lang", "auto" }});


        //產品
        routes.MapPageRoute("ProdList", "{lang}/Products", "~/myProd/ProdList.aspx", false,
            new RouteValueDictionary {
                { "lang", "auto" }});

        routes.MapPageRoute("ProdView", "{lang}/Product/{id}", "~/myProd/ProdView.aspx", false);

        routes.MapPageRoute("ProdSearch", "{lang}/Search/{page}", "~/myProd/ProdSearch.aspx", false,
          new RouteValueDictionary {
                { "lang", "auto" }
                , { "page", "1" }});
                

        //活動
        routes.MapPageRoute("ExpoList", "{lang}/Activities/{page}", "~/myExpo/ExpoList.aspx", false,
         new RouteValueDictionary {
                { "lang", "auto" }
                , { "page", "1" }});
        routes.MapPageRoute("ExpoView", "{lang}/Activity/{id}", "~/myExpo/ExpoView.aspx", false);


        //News
        routes.MapPageRoute("NewsList", "{lang}/News/{page}", "~/myNews/NewsList.aspx", false,
         new RouteValueDictionary {
                { "lang", "auto" }
                , { "page", "1" }});
        routes.MapPageRoute("NewsView", "{lang}/NewsPage/{id}", "~/myNews/NewsView.aspx", false);


        //影片
        routes.MapPageRoute("VideoList", "{lang}/Videos", "~/myVideo/VideoList.aspx", false,
            new RouteValueDictionary {
                { "lang", "auto" }});

        
        //FAQ
        routes.MapPageRoute("FAQHome", "{lang}/FAQ-Home", "~/myQA/Index.aspx", false,
           new RouteValueDictionary {
                { "lang", "auto" }});
        routes.MapPageRoute("FAQList", "{lang}/FAQ-List/{page}", "~/myQA/List.aspx", false,
           new RouteValueDictionary {
                { "lang", "auto" }
                , { "page", "1" }});

        routes.MapPageRoute("FAQView", "{lang}/FAQ/{id}", "~/myQA/View.aspx", false);

        routes.MapPageRoute("FAQSearch", "{lang}/FAQ-Search/{page}", "~/myQA/Search.aspx", false,
          new RouteValueDictionary {
                { "lang", "auto" }
                , { "page", "1" }});

        /*
         * [Route設定] -- 範例
         * Route名稱, Url顯示模式, 實體Url, 使用者是否可讀取實體Url, 預設值, 條件約束
         */
        //routes.MapPageRoute("DemoList", "DemoList", "~/Demo/DemoList.aspx", false);
        //routes.MapPageRoute("DemoDetail", "DemoDetail/{ID}/{DemoTitle}", "~/Demo/DemoDetail.aspx", false,
        //     new RouteValueDictionary {
        //            { "ID", "0" },
        //            { "DemoTitle", "Data-not-found" }},
        //     new RouteValueDictionary {
        //            { "ID", "[0-9]{1,8}" }
        //        });

    }


    public class HttpHandlerRouteHandler<THandler>
    : IRouteHandler where THandler : IHttpHandler, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new THandler();
        }
    }
</script>
