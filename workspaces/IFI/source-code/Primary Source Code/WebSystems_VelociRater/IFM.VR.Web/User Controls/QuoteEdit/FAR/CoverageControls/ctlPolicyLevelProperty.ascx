<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPolicyLevelProperty.ascx.vb" Inherits="IFM.VR.Web.ctlPolicyLevelProperty" %>

<div id="dvFarmPolicyPropertyCoverage" runat="server">
    <h3>
        <asp:Label ID="lblPropertyHdr" runat="server" Text="Property"></asp:Label>
        <span style="float: right">
            <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" OnClientClick="var confirmed = confirm('Clear Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Property to Default Values" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all" runat="server">Save</asp:LinkButton>            
        </span>
    </h3>
    <div>
        <asp:Panel ID="pnlPolicyLiability" runat="server">
            <div id="dvFarmAllStar" runat="server">
                <asp:CheckBox ID="chkFarmAllStar" runat="server" />&nbsp;
                <asp:Label ID="lblFarmAllStar" runat="server" Text="Farm All Star"></asp:Label>
                <div id="dvBackSewerDrain" runat="server" style="display:none">
                    <table style="width:100%">
                        <tr>
                            <td></td>
                            <td>
                                <asp:Label ID="lblBackSewerDrain" runat="server" Text="Backup of Sewer or Drain"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>
                                <asp:Label ID="lblBSDLimit" runat="server" Text="Limit"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlBSDLimit" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvEquipBreak" runat="server">
                    <asp:CheckBox ID="chkEquipBreak" runat="server" />&nbsp;
                    <asp:Label ID="lblEquipBreak" runat="server" Text="Equipment Breakdown"></asp:Label>
                </div>
                <div id="dvPollution" runat="server">
                    <asp:CheckBox ID="chkPollution" runat="server" />&nbsp;
                    <asp:Label ID="lblPollution" runat="server" Text="Pollutant Clean Up and Removal - Increased Limits"></asp:Label>
                </div>
            </div>
        </asp:Panel>
    </div>
    <asp:HiddenField ID="hiddenFarmPropertyCoverage" runat="server" />
</div>