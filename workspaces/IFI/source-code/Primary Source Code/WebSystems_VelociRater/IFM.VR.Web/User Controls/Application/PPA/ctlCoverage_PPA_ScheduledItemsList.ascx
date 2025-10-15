<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCoverage_PPA_ScheduledItemsList.ascx.vb" Inherits="IFM.VR.Web.ctlCoverage_PPA_ScheduledItemsList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlCoverage_PPA_ScheduledItem.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA_ScheduledItem" %>

<div runat="server" id="divScheduledItemsList" class="standardSubSection">
    <h3>
        <asp:Label ID="lblAccordTitle" runat="server" Text="Custom Equipment"></asp:Label>
        <span style="float: right">
            <asp:LinkButton ID="lnkBtnAdd" ToolTip="Add a Scheduled Item" runat="server">Add Scheduled Item</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" ToolTip="Save all scheduled items" runat="server">Save</asp:LinkButton></span>
    </h3>
    <div>
        <table style="width: 100%">
            <tr>
                <td>Additional Scheduled Amount:</td>
                <td>
                    <asp:TextBox ID="txtCustomEquipmentAmount" Enabled="false" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2">
                    <div id="divScheduledItems" runat="server">
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <uc1:ctlCoverage_PPA_ScheduledItem runat="server" ID="ctlCoverage_PPA_ScheduledItem" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </td>
            </tr>
            <tr>
                <td>Amount Remaining:
                </td>
                <td>
                    <asp:TextBox ID="txtRemaining" Enabled="false" runat="server"></asp:TextBox></td>
            </tr>
        </table>
    </div>
</div>

<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="HiddenFieldMainAccord" runat="server" />