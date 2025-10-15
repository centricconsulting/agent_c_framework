<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDefaultCoverageRowHeaders.ascx.vb" Inherits="IFM.VR.Web.ctlDefaultCoverageRowHeaders" %>

<div runat="server" id="divUserDefaultCoverageHeader">
    <table style="width: 100%; vertical-align: top">
        <tr>
            <td style="min-height: 20px; height: 20px; width: 100%; vertical-align: top">
                <asp:Label ID="lblType" runat="server" Text="Vehicle Type" Font-Bold="True"></asp:Label>
            </td>
        </tr>
    </table>
    <div style="vertical-align: top">
        <table style="width: 100%; vertical-align: top; font-family: Calibri; font-size: small">
            <tr>
                <td id="CoverageHeaderRow" runat="server" style="vertical-align: top">
                    <asp:Panel ID="pnlComp" runat="server" Height="25px">
                        <asp:Label ID="lblComp" class="label" runat="server" Text="Comp Deduct"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlColl" runat="server" Height="25px">
                        <asp:Label ID="lblColl" class="label" runat="server" Text="Coll Deduct"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlSSL" runat="server" Height="25px">
                        <asp:Label ID="lblSLL" class="label" runat="server" Text="Single Limit Liability"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlBodily" runat="server" Height="25px">
                        <asp:Label ID="lblBodily" class="label" runat="server" Text="Bodily Injury"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlProperty" runat="server" Height="25px">
                        <asp:Label ID="lblProperty" class="label" runat="server" Text="Property Damage"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlMP" runat="server" Height="25px">
                        <asp:Label ID="lblMedical" class="label" runat="server" Text="MP"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUMUIMSL" runat="server" Height="25px">
                        <asp:Label ID="lblUMUIMSL" class="label" runat="server" Text="UM/UIM SL"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUMUIMBI" runat="server" Height="25px">
                        <asp:Label ID="lblUMUIMBI" class="label" runat="server" Text="UM/UIM BI"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUMPD" runat="server" Height="25px">
                        <asp:Label ID="lblUMPD" class="label" runat="server" Text="UM PD"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUMPDDed" runat="server" Height="25px">
                        <asp:Label ID="lblUMPDDed" class="label" runat="server" Text="UM PD Ded"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlRental" runat="server" Height="33px">
                        <asp:Label ID="lblRental" class="label" runat="server" Text="Transportation Expense"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlTowing" runat="server" Height="25px">
                        <asp:Label ID="lblTowing" class="label" runat="server" Text="Towing"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlTravelInterrupt" runat="server" Height="33px">
                        <asp:Label ID="lblTravelInterrupt" runat="server" Text="Interruption of Travel"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlSoundEquip" runat="server" Height="25px">
                        <asp:Label ID="lblSoundEquip" runat="server" Text="Sound Equipment"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlDataEquip" runat="server" Height="33px">
                        <asp:Label ID="lblDataEquip" runat="server" Text="Audio/Visual/Data Equipment"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlMedia" runat="server" Height="33px">
                        <asp:Label ID="lblMedia" runat="server" Text="Tapes,Records,Discs and other Media"></asp:Label>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlPollution" runat="server" Height="25px">
                        <asp:Label ID="lblPollution" runat="server" Text="Pollution Liability"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hiddenDriverAssignment" Value="0" runat="server" />
</div>