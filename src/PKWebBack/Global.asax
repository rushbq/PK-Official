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
        Application["WebName"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebName"];
        Application["DiskUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"];
        Application["WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        Application["File_DiskUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"];
        Application["File_WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"];
        Application["DesKey"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
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
        //routes.MapPageRoute("AuthSetByGroup", "Auth/Group/Set/{DataID}", "~/Authorization/SetGroup.aspx", false,
        //  new RouteValueDictionary {
        //            { "DataID", "New" }});

        // [登入]
        routes.MapPageRoute("LoginPage", "ServiceLogin", "~/Login/LoginPage.aspx", false);
        routes.MapPageRoute("LoginFail", "ServiceLoginFail/{ErrCode}", "~/Login/LoginFail.aspx", false,
             new RouteValueDictionary {
                    { "ErrCode", "9999" }},
             new RouteValueDictionary {
                    { "ErrCode", "[0-9]{1,8}" }
                });


        // [新聞中心]
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


        // [Expo]
        routes.MapPageRoute("Expo_Search", "Expo/Search/{PageID}", "~/myExpo/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Expo_Edit", "Expo/Edit/{DataID}", "~/myExpo/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("Expo_EditSub", "Expo/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myExpo/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});


        // [最新消息]
        routes.MapPageRoute("Article_Search", "Article/Search/{PageID}", "~/myArticle/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Article_Edit", "Article/Edit/{DataID}", "~/myArticle/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("Article_EditSub", "Article/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myArticle/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        routes.MapPageRoute("Article_EditBox", "Article/Edit/DetailBox/{GroupID}/{ParentID}/{DataID}", "~/myArticle/Edit_DetailBox.aspx", false);


        // [Promo] - Stop on 20150826
        //routes.MapPageRoute("Promo_Search", "Promo/Search/{PageID}", "~/myPromo/Search.aspx", false,
        //    new RouteValueDictionary {
        //            { "PageID", "1" }});
        //routes.MapPageRoute("Promo_Edit", "Promo/Edit/{DataID}", "~/myPromo/Edit.aspx", false,
        //    new RouteValueDictionary {
        //            { "DataID", "New" }});
        //routes.MapPageRoute("Promo_EditSub", "Promo/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myPromo/Edit_Sub.aspx", false,
        //   new RouteValueDictionary {
        //            { "LangCode", "en-us" },
        //            { "DataID", "New" }});

        // [Country]
        routes.MapPageRoute("Country_Search", "Country/Search/{PageID}", "~/myCountry/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Country_Edit", "Country/Edit/{DataID}", "~/myCountry/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});

        // [Dealer]
        routes.MapPageRoute("Dealer_Search", "Dealer/Search/{PageID}", "~/myDealer/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Dealer_Edit", "Dealer/Edit/{DataID}", "~/myDealer/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});

        // [FAQ]
        routes.MapPageRoute("FAQ_Search", "FAQ/Search/{PageID}", "~/myFAQ/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("FAQ_Edit", "FAQ/Edit/{DataID}", "~/myFAQ/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("FAQ_EditSub", "FAQ/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myFAQ/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        routes.MapPageRoute("FAQ_EditBox", "FAQ/Edit/DetailBox/{ParentID}/{DataID}", "~/myFAQ/Edit_DetailBox.aspx", false);

        // [KE] - Stop on 20150826
        //routes.MapPageRoute("KE_Search", "KE/Search/{PageID}", "~/myKE/Search.aspx", false,
        //    new RouteValueDictionary {
        //            { "PageID", "1" }});
        //routes.MapPageRoute("KE_Edit", "KE/Edit/{DataID}", "~/myKE/Edit.aspx", false,
        //    new RouteValueDictionary {
        //            { "DataID", "New" }});
        //routes.MapPageRoute("KE_EditSub", "KE/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myKE/Edit_Sub.aspx", false,
        //   new RouteValueDictionary {
        //            { "LangCode", "en-us" },
        //            { "DataID", "New" }});

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
        routes.MapPageRoute("Prod_Batch", "Prod/Batch", "~/myProd/Batch.aspx");
        routes.MapPageRoute("Prod_EditSub", "Prod/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myProd/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        routes.MapPageRoute("Tag_Search", "Config/Tags/{PageID}", "~/mySetting/Tags_Search.aspx", false,
        new RouteValueDictionary {
                    { "PageID", "1" }});

        // [會員]
        routes.MapPageRoute("Member_Search", "Member/Search/{PageID}", "~/myMember/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("Member_Edit", "Member/Edit/{DataID}", "~/myMember/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("EduMember_Search", "EduMember/Search", "~/myEduMember/Search.aspx", false);
        routes.MapPageRoute("WarrMember_Search", "WarrMember/Search", "~/myWarrMember/Search.aspx", false);
        routes.MapPageRoute("DealerMemSearch", "DealerMem/Search/{PageID}", "~/myMember/DealerSearch.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});

        // [產品影片]
        routes.MapPageRoute("PV_Search", "PV/Search/{PageID}", "~/myProdVideo/Search.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});
        routes.MapPageRoute("PV_Edit", "PV/Edit/{DataID}", "~/myProdVideo/Edit.aspx", false,
            new RouteValueDictionary {
                    { "DataID", "New" }});
        routes.MapPageRoute("PV_EditSub", "PV/Edit/Detail/{ParentID}/{LangCode}/{DataID}", "~/myProdVideo/Edit_Sub.aspx", false,
           new RouteValueDictionary {
                    { "LangCode", "en-us" },
                    { "DataID", "New" }});
        routes.MapPageRoute("PV_ModelList", "PVList/Search/{PageID}", "~/myProdVideo/VideoList.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }});


        // [統計資料]
        routes.MapPageRoute("Stat_ProdClass", "Stat/ProdClass", "~/myStat/ProdClass.aspx", false);
        routes.MapPageRoute("Stat_ProdItem", "Stat/ProdItem/{DataID}", "~/myStat/ProdItem.aspx", false,
            new RouteValueDictionary {
                {"DataID", "ALL"}
            });


        // [Orders]
        routes.MapPageRoute("Order_Search", "eOrder/List", "~/myOrders/Search.aspx", false);

    }
</script>
