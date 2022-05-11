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
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/bootbox.min.js",
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/sidebar.js",
                        "~/Scripts/UItoTop/jquery.ui.totop.min.js",
                        "~/Scripts/common/pub.js"));
        
        // 加入共用的Css (StyleBundle)
        bundles.Add(new StyleImagePathBundle("~/bundles/css").Include(
                        "~/Css/bootstrap.min.css",
                        "~/Css/Site.css",
                        "~/Css/sidebar.css",
                        "~/Css/metro-nav.css",
                        "~/Css/font-awesome.min.css",
                        "~/Scripts/UItoTop/ui.totop.css"));

        // jQueryUI Js
        bundles.Add(new ScriptBundle("~/bundles/JQ-UI-script").Include(
                       "~/Scripts/jqueryUI/jquery-ui.min.js",
                       "~/Scripts/jqueryUI/catcomplete/catcomplete.js"));
        // jQueryUI Css
        bundles.Add(new StyleImagePathBundle("~/bundles/JQ-UI-css").Include(
                        "~/Scripts/jqueryUI/jquery-ui.min.css",
                        "~/Scripts/jqueryUI/catcomplete/catcomplete.css"));

        // lazyload Js
        bundles.Add(new ScriptBundle("~/bundles/lazyload-script").Include(
                       "~/Scripts/lazyload/jquery.lazyload.min.js"));

        // BlockUI Js
        bundles.Add(new ScriptBundle("~/bundles/blockUI-script").Include(
                       "~/Scripts/blockUI/jquery.blockUI.js",
                       "~/Scripts/blockUI/customFunc.js"));

        // Wookmark Js
        bundles.Add(new ScriptBundle("~/bundles/Wookmark-script").Include(
                       "~/Scripts/Wookmark/jquery.imagesloaded.js",
                       "~/Scripts/Wookmark/jquery.wookmark.min.js"));
        // Wookmark Css
        bundles.Add(new StyleImagePathBundle("~/bundles/Wookmark-css").Include(
                        "~/Scripts/Wookmark/wookmark.css"));

        // VenoBox Js
        bundles.Add(new ScriptBundle("~/bundles/Venobox-script").Include(
                       "~/Scripts/venobox/venobox.min.js"));
        // VenoBox Css
        bundles.Add(new StyleImagePathBundle("~/bundles/Venobox-css").Include(
                        "~/Scripts/venobox/venobox.css"));

        // fancybox Js
        bundles.Add(new ScriptBundle("~/bundles/fancybox-script").Include(
                       "~/Scripts/fancybox/jquery.fancybox.pack.js"));
        // fancybox Css
        bundles.Add(new StyleImagePathBundle("~/bundles/fancybox-css").Include(
                        "~/Scripts/fancybox/jquery.fancybox.css"));

        // MultiFile Js (migrate-讓舊版判斷瀏覽器的功能可以使用)
        bundles.Add(new ScriptBundle("~/bundles/MultiFile-script").Include(
                       "~/Scripts/jquery-migrate-1.2.1.min.js",
                       "~/Scripts/multiFile/jquery.MultiFile.pack.js"));

        // zTree Js
        bundles.Add(new ScriptBundle("~/bundles/zTree-script").Include(
                       "~/Scripts/zTree/jquery.ztree.core-3.5.min.js",
                       "~/Scripts/zTree/jquery.ztree.excheck-3.5.min.js"));
        // zTree Css
        bundles.Add(new StyleImagePathBundle("~/bundles/zTree-css").Include(
                        "~/Scripts/zTree/css/zTreeStyle.css"));

        // DateTimePicker Js
        bundles.Add(new ScriptBundle("~/bundles/DTpicker-script").Include(
                       "~/Scripts/bootstrap-datetimepicker/bootstrap-datetimepicker.min.js"));
        // DateTimePicker Css
        bundles.Add(new StyleImagePathBundle("~/bundles/DTpicker-css").Include(
                        "~/Scripts/bootstrap-datetimepicker/bootstrap-datetimepicker.min.css"));

        bundles.Add(new ScriptBundle("~/bundles/Moment-script").Include(
               "~/Scripts/bootstrap-datetimepicker/moment-with-locales.min.js"));

        // CKEditor Js
        bundles.Add(new ScriptBundle("~/bundles/CKEditor-script").Include(
                       "~/Scripts/ckeditor/ckeditor.js"));


        /* 頁面自訂Js */
        //群組頁面(Edit.aspx)
        bundles.Add(new ScriptBundle("~/bundles/group-base").Include(
                       "~/Scripts/common/group-base.js"));
        bundles.Add(new ScriptBundle("~/bundles/group-datepicker").Include(
                       "~/Scripts/common/group-datepicker.js"));
        


        // 使用 Modernizr 的開發版本來開發並深入了解。當您準備好量產時，
        // 請使用 http://modernizr.com 中的建置工具來挑選出您需要的測試
        bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


        //使用 respond 讓 IE6-8 支援 CSS3 Media Query
        bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                        "~/Scripts/respond.min.js"));

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