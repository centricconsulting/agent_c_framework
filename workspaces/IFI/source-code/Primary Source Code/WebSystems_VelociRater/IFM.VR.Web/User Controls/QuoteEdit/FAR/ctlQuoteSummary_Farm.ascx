<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQuoteSummary_Farm.ascx.vb" Inherits="IFM.VR.Web.ctlQuoteSummary_Farm" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>

<style>
    .qs_hom_basic_info_labels_cell {
        width: 160px;
    }

    .hom_qs_Main_Sections {
        margin-top: 15px;
        width: 100%;
    }

    .hom_qs_Sub_Sections {
        margin-left: 0px;
        width: 100%;
    }

    .hom_qa_table_shades {
        width: 100%;
        border-collapse: collapse;
    }

        .hom_qa_table_shades tr:nth-child(even):not(.ui-widget-header) {
            background-color: #dce6f1;
        }

    .qs_hom_section_grid_headers {
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
</style>

<asp:HiddenField ID="HiddenField1" runat="server" />
<div id="divFarmQuoteSummary">
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
        <%--GENERAL INFO SECTION--%>
        <table style="width: 100%;">
            <colgroup>
                <col class="qs_hom_basic_info_labels_cell" />
            </colgroup>
            <tr>
                <td>Policyholder Name</td>
                <td>
                    <asp:Label CssClass="qs_hom_basic_info_labels" ID="lblPhName" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr id="trCareOf" runat="server" visible="false">
                <td>Other: </td>
                <td>
                    <asp:Label CssClass="qs_hom_basic_info_labels" ID="lblCareOf" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Policyholder Address</td>
                <td>
                    <asp:Label ID="lblPhAddress" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblQuoteNo" runat="server" Text="Quote Number"></asp:Label></td>
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
                <td>Rated Tier</td>
                <td>
                    <asp:Label ID="lblRatedTier" runat="server" Text="Label"></asp:Label></td>
            </tr>
        </table>

        <%--APPLICANTS--%>
        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">APPLICANTS</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblApplicants" runat="server"></asp:Literal>
            </div>
        </div>

        <%--LIABILITY AND MED PAY--%>
        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">LIABILITY COVERAGE LIMITS AND PREMIUMS</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblLandM" runat="server"></asp:Literal>
            </div>
        </div>

        <%--LOCATION SUMMARY (APP ONLY) --%>
        <div id="divLocationSummary" class="hom_qs_Main_Sections" runat="server" visible="false">
            <span class="qs_section_headers">LOCATION SUMMARY</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblLocationSummary" runat="server"></asp:Literal>
            </div>
        </div>

        <%--PRIMARY DWELLING--%>
        <div id="divPrimaryDwelling" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">PRIMARY DWELLING</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblPrimaryDwelling" runat="server"></asp:Literal>
            </div>
        </div>
        <%-- PRIMARY DWELLING COVERAGES--%>
        <div id="divPrimaryDwellingCoverages" runat="server">
            <span style="font-weight: 700;">Coverages</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblCoverageSummary" runat="server"></asp:Literal>
            </div>
        </div>

        <%--ADDITIONAL DWELLINGS--%>
        <div id="divAdditionalDwellings" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">ADDITIONAL DWELLINGS</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblAdditionalDwellings" runat="server"></asp:Literal>
            </div>
        </div>

        <%--BARNS AND BULDINGS--%>
        <div id="divBarnsAndBuildings" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">BARNS AND BUILDINGS</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblBarnsAndBuildings" runat="server"></asp:Literal>
            </div>
        </div>

        <%--FARM PERSONAL PROPERTY--%>
        <div id="divFarmPersonalProperty" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">FARM PERSONAL PROPERTY</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblFarmPersonalProperty" runat="server"></asp:Literal>
            </div>
        </div>

         <%--FARM INCIDENTAL LIMITS--%>
        <div id="divFarmIncidentalLimits" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">FARM INCIDENTAL LIMITS</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblFarmIncidentalLimits" runat="server"></asp:Literal>
            </div>
        </div>

        <%--INLAND MARINE--%>
        <div id="divInlandMarine" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">INLAND MARINE</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblInlandMarine" runat="server"></asp:Literal>
            </div>
        </div>

        <%--RV WATERCRAFT--%>
        <div id="divRVWatercraft" runat="server" class="hom_qs_Main_Sections">
            <span class="qs_section_headers">RV AND WATERCRAFT</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblRVWatercraft" runat="server"></asp:Literal>
            </div>
        </div>

        <%--ADDITIONAL COVERAGES--%>
        <div class="hom_qs_Main_Sections">
            <%--            <span class="qs_section_headers">LIABILITY COVERAGE LIMITS AND PREMIUMS</span>--%>
            <span class="qs_section_headers">ADDITIONAL COVERAGES</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblAdditionalCoverages" runat="server"></asp:Literal>
            </div>
        </div>

        <%--CREDITS AND SURCHARGES--%>
        <div class="hom_qs_Main_Sections" id="creditsAndSurchargesSection" runat="server">
            <span class="qs_section_headers">CREDITS AND SURCHARGES</span>
            <div class="hom_qs_Sub_Sections">
                <table style="width: 100%;">
                    <tr style="vertical-align: top;">
                        <td style="width: 50%;">
                            <asp:Literal ID="tblDiscounts" runat="server"></asp:Literal>
                        </td>
                        <td style="width: 50%;">
                            <asp:Literal ID="tblSurcharges" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <%--THE ACTION BUTTONS--%>
        <uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />

        <br />

        <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
    </div>
</div>
