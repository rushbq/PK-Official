<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Step4.aspx.cs" Inherits="myOrder_Step4" %>

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
                <div class="form-group">
                    <label><%=this.GetLocalResourceObject("txt_訂單編號").ToString()%></label>
                    <div>
                        <h4>
                            <b class="text-primary">
                                <asp:Literal ID="lt_TraceID" runat="server"></asp:Literal>
                            </b>
                        </h4>
                    </div>
                </div>

                <div class="row col-xs-12">
                    <h4><%=this.GetLocalResourceObject("txt_Note").ToString()%></h4>
                    <a href="<%=Application["WebUrl"] %>EO/Step1" class="btn btn-primary"><%=this.GetLocalResourceObject("btn_Url1").ToString()%></a>
                    <a href="<%=Application["WebUrl"] %>EO/List" class="btn btn-success"><%=this.GetLocalResourceObject("btn_Url2").ToString()%></a>
                </div>
            </div>

        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

