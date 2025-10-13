<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_Crime.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_Crime" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ClassCode/ctl_CPP_CrimeClassCodeLookup.ascx" TagPrefix="uc1" TagName="ctl_CPP_CrimeClassCodeLookup" %>



<style>
    .qs_basic_info_labels_cell {
        width: 160px;
    }

    .qs_Main_Sections {
        margin-top: 15px;
        width: 100%;
    }

    .qs_Sub_Sections {
        margin-left: 0px;
        width: 100%;
    }

    .qa_table_shades {
        width: 100%;
        border-collapse: collapse;
    }

        .qa_table_shades tr:nth-child(even) {
            background-color: #dce6f1;
        }

    .qa_table_base {
        width: 100%;
        border-collapse: collapse;
    }

    .qs_section_grid_headers {
        /*border: 1px solid black;*/
        text-align: left;
        font-weight: bold;
        /*background-color: #f4cb8f;*/
    }

    .qs_section_headers {
        font-weight: bold;
        font-size: 13px;
    }

    .qs_section_header_double_hieght {
        min-height: 40px;
    }

    .qs_Grid_cell_premium {
        width: 70px;
        max-width: 70px;
    }

    .qs_Grid_Total_Row {
        border-top: 2px solid black;
    }

    .qs_grid_5_columns td {
        width: 20%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_grid_4_columns td {
        width: 25%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_grid_3_columns td {
        width: 15%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

        .qs_grid_3_columns td:first-child {
            width: 70%;
            /*min-width: 25%;
        max-width: 25%;*/
        }

    .qs_grid_5_columns td:first-child {
        width: 12%;
        /*min-width: 25%;
        max-width: 25%;*/
    }

    .qs_grid_5_columns td:nth-child(2) {
        width: 185px;
        /*min-width: 25%;
        max-width: 25%;*/
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        display: inline-block;
    }

    .qs_grid_5_columns td:nth-child(5) {
        width: 12%;
        /*min-width: 25%;
        max-width: 25%;*/
    }

    .qs_indent td {
        padding-left: 1em;
    }

    #divWCPQuoteSummary tr {
        width: 100%;
        line-height: 1.5;
    }

    .qs_grid_4_columns .ThreeHeaders td:nth-child(3), .qs_grid_4_columns.3Title td:nth-child(4) {
        padding-right: 15%;
    }

    .qs_rightJustify {
        text-align: right;
    }

    .qs_resetWidth {
        width: 40%;
    }

    .qs_padRight {
        padding-right: 1em;
    }

    .form4Em {
        width: 4em;
    }

    .form6Em {
        width: 6em;
    }

    .form8Em {
        width: 8em;
    }

    .form10Em {
        width: 10em;
    }

    .form13Em {
        width: 13em;
    }

    .form15Em {
        width: 15em;
    }

    .PopupPanel {
        border: solid 1px black;
        position: fixed;
        left: 50%;
        top: 50%;
        background-color: #ffffff;
        z-index: 100;
        height: 200px;
        margin-top: -200px;
        width: 400px;
        margin-left: -300px;
    }
</style>

<script>

    function preventBackspace(e) {
        var evt = e || window.event;
        if (evt) {
            var keyCode = evt.charCode || evt.keyCode;
            if (keyCode === 8 || keyCode == 46 || keyCode == 45) {
                if (evt.preventDefault) {
                    evt.preventDefault();
                } else {
                    evt.returnValue = false;
                }
            }
        }
    }

    function ClearForm() {
        return true
    }

    function ValidateForm() {
        return true
    }

    function ShowOrHideDetailPanel(element, ShowOrHide) {
        var DetailInfo = $(element).closest('.ItemGroup').find('.chkDetail').first();

        if (ShowOrHide == "SHOW") {
            $(DetailInfo).show();
        }
        else {
            $(DetailInfo).hide();
        }
    }

    function IM_ClickClearButton(confirmItem) {
        switch (confirmItem) {
            case "EmployeeTheft":
                $("#" + "<%=chkTheft.Clientid%>").attr('checked', false);
                $("#" + "<%=txtEmployee_Limit.Clientid%>").val('');
                $("#" + "<%=ddEmployee_Deductible.Clientid%>").val("8");
                $("#" + "<%=txtRateAbleEmployees.Clientid%>").val('');
                $("#" + "<%=txtEmployee_ERISA.Clientid%>").val('');
                $("#" + "<%=txtEmployee_AdditionalPremises.Clientid%>").val('');
                break;
            case "InsidePremises":
                $("#" + "<%=chkInside.Clientid%>").attr('checked', false);
                $("#" + "<%=txtInside_Limit.Clientid%>").val('');
                $("#" + "<%=ddInside_Deductible.Clientid%>").val("8");
                $("#" + "<%=txtInside_PremisesCount.Clientid%>").val('');
                break;
            case "OutsidePremises":
                $("#" + "<%=chkOutside.Clientid%>").attr('checked', false);
                $("#" + "<%=txtOutside_Limit.Clientid%>").val('');
                $("#" + "<%=ddOutside_Deductible.Clientid%>").val("8");
                $("#" + "<%=txtOutside_PremisesCount.Clientid%>").val('');
                break;
            case "ForgeryAlteration":
                $("#<%=chkForgeryAlteration.ClientID%>").attr('checked', false);
                $("#<%=txtForgeryAlteration_Limit.Clientid%>").val('');
                $("#<%=ddForgeryAlteration_Deductible.Clientid%>").val("8");
                $("#<%=txtForgeryAlteration_RatableEmployees.Clientid%>").val('');
                $("#<%=txtForgeryAlteration_AdditionalPremises.Clientid%>").val('');
                break;
            case "ComputerFraud":
                $("#<%=chkComputerFraud.ClientID%>").attr('checked', false);
                $("#<%=txtComputerFraud_Limit.Clientid%>").val('');
                $("#<%=ddComputerFraud_Deductible.Clientid%>").val("8");
                $("#<%=txtComputerFraud_RatableEmployees.Clientid%>").val('');
                $("#<%=txtComputerFraud_AdditionalPremises.Clientid%>").val('');
                break;
            case "FundsTransferFraud":
                $("#<%=chkFundsTransferFraud.ClientID%>").attr('checked', false);
                $("#<%=txtFundsTransferFraud_Limit.Clientid%>").val('');
                $("#<%=ddFundsTransferFraud_Deductible.Clientid%>").val("8");
                $("#<%=txtFundsTransferFraud_RatableEmployees.Clientid%>").val('');
                $("#<%=txtFundsTransferFraud_AdditionalPremises.Clientid%>").val('');
                break;
            default:
                break;
        }
    }

    function ClearCrimeCoverages() {
        ///TODO: refactor - use jquery + css classes

        IM_ClickClearButton("EmployeeTheft");
        IM_ClickClearButton("InsidePremises");
        IM_ClickClearButton("OutsidePremises");
        IM_ClickClearButton("ForgeryAlteration");
        IM_ClickClearButton("ComputerFraud");
        IM_ClickClearButton("FundsTransferFraud");

        ShowOrHideDetailPanel($("#" + "<%=chkTheft.Clientid%>"), 'HIDE');
        ShowOrHideDetailPanel($("#" + "<%=chkInside.Clientid%>"), 'HIDE');
        ShowOrHideDetailPanel($("#" + "<%=chkOutside.Clientid%>"), 'HIDE');
        ShowOrHideDetailPanel($("#<%=chkForgeryAlteration.Clientid%>"), 'HIDE');
        ShowOrHideDetailPanel($("#<%=chkComputerFraud.Clientid%>"), 'HIDE');
        ShowOrHideDetailPanel($("#<%=chkFundsTransferFraud.Clientid%>"), 'HIDE');
        return false
    }

    function ClearClassCode() {
        $("#" + "<%=txtClassCode.Clientid%>").val('');
        $("#" + "<%=txtDescription.Clientid%>").val('');
        return false
    }

    // Initial Page load.
    $(function () {
        $('.chkOption input[type=checkbox]').each(function () {
            if (this.checked) {
                ShowOrHideDetailPanel($(this), 'SHOW')
            }

        });


    });

    // Live functionality
    $(function () {
        $('.chkOption input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                ShowOrHideDetailPanel($(this), 'SHOW')
                return false;
            }
            else {
                // Check the caller
                var confirmString;
                var confirmItem = $(this).closest('div').attr("id");
                confirmItem = confirmItem.substring(confirmItem.lastIndexOf("_") + 1);
                switch (confirmItem) {
                    case "EmployeeTheft":
                        confirmString = "Are you sure you want to delete this Employee Theft coverage?";
                        break;
                    case "InsidePremises":
                        confirmString = "Are you sure you want to delete this Inside Premises coverage?";
                        break;
                    case "OutsidePremises":
                        confirmString = "Are you sure you want to delete this Outside the Premises coverage?";
                        break;
                    case "ForgeryAlteration":
                        confirmString = "Are you sure you want to delete this Forgery or Alteration coverage?";
                        break;
                    case "ComputerFraud":
                        confirmString = "Are you sure you want to delete this Computer Fraud coverage?";
                        break;
                    case "FundsTransferFraud":
                        confirmString = "Are you sure you want to delete this Funds Transfer Fraud coverage?";
                        break;
                    default:
                        confirmString = "Are you sure you want to delete this coverage?";
                        break;
                }
                //clear Options
                if (confirm(confirmString)) {
                    ShowOrHideDetailPanel($(this), 'HIDE');
                    IM_ClickClearButton(confirmItem);
                }
                else {
                    this.checked = true;
                    return false;
                }

            }
        });

    });

    function ClassCodeNoCoverageCheck() {
        if ($("#" + "<%=txtClassCode.Clientid%>").val() != '' &&
            $("#" + "<%=chkTheft.Clientid%>").prop('checked') == false &&
            $("#" + "<%=chkInside.Clientid%>").prop('checked') == false &&
            $("#" + "<%=chkOutside.Clientid%>").prop('checked') == false &&
            $("#<%=chkForgeryAlteration.Clientid%>").prop('checked') == false &&
            $("#<%=chkComputerFraud.Clientid%>").prop('checked') == false &&
            $("#<%=chkFundsTransferFraud.Clientid%>").prop('checked') == false) {
            if (confirm('You have selected a Crime class code but have not selected any coverages. If you continue the crime class code will be removed.')) { ClearClassCode(); DisableFormOnSaveRemoves(); return true; } else { return false; }
        }
    };

    function ValidateEmployeeCountCalcForm() {
        var txtOfficer = "<%=txtEmployee_OfficerCount.Clientid%>";
        var txtHandle = "<%=txtEmployee_HandleMoneyCount.Clientid%>";
        var txtRemaining = "<%=txtEmployee_RemainingEmployees.Clientid%>";
        var hidTarget = "#<%=hidTarget.ClientID%>";
        var targetPrefix = '';

        switch ($(hidTarget).val()) {
            case 'employeeTheft':
                targetPrefix = 'Employee Theft';
                break;
            case 'forgeryAlteration':
                targetPrefix = 'Forgery Or Alteration';
                break;
            case 'computerFraud':
                targetPrefix = "Computer Fraud";
                break;
            case 'fundsTransferFraud':
                targetPrefix = "Funds transfer Fraud";
                break;
        }
        $("#" + txtOfficer).css('background-color', 'white');
        $("#" + txtHandle).css('background-color', 'white');
        $("#" + txtRemaining).css('background-color', 'white');



        if (($("#" + txtOfficer).val() == "0" | $("#" + txtOfficer).val().indexOf(".") > -1 | $("#" + txtOfficer).val().indexOf("-") > -1) &
            ($("#" + txtHandle).val() == "0" | $("#" + txtHandle).val().indexOf(".") > -1 | $("#" + txtHandle).val().indexOf("-") > -1) &
            ($("#" + txtRemaining).val() == "0" | $("#" + txtRemaining).val().indexOf(".") > -1 | $("#" + txtRemaining).val().indexOf("-") > -1)) {
            $("#" + txtOfficer).css('background-color', 'red');
            $("#" + txtHandle).css('background-color', 'red');
            $("#" + txtRemaining).css('background-color', 'red');
            alert(targetPrefix + ' - Number of employees who handle money, Number of Officers or Number of remaining employees must have a whole number value greater than 0.');
            return false;
        }
        else {

            var hasError = false;
            if ($("#" + txtOfficer).val() == "") {
                $("#" + txtOfficer).css('background-color', 'red');
                alert(targetPrefix + ' - Missing Number of Officers');
                hasError = true;
            }
            else {
                if ($("#" + txtOfficer).val().indexOf(".") > -1 | $("#" + txtOfficer).val().indexOf("-") > -1) {
                    $("#" + txtOfficer).css('background-color', 'red');
                    alert(targetPrefix + ' - Number of Officers requires a whole number value.');
                    hasError = true;
                }
            }

            if ($("#" + txtHandle).val() == "") {
                $("#" + txtHandle).css('background-color', 'red');
                alert(targetPrefix + ' - Missing number of employees who handle money');
                hasError = true;
            }
            else {
                if ($("#" + txtHandle).val().indexOf(".") > -1 | $("#" + txtHandle).val().indexOf("-") > -1) {
                    $("#" + txtHandle).css('background-color', 'red');
                    alert(targetPrefix + ' - Number of employees who handle money requires a whole number value.');
                    hasError = true;
                }
            }


            if ($("#" + txtRemaining).val() == "") {
                $("#" + txtRemaining).css('background-color', 'red');
                alert(targetPrefix + +' - Missing number of remaining employees');
                hasError = true;
            }
            else {
                if ($("#" + txtRemaining).val().indexOf(".") > -1 | $("#" + txtRemaining).val().indexOf("-") > -1) {
                    $("#" + txtRemaining).css('background-color', 'red');
                    alert(targetPrefix + ' - Number of remaining employees requires a whole number value.');
                    hasError = true;
                }
            }




            if (hasError) {
                return false;
            }
            else {
                $("#divCalcEmployeeCount").hide();
                return true;
            }

        }
    }


