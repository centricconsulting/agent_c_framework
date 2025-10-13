<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDefaultUserCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlDefaultUserCoverages" %>
<%@ Register Src="~/User Controls/Configuration/PPA/ctlDefaultCoverageList.ascx" TagPrefix="uc1" TagName="ctlDefaultCoverageList" %>
<%@ Register Src="~/User Controls/Configuration/PPA/ctlDefaultCoverageRowHeaders.ascx" TagPrefix="uc1" TagName="ctlDefaultCoverageRowHeaders" %>

<table style="width: 100%; vertical-align: top">
    <tr>
        <td style="vertical-align: top; width: 10%; min-width: 10%; max-width: 10%">
            <table>
                <tr>
                    <td style="min-height: 49px; height: 49px">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 25%">
                                    <img alt="" src="images/VR-Search.png" />
                                </td>
                                <td>
                                    <span style="font-weight: bold;">Find My Defaults</span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%">
                        <table style="width: 100%">
                            <tr>
                                <td align="center" style="width: 100%">
                                    <asp:Label ID="lblDriver" class="label" runat="server" Text="Driver" Width="100%" BackColor="LightGray" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblAgeRange" runat="server" Text="Age Range"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddAgeRange" runat="server" AutoPostBack="true"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%">
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td align="center" style="width: 100%">
                                    <asp:Label ID="lblVehicle" runat="server" Text="Vehicle" Width="100%" BackColor="LightGray" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblOver7Years" runat="server" Text="Over 7 Years?"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButtonList ID="rdoOver7" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"></asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%">
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td align="center" style="width: 100%">
                                    <asp:Label ID="lblVehicleDrivers" runat="server" Text="Vehicles/Driver" Width="100%" BackColor="LightGray" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblMoreVehicles" runat="server" Text="More Vehicles?"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButtonList ID="rdoVehDrvr" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"></asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <td style="vertical-align: top; width: 60%">
            <table style="width: 100%">
                <tr>
                    <td style="min-width: 20%; width: 20%">
                        <uc1:ctlDefaultCoverageRowHeaders runat="server" ID="ctlDefaultCoverageRowHeaders" />
                    </td>
                    <td>
                        <uc1:ctlDefaultCoverageList runat="server" ID="ctlDefaultCoverageList" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td></td>
        <td>
            <asp:Button ID="btnSave" CssClass="StandardButton" runat="server" Text="Save" />
        </td>
    </tr>
</table>