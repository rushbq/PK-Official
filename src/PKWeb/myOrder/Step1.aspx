<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Step1.aspx.cs" Inherits="myOrder_Step1" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="310" />
        <!-- 路徑導航 End -->
        <!-- 偵測到未完成，詢問是否繼續匯入 -->
        <asp:PlaceHolder ID="ph_InComplete" runat="server" Visible="false">
            <div class="alert alert-danger" role="alert">
                <h4><%=this.GetLocalResourceObject("tip1").ToString()%></h4>
                <p><%=this.GetLocalResourceObject("tip2").ToString()%></p>
                <p>
                    <asp:Literal ID="lt_UrlStep2" runat="server"></asp:Literal>
                    <button type="button" id="closeAlert" class="btn btn-default"><%=this.GetLocalResourceObject("tip4").ToString()%></button>
                </p>
            </div>
        </asp:PlaceHolder>
        <!-- 錯誤訊息 -->
        <asp:PlaceHolder ID="ph_errMessage" runat="server" Visible="false">
            <div class="alert alert-danger">
                <h4><%=this.GetLocalResourceObject("txt_oops").ToString()%></h4>
                <p><a class="btn btn-default" href="<%=Application["WebUrl"] %>EO/Step1"><%=this.GetLocalResourceObject("txt_url1").ToString()%></a></p>               
            </div>
        </asp:PlaceHolder>
        <!-- Content -->
        <div class="panel panel-success">
            <div class="panel-heading">
                <h4><%=this.GetLocalResourceObject("txt_header").ToString()%></h4>
            </div>
            <div class="panel-body">
                <div class="form-group">
                    <label><%=this.GetLocalResourceObject("txt_請選擇品號來源").ToString()%></label>
                    <div>
                        <div class="btn-group" data-toggle="buttons">
                            <asp:RadioButton ID="rb_DataType1" runat="server" CssClass="btn btn-default active" GroupName="rbl_Type" Checked="true" />
                            <asp:RadioButton ID="rb_DataType2" runat="server" CssClass="btn btn-default" GroupName="rbl_Type" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <label><%=this.GetLocalResourceObject("txt_選擇Excel").ToString()%></label>
                    <div>
                        <asp:FileUpload ID="file_Upload" runat="server" AllowMultiple="false" accept=".xls,.xlsx" />
                        <div class="text-danger">
                            <asp:Literal ID="lt_UploadMessage" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <div class="form-group"><a href="<%=refUrl() %>OrderSample/<%=fn_Language.PKWeb_Lang %>.xlsx"><i class="fa fa-download"></i>&nbsp;<%=this.GetLocalResourceObject("txt_Sample").ToString()%></a></div>
            </div>
        </div>
        <div id="myBtns" class="btn-group btn-group-justified btn-group-lg" role="group">
            <a href="<%=Application["WebUrl"] %>EO/List" class="btn btn-default"><%=this.GetLocalResourceObject("btn_上一步").ToString()%></a>
            <a href="#!" id="trigger-Next" class="btn btn-success"><%=this.GetLocalResourceObject("btn_下一步").ToString()%></a>
        </div>
        <div id="myProcess" class="progress" style="display: none;">
            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
            </div>
        </div>
        <div style="display: none;">
            <asp:Button ID="btn_Next" runat="server" Text="Next" OnClick="btn_Next_Click" Style="display: none;" ValidationGroup="Add" />
            <asp:HiddenField ID="hf_OldTraceID" runat="server" />
            <asp:HiddenField ID="hf_OldDataID" runat="server" />
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

            $("#closeAlert").click(function () {
                $(".alert").hide();
            });

        });


    </script>

</asp:Content>

