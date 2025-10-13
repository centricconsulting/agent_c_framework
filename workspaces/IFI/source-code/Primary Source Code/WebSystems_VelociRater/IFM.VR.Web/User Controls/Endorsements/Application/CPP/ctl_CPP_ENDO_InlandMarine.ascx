<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_ENDO_InlandMarine.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_ENDO_InlandMarine" %>
<%@ Register Src="~/User Controls/Endorsements/CommonControls/ctl_Endo_VehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_VehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/ctl_CPP_ENDO_App_InlandMarine.ascx" TagPrefix="uc1" TagName="ctl_CPP_ENDO_App_InlandMarine" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>




<%--<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_ContractorIMEndorsement.ascx" TagPrefix="uc1" TagName="cov_ContractorIMEndorsement" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_BuildersRisk.ascx" TagPrefix="uc1" TagName="cov_BuildersRisk" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Computer.ascx" TagPrefix="uc1" TagName="cov_Computer" %>--%>
<%--<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Contractor.ascx" TagPrefix="uc1" TagName="cov_Contractor" %>--%>
<%--<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_FineArtsFloater.ascx" TagPrefix="uc1" TagName="cov_FineArtsFloater" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_InstallationFloater.ascx" TagPrefix="uc1" TagName="cov_InstallationFloater" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_MotorTruckCargo.ascx" TagPrefix="uc1" TagName="cov_MotorTruckCargo" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_OwnersCargo.ascx" TagPrefix="uc1" TagName="cov_OwnersCargo" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Transportation.ascx" TagPrefix="uc1" TagName="cov_Transportation" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_ScheduledPropertyFloater.ascx" TagPrefix="uc1" TagName="cov_ScheduledPropertyFloater" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Signs.ascx" TagPrefix="uc1" TagName="cov_Signs" %>--%>

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

</style>

<script>

    function ClearForm() {
        return true
    }

    function ValidateForm() {
        return true
    }

    function doClearItem(selection) {
        element = $(selection)
        if (element.prop("checked")) {
            ShowOrHideDetailPanel(element, 'SHOW');
            element.prop("checked", true);
        }
        else {
            if (confirm('Are you sure you want to delete this line item?')) {
                ShowOrHideDetailPanel(element, 'HIDE');
                element.prop("checked", false);
                return true;
            }
            else {
                element.prop("checked", true);
            }

        }
        return false;
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

    function IM_ClickClearButton(element) {
        var button = $(element).closest('div').find('.hiddenclearbutton').first();
        DisableMainFormOnSaveRemoves()
        button.click();
    }

    // Initial Page load.
    $(function () {
        $('.chkOption input[type=checkbox]').each(function () {
            if (this.checked) {
                ShowOrHideDetailPanel($(this), 'SHOW')
            }
            else {
                ShowOrHideDetailPanel($(this), 'HIDE')
            }

        });

        $('.chkOption2 input[type=checkbox]').each(function () {
            if (this.checked) {
                ShowOrHideDetailPanel($(this), 'SHOW')
            }
            else {
                ShowOrHideDetailPanel($(this), 'HIDE')
            }

        });

        $('.chkScheduledTools input[type=checkbox]').each(function () {
            if (this.checked) {
                $('.coverageMessage').hide()
            }
        });

        $('.motortruckoption input[type=checkbox]').each(function (event) {
            if (this.checked) {
                $(this).removeAttr("disabled")
                $('.ownerscargooption input[type=checkbox]').attr("disabled", true)
                $('.transportationoption input[type=checkbox]').attr("disabled", true)
            }
        });

        $('.ownerscargooption input[type=checkbox]').each(function (event) {
            if (this.checked) {
                $(this).removeAttr("disabled")
                $('.motortruckoption input[type=checkbox]').attr("disabled", true)
                $('.transportationoption input[type=checkbox]').attr("disabled", true)
            }
        });

        $('.transportationoption input[type=checkbox]').each(function (event) {
            if (this.checked) {
                $(this).removeAttr("disabled")
                $('.ownerscargooption input[type=checkbox]').attr("disabled", true)
                $('.motortruckoption input[type=checkbox]').attr("disabled", true)
            }
        });

        $('.transportationoption_FoodManuf input[type=checkbox]').each(function (event) {
            if (this.checked) {
                $(this).removeAttr("disabled")
                $('.ownerscargooption input[type=checkbox]').attr("disabled", true)
                $('.motortruckoption input[type=checkbox]').attr("disabled", true)
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
                    case "divBuildersRiskOption":
                        confirmString = "Are you sure you want to delete this Builders Risk item?";
                        break;
                    case "divComputerOption":
                        confirmString = "Are you sure you want to delete this Computer item?";
                        break;
                    case "divContractorOption":
                        confirmString = "Are you sure you want to delete this Contractors Equipment item?";
                        break;
                    case "divFineArtsFloaterOption":
                        confirmString = "Are you sure you want to delete this Fine Arts Floater item?";
                        break;
                    case "divInstallationFloaterOption":
                        confirmString = "Are you sure you want to delete this Installation Floater item?";
                        break;
                    case "divMotorTruckCargoRiskOption":
                    case "divUnScheduledMotorTruckCargoRiskOption":
                        confirmString = "Are you sure you want to delete this Motor Truck Cargo item?";
                        break;
                    case "divOwnersCargoOption":
                        confirmString = "Are you sure you want to delete this Owners Cargo item?";
                        break;
                    case "divScheduledPropertyFloaterOption":
                        confirmString = "Are you sure you want to delete this Scheduled Property Floater item?";
                        break;
                    case "divSignsOption":
                        confirmString = "Are you sure you want to delete this Signs item?";
                        break;
                    case "divTransportationOption", "divTransportationOption_FoodManuf":
                        confirmString = "Are you sure you want to delete this Transportation item?";
                        break;
                    default:
                        confirmString = "Are you sure you want to delete this line item?";
                        break;
                }
                //clear Options
                if (confirm(confirmString))
                {
                    ShowOrHideDetailPanel($(this), 'HIDE');
                    IM_ClickClearButton($(this));
                }
                else
                {
                    this.checked = true;
                    return false;
                }
                
            }
        });



    });

    $(function () {
        $('.chkOption2 input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                ShowOrHideDetailPanel($(this), 'SHOW')
            }
            else {
                ShowOrHideDetailPanel($(this), 'HIDE');
            }
        });

    });

    $(function () {
        $('.chkScheduledTools input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                $('.coverageMessage').hide()
            }
            else {
                //clear Options
                $('.coverageMessage').show()
            }
        });

    });

    $(function () {
        $('.motortruckoption input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                $('.ownerscargooption input[type=checkbox]').attr("disabled", true)
                $('.transportationoption input[type=checkbox]').attr("disabled", true)
            }
            else {
                //clear Options
                $('.ownerscargooption input[type=checkbox]').removeAttr("disabled")
                $('.transportationoption input[type=checkbox]').removeAttr("disabled")
            }
        });

        $('.ownerscargooption input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                $('.motortruckoption input[type=checkbox]').attr("disabled", true)
                $('.transportationoption input[type=checkbox]').attr("disabled", true)
            }
            else {
                //clear Options
                $('.motortruckoption input[type=checkbox]').removeAttr("disabled")
                $('.transportationoption input[type=checkbox]').removeAttr("disabled")
            }
        });

        $('.transportationoption input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                $('.ownerscargooption input[type=checkbox]').attr("disabled", true)
                $('.motortruckoption input[type=checkbox]').attr("disabled", true)
            }
            else {
                //clear Options
                $('.ownerscargooption input[type=checkbox]').removeAttr("disabled")
                $('.motortruckoption input[type=checkbox]').removeAttr("disabled")
            }
        });

        $('.transportationoption_FoodManuf input[type=checkbox]').on('change', function (event) {
            if (this.checked) {
                $('.ownerscargooption input[type=checkbox]').attr("disabled", true)
                $('.motortruckoption input[type=checkbox]').attr("disabled", true)
            }
            else {
                //clear Options
                $('.ownerscargooption input[type=checkbox]').removeAttr("disabled")
                $('.motortruckoption input[type=checkbox]').removeAttr("disabled")
            }
        });
    });

    // Delete button Warnings
    $(function () {
        $('.btnCIMDelete').on('click', function (event) {
            if (confirm('Are you sure you want to delete this line item?')) { return true; } else { return false; }
        });
    });
