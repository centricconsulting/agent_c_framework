<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_PaymentOptions_Actual.ascx.vb" Inherits="controls_Proposal_VRProposal_PaymentOptions_Actual" %>

<br />
<p width="100%" align="center" class="tableRowHeader">
    Payment Schedule
</p>
<asp:Panel runat="server" ID="pnlPaymentOptions" Visible="false">
    <asp:Repeater runat="server" ID="rptPaymentOptions">
        <HeaderTemplate>
            <table width="100%" class="quickQuoteSectionTable">
                <tr style="border-bottom: 1px solid black;">
                    <td class="tableFieldHeader">
                        <b>Description</b>
                    </td>
                    <td class="tableFieldHeader">
                        <b>Down Payment</b>
                    </td>
                    <td class="tableFieldHeader">
                        <b># of Installments</b>
                    </td>
                    <td class="tableFieldHeader">
                        <b>Installment Amount</b>
                    </td>
                    <td class="tableFieldHeader">
                        <b>Installment Charge</b>
                    </td>
                    <td class="tableFieldHeader">
                        <b>Total Amount of Installment</b>
                    </td>
                    <td class="tableFieldHeader">
                        <b>Due Every</b>
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblDescription" Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblDownPayment" Text='<%# DataBinder.Eval(Container.DataItem, "DownPayment") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblNumInstalls" Text='<%# DataBinder.Eval(Container.DataItem, "NumInstalls") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblInstallAmt" Text='<%# DataBinder.Eval(Container.DataItem, "InstallAmt") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblInstallChg" Text='<%# DataBinder.Eval(Container.DataItem, "InstallChg") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblTotalInstallAmt" Text='<%# DataBinder.Eval(Container.DataItem, "TotalInstallAmt") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="lblDueEvery" Text='<%# DataBinder.Eval(Container.DataItem, "DueEvery") %>'></asp:Label>
                    </td>
                    <td class="tableFieldValue">
                        <asp:Label runat="server" ID="Label7"></asp:Label>
                    </td>
                </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Panel>