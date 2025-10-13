<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRecentlyManufacturedVehicle_APP.ascx.vb" Inherits="IFM.VR.Web.ctlRecentlyManufacturedVehicle_APP" %>
<div runat="server" id="divNewVehicalPopup_APP" >
    <div align="center">
        <p>Optional coverage Loan/Lease Gap is automatically added to vehicles 5 years old and newer.</p>
    </div>
    <div id="NewVehicalButtons_APP" align="center">
        <asp:Button ID="btnOK" ToolTip="OK" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="OK" />
    </div>
</div>
