<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_Building.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_Building" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/Buildings/ctl_BOP_Building_Information.ascx" TagPrefix="uc1" TagName="ctl_BOP_Building_Information" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/Buildings/ctl_BOP_BuildingCoverages.ascx" TagPrefix="uc1" TagName="ctl_BOP_BuildingCoverages" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_BOP_ENDO_App_Building.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_App_Building" %>


<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Building - "></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add New Building">Add New</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Delete Building" >Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Building Information">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>

<div>  
    <uc1:ctl_BOP_Building_Information runat="server"  id="ctl_BOP_Building_Information" />    
    <uc1:ctl_BOP_BuildingCoverages runat="server" id="ctl_BOP_BuildingCoverages" />    
    <uc2:ctl_BOP_ENDO_App_Building runat="server" ID="ctl_BOP_ENDO_App_Building" />
    <asp:Button ID="btnSave" runat="server" Text="Save Building" ToolTip="Save Building" CssClass="StandardSaveButton" />
</div>

