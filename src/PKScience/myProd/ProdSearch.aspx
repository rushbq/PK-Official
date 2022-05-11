<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ProdSearch.aspx.cs" Inherits="myProd_ProdSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/dev/products.css?v=20171013" rel="stylesheet" />
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/dev/search.css?v=20171013" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">
            <!-- 主要內容區 -->
            <div class="main-content">
                <!-- 列表 -->
                <div class="container products-list-section">
                    <div class="card-content grey lighten-4">
                        <div>
                            搜尋&nbsp;"<asp:Literal ID="lt_Keyword" runat="server"></asp:Literal>"&nbsp;結果共
                            <asp:Literal ID="lt_Count" runat="server"></asp:Literal>
                            筆
                        </div>
                    </div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <div class="row">
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </div>

                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="col s12 m4 l3">
                                <div class="card">
                                    <div class="card-image">
                                        <a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/">
                                            <img class="lazy" src="<%#Eval("ListPic") %>" alt="" />
                                        </a>
                                    </div>

                                    <div class="card-content">
                                        <asp:PlaceHolder ID="ph_NewMark" runat="server">
                                            <div class="new-label">NEW</div>
                                        </asp:PlaceHolder>
                                        <span class="card-title"><a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/"><%#Eval("ModelName") %></a></span>
                                        <div class="model-munber"><%#Eval("ModelNo") %></div>
                                        <p class="JQellipsis card-description"><%#Eval("ListDesc") %></p>
                                    </div>
                                    <div class="card-action">
                                        <a href="<%#ShopUrl(Eval("ModelNo").ToString()) %>" target="_blank"><%=this.GetLocalResourceObject("txt_我要購買").ToString()%><i class="fa fa-angle-right" aria-hidden="true"></i></a>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- lazyload 圖片延遲載入 -->
    <script src="<%=cdnUrl %>js/lazyload/jquery.lazyload.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("img.lazy").lazyload();
        });
    </script>

</asp:Content>

