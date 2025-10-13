<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_Building.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_Building" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_BldgClassCodeLookup.ascx" TagPrefix="uc1" TagName="ctl_CCBLookup" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_EQClassCodeLookup.ascx" TagPrefix="uc1" TagName="ctl_EarthquakeCCLookup" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_BOP_ENDO_App_Building.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_App_Building" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Building - "></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add New Building">Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Building" >Delete</asp:LinkButton>
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

    <div id="divClassCodeLookup" class="classcode" runat="server">
        <h3>
            Building Information
            <span style="float: right;">
                <asp:LinkButton ID="lnkSaveBuildingInfo" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Save">Save</asp:LinkButton>
                <asp:LinkButton ID="lnkClearBuildingInfo" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Building Info" >Clear</asp:LinkButton>
            </span>
        </h3>
        <uc1:ctl_CCBLookup ID="ctl_CPR_ClassCodeLookup" runat="server" />
        <uc1:ctl_EarthquakeCCLookup ID="ctl_CPR_PPC_EQCCLookup" runat="server" />  <!-- PPC -->
        <uc1:ctl_EarthquakeCCLookup ID="ctl_CPR_PPO_EQCCLookup" runat="server" />  <!-- PPO -->
        <table style="width:100%;">
            <tr id="trPrefillInfoTextOldCo" runat="server" style="display:none;" class="informationalText">
                <td colspan="2">The Number of Stories field might be pre-populated with data from our database service.  If prefilled data is modified, please be aware changes are subject to review by your underwriter.</td>
            </tr>
            <tr id="trPrefillInfoTextNewCo" runat="server" style="display:none;" class="informationalText">
                <td colspan="2">The Year Built, Square Feet and Number of Stories might be pre-populated with data from our database service.  If prefilled data is modified, please be aware changes are subject to review by your underwriter.</td>
            </tr>
            <tr id="trClassCodeRow" runat="server">
                <td class="CPRBLDG_LabelColumn">
                    Class Code
                </td>
                <td class="CPRBLDG_DataColumn">
                    <table style="width:100%;">
                        <tr>
                            <td style="width:75%;text-align:left;">
                                <asp:TextBox ID="txtINFClassCode" runat="server" onKeyDown="preventBackspace();" BackColor="#cccccc" onkeypress='return false' Width="95%" ></asp:TextBox>
                            </td>
                            <td style="width:25%;text-align:left;vertical-align:top;">
                                <asp:Button ID="btnClassCodeLookup" runat="server" Text="Lookup" />
                            </td>
                        </tr>
                    </table>
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
            <tr id="trYearBuilt" runat="server">
                <td class="CPRBLDG_LabelColumn">
                    *Year Built
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:TextBox ID="txtYearBuilt" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="trSquareFeet" runat="server">
                <td class="CPRBLDG_LabelColumn">
                    *Square Feet
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:TextBox ID="txtSquareFeet" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' MaxLength="6"></asp:TextBox>
                </td>
            </tr>
            <tr id="trNumOfStories" runat="server">
                <td id="tdNumberOfStories" class="CPRBLDG_LabelColumn">
                    Number of Stories
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:TextBox ID="txtNumOfStories" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' MaxLength="3" Width="50px"></asp:TextBox>
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
            <tr id="trEarthquakeDeductibleRow" runat="server" >
                <td class="CPRBLDG_LabelColumn">
                    Earthquake Deductible Options
                </td>
                <td class="CPRBLDG_DataColumn">
                    <asp:DropDownList ID="ddEarthquakeDeductible" runat="server" Width="100%"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>

    <div id="divCovs" class="building" runat="server">
        <h3>
            Building Coverages
            <span style="float: right;">
                <asp:LinkButton ID="lnkClearBuildingCoverages" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear all building coverages">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSaveBuildingCoverages" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Save" >Save</asp:LinkButton>
            </span>
        </h3>
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
                        <tr id="trBCBlanketRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Blanket Applied
                            </td>
                            <td>
                                <asp:CheckBox ID="chkBCBlanketApplied" runat="server" Text="" />
                                <span class="informationalText" id="BCBlanketText" runat="server" visible="false">Dwellings do not qualify for blanket coverage.</span>
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
                                <asp:CheckBox ID="chkBCUseSpecificRates" runat="server" Text="" Enabled="false" />
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
                        <tr id="trBCDeductibleRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Deductible
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddBCDeductible" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr Id="trOwnerOccupiedPercentageRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Owner Occupied Percentage
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddOwnerOccupiedPercentage" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="CPRBLDG_LabelColumn">
                                Agreed Amount
                            </td>
                            <%--<td class="CPRBLDG_DataColumn">--%>
                            <td>
                                <asp:CheckBox ID="chkBCAgreedAmount" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trBCAgreedAmountInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trBCWindHailRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Wind Hail Deductible %
                            </td>
                            <td>
                                <asp:CheckBox ID="chkBCWindHail" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trBCWindHailText" class="informationalText" runat="server">
                            <td colspan="2">
                                 A minimum 1% deductible applies to building coverage for lessors' risk exposures.
                            </td>
                        </tr>
                        <tr id="trMineSubsidenceRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Mine Subsidence
                            </td>
                            <%--<td class="CPRBLDG_DataColumn">--%>
                            <td>
                                <asp:CheckBox ID="chkMineSubsidence" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trMineSubsidenceNumberOfUnitsRow" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'>
                            <td>
                                Total Number of Units
                            </td>
                            <td>
                                <asp:TextBox ID="txtMineSubNumberOfUnits" runat="server" Width="30px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trMineSubsidenceInfoForRequiredMineSubsidence_IL" runat="server" style="display: none;" class="informationalText">
                            <td colspan="2">
                                Mine Subsidence is required for this location, if you would like to opt out please contact your underwriter.
                            </td>
                        </tr>
                        <tr id="trMineSubsidenceInfoForNOTRequiredMineSubsidence_IL" runat="server" style="display: none;" class="informationalText">
                            <td colspan="2">
                                Mine Subsidence must be selected for all IL locations if selected for any IL locations.  It has been added automatically to other IL locations.
                            </td>
                        </tr>
                        <tr id="trMineSubsidenceInfoForRequiredMineSubsidence_OH" runat="server" style="display: none;" class="informationalText">
                            <td colspan="2">
                                Mine Subsidence is required for this location.
                            </td>
                        </tr>
                        <tr id="trMineSubsidenceLimitInfo" runat="server" style="display: none;" class="informationalText">
                            <td colspan="2">
                                Building limit over 300,000. Mine subsidence limit 300,000.
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
                        <tr id="trBICLimitTypeRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                *Limit Type
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:RadioButton ID="rbM" runat="server" Text="Monthly Period" GroupName="LT" />
                                <asp:RadioButton ID="rbC" runat="server" Text="Co-Insurance" GroupName="LT" />
                                <br />
                                <asp:DropDownList ID="ddBICLimitTypeM" runat="server" CssClass="CPRBLDGDDL" style="display:none;">
                                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="1/3" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="1/4" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="1/6" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddBICLimitTypeC" runat="server" CssClass="CPRBLDGDDL" style="display:none;">
                                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="50%" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="60%" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="70%" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="80%" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="90%" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="100%" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="125%" Value="12"></asp:ListItem>
                                </asp:DropDownList>
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
                                <asp:CheckBox ID="chkBICUseSpecificRates" runat="server" Text="" Enabled="false" />
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
                        <tr id="trPPCBlanketRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Blanket Applied
                            </td>
                            <td>
                                <asp:CheckBox ID="chkPPCBlanketApplied" runat="server" Text="" />
                                <span class="informationalText" id="PPCBlanketText" runat="server" visible="false">Dwellings do not qualify for blanket coverage.</span>
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
                                <asp:CheckBox ID="chkPPCUseSpecificRates" runat="server" Text="" Enabled="false" />
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
                         <tr id="trPPCDeductibleRow" runat="server">
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
                            <%--<td class="CPRBLDG_DataColumn">--%>
                            <td>
                                <asp:CheckBox ID="chkPPCAgreedAmount" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr id="trPPCAgreedAmountInfoRow" runat="server" class="informationalText">
                            <td colspan="2">
                                Blanket and/or Agreed Amount require a signed statement of values. Please forward this to your underwriter upon binding coverage.
                            </td>
                        </tr>
                        <tr id="trPPCWindHailRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Wind Hail Deductible %
                            </td>
                            <td>
                                <asp:CheckBox ID="chkPPCWindHail" runat="server" Text="" />
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
                        <tr id="trPPOBlanketRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Blanket Applied
                            </td>
                            <td>
                                <asp:CheckBox ID="chkPPOBlanketApplied" runat="server" Text="" />
                                <span class="informationalText" id="PPOBlanketText" runat="server" visible="false">Dwellings do not qualify for blanket coverage.</span>
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
                                <asp:CheckBox ID="chkPPOUseSpecificRates" runat="server" Text="" Enabled="false" />
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
                        <tr id="trPPODeductibleRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Deductible
                            </td>
                            <td class="CPRBLDG_DataColumn">
                                <asp:DropDownList ID="ddPPODeductible" runat="server" CssClass="CPRBLDGDDL"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trPPOWindHailRow" runat="server">
                            <td class="CPRBLDG_LabelColumn">
                                Wind Hail Deductible %
                            </td>
                            <td>
                                <asp:CheckBox ID="chkPPOWindHail" runat="server" Text="" />
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
    <uc2:ctl_BOP_ENDO_App_Building runat="server" ID="ctl_BOP_ENDO_App_Building" />
    
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

    <!-- hold visibility status of the Specific Rates fields -->
    <asp:HiddenField ID="hdn_BC_UseSpecific_Visible" runat="server" />
    <asp:HiddenField ID="hdn_BIC_UseSpecific_Visible" runat="server" />
    <asp:HiddenField ID="hdn_PPC_UseSpecific_Visible" runat="server" />
    <asp:HiddenField ID="hdn_PPO_UseSpecific_Visible" runat="server" />

    <!-- Hold the checked value of the PPC and PPO specific rates checkbox status -->
    <asp:HiddenField ID="hdn_BC_UseSpecific_Checked" runat="server" />
    <asp:HiddenField ID="hdn_BIC_UseSpecific_Checked" runat="server" />
    <asp:HiddenField ID="hdn_PPC_UseSpecific_Checked" runat="server" />
    <asp:HiddenField ID="hdn_PPO_UseSpecific_Checked" runat="server" />

    <!-- Hold the checked values of the Agreed Amount checkboxes -->
    <asp:HiddenField ID="hdn_Agreed_BC" runat="server" />
    <asp:HiddenField ID="hdn_Agreed_PPC" runat="server" />

    <asp:HiddenField ID="hdnBICLimitTypeValues" runat="server" />
</div> 