<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_LocationsWF.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_LocationsWF" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/Locations/ctl_CGL_LocationList.ascx" TagPrefix="uc1" TagName="ctl_CGL_LocationList" %>


<uc1:ctl_CGL_LocationList runat="server" ID="ctl_CGL_LocationList" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 400px;">
            <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Locations" Text="Save Locations" />
            <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate" /><br />
        </div>