<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.Http" %>
<script RunAt="server">

    protected void Application_Error(object sender, EventArgs e)
    {
        //Exception ex = Server.GetLastError();
        //if (ex is HttpException)
        //{
        //    if (((HttpException)(ex)).GetHttpCode() == 404)
        //        Server.Transfer("~/error.aspx");
        //}
        //// Code that runs when an unhandled error occurs
        //Server.Transfer("~/error.aspx");

    }

    void Application_Start(object sender, EventArgs e)
    {
        // 經常使用的參數
        Application["WebName"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebName"];
        Application["WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        Application["File_WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"];
        Application["API_WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["API_WebUrl"];
        Application["DesKey"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];

        // Js & Css打包壓縮功能
        BundleTable.EnableOptimizations = true;
        BundleConfig.RegisterBundles(BundleTable.Bundles);


        // 載入Routing設定
        RegisterRoutes(RouteTable.Routes);

        /* Web API */
        RouteTable.Routes.MapHttpRoute(
            name: "CheckAccountApi",
            routeTemplate: "Check/{controller}/{account}/",
            defaults: new { account = System.Web.Http.RouteParameter.Optional }
            );

        //[Web API]關閉 XML 回應支援, 強制回應Json
        GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        #region -- 語系判斷 --

        string getLang;

        //[判斷參數] - 判斷Cookie是否存在
        HttpCookie cLang = Request.Cookies["PKWeb_Lang"];
        if ((cLang != null))
        {
            //依Cookie選擇，變換語言別
            switch (cLang.Value.ToString().ToUpper())
            {
                case "ZH-TW":
                    System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo("zh-TW");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;

                case "ZH-CN":
                    currentInfo = new System.Globalization.CultureInfo("zh-CN");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;

                default:
                    currentInfo = new System.Globalization.CultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;
            }

            getLang = cLang.Value;
        }
        else
        {
            //Cookie不存在, 新增預設語系(依瀏覽器預設)
            string defCName = System.Globalization.CultureInfo.CurrentCulture.Name;

            //判斷瀏覽器預設的語系, 除了繁中簡中，其他國家語系都帶英文
            switch (defCName.ToUpper())
            {
                case "ZH-TW":
                case "ZH-CN":
                    break;

                default:
                    defCName = "en-US";
                    break;
            }

            Response.Cookies.Add(new HttpCookie("PKWeb_Lang", defCName));
            Response.Cookies["PKWeb_Lang"].Expires = DateTime.Now.AddYears(1);
            System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo(defCName);
            System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;

            getLang = defCName;
        }

        #endregion

        #region -- 區域判斷 --

        //[判斷參數] - 判斷Cookie是否存在
        HttpCookie cArea = Request.Cookies["PKWeb_Area"];
        if ((cArea == null))
        {
            /* 
             * Cookie不存在, 新增預設區域(依語系判斷) 
             * 區域編號參考: PKSYS.Param_Area
             */
            string defAreaCode;

            switch (getLang.ToUpper())
            {
                case "ZH-TW":
                    //台灣
                    defAreaCode = "2";
                    break;

                case "ZH-CN":
                    //中国
                    defAreaCode = "3";
                    break;

                default:
                    //Global
                    defAreaCode = "1";
                    break;
            }

            //代入Cookie
            Response.Cookies.Add(new HttpCookie("PKWeb_Area", defAreaCode));
            Response.Cookies["PKWeb_Area"].Expires = DateTime.Now.AddYears(1);

        }

        #endregion


        #region -- 來源國家判斷 --

        //[判斷參數] - 判斷Cookie是否存在
        HttpCookie ckFromRegion = Request.Cookies["PKWeb_FromRegion"];
        if ((ckFromRegion == null))
        {
            string CountryCode = fn_Param.GetCountryCode_byIP();

            //代入Cookie
            Response.Cookies.Add(new HttpCookie("PKWeb_FromRegion", CountryCode));
            Response.Cookies["PKWeb_FromRegion"].Expires = DateTime.Now.AddDays(7);

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
        routes.MapPageRoute("HomeRoute", "", "~/Default.aspx");

        /* 產品-Tools */
        routes.MapPageRoute("ProdList", "Products/{ClassID}/{PageID}", "~/myProd/ProdList.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});
        routes.MapPageRoute("HotProdList", "HotProd/{ClassID}/{PageID}", "~/myProd/HotProd.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});
        routes.MapPageRoute("NewProdList", "NewProd/{ClassID}/{PageID}", "~/myProd/NewProd.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});
        routes.MapPageRoute("ProdView", "Product/{DataID}", "~/myProd/ProdView.aspx", false);
        routes.MapPageRoute("ToolSearch", "Search/Tool/{ClassID}/{PageID}", "~/myProd/ProdSearch.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});


        /* 產品-Toy */
        routes.MapPageRoute("ProdToyList", "RobotKits/{ClassID}/{PageID}", "~/myToyProd/ProdList.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});
        routes.MapPageRoute("HotToyProdList", "HotRobot/{ClassID}/{PageID}", "~/myToyProd/HotProd.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});
        routes.MapPageRoute("NewToyProdList", "NewRobot/{ClassID}/{PageID}", "~/myToyProd/NewProd.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});
        routes.MapPageRoute("ProdToyView", "RobotKit/{DataID}", "~/myToyProd/ProdView.aspx", false);
        routes.MapPageRoute("ToySearch", "Search/RobotKit/{ClassID}/{PageID}", "~/myToyProd/ProdSearch.aspx", false,
            new RouteValueDictionary {
                    { "ClassID", "ALL" }
                    , { "PageID", "1" }});

        //Tag Page
        routes.MapPageRoute("TagsHome", "TagHome/{eventName}", "~/myTagEvent/Default.aspx");
        routes.MapPageRoute("TagsSearch", "Tags/{tagName}", "~/myTagEvent/TagSearch.aspx");


        /* 最新消息 */
        //[News]
        routes.MapPageRoute("NewsList", "News/{Year}", "~/myNews/NewsList.aspx", false,
              new RouteValueDictionary {
                    { "Year", DateTime.Now.Year }});
        routes.MapPageRoute("NewsView", "News/View/{DataID}", "~/myNews/NewsView.aspx", false);

        //[展覽]
        routes.MapPageRoute("ExpoList", "Expo/{Year}", "~/myExpo/ExpoList.aspx", false,
              new RouteValueDictionary {
                    { "Year", DateTime.Now.Year }});
        routes.MapPageRoute("ExpoPhotos", "Expo/Photos/{DataID}", "~/myExpo/ExpoPhotos.aspx", false);
        //[Article]
        routes.MapPageRoute("ArticleList", "Article/{Year}", "~/myArticle/ArticleList.aspx", false,
              new RouteValueDictionary {
                    { "Year", DateTime.Now.Year }});
        routes.MapPageRoute("ArticleView", "Article/View/{DataID}", "~/myArticle/ArticleView.aspx", false);


        /* 技術支援 */
        // [常見問題]
        routes.MapPageRoute("QAList", "QA/List/{PageID}/{myData}", "~/myQA/QAList.aspx", false,
            new RouteValueDictionary {
                    { "PageID", "1" }
                    ,{ "myData", "ALL" }});
        routes.MapPageRoute("QAListContent", "QA/Content/{myModel}/{myData}", "~/myQA/QAListContent.aspx", false,
            new RouteValueDictionary {
                    { "myModel", "" }
                    ,{ "myData", "" }});
        routes.MapPageRoute("QAView", "QA/View/{myModel}/{DataID}", "~/myQA/QADetail.aspx", false);

        // [下載中心]
        routes.MapPageRoute("DWIndex", "Download", "~/myDownload/DWIndex.aspx", false);
        routes.MapPageRoute("DWList", "Download/{myClass}/{myModel}/{myData}", "~/myDownload/DWList.aspx", false,
            new RouteValueDictionary {
                    { "myClass", "1" }
                    ,{ "myModel", "ALL" }
                    ,{ "myData", "" }});

        // [產品影片]
        routes.MapPageRoute("PVList_Tool", "Video/Tool/{PageID}", "~/myProd/PVList.aspx", false,
            new RouteValueDictionary { { "PageID", "1" } });
        routes.MapPageRoute("PVList_Toy", "Video/Toy/{PageID}", "~/myToyProd/PVList.aspx", false,
            new RouteValueDictionary { { "PageID", "1" } });


        // [銷售據點]
        routes.MapPageRoute("DealerList", "WhereToBuy", "~/mySupport/DealerList.aspx", false);
        routes.MapPageRoute("DealerView", "WhereToBuy/{DataID}", "~/mySupport/DealerView.aspx", false);

        // [聯絡我們]
        routes.MapPageRoute("myInquiry", "ContactUs", "~/mySupport/Inquiry.aspx", false);
        routes.MapPageRoute("myInqNoti", "ContactNoti/{DataID}", "~/mySupport/Message.aspx", false,
          new RouteValueDictionary {
                    { "DataID", "999" }});

        /* 關於寶工 */
        routes.MapPageRoute("myProfile", "Profile", "~/myInfo/Profile.aspx", false);
        routes.MapPageRoute("myTrademark", "Trademark", "~/myInfo/Trademark.aspx", false);
        routes.MapPageRoute("myQuality", "Quality", "~/myInfo/Quality.aspx", false);
        routes.MapPageRoute("myPrivacy", "Privacy", "~/myInfo/Privacy.aspx", false);
        routes.MapPageRoute("myTerms", "Terms", "~/myInfo/Terms.aspx", false);
        routes.MapPageRoute("myCookiePrivacy", "PrivacyCookies", "~/myInfo/Cookie.aspx", false);
        routes.MapPageRoute("myContact", "Contact", "~/myInfo/ContactUs.aspx", false);

        /* 會員中心 */
        //[Member]
        routes.MapPageRoute("myLogin", "Login", "~/myMember/Login.aspx", false);
        routes.MapPageRoute("myForgotPwd", "ForgotPwd", "~/myMember/ForgotPwd.aspx", false);
        routes.MapPageRoute("myRegister", "SignUp", "~/myMember/SignUp.aspx", false);
        routes.MapPageRoute("myNotification", "Notification/{DataID}", "~/myMember/Message.aspx", false,
          new RouteValueDictionary {
                    { "DataID", "999" }});
        routes.MapPageRoute("myMemberCheck", "Register/{mode}/{token}", "~/myMember/Check.aspx", false);
        routes.MapPageRoute("myChgPwd", "ChangePwd", "~/myMember/ChangePwd.aspx", false);
        routes.MapPageRoute("myData", "MemberData", "~/myMember/MemberData.aspx", false);
        routes.MapPageRoute("myDealer", "DealerApply", "~/myMember/MemberData_Dealer.aspx", false);
        routes.MapPageRoute("myNotification_Box", "Notification_Box/{DataID}", "~/myMember/Message_Box.aspx", false,
          new RouteValueDictionary {
                    { "DataID", "999" }});
        //會員轉換
        routes.MapPageRoute("myConvert", "Transfer/{DataID}", "~/myMember/EcLifeConvert.aspx", false);

        /* 學生期 */
        routes.MapPageRoute("eduRegister", "edu/register/{code}", "~/myEducation/SignUp.aspx", false,
          new RouteValueDictionary {
                    { "code", "tw" }});
        routes.MapPageRoute("eduUpdate", "edu/update/{code}", "~/myEducation/MemberData.aspx", false,
          new RouteValueDictionary {
                    { "code", "tw" }});

        /* 產品註冊 */
        routes.MapPageRoute("prodRegister", "myTool/register/{code}", "~/mySupport/ProdReg.aspx", false,
          new RouteValueDictionary {
                    { "code", "tw" }});

        /* 抽獎活動 */
        routes.MapPageRoute("lotteryIndex", "Lottery/{code}", "~/myLottery/Lot_Index.aspx", false);
        routes.MapPageRoute("lotteryPrize", "Prize/{ParentID}/{DataID}", "~/myLottery/Lot_Result.aspx", false);
        routes.MapPageRoute("lotteryWinner", "Winner/{DataID}/{PageID}", "~/myLottery/Lot_NameList.aspx", false,
           new RouteValueDictionary {
                    { "PageID", "1" }});

        /* 經銷商專區 */
        routes.MapPageRoute("reportIndex", "Report", "~/myReport/ReportList.aspx", false);
        routes.MapPageRoute("reportShow", "Report/{DataID}", "~/myReport/ReportShow.aspx", false);
        routes.MapPageRoute("reportPrice", "PriceList", "~/myReport/PriceList.aspx", false);
        routes.MapPageRoute("reportPriceFilter", "PLfilter", "~/myReport/PriceList_Index.aspx", false);

        /* 經銷商專區-下載 */
        routes.MapPageRoute("DDWPhoto", "D-Download/Photo", "~/myDealer-DW/ProdPhoto.aspx", false);
        routes.MapPageRoute("DDWPackage", "D-Download/Package", "~/myDealer-DW/ProdPackage.aspx", false);
        routes.MapPageRoute("DDWCert", "D-Download/Certification", "~/myDealer-DW/ProdCert.aspx", false);

        /* 經銷商專區-線上下單 */
        routes.MapPageRoute("CartIndex", "EO/List", "~/myOrder/List.aspx", false);
        routes.MapPageRoute("CartS1", "EO/Step1", "~/myOrder/Step1.aspx", false);
        routes.MapPageRoute("CartS101", "EO/Step1-1/{DataID}", "~/myOrder/Step1-1.aspx", false);
        routes.MapPageRoute("CartS2", "EO/Step2/{DataID}", "~/myOrder/Step2.aspx", false);
        routes.MapPageRoute("CartS3", "EO/Step3/{DataID}", "~/myOrder/Step3.aspx", false);
        routes.MapPageRoute("CartS4", "EO/Step4/{DataID}", "~/myOrder/Step4.aspx", false);
        routes.MapPageRoute("CartView", "EO/View/{DataID}", "~/myOrder/View.aspx", false);
        routes.MapPageRoute("CartLog", "EO/Log/{DataID}", "~/myOrder/ViewLog.aspx", false);
        routes.MapPageRoute("CartPdfHtml", "EO/PDFHtml/{DataID}", "~/myOrder/PdfHtml.aspx", false);
        routes.MapPageRoute("CartunShip", "EO/unShip", "~/myOrder/unShipList.aspx", false);


        /* 其他 */
        //Ref網站,檔案下載
        routes.MapPageRoute("fileInRef", "Ref/{Path}/{Name}/{token}", "~/myHandler/Ashx_FileDownload.ashx", false);
        //庫存盤點填寫
        routes.MapPageRoute("StockWrite", "Stock/{token}", "~/myStock/WriteStock.aspx", false);
        routes.MapPageRoute("StockWriteDone", "StockOK", "~/myStock/WriteDone.aspx", false);


        /* EDM */
        //[EDM]
        routes.MapPageRoute("EDMList", "EDM/{ClassID}/{ProgID}/{Year}", "~/myEDM/EDMList.aspx", false,
              new RouteValueDictionary {
                    { "Year", DateTime.Now.Year }});


        //exception
        routes.MapPageRoute("myException", "myExp/{DataID}", "~/myException/Message.aspx", false,
          new RouteValueDictionary {
                    { "DataID", "999" }});

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
