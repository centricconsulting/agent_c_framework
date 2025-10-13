<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_Locations.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_Locations" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/ctl_BOP_ENDO_LocationList.ascx" TagPrefix="uc1" TagName="ctl_BOP_ENDO_LocationList" %>



<uc1:ctl_BOP_ENDO_LocationList runat="server" ID="ctl_BOP_ENDO_LocationList" />
<asp:HiddenField ID="hdnAccord" runat="server" />
