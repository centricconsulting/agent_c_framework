<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FUPPUP_QuoteSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_FUPPUP_QuoteSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/SummaryControls/Summary4column.ascx" TagPrefix="uc1" TagName="Summary4column" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/SummaryControls/Summary3column.ascx" TagPrefix="uc1" TagName="Summary3column" %>



<style>
    .qs_basic_info_labels_cell {
        width: 160px;
    }

    .qs_Main_Sections {
        margin-top: 15px;
        width: 100%;
        page-break-inside: avoid;
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

    .CPPQSSpacer {
        width: 10px;
    }

    .UnderwriterWarningText {
        text-align: center;
        margin: 10px 50px;
        font-weight: bold;
        font-size: 13px;
    }
</style>

<div id="divUmbrellaQuoteSummary">
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
            <span class="qs_section_headers">Umbrella Coverages</span>
            <div class="qs_Sub_Sections">
                <table class="qa_table_shades">

                    <%-- Header Row --%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns">
                        <td style="width: 70%;">Coverage</td>
                        <td class="qs_rightJustify qs_padRight ">Limit</td>
                        <td class="qs_rightJustify qs_padRight ">Premium</td>
                    </tr>

                    <%-- Data Rows --%>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowUmbrella">
                        <td style="width: 70%;">Umbrella</td>
                        <td id="tdUmbrellaLimit" runat="server" class="qs_rightJustify qs_padRight"></td>
                        <td id="tdUmbrellaPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowUMUIM">
                        <td style="width: 70%;">UM/UIM</td>
                        <td id="tdUMUIMLimit" runat="server" class="qs_rightJustify qs_padRight"></td>
                        <td id="tdUMUIMPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowSelfInsuredRetention">
                        <td style="width: 70%;">Self Insured Retention</td>
                        <td id="tdSelfInsuredRetentionLimit" runat="server" class="qs_rightJustify qs_padRight"></td>
                        <td id="tdSelfInsuredRetentionPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                </table>
            </div>
        </div>
        <%--personal--%>
        <uc1:Summary4column runat="server" visible="false" id="perPLHomeCoverage"/>
        <uc1:Summary3column runat="server" visible="false" id="perRecreationalVehicle" />
        <uc1:Summary3column runat="server" visible="false" id="perWatercraft" />
        <uc1:Summary3column runat="server" visible="false" id="perMiscellaneous" />
        <uc1:Summary3column runat="server" visible="false" id="perInvestmentProperty" />
        <uc1:Summary3column runat="server" visible="false" id="perProfessionalLiability" />
        <uc1:Summary4column runat="server" visible="false" id="perPLDfrCoverage" />
        <uc1:Summary3column runat="server" visible="false" id="perAuto" />
        <%--farm--%>
        <uc1:Summary4column runat="server" visible="false" id="ctlPLFarmCoverage"/>
        <uc1:Summary3column runat="server" visible="false" id="ctlCustomFarmingCoverage" />
        <uc1:Summary3column runat="server" visible="false" id="ctlWatercraft" />
        <uc1:Summary3column runat="server" visible="false" id="ctlProfessionalLiability" />
        <uc1:Summary3column runat="server" visible="false" id="ctlStopGapCoverage" />
        <uc1:Summary4column runat="server" visible="false" id="ctlPLHomeCoverage" />
        <uc1:Summary3column runat="server" visible="false" id="ctlRecreationalVehicle" />
        <uc1:Summary3column runat="server" visible="false" id="ctlMiscellaneous" />
        <uc1:Summary3column runat="server" visible="false" id="ctlInvestmentProperty" />
        <uc1:Summary4column runat="server" visible="false" id="ctlPLDfrCoverage" />
        <uc1:Summary3column runat="server" visible="false" id="ctlAuto" />
        <uc1:Summary3column runat="server" visible="false" id="ctlCommercialAutoCoverage" />
        <uc1:Summary3column runat="server" visible="false" id="ctlWorkersCompCoverage" />

        <uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />

        <div class="UnderwriterWarningText">
            <span style="text-align:center;">Please contact your Underwriter if any of the above underlying policy information is incorrect.</span>
        </div>
        <div class="informationalText" >
            <asp:Label ID="lblUnderwriterReminder" runat="server" style="font-size:smaller;margin-left:25px;margin-right:25px;font-weight:bold" Visible="false">Reminder: Underwriting requires a fully completed and signed application to issue the Umbrella policy.</asp:Label>
        </div>
        <asp:HiddenField ID="HiddenField1" runat="server" />
    </div>
</div>