<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ExpoList.aspx.cs" Inherits="myProd_ExpoList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/acivity.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">

            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=Resources.resPublic.menu_300 %></h2>
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
                <div class="container products-list-section">

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
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
                                        <a href="<%#webUrl %><%:Req_Lang %>/Activity/<%#Server.UrlEncode(Eval("ID").ToString()) %>/">
                                            <img class="lazy" src="<%#Eval("ListPic") %>" alt="" />
                                        </a>
                                    </div>
                                    <div class="card-content">
                                        <span class="card-title"><%#Eval("Title") %></span>
                                        <p class="acivity-description"><%#Eval("SubTitle") %></p>
                                        <div class="acivity-time">
                                            <div class="fix-icon"><i class="material-icons">date_range</i><%#Eval("StartTime") %> ~ <%#Eval("EndTime") %></div>
                                        </div>
                                        <div class="right-align">
                                            <a class="waves-effect waves-light btn" href="<%#webUrl %><%:Req_Lang %>/Activity/<%#Server.UrlEncode(Eval("ID").ToString()) %>/"><%=this.GetLocalResourceObject("txt_活動內容").ToString()%></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <h3>Coming soon!</h3>
                        </EmptyDataTemplate>
                    </asp:ListView>
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

