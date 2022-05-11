<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewLog.aspx.cs" Inherits="myOrder_ViewLog" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="310" />
        <!-- 路徑導航 End -->
        <!-- 錯誤訊息 -->
        <asp:PlaceHolder ID="ph_errMessage" runat="server" Visible="false">
            <div class="alert alert-danger">
                <h4><%=this.GetLocalResourceObject("txt_oops").ToString()%></h4>
            </div>
        </asp:PlaceHolder>
        <!-- Content -->
        <div class="panel panel-success">
            <div class="panel-heading">
                <h4><%=this.GetLocalResourceObject("txt_header").ToString()%></h4>
            </div>
            <div class="panel-body">
                <div class="form-group col-xs-6">
                    <label><%=this.GetLocalResourceObject("txt_訂單編號").ToString()%></label>
                    <div>
                        <h4>
                            <b class="text-primary">
                                <asp:Literal ID="lt_TraceID" runat="server"></asp:Literal>
                            </b>
                        </h4>
                    </div>
                </div>
                <div class="form-group col-xs-6 text-right">
                </div>
            </div>
            <asp:ListView ID="lv_LogList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <table class="table table-striped table-bordered">
                        <tbody>
                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("Log_Desc") %>
                        </td>
                        <td>
                            <%#Eval("Create_Time") %>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="text-center"><h4>- 無錯誤記錄 -</h4></div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <div class="btn-group btn-group-justified btn-group-lg" role="group">
            <a href="<%=Application["WebUrl"] %>EO/List" class="btn btn-success"><%=this.GetLocalResourceObject("btn_Back").ToString()%></a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

