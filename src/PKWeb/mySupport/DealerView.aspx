<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DealerView.aspx.cs" Inherits="mySupport_DealerView" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
    <%: Styles.Render("~/bundles/venobox-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="254" />
        <!-- 路徑導航 End -->

        <!-- Content Start -->
        <div class="page-title">
            <div class="header form-inline">
                <asp:Literal ID="lt_Title" runat="server"></asp:Literal>
                <asp:DropDownList ID="ddl_RegionCode" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_RegionCode_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>

        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div class="page-body">
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="dealer-info">
                    <h4 class="title"><%#Eval("Dealer_Name") %></h4>
                    <div class="pull-left">
                        <ul class="list-inline">
                            <li>
                                <%#showInfo("fa-location-arrow", Eval("Dealer_Location").ToString(), false, "") %>
                                <%#showInfo("fa-user", Eval("Dealer_Contact").ToString(), false, "") %>
                                <%#showInfo("fa-envelope-o", Eval("Dealer_Email").ToString(), false, "") %>
                            </li>
                            <li>
                                <%#showInfo("fa-globe", Eval("Dealer_Website").ToString(), true,Eval("Dealer_Website").ToString()) %>
                                <%#showInfo("fa-fax", Eval("Dealer_Fax").ToString(), false, "") %>

                                <asp:Literal ID="lt_phone" runat="server"></asp:Literal>
                            </li>
                        </ul>
                    </div>
                    <div class="pull-right">
                        <div class="btn-group">
                            <asp:Literal ID="lt_photos" runat="server"></asp:Literal>
                            <div class="btn-group">
                                <a class="btn btn-default" title="Google Map" href="http://maps.google.com/maps?ll=<%#Eval("Dealer_Lat") %>,<%#Eval("Dealer_Lng") %>&q=loc:<%#Eval("Dealer_Lat") %>,<%#Eval("Dealer_Lng") %>" target="_blank"><i class="fa fa-map-marker fa-fw fa-2x"></i></a>
                                <asp:PlaceHolder ID="ph_CN" runat="server" Visible="false">
                                    <a class="btn btn-default" title="Baidu Map" href="http://api.map.baidu.com/geocoder?address=<%#HttpUtility.UrlEncode(Eval("Dealer_Location").ToString()) %>&output=html" target="_blank"><i class="fa fa-paw fa-fw fa-2x"></i></a>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                    <div class="clearfix"></div>

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
    <%: Scripts.Render("~/bundles/venobox-script") %>
    <script>
        $(function () {
            /* Venobox */
            $('.zoomPic').venobox({
                border: '10px',
                bgcolor: '#ffffff',
                numeratio: true,
                infinigall: false
            });

        });
    </script>
</asp:Content>

