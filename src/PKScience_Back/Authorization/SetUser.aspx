<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SetUser.aspx.cs" Inherits="Authorization_SetUser" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/zTree-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a>帳號 & 權限</a></li>
                <li class="active">設定使用者權限</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-edit"></span>
                        <span>資料維護</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#data">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-inline">
                                    <asp:DropDownListGP ID="ddl_Dept" runat="server" CssClass="form-control">
                                    </asp:DropDownListGP>
                                    &nbsp;
                                    <select id="ddl_Employees" class="form-control"></select>
                                </div>

                                <asp:RequiredFieldValidator ID="rfv_ddl_Dept" runat="server" ErrorMessage="請選擇「部門」" ControlToValidate="ddl_Dept" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfv_ddl_Employees" runat="server" ErrorMessage="請選擇「人員」" ControlToValidate="tb_EmpValue" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                            </div>
                            <div class="hidden-field">
                                <asp:TextBox ID="tb_EmpValue" runat="server" Style="display: none;" ToolTip="動態產生選單會造成EventValidation 安全性問題，所以將值帶進此欄位(工號)"></asp:TextBox>
                                <asp:Literal ID="lt_Guid" runat="server" Visible="false"></asp:Literal>
                                <asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" Style="display: none;" />
                                <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                            </div>
                        </div>
                        <hr />
                        <asp:PlaceHolder ID="ph_data" runat="server" Visible="false">
                            <div class="row">
                                <div class="col-sm-6">
                                    <div style="padding-bottom: 5px;">
                                        <input type="button" id="showAll" class="btn btn-info" value="展開" />
                                        <input type="button" id="hideAll" class="btn btn-warning" value="折疊" />
                                        <input type="button" id="copyGP" class="btn btn-success" value="複製群組權限" />
                                    </div>
                                    <div id="copyGPmessage" class="alert alert-danger hide">群組權限已複製，記得按下「設定權限」才會生效!</div>
                                    <ul id="myTree" class="ztree">
                                    </ul>

                                    <asp:TextBox ID="tb_IDvalues" runat="server" Style="display: none"></asp:TextBox>
                                </div>
                                <div class="col-sm-6">
                                    <div>
                                        <a class="btn btn-success">
                                            <i class="fa fa-users fa-lg"></i>&nbsp;群組權限參考
                                        </a>
                                    </div>
                                    <ul id="myTree_GP" class="ztree">
                                    </ul>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <!-- Content End -->
                    </div>
                </div>
            </div>
        </div>
        <!-- buttons -->
        <div class="col-sm-3 col-md-2">
            <asp:PlaceHolder ID="ph_btns" runat="server" Visible="false">
                <div class="FormBtn-fixPos">
                    <div class="metro-nav metro-fix-view">
                        <div class="metro-nav-block nav-block-blue">
                            <asp:LinkButton ID="lbtn_Save" runat="server" CssClass="text-center" ValidationGroup="Add" OnClientClick="getCbValue('myTree', 'MainContent_tb_IDvalues');blockBox1('Add','資料處理中...')" OnClick="lbtn_Save_Click">
                            <i class="glyphicon glyphicon-floppy-save"></i>
                            <div class="status">設定權限</div>
                            </asp:LinkButton>
                        </div>
                        <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Remove" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定移除權限?')" OnClick="lbtn_Remove_Click">
                            <i class="glyphicon glyphicon-floppy-remove"></i>
                            <div class="status">移除權限</div>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/zTree-script") %>
    <%-- zTree Start --%>
    <script>
        //zTree 設定
        var setting = {
            view: {
                dblClickExpand: false
            },
            callback: {
                onClick: MMonClick
            },
            check: {
                enable: true
            },
            data: {
                simpleData: {
                    enable: true
                }
            }
        };

        //Event - onClick
        function MMonClick(e, treeId, treeNode) {
            var zTree = $.fn.zTree.getZTreeObj("myTree");
            zTree.expandNode(treeNode);
        }

        //宣告節點
        var zNodes;

        //取得資料
        function getAuthList() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Ajax_Data/Json_GetAuthList.aspx",
                data: {
                    DataType: 'User',
                    Guid: '<%=Param_Guid %>'
                },
                error: function () {
                    alert('樹狀選單載入失敗!');
                },
                success: function (data) {
                    zNodes = data;
                }
            });
            //載入zTree
            $.fn.zTree.init($("#myTree"), setting, zNodes);
        }

        // 所有節點的收合(true = 展開, false = 折疊)
        function expandAll(objbool) {
            var treeObj = $.fn.zTree.getZTreeObj("myTree");
            treeObj.expandAll(objbool);
        }

        /* 取值(zTree名稱, 要放值的欄位名) */
        function getCbValue(eleName, valName) {

            var treeObj = $.fn.zTree.getZTreeObj(eleName);
            var nodes = treeObj.getCheckedNodes(true);
            var ids = "";
            for (var i = 0; i < nodes.length; i++) {
                //加入分隔符號("||")
                if (ids != "") {
                    ids += "||"
                }
                //取得id值
                ids += nodes[i].id;
            }
            //輸出組合完畢的字串值
            document.getElementById(valName).value = ids;
            return true;
        }

        //Load
        $(document).ready(function () {
            <%if (!string.IsNullOrEmpty(Param_Guid))
              {%>
            getAuthList();
            <%}%>

            //顯示所有節點
            $('#showAll').click(function () {
                expandAll(true);
            });

            //隱藏所有節點
            $('#hideAll').click(function () {
                expandAll(false);
            });

            //複製群組權限按鈕
            $("#copyGP").click(function () {
                //開始複製群組權限
                $.ajax({
                    async: false,
                    cache: false,
                    type: 'POST',
                    dataType: "json",
                    url: "<%=Application["WebUrl"]%>Ajax_Data/Json_GetAuthList.aspx",
                data: {
                    DataType: 'COPY_GROUP',
                    Guid: '<%=Param_Guid %>'
                },
                     error: function () {
                         alert('樹狀選單載入失敗!');
                     },
                     success: function (data) {
                         zNodes = data;

                         //顯示訊息
                         $('#copyGPmessage').removeClass("hide").addClass("show");

                         //載入zTree
                         $.fn.zTree.init($("#myTree"), setting, zNodes);
                     }
                });

            });
        });


    </script>
    <%-- zTree End --%>
    <%-- zTree(群組權限) Start --%>
    <script>
        //宣告節點
        var zNodes_GP;

        //取得資料
        function getAuthList_GP() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Ajax_Data/Json_GetAuthList.aspx",
                data: {
                    DataType: 'User_IN_Group',
                    Guid: '<%=Param_Guid %>'
                },
                error: function () {
                    alert('樹狀選單載入失敗!');
                },
                success: function (data) {
                    zNodes_GP = data;
                }
            });
            //載入zTree
            $.fn.zTree.init($("#myTree_GP"), setting, zNodes_GP);
        }


        //Load
        $(document).ready(function () {
            <%if (!string.IsNullOrEmpty(Param_Guid))
              {%>
            getAuthList_GP();
            <%}%>

        });

    </script>
    <%-- zTree(群組權限) End --%>
    <%-- 連動式選單 Start --%>
    <script type="text/javascript">
        $(function () {
            //部門選單變動時觸發事件
            $('select#MainContent_ddl_Dept').change(function () {
                var GetVal = $('#MainContent_ddl_Dept option:selected').val();
                //取得部門人員
                GetEmployees(GetVal);

            });

            //人員選單變動時觸發事件
            $('select#ddl_Employees').change(function () {
                var GetVal = $('#ddl_Employees option:selected').val();
                //填入選擇的人員
                $("#MainContent_tb_EmpValue").val(GetVal);

                //觸發Click Search
                blockBox2_NoMsg();
                $('#MainContent_btn_Search').trigger("click");
            });

            //若部門有帶預設值, 則自動觸發事件
            $('select#MainContent_ddl_Dept').trigger("change");
        });

        /* 取得部門人員 - 連動選單 Start */
        function GetEmployees(DeptId) {
            var flag = $("#MainContent_hf_flag").val();

            //宣告 - 取得物件,人員
            var myMenu = $('select#ddl_Employees');
            myMenu.empty();
            myMenu.append($('<option></option>').val('').text('loading.....'));

            //判斷部門編號是否空白
            if (DeptId.length == 0) {
                SetEmployeeMenuEmpty(myMenu);
                return false;
            }

            //這段必須加入, 不然會有No Transport的錯誤
            jQuery.support.cors = true;
            //API網址
            var uri = '<%=ApiUrl%>api/employees/?deptid=' + DeptId;

            // Send an AJAX request
            $.getJSON(uri)
                .done(function (data) {
                    //清空選項
                    myMenu.empty();

                    //加入選項
                    myMenu.append($('<option></option>').val('').text('-- 請選擇 --'));
                    $.each(data, function (key, item) {
                        myMenu.append($('<option></option>').val(item.UserId).text('(' + item.UserId + ') ' + item.UserName))
                    });

                    //判斷目前為新增或修改
                    if (flag.toUpperCase() == "ADD") {
                        //設定預設值

                    } else {
                        //若為修改, 則帶入資料
                        var getVal = $("#MainContent_tb_EmpValue").val();
                        myMenu.val(getVal);
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    alert("無法取得人員選單\n\r" + err);
                });

        }

        //重設選單
        function SetEmployeeMenuEmpty(menuID) {
            //清空選項
            menuID.empty();

            //加入選項
            menuID.append($('<option></option>').val('').text('-- 請先選擇部門 --'));
        }
        /* 取得部門人員 - 連動選單 End */
    </script>
    <%-- 連動式選單 End --%>
</asp:Content>

