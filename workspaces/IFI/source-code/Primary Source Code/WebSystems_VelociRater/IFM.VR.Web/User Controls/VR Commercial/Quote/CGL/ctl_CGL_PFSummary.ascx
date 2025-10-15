<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_PFSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_PFSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/PolicyDiscounts.ascx" TagPrefix="uc1" TagName="PolicyDiscounts" %>


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
        border: 1px solid black;
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
</style>

<div id="divCGLQuoteSummary">
    <h3>
        <asp:Label ID="lblMainAccord" runat="server" Text="Quote Summary"></asp:Label>
        <span style="float: right;">
            <asp:Label ID="lblPremiumMainAccord" runat="server" Text="$1234.56"></asp:Label>
            <%--<asp:LinkButton ID="lnkPrint" ToolTip="Show Printer Friendly Summary" CssClass="RemovePanelLink" runat="server">Printer Friendly</asp:LinkButton>--%>
        </span>
        <span runat="server" id="ImageDateAndPremChangeLine" visible="false">
            <br />
            <asp:Label ID="lblTranEffDate" runat="server"></asp:Label>
            <asp:Label ID="lblAnnualPremChg" runat="server" Style="margin-left: 20px;"></asp:Label>
        </span>
    </h3>
    <div>
        <table style="width: 100%;">
            <colgroup>
                <col class="qs_basic_info_labels_cell qs_resetWidth" />
            </colgroup>
            <tr>
                <td>Policy Holder Name</td>
                <td>
                    <asp:Label CssClass="qs_basic_info_labels" ID="lblPhName" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr id="trCareOf" runat="server" visible="false">
                <td>Other: </td>
                <td>
                    <asp:Label CssClass="qs_hom_basic_info_labels" ID="lblCareOf" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Policy Holder Address</td>
                <td>
                    <asp:Label ID="lblPhAddress" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>Quote Number</td>
                <td>
                    <asp:Label ID="lblQuoteNum" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>Effective Date</td>
                <td>
                    <asp:Label ID="lblEffectiveDate" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>Expiration Date</td>
                <td>
                    <asp:Label ID="lblExpirationDate" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>Full Term Premium</td>
                <td>
                    <asp:Label ID="lblFullPremium" runat="server" Text="Label"></asp:Label></td>
            </tr>
        </table>

        <div class="qs_Main_Sections">
            <span class="qs_section_headers">Policy Level Coverage Options</span>
            <div class="qs_Sub_Sections">
                <asp:Literal ID="tblPolicyCoverages" runat="server"></asp:Literal>
                <table class="qa_table_shades">

                    <%-- Header Row --%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns">
                        <td>Coverage</td>
                        <td class="qs_rightJustify">Limit</td>
                        <td class="qs_rightJustify qs_padRight ">Premium</td>
                    </tr>

                    <%-- Data Rows --%>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowGenAgg">
                        <td>General Aggregate</td>
                        <td id="tdGenAggLimit" runat="server" class="qs_rightJustify"></td>
                        <td id="tdGenAggPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowProdCompOpsAgg">
                        <td>Products/Completed Operations Aggregate</td>
                        <td id="ProdCompOpsAggLimit" runat="server" class="qs_rightJustify"></td>
                        <td id="ProdCompOpsAggLimitQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowPersAndAdInjury">
                        <td>Personal and Advertising Injury</td>
                        <td id="tdPersAndAdInjuryLimit" runat="server" class="qs_rightJustify"></td>
                        <td id="tdPersAndAdInjuryLimitQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowOccLiability">
                        <td>Occurrence Liability Limit</td>
                        <td id="tdOccLiabilityLimit" runat="server" class="qs_rightJustify"></td>
                        <td id="tdOccLiabilityLimitQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowDamageToPremsRented">
                        <td>Damage to Premises Rented by You</td>
                        <td id="tdDamageToPremsRentedLimit" runat="server" class="qs_rightJustify"></td>
                        <td id="tdDamageToPremsRentedLimitQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowMedicalExpenses">
                        <td>Medical Expenses</td>
                        <td id="tdMedicalExpensesLimit" runat="server" class="qs_rightJustify"></td>
                        <td id="tdMedicalExpensesLimitQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowBusMasterEnhancement">
                        <td>General Liability Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdBusMasterEnhancementQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                     <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowGlPlusEnhancement">
                        <td>General Liability PLUS Enhancement Endorsement</td>
                        <td></td>
                        <td id="GlPlusEnhancementQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowBlanketWaiverOfSubro">
                        <td id="tdBlanketWaiverOfSubroText">Blanket Waive of Subro</td>
                        <td></td>
                        <td id="tdBlanketWaiverOfSubroQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <asp:Panel runat="server" Visible="false" ID="pnlGenLibDeductible">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>General Liability Deductible</td>
                            <td></td>
                            <td id="tdGenLibDeductibleQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Type: <span runat="server" id="GenLibDeductibleType">$type</span></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Amount: <span runat="server" id="GenLibDeductibleAmount">$amount</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Basis: <span runat="server" id="GenLibDeductibleBasis">$basis</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </asp:Panel>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowAdditionalInsureds">
                        <td>Additional Insureds</td>
                        <td></td>
                        <td id="tdAdditionalInsuredsPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowEmployeeBenefitsLiability">
                        <td>Employee Benefits Liability: <span runat="server" id="EmpBLibNum">$EmpBLibNum</span> </td>
                        <td></td>
                        <td id="tdEmployeeBenefitsLiabilityQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowEPLI">
                        <td>EPLI: (non-underwritten)</td>
                        <td></td>
                        <td id="tdEPLIQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowHiredAuto">
                        <td>Hired Auto</td>
                        <td></td>
                        <td id="tdHiredAutoQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowNonOwnedAuto">
                        <td>Non-Owned Auto</td>
                        <td></td>
                        <td id="tdNonOwnedAutoQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowStopGap">
                        <td>Stop Gap (OH)</td>
                        <td></td>
                        <td id="tdStopGapQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <asp:Panel runat="server" Visible="false" ID="pnlLiquorLiability_IN">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>Liquor Liability - Indiana</td>
                            <td></td>
                            <td id="tdLiquorLiabilityQuotedPremium_IN" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Occurrence Limit: <span runat="server" id="LiquorLiabilityOccLimit_IN">$Occ Limit</span></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Classification: <span runat="server" id="LiquorLiabilityClass_IN">$class</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Liquor Sales: <span runat="server" id="LiquorLiabilityLSales_IN">$sales</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </asp:Panel>
                    <!-- Ohio Liquor Liability -->
                    <asp:Panel runat="server" Visible="false" ID="pnlLiquorLiability_OH">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>Liquor Liability - Ohio</td>
                            <td></td>
                            <td id="tdLiquorLiabilityQuotedPremium_OH" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Occurrence Limit: <span runat="server" id="LiquorLiabilityOccLimit_OH">$Occ Limit</span></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Classification: <span runat="server" id="LiquorLiabilityClass_OH">$class</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Liquor Sales: <span runat="server" id="LiquorLiabilitySales_OH">$sales</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </asp:Panel>
                    <!-- Illinois Liquor Liability -->
                    <asp:Panel runat="server" Visible="false" ID="pnlLiquorLiability_IL">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>Liquor Liability - Illinois</td>
                            <td></td>
                            <td id="tdLiquorLiabilityQuotedPremium_IL" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Occurrence Limit: <span runat="server" id="LiquorLiabilityOccLimit_IL">$Occ Limit</span></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Classification: <span runat="server" id="LiquorLiabilityClass_IL">$class</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Liquor Sales: <span runat="server" id="LiquorLiabilityLSales_IL">$sales</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </asp:Panel>
                    <!-- end Illinois Liquor Liability -->
                    <asp:Panel runat="server" Visible="false" ID="pnlContractorsHomeRepairAndRemodeling">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>IL Contractors - Home Repair & Remodeling</td>
                            <td id="tdContractorsHomeRepairAndRemodelingLimit" runat="server" class="qs_rightJustify">10,000</td>
                            <td id="tdContractorsHomeRepairAndRemodelingPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel runat="server" Visible="false" ID="pnlProfessionalLiability">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>Professional Liability</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns" runat="server" visible="false" id="rowProfessionalLiability_CP">
                            <td>Cemetary Professional: <span runat="server" id="PL_CP_Num"></span></td>
                            <td></td>
                            <td id="tdPL_CP_Premium" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns" runat="server" visible="false" id="rowProfessionalLiability_FP">
                            <td>Funeral Directors Professional: <span runat="server" id="PL_FP_Num"></span></td>
                            <td></td>
                            <td id="tdPL_FP_Premium" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns" runat="server" visible="false" id="rowProfessionalLiability_PP">
                            <td>Pastoral Professional: <span runat="server" id="PL_PP_Num"></span></td>
                            <td></td>
                            <td id="tdPL_PP_Premium" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                    </asp:Panel>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowMinimumPremiumAdjustment">
                        <%--<td>Minimum Premium Adjustment - (<asp:Label runat="server" ID="lblDec_Minimum_Prem"></asp:Label>)</td>--%>
                        <td>Minimum Premium Adjustment</td>
                        <td></td>
                        <td id="tdMinimum_Prem" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowPremisesAdjustment">
                        <%--<td>Amount to Equal Minimum Premium (Premises) - (<asp:Label runat="server" ID="lblDec_Premises_Minimum_Prem"></asp:Label>)</td>--%>
                        <td>Amount to Equal Minimum Premium (Premises)</td>
                        <td></td>
                        <td id="tdPremises_MinPremAdj_Prem" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowProductsAdjustment">
                        <%--<td>Amount to Equal Minimum Premium (Products) - (<asp:Label runat="server" ID="lblDec_Products_Minimum_Prem"></asp:Label>)</td>--%>
                        <td>Amount to Equal Minimum Premium (Products)</td>
                        <td></td>
                        <td id="tdProducts_MinPremAdj_Prem" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                </table>
            </div>
        </div>
        <uc1:PolicyDiscounts runat="server" id="ctlPolicyDiscounts" />
        <div class="qs_Main_Sections">
            <span class="qs_section_headers">LOCATION INFORMATION</span>
            <div class="qs_Sub_Sections">
                <asp:Literal ID="tblLocations" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="qs_Main_Sections">
            <span class="qs_section_headers">CLASSIFICATIONS</span>
            <div class="qs_Sub_Sections">
                <asp:Literal ID="tblClassifications" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLossHistory" runat="server"></asp:Literal>
        </div>

        <%--<uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />--%>

        <asp:HiddenField ID="HiddenField1" runat="server" />
    </div>
</div>
