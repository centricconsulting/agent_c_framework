<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_LocationCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_LocationCoverages" %>

<div runat="server" id="divCPRLocationCoverages">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text=""></asp:Label>

        <span style="float: right;">
            <asp:LinkButton ID="lnkClear" CssClass="RemovePanelLink" ToolTip="Clear the Location Coverages" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>

    <div runat="server" id="divContents">
        <table id="tblLocationPropertyDeductible" style="width: 100%;" runat="server">
            <tr id="trLocationPropertyDeductible" runat="server">
                <td style="text-align: right; width: 50%;">
                    <asp:Label ID="lblLocationPropertyDeductible" runat="server">Property Deductible</asp:Label>
                </td>
                <td style="text-align: left; width: 50%;">
                    <asp:DropDownList ID="ddLocationPropertyDeductible" runat="server" Style="width: 75%;"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td id="tdLocationPropertyDeductibleInfo" runat="server" colspan="2" class="informationalText" style="text-align: center;">The selected property deductible applies for all buildings, personal property, and property in the open at this location.
                </td>
            </tr>
        </table>
        <br />
        <table style="width:100%;">
            <tr id="tdWindHailDeductible" runat="server">
                <td style="text-align:right;width:50%;">
                    <asp:Label ID="lblWindHailDeductible" runat="server">Wind Hail Deductible %</asp:Label>
                </td>
                <td style="text-align:left;width:50%;">
                    <asp:DropDownList ID="ddWindHailDeductible" runat="server" style="width:75%;"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td id="tdWindHailDeductInfo" runat="server" colspan="2" class="informationalText" style="text-align:center;">
                    Wind/Hail deductibles are applied per building in the event of a loss.
                </td>
            </tr>
        </table>
        <br />
        <table style="width:100%;">
            <tr>
                <td style="text-align:right;width:50%;">
                    Equipment Breakdown
                </td>
                <td style="text-align:left;width:50%;">
                    <asp:CheckBox ID="chkEquipmentBreakdown" runat="server" />
                </td>
            </tr>
            <tr id="trEquipmentBreakDownInfoRow" runat="server" style="display:none;">
                <td colspan="2" style="color:red;">
                    Equipment Breakdown is not available for this risk.  If Equipment Breakdown is desired contact your underwriter once you have completed building the quote.
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
</div>
