<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_DFR_AppSection.ascx.vb" Inherits="IFM.VR.Web.ctl_DFR_AppSection" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholder" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_PropertyUpdates_HOM_App.ascx" TagPrefix="uc1" TagName="ctl_PropertyUpdates_HOM_App" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Employment_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Employment_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>






<div id="div_master_hom_app" runat="server">
    <h3>Application
    </h3>
    <div>
        <uc1:ctl_AppPolicyholder runat="server" ID="ctl_AppPolicyholder" />
        <uc1:ctl_PropertyUpdates_HOM_App runat="server" ID="ctl_PropertyUpdates_HOM_App" />
        <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
        <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
        <uc1:ctlAccidentHistoryList runat="server" DriverIndex="0" ID="ctlAccidentHistoryList" />
        <uc1:ctl_Employment_Info_PPA runat="server" ID="ctl_Employment_Info_PPA" />
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
        <uc1:ctl_Producer runat="server" ID="ctl_Producer" />
        <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_Carrier_PPA" />
        <uc1:ctl_App_Rate runat="server" ID="ctl_App_Rate" />
    </div>
</div>
<asp:HiddenField ID="hiddendiv_master_hom_app" runat="server" />