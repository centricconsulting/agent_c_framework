<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomAdditionalInterests.ascx.vb" Inherits="IFM.VR.Web.ctlHomAdditionalInterests" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInterestAddress.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInterestAddress" %>

<table style="width:100%">
    <tr>
        <td runat="server" id="tdName">
            <asp:Label ID="lblName" runat="server">Name:</asp:Label>
            <br />
            <asp:TextBox ID="txtName" Width="125" MaxLength="250" runat="server"></asp:TextBox>
        </td>
        <td runat="server" id="tdDescription">
            <asp:Label ID="lblDescription" runat="server">Description:</asp:Label>
            <br />
            <asp:TextBox ID="txtDescription" Width="150" MaxLength="250" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtAiId" ToolTip="This should be hidden. AI ID" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtIsEditable" Style="display: none;" ToolTip="This should be hidden. Was created this session." runat="server"></asp:TextBox>                            
            <asp:Label ID="lblExpanderText" runat="server" Text="Label"></asp:Label>
        </td>
        <td><asp:LinkButton ID="lnkAddAdress" runat="server">Add Address</asp:LinkButton></td>
        <td><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
<div runat="server" id="divAddress">
    <uc1:ctlHomAdditionalInterestAddress runat="server" id="ctlHomAdditionalInterestAddress" />
</div>