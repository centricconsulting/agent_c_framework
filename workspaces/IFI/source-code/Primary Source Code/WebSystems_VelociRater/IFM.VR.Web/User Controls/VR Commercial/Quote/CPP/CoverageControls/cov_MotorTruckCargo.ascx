<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_MotorTruckCargo.ascx.vb" Inherits="IFM.VR.Web.cov_MotorTruckCargo" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_MotorTruckCargo_Item.ascx" TagPrefix="uc1" TagName="cov_MotorTruckCargo_Item" %>

<table id="mtSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table; width: 100%;">
    <tr id="trMotorTruckCargoRiskOption" runat="server">
        <td>
            <div id="divMotorTruckCargoRiskOption" runat="server">
                <asp:CheckBox ID="chkTruckCargo" runat="server" class="chkOption motortruckoption" Text="Motor Truck Cargo - Scheduled**" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text="" Style="display: none" />
            </div>
        </td>
    </tr>
    <tr id="trMotorTruckCargoRiskDetail" runat="server">
        <td>
            <div id="divMotorTruckCargoRiskDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="tblMotorTruckCargo" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>*Cargo Description</td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            <asp:DropDownList ID="mtDeductible" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" Text='' class="form13Em"></asp:TextBox>
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <table class="qs_grid_4_columns">
                                <uc1:cov_MotorTruckCargo_Item runat="server" ID="cov_MotorTruckCargo_Item" />
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">Catastrophe Limit  
                            <asp:TextBox ID="txtCatLimit" runat="server" Text='' class="form13Em" Enabled="false"></asp:TextBox>
                        </td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="padding-right: 2em; text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum of $100,000. Maximum radius is 150 miles.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>
<%--  UnScheduled Section--%>
<table id="mtUnSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table; width: 100%;">
    <tr id="trUnScheduledMotorTruckCargoRiskOption" runat="server">
        <td>
            <div id="divUnScheduledMotorTruckCargoRiskOption">
                <asp:CheckBox ID="chkUnscheduledTruckCargo" runat="server" class="chkOption motortruckoption" Text="Motor Truck Cargo**" />
                <asp:Button ID="clearUnScheduledButton" class="hiddenclearbutton" runat="server" Text="" Style="display: none" />
            </div>
        </td>
    </tr>
    <tr id="trUnScheduledMotorTruckCargoRiskDetail" runat="server">
        <td>
            <div id="divUnScheduledMotorTruckCargoRiskDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="tblUnScheduledMotorTruckCargo" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>*Cargo Description</td>
                        <td>*Limit per Vehicle</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            <asp:DropDownList ID="mtUnScheduledDeductible" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtUnScheduledDescription" TextMode="MultiLine" runat="server" Text='' class="form13Em"></asp:TextBox>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtUnScheduledLimitPerVehicle" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='' class="form13Em"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>*Number of Vehicles
                            <asp:TextBox ID="txtUnScheduledNumberOfVehicles" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox></td>
                        <td colspan="4" style="text-align: center; text-align: center; padding-top: 15px;"><span class="informationalText" style="margin-top: 5px;">The number of cargo carrying vehicles will determine the catastrophe limit.</span></td>
                    </tr>
                    <tr>
                        <td>Catastrophe Limit
                            <asp:TextBox ID="txtUnScheduledCatLimit" runat="server" Enabled="false"></asp:TextBox></td>
                    </tr>

                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum limit of $100,000. </span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>


