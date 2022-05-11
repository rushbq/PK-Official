<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ContactUs.aspx.cs" Inherits="myInfo_ContactUs" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/about-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="504" />
        <!-- 路徑導航 End -->
        <div class="service-group">
            <div class="service-item">
                <a href="mailto:pk@mail.prokits.com.tw">
                    <img src="<%=Application["WebUrl"] %>images/about_us/icons8-new-message-96.png" alt="mail" />
                    <p>E-Mail</p>
                </a>
            </div>
            <div class="service-item">
                <a href="https://m.me/Proskit.taiwan?fbclid=IwAR01NpmeqBRQF1zSGgQWtxWBRRecB1vfidF3Koy7lw1bACJFnfZ28l5FUkc">
                    <img src="<%=Application["WebUrl"] %>images/about_us/icons8-facebook-messenger-96.png" alt="Messenger" />
                    <p>Messenger</p>
                </a>
            </div>
            <div class="service-item">
                <a class="venobox-qrcode" data-vbtype="inline" href="#WeChat-pop">
                    <img src="<%=Application["WebUrl"] %>images/about_us/icons8-weixin-96.png" alt="WeChat" />
                    <p>WeChat</p>
                </a>
                <div id="WeChat-pop" style="display: none;">
                    <img src="<%=Application["WebUrl"] %>images/about_us/工具微信 QR .jpg" alt="WeChat-qrcode" />
                    <h5>請使用WeChat的APP掃描</h5>
                    <h5>或搜尋[宝工工具]</h5>
                    <h5>或搜尋 ID：proskit 02</h5>
                </div>
            </div>
            <div class="service-item">
                <a href="https://line.me/R/ti/p/%40alp6806x">
                    <img src="<%=Application["WebUrl"] %>images/about_us/icons8-line-96.png" alt="LINE" />
                    <p>LINE</p>
                </a>
            </div>
            <div class="service-item">
                <a href="<%=Application["WebUrl"] %>ContactUs/">
                    <img src="<%=Application["WebUrl"] %>images/about_us/icons8-info-squared-96.png" alt="Contact Us" />
                    <p>Support</p>
                </a>
            </div>
        </div>

        <div class="page-header">
            <h4>公司資訊</h4>
        </div>
        <div id="ajaxHtml"></div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            var WebUrl = '<%=Application["WebUrl"]%>';
            var Lang = '<%=fn_Language.PKWeb_Lang%>';
            var strWaiting = '<h3><i class="fa fa-spinner fa-spin"></i> Loading...<h3>';

            //顯示等待中
            $('#ajaxHtml').html(strWaiting);

            //載入Html
            $.ajax({
                url: WebUrl + 'myInfo/html/' + Lang + '/Contact-Us.html?v=0209',
                dataType: "html"

            }).done(function (response) {
                //輸出Html
                $('#ajaxHtml').html(response.replace(/#Weburl#/g, WebUrl));

            }).fail(function (jqXHR, textStatus) {
                //alert(sysErr + textStatus + '-QuickList');
            });

        });
    </script>
    <!-- venobox JS -->
    <link rel="stylesheet" href="<%=Application["WebUrl"] %>js/VenoBox-master/venobox/venobox.css">
    <script src="<%=Application["WebUrl"] %>js/VenoBox-master/venobox/venobox.min.js"></script>
    <script src="<%=Application["WebUrl"] %>js/venobox_zoom.js"></script>
</asp:Content>

