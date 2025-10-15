<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WCP_NamedIndividual.ascx.vb" Inherits="IFM.VR.Web.ctl_WCP_NamedIndividual" %>
<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="NI Type"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add new Named Individual" >Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Workplace" >Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div id="divWCPNamedIndividual" runat="server">
    <style type="text/css">
        .WCPNILabelColumn {
            width:20%;
            text-align:left;
        }
        .WCPNIDataColumn {
            width:70%;
            text-align:left;
        }
    </style>
    <table id="tblWCPNI" runat="server" style="width:100%;">
        <tr>
            <td class="WCPNILabelColumn">
                *Name
            </td>
            <td class="WCPNIDataColumn">
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr id="trTypeRow" runat="server">
            <td class="WCPNILabelColumn">
                *Type
            </td>
            <td class="WCPNIDataColumn">
                <asp:DropDownList ID="ddlType" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr id="trInfoRow" runat="server">
            <td colspan="2" class="informationalText">
                Sole Proprietors, Partners & LLC Members who elect to be included must provide written proof of health insurance coverage via the Upload tool in VelociRater or sent to your Underwriter.
            </td>
        </tr>
    </table>
    <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>    
    <asp:HiddenField ID="hdnNIType" runat="server" />
</div>

