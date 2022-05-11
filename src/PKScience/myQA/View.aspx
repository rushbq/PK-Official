<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="View.aspx.cs" Inherits="myQA_View" %>

<%@ Register Src="Ascx_Search.ascx" TagName="Ascx_Sh" TagPrefix="ucSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/dev/faq.css?v=20171012" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">
            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=Resources.resPublic.menu_700 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%=webUrl %>"><%=Resources.resPublic.menu_Home %></a></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容區 -->
            <div class="main-content">
                <!-- 常見問題搜尋 -->
                <div class="subheader">
                    <ucSearch:Ascx_Sh ID="Ascx_Sh1" runat="server" />
                </div>

                <div class="container">
                    <!--問題麵包屑-->
                    <div class="question-guidance">
                        <div class="guidance-floor">
                            <span class="floor-link">
                                <asp:Literal ID="lt_ClassHeader" runat="server"></asp:Literal>
                            </span>
                            <span class="floor-link"><i class="material-icons">trending_flat</i></span>
                            <span class="floor-link">
                                <asp:Literal ID="lt_Header" runat="server"></asp:Literal>
                            </span>
                        </div>
                        <div class="clearfix"></div>
                    </div>

                    <!-- 內容 -->
                    <div class="question-content">
                        <div>
                            <h2 class="question-title">
                                <asp:Literal ID="lt_Title" runat="server"></asp:Literal>
                            </h2>
                        </div>
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                            <LayoutTemplate>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                <%# Server.HtmlDecode(Eval("Desc").ToString()) %>
                                <asp:Literal ID="lt_Img" runat="server"></asp:Literal>
                            </ItemTemplate>
                        </asp:ListView>

                        <div class="model-name-associated">
                            <h5><%=Resources.resPublic.txt_FAQ_Prods %></h5>
                            <asp:ListView ID="lvProdList" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <ul>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </ul>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <li>
                                        <a href="<%#webUrl %><%:Req_Lang %>/Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>"><span class="model-number"><%#Eval("ModelNo") %></span><%#Eval("ModelName") %></a>
                                    </li>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>

                        <!-- 回列表 -->
                        <div class="center-align">
                            <a href="<%=webUrl %><%:Req_Lang %>/FAQ-Home" class="waves-effect waves-light btn"><i class="material-icons right">list</i><%=Resources.resPublic.btn_backlist%></a>
                        </div>
                    </div>

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

    <script>
        $(function () {
            /* Search */
            $(".faq-Search").keypress(function (event) {
                code = (event.keyCode ? event.keyCode : event.which);
                if (code == 13) {
                    //get string
                    var searchTxt = $(this).val();

                    //redirect
                    location.href = '<%=webUrl%><%:Req_Lang%>/FAQ-Search/?k=' + encodeURIComponent(searchTxt);

                    //停止預設動作(ex:submit)
                    event.preventDefault();
                }
            });

        });
    </script>
</asp:Content>

