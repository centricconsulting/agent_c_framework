<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" EnableEventValidation="false" CodeBehind="PF_ContractorEquipment.aspx.vb" Inherits="IFM.VR.Web.PF_ContractorEquipment" %>
<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/PF_ScheduledConstEquipment/ctl_PF_ScheduledConstEquipment.ascx" TagPrefix="uc1" TagName="ctl_PF_ScheduledConstEquipment" %>



<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_PF_ScheduledConstEquipment runat="server" ID="ctl_PF_ScheduledConstEquipment" />
</asp:Content>
    
