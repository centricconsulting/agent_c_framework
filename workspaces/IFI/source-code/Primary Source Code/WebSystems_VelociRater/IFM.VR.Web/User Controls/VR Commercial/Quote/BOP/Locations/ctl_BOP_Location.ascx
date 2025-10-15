<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_Location.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_Location" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/ctl_BOP_Location_Coverages.ascx" TagPrefix="uc1" TagName="ctl_BOP_Location_Coverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctlProperty_Address" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/Buildings/ctl_BOP_BuildingList.ascx" TagPrefix="uc1" TagName="ctl_BOP_BuildingList" %>



<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Location #0 - "></asp:Label>
        <span style="float: right;">
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add New Location">Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Location" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Location Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>

<div runat="server" id="divContents">
    <uc1:ctlProperty_Address runat="server" id="ctlProperty_Address" />
    <uc1:ctl_BOP_Location_Coverages runat="server" id="ctl_BOP_Location_Coverages" />
    <uc1:ctl_BOP_BuildingList runat="server" id="ctl_BOP_BuildingList" />

    <asp:Button ID="btnAddBuilding" runat="server" Text="Add a building to this location" CssClass="StandardSaveButton" />

</div>

