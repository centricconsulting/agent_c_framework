<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_QuoteSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_QuoteSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>

<style>
    .qs_bop_basic_info_labels_cell {
        width: 160px;
    }

    .bop_qs_Main_Sections {
        margin-top: 15px;
        width: 100%;
    }

    .bop_qs_Sub_Sections {
        margin-left: 0px;
        width: 100%;
    }

    .bop_qa_table_shades {
        width: 100%;
        border-collapse: collapse;
    }

        .bop_qa_table_shades tr:nth-child(even) {
            background-color: #dce6f1;
        }

    .qs_bop_section_grid_headers {
        /*border: 1px solid black;*/
        text-align: left;
        font-weight: bold;
        /*background-color: #f4cb8f;*/
    }

    .qs_bop_section_headers {
        font-weight: bold;
        font-size: 13px;
    }

    .qs_bop_section_header_double_hieght {
        min-height: 40px;
    }

    .qs_bop_Grid_cell_premium {
        text-align: right;
        width: 70px;
        max-width: 70px;
    }

    .qs_bop_Grid_Total_Row {
        border-top: 2px solid black;
    }
</style>

<div id="divBOPQuoteSummary">
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
                <col class="qs_bop_basic_info_labels_cell" />
            </colgroup>
            <tr>
                <td>Policy Holder Name</td>
                <td>
                    <asp:Label CssClass="qs_bop_basic_info_labels" ID="lblPhName" runat="server" Text="Label"></asp:Label></td>
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

        <div class="bop_qs_Main_Sections">
            <span class="qs_bop_section_headers">LIABILITY COVERAGE LIMITS AND PREMIUMS</span>
            <div class="bop_qs_Sub_Sections">
                <asp:Literal ID="tblLiabilityInfo" runat="server"></asp:Literal>
            </div>
        </div>

        <div class="bop_qs_Main_Sections">
            <span class="qs_bop_section_headers">POLICY LEVEL COVERAGE OPTIONS</span>
            <div class="bop_qs_Sub_Sections">
                <asp:Literal ID="tblPolicyCoverages" runat="server"></asp:Literal>
            </div>
        </div>
        <div class="cap_qs_Main_Sections">
            <span class="qs_bop_section_headers">POLICY DISCOUNTS</span>
            <asp:Literal ID="tblPolicyDiscounts" runat="server"></asp:Literal>
        </div>
        <div class="bop_qs_Main_Sections">
            <asp:Literal ID="tblLocationInfo" runat="server"></asp:Literal>
        </div>

        <div class="bop_qs_Main_Sections">
            <asp:Literal ID="tblLossHistory" runat="server"></asp:Literal>
        </div>

        <br />
        <uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />
        <br />
        <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>

        <asp:HiddenField ID="HiddenField1" runat="server" />
        <div style="text-align:center">
            <asp:Label ID="lblPrintReminder" runat="server" CssClass="informationalText" Visible="false" Text="Reminder:  After submitting your application, access to policy documents, including the ACORD application, will not be available until the policy is issued."></asp:Label>
        </div>
    </div>
</div>
