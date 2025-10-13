<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_WindHail.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_WindHail" %>

<div runat="server" id="divWindHail">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Wind Hail Deductible"></asp:Label>

        <span style="float: right;">
            <asp:LinkButton ID="lnkClear" CssClass="RemovePanelLink" ToolTip="Clear the Wind/Hail Deductible" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>

    <div runat="server" id="divContents">
        <table>
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
    </div>
</div>
<asp:HiddenField ID="hdnAccord" runat="server" />

