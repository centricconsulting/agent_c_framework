<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQuoteSummary_HOM.ascx.vb" Inherits="IFM.VR.Web.ctlQuoteSummary_HOM" %>
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

        .hom_qa_table_shades tr:nth-child(even) {
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
<div id="divHomeQuoteSummary">
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
                <col class="qs_hom_basic_info_labels_cell" />
            </colgroup>
            <tr>
                <td>Policy Holder Name</td>
                <td>
                    <asp:Label CssClass="qs_hom_basic_info_labels" ID="lblPhName" runat="server" Text="Label"></asp:Label>
                    
                </td>
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
                <td>Rated Tier</td>
                <td>
                    <asp:Label ID="lblRatedTier" runat="server" Text="Label"></asp:Label></td>
            </tr>
        </table>

        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">Applicants</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblApplicants" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">Location</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblLocations" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">Coverage Summary</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblCoverageSummary" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">Credits and Surcharges</span>
            <div class="hom_qs_Sub_Sections">
                <table style="width: 100%">
                    <tr style="vertical-align: top;">
                        <td style="width: 50%">
                            <asp:Literal ID="tblDiscounts" runat="server"></asp:Literal></td>
                        <td style="width: 50%">
                            <asp:Literal ID="tblSurcharges" runat="server"></asp:Literal></td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">Included Coverages</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblIncludedCoverages" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="hom_qs_Main_Sections">
            <span class="qs_section_headers">Optional Coverage Summary</span>
            <div class="hom_qs_Sub_Sections">
                <asp:Literal ID="tblOptionalCoverages" runat="server"></asp:Literal>
            </div>
        </div>

        <asp:Literal ID="tblInlandMarine" runat="server"></asp:Literal>

        <asp:Literal ID="tblRvWaterCraft" runat="server"></asp:Literal>

        <uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />
    </div>
</div>