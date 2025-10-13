<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_QuoteSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_QuoteSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>
<%@ register src="~/User Controls/VR Commercial/Quote/PolicyDiscounts.ascx" tagprefix="uc1" tagname="PolicyDiscounts" %>


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

    .qs_grid_5_columns td{
        width: 20%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_grid_4_columns td{
        width: 25%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_grid_3_columns td{
        width: 15%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_grid_3_columns td:first-child{
        width: 70%;
        /*min-width: 25%;
        max-width: 25%;*/
    }

    .qs_grid_5_columns td:first-child{
        width: 12%;
        /*min-width: 25%;
        max-width: 25%;*/
    }

    .qs_grid_5_columns td:nth-child(2){
        width: 185px;
        /*min-width: 25%;
        max-width: 25%;*/
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        display:inline-block;
    }

    .qs_grid_5_columns td:nth-child(5){
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
    .qs_grid_4_columns .ThreeHeaders td:nth-child(3), .qs_grid_4_columns.3Title td:nth-child(4){
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
    .CPPQSSpacer {
        width:10px;
    }
</style>

<div id="divCPPQuoteSummary">
    <h3>
        <asp:Label ID="lblMainAccord" runat="server" Text="Quote Summary"></asp:Label>
        <span style="float: right;">
            <asp:Label ID="lblPremiumMainAccord" runat="server" Text="$1234.56"></asp:Label>
            <asp:LinkButton ID="lnkPrint" ToolTip="Show Printer Friendly Summary" CssClass="RemovePanelLink" runat="server">Printer Friendly</asp:LinkButton>
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
                <col class="qs_basic_info_labels_cell qs_resetWidth"/>
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
                <td>Property Quoted Premium</td>
                <td>
                    <asp:Label ID="lblPropertyQuotedPremium" runat="server" Text="Label"></asp:Label></td>
            </tr>            
            <tr>
                <td>General Liability Quoted Premium</td>
                <td>
                    <asp:Label ID="lblGeneralLiabilityQuotedPremium" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>            
            <tr>
                <td>Inland Marine Quoted Premium</td>
                <td>
                    <asp:Label ID="lblInlandMarineQuotedPremium" runat="server" Text="Label"></asp:Label></td>
            </tr>            
            <tr>
                <td>Crime Quoted Premium</td>
                <td>
                    <asp:Label ID="lblCrimeQuotedPremium" runat="server" Text="Label"></asp:Label></td>
            </tr>            
            <tr>
                <td>Full Term Premium</td>
                <td>
                    <asp:Label ID="lblFullPremium" runat="server" Text="Label"></asp:Label></td>
            </tr>            
        </table>

        <div class="qs_Main_Sections">
            <span class="qs_section_headers">GENERAL INFORMATION</span>
            <div class="qs_Sub_Sections">
                <%--<asp:Literal ID="tblPolicyCoverages" runat="server"></asp:Literal>--%>
                <table class="qa_table_shades">

                    <%-- Header Row --%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns">
                        <td style="width:70%;">Coverage</td>
                        <td class="qs_rightJustify qs_padRight ">Limit</td>
                        <td class="qs_rightJustify qs_padRight ">Premium</td>
                    </tr>

                    <%-- Data Rows --%>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowPropEnhEndo">
                        <td style="width:70%;">Property Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdPropertyEnhancementEndorsementPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                     <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowPropPlusEnhEndo">
                        <td style="width:70%;">Property PLUS Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdPropertyPlusEnhancementEndorsementPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowGLEnhEndo">
                        <td style="width:70%;">General Liability Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdGeneralLiabilityEnhancementEndorsementPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                     <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="rowGlPlusEnhancement">
                        <td>General Liability PLUS Enhancement Endorsement</td>
                        <td></td>
                        <td id="GlPlusEnhancementQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowContEnhEndo">
                        <td style="width:70%;">Contractors Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdContractorsEnhancementEndorsementPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowManuEnhEndo">
                        <td style="width:70%;">Manufacturer Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdManufacturerEnhancementEndorsementPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowFoodManuEndo">
                        <td style="width:70%;">Food Manufacturers Enhancement Endorsement Package</td>
                        <td></td>
                        <td id="tdFoodManufacturersEnhancementEndorsementPackagePremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowGenAgg">
                        <td style="width:70%;">General Aggregate</td>
                        <td id="tdGeneralAggregateLimit" runat="server" class="qs_rightJustify qs_padRight">$0</td>
                        <td id="tdGeneralAggregatePremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowProdCompOps">
                        <td style="width:70%;">Products/Completed Operations Aggregate</td>
                        <td id="tdProductsCompletedOpsAggregateLimit" runat="server" class="qs_rightJustify qs_padRight">$0</td>
                        <td id="tdProductsCompletedOpsAggregatePremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowPersAdvInj">
                        <td style="width:70%;">Personal and Advertising Injury</td>
                        <td id="tdPersonalAndAdvertisingInjuryLimit" runat="server" class="qs_rightJustify qs_padRight">$0</td>
                        <td id="tdPersonalAndAdvertisingInjuryPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowOccLiabLimit">
                        <td style="width:70%;">Occurrence Liability Limit</td>
                        <td id="tdOccurrenceLiabilityLimitLimit" runat="server" class="qs_rightJustify qs_padRight">$0</td>
                        <td id="tdOccurrenceLiabilityLimitPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowDmgToPremRentedByYou">
                        <td style="width:70%;">Damage to Premises Rented by You</td>
                        <td id="tdDmgToPremisesRentedByYouLimit" runat="server" class="qs_rightJustify qs_padRight">$0</td>
                        <td id="tdDmgToPremisesRentedByYouPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowMedicalExpenses">
                        <td style="width:70%;">Medical Expenses</td>
                        <td id="tdMedicalExpensesLimit" runat="server" class="qs_rightJustify qs_padRight">$0</td>
                        <td id="tdMedicalExpensesPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowBlanketWaiverOfSubro">
                        <td style="width:70%;">Blanket Waiver of Subro</td>
                        <td></td>
                        <td id="tdBlanketWaiverOfSubroPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowBlanketWaiverOfSubroCompletedOps">
                        <td style="width:70%;">Blanket Waiver of Subro w/Completed Ops</td>
                        <td></td>
                        <td id="tdBlanketWaiverSubroCompletedOpsPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowGLDeductible">
                        <td colspan="3">General Liability Deductible</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowGLDeductibleType">
                        <td id="tdGLDeductibleType" colspan="3" style="margin-left:10px;">&nbsp;&nbsp;&nbsp;&nbsp;Type:</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowGLDeductibleAmount">
                        <td id="tdGLDeductibleAmount" colspan="3" style="margin-left:10px;">&nbsp;&nbsp;&nbsp;&nbsp;Amount:</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowGLDeductibleBasis">
                        <td id="tdGLDeductibleBasis" colspan="3" style="margin-left:10px;">&nbsp;&nbsp;&nbsp;&nbsp;Basis:</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowAdditionalInsureds">
                        <td style="width:70%;">Additional Insureds</td>
                        <td></td>
                        <td id="tdAdditionalInsuredsPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowCondoDirectorsAndOfficers">
                        <td style="width:70%;">Condo Directors and Officers</td>
                        <td></td>
                        <td id="tdCondoCirectorsAndOfficersPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowEBL">
                        <td id="tdEBLInfo" colspan="2" runat="server">Employee Benefits Liability</td>
                        <td id="tdEBLPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowEPLI">
                        <td style="width:70%;">EPLI:(non-underwritten)</td>
                        <td></td>
                        <td id="tdEPLIPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowCLI">
                        <td id="tdCLICovName" runat="server">Cyber Liability</td>
                        <td></td>
                        <td id="tdCLIPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowHiredAuto">
                        <td style="width:70%;">Hired Auto</td>
                        <td></td>
                        <td id="tdHiredAutoPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowNonOwnedAuto">
                        <td style="width:70%;">Non-Owned Auto</td>
                        <td></td>
                        <td id="tdNonOwnedAutoPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>

                    <!-- Indiana Liquor Liability -->
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
                            <td>Liquor Sales: <span runat="server" id="LiquorLiabilityLSales_OH">$sales</span> </td>
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
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" >
                            <td>IL Contractors - Home Repair & Remodeling</td>
                            <td id="tdContractorsHomeRepairAndRemodelingLimit" runat="server" class="qs_rightJustify">10,000</td>
                            <td id="tdContractorsHomeRepairAndRemodelingPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                        </tr>
                    </asp:Panel>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowBlanketRating">
                        <td id="tdBlanketRatingName" runat="server" style="width:50%;">Blanket Rating</td>
                        <td></td>
                        <td id="tdBlanketRatingPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowStopGapOH">
                        <td style="width:70%;">Stop Gap (OH)</td>
                        <td></td>
                        <td id="tdStopGapPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowMinPremAdj">
                        <td style="width:50%;">Minimum Premium Adjustment</td>
                        <td></td>
                        <td id="tdMinimumPremiumAdjustmentPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                </table>
            </div>
        </div>
        <uc1:policydiscounts runat="server" id="ctlPolicyDiscounts" />
        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLocations" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblBuildings" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblPropertyInTheOpen" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblClassifications" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblInlandMarine" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblCrime" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLossHistory" runat="server"></asp:Literal>
        </div>

        <uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />

        <asp:HiddenField ID="HiddenField1" runat="server" />
        <div style="text-align:center">
            <asp:Label ID="lblPrintReminder" runat="server" CssClass="informationalText" Visible="false" Text="Reminder:  After submitting your application, access to policy documents, including the ACORD application, will not be available until the policy is issued."></asp:Label>
        </div>
    </div>
</div>
