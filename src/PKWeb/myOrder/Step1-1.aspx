<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Step1-1.aspx.cs" Inherits="myOrder_Step1_1" %>

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
                <asp:Literal ID="lt_ErrMsg" runat="server"></asp:Literal>
                <p><a class="btn btn-default" href="<%=Application["WebUrl"] %>EO/Step1-1/<%=Req_DataID %>"><%=this.GetLocalResourceObject("txt_url1").ToString()%></a></p>
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
                <div class="form-group">
                    <label><%=this.GetLocalResourceObject("txt_選擇工作表").ToString()%></label>
                    <div>
                        <asp:DropDownList ID="ddl_Sheet" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddl_Sheet_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <label><%=this.GetLocalResourceObject("txt_資料預覽").ToString()%></label>
                    <div>
                        <table id="listTable" class="stripe" cellspacing="0" width="100%" style="width: 100%;">
                            <asp:Literal ID="lt_tbBody" runat="server"></asp:Literal>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div id="myBtns" class="btn-group btn-group-justified btn-group-lg" role="group">
            <asp:LinkButton ID="lbtn_Prev" runat="server" CssClass="btn btn-default" OnClick="lbtn_Prev_Click" CausesValidation="false"><%=this.GetLocalResourceObject("btn_上一步").ToString()%></asp:LinkButton>
            <a href="#!" id="trigger-Next" class="btn btn-success"><%=this.GetLocalResourceObject("btn_下一步").ToString()%></a>
        </div>
         <div id="myProcess" class="progress" style="display: none;">
            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
            </div>
        </div>
        <div style="display: none;">
            <asp:Button ID="btn_Next" runat="server" Text="Next" OnClick="btn_Next_Click" Style="display: none;" />

            <!-- Hidden Field -->
            <asp:HiddenField ID="hf_FullFileName" runat="server" />
            <asp:HiddenField ID="hf_Type" runat="server" />
            <asp:HiddenField ID="hf_CustID" runat="server" />
            <asp:HiddenField ID="hf_TraceID" runat="server" />
            <asp:HiddenField ID="hf_DBName" runat="server" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script>
        $(function () {
            //觸發Next
            $("#trigger-Next").click(function () {
                $("#myBtns").hide();
                $("#myProcess").show();

                //trigger
                $("#MainContent_btn_Next").trigger("click");
            });

            //使用DataTable
            $('#listTable').DataTable({
                "searching": true,  //搜尋
                "ordering": true,   //排序
                "paging": true,     //分頁
                "info": false,      //頁數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                "pageLength": 20,   //顯示筆數預設值     
                //捲軸設定
                "scrollY": '50vh',
                "scrollCollapse": true,
                "scrollX": true
            });


        });
    </script>
    <%-- DataTable End --%>
    <style>
        #listTable td {
            word-break: keep-all;
            word-wrap: break-word;
        }
    </style>
</asp:Content>

