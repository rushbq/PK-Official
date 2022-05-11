using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

/// <summary>
/// Js/Css 打包壓縮功能設定
/// </summary>
/// <remarks>
/// 1. <compilation debug="false" /> 要設為false 才有打包效果
/// 2. 若css有使用到圖片，需注意圖片路徑
/// 3. Js 使用 ScriptBundle ; Css 使用 StyleBundle
/// </remarks>
public class BundleConfig
{
    // 如需統合的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkID=303951
    public static void RegisterBundles(BundleCollection bundles)
    {
        // 加入共用的js
        bundles.Add(new ScriptBundle("~/bundles/public").Include(
                        "~/js/bootstrap.min.js",
                        "~/js/jquery-2.1.1.min.js",
                        "~/js/respond.min.js",
                        "~/js/modernizr-*",
                        "~/js/pub.js"));

        // 加入共用的Css (StyleBundle)
        bundles.Add(new StyleImagePathBundle("~/bundles/css").Include(
                        "~/Css/bootstrap.min.css",
                        "~/Css/font-awesome.min.css",
                        "~/Css/site.css"));

        // jQueryUI Js
        bundles.Add(new ScriptBundle("~/bundles/JQ-UI-script").Include(
                       "~/js/jqueryUI/jquery-ui.min.js",
                       "~/js/jqueryUI/catcomplete/catcomplete.js"));
        // jQueryUI Css
        bundles.Add(new StyleImagePathBundle("~/bundles/JQ-UI-css").Include(
                        "~/js/jqueryUI/jquery-ui.min.css",
                        "~/js/jqueryUI/catcomplete/catcomplete.css"));

        // 動態欄位 Js
        bundles.Add(new ScriptBundle("~/bundles/dynamic-Item-script").Include(
                       "~/js/dynamic-ListItem.js"));

        // lazyload Js
        bundles.Add(new ScriptBundle("~/bundles/lazyload-script").Include(
                       "~/js/lazyload/jquery.lazyload.min.js"));

        // BlockUI Js
        bundles.Add(new ScriptBundle("~/bundles/blockUI-script").Include(
                       "~/js/blockUI/jquery.blockUI.js",
                       "~/js/blockUI/customFunc.js"));

        // Wookmark Js
        bundles.Add(new ScriptBundle("~/bundles/Wookmark-script").Include(
                       "~/js/Wookmark/jquery.imagesloaded.js",
                       "~/js/Wookmark/jquery.wookmark.min.js"));

        // fancybox Js
        bundles.Add(new ScriptBundle("~/bundles/fancybox-script").Include(
                       "~/js/fancybox/jquery.fancybox.pack.js",
                       "~/js/fancybox/helpers/jquery.fancybox-thumbs.js",
                       "~/js/fancybox/helpers/jquery.fancybox-buttons.js"));
        // fancybox Css
        bundles.Add(new StyleImagePathBundle("~/bundles/fancybox-css").Include(
                        "~/js/fancybox/jquery.fancybox.css",
                        "~/js/fancybox/helpers/jquery.fancybox-thumbs.css",
                        "~/js/fancybox/helpers/jquery.fancybox-buttons.css"));


        // venobox Js
        bundles.Add(new ScriptBundle("~/bundles/venobox-script").Include(
                       "~/js/venobox/venobox.min.js"));
        // venobox Css
        bundles.Add(new StyleImagePathBundle("~/bundles/venobox-css").Include(
                        "~/js/venobox/venobox.css"));

        // flexslider Js
        bundles.Add(new ScriptBundle("~/bundles/flexslider-script").Include(
                       "~/js/flexslider/jquery.flexslider-min.js"));
        // flexslider Css
        bundles.Add(new StyleImagePathBundle("~/bundles/flexslider-css").Include(
                        "~/js/flexslider/flexslider.css"));

        // DateTimePicker Js
        bundles.Add(new ScriptBundle("~/bundles/DTpicker-script").Include(
                       "~/js/bootstrap-datetimepicker/bootstrap-datetimepicker.js"));
        // DateTimePicker Css
        bundles.Add(new StyleImagePathBundle("~/bundles/DTpicker-css").Include(
                        "~/js/bootstrap-datetimepicker/bootstrap-datetimepicker.min.css"));

        // Steps Js
        bundles.Add(new ScriptBundle("~/bundles/steps-script").Include(
                       "~/js/steps/jquery-steps.js"));
        // Steps Css
        bundles.Add(new StyleImagePathBundle("~/bundles/steps-css").Include(
                        "~/js/steps/jquery-steps.css"));

        // Dealer Download Js
        bundles.Add(new ScriptBundle("~/bundles/floatBar-script").Include(
                       "~/js/DDW-floatBar.js"));

        //每個功能頁的Css
        bundles.Add(new StyleImagePathBundle("~/bundles/index-css").Include("~/Css/index.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/member-css").Include("~/Css/member.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/news-css").Include("~/Css/news.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/product-css").Include("~/Css/product.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/support-css").Include("~/Css/support.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/about-css").Include("~/Css/about_us.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/distributor-css").Include("~/Css/distributor.css"));
        bundles.Add(new StyleImagePathBundle("~/bundles/privacy-css").Include("~/Css/privacy.css"));
        
    }

    /*
     * [使用方式]
     * JS: <%: Scripts.Render("~/bundles/blockUI-script") %>
     * CSS: <%: Styles.Render("~/bundles/JQ-UI-css") %>
     */

}

/// <summary>
/// 解決使用Bundle功能後, Css指定的圖檔路徑會抓不到的問題
/// </summary>
public class StyleImagePathBundle : Bundle
{
    public StyleImagePathBundle(string virtualPath)
        : base(virtualPath, new IBundleTransform[1]
      {
        (IBundleTransform) new CssMinify()
      })
    {
    }

    public StyleImagePathBundle(string virtualPath, string cdnPath)
        : base(virtualPath, cdnPath, new IBundleTransform[1]
      {
        (IBundleTransform) new CssMinify()
      })
    {
    }

    public new Bundle Include(params string[] virtualPaths)
    {
        if (HttpContext.Current.IsDebuggingEnabled)
        {
            // Debugging. Bundling will not occur so act normal and no one gets hurt.
            base.Include(virtualPaths.ToArray());
            return this;
        }

        // In production mode so CSS will be bundled. Correct image paths.
        var bundlePaths = new List<string>();
        var svr = HttpContext.Current.Server;
        foreach (var path in virtualPaths)
        {
            var pattern = new System.Text.RegularExpressions.Regex(@"url\s*\(\s*([""']?)([^:)]+)\1\s*\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var contents = System.IO.File.ReadAllText(svr.MapPath(path));
            if (!pattern.IsMatch(contents))
            {
                bundlePaths.Add(path);
                continue;
            }


            var bundlePath = (System.IO.Path.GetDirectoryName(path) ?? string.Empty).Replace(@"\", "/") + "/";
            var bundleUrlPath = VirtualPathUtility.ToAbsolute(bundlePath);
            var bundleFilePath = String.Format("{0}{1}.bundle{2}",
                                               bundlePath,
                                               System.IO.Path.GetFileNameWithoutExtension(path),
                                               System.IO.Path.GetExtension(path));
            contents = pattern.Replace(contents, "url($1" + bundleUrlPath + "$2$1)");
            System.IO.File.WriteAllText(svr.MapPath(bundleFilePath), contents);
            bundlePaths.Add(bundleFilePath);
        }
        base.Include(bundlePaths.ToArray());
        return this;
    }

}