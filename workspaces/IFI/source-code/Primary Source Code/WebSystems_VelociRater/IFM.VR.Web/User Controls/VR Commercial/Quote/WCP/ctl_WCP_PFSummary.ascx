<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WCP_PFSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_WCP_PFSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>



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

</style>

<div id="divWCPQuoteSummary">
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
    <div >
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
                <td>Full Term Premium</td>
                <td>
                    <asp:Label ID="lblFullPremium" runat="server" Text="Label"></asp:Label></td>
            </tr> 
            <tr>
                <td>Indiana Second Injury Fund Surcharge</td>
                <td>
                    <asp:Label ID="lblIndSecondInjurySurcharge" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>Employer&#39;s Liability</td>
                <td>
                    <asp:Label ID="lblEmployersLiability" runat="server" Text="Label"></asp:Label></td>
            </tr>           
        </table>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLocationInfo" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblClassCodes" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblNamedIndividuals" runat="server"></asp:Literal>
            <div class="qs_Sub_Sections">
                <table class="qa_table_shades">

                    <%--' Header Row--%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns">
                        <td>Named Individuals</td>
                        <td></td>
                        <td class="qs_rightJustify qs_padRight ">Premium</td>
                    </tr>

                    <%-- ' Data Rows--%>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="Inclusion">
                        <td>Inclusion of Sole Proprietors, Partners and LLC members (WC 00 03 10)</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <asp:Panel runat="server" Visible="false" ID="Waiver13">
                        <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns">
                            <td>Waiver of Subrogation (WC 00 03 13)</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Number of Waivers: <span runat="server" id="numWaivers">X</span></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr class="qs_basic_info_labels_cell qs_indent qs_cap_grid_3_columns">
                            <td>Amount Per Waiver: <span runat="server" id="AmtWaivers">$xx</span> </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </asp:Panel>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="WaiverContract">
                        <td>Waiver of Subrogation &#45; When required by Written Contract</td>
                        <td></td>
                        <td id="WiaverContractPremium" class="qs_rightJustify qs_padRight "></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="ExclusionAmish">
                        <td>Exclusion of Amish Workers (WC 00 03 08)</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="ExclusionOfficer">
                        <td>Exclusion of Executive Officer (WC 00 03 08)</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="trExclusionOfExecutiveOfficerEtc">
                        <td>Exclusion of Sole Proprietors, Partners, Officers, LLC Members & others (WC 12 03 07)(IL)</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="false" id="trRejectionOfCoverageEndorsement">
                        <td>Rejection of Coverage Endorsement (WC 16 03 01)(KY)</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <%--' Close table--%>
                </table>
            </div>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLossHistory" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <span class="qs_section_headers">POLICY COVERAGES</span>
            <div class="qs_Sub_Sections">
                <table class="qa_table_shades">

                    <%--' Header Row--%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_grid_3_columns">
                        <td>Coverage</td>
                        <td></td>
                        <td class="qs_rightJustify">Premium</td>
                    </tr>

                    <%-- ' Data Rows--%>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_TotalEstimatedPlanPremium_Row">
                        <td>Total Estimated Plan Premium</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_TotalEstimatedPlanPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_EmployersLiabilityQuotedPremium_Row">
                        <td>Increased Limit</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_EmployersLiabilityQuotedPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_ExpModQuotedPremium_Row">
                        <td>Experience Modification</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_ExpModQuotedPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_ScheduleModQuotedPremium_Row">
                        <td>Schedule Modification</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_ScheduleModQuotedPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_PremDiscountQuotedPremium_Row">
                        <td>Premium Discount</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_PremDiscountQuotedPremium">$200</td>
                    </tr>
                    <%--<tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_Dec_LossConstantPremium_Row">
                        <td>Loss Constant</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_Dec_LossConstantPremium">$200</td>
                    </tr>--%>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_Dec_ExpenseConstantPremium_Row">
                        <td>Expense Constant</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_Dec_ExpenseConstantPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_TerrorismQuotedPremium_Row">
                        <td>Certified Acts of Terrorism Coverage</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_TerrorismQuotedPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_MinimumPremiumAdjustment_Row">
                        <td>Amount to Equal Minimum Premium<%-- - <span runat="server" id="wcp_MinimumQuotedPremium"></span>--%></td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_MinimumPremiumAdjustment">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_TotalQuotedPremium_Row">
                        <td>Total Estimated Written Premium</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_TotalQuotedPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_SecondInjuryFundQuotedPremium_Row">
                        <td>Indiana Second Injury Fund Surcharge</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_SecondInjuryFundQuotedPremium">$200</td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_KentuckySpecialFundAssessment_Row">
                        <td>Kentucky Special Fund Assessment</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_KentuckySpecialFundAssessmentPremiumColumn">
                            <asp:Label ID="lblWCPKYSpecialFundAssessmentPremium" runat="server"></asp:Label>
                        </td>
                    </tr>                    
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_KentuckySurcharge_Row">
                        <td>Kentucky Surcharge</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_KentuckySurchargePremiumColumn">
                            <asp:Label ID="lblWCPKentuckySurchargePremium" runat="server"></asp:Label>
                        </td>
                    </tr>                    
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="Tr11">
                        <td>&nbsp;</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell  qs_grid_3_columns" runat="server" visible="true" id="wcp_Dec_WC_TotalPremiumDue_Row">
                        <td>Total Premium Due</td>
                        <td></td>
                        <td class="qs_rightJustify" runat="server" id="wcp_Dec_WC_TotalPremiumDue">$200</td>
                    </tr>
                    

                    <%--' Close table--%>
                </table>
            </div>
        </div>

        

        <br />
        <%--<uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />--%>
        <br />
        <%--<asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>--%>

        <asp:HiddenField ID="HiddenField1" runat="server" />

    </div>
</div>

