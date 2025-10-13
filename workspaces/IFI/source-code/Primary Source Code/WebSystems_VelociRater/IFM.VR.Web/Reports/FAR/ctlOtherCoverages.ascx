<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlOtherCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlOtherCoverages" %>

<table style="width: 100%" class="table">
    <tr>
        <td style="width: 50%">
            <asp:Label ID="lblOtherPropCov" runat="server"></asp:Label>
        </td>
        <td>
            <table style="width: 100%" class="table">
                <tr>
                    <td align="right" style="width: 65%">
                        <asp:Label ID="lblOtherLimit" runat="server"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblOtherPrem" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>

<asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true" Visible="false"></asp:Label>