<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="myQA_Search" %>

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
                    <!-- 搜尋結果宣告 -->
                    <div class="result-note">
                        搜尋&nbsp;"<asp:Literal ID="lt_Keyword" runat="server"></asp:Literal>"&nbsp;結果共
                            <asp:Literal ID="lt_Count" runat="server"></asp:Literal>
                        筆
                    </div>

                    <!-- 搜尋結果列表 -->
                    <div class="result-list">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <ul>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </ul>

                                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <li>
                                    <a href="<%=webUrl %><%:Req_Lang %>/FAQ/<%#Eval("ID") %>">
                                        <div class="search-result-single">
                                            <div class="result-category"><%#Eval("ClassName") %></div>
                                            <div class="result-title"><%#Eval("Title") %></div>
                                        </div>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
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

