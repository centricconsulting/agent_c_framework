<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppSection_Farm.ascx.vb" Inherits="IFM.VR.Web.ctl_AppSection_Farm" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctl_PolicyholderFarm_AppSide.ascx" TagPrefix="uc1" TagName="ctl_PolicyholderFarm_AppSide" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_LocationsList_App.ascx" TagPrefix="uc1" TagName="ctl_Farm_LocationsList_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctl_Farm_Remarks.ascx" TagPrefix="uc1" TagName="ctl_Farm_Remarks" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/ctl_FarmPolicyCoverage_AppSide.ascx" TagPrefix="uc1" TagName="ctl_FarmPolicyCoverage_AppSide" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmPersonalProperty.ascx" TagPrefix="uc1" TagName="ctlFarmPersonalProperty" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIMRVWatercraft.ascx" TagPrefix="uc1" TagName="ctlIMRVWatercraft" %>
<%--<%@ Register Src="~/User Controls/Application/HOM/ctl_RVwatercraft_HOM_App.ascx" TagPrefix="uc1" TagName="ctl_RVwatercraft_HOM_App" %>--%>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholder" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>


<div runat="server" id="divMain">
    <h3>Application
         <span style="float: right;">
             <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
         </span>
    </h3>
    <div>
        <uc1:ctl_AppPolicyholder runat="server" ID="ctl_AppPolicyholder" />
        <%--<uc1:ctl_PolicyholderFarm_AppSide runat="server" ID="ctl_PolicyholderFarm_AppSide" />--%>
        <uc1:ctl_FarmPolicyCoverage_AppSide runat="server" ID="ctl_FarmPolicyCoverage_AppSide" />
        <uc1:ctl_Farm_LocationsList_App runat="server" ID="ctl_Farm_LocationsList_App" />
        <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
        <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
        <uc1:ctlFarmPersonalProperty runat="server" ID="ctlFarmPersonalProperty" />
        <div runat="server" id="divIM_RV">
            <h3>IM/RV/Watercraft</h3>
            <div>
                <uc1:ctlIMRVWatercraft runat="server" ID="ctlIMRVWatercraft" />
            </div>
        </div>
        <asp:HiddenField ID="hdnIM_RV" runat="server" />

        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
        <%-- <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_Carrier_PPA" />--%>
        <uc1:ctl_Producer runat="server" ID="ctl_Producer" />
        <uc1:ctl_Farm_Remarks runat="server" ID="ctl_Farm_Remarks" />       
        <uc1:ctl_App_Rate runat="server" ID="ctl_App_Rate" />
    </div>
</div>

<asp:HiddenField ID="HiddenField1" runat="server" />