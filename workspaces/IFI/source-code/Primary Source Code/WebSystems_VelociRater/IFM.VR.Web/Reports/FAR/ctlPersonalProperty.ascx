<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPersonalProperty.ascx.vb" Inherits="IFM.VR.Web.ctlPersonalProperty" %>
<%@ Register Src="~/Reports/FAR/ctlPeakSeasons.ascx" TagPrefix="uc1" TagName="ctlPeakSeasons" %>

<table id="tblPersPropDetail" runat="server" style="width:100%;margin:0px;border-collapse:collapse;" class="table">
    <tr>
        <td style="width: 250px; min-width:250px;vertical-align:top;">
            <asp:Label ID="lblCoverage" runat="server"></asp:Label>
        </td>
        <td id="tdDescription" runat="server" style="width: 250px; min-width:250px;vertical-align:top;">
            <asp:Label ID="lblCoverageDesc" runat="server" Width="100%"></asp:Label>
        </td>
        <td id="tdEarthquake" runat="server" style="width: 110px; min-width:110px;vertical-align:top;">
            <asp:Label ID="lblEarthquake" runat="server"></asp:Label>
        </td>
        <td align="right" style="width: 100px;vertical-align:top;min-width:100px;text-align:right;">
            <asp:Label ID="lblCoverageLimit" runat="server"></asp:Label>
        </td>
        <td align="right" style="width: 100px;vertical-align:top; min-width:100px;text-align:right;">
            <asp:Label ID="lblDeductible" runat="server"></asp:Label>
        </td>
        <td align="right" style="width: 113px; vertical-align:top;text-align:right;">
            <asp:Label ID="lblCoveragePrem" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<uc1:ctlPeakSeasons runat="server" ID="ctlPeakSeasons" />