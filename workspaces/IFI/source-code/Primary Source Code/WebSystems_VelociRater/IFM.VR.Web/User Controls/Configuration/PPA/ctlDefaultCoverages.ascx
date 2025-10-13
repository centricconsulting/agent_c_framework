<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDefaultCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlDefaultCoverages" %>

<div runat="server" id="divUserDefaultCoverage">
    <table style="width: 100%; vertical-align: top">
        <tr>
            <td align="center" style="min-height: 25px; height: 25px; width: 100%">
                <asp:Label ID="lblVehType" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
    </table>
    <div style="vertical-align: top">
        <table style="width: 100%; vertical-align: top; font-family: Calibri; font-size: small">
            <tr>
                <td style="vertical-align: top; width: 100%">
                    <asp:Panel ID="pnlCompData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlComp" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlCollData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlColl" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlSSLData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlSLL" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlBIData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlBI" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlPDData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlPD" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlMedicalData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlMedical" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUIMSLData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlUIMSL" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUIMBIData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlUIMBI" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUMPData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlUMPD" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlUMPDDedData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlUMPDDed" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlRentalData" runat="server" Height="33px">
                        <asp:DropDownList ID="ddlRental" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlTowingData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlTowing" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlTravelInterruptData" runat="server" Height="33px">
                        <asp:DropDownList ID="ddlTraveInterrupt" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlSoundData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlSoundEquip" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlAVEquipData" runat="server" Height="33px">
                        <asp:DropDownList ID="ddlAVEquip" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlMediaData" runat="server" Height="33px">
                        <asp:DropDownList ID="ddlMedia" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                    <hr />
                    <asp:Panel ID="pnlPollutionData" runat="server" Height="25px">
                        <asp:DropDownList ID="ddlPollution" runat="server" Width="90px"></asp:DropDownList>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hiddenDriverAssignment" Value="0" runat="server" />
</div>