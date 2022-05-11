<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Step2.aspx.cs" Inherits="myOrder_Step2" %>

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
                <p><a class="btn btn-default" href="<%=Application["WebUrl"] %>EO/Step2/<%=Req_DataID %>"><%=this.GetLocalResourceObject("txt_url1").ToString()%></a></p>
                <p><a class="btn btn-default" href="<%=Application["WebUrl"] %>EO/Log/<%=Req_DataID %>"><%=this.GetLocalResourceObject("txt_log").ToString()%></a></p>
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
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
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
                                <th class="text-center" style="width: 100px;">
                                    <asp:Literal ID="lt_Header3" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center" style="width: 100px;">
                                    <asp:Literal ID="lt_Header4" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center" style="width: 100px;">
                                    <asp:Literal ID="lt_Header5" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center" style="width: 100px;">
                                    <asp:Literal ID="lt_Header6" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center" style="width: 80px;">
                                    <asp:Literal ID="lt_Header7" runat="server"></asp:Literal>
                                </th>
                                <th>&nbsp;
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
                            <asp:Literal ID="lt_ModelNo" runat="server" Text='<%#Eval("ERP_ModelNo") %>'></asp:Literal>
                        </td>
                        <td class="text-center">
                            <%#Eval("Cust_ModelNo") %>
                        </td>
                        <td class="text-center">
                            <asp:Literal ID="lt_MOQ" runat="server" Text='<%#Eval("MOQ") %>'></asp:Literal>
                        </td>
                        <td class="text-center">
                            <asp:Literal ID="lt_MinQty" runat="server" Text='<%#Eval("MinQty") %>'></asp:Literal>
                        </td>
                        <td class="text-center text-warning">
                            <%#Eval("InputCnt") %>
                        </td>
                        <td class="text-center">
                            <asp:TextBox ID="tb_BuyQty" runat="server" CssClass="form-control text-center" Text='<%#Eval("BuyCnt") %>' ValidationGroup="List"></asp:TextBox>
                        </td>
                        <td class="text-center">
                            <strong><%#Eval("IsPass") %></strong>
                        </td>
                        <td class="text-center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <asp:LinkButton ID="btn_Edit" runat="server" CssClass="btn btn-primary" CommandName="doEdit" ValidationGroup="List" Visible="false"></asp:LinkButton>
                                &nbsp;<asp:LinkButton ID="btn_Del" runat="server" CssClass="btn btn-danger" CommandName="doDel"></asp:LinkButton>
                                <asp:Literal ID="lt_Msg" runat="server"></asp:Literal>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_Lock" runat="server">
                                <div>
                                    <b class="text-danger"><%#Eval("ProdID") %></b>&nbsp;<small><%=this.GetLocalResourceObject("tip3").ToString()%></small>
                                </div>
                                <%#Eval("doWhat") %>
                            </asp:PlaceHolder>
                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                            <div class="text-left">
                                <asp:RangeValidator ID="rv_tb_BuyQty" runat="server" CssClass="text-danger"
                                    Display="Dynamic" Type="Integer" MaximumValue="999999" MinimumValue="1" ControlToValidate="tb_BuyQty" ValidationGroup="List"></asp:RangeValidator>
                                <asp:RequiredFieldValidator ID="rfv_tb_BuyQty" runat="server" CssClass="text-danger" Display="Dynamic" ControlToValidate="tb_BuyQty" ValidationGroup="List"><%#resPublic.tip_Require %></asp:RequiredFieldValidator>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <div id="myBtns" class="btn-group btn-group-justified btn-group-lg" role="group">
            <a href="<%=Application["WebUrl"] %>EO/Step1-1/<%=Req_DataID %>" class="btn btn-default"><%=this.GetLocalResourceObject("btn_上一步").ToString()%></a>
            <asp:LinkButton ID="lbtn_Reset" runat="server" CssClass="btn btn-primary" OnClick="lbtn_Reset_Click" CausesValidation="false"><span class="fa fa-refresh"></span>&nbsp;<%=this.GetLocalResourceObject("txt_reNew").ToString()%></asp:LinkButton>
            <a href="#!" id="trigger-Next" class="btn btn-success"><%=this.GetLocalResourceObject("btn_下一步").ToString()%></a>
        </div>
        <div id="myProcess" class="progress" style="display: none;">
            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
            </div>
        </div>
        <div style="display: none;">
            <asp:Button ID="btn_Next" runat="server" Text="Next" OnClick="btn_Next_Click" Style="display: none;" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //觸發Next
            $("#trigger-Next").click(function () {
                $("#myBtns").hide();
                $("#myProcess").show();

                //trigger
                $("#MainContent_btn_Next").trigger("click");
            });

        });
    </script>
</asp:Content>

