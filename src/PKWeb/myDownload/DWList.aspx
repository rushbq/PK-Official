<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DWList.aspx.cs" Inherits="mySupport_DWList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="252" />
        <!-- 路徑導航 End -->

        <!-- Content Start -->
        <asp:PlaceHolder ID="ph_ModelInfo" runat="server" Visible="false">
            <div class="page-header df-page-header">
                <h3>
                    <asp:Literal ID="lt_ModelName" runat="server"></asp:Literal>&nbsp;&nbsp;
                <small>
                    <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal></small></h3>
            </div>
        </asp:PlaceHolder>
        <div class="has-success">
            <asp:DropDownList ID="ddl_Class" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_Class_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
        <hr />
        <asp:PlaceHolder ID="ph_Data" runat="server" Visible="true">
            <div>
                <asp:Literal ID="lt_DataList" runat="server"></asp:Literal>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_noData" runat="server" Visible="false">
            <div class="exception-info">
                <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
            </div>
        </asp:PlaceHolder>

        <nav>
            <ul class="pager">
                <li><a href="<%=Application["WebUrl"] %>Download">Back</a></li>
            </ul>
        </nav>
        <!-- Content End -->
    </div>
    <!-- start -->

    <!-- end -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

