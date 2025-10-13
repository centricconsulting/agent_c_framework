<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_PFSummary.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_PFSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlQuoteSummaryActions.ascx" TagPrefix="uc1" TagName="ctlQuoteSummaryActions" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CAP/ctl_vehicle_item.ascx" TagPrefix="uc1" TagName="ctl_vehicle_item" %>
<%@ Register Src="~/Reports/ctlPFHeader.ascx" TagPrefix="uc1" TagName="ctlPFHeader" %>

<style>
    .qs_cap_basic_info_labels_cell {
        width: 160px;
    }

    .cap_qs_Main_Sections {
        margin-top: 15px;
        width: 100%;
    }

    .cap_qs_Sub_Sections {
        margin-left: 0px;
        width: 100%;
    }

    .cap_qa_table_shades {
        width: 100%;
        border-collapse: collapse;
    }

    .cap_qa_table_shades tr:nth-child(even) {
            /*background-color: #dce6f1;*/
        }

    .qs_cap_section_grid_headers {
        border: 1px solid black;
        text-align: left;
        font-weight: bold;
        /*background-color: #f4cb8f;*/
    }

    .qs_cap_section_headers {
        font-weight: bold;
        font-size: 13px;
    }

    .qs_cap_section_header_double_hieght {
        min-height: 40px;
    }

    .qs_cap_Grid_cell_premium {
        width: 70px;
        max-width: 70px;
    }

    .qs_cap_Grid_Total_Row {
        border-top: 2px solid black;
    }

    .qs_cap_grid_4_columns td{
        width: 25%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_cap_grid_3_columns td{
        width: 15%;
        /*min-width: 25%;
        max-width: 25%;*/
        padding-right: 5px;
    }

    .qs_cap_grid_3_columns td:first-child{
        width: 70%;
        /*min-width: 25%;
        max-width: 25%;*/
    }

    .qs_cap_indent td {
        padding-left: 1em;
    }

    #divCAPQuoteSummary tr {
        width: 100%;
        line-height: 1.5;
    }
    .qs_cap_grid_4_columns .ThreeHeaders td:nth-child(3), .qs_cap_grid_4_columns.3Title td:nth-child(4){
        padding-right: 15%;
    }
</style>

<div id="divCAPQuoteSummary">
    <h3 id="quoteSummaryHeader" runat="server">
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
    <div id="EndorsementPrintSection" runat="server" width="100%" style="background-color:#FFFFFF;">
        <uc1:ctlPFHeader runat="server" Visible="false" id="ctlEndorsementOrChangeHeader"></uc1:ctlPFHeader>               
    </div>
    <div >
        <table style="width: 100%;" id="quoteSummarySection" runat="server">
            <colgroup>
                <col class="qs_cap_basic_info_labels_cell" />
            </colgroup>
            <tr>
                <td>Policy Holder Name</td>
                <td>
                    <asp:Label CssClass="qs_cap_basic_info_labels" ID="lblPhName" runat="server" Text="Label"></asp:Label></td>
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

        <div class="cap_qs_Main_Sections">
            <span class="qs_cap_section_headers">POLICY LEVEL COVERAGE OPTIONS</span>
            <asp:Literal ID="tblPolicyCoverages" runat="server"></asp:Literal>
        </div>

        <div class="cap_qs_Main_Sections">
            <span class="qs_cap_section_headers">OPTIONAL COVERAGES </span>
            <asp:Literal ID="tblOptionalCoverages" runat="server"></asp:Literal>
        </div>

        <div class="cap_qs_Main_Sections">
            <span class="qs_cap_section_headers">POLICY DISCOUNTS</span>
            <asp:Literal ID="tblPolicyDiscounts" runat="server"></asp:Literal>
        </div>


        <%--Vehicle control--%>
        <div class="cap_qs_Main_Sections">
            <uc1:ctl_vehicle_item runat="server" id="ctl_vehicle_item" />
        </div>

        <div class="cap_qs_Main_Sections">
            <asp:Literal ID="tblLossHistory" runat="server"></asp:Literal>
        </div>

        <br />
        <%--<uc1:ctlQuoteSummaryActions runat="server" ID="ctlQuoteSummaryActions" />
        <br />--%>
        <%--<asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>--%>

        <asp:HiddenField ID="HiddenField1" runat="server" />

    </div>
</div>
