<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppSection_CGL.ascx.vb" Inherits="IFM.VR.Web.ctl_AppSection_CGL" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalPolicyholderList.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholderList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%--<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_BOP_App_LocationList.ascx" TagPrefix="uc1" TagName="ctl_App_LocationList" %>--%>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalPolicyholderList.ascx" TagPrefix="uc1" TagName="ctl_AddlPolicyholderList" %>
<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyHolder" %>
<%--<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalInsureds.ascx" TagPrefix="uc1" TagName="ctl_App_AdditionalInsureds" %>--%>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Locations_displayAddressList.ascx" TagPrefix="uc1" TagName="ctl_Locations_displayAddressList" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CGL/ctl_App_AdditionalInsureds_CGL.ascx" TagPrefix="uc1" TagName="ctl_App_AdditionalInsureds" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>




<div runat="server" id="divMain">
    <h3>Application
         <span style="float: right;">
             <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
         </span>
    </h3>
    <div>
        <uc1:ctl_AppPolicyholder runat="server" ID="ctl_App_Policyholder" />
        <uc1:ctl_AddlPolicyholderList runat="server" ID="ctl_AddlPolicyholderList" />
        <uc1:ctl_Locations_displayAddressList runat="server" id="ctl_Locations_displayAddress" />
        <uc1:ctl_App_AdditionalInsureds runat="server" ID="ctl_App_AdditionalInsureds" />
        <uc1:ctlAccidentHistoryList runat="server" ID="ctlAccidentHistoryList" />
        <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_CarrierPPA" />        
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
        <uc1:ctl_Producer runat="server" ID="ctl_Producer" />
        <uc1:ctl_App_Rate runat="server" ID="ctlApp_Rate" />


    </div>
</div>

<asp:HiddenField ID="HiddenField1" runat="server" />

<asp:HiddenField ID="hdn_master_cgl_app" runat="server" />
