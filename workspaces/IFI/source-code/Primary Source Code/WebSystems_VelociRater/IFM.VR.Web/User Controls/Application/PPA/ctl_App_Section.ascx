<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_Section.ascx.vb" Inherits="IFM.VR.Web.ctl_App_Section" %>

<%@ Register Src="~/User Controls/Application/PPA/ctl_VehicleList_App.ascx" TagPrefix="uc1" TagName="ctl_VehicleList_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_DriverList_App.ascx" TagPrefix="uc1" TagName="ctl_DriverList_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Employment_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Employment_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholder" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>


<uc1:ctl_AppPolicyholder runat="server" ID="ctl_AppPolicyholder" />
<uc1:ctl_DriverList_App runat="server" ID="ctl_DriverList_App" />
<uc1:ctl_VehicleList_App runat="server" ID="ctl_VehicleList_App" />
<uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
<uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
<uc1:ctl_Producer runat="server" ID="ctl_Producer" />
<uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_Carrier_PPA" />
<uc1:ctl_App_Rate runat="server" ID="ctl_App_Rate" />