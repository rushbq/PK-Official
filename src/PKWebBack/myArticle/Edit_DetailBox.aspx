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
                <label class="control-label col-xs-2">區塊內容 <em>*</em></label>
                <div class="col-xs-10">
                    <asp:TextBox ID="tb_Block_Desc" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-2">區塊圖</label>
                <div class="col-xs-8">
                    <div class="pull-left">
                        <asp:FileUpload ID="fu_BlockPic" runat="server" />
                        <asp:HiddenField ID="hf_OldFile" runat="server" />
                    </div>
                    <div class="pull-right">
                        <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                    </div>
                    <div class="clearfix"></div>
                    <div class="help-block">
                        (<kbd>建議大小：600*400</kbd>)
                    </div>
                </div>
                <div class="col-xs-2">
                    <asp:PlaceHolder ID="ph_files" runat="server" Visible="false">
                        <asp:LinkButton ID="lbtn_DelFile" runat="server" CssClass="btn btn-warning" OnClick="lbtn_DelFile_Click" CausesValidation="false" OnClientClick="return confirm('是否確定刪除檔案?')"><i class="fa fa-trash"></i></asp:LinkButton>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div class="form-group">
                <label class="control-label col-xs-2" for="MainContent_tb_Sort">自訂排序</label>
                <div class="col-xs-5">
                    <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「排序」"
                        Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="BlockAdd" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                        ValidationGroup="BlockAdd" CssClass="styleRed help-block"></asp:RangeValidator>
                </div>
                <div class="col-xs-5 text-right">
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