</script>

<div id="CrimeDiv" class="CrimeDiv">
    <h3>
        <asp:Label ID="lblTitle" runat="server" Text="Crime"></asp:Label>
        <span style="float: right;">
            <%--<asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Data" OnClientClick="return ClearForm()">Clear</asp:LinkButton>--%>
            <asp:LinkButton ID="lnkSave" runat="server" CssClass="RemovePanelLink" ToolTip="Save Crime Data" OnClientClick="return ClassCodeNoCoverageCheck();">Save</asp:LinkButton>
        </span>
    </h3>
    <div style="min-height: 600px">
        <%--Class Code--%>
        <div runat="server" id="divMainClassCode" class="standardSubSection">
            <h3>Crime Class Code
            <span style="float: right">
                <asp:LinkButton ID="btnClassCodeClear" runat="server" CssClass="RemovePanelLink" ToolTip="Clear Data" OnClientClick="return ClearClassCode()">Clear</asp:LinkButton>
                <asp:LinkButton ID="btnClassCodeSave" runat="server" CssClass="RemovePanelLink" ToolTip="Save Inland Marine Data" OnClientClick="return ClassCodeNoCoverageCheck();">Save</asp:LinkButton>
            </span>
            </h3>

            <div id="divClassCode" runat="server">
                <table class="qa_table_base">
                    <%-- Data Rows --%>
                    <tr class="qs_basic_info_labels_cell" runat="server" visible="true" id="Tr1">
                        <td style="text-align:right">Class Code</td>
                        <td class="form15Em">
                        <asp:TextBox ID="txtClassCode" onkeyup='' runat="server" Text='' class="form15Em" onKeyDown="preventBackspace();" BackColor="#cccccc" onkeypress='return false'></asp:TextBox>
                            </td>
                            <td style="text-align:left">
                            <asp:Button ID="btnClassCodeLookup" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="" ToolTip="Class Code Lookup" Text="Lookup" />
                        </td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell" runat="server" visible="true" id="Tr2">
                        <td style="text-align:right">Description</td>
                        <td>
                        <asp:TextBox ID="txtDescription" onkeyup='' runat="server" Text='' class="form15Em" onKeyDown="preventBackspace();" BackColor="#cccccc" onkeypress='return false'></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <%--Crime Coverages--%>
        <div runat="server" id="divMainCrimeCoverages" class="standardSubSection">
            <h3>Crime Coverages
            <span style="float: right">
                <asp:LinkButton ID="btnCrimeCoveragesClear" runat="server" CssClass="RemovePanelLink" ToolTip="Clear Data" OnClientClick="return ClearCrimeCoverages()">Clear</asp:LinkButton>
                <asp:LinkButton ID="btnCrimeCoveragesSave" runat="server" CssClass="RemovePanelLink" ToolTip="Save Inland Marine Data" OnClientClick="return ClassCodeNoCoverageCheck();">Save</asp:LinkButton>
            </span>
            </h3>
            <div id="div4" runat="server" style="min-height: 400px">
                <table class="qa_table_base">
                    <%-- Data Rows --%>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="theftRow">
                        <td style="width: 70%;">
                            <div class="ItemGroup" id="EmployeeTheft">
                                <asp:CheckBox ID="chkTheft" runat="server" class="chkOption" />
                                Employee Theft
                        <br />
                                <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                                    <div runat="server" id="divItemDetails" style="padding-left: 20px; padding-top: 10px;">
                                        <table class="basictbl">
                                            <tr>
                                                <td class="requiredText form10Em">
                                                    *Limit
                                                    <br />
                                                    <asp:TextBox ID="txtEmployee_Limit" MaxLength="15" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="requiredText form10Em" style="text-align:center">
                                                    *Deductible
                                                    <br />
                                                    <asp:DropDownList ID="ddEmployee_Deductible" runat="server">
                                                    </asp:DropDownList>

                                                </td>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Ratable Employees
                                                    <br />
                                                    <asp:TextBox ID="txtRateAbleEmployees" Enabled="false" ToolTip="Set with the link to the right." runat="server"></asp:TextBox>
                                                    <a href="#" id="openRateableModal" class="informationalText" style="padding-left:10px;color: blue;" onclick='$("#<%=hidTarget.ClientID %>").val("employeeTheft");$("#divCalcEmployeeCount").toggle();'>Set</a>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Additional Premises
                                                    <br />
                                                    <asp:TextBox ID="txtEmployee_AdditionalPremises" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="">
                                                    ERISA Plan Name
                                                    <br />
                                                    <asp:TextBox ID="txtEmployee_ERISA" MaxLength="250" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="insideRow">
                        <td style="width: 70%;">
                            <div class="ItemGroup" id="InsidePremises">
                                <asp:CheckBox ID="chkInside" runat="server" class="chkOption" />
                                Inside Premises - Theft of Money and Securities
                        <br />
                                <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                                    <div runat="server" id="div1" style="padding-left: 20px; padding-top: 10px;">
                                        <table class="basictbl">
                                            <tr>
                                                <td class="requiredText form10Em">
                                                    *Limit
                                                    <br />
                                                    <asp:TextBox ID="txtInside_Limit" MaxLength="15" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="requiredText  form10Em" style="text-align:center">
                                                    *Deductible
                                                    <br />
                                                    <asp:DropDownList ID="ddInside_Deductible" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="requiredText form15Em">
                                                    *Number of Premises
                                                    <br />
                                                    <asp:TextBox ID="txtInside_PremisesCount" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="outsideRow">
                        <td style="width: 70%;">
                            <div class="ItemGroup" id="OutsidePremises">
                                <asp:CheckBox ID="chkOutside" runat="server" class="chkOption" />
                                Outside the Premises
                        <br />
                                <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                                    <div runat="server" id="div2" style="padding-left: 20px; padding-top: 10px;">
                                        <table class="basictbl">
                                            <tr>
                                                <td class="requiredText form10Em">
                                                    *Limit
                                                    <br />
                                                    <asp:TextBox ID="txtOutside_Limit" MaxLength="15" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="requiredText form10Em" style="text-align:center">
                                                    *Deductible
                                                    <br />
                                                    <asp:DropDownList ID="ddOutside_Deductible" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="requiredText form15Em">
                                                    *Number of Premises
                                                    <br />
                                                    <asp:TextBox ID="txtOutside_PremisesCount" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="Tr3">
                        <td style="width: 70%;">
                            <div class="ItemGroup" id="ForgeryAlteration">
                                <asp:CheckBox ID="chkForgeryAlteration" runat="server" class="chkOption" />
                                Forgery Or Alteration
                        <br />
                                <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                                    <div runat="server" id="div3" style="padding-left: 20px; padding-top: 10px;">
                                        <table class="basictbl">
                                            <tr>
                                                <td class="requiredText form10Em">
                                                    *Limit
                                                    <br />
                                                    <asp:TextBox ID="txtForgeryAlteration_Limit" MaxLength="15" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="requiredText form10Em" style="text-align:center">
                                                    *Deductible
                                                    <br />
                                                    <asp:DropDownList ID="ddForgeryAlteration_Deductible" runat="server">
                                                    </asp:DropDownList>

                                                </td>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Ratable Employees
                                                    <br />
                                                    <asp:TextBox ID="txtForgeryAlteration_RatableEmployees" Enabled="false" ToolTip="Set with the link to the right." runat="server"></asp:TextBox>
                                                    <a href="#" id="ForgeryAlteration_openRateableModal" class="informationalText" style="padding-left:10px;color: blue;" onclick='$("#<%=hidTarget.ClientID %>").val("forgeryAlteration");$("#divCalcEmployeeCount").toggle();'>Set</a>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Additional Premises
                                                    <br />
                                                    <asp:TextBox ID="txtForgeryAlteration_AdditionalPremises" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="Tr4">
                        <td style="width: 70%;">
                            <div class="ItemGroup" id="ComputerFraud">
                                <asp:CheckBox ID="chkComputerFraud" runat="server" class="chkOption" />
                                Computer Fraud
                        <br />
                                <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                                    <div runat="server" id="div5" style="padding-left: 20px; padding-top: 10px;">
                                        <table class="basictbl">
                                            <tr>
                                                <td class="requiredText form10Em">
                                                    *Limit
                                                    <br />
                                                    <asp:TextBox ID="txtComputerFraud_Limit" MaxLength="15" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="requiredText form10Em" style="text-align:center">
                                                    *Deductible
                                                    <br />
                                                    <asp:DropDownList ID="ddComputerFraud_Deductible" runat="server">
                                                    </asp:DropDownList>

                                                </td>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Ratable Employees
                                                    <br />
                                                    <asp:TextBox ID="txtComputerFraud_RatableEmployees" Enabled="false" ToolTip="Set with the link to the right." runat="server"></asp:TextBox>
                                                    <a href="#" id="ComputerFraud_openRateableModal" class="informationalText" style="padding-left:10px;color: blue;" onclick='$("#<%=hidTarget.ClientID %>").val("computerFraud");$("#divCalcEmployeeCount").toggle();'>Set</a>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Additional Premises
                                                    <br />
                                                    <asp:TextBox ID="txtComputerFraud_AdditionalPremises" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="Tr5">
                        <td style="width: 70%;">
                            <div class="ItemGroup" id="FundsTransferFraud">
                                <asp:CheckBox ID="chkFundsTransferFraud" runat="server" class="chkOption" />
                                Funds Transfer Fraud
                        <br />
                                <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                                    <div runat="server" id="div6" style="padding-left: 20px; padding-top: 10px;">
                                        <table class="basictbl">
                                            <tr>
                                                <td class="requiredText form10Em">
                                                    *Limit
                                                    <br />
                                                    <asp:TextBox ID="txtFundsTransferFraud_Limit" MaxLength="15" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                                <td class="requiredText form10Em" style="text-align:center">
                                                    *Deductible
                                                    <br />
                                                    <asp:DropDownList ID="ddFundsTransferFraud_Deductible" runat="server">
                                                    </asp:DropDownList>

                                                </td>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Ratable Employees
                                                    <br />
                                                    <asp:TextBox ID="txtFundsTransferFraud_RatableEmployees" Enabled="false" ToolTip="Set with the link to the right." runat="server"></asp:TextBox>
                                                    <a href="#" id="FundsTransferFraud_openRateableModal" class="informationalText" style="padding-left:10px;color: blue;" onclick='$("#<%=hidTarget.ClientID %>").val("fundsTransferFraud");$("#divCalcEmployeeCount").toggle();'>Set</a>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="requiredText form15Em" colspan="2">
                                                    *Number of Additional Premises
                                                    <br />
                                                    <asp:TextBox ID="txtFundsTransferFraud_AdditionalPremises" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td  colspan="4" style="padding-right: 2em; text-align: center;"><br />
                            <span class="informationalText">Other Optional Coverages are available. Please contact your Commercial Underwriter for assistance and approval.</span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

    </div>
