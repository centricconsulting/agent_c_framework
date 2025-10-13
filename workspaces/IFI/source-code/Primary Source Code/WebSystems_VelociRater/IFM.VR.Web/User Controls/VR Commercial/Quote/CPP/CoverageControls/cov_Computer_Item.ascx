<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Computer_Item.ascx.vb" Inherits="IFM.VR.Web.cov_Computer_Item" %>

<asp:Repeater ID="cpRepeater" runat="server">
    <ItemTemplate>
        <div class="ItemGroup">
            <asp:CheckBox ID="chkApply" runat="server" class="chkOption2" />
            Location:
            <asp:Label ID="txtAddress" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Address")%>' Style="margin-right: 13px;"></asp:Label>
            <br />
            <span style="margin-left: 25px">Building #<asp:Label ID="Label1" Text='<%# DataBinder.Eval(Container.DataItem, "BuildingIndex") + 1%>' runat="server"></asp:Label></span>
            <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                <div style="margin-left: 25px;">
                    <div style="float: right;">
                        <asp:Button ID="btnCopyToOtherLocations" Visible="false" CommandName="btnCopy" CssClass="roundedContainer StandardButton" ToolTip="Copy this buildings computer details to all buildings on all locations." runat="server" Text="Copy Coverage Details to All Locations and Buildings" />
                    </div>
                </div>
                <div runat="server" id="divItemDetails" style="padding-left: 20px; padding-top: 10px;">
                    <table class="qs_grid_4_columns">
                        <tr>
                            <td class="qs_rightJustify">*Hardware Limit
                            </td>
                            <td>
                                <asp:TextBox ID="txtHardwareLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Building.ComputerHardwareLimit")%>' class="form10Em"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="qs_rightJustify">Programs, Applications & Media Limit
                            </td>
                            <td>
                                <asp:TextBox ID="txtProgramsLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Building.ComputerProgramsApplicationsAndMediaLimit")%>' class="form10Em"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="qs_rightJustify">Business Income Limit
                            </td>
                            <td>
                                <asp:TextBox ID="txtBusinessIncomeLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Building.ComputerBusinessIncomeLimit")%>' class="form10Em"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
