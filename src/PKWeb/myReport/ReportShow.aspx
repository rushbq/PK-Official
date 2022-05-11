<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ReportShow.aspx.cs" Inherits="ReportShow" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- Content Start -->
        <div class="page-header df-page-header">
            <h3><%=Resources.resPublic.title_報表專區 %>&nbsp;<small><asp:Literal ID="lt_Title" runat="server"></asp:Literal></small></h3>
        </div>
        <div class="text-right">
            <a href="<%=BackUrl %>" class="btn btn-warning">Back</a>
        </div>
        <div style="width: auto; padding-top: 5px;">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="RptData" runat="server" Width="100%" Height="100%" AsyncRendering="False" SizeToReportContent="True">
            </rsweb:ReportViewer>
        </div>
        <!-- Content End -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

