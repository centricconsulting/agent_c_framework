<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmLocationCoverage.ascx.vb" Inherits="IFM.VR.Web.ctlFarmLocationCoverage" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmLocationList.ascx" TagPrefix="uc1" TagName="ctlFarmLocationList" %>

<uc1:ctlFarmLocationList runat="server" ID="ctlFarmLocationList" />

<asp:HiddenField ID="hiddenActiveLocation" runat="server" Value="false" />