<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_PolicyLevelCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/PolicyLevelCoverages/ctl_CPR_Coverages.ascx" TagPrefix="uc1" TagName="ctl_CPR_Coverages" %>
<%--<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CPR_ClassCodeList" %>--%>

<uc1:ctl_CPR_Coverages runat="server" ID="ctl_CPR_Coverages" />
<%--<uc1:ctl_CPR_ClassCodeList runat="server" ID="ctl_CPR_ClassCodeList" />--%>

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 400px;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Policy Level Coberages and continues to Locations." Text="Save Policy Coverages" />
    <asp:Button ID="btnSaveAndGotoLocations" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Locations Page" /><br />
</div>
