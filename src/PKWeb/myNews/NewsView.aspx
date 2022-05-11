<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="NewsView.aspx.cs" Inherits="myNews_NewsView" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/news-css") %>
    <meta name="description" content="<%=meta_Title %>" />
    <meta property="og:type" content="website" />
    <meta property="og:site_name" content="Pros'Kit" />
    <meta property="og:url" content="<%=meta_Url %>" />
    <meta property="og:title" content="<%=meta_Title %>" />
    <meta property="og:description" content="<%=meta_Desc %>" />
    <meta property="og:image" content="<%=meta_Image %>" />
    <meta property="og:locale" content="zh_TW" />
    <meta property="fb:app_id" content="<%=fn_Param.FB_AppID %>" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="201" />
        <!-- 路徑導航 End -->
        <!-- News Start -->
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div class="New_Release_Info">
                    <p class="title">
                        <asp:Literal ID="lt_Title" runat="server"></asp:Literal>
                    </p>
                    <p class="date">
                        <asp:Literal ID="lt_Date" runat="server"></asp:Literal>
                        <!-- AddToAny BEGIN -->
                        <div class="a2a_kit a2a_kit_size_32 a2a_default_style">
                            <a class="a2a_dd" href="https://www.addtoany.com/share_save"></a>
                            <a class="a2a_button_facebook"></a>
                            <a class="a2a_button_twitter"></a>
                            <a class="a2a_button_sina_weibo"></a>
                        </div>
                        <script type="text/javascript" src="//static.addtoany.com/menu/page.js"></script>
                        <!-- AddToAny END -->

                    </p>
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <p>
                    <%#HttpUtility.HtmlDecode(Eval("Block_Desc").ToString()).Replace("\r","<br/>") %>
                    <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                </p>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div class="exception-info">
                    <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                    <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>

        <nav>
            <ul class="pager">
                <li>
                    <asp:Literal ID="lt_BackUrl" runat="server"></asp:Literal></li>
            </ul>
        </nav>
        <!-- News End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <script>
        $(function () {
            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn"
            });
        });
    </script>
</asp:Content>
