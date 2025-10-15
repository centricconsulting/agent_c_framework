<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Signs.ascx.vb" Inherits="IFM.VR.Web.cov_Signs" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Signs_Item.ascx" TagPrefix="uc1" TagName="cov_Signs_Item" %>

<table id="siSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table; width: 100%">
    <tr>
        <td>
            <div id="divSignsOption" runat="server">
                <asp:CheckBox ID="chkSigns" runat="server" class="chkOption" Text="Signs" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divSignsDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>
                            <table class="qs_grid_4_columns">
                                <tbody>
                                    <tr>
                                        <td>Deductible</td>
                                        <td>Valuation</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="siDeductible" runat="server" class="form10Em"></asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="siValuation" runat="server" class="form13Em"></asp:DropDownList>
                                        </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tbody>

                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4"> 
                            <uc1:cov_Signs_Item runat="server" ID="cov_Signs_Item" />
                        </td>
                    </tr>
                </table>
                    <div style="text-align: center;margin-top: 5px;">
                        <span class="informationalText">Your binding authority for this coverage is a maximum of $100,000</span>
                    </div>
            </div>
        </td>
    </tr>

</table>
