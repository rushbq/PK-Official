<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="myNews_NewsList" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/news.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">

            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=Resources.resPublic.menu_500 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%=webUrl %>"><%=Resources.resPublic.menu_Home %></a></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容區 -->
            <div class="main-content">
                <!-- 列表 -->
                <div class="container">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <div class="page-body">
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="media">
                                <div class="myImg pull-left hidden-xs">
                                    <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/">
                                        <img class="lazy" src="<%#Eval("ListPic") %>" alt="" />
                                    </a>
                                </div>
                                <div class="media-body">
                                    <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>">
                                        <h5 class="date"><%#Eval("StartTime").ToString().ToDateString("yyyy/MM/dd") %></h5>
                                        <h4 class="title"><%#Eval("Title") %></h4>
                                        <p class="text"><%#Eval("Desc").ToString().Replace("\r","<br/>") %></p>
                                    </a>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="exception-info">
                                <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                                <div class="message">Coming soon!</div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>


                    <%--<asp:ListView ID="lvDataList-old" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <div class="row">
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </div>

                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="col s12 m12 l4">
                                <div class="card">
                                    <div class="card-image">
                                        <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/">
                                            <img class="lazy" src="<%#Eval("ListPic") %>" alt="" />
                                        </a>
                                    </div>
                                    <div class="card-content">
                                        <span class="card-title"><a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%#Eval("Title") %></a></span>
                                        <p class="JQellipsis news-description"><%#Eval("Desc") %></p>
                                        <div class="fix-icon"><i class="material-icons">date_range</i><%#Eval("StartTime") %></div>
                                    </div>
                                    <div class="card-action">
                                        <a href="<%#webUrl %><%:Req_Lang %>/NewsPage/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%=Resources.resPublic.txt_More%></a>

                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <h3>Coming soon!</h3>
                        </EmptyDataTemplate>
                    </asp:ListView>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- lazyload 圖片延遲載入 -->
    <script src="https://cdn.prokits.com.tw/js/lazyload/jquery.lazyload.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("img.lazy").lazyload();


        });
    </script>
</asp:Content>

