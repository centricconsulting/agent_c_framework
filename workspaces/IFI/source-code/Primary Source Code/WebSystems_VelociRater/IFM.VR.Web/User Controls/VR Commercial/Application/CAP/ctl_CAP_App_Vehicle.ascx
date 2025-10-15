<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_App_Vehicle.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_App_Vehicle" %>

<div id="divCAP" runat="server">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Vehicle #"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
    <table style="width: 100%">
        <tr id="trVinMessage" runat="server">
            <td colspan="3" style="width:100%; text-align:center;font-weight:bold;" class="informationalText">
                Adding or changes to the VIN may return data that will adjust your class code 
                based on the correct size and cost. The premium will be adjusted accordingly.
            </td>
        </tr>
        <tr>
            <td class="CAPVEHColumn1">
                <label for="<%=Me.txtVinNumber.ClientID%>">*VIN:</label>
                <br />
                <asp:TextBox ID="txtVinNumber" runat="server" MaxLength="30" ></asp:TextBox>
            </td>
            <td class="CAPVEHLabelColumn2">
                <label for="<%=Me.ddlLossPayeeName.ClientID%>">Loss/Payee Name:</label>
            </td>
            <td class="CAPVEHDataColumn2">
                <asp:DropDownList ID="ddlLossPayeeName" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="CAPVEHColumn1">
                &nbsp;
            </td>
            <td class="CAPVEHLabelColumn2">
                <label for="<%=Me.ddlLossPayeeType.ClientID%>">Loss/Payee Type:</label>
            </td>
            <td class="CAPVEHDataColumn2">
                <asp:DropDownList ID="ddlLossPayeeType" runat="server">                   
                    <asp:ListItem Value="64">CA 20 01 Additional Interest</asp:ListItem>
                    <asp:ListItem Value="76">CA 99 44 Loss Payable</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="CAPVEHColumn1">
                &nbsp;
            </td>
            <td class="CAPVEHLabelColumn2">
                <label for="<%=Me.ddlATIMA.ClientID%>">ATIMA:</label>
            </td>
            <td class="CAPVEHDataColumn2">
                <asp:DropDownList ID="ddlATIMA" runat="server">
                    <asp:ListItem Value="0">None</asp:ListItem>
                    <asp:ListItem Value="1">ATIMA</asp:ListItem>
                    <asp:ListItem Value="2">ISAOA</asp:ListItem>
                    <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <%--VIN Lookup Validation Popup--%>
    <div id="divVinLookupValidation" runat="server" style="display: none;">
        <div>
            <div style="font-weight:bold;"> Errors: </div>
            <br />
            VIN could not be validated. Please confirm.
        </div>

        <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
            <asp:Button ID="btnVLVOK" CssClass="StandardButton" runat="server" Text="OK" />
        </div>
    </div>
</div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnVehicleNum" runat="server" />
    <asp:HiddenField ID="hdnVehicleYear" runat="server" />
    <asp:HiddenField ID="hdnVehicleMake" runat="server" />
    <asp:HiddenField ID="hdnVehicleModel" runat="server" />
    <asp:HiddenField ID="hdnVehicleSize" runat="server" />
    <asp:HiddenField ID="hdnVehicleCostNew" runat="server" />
    <asp:HiddenField ID="hdnVehicleClassCode" runat="server" />
    <asp:HiddenField ID="hdnRateType" runat="server" />
    <asp:HiddenField ID="hdnUseCode" runat="server" />
    <asp:HiddenField ID="hdnOperator" runat="server" />
    <asp:HiddenField ID="hdnOperatorType" runat="server" />
    <asp:HiddenField ID="hdnTrailerType" runat="server" />
    <asp:HiddenField ID="hdnRadius" runat="server" />
    <asp:HiddenField ID="hdnSecClass" runat="server" />
    <asp:HiddenField ID="hdnSecClassType" runat="server" />
    <asp:HiddenField ID="hdnValidVin" runat="server" />
    <asp:HiddenField ID="hdnOtherThanCollisionSymbol" runat="server" />
    <asp:HiddenField ID="hdnCollisionSymbol" runat="server" />
    <asp:HiddenField ID="hdnLiabilitySymbol" runat="server" />
    <asp:HiddenField ID="hdnOtherThanCollisionOverride" runat="server" />
    <asp:HiddenField ID="hdnCollisionOverride" runat="server" />
    <asp:HiddenField ID="hdnLiabilityOverride" runat="server" />
</div>
