<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRecentManufacturedVehicle.ascx.vb" Inherits="IFM.VR.Web.ctlRecentManufacturedVehicle" %>

<style>
    #EndorsementVehicleSelector {
        display: none;
    }

</style>
<script>
$(document).ready(function () {

});
</script>
<div runat="server" id="divNewVehicalPopup">
    <div align="center">
        <%--<p>You have quoted a change on a vehicle recently manufactured. The existence or lack of a lien impacts rating.</p>--%>
        <p>Vehicle added with no lienholder.</p>
        <p>If you wish to continue without adding a lienholder select OK or select Cancel to add a lienholder.</p>
    </div>
    <div id="NewVehicalButtons" align="center">
        <asp:Button ID="btnOK" ToolTip="OK" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="OK" />
        <asp:Button ID="btnCancel" ToolTip="Cancel" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Cancel" />
    </div>
</div>