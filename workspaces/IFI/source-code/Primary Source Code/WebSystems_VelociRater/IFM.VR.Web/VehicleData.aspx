<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VehicleData.aspx.vb" Inherits="IFM.VR.Web.VehicleData" %>
<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/PF_VehicleDataReport/ctl_PF_VehicleData.ascx" TagPrefix="uc2" TagName="ctl_PF_VehicleData" %>


<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc2:ctl_PF_VehicleData runat="server" ID="ctl_PF_VehicleData1" />
</asp:Content>
