<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="~/myController/Ascx_Adv.ascx" TagName="Ascx_Adv" TagPrefix="ucAdv" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/index-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- // ADV Start // -->
    <ucAdv:Ascx_Adv ID="Ascx_Adv1" runat="server" />
    <!-- // ADV End // -->
    <div class="container">
        <!-- 4格廣告 Start -->
        <asp:PlaceHolder ID="ph_Block_Adv" runat="server">
            <div class="promo-group">
                <asp:Literal ID="lt_BlockAdv" runat="server"></asp:Literal>
            </div>
        </asp:PlaceHolder>
        <!-- 4格廣告 End -->
        <!-- News & Event Start -->
        <div class="Middle_Message_zone">
            <div class="row">
                <div class="news col-sm-6">
                    <asp:Repeater ID="myNews" runat="server" OnItemDataBound="myNews_ItemDataBound">
                        <ItemTemplate>
                            <div class="header">
                                <h5><%=Resources.resPublic.home_最新消息 %></h5>
                                <div class="More_bt"><a href="<%=Application["WebUrl"] %>News">More</a></div>
                            </div>
                            <div class="media myBody">
                                <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                                <div class="col-xs-6 info-box">
                                    <p class="date"><%#Eval("News_PubDate").ToString().ToDateString("yyyy/MM/dd") %></p>
                                    <a href="<%=Application["WebUrl"] %>News/View/<%#Cryptograph.MD5Encrypt(Eval("News_ID").ToString(), Application["DesKey"].ToString()) %>/">
                                        <h4 class="title"><%#Eval("News_Title") %></h4>
                                    </a>
                                    <a class="text-center detail-btn btn" href="<%=Application["WebUrl"] %>News/View/<%#Cryptograph.MD5Encrypt(Eval("News_ID").ToString(), Application["DesKey"].ToString()) %>/" type="button">
                                        <p>Detail</p>
                                    </a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="events col-sm-6">
                    <asp:Repeater ID="myExpo" runat="server" OnItemDataBound="myExpo_ItemDataBound">
                        <ItemTemplate>
                            <div class="header">
                                <h5><%=Resources.resPublic.home_最新活動 %></h5>
                                <div class="More_bt"><a href="<%=Application["WebUrl"] %>Expo">More</a></div>
                            </div>
                            <div class="media myBody">
                                <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                                <div class="col-xs-6 info-box">
                                    <a href="<%=Application["WebUrl"] %>Expo/<%#Eval("Expo_PubDate").ToString().ToDateString("yyyy") %>">
                                        <h4 class="title"><%#Eval("Expo_Title") %></h4>
                                    </a>
                                    <div class="location">
                                        <i class="material-icons glyphicon glyphicon-map-marker"></i>
                                        <p><%#Eval("Expo_Desc")%></p>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
        <!-- News & Event End -->
        <!-- 四格訊息區塊 Start -->
        <div class="Small_Message_zone">
            <div class="row">
                <div class="col-xs-6 col-sm-3 col-md-3">
                    <div class="block">
                        <h3><%=Resources.resPublic.url_工具達人 %></h3>
                        <a href="http://www.proskittools.com/" target="_blank">
                            <img src="<%=CDN_Url %>images/PKWeb/index/proskittools.jpg" class="img-responsive" alt="" /></a>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-3 col-md-3">
                    <div class="block">
                        <h3><%=Resources.resPublic.home_電子目錄 %></h3>
                        <a href="<%=Catalog_Url%>" target="_blank">
                            <img src="<%=CDN_Url %>images/PKWeb/index/catalogue/<%=fn_Language.PKWeb_Lang %>.jpg?v=20210605" class="img-responsive" alt="Catalog" /></a>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-3 col-md-3">
                    <div class="block">
                        <h3><%=Resources.resPublic.home_聯絡我們 %></h3>
                        <a href="<%=Application["WebUrl"] %>Contact/">
                            <img src="<%=Application["WebUrl"] %>images/contact_us.jpg" class="img-responsive" alt="" /></a>
                    </div>
                </div>
                <div class="col-xs-6 col-sm-3 col-md-3">
                    <div class="block">
                        <h3><%=Resources.resPublic.home_產品影片 %></h3>
                        <a href="<%=Application["WebUrl"] %>Video/Tool/" target="_blank">
                            <img src="<%=CDN_Url %>images/PKWeb/index/product_video.jpg" class="img-responsive" alt="" /></a>
                    </div>
                </div>
            </div>
        </div>
        <!-- 四格訊息區塊 End -->

        <!-- Categories Start -->
        <div class="Categories_zone">
            <h3><%=Resources.resPublic.home_產品類別 %></h3>
            <div class="row">
                <div class="col-md-12">
                    <asp:Literal ID="lt_Catelist" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
        <!-- Categories End-->

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <script>
        $(function () {
            //執行Lazyload
            $("img.lazy").lazyload({
                effect: "fadeIn",
                event: "loadme"
            });
            //觸發Lazyload
            $("img.lazy").trigger("loadme");

        });
    </script>
    <script>
        /*
          四格廣告Click事件
        */
        var captureOutboundLink = function (url, tag) {
            //ga('send', 'event', '事件類別', '事件動作' , '事件標籤')
            ga('send', 'event', '四格廣告', 'Click', tag + '(' + url + ')')
        }
    </script>
</asp:Content>
