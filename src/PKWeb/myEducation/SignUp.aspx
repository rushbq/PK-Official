<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SignUp.aspx.cs" Inherits="SignUp" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="610" />
        <!-- 路徑導航 End -->
        <div class="row login">
            <div class="login-area col-sm-7">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_資料提供").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 text-right"><%=this.GetLocalResourceObject("txt_帳號").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Email" runat="server" CssClass="form-control myLogin" type="email" autocomplete="off"></asp:TextBox>
                            <div id="checkOK" class="alert alert-info text-green" style="display: none;">
                                <i class="fa fa-check fa-fw"></i>&nbsp;<%=this.GetLocalResourceObject("tip_帳號可使用").ToString()%>
                            </div>
                            <div id="checkNO" class="alert alert-info text-red" style="display: none;">
                                <i class="fa fa-times fa-fw"></i>&nbsp;<%=this.GetLocalResourceObject("tip_帳號不可使用").ToString()%>
                            </div>
                            <div class="help-block text-blue">
                                <a href="<%=Application["WebUrl"] %>Login?u=<%=Cryptograph.MD5Encrypt(Application["WebUrl"] + "edu/update/tw", Application["DesKey"].ToString()) %>"><i class="fa fa-info-circle fa-fw"></i>&nbsp;已經是會員請點此</a>
                            </div>
                            <asp:RequiredFieldValidator ID="rfv_tb_Email" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Email" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 text-right"><%=this.GetLocalResourceObject("txt_密碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Password" runat="server" CssClass="form-control myLogin" TextMode="Password" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Password" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Password" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Password" runat="server"
                                Display="Dynamic" ControlToValidate="tb_Password" ValidationExpression="^(?!.*[^\x30-\x39\x41-\x5A\x61-\x7A])(?=.{8,20})(?=.*\d)(?=.*[A-Z]).*$"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_密碼規則").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 text-right"><%=this.GetLocalResourceObject("txt_確認密碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_CfmPassword" runat="server" CssClass="form-control myLogin" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_CfmPassword" runat="server" Display="Dynamic"
                                ControlToValidate="tb_CfmPassword" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cv_tb_ConfirmPwd" runat="server" Display="Dynamic" ControlToValidate="tb_CfmPassword" ControlToCompare="tb_Password" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_密碼不一致").ToString()%></div></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_名").ToString()%></label>
                        <div class="col-md-3">
                            <asp:TextBox ID="tb_LastName" runat="server" CssClass="form-control myLogin" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_LastName" runat="server" Display="Dynamic"
                                ControlToValidate="tb_LastName" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_姓").ToString()%></label>
                        <div class="col-md-3">
                            <asp:TextBox ID="tb_FirstName" runat="server" CssClass="form-control myLogin" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_FirstName" runat="server" Display="Dynamic"
                                ControlToValidate="tb_FirstName" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_手機").ToString()%></label>
                        <div class="col-md-9">
                            <asp:TextBox ID="tb_Mobile" runat="server" CssClass="form-control myLogin" MaxLength="30" type="tel"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_縣市").ToString()%></label>
                        <div class="col-md-9 form-inline">
                            <asp:DropDownList ID="ddl_RegionCode" runat="server" CssClass="form-control myLogin"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfv_ddl_RegionCode" runat="server" Display="Dynamic"
                                ControlToValidate="ddl_RegionCode" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_學校").ToString()%></label>
                        <div class="col-md-9 form-inline">
                            <select id="ddl_School" class="form-control myLogin">
                                <option value="">-- Select --</option>
                            </select>
                            <asp:TextBox ID="tb_DataValue" runat="server" Style="display: none;"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_DataValue" runat="server" Display="Dynamic"
                                ControlToValidate="tb_DataValue" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>

                            <asp:Label ID="lb_ShowSchool" runat="server" CssClass="label label-danger"></asp:Label>

                            <!-- 自填其他科系 -->
                            <div style="padding-top: 10px;">
                                <label>
                                    <input id="cb_IsOther" type="checkbox" /><%=this.GetLocalResourceObject("txt_我的科系不在選單中").ToString()%></label>
                            </div>
                            <div id="divOther" style="display: none;">
                                <asp:TextBox ID="tb_SchoolName" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:TextBox ID="tb_SchoolDept" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>


                    <div class="form-group">
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_註冊日").ToString()%></label>
                        <div class="col-md-9 form-inline">
                            <div class="input-group date showDate" data-link-field="MainContent_tb_RegDate">
                                <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center myLogin" ReadOnly="true"></asp:TextBox>
                                <%-- <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>--%>
                            </div>
                            <asp:TextBox ID="tb_RegDate" runat="server" Style="display: none"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_RegDate" runat="server" Display="Dynamic"
                                ControlToValidate="tb_RegDate" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_保固日").ToString()%></label>
                        <div class="col-md-9 form-inline">
                            <div class="input-group date showDate" data-link-field="MainContent_tb_WarrantyDate">
                                <asp:TextBox ID="show_eDate" runat="server" CssClass="form-control text-center myLogin" ReadOnly="true"></asp:TextBox>
                                <%--  <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>--%>
                            </div>
                            <asp:TextBox ID="tb_WarrantyDate" runat="server" Style="display: none"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_WarrantyDate" runat="server" Display="Dynamic"
                                ControlToValidate="tb_WarrantyDate" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <!-- 驗證碼 Start -->
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 text-right"><%=this.GetLocalResourceObject("txt_驗證碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myLogin" rel="nofollow"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                                ControlToValidate="tb_VerifyCode" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-3 col-md-9">
                            <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                            <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                        </div>
                    </div>
                    <!-- 驗證碼 End -->
                    <div class="text-right">
                        <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-success" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Add','Processing...')" ValidationGroup="Add" />
                    </div>
                </div>
            </div>

            <div class="login-area col-sm-5">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_註冊表頭").ToString()%></div>
                </div>
                <%=this.GetLocalResourceObject("txt_註冊說明").ToString()%>
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

            /* 驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            /* 判斷帳號可否使用 Start */
            $('#MainContent_tb_Email').focusout(function () {
                //取得顯示區塊
                var showOK = $('#checkOK');
                var showNO = $('#checkNO');

                //判斷欄位
                var email = $(this).val();
                if (email == '') {
                    showNO.hide();
                    showOK.hide();

                } else {
                    //API 網址
                    var uri = '<%=Application["WebUrl"]%>Check/CheckAccount/' + encodeURIComponent(email) + '/';

                    // Send an AJAX request
                    $.getJSON(uri)
                        .done(function (data) {
                            //回傳資料轉成大寫
                            var getData = data.toUpperCase();

                            //判斷回傳內容
                            if (getData == 'OK') {
                                showOK.show('fast');
                                showNO.hide();
                            } else if (getData == 'NO') {
                                showOK.hide();
                                showNO.show('fast');
                            } else {
                                showNO.hide();
                                showOK.hide();
                            }
                        })

                        .fail(function (jqxhr, textStatus, error) {
                            //var err = textStatus + ", " + error;
                            //alert("Request Failed: " + err);
                            alert('Please try it later.');
                            showNO.hide();
                            showOK.hide();
                        });
                }
            });

            /* 判斷帳號可否使用 End */


            /* 是否為其他科系 Start */
            $('#cb_IsOther').click(function () {
                var showDiv = $('#divOther');
                var schoolID = $('#MainContent_tb_DataValue');

                if ($(this).prop("checked")) {
                    //清空選單的選項
                    $('select#ddl_School').val("");
                    $('#MainContent_lb_ShowSchool').text('');

                    //顯示自訂欄
                    showDiv.show();
                    //塞值給schoolID
                    schoolID.val("-1");

                } else {
                    showDiv.hide();
                    schoolID.val("");
                }
            });
            /* 是否為其他科系 End */

        });
    </script>
    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            //$('.showDate').datetimepicker({
            //    format: 'yyyy/mm/dd',   //目前欄位格式
            //    linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
            //    todayBtn: false,     //顯示today
            //    todayHighlight: false,   //將today設置高亮
            //    autoclose: true,    //選擇完畢後自動關閉
            //    startView: 4,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
            //    maxView: 4,
            //    minView: 2,
            //    forceParse: false
            //});
        });

    </script>
    <%-- DatePicker End --%>
    <script type="text/javascript">
        /* ----- Init Start ----- */
        $(function () {

            //宣告
            var menu_myCode = 'select#MainContent_ddl_RegionCode';
            var menu_School = 'select#ddl_School';

            //onchange事件 - 縣市
            $(menu_myCode).change(function () {
                var GetVal = $(menu_myCode + ' option:selected').val();
                //取得學校
                GetSchools(GetVal);
                $('#MainContent_lb_ShowSchool').text('');
            });

            //onchange事件 - 學校
            $(menu_School).change(function () {
                var GetVal = $(menu_School + ' option:selected').val();
                var GetText = $(menu_School + ' option:selected').text();
                //取得optgroup的label
                var GetLabel = $(this.options[this.selectedIndex]).closest('optgroup').prop('label').replace('= ', '').replace(' =', '');

                //填入選擇的學校
                $("#MainContent_tb_DataValue").val(GetVal);
                $('#MainContent_lb_ShowSchool').text(GetLabel + ' - ' + GetText);

                //重置其他科系
                var cb_IsOther = $('#cb_IsOther');
                cb_IsOther.prop("checked", "");
                $('#divOther').hide();
            });

            //觸發器 - 若縣市有帶預設值, 則自動觸發onchange
            //$(menu_myCode).trigger("change");

        });
        /* ----- Init End ----- */

        /* 取得縣市學校 - 連動選單 Start (使用optgroup) */
        function GetSchools(myCode) {
            //宣告 - 取得物件,學校
            var myMenu = $('select#ddl_School');
            myMenu.empty();
            myMenu.append($('<option></option>').val('').text('loading.....'));

            //判斷縣市編號是否空白
            if (myCode.length == 0) {
                SetMenuEmpty(myMenu);
                return false;
            }

            //這段必須加入, 不然會有No Transport的錯誤
            jQuery.support.cors = true;

            //API網址
            var uri = '<%=Application["API_WebUrl"] %>edu/schools/<%=Req_Code%>/?region=' + myCode;

            // Send an AJAX request
            $.getJSON(uri)
                .done(function (data) {
                    //清空選項
                    myMenu.empty();
                    //宣告
                    var optgroup;

                    //加入空白選項
                    myMenu.append($('<option></option>').val('').text('-- Select --'));
                    $.each(data, function (key, item) {
                        //判斷群組別
                        if (item.GPRank == "1") {
                            //設定OptGroup
                            optgroup = $('<optgroup>');
                            optgroup.attr('label', '= ' + item.Name + ' =');
                        }

                        //取得Option項目
                        var option = $('<option></option>');
                        option.val(item.ID);
                        option.text(item.Dept);

                        //將Option加入OptGroup
                        optgroup.append(option);

                        //將設定好的OptGroup加入Menu
                        myMenu.append(optgroup);
                    });

                    //refresh the select here
                    myMenu.multiselect('refresh');

                    //取得值
                    var getVal = $("#MainContent_tb_DataValue").val();
                    myMenu.val(getVal);

                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    //alert("無法取得資料\n\r" + err);
                });
        }

        //重設選單
        function SetMenuEmpty(menuID) {
            //清空選項
            menuID.empty();

            //加入空白選項
            menuID.append($('<option></option>').val('').text('-- Select --'));

        }
        /* 取得縣市學校 - 連動選單 End */
    </script>
</asp:Content>

