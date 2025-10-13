<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDisclaimer.ascx.vb" Inherits="IFM.VR.Web.ctlDisclaimer" %>
<br />
<div class="tableField">
    <%--<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td align="left">
            <ul>
                <li>This quotation has been developed based on the information provided to Indiana Farmers Mutual Insurance Company and does not bind or provide actual coverage. It is subject to final underwriting approval, acceptable loss experience and favorable loss control inspection.</li>
                <li>This quotation is valid for 60 days, until <asp:Label runat="server" ID="lblValidQuoteDate"></asp:Label></li>
                <li>Additional coverages, exposures, or increased limits may be added for additional premium.</li>
            </ul>
        </td>
    </tr>
</table>--%>
    <div class="DisclaimerHolder">
        <div class="QuickQuoteProposalDisclaimer">
            <ul>
                <li>This quotation has been developed based on the information provided to Indiana Farmers Mutual Insurance Group and does not bind or provide actual coverage. It is subject to final underwriting approval, acceptable loss experience and favorable loss control inspection.</li>
                <li runat="server" ID="ExpireContainer" Visible="false"><asp:Label runat="server" ID="ExpireNotice"/></li>
                <li>Additional coverages, exposures, or increased limits may be added for additional premium.</li>
            </ul>
        </div>
    </div>
</div>
<br />