<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FarBuilding.ascx.vb" Inherits="IFM.VR.Web.ctl_FarBuilding" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/FarmBuildingControls/ctl_FarmBuilding_Property.ascx" TagPrefix="uc1" TagName="ctl_FarmBuilding_Property" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/FarmBuildingControls/ctl_FarmBuilding_Coverages.ascx" TagPrefix="uc1" TagName="ctl_FarmBuilding_Coverages" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Building"></asp:Label>

    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Building" CssClass="RemovePanelLink">Add Building</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Delete this Building" runat="server">Delete</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div runat="server" id="divContents">
    <uc1:ctl_FarmBuilding_Property runat="server" ID="ctl_FarmBuilding_Property" />
    <uc1:ctl_FarmBuilding_Coverages runat="server" ID="ctl_FarmBuilding_Coverages" />
    <div id="divAddButton" runat="server" style="margin-top:10px;margin-left:20px;">
    <asp:Button ID="btnSubmit"  OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Add Another Building" />
        </div>
    <asp:HiddenField ID="isNewBuilding" runat="server" />
</div>

