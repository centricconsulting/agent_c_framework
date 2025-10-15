<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlVinLookup.ascx.vb" Inherits="IFM.VR.Web.ctlVinLookup" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#vinLookupPopupContent").dialog({
            title: "Vehicle Information Lookup",
            width: 600,
            autoOpen: false,
            dialogClass: "no-close",
            draggable: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $('.divSearchVehicleLookupContents').html('');
                    $('.divSearchVehicleLookup').hide();
                    $("#vinLookupPopupContent").dialog("close");
                }
            }
        });
        //sets the cancel button color coding
        $(".ui-dialog-buttonpane button span").addClass("StandardButtonVinLookup");
    });
</script>
<div id="vinLookupPopupContent" style="display: none;" class="ctlVehicle_PPA_container">
    <asp:Panel runat="server" ID="pnlBasicInfo" DefaultButton="btnSearchVinLookup">
        <table style="width: 100%" title="Basic Information">
            <tr>
                <td style="width: 260px;">
                    <label for="<% =Me.txtSearchVinNumber.ClientID%>">*VIN:</label>
                    <br />
                    <asp:TextBox ID="txtSearchVinNumber" CssClass="txtSearchVehicleVin" TabIndex="1" runat="server" MaxLength="30" autofocus></asp:TextBox>
                </td>
                <td rowspan="4" align="center">
                    <asp:Button ID="btnSearchVinLookup" TabIndex="5" runat="server" ToolTip="VIN or Year/Make/Model Lookup" CssClass="StandardButton btnVehiclePopupLookup" Text="Lookup" Width="73px" />
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddSearchYear.ClientID%>">*Year:</label>
                    <br />
                    <asp:DropDownList ID="ddSearchYear" CssClass="ddSearchVehicleYear" TabIndex="2" runat="server"></asp:DropDownList>
                    <asp:HiddenField ID="hdnSearchYear" runat="server" />
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddSearchMake.ClientID%>">*Make:</label>
                    <br />
                    <asp:DropDownList ID="ddSearchMake" CssClass="ddSearchVehicleMake" TabIndex="3" runat="server"></asp:DropDownList>
                    <asp:HiddenField ID="hdnSearchMake" runat="server" />
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddSearchModel.ClientID%>">*Model:</label>
                    <br />
                    <asp:DropDownList ID="ddSearchModel" CssClass="ddSearchVehicleModel" TabIndex="4" runat="server"></asp:DropDownList>
                    <asp:HiddenField ID="hdnSearchModel" runat="server" />
                </td>
                <td></td>
            </tr>
        </table>
    </asp:Panel>

    <div runat="server" id="divSearchVinLookup" class="divSearchVehicleLookup" style="margin-top: 30px; width: 100%; display: none;">
        <h3><span style="float: right;"><a href="#" onclick="StopEventPropagation(event); $(this).parent().parent().parent().hide();">Close</a></span>Vin/Model Lookup</h3>
        <div runat="server" style="overflow-x: auto; max-height: 220px;" id="divSearchVinLookupContents" class="divSearchVehicleLookupContents">
        </div>
    </div>

    <asp:HiddenField ID="hiddenSearchVinLookup" Value="0" runat="server" />
    <asp:HiddenField ID="hdnVehicleIndex" runat="server" />
    <asp:HiddenField ID="HiddenSearchLookupWasFired" Value="0" runat="server" />
    <asp:HiddenField ID="HiddenParentLookupWasFired" Value="0" runat="server" />
</div>