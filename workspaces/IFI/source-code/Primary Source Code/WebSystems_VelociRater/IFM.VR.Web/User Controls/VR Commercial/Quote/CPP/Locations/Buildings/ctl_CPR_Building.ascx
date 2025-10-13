<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_Building.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_Building" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_BldgClassCodeLookup.ascx" TagPrefix="uc1" TagName="ctl_CCBLookup" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_EQClassCodeLookup.ascx" TagPrefix="uc1" TagName="ctl_EarthquakeCCLookup" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Building - "></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add New Building">Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Building" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Building Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>

<div>  
    <style type="text/css">
        .CPRBLDG_LabelColumn {
            width:30%;
            text-align:right;
        }
        .CPRBLDG_DataColumn {
            width:70%;
            text-align:left;
        }
        .CPRBLDGDDL {
            width:85%;
        }
        .CPRBLDGTXT {
            width:75%;
        }
    </style>
    <script type="text/javascript">
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
    </script>

    <div id="divClassCodeLookup" runat="server">
        <h3>Building Information</h3>
        <uc1:ctl_CCBLookup ID="ctl_CPR_ClassCodeLookup" runat="server" />
        <uc1:ctl_EarthquakeCCLookup ID="ctl_CPR_PPC_EQCCLookup" runat="server" />  <!-- PPC -->
        <uc1:ctl_EarthquakeCCLookup ID="ctl_CPR_PPO_EQCCLookup" runat="server" />  <!-- PPO -->
        <table style="width:100%;">
            <tr>
                <td class="CPRBLDG_LabelColumn">
                    Class Code
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:TextBox ID="txtINFClassCode" runat="server" onKeyDown="preventBackspace();" BackColor="#cccccc" onkeypress='return false' Width="70%" ></asp:TextBox>
                    <asp:Button ID="btnClassCodeLookup" runat="server" Text="Lookup" Width="25%" />
                </td>
            </tr>
            <tr>
                <td class="CPRBLDG_LabelColumn">
                    Description
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:TextBox ID="txtINFDescription" runat="server" Width="100%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="CPRBLDG_LabelColumn">
                    *Construction
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:DropDownList ID="ddINFConstruction" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CPRBLDG_LabelColumn">
                    Earthquake
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:CheckBox ID="chkINFEarthquake" runat="server" Text="" />
                </td>
            </tr>
            <tr id="trEarthquakeClassificationRow" runat="server" >
                <td class="CPRBLDG_LabelColumn">
                    Earthquake Classification
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:DropDownList ID="ddEarthquakeClassification" runat="server" Width="100%"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>

    <div id="divCovs" runat="server">
        <h3>Building Coverages</h3>
        <table id="tblBuildingCoverages" runat="server" style="width:100%;">
            <tr>
                <td>
                    <asp:CheckBox ID="chkBuildingCoverage" runat="server" Text="Building Coverage" />
                </td>
            </tr>
            <tr id="trBuildingCoverageDataRow" runat="server" >
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Building Limit
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtBCBuildingLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Inflation Guard
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBCInflationGuard" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Blanket Applied
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkBCBlanketApplied" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trBCBlanketInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trBCUseSpecificRatesRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Use Specific Rates
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkBCUseSpecificRates" runat="server" Text="" Enabled="false" Checked="true" />
                            </td>
                        </tr>
                        <tr id="trBCUseSpecificRatesInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                This classification requires specific rates. Please contact your underwriter or call the commercial help line at ext. 14001 for assistance.
                            </td>
                        </tr>
                        <tr id="trBCGroupIRow" runat="server" >
                            <td class="CPRBLDG_LabelColumn">
                                Group I
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtBCGroupI" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBCGroupIIRow" runat="server" >
                            <td class="CPRBLDG_LabelColumn">
                                Group II
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtBCGroupII" runat="server"  onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Cause Of Loss
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBCCauseOfLoss" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Co-Insurance
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBCCoInsurance" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Valuation
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBCValuation" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Deductible
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBCDeductible" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Agreed Amount
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkBCAgreedAmount" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trBCAgreedAmountInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trBCEarthquakeRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Earthquake
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkBCEarthquake" runat="server" Text="" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkBusinessIncomeCoverage" runat="server" Text="Business Income Coverage" />
                </td>
            </tr>
            <tr id="trBusinessIncomeDataRow" runat="server" >
                <td>
                    <table style="width:100%;">
                        <tr id="trBICLimit" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                *Business Income Limit
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtBICBusinessIncomeLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBICLimitType" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                *Limit Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:UpdatePanel ID="upCPRLimitType" runat="server">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="rbM" />
                                        <asp:AsyncPostBackTrigger ControlID="rbC" />
                                    </Triggers>

                                    <ContentTemplate>
                                        <asp:RadioButton ID="rbM" runat="server" AutoPostBack="true" Text="Monthly Period" GroupName="LT" />
                                        <asp:RadioButton ID="rbC" runat="server" AutoPostBack="true" Text="Co-Insurance" GroupName="LT" />
                                        <br />
                                        <asp:DropDownList ID="ddBICLimitType" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Income Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBICIncomeType" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Risk Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBICRiskType" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trBICUseSpecificRatesRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Use Specific Rates
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkBICUseSpecificRates" runat="server" Text="" Enabled="false" Checked="true" />
                            </td>
                        </tr>
                        <tr id="trBICUseSpecificRatesInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                This classification requires specific rates. Please contact your underwriter or call the commercial help line at ext. 14001 for assistance.
                            </td>
                        </tr>
                        <tr id="trBICGroupIRow" runat="server" >
                            <td class="CPRBLDG_LabelColumn">
                                Group I
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtBICGroupI" runat="server"  onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBICGroupIIRow" runat="server" >
                            <td class="CPRBLDG_LabelColumn">
                                Group II
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtBICGroupII" runat="server"  onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Cause of Loss
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBICCauseOfLoss" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trBICEarthquakeRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Earthquake
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkBICEarthquake" runat="server" Text="" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkPersonalPropertyCoverage" runat="server" Text="Personal Property Coverage" />
                </td>
            </tr>
            <tr id="trPersonalPropertyDataRow" runat="server" >
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Personal Property Limit
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPCPersonalPropertyLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Property Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPCPropertyType" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Risk Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPCRiskType" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Blanket Applied
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPCBlanketApplied" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trPPCBlanketAppliedInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trPPCUseSpecificRatesRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Use Specific Rates
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPCUseSpecificRates" runat="server" Text="" Enabled="false" Checked="true" />
                            </td>
                        </tr>
                        <tr id="trPPCUseSpecificRatesInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                This classification requires specific rates. Please contact your underwriter or call the commercial help line at ext. 14001 for assistance.
                            </td>
                        </tr>
                        <tr id="trPPCGroupIRow" runat="server" >
                            <td class="CPRBLDG_LabelColumn">
                                Group I
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPCGroupI" runat="server"  onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trPPCGroupIIRow" runat="server" >
                            <td class="CPRBLDG_LabelColumn">
                                Group II
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPCGroupII" runat="server"  onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Cause of Loss
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPCCauseOfLoss" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Co-Insurance
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPCCoinsurance" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Valuation
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPCValuation" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Deductible
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPCDeductible" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Agreed Amount
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPCAgreedAmount" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trPPCAgreedAmountInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trPPCEarthquakeRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Earthquake
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPCEarthquake" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trPPCEarthquakeLookupRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                *EQ Personal Property Classification
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPCEarthquakeClassification" runat="server" BackColor="#cccccc" Width="100%"></asp:TextBox>
                                <asp:Button ID="btnPPCLookupEQClassification" Text="Lookup Classification" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkPersonalPropertyOfOthers" runat="server" Text="Personal Property of Others" />
                </td>
            </tr>
            <tr id="trPersonalPropertyOfOthersDataRow" runat="server">
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Personal Property Limit
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPOPersonalPropertyLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Property Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPOPropertyType" runat="server" Enabled="false" Text="Personal Property of Others" Width="81%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                *Risk Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPORiskType" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Blanket Applied
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPOBlanketApplied" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trPPOBlanketAppliedInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trPPOUseSpecificRatesRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Use Specific Rates
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPOUseSpecificRates" runat="server" Text="" Enabled="false" Checked="true" />
                            </td>
                        </tr>
                        <tr id="trPPOUseSpecificRatesInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                This classification requires specific rates. Please contact your underwriter or call the commercial help line at ext. 14001 for assistance.
                            </td>
                        </tr>
                        <tr id="trPPOGroupIRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Group I
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPOGroupI" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trPPOGroupIIRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Group II
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPOGroupII" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Cause of Loss
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPOCauseOfLoss" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Co-Insurance
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPOCoinsurance" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Valuation
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPOValuation" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Deductible
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPODeductible" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trPPOEarthquakeRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Earthquake
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:CheckBox ID="chkPPOEarthquake" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trPPOEarthquakeLookupRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                *EQ Personal Property Classification
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:TextBox ID="txtPPOEQClassification" runat="server" BackColor="#cccccc" Width="100%"></asp:TextBox>
                                <asp:Button ID="btnPPOLookupEQClassCode" Text="Lookup Classification" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>    

    <asp:Button ID="btnSave" runat="server" Text="Save Building" ToolTip="Save Building" CssClass="StandardSaveButton" />
    <asp:HiddenField ID="hdnCCAccord" runat="server" />
    <asp:HiddenField ID="hdnCovsAccord" runat="server" />
    <!-- The hidden fields below hold the selected values from the class code lookup control --> 
    <!-- Note that the ClassCode and Description are stored in text fields -->
    <asp:HiddenField ID="hdnDIA_Id" runat="server" />
    <asp:HiddenField ID="hdnPMAID" runat="server" />
    <asp:HiddenField ID="hdnGroupRate" runat="server" />
    <asp:HiddenField ID="hdnClassLimit" runat="server" />
    <asp:HiddenField ID="hdnYardRateId" runat="server" />
    <!-- The hidden fields below hold the selected values from the Earthquake Class Code Lookup control -->
    <asp:HiddenField ID="hdnDIA_PPC_EQCC_Id" runat="server" />
    <asp:HiddenField ID="hdnDIA_PPO_EQCC_Id" runat="server" />
    <asp:HiddenField ID="hdn_PPC_RateGroup" runat="server" />
    <asp:HiddenField ID="hdn_PPO_RateGroup" runat="server" />

    <asp:HiddenField ID="hdnBICLimitTypeValues" runat="server" />
</div> 