<%@ Page Title="" Language="C#" MasterPageFile="~/Site_Clean.master" AutoEventWireup="true" CodeFile="PdfHtml.aspx.cs" Inherits="myOrder_PdfHtml" %>

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
                <p><a class="btn btn-default" href="<%=Application["WebUrl"] %>EO/Step3"><%=this.GetLocalResourceObject("txt_url1").ToString()%></a></p>
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
                    <label><%=this.GetLocalResourceObject("txt_Total").ToString()%></label>
                    <div>
                        <h4>
                            <b class="text-danger">
                                <asp:Literal ID="lt_TotalPrice" runat="server"></asp:Literal>
                            </b>
                        </h4>
                    </div>
                </div>
                <asp:PlaceHolder ID="ph_Freight" runat="server">
                    <div class="row col-xs-12">
                        <div class="alert alert-info text-center">
                            <%=this.GetLocalResourceObject("txt_FreightNote").ToString()%>
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <table class="table table-striped table-bordered">
                        <thead>
                            <tr>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header1" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header2" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header3" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header4" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header5" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header6" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center">
                                    <asp:Literal ID="lt_Header7" runat="server"></asp:Literal>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="text-center">
                            <%#Eval("ERP_ModelNo") %>
                        </td>
                        <td class="text-center">
                            <%#Eval("Cust_ModelNo") %>
                        </td>
                        <td class="text-center">
                            <%#Eval("MOQ") %>
                        </td>
                        <td class="text-center">
                            <%#Eval("MinQty") %>
                        </td>
                        <td class="text-right">
                            <%#fn_stringFormat.Money_Format(Eval("ERP_Price").ToString()) %>
                        </td>
                        <td class="text-center">
                            <%#Eval("BuyCnt") %>
                        </td>
                        <td class="text-right">
                            <b><%# fn_stringFormat.Money_Format((Convert.ToDouble(Eval("ERP_Price")) * Convert.ToInt32(Eval("BuyCnt"))).ToString()) %></b>
                            &nbsp;(<%#Eval("Currency") %>)
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <div id="myBtns" class="btn-group btn-group-justified btn-group-lg" role="group">
            <a href="#!" class="btn btn-default"><%=this.GetLocalResourceObject("btn_上一步").ToString()%></a>
            <a href="#!" id="trigger-Next" class="btn btn-success"><%=this.GetLocalResourceObject("btn_下一步").ToString()%></a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

