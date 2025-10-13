<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmIncidentalLimits.ascx.vb" Inherits="IFM.VR.Web.ctlFarmIncidentalLimits" %>

<div id="dvPersPropList" runat="server" class="div">
    <table id="tblPersPropHdr" runat="server" style="width: 100%;" class="table">
        <tr style="vertical-align: bottom">
            <td style="width: 50%">
                <asp:Label ID="lblPropCoverage" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
            </td>
            <td>
                <table style="width: 100%" class="table">
                    <tr style="vertical-align: bottom">
                        <td align="right" style="width: 54%;">
                            <asp:Label ID="lblLimits" runat="server" Text="Limit" Font-Bold="true"></asp:Label>
                        </td>
                        <td align="right" style="width: 23%;">
                            <asp:Label ID="lblPropLimit" runat="server" Text="Deductible" Font-Bold="true"></asp:Label>
                        </td>
                        <td align="right" style="width: 23%;">
                            <asp:Label ID="lblPropPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Literal ID="tblFarmIncidentalLimits" runat="server"></asp:Literal>
    <hr style="border-color: black" />
    <table id="tblTotalPrem" runat="server" style="width: 100%" class="table">
        <tr>
            <td>
                <asp:Label ID="lblTotalPrem" runat="server" Text="Total Farm Incidental Limits Premium"></asp:Label>
            </td>
            <td align="right">
                <asp:Label ID="lblTotalPremData" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</div>