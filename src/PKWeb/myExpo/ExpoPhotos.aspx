<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ExpoPhotos.aspx.cs" Inherits="myExpo_ExpoPhotos" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/News-css") %>
    <%: Styles.Render("~/bundles/venobox-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="202" />
        <!-- 路徑導航 End -->

        <!-- Expo Start -->
        <div class="page-header text-center">
            <h3>
                <asp:Literal ID="lt_Title" runat="server"></asp:Literal></h3>
        </div>

        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div id="myGallery">
                    <ul id="GItems">
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </ul>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <li>
                    <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                </li>
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
        <!-- Expo End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <%: Scripts.Render("~/bundles/venobox-script") %>
    <%: Scripts.Render("~/bundles/Wookmark-script") %>
    <script>
        $(function () {
            /* Venobox */
            $('.zoomPic').venobox({
                border: '10px',
                bgcolor: '#ffffff',
                numeratio: true,
                infinigall: false
            });

            /* Wookmark Start */
            var wk_options = {
                autoResize: true, //當瀏覽器大小改變時會自動更新佈局
                container: $('#myGallery'), //可選配置項，用於一些額外的CSS樣式
                offset: 2, //可選配置項，項目的間距
                itemWidth: 210 //可選配置項，項目的寬度
            };

            // 取得Item
            var wk_handler = $('#GItems li');

            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn",
                event: "loadme",
                load: function () {
                    wk_handler.wookmark(wk_options);
                }
            });
            //觸發Lazyload
            $("img.lazy").trigger("loadme");

            /* Wookmark End */

        });
    </script>
</asp:Content>