</div>

<div id="divCalcEmployeeCount" style="display: none;" class="PopupPanel">
    <asp:HiddenField ID="hidTarget" runat="server" />
    <div id="calcEmployee" class="calcEmployee" runat="server">
        <h3>
            <asp:Label ID="popTitle" runat="server" Text="Number of Rated Employees Calculator"></asp:Label>
        </h3>
        <div style="min-height: 140px">
            <table>
                <tr>
                    <td class="tblcellLeftSide formStandardLeftWidthForIndented requiredText">*Number of Officers</td>
                    <td class="tblcellRightSide">
                        <asp:TextBox ID="txtEmployee_OfficerCount" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="tblcellLeftSide  formStandardLeftWidthForIndented requiredText">*Number of employees who handle money</td>
                    <td class="tblcellRightSide">
                        <asp:TextBox ID="txtEmployee_HandleMoneyCount" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="tblcellLeftSide  formStandardLeftWidthForIndented requiredText">*Number of remaining employees</td>
                    <td class="tblcellRightSide">
                        <asp:TextBox ID="txtEmployee_RemainingEmployees" MaxLength="5" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                    </td>

                </tr>
                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:Button CssClass="StandardButton" OnClientClick="return ValidateEmployeeCountCalcForm();" ID="btnSetEmployeeRateable" runat="server"
                            Text="Calculate Ratable Employees" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>

<uc1:ctl_CPP_CrimeClassCodeLookup runat="server" ID="ctl_CPP_CrimeClassCodeLookup" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="return ClassCodeNoCoverageCheck();" CssClass="StandardSaveButton" ToolTip="Save Crime" Text="Save Crime" />
    <asp:Button ID="btnRate" OnClientClick="return ClassCodeNoCoverageCheck();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" style="margin-top: 5px;" />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
    <asp:Button ID="btnViewBillingInfo" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Billing Information" />
</div>

<!-- The hidden fields below hold the selected values from the class code lookup control --> 
    <!-- Note that the ClassCode and Description are stored in text fields -->
    <asp:HiddenField ID="hdnDIA_Id" runat="server" />
    <asp:HiddenField ID="hdnPMAID" runat="server" />
    <asp:HiddenField ID="hdnGroupRate" runat="server" />
    <asp:HiddenField ID="hdnClassLimit" runat="server" />
