<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlResidenceCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlResidenceCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/ctlFARAddlResidenceRentedToOthers.ascx" TagPrefix="uc1" TagName="ctlAddlResidence" %>

<div id="dvResidenceCoverage" runat="server">
    <h3>
        <asp:Label ID="lblResidenceCovHdr" runat="server" Text="Dwelling Coverages"></asp:Label>
        <span style="float: right">
            <asp:LinkButton ID="lnkRCClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Reset Policy Liability to Default Values" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkRCSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table style="width: 100%">
            <tr>
                <td>
                    <asp:Label ID="lblFarmLocDeduct" runat="server" Text="Deductible (A-D)"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlDeductible" runat="server"></asp:DropDownList>
                    <asp:Label ID="lblDeductible" runat="server"></asp:Label>
                </td>
                <td>
                    <div id="dvReplacementCC" runat="server">
                        <%--<asp:Panel ID="pnlE2Value" runat="server">--%>
                        <table style="width: 100%">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnReplacementCC" runat="server" Text="Replacement Cost Calculator" CssClass="StandardButtonWrap" Height="40px" Width="120px" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblCalValue" runat="server" Text="Calculated Value" Font-Italic="true"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblReplacementCCValue" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <%--</asp:Panel>--%>
                </td>
            </tr>
        </table>
        <table style="width: 100%">
            <tr>
                <td>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 36%">&nbsp;</td>
                            <td>
                                <table>
                                    <tr>
                                        <td align="center" style="width: 80px" class="CovTableColumn">
                                            <asp:Label ID="lblLimit" runat="server" Text="Limit" CssClass="CovTableHeaderLabel"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 80px" class="CovTableColumn">
                                            <asp:Label ID="lblChange" runat="server" Text="Change" CssClass="CovTableHeaderLabel"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 80px" class="CovTableColumn">
                                            <asp:Label ID="lblTotal" runat="server" Text="Total" CssClass="CovTableHeaderLabel"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table style="width: 100%">
            <tr>
                <td>
                    <div id="dvFarmLocation" runat="server">
                        <div id="dvCovA" runat="server">
                            <table style="width: 100%">
                                <tr id="trDwelling" runat="server">
                                    <td style="width: 36%">
                                        <asp:Label ID="lblDwellingReq" runat="server" Text="*"></asp:Label>
                                        <asp:Label ID="lblDwelling" runat="server" Text="A - Dwelling"></asp:Label>
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtDwLimit" runat="server" CssClass="CovTableItem" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtDwellingChangeInLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtDwellingTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvCovB" runat="server">
                            <table style="width: 100%">
                                <tr id="trStructures" runat="server">
                                    <td style="width: 36%">
                                        <asp:Label ID="lblPvtStructures" runat="server" Text="B - Related Private Structures"></asp:Label>
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtRPSLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtRPSChgInLimit" runat="server" CssClass="CovTableItem" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtRPSTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvCovC" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 36%">
                                        <asp:Label ID="lblPPReq" runat="server" Text="*"></asp:Label>
                                        <asp:Label ID="lblPersonalProp" runat="server" Text="C - Personal Property"></asp:Label>
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtPPLimit" runat="server" CssClass="CovTableItem" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtPPChgInLimit" runat="server" CssClass="CovTableItem" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtPPTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvCovD" runat="server">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 36%">
                                        <asp:Label ID="lblLivingCost" runat="server" Text="D - Loss of Use"></asp:Label>
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtLossLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtLossChgInLimit" runat="server" CssClass="CovTableItem" Text="0"></asp:TextBox>
                                                </td>
                                                <td class="CovTableColumn">
                                                    <asp:TextBox ID="txtLossTotalLimit" runat="server" CssClass="CovTableItem" BackColor="LightGray" ForeColor="Gray" Enabled="false" Text="0"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hiddenCovALimit" runat="server" />
        <asp:HiddenField ID="hiddenCovAChange" runat="server" Value="0" />
        <asp:HiddenField ID="hiddenCovATotal" runat="server" />
        <asp:HiddenField ID="hiddenCovBLimit" runat="server" />
        <asp:HiddenField ID="hiddenCovBChange" runat="server" />
        <asp:HiddenField ID="hiddenCovBTotal" runat="server" />
        <asp:HiddenField ID="hiddenCovCLimit" runat="server" />
        <asp:HiddenField ID="hiddenCovCChange" runat="server" />
        <asp:HiddenField ID="hiddenCovCTotal" runat="server" />
        <asp:HiddenField ID="hiddenCovDLimit" runat="server" />
        <asp:HiddenField ID="hiddenCovDChange" runat="server" />
        <asp:HiddenField ID="hiddenCovDTotal" runat="server" />
        <asp:HiddenField ID="hiddenMineSubDwellingChecked" runat="server" />
        <asp:HiddenField ID="hiddenMineSubDwellingEnabled" runat="server" />
        <div id="dvResidenceCovProp" runat="server">
            <h3>
                <asp:Label ID="lblResCovPropHdr" runat="server" Text="Dwelling Coverages - Property"></asp:Label>
                <span style="float: right">
                    <asp:LinkButton ID="lnkRCPClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Reset Policy Liability to Default Values" runat="server">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkRCPSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all" runat="server">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <div id="dvReplaceCost" runat="server">
                    <asp:CheckBox ID="chkReplaceCost" runat="server" />&nbsp;
                    <asp:Label ID="lblReplaceCost" runat="server" Text="Replacement Cost - Cov C"></asp:Label>&nbsp;
                </div>
                <div id="dvReplaceCostIncl" runat="server" style="display: none">
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblReplaceCostIncl" runat="server" Text="Replacement Cost - Cov C"></asp:Label>&nbsp;
                    <asp:Label ID="lblRCInclude" runat="server" Text="(Included)" Font-Bold="true"></asp:Label>
                </div>
                <div id="dvACV" runat="server">
                    <asp:CheckBox ID="chkACV" runat="server" />&nbsp;
                    <asp:Label ID="lblACV" runat="server" Text="Actual Cash Value"></asp:Label>
                </div>
                <div id="dvACVWind" runat="server">
                    <asp:CheckBox ID="chkACVWind" runat="server" />&nbsp;
                    <asp:Label ID="lblACVWind" runat="server" Text="Actual Cash Value Loss Settlement - Windstorm or Hail Losses to Roof Surfacing"></asp:Label>
                </div>
                <div id="dvTheft" runat="server">
                    <asp:CheckBox ID="chkTheft" runat="server" />&nbsp;
                    <asp:Label ID="lblTheft" runat="server" Text="Theft of Building Materials"></asp:Label>
                    <div id="dvTheftLimit" runat="server" style="display: none">
                        <asp:Label ID="lblTheftLimit" runat="server" Text="Total Limit"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtTheftLimit" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div id="dvEarthquake" runat="server">
                    <asp:CheckBox ID="chkEarthquake" runat="server" />&nbsp;
                    <asp:Label ID="lblEarthquake" runat="server" Text="Earthquake"></asp:Label>
                </div>
                <div id="dvExpandReplacement" runat="server">
                    <asp:CheckBox ID="chkExpandReplacement" runat="server" />&nbsp;
                    <asp:Label ID="lblExpandReplacement" runat="server" Text="Expanded Replacement Cost Terms"></asp:Label>
                </div>
                <div id="dvReplacement" runat="server">
                    <asp:CheckBox ID="chkReplacement" runat="server" />&nbsp;
                    <asp:Label ID="lblReplacement" runat="server" Text="Functional Replacement Cost Loss Settlement"></asp:Label>
                </div>
                <div id="dvMineSubsidence" runat="server" style="display: none">
                    <asp:CheckBox ID="chkMineSubsidence" runat="server" />&nbsp;
                    <asp:Label ID="lblMineSubsidence" runat="server" Text="Mine Subsidence"></asp:Label>
                </div>
                <div id="dvMineSubsidenceReqHelpInfo" runat="server" style="display: none" class="informationalText">
                    <asp:Label ID="lblMineSubsidenceReqHelpInfo" runat="server" Text="">Mine Subsidence is required for this location, if you would like to opt out, please contact your underwriter. We require the completed, signed form <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("FAR_Help_MineSubReject")%>" style="text-decoration-color:blue;"><font style="color:blue;">PHN-ML-0002</font></a> when removing this coverage.</asp:Label>
                </div>
                <div id="dvMineSubsidenceReqHelpInfo_OH" runat="server" style="display: none" class="informationalText">
                    <asp:Label ID="lblMineSubsidenceReqHelpInfo_OH" runat="server" Text="Mine Subsidence is required for this location."></asp:Label>
                </div>
                <div id="dvMineSubsidenceNotReqHelpInfo" runat="server" style="display: none" class="informationalText">
                    <asp:Label ID="lblMineSubsidenceNotReqHelpInfo" runat="server" Text="Mine Subsidence must be applied to all structures on one location if selected on another structure. It has been automatically applied to all buildings on this location."></asp:Label>
                </div>
                <div id="dvMineSubLimitInfo" runat="server" style="margin-left: 20px; display: none;" class="informationalText">
                    <asp:Label ID="lblMineSubLimitInfo" runat="server" Text="Building limit over 300,000. Mine subsidence limit 300,000."></asp:Label>
                </div>
                <div id="dvRPS" runat="server">
                    <asp:CheckBox ID="chkRPS" runat="server" Enabled="false" />&nbsp;
                    <asp:Label ID="lblRPS" runat="server" Text="Increase to Related Private Structures"></asp:Label>
                </div>
                <div id="dvCosmeticDamageExclusion" runat="server">
                    <asp:CheckBox ID="chkCosmeticDamageExclusion" runat="server" />&nbsp;
                    <asp:Label ID="lblCDE" runat="server" Text="Cosmetic Damage Exclusion"></asp:Label>
                    <div id="dvCosmeticDamageExclusionData" style="text-indent: 25px; display: none;" runat="server">
                        <table>
                            <tr id="trCDEValidationMessage" runat="server" style="display: none;">
                                <td colspan="2" style="text-align: center;">
                                    <asp:Label ID="lblCDEValidationMessage" runat="server" ForeColor="Red" Font-Bold="true" Text="Must select at least one Cosmetic Damage Exclusion Option"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCDEExteriorDoorAndWindowSurfacing" runat="server" Text="Exterior Door and Window Surfacing"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkCDEExteriorDoorAndWindowSurfacing" runat="server" Text="" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCDEExteriorWallSurfacing" runat="server" Text="Exterior Wall Surfacing"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkCDEExteriorWallSurfacing" runat="server" Text="" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCDERoofSurfacing" runat="server" Text="Roof Surfacing"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkCDERoofSurfacing" runat="server" Text="" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="dvUndergroundServiceLine" runat="server">
                    <asp:CheckBox ID="chkUndergroundServiceLine" runat="server" />&nbsp;
                    <asp:Label ID="lblUndergroundServiceLine" runat="server" Text="Underground Service Line"></asp:Label>
                    <div id="dvUndergroundServiceLineLimit" runat="server" style="display: none">
                        <asp:Label ID="lblUndergroundServiceLineLimit" runat="server" Text="Included Limit"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtUndergroundServiceLineLimit" runat="server" Enabled="false">10,000</asp:TextBox>
                    </div>
                </div>
                <div id="dvMotorizedVehiclesLimit" runat="server">
                    <asp:CheckBox ID="chkMotorizedVehiclesIncreasedLimit" runat="server" />&nbsp;
                    <asp:Label ID="lblMotorizedVehiclesIncreasedLimit" runat="server" Text="Motorized Vehicles Increased Limit"></asp:Label>
                    <div id="dvMotorizedVehiclesIncreasedLimit" runat="server">
                        <table>
                            <tr id="trStandard" runat="server">
                                <td>
                                    <label for="<%=txtMotorizedIncludedLimit.ClientID%>">Included Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtMotorizedIncludedLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtMotorizedIncreaseLimit.ClientID%>">Increased Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtMotorizedIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtMotorizedTotalLimit.ClientID%>">Total Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtMotorizedTotalLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="dvPrivatePowerPoles" runat="server" style="display: none">
                        <asp:CheckBox ID="chkPrivatePowerPoles" runat="server" />&nbsp;
                        <asp:Label ID="lblPrivatePowerPoles" runat="server" Text="Private Power/Light Poles"></asp:Label>
                        <table id="tblPrivatePowerPoles" runat="server" style="display: none">
                            <tr>
                                <td>
                                    <label for="<%=txtPrivatePowerPolesIncludedLimit.ClientID%>">Included Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtPrivatePowerPolesIncludedLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtPrivatePowerPolesIncreaseLimit.ClientID%>">Increased Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtPrivatePowerPolesIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtPrivatePowerPolesTotalLimit.ClientID%>">Total Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtPrivatePowerPolesTotalLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
            </div>
            <asp:HiddenField ID="hiddenFormTypeId" runat="server" /><%-- added 10/14/22 for task 68166 BD --%>
            <asp:HiddenField ID="hiddenResCovProp" runat="server" />
            <asp:HiddenField ID="hiddenFormType" runat="server" />
            <asp:HiddenField ID="hiddenProgramType" runat="server" />
            <asp:HiddenField ID="hiddenBuiltYear" runat="server" />
            <asp:HiddenField ID="hiddenDwellingClassification" runat="server" />
            <asp:HiddenField ID="hdnYearBuiltTextboxClientId" runat="server" />
            <asp:HiddenField ID="hdnCoverageFormDropdownClientId" runat="server" />
        </div>
        <div id="dvResidenceCovLiab" runat="server">
            <h3>
                <asp:Label ID="lblResidenceCovLiabHdr" runat="server" Text="Dwelling Coverages - Liability"></asp:Label>
                <span style="float: right">
                    <asp:LinkButton ID="lnkDCClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Reset Policy Liability to Default Values" runat="server">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkDCSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all" runat="server">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <div id="dvAddlResidenceRentedToOthers" runat="server" >
                    <uc1:ctlAddlResidence id="ctlFARAddlResidence" runat="server"></uc1:ctlAddlResidence>
                </div>
                <%--Removed until IS fixes issue--%>
                <div style="display: none">
                    <asp:CheckBox ID="chkAddlResidence" runat="server" />&nbsp;
                    <asp:Label ID="lblAddlResidence" runat="server" Text="Additional Residence - Occupied by Insured"></asp:Label>
                </div>
            </div>
            <asp:HiddenField ID="hiddenResCovLiab" runat="server" />
        </div>
    </div>
    <asp:HiddenField ID="hiddenResCov" runat="server" />
</div>