<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PromoList.aspx.cs" Inherits="myNews_PromoList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/news-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="203" />
        <!-- 路徑導航 End -->

        <!-- Content Start -->
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div class="accordion-style panel-group" id="PromoInfo">
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="panel panel-default">
                    <div class="panel-heading " role="tab" id="heading1">
                        <h4 class="panel-title">
                            <a class="accordion-toggle collapsed" data-toggle="collapse" data-parent="#PromoInfo" href="#ptab<%#Eval("Promo_ID") %>">
                                <asp:Literal ID="lt_Pic_S" runat="server"></asp:Literal><%#Eval("Promo_Title") %>
                            </a>
                        </h4>
                    </div>
                    <div id="ptab<%#Eval("Promo_ID") %>" class="panel-collapse collapse">
                        <div class="panel-body">
                            <asp:Literal ID="lt_Pic_B" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div class="exception-info">
                    <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                    <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>


        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <script>
        $(function () {
            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn",
                event: "loadme"
            });
            //觸發Lazyload
            $("img.lazy").trigger("loadme");

            //滑至指定目標
            $('#PromoInfo').on('shown.bs.collapse', function (e) {
                var offset = $('.panel.panel-default > .panel-collapse.in').offset();
                if (offset)
                {
                    $('html, body').animate({
                        scrollTop: offset.top - 150
                    }, 600);
                }
            });

        });
    </script>
</asp:Content>

