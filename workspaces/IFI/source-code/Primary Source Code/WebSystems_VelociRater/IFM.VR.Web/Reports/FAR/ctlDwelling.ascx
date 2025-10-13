<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDwelling.ascx.vb" Inherits="IFM.VR.Web.ctlDwelling" %>
<%@ Register Src="~/Reports/FAR/ctlOtherCoverages.ascx" TagPrefix="uc1" TagName="ctlOtherCoverages" %>

<div id="dvDwelling" runat="server">
    <table style="width: 100%" class="table">
        <tr>
            <td>
                <table style="width: 100%" class="table">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblAddlDwelling" runat="server" Text="" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblAddress" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblCityState" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblForm" runat="server" Text="Form Number: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblFormData" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDeductible" runat="server" Text="Dwelling Deductible: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblDeductibleData" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td></td>
            <td>
                <table style="width: 100%" class="table">
                    <tr>
                        <td>
                            <asp:Label ID="lblContruction" runat="server" Text="Construction: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblConstructionData" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblYearBuilt" runat="server" Text="Year Built: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblYearBuiltData" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblSqFeet" runat="server" Text="Square Feet: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblSqFeetData" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblStruct" runat="server" Text="Structure: "></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblStructData" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 100%" class="table">
        <tr>
            <td style="width: 5%"></td>
            <td style="vertical-align: top; width: 90%">
                <table id="pnlCoverageHdr" runat="server" style="width: 100%" class="table">
                    <tr style="vertical-align: bottom">
                        <td style="width: 50%">
                            <asp:Label ID="lblPropCoverage" runat="server" Text="Coverage" Font-Bold="true"></asp:Label>
                        </td>
                        <td>
                            <table style="width: 100%" class="table">
                                <tr style="vertical-align: bottom">
                                    <td align="right" style="width: 65%">
                                        <asp:Label ID="lblPropLimit" runat="server" Text="Limits" Font-Bold="true"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="lblPropPrem" runat="server" Text="Premium" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table id="pnlCovADwelling" runat="server" style="width: 100%" class="table">
                    <tr>
                        <td style="width: 50%">
                            <asp:Label ID="lblCovA" runat="server" Text="Coverage A - Dwelling"></asp:Label>
                        </td>
                        <td>
                            <table style="width: 100%" class="table">
                                <tr>
                                    <td align="right" style="width: 65%">
                                        <asp:Label ID="lblCovALimit" runat="server"></asp:Label>
                                        <asp:Label ID="lblCovASup" runat="server" Text="<sup>1</sup>"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="lblCovAPrem" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table id="pnlCovBStruct" runat="server" style="width: 100%" class="table">
                    <tr>
                        <td style="width: 50%">
                            <asp:Label ID="lblCovB" runat="server" Text="Coverage B - Other Structures"></asp:Label>
                        </td>
                        <td>
                            <table style="width: 100%" class="table">
                                <tr>
                                    <td align="right" style="width: 65%">
                                        <asp:Label ID="lblCovBLimit" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="lblCovBPrem" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" class="table">
                    <tr>
                        <td style="width: 50%">
                            <asp:Label ID="lblCovC" runat="server" Text="Coverage C - Personal Property"></asp:Label>
                        </td>
                        <td>
                            <table style="width: 100%" class="table">
                                <tr>
                                    <td align="right" style="width: 65%">
                                        <asp:Label ID="lblCovCLimit" runat="server"></asp:Label>
                                        <asp:Label ID="lblCovCSup" runat="server" Text="<sup>1</sup>" Visible="false"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="lblCovCPrem" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" class="table">
                    <tr>
                        <td style="width: 50%">
                            <asp:Label ID="lblCovD" runat="server" Text="Coverage D - Loss Of Use"></asp:Label>
                        </td>
                        <td>
                            <table style="width: 100%" class="table">
                                <tr>
                                    <td align="right" style="width: 65%">
                                        <asp:Label ID="lblCovDLimit" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="lblCovDPrem" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" class="table">
                    <tr>
                        <td>
                            <%--                        <asp:DataList ID="dlOtherPropCoverage" runat="server" Width="100%">
                                <ItemTemplate>
                                    <uc1:ctlOtherCoverages runat="server" id="ctlOtherCoverages" />
                                </ItemTemplate>
                            </asp:DataList>--%>
                            <table id="tblReplaceCost" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblReplaceCost" runat="server" Text="Replacement Cost - Cov C"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblReplaceCost_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblReplaceCost_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblMineSubCost" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblMineSubCost" runat="server" Text="Mine Subsidence Cov A & Cov B"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblMineSubCost_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblMineSubCost_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblBusiness" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblBusiness" runat="server" Text="Business Property Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblBusiness_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblBusiness_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblMoney" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblMoney" runat="server" Text="Money Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblMoney_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblMoney_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblVehicle" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblVehicle" runat="server" Text="Motorized Vehicle Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblVehicle_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblVehicle_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblSecurities" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblSecurities" runat="server" Text="Securities Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblSecurities_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblSecurities_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblSilverware" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblSilverware" runat="server" Text="Silverware, Gold ware and Pewter ware Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblSilverware_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblSilverware_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblGuns" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblGuns" runat="server" Text="Guns Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblGuns_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblGuns_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblJewelry" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblJewelry" runat="server" Text="Jewelry Limits"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblJewelry_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblJewelry_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblPrivatePowerPoles" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblPrivatePowerPoles" runat="server" Text="Private Power/Light Poles"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width:41.5%">
                                                    <asp:Label ID="lblPrivatePowerPoles_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblPrivatePowerPoles_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblACV" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblACV" runat="server" Text="Actual Cash Value"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblACV_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblACV_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblACVWind" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblACVWind" runat="server" Text="Actual Cash Value Loss Settlement - Windstorm or Hail Losses to Roof Surfacing"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblACVWind_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblACVWind_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblTheft" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="lblTheft" runat="server" Text="Theft of Building Materials"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblTheft_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblTheft_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblEarthquake" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblEarthquake" runat="server" Text="Earthquake"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblEarthquake_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblEarthquake_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblExpandReplace" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblExpandReplace" runat="server" Text="Expanded Replacement Cost Terms"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblExpandReplace_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblExpandReplace_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblFunReplace" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblFunReplace" runat="server" Text="Functional Replacement Cost Loss Settlement"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width:41.5%">
                                                    <asp:Label ID="lblFunReplace_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblFunReplace_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblMineSub" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblMineSub" runat="server" Text="Mine Subsidence"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width:41.5%">
                                                    <asp:Label ID="lblMineSub_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblMineSub_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblRPS" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblRPS" runat="server" Text="Cov. B Related Private Structures"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width:41.5%">
                                                    <asp:Label ID="lblRPS_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblRPS_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblPersInjury" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblPersInjury" runat="server" Text="Personal Injury"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width:41.5%">
                                                    <asp:Label ID="lblPersInjury_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblPersInjury_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <%--Underground Service Line--%>
                            <table id="tblUndergroundServiceLine" runat="server" visible="false" style="width:100%" class="table">
                                <tr>
                                    <td style="width:70%">
                                        <asp:Label ID="lblUndergroundServiceLine" runat="server" Text="Underground Service Line Coverage (FAR 1015)"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width:100%" class="table">
                                            <tr>
                                                <td align="right" style="width:41.5%">
                                                    <asp:Label ID="lblUndergroundServiceLine_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblUndergroundServiceLine_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <%--Cosmetic Damage Exclusion--%>
                            <table id="tblCosDamageEx" runat="server" visible="false" style="width: 100%" class="table">
                                <tr>
                                    <td style="width: 70%">
                                        <asp:Label ID="Label1" runat="server" Text="Cosmetic Damage Exclusion"></asp:Label>
                                    </td>
                                    <td>
                                        <table style="width: 100%" class="table">
                                            <tr>
                                                <td align="right" style="width: 41.5%">
                                                    <asp:Label ID="lblCosDamageEx_Limit" runat="server"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <asp:Label ID="lblCosDamageEx_Prem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>