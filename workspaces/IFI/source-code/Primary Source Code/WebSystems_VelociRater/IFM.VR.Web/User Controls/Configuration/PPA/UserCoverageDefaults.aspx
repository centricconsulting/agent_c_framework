<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="UserCoverageDefaults.aspx.vb" Inherits="IFM.VR.Web.UserCoverageDefaults" %>

<%@ Register Src="~/User Controls/Configuration/PPA/ctlDefaultUserCoverages.ascx" TagPrefix="uc1" TagName="ctlDefaultUserCoverages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctlDefaultUserCoverages runat="server" ID="ctlDefaultUserCoverages" />
</asp:Content>