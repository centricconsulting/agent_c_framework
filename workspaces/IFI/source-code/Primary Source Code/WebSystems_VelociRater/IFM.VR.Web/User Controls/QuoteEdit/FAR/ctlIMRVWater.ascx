<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlIMRVWater.ascx.vb" Inherits="IFM.VR.Web.ctlIMRVWater" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmInlandMarine.ascx" TagPrefix="uc1" TagName="ctlFarmInlandMarine" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/RV-Watercraft/ctlFarmRVWatercraftList.ascx" TagPrefix="uc1" TagName="ctlFarmRVWatercraftList" %>


<uc1:ctlfarminlandmarine runat="server" id="ctlFarmInlandMarine" />
<uc1:ctlfarmrvwatercraftlist runat="server" id="ctlFarmRVWatercraftList" />

<div align="center">
    <br />
    <asp:Button ID="btnRateLocation" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" style="height: 26px" />
</div>