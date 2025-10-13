<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_LocationCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_LocationCoverages" %>

<div runat="server" id="divCPRLocationCoverages">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Optional Location Coverages"></asp:Label>

        <span style="float: right;">
            <asp:LinkButton ID="lnkClear" CssClass="RemovePanelLink" ToolTip="Clear the Location Coverages" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>

    <div runat="server" id="divContents">
        <table style="width:100%;">
            <tr>
                <td style="text-align:right;width:50%;">
                    Wind Hail Deductible %
                </td>
                <td style="text-align:left;width:50%;">
                    <asp:DropDownList ID="ddWindHailDeductible" runat="server" style="width:75%;"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="informationalText" style="text-align:center;">
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
        </table>
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
</div>
