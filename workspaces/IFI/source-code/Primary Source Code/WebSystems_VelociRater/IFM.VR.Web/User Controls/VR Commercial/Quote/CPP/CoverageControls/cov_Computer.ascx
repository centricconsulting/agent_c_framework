<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Computer.ascx.vb" Inherits="IFM.VR.Web.cov_Computer" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Computer_Item.ascx" TagPrefix="uc1" TagName="cov_Computer_Item" %>

<table id="cpSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table;width: 100%;">
    <tr>
        <td>
            <div id="divComputerOption" runat="server">
                <asp:CheckBox ID="chkComputer" runat="server" class="chkOption" Text="Computer" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divComputerDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>Valuation</td>
                        <td>Coinsurance</td>
                        <td></td>

                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="cpDeductible" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="cpValuation" runat="server" class="form13Em">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="cpCoinsurance" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                        <td></td>

                    </tr>
                    <tr>
                        <td class="ItemGroup" colspan="4">
                            <asp:CheckBox ID="chkEarthQuake" Text="Earthquake/Volcanic Eruption" runat="server" class="chkOption"/>
                            <div id="div_com_Earthquake" runat="server" style="display: none;" class="chkDetail">
                                <table class="qs_grid_4_columns" style="width: 100%">
                                    <tr>
                                        <td class="qs_rightJustify" colspan="3">*Earthquake/Volcanic Eruption Deductible 
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddEqDeductible" runat="server" class="form10Em">
                                                <%--<asp:ListItem Value="">-- Select --</asp:ListItem>--%>
                                                <asp:ListItem Value="1000">1,000</asp:ListItem>
                                                <asp:ListItem Value="2500">2,500</asp:ListItem>
                                                <asp:ListItem Value="5000">5,000</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>

                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="ItemGroup" colspan="4">
                            <asp:CheckBox ID="chkMechanicalBreakdown" Text="Mechanical Breakdown" runat="server" class="chkOption" />
                            <div id="divComMechanicalBreakdown" runat="server" style="display: none;" class="chkDetail">
                                <table class="qs_grid_4_columns" style="width: 100%">
                                    <tr>
                                        <td class="qs_rightJustify" colspan="3">*Mechanical Breakdown Deductible
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddMechanicalDeductible" runat="server" class="form10Em">
                                                <%--<asp:ListItem Value="">-- Select --</asp:ListItem>--%>
                                                <asp:ListItem Value="1000">1,000</asp:ListItem>
                                                <asp:ListItem Value="2500">2,500</asp:ListItem>
                                                <asp:ListItem Value="5000">5,000</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <uc1:cov_Computer_Item runat="server" ID="cov_Computer_Item" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum of $500,000.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>