</script>

<div id="InlandMarineDiv" class="InlandMarineDiv">
    <h3>
        <asp:Label ID="lblTitle" runat="server" Text="INLAND MARINE"></asp:Label>
        <span style="float: right;">
            <%--<asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Data" OnClientClick="return ClearForm()">Clear</asp:LinkButton>--%>
            <asp:LinkButton ID="lnkSave" runat="server" CssClass="RemovePanelLink" ToolTip="Save Inland Marine Data" OnClientClick="return ValidateForm();">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
<%--        <uc1:cov_ContractorIMEndorsement runat="server" id="cov_ContractorIMEndorsement" />
        <uc1:cov_BuildersRisk runat="server" id="cov_BuildersRisk" />
        <uc1:cov_Computer runat="server" ID="cov_Computer" />--%>
        <%--<uc1:cov_Contractor runat="server" id="cov_Contractor" />--%>
<%--        <uc1:cov_FineArtsFloater runat="server" id="cov_FineArtsFloater" />
        <uc1:cov_InstallationFloater runat="server" ID="cov_InstallationFloater" />
        <uc1:cov_MotorTruckCargo runat="server" id="cov_MotorTruckCargo" />
        <uc1:cov_OwnersCargo runat="server" id="cov_OwnersCargo" />
        <uc1:cov_ScheduledPropertyFloater runat="server" id="cov_ScheduledPropertyFloater" />
        <uc1:cov_Signs runat="server" id="cov_Signs" />
        <uc1:cov_Transportation runat="server" id="cov_Transportation" />--%>
        <uc1:ctl_Endo_VehicleAdditionalInterestList runat="server" ID="ctl_Endo_VehicleAdditionalInterestList" />
        <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
        <uc1:ctl_CPP_ENDO_App_InlandMarine runat="server" ID="ctl_CPP_ENDO_App_InlandMarine" />
        <br />
    </div>
</div>

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width:100%; text-align:center;">
    <asp:Button ID="btnToCrime" runat="server" CssClass="StandardSaveButton" ToolTip="Continue to Crime" Text="Continue to Crime" />
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Inland Marine Coverages" Text="Save Inland Marine Coverages" />
    <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <br /><br />
    <uc1:ctl_RouteToUw runat="server" ID="ctl_RouteToUw" />
    <%--<input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" style="margin-top: 5px;" />--%>
    <br />
</div>
