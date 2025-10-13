<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlBarnsBuildings.ascx.vb" Inherits="IFM.VR.Web.ctlBarnsBuildings" %>
<%@ Register Src="~/Reports/FAR/ctlOtherCoverages.ascx" TagPrefix="uc1" TagName="ctlOtherCoverages" %>

<table style="width: 100%" class="table">
    <tr>
        <td>
            <asp:Label ID="lblAddress" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<table style="width: 100%" class="table">
    <tr style="vertical-align: bottom">
        <td style="width: 50%"></td>
        <td>
            <table style="width: 100%" class="table">
                <tr style="vertical-align: bottom">
                    <td align="right" style="width: 65%">
                        <asp:Label ID="lblBuildLimit" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblBuildPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<table style="width: 100%" class="table">
    <tr>
        <td style="width: 5%"></td>
        <td style="width: 45%">
            <asp:Label ID="lblStructure" runat="server"></asp:Label>
        </td>
        <td>
            <table style="width: 100%" class="table">
                <tr>
                    <td align="right" style="width: 65%">
                        <asp:Label ID="lblBuildLimitData" runat="server"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblBuildLimitPremData" runat="server"></asp:Label>
                    </td>
                </tr>                
            </table>
        </td>
    </tr>
    <tr runat="server" id="BuildingDwellingContentsRow" visible="false">
        <td style="width: 5%"></td>
        <td style="width: 45%">
            <asp:Label ID="lblStructureDwellingContents" runat="server"></asp:Label>
        </td>
        <td>
            <table style="width: 100%" class="table">
                <tr>
                    <td align="right" style="width: 65%">
                        <asp:Label ID="lblBuildDwellingContentsLimitData" runat="server"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblBuildDwellingContentsPremData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="width: 5%"></td>
        <td colspan="3">
            <table style="width: 100%" class="table">
                <tr>
                    <td style="width: 5%"></td>
                    <td style="width: 50%">
                        <asp:Label ID="lblType" runat="server" Text="Building Type:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblTypeData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <table style="width: 100%" class="table">
                <tr>
                    <td style="width: 5%"></td>
                    <td style="width: 50%">
                        <asp:Label ID="lblContruction" runat="server" Text="Construction:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblConstructionData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <table style="width: 100%" class="table">
                <tr>
                    <td style="width: 5%"></td>
                    <td style="width: 50%">
                        <asp:Label ID="lblDeductible" runat="server" Text="Deductible:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblDeductibleData" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<div id="dvBldngCoverage" runat="server">
    <br />
    <table id="tblBldngCoverage" runat="server" style="width: 100%" class="table">
        <tr>
            <td style="width: 5%"></td>
            <td style="vertical-align: top; width: 50%">
                <table style="width: 100%" class="table">
                    <tr>
                        <td>
                            <asp:DataGrid ID="dgBuildingCoverage" runat="server" HorizontalAlign="Left" AutoGenerateColumns="false" GridLines="None">
                                <ItemStyle CssClass="GridItem"></ItemStyle>
                                <HeaderStyle CssClass="GridHeader"></HeaderStyle>
                                <Columns>
                                    <asp:BoundColumn DataField="CoverageName" SortExpression="CoverageName" HeaderText="Coverage" HeaderStyle-Font-Bold="true" ItemStyle-Width="875px" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="CoverageLimit" SortExpression="CoverageLimit" HeaderText="Limits" HeaderStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="CoveragePrem" SortExpression="CoveragePrem" HeaderText="Premium" HeaderStyle-Font-Bold="true" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right"></asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>