<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WCP_Location.ascx.vb" Inherits="IFM.VR.Web.ctl_WCP_Location" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctlProperty_Address" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Location #0 - "></asp:Label>
        <span style="float: right;">
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add New Location">Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Location" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Location Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <uc1:ctlProperty_Address runat="server" id="ctlProperty_Addresss" />
</div>
