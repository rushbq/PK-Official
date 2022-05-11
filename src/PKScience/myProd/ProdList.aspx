<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ProdList.aspx.cs" Inherits="myProd_ProdList" %>

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
                            <h2 class="title-header"><%=resPublic.menu_100 %></h2>
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

                        <asp:ListView ID="lvDataList_Cls1" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_Cls1_ItemDataBound">
                            <LayoutTemplate>
                                <div class="row">
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div class="col s12 m4 l3">
                                    <div class="card">
                                        <div class="row">
                                            <div class="col s6 m12 l12">
                                                <div class="card-image">
                                                    <a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/">
                                                        <img src="<%#Eval("ListPic") %>" alt="" />
                                                    </a>
                                                </div>
                                            </div>
                                            <div class="col s6 m12 l12">
                                                <div class="card-content">
                                                    <div class="product-violator">
                                                        <asp:PlaceHolder ID="ph_NewMark" runat="server">
                                                            <div class="new-label">NEW</div>
                                                        </asp:PlaceHolder>
                                                    </div>

                                                    <span class="card-title"><a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/"><%#Eval("ModelName") %></a></span>
                                                    <div class="model-munber"><%#Eval("ModelNo") %></div>
                                                    <p class="JQellipsis card-description hide-on-small-only"><%#Eval("ListDesc") %></p>
                                                </div>
                                            </div>
                                        </div>
                                        <div>
                                            <p class="JQellipsis card-description hide-on-med-and-up mobile-text-show"><%#Eval("ListDesc") %></p>
                                        </div>
                                        <div class="card-action">
                                            <a href="<%#ShopUrl(Eval("ModelNo").ToString()) %>" target="_blank"><%=this.GetLocalResourceObject("txt_我要購買").ToString()%><i class="fa fa-angle-right" aria-hidden="true"></i></a>
                                        </div>
                                    </div>
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

                        <asp:ListView ID="lvDataList_Cls2" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_Cls2_ItemDataBound">
                            <LayoutTemplate>
                                <div class="row">
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div class="col s12 m4 l3">
                                    <div class="card">
                                        <div class="row">
                                            <div class="col s6 m12 l12">
                                                <div class="card-image">
                                                    <a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/">
                                                        <img src="<%#Eval("ListPic") %>" alt="" />
                                                    </a>
                                                </div>
                                            </div>
                                            <div class="col s6 m12 l12">
                                                <div class="card-content">
                                                    <div class="product-violator">
                                                        <asp:PlaceHolder ID="ph_NewMark" runat="server">
                                                            <div class="new-label">NEW</div>
                                                        </asp:PlaceHolder>
                                                    </div>

                                                    <span class="card-title"><a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/"><%#Eval("ModelName") %></a></span>
                                                    <div class="model-munber"><%#Eval("ModelNo") %></div>
                                                    <p class="JQellipsis card-description hide-on-small-only"><%#Eval("ListDesc") %></p>
                                                </div>
                                            </div>
                                        </div>
                                        <div>
                                            <p class="JQellipsis card-description hide-on-med-and-up mobile-text-show"><%#Eval("ListDesc") %></p>
                                        </div>
                                        <div class="card-action">
                                            <a href="<%#ShopUrl(Eval("ModelNo").ToString()) %>" target="_blank"><%=this.GetLocalResourceObject("txt_我要購買").ToString()%><i class="fa fa-angle-right" aria-hidden="true"></i></a>
                                        </div>
                                    </div>
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
</asp:Content>

