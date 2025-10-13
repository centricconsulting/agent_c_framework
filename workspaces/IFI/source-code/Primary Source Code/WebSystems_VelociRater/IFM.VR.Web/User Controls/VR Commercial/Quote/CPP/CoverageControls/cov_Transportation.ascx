<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Transportation.ascx.vb" Inherits="IFM.VR.Web.cov_Transportation" %>

<table id="cpSubGroup_FoodManuf" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table; width: 100%;">
    <tr>
        <td>
            <div id="divTransportationOption_FoodManuf" runat="server">
                <asp:CheckBox ID="chkTransportation_FoodManuf" runat="server" class="chkOption transportationoption_FoodManuf" Text="Transportation**" />
                <asp:Button ID="clearButton_FoodManuf" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divTransportationDetail_FoodManuf" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2_FoodManuf" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>*Cargo Description</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            <asp:DropDownList ID="trDeductible_FoodManuf" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtDescription_FoodManuf" TextMode="MultiLine" runat="server" class="form13Em"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>*Limit per Vehicle</td>
                        <td>Increased Vehicle Limit</td>
                        <td>Total Vehicle Limit</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtLimitPerVehicle_FoodManuf" runat="server"  class="form13Em" text="50,000" ReadOnly="true" BackColor="LightGray"></asp:TextBox>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtIncreasedLimit_FoodManuf" runat="server"  class="form13Em transportationoption_FoodManuf_IncreasedLimit" ></asp:TextBox>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtTotalVehicleLimit_FoodManuf" runat="server"  class="form13Em" ReadOnly="true" BackColor="LightGray"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>*Number of Vehicles <asp:TextBox ID="txtNumOfVehicles_FoodManuf" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox></td>
                        <td colspan="4" style="text-align: center; text-align: center; padding-top: 15px;"><span class="informationalText" style="margin-top: 5px;">The number of owned vehicles will determine the catastrophe limit.</span></td>
                    </tr>
                    <tr>
                        <td>Catastrophe Limit <asp:TextBox ID="txtCatLimit_FoodManuf" runat="server" Enabled="false"></asp:TextBox></td>
                    </tr>
                   
                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum of $150,000. Maximum radius is 150 miles.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>

<table id="cpSubGroup_Normal" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table; width: 100%;">
    <tr>
        <td>
            <div id="divTransportationOption" runat="server">
                <asp:CheckBox ID="chkTransportation" runat="server" class="chkOption transportationoption" Text="Transportation**" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divTransportationDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>*Cargo Description</td>
                        <td>*Limit per Vehicle</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            <asp:DropDownList ID="trDeductible" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>

                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" class="form13Em"></asp:TextBox>
                        </td>
                        <td style="vertical-align: top">
                            <asp:TextBox ID="txtLimitPerVehicle" runat="server"  class="form13Em" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));'></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>*Number of Vehicles <asp:TextBox ID="txtNumOfVehicles" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox></td>
                        <td colspan="4" style="text-align: center; text-align: center; padding-top: 15px;"><span class="informationalText" style="margin-top: 5px;">The number of cargo carrying vehicles will determine the catastrophe limit.</span></td>
                    </tr>
                    <tr>
                        <td>Catastrophe Limit <asp:TextBox ID="txtCatLimit" runat="server" Enabled="false"></asp:TextBox></td>
                    </tr>
                   
                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum of $150,000. Maximum radius is 150 miles.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>