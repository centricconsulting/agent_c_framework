<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_PFSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_PFSummary" %>
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

<div id="divCPRQuoteSummary">
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
        </table>

        <div class="qs_Main_Sections">
            <span class="qs_section_headers">POLICY LEVEL COVERAGE OPTIONS</span>
            <div class="qs_Sub_Sections">
                <asp:Literal ID="tblPolicyCoverages" runat="server"></asp:Literal>
                <table class="qa_table_shades">

                    <%-- Header Row --%>
                    <tr class="qs_section_grid_headers ui-widget-header qs_cap_grid_3_columns">
                        <td>Coverage</td>
                        <td></td>
                        <td class="qs_rightJustify qs_padRight ">Premium</td>
                    </tr>

                    <%-- Data Rows --%>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowEnhEndo">
                        <td>Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdEnhEndoPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowPropPlusEnhEndo">
                        <td style="width:70%;">Property PLUS Enhancement Endorsement</td>
                        <td></td>
                        <td id="tdPropertyPlusEnhancementEndorsementPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowBlanket">
                        <td>Blanket Rating: <span runat="server" id="BlanketType">$BlanketType</span></td>
                        <td></td>
                        <td id="tdBlanketQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>
                    <tr class="qs_basic_info_labels_cell qs_cap_grid_3_columns" runat="server" visible="true" id="rowMenPremAdj">
                        <td>Minimum Premium Adjustment</td>
                        <td></td>
                        <td id="tdMenPremAdjQuotedPremium" runat="server" class="qs_rightJustify qs_padRight"></td>
                    </tr>

                </table>
            </div>
        </div>
        <uc1:PolicyDiscounts runat="server" ID="ctlPolicyDiscounts" />
        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLocations" runat="server"></asp:Literal>
        </div>

        <div class="qs_Main_Sections">
            <asp:Literal ID="tblLossHistory" runat="server"></asp:Literal>
        </div>

        <%--<uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />--%>

        <asp:HiddenField ID="HiddenField1" runat="server" />
    </div>
</div>
