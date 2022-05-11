<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <!-- [引用] FlexSlider CSS -->
    <link rel="stylesheet" type="text/css" href="<%=cdnUrl %>js/flexslider/flexslider.css" />
    <!-- [自訂] CSS -->
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/index.css?v=20170818" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <!-- 輪播廣告 -->
    <asp:ListView ID="lvAdvList" runat="server" ItemPlaceholderID="ph_Items">
        <LayoutTemplate>
            <div class="flexslider">
                <ul class="slides">
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </ul>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="<%#Eval("Url") %>" target="<%#Eval("Target") %>">
                    <img src="<%#Eval("ImgUrl") %>" alt="<%#Eval("Title") %>" />
                </a>
            </li>
        </ItemTemplate>
    </asp:ListView>

    <!-- 內容區 Start -->
    <div id="main">

        <div class="main-content">
            <div class="container">

                <!-- 哈燒推薦 Start -->
                <asp:PlaceHolder ID="ph_Video" runat="server" Visible="false">
                    <div class="title">
                        <span class="caption"><span class="line line-l"></span><%=this.GetLocalResourceObject("txt_哈燒推薦").ToString()%><span class="line line-r"></span></span>
                    </div>

                    <!-- 產品影片 -->
                    <div class="video-container">
                        <asp:Literal ID="lt_Video" runat="server"></asp:Literal>
                    </div>

                    <!-- 看更多影片 -->
                    <a href="<%=webUrl %><%:Req_Lang %>/Videos">
                        <div class="more-products center card-panel orange"><%=this.GetLocalResourceObject("txt_看更多影片").ToString()%></div>
                    </a>
                </asp:PlaceHolder>
                <!-- 哈燒推薦 End -->

                <!-- 最新消息 -->
                <asp:Repeater ID="myNews" runat="server">
                    <ItemTemplate>
                        <div class="index-news">
                            <div class="title">
                                <span class="caption"><span class="line line-l"></span><%=this.GetLocalResourceObject("txt_最新消息").ToString()%><span class="line line-r"></span></span>
                            </div>
                            <div class="unboxing">
                                <div class="row delete-bottom">
                                    <div class="col l8 s12">
                                        <div class="unboxing-image">
                                            <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/">
                                                <img class="responsive-img" src="<%#Eval("ListPic") %>" alt="" />
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col l4 s12">
                                        <p class="description">
                                            <span class="card-title">
                                                <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%#Eval("Title") %></a>
                                            </span>
                                            <p class="JQellipsis_50 news-description"><a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%#Eval("Desc") %></a></p>
                                            <div class="fix-icon"><i class="material-icons">date_range</i><%#Eval("StartTime") %></div>
                                        </p>
                                        <div class="buy buy-red">
                                            <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%=this.GetLocalResourceObject("txt_閱讀更多").ToString()%><i class="fa fa-angle-right" aria-hidden="true"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- 最新活動 -->
                <asp:Repeater ID="myActivity" runat="server">
                    <ItemTemplate>
                        <div class="activity-news">
                            <div class="title">
                                <span class="caption"><span class="line line-l"></span><%=this.GetLocalResourceObject("txt_最新活動").ToString()%><span class="line line-r"></span></span>
                            </div>
                            <div class="unboxing">
                                <div class="row delete-bottom">
                                    <div class="col l8 s12">
                                        <div class="unboxing-image">
                                            <a href="<%#webUrl %><%:Req_Lang %>/Activity/<%#Server.UrlEncode(Eval("ID").ToString()) %>/">
                                                <img class="responsive-img" src="<%#Eval("ListPic") %>" alt="" />
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col l4 s12">
                                        <p class="description">
                                            <span class="card-title">
                                                <a href="<%#webUrl %><%:Req_Lang %>/Activity/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%#Eval("Title") %></a>
                                            </span>
                                            <p class="JQellipsis_50 news-description"><a href="<%#webUrl %><%:Req_Lang %>/Activity/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%#Eval("SubTitle") %></a></p>
                                            <div class="fix-icon"><i class="material-icons">date_range</i><%#Eval("StartTime") %> ~ <%#Eval("EndTime") %></div>
                                        </p>
                                        <div class="buy buy-red"><a href="<%#webUrl %><%:Req_Lang %>/Activity/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%=this.GetLocalResourceObject("txt_活動內容").ToString()%><i class="fa fa-angle-right" aria-hidden="true"></i></a></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- 熱銷推薦 -->
                <div class="title">
                    <span class="caption"><span class="line line-l"></span><%=this.GetLocalResourceObject("txt_新貨到").ToString()%><span class="line line-r"></span></span>
                </div>
                <asp:ListView ID="lvNewProdList" runat="server" ItemPlaceholderID="ph_Items" GroupItemCount="3" GroupPlaceholderID="ph_Group" OnItemDataBound="lvNewProdList_ItemDataBound">
                    <LayoutTemplate>
                        <asp:PlaceHolder ID="ph_Group" runat="server" />

                    </LayoutTemplate>
                    <GroupTemplate>
                        <div class="row">
                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                        </div>
                    </GroupTemplate>
                    <ItemTemplate>
                        <div class="col l4 s12">
                            <asp:Panel ID="pl_ProdItem" runat="server">
                                <div>
                                    <a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/">
                                        <img class="responsive-img lazy" src="<%#Eval("ListPic") %>" alt="" />
                                    </a>
                                </div>
                                <div class="buy-buttom">
                                    <h3 class="name"><a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/"><%#Eval("ModelName") %></a></h3>
                                    <div class="model-munber"><%#Eval("ModelNo") %></div>
                                    <div class="right-align buy buy-wihte"><a href="<%#fn_Param.ShopUrl(Eval("ModelNo").ToString()) %>" target="_blank"><%=this.GetLocalResourceObject("txt_我要購買").ToString()%><i class="fa fa-angle-right" aria-hidden="true"></i></a></div>
                                </div>
                            </asp:Panel>
                        </div>
                    </ItemTemplate>
                </asp:ListView>
                <!-- 看更多產品 -->
                <a href="<%=webUrl %><%:Req_Lang %>/Products">
                    <div class="more-products center card-panel orange"><%=resPublic.txt_More %></div>
                </a>


                <!-- 哪裡買 -->
                <div class="title">
                    <span class="caption"><span class="line line-l"></span><%=this.GetLocalResourceObject("txt_哪裡買").ToString()%><span class="line line-r"></span></span>
                </div>
                <div class="where2buy-bg">
                    <div class="row">
                        <div class="col s12 m6 l4 offset-l4">
                            <a class="shopping center-align" href="<%=webUrl %><%:Req_Lang %>/OnlineShop">
                                <div>
                                    <img class="lazy" src="<%=cdnUrl %>images/ScienceKits/online-shop.png" alt="" />
                                </div>
                                <div class="buy buy-red"><%=resPublic.menu_201 %><i class="fa fa-angle-right" aria-hidden="true"></i></div>
                            </a>
                        </div>
                        <div class="col s12 m6 l4">
                            <a class="shopping center-align" href="<%=webUrl %><%:Req_Lang %>/Shop">
                                <div>
                                    <img class="lazy" src="<%=cdnUrl %>images/ScienceKits/shop-icon.png" alt="" />
                                </div>
                                <div class="buy buy-red"><%=resPublic.menu_202 %><i class="fa fa-angle-right" aria-hidden="true"></i></div>
                            </a>
                        </div>
                    </div>
                </div>
                <!-- 目錄下載 -->
                <div class="title">
                    <span class="caption"><span class="line line-l"></span><%=this.GetLocalResourceObject("txt_目錄下載").ToString()%><span class="line line-r"></span></span>
                </div>
                <div>
                    <div class="center-align">
                        <img class="responsive-img lazy" src="<%=cdnUrl %>images/ScienceKits/catalog-bg.jpg" alt="" />
                    </div>
                    <div class="center-align buy buy-red"><a href="http://sccatalog.prokits.com.tw/now/<%=fn_Language.Web_Lang %>/index.html" target="_blank"><%=this.GetLocalResourceObject("txt_點我下載").ToString()%></a></div>
                </div>
            </div>
        </div>
    </div>
    <!-- 內容區 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- flexslider -->
    <script src="<%=cdnUrl %>js/flexslider/jquery.flexslider-min.js"></script>
    <script>
        // Can also be used with $(document).ready()
        $(function () {
            $('.flexslider').flexslider({
                animation: "slide"
            });
        });
    </script>

    <!-- lazyload 圖片延遲載入 -->
    <script src="<%=cdnUrl %>js/lazyload/jquery.lazyload.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("img.lazy").lazyload();
        });
    </script>
</asp:Content>


