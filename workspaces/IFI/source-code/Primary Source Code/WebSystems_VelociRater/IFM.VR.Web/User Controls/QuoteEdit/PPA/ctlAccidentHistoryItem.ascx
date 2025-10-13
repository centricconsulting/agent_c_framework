<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlAccidentHistoryItem.ascx.vb" Inherits="IFM.VR.Web.ctlAccidentHistoryItem" %>

<h3>
    <asp:Label ID="lblAccordianHeader" runat="server" Text="Label"></asp:Label><span style="float: right">
        <asp:LinkButton ID="lnkRemove" OnClientClick="StopEventPropagation(event);var confirmed = confirm('Remove Loss Item?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" runat="server">Remove</asp:LinkButton></span></h3>
<div id="divLossHistoriesContent">
    <table style="width: 100%;">
        <tr>
            <td>
                <label for="<%=Me.ddLostType.ClientID%>">*Type of Loss</label></td>
            <td>
                <asp:DropDownList ID="ddLostType" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <td>
                <label for="<%=Me.txtLossDate.ClientID%>">*Loss Date</label></td>
            <td>
                <asp:TextBox ID="txtLossDate" MaxLength="10" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>
                <label for="<%=Me.txtAmountOfLoss.ClientID%>">*Loss Amount</label></td>
            <td>
                <asp:TextBox ID="txtAmountOfLoss" onkeyup='$(this).val(FormatAsCurrencyCheckMaxLength($(this)));' MaxLength="10" onblur='$(this).val(FormatAsCurrencyNoCents($(this).val()));' runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr runat="server" id="trFaultRow">
            <td>
                <label for="<%=Me.ddFaultIndicator.ClientID%>">*Fault Indicator</label></td>
            <td>
                <asp:DropDownList ID="ddFaultIndicator" runat="server"></asp:DropDownList></td>
        </tr>
        <tr runat="server" id="trLossDescription">
            <td>
                <label for="<%=Me.txtLossDescription.ClientID%>">*Loss Description</label>
            </td>
            <td>
                <asp:TextBox ID="txtLossDescription" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>