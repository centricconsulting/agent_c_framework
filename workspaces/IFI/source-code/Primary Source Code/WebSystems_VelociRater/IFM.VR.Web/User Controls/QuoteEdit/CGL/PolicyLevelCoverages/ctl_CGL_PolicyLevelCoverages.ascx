<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_PolicyLevelCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/PolicyLevelCoverages/ctl_CGL_GeneralInformation.ascx" TagPrefix="uc1" TagName="ctl_CGL_GeneralInformation" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/PolicyLevelCoverages/ctl_CGL_Coverages.ascx" TagPrefix="uc1" TagName="ctl_CGL_Coverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/ClassCode/ctl_CGL_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CGL_ClassCodeList" %>



<uc1:ctl_CGL_GeneralInformation runat="server" ID="ctl_CGL_GeneralInformation" />

<uc1:ctl_CGL_Coverages runat="server" ID="ctl_CGL_Coverages" />
<uc1:ctl_CGL_ClassCodeList runat="server" ID="ctl_CGL_ClassCodeList" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 400px;">
            <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Policy Level Coberages and continues to Locations." Text="Save Policy Coverages" />
            <asp:Button ID="btnSaveAndGotoLocations" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Locations Page" /><br />
        </div>
