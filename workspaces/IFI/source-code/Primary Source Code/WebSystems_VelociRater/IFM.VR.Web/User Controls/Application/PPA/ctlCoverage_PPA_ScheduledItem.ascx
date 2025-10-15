<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCoverage_PPA_ScheduledItem.ascx.vb" Inherits="IFM.VR.Web.ctlCoverage_PPA_ScheduledItem" %>

<h3>
    <asp:Label ID="lblAccordTitle" runat="server" Text="Scheduled Item"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" runat="server">Save</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);var confirmed = confirm('Remove this Custom Equipment Item?');if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" runat="server">Remove</asp:LinkButton></span>
</h3>
<div>
    <table style="width: 100%">
        <tr>
            <td>*Equipment Type
                <br />
                <asp:DropDownList ID="ddAdditionalEquipment" runat="server"></asp:DropDownList></td>
            <td>*Equipment Amount<br />
                <asp:TextBox ID="txtAmount" onblur='$(this).val(FormatAsCurrencyNoCents($(this).val(),""));' runat="server" MaxLength="7"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="2">*Description<br />
                <asp:TextBox ID="txtCoverageDescription" TextMode="MultiLine" runat="server" Height="99px" Width="100%"></asp:TextBox></td>
            <td></td>
        </tr>
    </table>
</div>