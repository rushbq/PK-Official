<%@ Page Language="C#" MasterPageFile="~/Site_Box.master" AutoEventWireup="true" CodeFile="Edit_DetailBox.aspx.cs" Inherits="Edit_DetailBox" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <style>
        /* 強迫設定 ckeditor 高度*/
        .cke_contents {
            height: 300px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container-fluid">
        <div class="page-header">
            <h3>編輯區塊內容</h3>
        </div>
        <div class="form-horizontal">
            <div class="form-group">
                <label class="control-label col-xs-2">標題</label>
                <div class="col-xs-10">
                    <asp:TextBox ID="tb_Block_Title" runat="server" CssClass="form-control tip" placeholder="填寫標題" MaxLength="150" ToolTip="字數上限 75 字"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-2">內文</label>
                <div class="col-xs-10">
                    <asp:TextBox ID="tb_Block_Desc" runat="server" TextMode="MultiLine"></asp:TextBox>
                </div>
            </div>

            <div class="form-group">
                <div class="col-xs-12 text-right">
                    <asp:Button ID="btn_Block_Save" runat="server" Text="儲存修改" CssClass="btn btn-primary" OnClick="btn_BlockSave_Click" ValidationGroup="BlockAdd" OnClientClick="blockBox1('BlockAdd', '資料處理中...');" />
                    <button type="button" class="btn btn-default" onclick="parent.$.fancybox.close();">關閉視窗</button>
                    <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="BlockAdd" ShowMessageBox="true" ShowSummary="false" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%-- ckeditor Start --%>
    <script src="//cdn.ckeditor.com/4.5.8/standard/ckeditor.js"></script>
    <script>
        CKEDITOR.replace('MainContent_tb_Block_Desc', {
            customConfig: '<%=Application["WebUrl"]%>/Scripts/ckeditor/config.js'
        });
    </script>
    <%-- ckeditor End --%>
</asp:Content>
