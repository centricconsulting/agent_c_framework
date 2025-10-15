<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlEndorsementRemoveVehicle.ascx.vb" Inherits="IFM.VR.Web.ctlEndorsementRemoveVehicle" %>

<script>
    function checkWarning() {
        if ($('select[id*="ddlRemoveVehicleSelect"]').val() == "-1") {
            $('[id*="EndorsementRemoveWarning"]').show();
            $('#EndorsementRemoveButtons input[id*="btnRemove"]').prop('disabled', true);;
 
        }
        else {
            $('[id*="EndorsementRemoveWarning"]').hide();
            $('#EndorsementRemoveButtons input[id*="btnRemove"]').prop('disabled', false);;
        }
    }

    $(document).ready(function () {
        checkWarning();
        $('select[id*="ddlRemoveVehicleSelect"]').focus();
        $('select[id*="ddlRemoveVehicleSelect"]').on('change', function () {
            checkWarning();
        });
});
</script>
<style>
    div[id*="divEndorsementRemoveVehiclePopup"] div {
        margin-top: 15px;
    }

</style>
<div runat="server" id="divEndorsementRemoveVehiclePopup">
    <div align="center">
        <div id="EndorsementVehicleSelector">
            <asp:Label ID="lblWhichVehicle" runat="server" Text="Label">Which Vehicle are you removing?</asp:Label><br />
            <asp:DropDownList ID="ddlRemoveVehicleSelect" runat="server" CssClass="EndorsmentDropDownList"></asp:DropDownList><br />
        </div>
    </div>
    <div id="EndorsementRemoveButtons" align="center">
        <asp:Button ID="btnCancel" ToolTip="Cancel" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Cancel" />
        <asp:Button ID="btnRemove" ToolTip="Remove" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Remove" />
    </div>
    <div id="EndorsementRemoveWarning" align="center">
        <asp:Label ID="lblWarning" runat="server" Text="Label" CssClass="informationalTextRed">A vehicle must be selected.</asp:Label>
    </div>
</div>

