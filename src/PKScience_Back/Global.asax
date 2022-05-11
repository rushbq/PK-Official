<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script RunAt="server">
    void Application_Start(object sender, EventArgs e)
    {
        // 在應用程式啟動時執行的程式碼
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        // 載入Routing設定
        RegisterRoutes(RouteTable.Routes);

        // 經常使用的參數
        Application["WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
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

        // [首頁預設頁面]
        routes.MapPageRoute("HomeRoute", "", "~/Default.aspx");

        // [權限設定]
        routes.MapPageRoute("AuthSetByUser", "Auth/User/Set/{DataID}", "~/Authorization/SetUser.aspx", false,
           new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("AuthSetByGroup", "Auth/Group/Set/{DataID}", "~/Authorization/SetGroup.aspx", false,
          new RouteValueDictionary {
                    { "DataID", "New" }});

        // [登入]
        routes.MapPageRoute("LoginPage", "ServiceLogin", "~/Login/LoginPage.aspx", false);
        routes.MapPageRoute("LoginFail", "ServiceLoginFail/{ErrCode}", "~/Login/LoginFail.aspx", false,
             new RouteValueDictionary {
                    { "ErrCode", "9999" }},
             new RouteValueDictionary {
                    { "ErrCode", "[0-9]{1,8}" }
                });


        // [News]
        routes.MapPageRoute("News_Search", "News/Search/{PageID}", "~/myNews/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("News_Edit", "News/Edit/{DataID}", "~/myNews/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("News_EditSub", "News/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myNews/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});

        routes.MapPageRoute("News_EditBox", "News/Edit/DetailBox/{GroupID}/{ParentID}/{DataID}", "~/myNews/Edit_DetailBox.aspx", false);


        // [FAQ]
        routes.MapPageRoute("FAQ_Search", "QA/Search/{PageID}", "~/myQA/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("FAQ_Edit", "QA/Edit/{DataID}", "~/myQA/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("FAQ_EditSub", "QA/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myQA/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        routes.MapPageRoute("FAQ_EditBox", "QA/Edit/DetailBox/{ParentID}/{DataID}", "~/myQA/Edit_DetailBox.aspx", false);

        
        routes.MapPageRoute("FAQcls_Search", "QAClass/Search/{PageID}", "~/myQA/ClsSearch.aspx", false,
           new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("FAQcls_Edit", "QAClass/Edit/{DataID}", "~/myQA/ClsEdit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        
        
        // [Expo]
        routes.MapPageRoute("Expo_Search", "Activity/Search/{PageID}", "~/myExpo/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Expo_Edit", "Activity/Edit/{DataID}", "~/myExpo/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("Expo_EditSub", "Activity/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myExpo/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
                
                
        // [廣告]
        routes.MapPageRoute("Adv_Search", "Adv/Search/{PageID}", "~/myAdv/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Adv_Edit", "Adv/Edit/{DataID}", "~/myAdv/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("Adv_EditSub", "Adv/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myAdv/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});


        // [產品]
        routes.MapPageRoute("Prod_Search", "Prod/Search/{PageID}", "~/myProd/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Prod_Edit", "Prod/Edit/{DataID}", "~/myProd/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("Prod_EditSub", "Prod/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myProd/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        

        // [Video]
        routes.MapPageRoute("Video_Search", "Video/Search/{PageID}", "~/myVideo/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Video_Edit", "Video/Edit/{DataID}", "~/myVideo/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("Video_EditSub", "Video/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myVideo/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        
    }
</script>
