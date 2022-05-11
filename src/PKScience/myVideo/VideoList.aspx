<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="VideoList.aspx.cs" Inherits="myVideo_VideoList" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/dev/products.css?v=20170818" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">

            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=resPublic.menu_600 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%=webUrl %>"><%=Resources.resPublic.menu_Home %></a></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容區 -->
            <div class="main-content">
                <!-- 產品列表 -->
                <div class="container products-list-section">
                    <!-- 機器人系列 Start -->
                    <div class="list-all-title">
                        <div class="series-section">
                            <div class="title-displayinline">
                                <h5><%=resPublic.menu_101 %></h5>
                            </div>
                        </div>
                        <div class="clearfix"></div>

                        <asp:ListView ID="lvDataList_Cls1" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <div class="row">
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div class="col s6 m6 l4">
                                    <a class="zoomVideo vbox-item" data-type="iframe" href="<%#Eval("Url") %>">
                                        <div class="card">
                                            <div class="card-image">
                                                <img src="<%#Eval("ListPic") %>" alt="" />
                                            </div>
                                            <div class="card-content">
                                                <span class="card-title"><%#Eval("ModelName") %></span>
                                                <div class="model-munber"><%#Eval("ModelNo") %></div>
                                            </div>
                                        </div>
                                    </a>
                                </div>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <h3>Coming soon!</h3>
                            </EmptyDataTemplate>
                        </asp:ListView>

                    </div>
                    <!-- 機器人系列 End -->

                    <!-- 科學系列 Start -->
                    <div class="list-all-title">
                        <div class="series-section">
                            <div class="title-displayinline">
                                <h5><%=resPublic.menu_102 %></h5>
                            </div>
                        </div>
                        <div class="clearfix"></div>

                        <asp:ListView ID="lvDataList_Cls2" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <div class="row">
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div class="col s6 m6 l4">
                                    <a class="zoomVideo vbox-item" data-type="iframe" href="<%#Eval("Url") %>">
                                        <div class="card">
                                            <div class="card-image">
                                                <img src="<%#Eval("ListPic") %>" alt="" />
                                            </div>
                                            <div class="card-content">
                                                <span class="card-title"><%#Eval("ModelName") %></span>
                                                <div class="model-munber"><%#Eval("ModelNo") %></div>
                                            </div>
                                        </div>
                                    </a>
                                </div>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <h3>Coming soon!</h3>
                            </EmptyDataTemplate>
                        </asp:ListView>

                    </div>
                    <!-- 科學系列 End -->
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- VenoBox -->
    <link rel="stylesheet" type="text/css" href="<%=cdnUrl %>js/venobox/venobox.css" media="screen" />
    <script type="text/javascript" src="<%=cdnUrl %>js/venobox/venobox.min.js"></script>
    <script>
        $(function () {
            $('.zoomVideo').venobox();
        });
    </script>

</asp:Content>

