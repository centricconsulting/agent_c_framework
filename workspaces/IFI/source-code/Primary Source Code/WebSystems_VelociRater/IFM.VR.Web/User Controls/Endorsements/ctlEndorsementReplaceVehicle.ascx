<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlEndorsementReplaceVehicle.ascx.vb" Inherits="IFM.VR.Web.ctlEndorsementReplaceVehicle" %>

<style>
    #EndorsementVehicleSelector {
        display: none;
    }

</style>
<script>
$(document).ready(function () {
    $("input[id*='rdoCoverageList_0'], input[id*='rdoCoverageList_1']").on('click', function () {
        $('#EndorsementVehicleSelector').show()
        $('select[id*="ddlReplaceVehicleSelect"').focus()
    });
});
</script>
<div runat="server" id="divEndorsementReplaceVehiclePopup">
    <div align="center">
        <asp:RadioButtonList ID="rdoCoverageList" runat="server" RepeatLayout="Table"  TextAlign="right" CssClass="EndorsmentRadioButtonList"/>
        <div id="EndorsementVehicleSelector">
            <asp:Label ID="lblWhichVehicle" runat="server" Text="Label">Which vehicle are you replacing?</asp:Label><br />
            <asp:DropDownList ID="ddlReplaceVehicleSelect" runat="server" CssClass="EndorsmentDropDownList"></asp:DropDownList>
        </div>
    </div>
    <div id="EndorsementReplaceButtons" align="center">
        <asp:Button ID="btnCancel" ToolTip="Cancel Replace" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Cancel" />
        <asp:Button ID="btnReplace" ToolTip="Replace Vehicle" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Replace" />
    </div>
</div>
