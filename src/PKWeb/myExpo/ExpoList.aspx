<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ExpoList.aspx.cs" Inherits="myExpo_ExpoList" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/News-css") %>
    <%: Styles.Render("~/bundles/venobox-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="page-header">
            <h3><%=Resources.resPublic.title_展覽活動 %>
            </h3>
        </div>

        <!-- Expo Start -->
        <div class="page-title">
            <div class="pull-left header"><%=Req_Year %></div>
            <div class="pull-right">
                <div class="form-inline">
                    <label for="MainContent_ddl_Year" class="hidden-xs"><%=this.GetLocalResourceObject("txt_年份").ToString()%></label>
                    <asp:DropDownList ID="ddl_Year" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>

        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div class="row">
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="Exhibition_Info">
                    <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
                        <div class="thumbnail">
                            <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                            <div class="caption">
                                <div class="content">
                                    <p class="title"><%#Eval("Expo_Title") %></p>
                                    <p class="location">
                                        <a href="http://maps.google.com/maps?ll=<%#Eval("Expo_Lat") %>,<%#Eval("Expo_Lng") %>&q=loc:<%#Eval("Expo_Lat") %>,<%#Eval("Expo_Lng") %>" target="_blank" title="online Map">
                                            <i class="fa fa-map-marker fa-2x text-danger"></i>
                                        </a>
                                        <%#Eval("Expo_Desc") %>
                                    </p>
                                </div>
                                <p>
                                    <asp:Literal ID="lt_Website" runat="server"></asp:Literal>
                                    <asp:Literal ID="lt_Booth" runat="server"></asp:Literal>
                                    <a href="<%=Application["WebUrl"] %>Expo/Photos/<%#Cryptograph.MD5Encrypt(Eval("Expo_ID").ToString(), Application["DesKey"].ToString()) %>" class="btn btn-default btn-sm" role="button"><%=this.GetLocalResourceObject("txt_照片").ToString()%></a>
                                    <a href="http://maps.google.com/maps?ll=<%#Eval("Expo_Lat") %>,<%#Eval("Expo_Lng") %>&q=loc:<%#Eval("Expo_Lat") %>,<%#Eval("Expo_Lng") %>" class="btn btn-default btn-sm" role="button" title="<%#Eval("Expo_Location") %>" target="_blank"><%=this.GetLocalResourceObject("txt_地圖").ToString()%></a>
                                </p>
                            </div>
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

        <div class="subtitle">
            <h4><%=this.GetLocalResourceObject("txt_年份").ToString()%></h4>
            <ul class="list-inline">
                <asp:Literal ID="lt_listYear" runat="server"></asp:Literal>
            </ul>
        </div>
        <!-- Expo End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <%: Scripts.Render("~/bundles/venobox-script") %>
    <script>
        $(function () {
            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn"
            });

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

