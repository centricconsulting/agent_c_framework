<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppSection_fuppup.ascx.vb" Inherits="IFM.VR.Web.ctl_AppSection_fuppup" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholder" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctl_Farm_Remarks.ascx" TagPrefix="uc1" TagName="ctl_Farm_Remarks" %>




<div runat="server" id="divMain">
    <h3>Application
         <span style="float: right;">
             <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
         </span>
    </h3>
    <div>
        <uc1:ctl_AppPolicyholder runat="server" ID="ctl_AppPolicyholder" />
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
        <uc1:ctl_Producer runat="server" ID="ctl_Producer" />
        <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_Carrier_PPA" />
        <uc1:ctl_Farm_Remarks runat="server" ID="ctl_Farm_Remarks" />
        <uc1:ctl_App_Rate runat="server" ID="ctl_App_Rate" />
    </div>
</div>

<asp:HiddenField ID="HiddenField1" runat="server" />