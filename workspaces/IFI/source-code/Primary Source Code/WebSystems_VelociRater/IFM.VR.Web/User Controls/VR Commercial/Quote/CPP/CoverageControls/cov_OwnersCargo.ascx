<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_OwnersCargo.ascx.vb" Inherits="IFM.VR.Web.cov_OwnersCargo" %>

<table id="cpSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table; width: 100%;">
    <tr>
        <td>
            <div id="divOwnersCargoOption" runat="server">
                <asp:CheckBox ID="chkOwnersCargo" runat="server" class="chkOption ownerscargooption" Text="Owners Cargo**" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divOwnersCargoDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>*Cargo Description</td>
                        <td>*Limit per Vehicle</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            <asp:DropDownList ID="ocDeductible" runat="server" class="form10Em">
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
                        <td colspan="4" style="text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum limit of $100,000. </span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>
