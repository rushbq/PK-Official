<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ReportList.aspx.cs" Inherits="ReportList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="305" />
        <!-- 路徑導航 End -->
        <!-- Content Start -->
        <div class="row">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="5" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                </LayoutTemplate>
                <GroupTemplate>
                    <div class="col-sm-6">
                        <div class="list-group">
                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                        </div>
                    </div>
                </GroupTemplate>
                <ItemTemplate>
                    <a href="#" class="list-group-item" data-toggle="modal" data-target="#myModal-<%#Eval("ID") %>"><%#Eval("Label") %></a>

                    <!-- /// Modal Start /// -->
                    <div id="myModal-<%#Eval("ID") %>" class="modal fade" tabindex="-1" role="dialog" style="display: none;">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>
                                    <h4 class="modal-title">
                                        <%#Eval("Label") %>
                                    </h4>
                                </div>
                                <div class="modal-body">
                                    <!-- Block Start (年份區間, ui_Year) -->
                                    <asp:PlaceHolder ID="ui_Year" runat="server">
                                        <div class="panel panel-warning">
                                            <div class="panel-heading">
                                                <i class="fa fa-filter"></i>
                                                <span><%=this.GetLocalResourceObject("filter_年份區間").ToString()%></span>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-inline">
                                                    <asp:DropDownList ID="sYear" runat="server" CssClass="form-control">
                                                    </asp:DropDownList>
                                                    ~ 
                                                    <asp:DropDownList ID="eYear" runat="server" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!-- Block End -->
                                </div>
                                <div class="modal-footer text-right">
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                    <asp:Button ID="btn_Open" runat="server" Text="Open" CssClass="btn btn-primary" />
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("ID") %>' />
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- /// Modal End /// -->
                </ItemTemplate>
            </asp:ListView>
        </div>
        <div class="row">
            <div class="col-sm-6">
                <div class="list-group">
                    <a href="<%=Application["WebUrl"] %>PLfilter" class="list-group-item list-group-item-info"><%=this.GetLocalResourceObject("text_報價單").ToString()%></a>
                </div>
            </div>
            <asp:PlaceHolder ID="ph_EC" runat="server">
                <div class="col-sm-6">
                    <div class="list-group">
                        <a href="<%=Application["WebUrl"] %>myReport/EC_Payment.aspx?vid=01dc29c3d3f33c66cab3e80a36647a52" class="list-group-item list-group-item-warning" target="_blank">Pending Payment</a>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
        <!-- Content End -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

