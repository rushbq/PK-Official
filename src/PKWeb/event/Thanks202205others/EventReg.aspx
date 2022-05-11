<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EventReg.aspx.cs" Inherits="EventReg" %>


<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <img src="act2.jpg?v=0429" class="img-responsive" alt="登錄活動" />
    <div class="container">
        <div class="login login-bg">
            <div class="login-area">
                <div class="page-title">
                    <div class="header"><%:eventTitle %>專區</div>
                </div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-md-2 text-right">* 姓名</label>
                        <div class="col-md-6 has-error">
                            <asp:TextBox ID="tb_FullName" runat="server" CssClass="form-control myLogin" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_FullName" runat="server" Display="Dynamic"
                                ControlToValidate="tb_FullName" ValidationGroup="Add"><div class="alert alert-danger">(此為必填欄位)</div></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-4">
                            <asp:RadioButtonList ID="rbl_Sex" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="M">&nbsp;先生&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="F">&nbsp;小姐</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">* 手機號碼</label>
                        <div class="col-md-10 has-error">
                            <asp:TextBox ID="tb_PhoneNumber" runat="server" CssClass="form-control myLogin" MaxLength="10" placeholder="0912345678"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_PhoneNumber" runat="server" Display="Dynamic"
                                ControlToValidate="tb_PhoneNumber" ValidationGroup="Add"><div class="alert alert-danger">(此為必填欄位)</div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_PhoneNumber" runat="server" ControlToValidate="tb_PhoneNumber" Display="Dynamic"
                                ValidationExpression="^09[0-9]{8}$" ValidationGroup="Add"><div class="alert alert-danger">(格式不正確)</div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">* Email</label>
                        <div class="col-md-10 has-error">
                            <asp:TextBox ID="tb_Email" runat="server" CssClass="form-control myLogin" MaxLength="100" placeholder="helloworld@gmail.com"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Email" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Email" ValidationGroup="Add"><div class="alert alert-danger">(此為必填欄位)</div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" Display="Dynamic"
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Add"><div class="alert alert-danger">(格式不正確)</div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">* 發票號碼</label>
                        <div class="col-md-4 has-error">
                            <asp:TextBox ID="tb_InvoiceNo" runat="server" CssClass="form-control myLogin" MaxLength="10" placeholder=""></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_InvoiceNo" runat="server" Display="Dynamic"
                                ControlToValidate="tb_InvoiceNo" ValidationGroup="Add"><div class="alert alert-danger">(此為必填欄位)</div></asp:RequiredFieldValidator>
                            <%--<asp:RegularExpressionValidator ID="rev_tb_InvoiceNo" runat="server" ControlToValidate="tb_InvoiceNo" Display="Dynamic"
                                ValidationExpression="^[A-Z]{2}\d{8}$" ValidationGroup="Add"><div class="alert alert-danger">(格式不正確:如 AB12345678)</div></asp:RegularExpressionValidator>--%>
                        </div>
                        <label class="col-md-2 text-right">* 金額</label>
                        <div class="col-md-4 has-error">
                            <asp:TextBox ID="tb_InvoicePrice" runat="server" CssClass="form-control myLogin" MaxLength="10" placeholder=""></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_InvoicePrice" runat="server" Display="Dynamic"
                                ControlToValidate="tb_InvoicePrice" ValidationGroup="Add"><div class="alert alert-danger">(此為必填欄位)</div></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cv_tb_InvoicePrice" runat="server" ControlToValidate="tb_InvoicePrice"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger">(格式不正確)</div></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">生日</label>
                        <div class="col-md-6 form-inline">
                            <div class="input-group date showDate" data-link-field="MainContent_tb_Birthday">
                                <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center myLogin" ReadOnly="true"></asp:TextBox>
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                            </div>
                            <asp:TextBox ID="tb_Birthday" runat="server" Style="display: none"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">Line ID</label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_Line" runat="server" CssClass="form-control myLogin" MaxLength="120"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">* 購入來源</label>
                        <div class="col-md-10">
                            <asp:RadioButtonList ID="rbl_BuyWhere" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="A">&nbsp;電商&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="B">&nbsp;門市</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right">* 購買產品</label>
                        <div class="col-md-10">
                            <asp:CheckBoxList ID="cbl_BuyTypes" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="A">&nbsp;工具&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="B">&nbsp;科學玩具</asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-2 text-right">輸入驗證碼</label>
                        <div class="col-md-5">
                            <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myLogin upper" rel="nofollow" placeholder="輸入圖示中的驗證碼"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                                ControlToValidate="tb_VerifyCode" ValidationGroup="Add"><div class="alert alert-danger">(此為必填欄位)</div></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-5">
                            <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                            <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-12 text-right">
                            <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-green-block btn-commit btn-block" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Add','Processing...')" ValidationGroup="Add" Text="填寫完成, 送出資料" Visible="false" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="login-area dealer-box" style="padding: 10px;">
                <div class="page-title">
                    <div class="header">&nbsp;</div>
                </div>
                <div>
登錄抽獎活動辦法如下：<br />
活動時間；2022/5/1 AM 00:00 開始至 2022/5/31 PM 23:59〈網路消費依結帳時間，店面消費依當日營業時間〉
滿額登錄抽獎活動，於任一平台，購買寶工工具或寶工科學玩具，單筆消費滿額即可參加一次抽獎機會。
抽獎活動預計於 2022/6/15 前抽出，中獎者須提供完整並清楚的購買證明與明細，否則取消中獎資格。
聯絡資料請務必填寫正確，若中獎聯絡不到本人視同放棄。
                </div>
            </div>

        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* 偵測enter */
            $(".myLogin").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Submit").trigger("click");
                }
            });


            /* refresh驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            /* 自動轉大寫 */
            $(document).on('blur', ".upper", function () {
                $(this).val(function (_, val) {
                    return val.toUpperCase();
                });
            });
        });

    </script>
    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            $('.showDate').datetimepicker({
                format: 'yyyy/mm/dd',   //目前欄位格式
                linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
                todayBtn: false,     //顯示today
                todayHighlight: false,   //將today設置高亮
                autoclose: true,    //選擇完畢後自動關閉
                startView: 4,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
                maxView: 4,
                minView: 2,
                forceParse: false
            });

        });

    </script>
    <%-- DatePicker End --%>
</asp:Content>

