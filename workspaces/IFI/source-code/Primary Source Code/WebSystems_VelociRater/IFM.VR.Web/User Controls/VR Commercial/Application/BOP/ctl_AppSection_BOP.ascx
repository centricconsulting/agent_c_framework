<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppSection_BOP.ascx.vb" Inherits="IFM.VR.Web.ctl_AppSection_BOP" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalPolicyholderList.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholderList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_BOP_App_LocationList.ascx" TagPrefix="uc1" TagName="ctl_App_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalPolicyholderList.ascx" TagPrefix="uc1" TagName="ctl_AddlPolicyholderList" %>
<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVEHAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyHolder" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_CTEQList.ascx" TagPrefix="uc1" TagName="ctl_ContEQList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_PhotographersEquipment.ascx" TagPrefix="uc1" TagName="ctl_PhotographersEquipment" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_BOP_App_AdditionalServices.ascx" TagPrefix="uc1" TagName="ctl_BOP_App_AdditionalServices" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalInsureds.ascx" TagPrefix="uc1" TagName="ctl_App_AdditionalInsureds" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ClassCode/ctl_BOP_NaicsCode.ascx" TagPrefix="uc1" TagName="ctl_BOP_NaicsCode" %>




<div id="div_master_bop_app" runat="server">
    <h3>
        Application
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <uc1:ctl_AppPolicyholder runat="server" ID="ctl_App_Policyholder" />
        <uc1:ctl_AddlPolicyholderList runat="server" ID="ctl_AddlPolicyholderList" />
        <uc1:ctl_BOP_NaicsCode runat="server" ID="ctl_BOP_NaicsCode" />
        <uc1:ctlVEHAdditionalInterestList runat="server" ID="ctl_AdditionalInterestList" />
        <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
        <uc1:ctl_App_LocationList runat="server" ID="ctl_LocationList" />
        <uc1:ctl_App_AdditionalInsureds runat="server" ID="ctl_App_AdditionalInsureds" />
        <uc1:ctl_ContEQList runat="server" ID="ctl_ContractorsEQList" />
        <uc1:ctl_PhotographersEquipment runat="server" ID="ctl_PhotogEquip" />
        <uc1:ctl_BOP_App_AdditionalServices runat="server" ID="ctl_BOP_App_AdditionalServices" />
        <uc1:ctlAccidentHistoryList runat="server" ID="ctlAccidentHistoryList" />
        <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_CarrierPPA" />        
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
        <uc1:ctl_Producer runat="server" ID="ctlProducer" />
        <uc1:ctl_App_Rate runat="server" ID="ctlApp_Rate" />
    </div>
</div>
<asp:HiddenField ID="hiddendiv_master_bop_app" runat="server" />