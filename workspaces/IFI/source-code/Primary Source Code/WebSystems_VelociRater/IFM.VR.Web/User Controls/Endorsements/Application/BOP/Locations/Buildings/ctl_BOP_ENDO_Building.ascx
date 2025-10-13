<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_Building.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_Building" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_BOP_ENDO_Building_Information.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_Building_Information" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_BOP_ENDO_BuildingCoverages.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_BuildingCoverages" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_BOP_ENDO_App_Building.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_App_Building" %>
<%--<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_Endo_AppliedAdditionalInterestList.ascx" TagPrefix="uc2" TagName="ctl_Endo_AppliedAdditionalInterestList" %>--%>






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
    <uc2:ctl_BOP_ENDO_Building_Information runat="server" ID="ctl_BOP_ENDO_Building_Information" />    
    <uc2:ctl_BOP_ENDO_BuildingCoverages runat="server" ID="ctl_BOP_ENDO_BuildingCoverages" />    
    <uc2:ctl_BOP_ENDO_App_Building runat="server" ID="ctl_BOP_ENDO_App_Building" />
    <%--<uc2:ctl_Endo_AppliedAdditionalInterestList runat="server" id="ctl_Endo_AppliedAdditionalInterestList" />--%>
    <asp:Button ID="btnSave" runat="server" Text="Save Building" ToolTip="Save Building" CssClass="StandardSaveButton" />
</div>

