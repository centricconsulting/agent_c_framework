<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDFROptionalCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlDFROptionalCoverages" %>

<div id="dvOptionalCoverages" runat="server">
    <div id="dvCoverageContainer" runat="server">
    <div id="dvACV" runat="server">
        <asp:CheckBox ID="chkACV" runat="server" />
        <asp:Label ID="lblACV" runat="server" Text=" Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing"></asp:Label>
    </div>
    <div id="dvEarthquake" runat="server">
        <asp:CheckBox ID="chkEarthquake" runat="server" />
        <asp:Label ID="lblEarthquake" runat="server" Text=" Earthquake"></asp:Label>
        <div id="dvEarthquakeDeduct" runat="server" style="display:none">
            <table style="width:100%">
                <tr>
                    <td style="width:25px"></td>
                    <td>
                        <asp:DropDownList ID="ddlEarthquakeDeduct" runat="server"></asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="dvReplacment" runat="server">
        <asp:CheckBox ID="chkReplacement" runat="server" />
        <asp:Label ID="lblReplacement" runat="server" Text=" Functional Replacement Cost Loss Settlement"></asp:Label>
    </div>
    <div id="dvMineSub" runat="server">
        <div id="dvMineSubA" runat="server">
            <asp:CheckBox ID="chkMineSubA" runat="server" />
            <asp:Label ID="lblMineSubA" runat="server" Text="Mine Subsidence Cov. A"></asp:Label>
        </div>
        <div id="dvMineSubAB" runat="server">
            <asp:CheckBox ID="chkMineSubAB" runat="server" />
            <asp:Label ID="lblMineSubAB" runat="server" Text="Mine Subsidence Cov. A & B"></asp:Label>
        </div>
    </div>
    <div id="dvSinkhole" runat="server">
        <asp:CheckBox ID="chkSinkhole" runat="server" />
        <asp:Label ID="lblSinkhole" runat="server" Text="Sinkhole Collapse"></asp:Label>
    </div>
    </div>
    <div id="dvOptionalButtons" runat="server">
        <table style="width: 100%">
            <tr>
                <td colspan="2"></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSaveOptionalCoverages" runat="server" Text="Save Optional Coverages" CssClass="StandardSaveButton" />
                </td>
                <td>
                    <asp:Button ID="btnRateOptional" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" />
                </td>
            </tr>
        </table>
    </div>
    <div id="dvEndorsementButtons" runat="server"><br />
        <table style="width: 100%">
            <tr>
                <td>
                    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" />
                </td>
                <td>
                    <asp:Button ID="btnViewGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Billing Information Page" />
                </td>
            </tr>
        </table>
    </div>
</div>