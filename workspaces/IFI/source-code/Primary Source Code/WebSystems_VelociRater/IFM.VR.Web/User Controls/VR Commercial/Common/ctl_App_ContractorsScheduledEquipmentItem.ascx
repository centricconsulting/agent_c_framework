<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_ContractorsScheduledEquipmentItem.ascx.vb" Inherits="IFM.VR.Web.ctl_App_ContractorsScheduledEquipmentItem" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Item #"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Remove Item" >Remove</asp:LinkButton>
    </span>
</h3>

<div>
    <style type="text/css">
        .ContEqColumn {
            width:50%;
            text-align:left;
        }
        .ContLabelColumn {
            width:30%;
            text-align:right;
        }
        .ContDataColumn {
            width:30%;
            text-align:left;
        }
    </style>
    <table id="tblContractorsItems" runat="server" style="width:100%;">
        <tr id="trContractorItemInputRow" runat="server">
            <td colspan="2">
                <table ID="tblContractorItemFields" runat="server" Style="width:100%;">
                    <tr>
                        <td class="ContEqColumn">*Valuation Method</td>
                        <td class="ContEqColumn">*Limit</td>
                    </tr>
                    <tr>
                        <td class="ContEqColumn">
                            <asp:DropDownList ID="ddlValuationMethod" runat="server" Width="75%">
                                <asp:ListItem Value="1">Replacement Cost</asp:ListItem>
                                <asp:ListItem Value="2">Actual Cash Value</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="ContEqColumn">
                            <asp:TextBox ID="txtLimit" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align:left;">*Description</td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="200px" Width="95%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="align-content:center;">
                        <td class="ContLabelColumn">Loss Payee Name</td>
                        <td class="ContDataColumn">
                            <asp:DropDownList ID="ddlLossPayeeName" runat="server" Width="60%"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="align-content:center;">
                        <td class="ContLabelColumn">Loss Payee Type</td>
                        <td class="ContDataColumn">
                            <asp:DropDownList ID="ddlLossPayeeType" runat="server" Width="60%">
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="align-content:center;">
                        <td class="ContLabelColumn">ATIMA</td>
                        <td class="ContDataColumn">
                            <asp:DropDownList ID="ddlATIMA" runat="server" Width="60%">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
    </div>
