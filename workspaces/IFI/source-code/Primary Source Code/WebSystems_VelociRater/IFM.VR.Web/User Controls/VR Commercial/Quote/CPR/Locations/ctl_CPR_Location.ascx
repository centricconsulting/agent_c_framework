<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_Location.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_Location" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctlProperty_Address" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_PropertyInOpenList.ascx" TagPrefix="uc1" TagName="ctl_CPR_PropertyList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/Buildings/ctl_CPR_BuildingList.ascx" TagPrefix="uc1" TagName="ctl_BuildingList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_LocationCoverages.ascx" TagPrefix="uc1" TagName="ctl_Coverages" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Location"></asp:Label>

    <span style="float: right;">
        <asp:LinkButton ID="lnkAdd" runat="server" ToolTip="Add Location" CssClass="RemovePanelLink">Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" CssClass="RemovePanelLink" ToolTip="Clear this Location" runat="server">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" CssClass="RemovePanelLink" ToolTip="Delete this Location" runat="server">Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkCopyLocation" CssClass="RemovePanelLink" ToolTip="Copy Location" runat="server">Copy</asp:LinkButton>
    </span>
</h3>

<div runat="server" id="divContents" class="parentLocation">
    <asp:Label ID="lblCPPMessage" runat="server" ForeColor="Red" Width="100%" style="text-align:center;display:none;" Text="Add all locations for both package parts here.  You will not be allowed to add/edit locations in the General Liability sections."></asp:Label>
    <uc1:ctlProperty_Address runat="server" ID="ctlProperty_Address" />  
    <uc1:ctl_Coverages runat="server" ID="ctl_LocationCoverages"></uc1:ctl_Coverages>
    <uc1:ctl_CPR_PropertyList runat="server" ID="Ctl_CPR_PIO"></uc1:ctl_CPR_PropertyList>
    <uc1:ctl_BuildingList runat="server" ID="ctl_CPR_BldgList"></uc1:ctl_BuildingList>
    <asp:Button ID="btnAddBuilding" runat="server" Text="Add a building to this location" CssClass="StandardSaveButton" />
</div>



