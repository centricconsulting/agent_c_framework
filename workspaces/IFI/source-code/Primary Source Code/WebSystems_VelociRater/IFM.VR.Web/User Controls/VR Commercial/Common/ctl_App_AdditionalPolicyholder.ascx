<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_AdditionalPolicyholder.ascx.vb" Inherits="IFM.VR.Web.ctl_App_AdditionalPolicyholder" %>

<div id="divAddlPH" runat="server"  style="text-align:left;">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Additional Policyholder #0 - "></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkRemove" runat="server" CssClass="RemovePanelLink" ToolTip="Remove">Remove</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <table style="width:100%;">
        <tr>
            <td style="width:25%;text-align:left;" >
                <label for="<%=ddTaxIdType.ClientID%>">*Tax ID Type</label>
                <br />
                <asp:DropDownList Width="70%" ID="ddTaxIdType" runat="server">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="FEIN" Value="2"></asp:ListItem>
                    <asp:ListItem Text="SSN" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width:25%;text-align:left;">
                <label id="lblSSN" runat="server" style="display:none;" for="<%=txtSSN.ClientID%>">*TIN</label>
                <label id="lblFEIN" runat="server" style="display:none;" for="<%=txtFEIN.ClientID%>">*TIN</label>
                <br />
                <asp:TextBox Width="70%" ID="txtSSN" runat="server" style="display:none;"></asp:TextBox>
                <asp:TextBox Width="70%" ID="txtFEIN" runat="server" style="display:none;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width:25%;text-align:left;">
                <label for="<%=txtName.ClientID%>">*Name</label>
                <br />
                <asp:TextBox Width="70%" ID="txtName" runat="server"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <asp:HiddenField ID="accordActive" runat="server" />
</div>
