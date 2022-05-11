<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Inquiry.aspx.cs" Inherits="Inquiry" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="page-header">
            <h3>
                <%=Resources.resPublic.title_技術諮詢 %>
            </h3>
        </div>
        <div class="inq">
            <div class="content-title">
                <div class="header"><%=this.GetLocalResourceObject("txt_Title").ToString()%></div>
            </div>
            <div class="form-horizontal">
                <div class="form-group">
                    <label class="col-sm-3"><%=this.GetLocalResourceObject("fld_問題類別").ToString()%> <em class="text-red">*</em></label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="ddl_ClassID" runat="server" CssClass="form-control"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfv_ddl_ClassID" runat="server" Display="Dynamic"
                            ControlToValidate="ddl_ClassID" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-3"><%=this.GetLocalResourceObject("fld_訊息").ToString()%> <em class="text-red">*</em></label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="tb_Message" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="10" MaxLength="500"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_Message" runat="server" Display="Dynamic"
                            ControlToValidate="tb_Message" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                    </div>
                </div>

                <!-- 驗證碼 Start -->
                <div class="form-group">
                    <label class="col-sm-3"><%=this.GetLocalResourceObject("txt_驗證碼").ToString()%> <em class="text-red">*</em></label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myEnter"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                            ControlToValidate="tb_VerifyCode" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-3 col-sm-9">
                        <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                        <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                    </div>
                </div>
                <!-- 驗證碼 End -->
                <div class="form-group">
                    <div class="col-sm-offset-3 col-sm-9">
                        <label>
                            <asp:CheckBox ID="cb_agree" runat="server" /><%=this.GetLocalResourceObject("txt_隱私權聲明").ToString()%></label>
                    </div>
                </div>

                <div class="form-group">
                    <div class="col-sm-offset-3 col-sm-9">
                        <input type="button" id="doSubmit" class="btn btn-success" disabled="disabled" value="<%=this.GetLocalResourceObject("txt_傳送").ToString() %>" />
                    </div>
                </div>
                <div class="hidden">
                    <asp:Button ID="btn_Submit" runat="server" OnClick="btn_Submit_Click" ValidationGroup="Add" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* Document Load, Postback時判斷是否已勾選 */
            var get_check = $('#MainContent_cb_agree');
            var get_submit = $('#doSubmit');

            if (get_check.prop("checked")) {
                get_submit.prop("disabled", "");

            } else {
                get_submit.prop("disabled", "disabled");
            }


            /* 驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            /* 同意鈕 */
            $('#MainContent_cb_agree').click(function () {
                var btn_submit = $('#doSubmit');

                if ($(this).prop("checked")) {
                    btn_submit.prop("disabled", "");

                } else {
                    btn_submit.prop("disabled", "disabled");

                }

            });

            /* 送出鈕 */
            $('#doSubmit').click(function () {
                blockBox1('Add', 'Processing...');
                $('#MainContent_btn_Submit').trigger('click');
            });

            /* Enter 偵測 */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#doSubmit").trigger("click");
                }
            });

        });
    </script>
</asp:Content>

