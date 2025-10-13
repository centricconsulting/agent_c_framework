<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppSection_CGL.ascx.vb" Inherits="IFM.VR.Web.ctl_AppSection_CGL" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyholder" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>


<div runat="server" id="divMain">
    <h3>Application
         <span style="float: right;">
             <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
         </span>
    </h3>
    <div>
       
        <uc1:ctl_AppPolicyholder runat="server" ID="ctl_AppPolicyholder" />
        <%--add additional pollicyholder ??? probably just need to open this ability on quote side --%>
        <%--Need Description of Operations--%>
        <%--<WorkPlace Addresses--%>
        <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
        <%--Loss History--%>
        <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_Carrier_PPA" />
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Producer runat="server" ID="ctl_Producer" />
    </div>
</div>

<asp:HiddenField ID="HiddenField1" runat="server" />