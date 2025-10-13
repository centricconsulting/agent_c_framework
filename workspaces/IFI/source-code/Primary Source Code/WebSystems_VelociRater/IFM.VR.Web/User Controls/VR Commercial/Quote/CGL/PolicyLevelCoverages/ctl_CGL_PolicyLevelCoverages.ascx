<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_PolicyLevelCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/PolicyLevelCoverages/ctl_CGL_Coverages.ascx" TagPrefix="uc1" TagName="ctl_CGL_Coverages" %>
<%--<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ClassCode/ctl_CGL_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CGL_ClassCodeList" %>--%>

<uc1:ctl_CGL_Coverages runat="server" ID="ctl_CGL_Coverages" />
<%--<uc1:ctl_CGL_ClassCodeList runat="server" ID="ctl_CGL_ClassCodeList" />--%>

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 400px;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Policy Level Coverages and continues to Locations." Text="Save Policy Coverages" />
    <asp:Button ID="btnSaveAndGotoLocations" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Locations Page" /><br />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
    <asp:Button ID="btnViewGLLocations" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="GL Locations/Class Code" />
</div>
