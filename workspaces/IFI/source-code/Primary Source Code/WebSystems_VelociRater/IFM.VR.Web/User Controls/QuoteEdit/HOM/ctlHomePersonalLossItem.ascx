<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomePersonalLossItem.ascx.vb" Inherits="IFM.VR.Web.ctlHomePersonalLossItem" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save all Loss Histories" runat="server">Save</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Remove this Loss History Item" runat="server">Remove</asp:LinkButton>
    </span>
</h3>
<div id="LossHistoryItemDiv">
    <asp:Panel ID="pnlLossHistoryItem" runat="server">
        <table>
            <tr>
                <td>
                    <asp:Label ID="label1" runat="server" Text="Claim Number"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtClaimNumber" runat="server" Width="200px" TabIndex="1"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label2" runat="server" Text="Loss Date"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtLossDate" runat="server" Width="100px" TabIndex="2"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label3" runat="server" Text="Type of Loss"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlTypeOfLoss" runat="server" Width="200px" TabIndex="3"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label4" runat="server" Text="Paid Amount"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtPaidAmount" runat="server" Width="200px" TabIndex="4"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label5" runat="server" Text="Reserve Amount"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtReserveAmount" runat="server" Width="200px" TabIndex="5"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label6" runat="server" Text="Surcharge"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSurcharge" runat="server" Width="200px" TabIndex="6"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label7" runat="server" Text="Source"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSource" runat="server" Width="200px" TabIndex="7"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label8" runat="server" Text="Catastrophic"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkCatastrophic" runat="server" Text="&nbsp;" TabIndex="8"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label9" runat="server" Text="Description"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDescription" runat="server" Width="300px" TextMode="MultiLine" Height="40px" TabIndex="9"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label10" runat="server" Text="Fault Indicator"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlFaultIndicator" runat="server" Width="200px" TabIndex="10"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label11" runat="server" Text="Weather-Related"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkWeatherRelated" runat="server" Text="&nbsp;" TabIndex="11"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label12" runat="server" Text="Comments"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtComments" runat="server" Width="300px" TextMode="MultiLine" Height="40px" TabIndex="12"></asp:TextBox>
                </td>
            </tr>
        </table>
        <div align="center">
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
        </div>
    </asp:Panel>
</div>