<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FarmBuilding_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_FarmBuilding_Coverages" %>

<div runat="server" id="divMain">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Coverage"></asp:Label>
        <span style="float: right;">            
            <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" ToolTip="Clear" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="dvBuildingCoverages" runat="server">
    <div id="optionalLiability" runat="server">
        <asp:Label ID="lblOptLiability" runat="server" Text="OPTIONAL LIABILITY COVERAGES"></asp:Label>
        <div id="dvCustomFeeding" runat="server">
            <div style="margin-left:20px;">
                <asp:CheckBox ID="chkCustomFeeding" runat="server" CssClass="chkCF" />
                <asp:Label ID="lblCustomFeedingCheckboxLabel" runat="server" Text="Custom Feeding"></asp:Label>
            </div>
            <div id="dvCustomFeedingData" runat="server" style="display:none;" class="divCFData">
                <table id="tblCustomFeedingData" runat="server" style="width:100%;">
                    <tr>
                        <td class="CFColumn1">
                            <asp:Label runat="server" ID="lblCFCattleLabel" Text="Cattle Limit"></asp:Label>
                        </td>
                        <td class="CFColumn2">
                            <asp:DropDownList ID="ddCFCattleLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Cattle"></asp:DropDownList>
                        </td>
                        <td class="CFColumn3">
                            <asp:TextBox ID="txtCFCattleDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Cattle"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="CFColumn1">
                            <asp:Label runat="server" ID="lblCFEquineLabel" Text="Equine Limit"></asp:Label>
                        </td>
                        <td class="CFColumn2">
                            <asp:DropDownList ID="ddCFEquineLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Equine"></asp:DropDownList>
                        </td>
                        <td class="CFColumn3">
                            <asp:TextBox ID="txtCFEquineDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Equine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="CFColumn1">
                            <asp:Label runat="server" ID="lblCFPoultryLabel" Text="Poultry Limit"></asp:Label>
                        </td>
                        <td class="CFColumn2">
                            <asp:DropDownList ID="ddCFPoultryLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Poultry"></asp:DropDownList>
                        </td>
                        <td class="CFColumn3">
                            <asp:TextBox ID="txtCFPoultryDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Poultry"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="CFColumn1">
                            <asp:Label runat="server" ID="lblCFSwineLabel" Text="Swine Limit"></asp:Label>
                        </td>
                        <td class="CFColumn2">
                            <asp:DropDownList ID="ddCFSwineLimit" runat="server" Width="100%" CssClass="ddlCFLimit_Swine"></asp:DropDownList>
                        </td>
                        <td class="CFColumn3">
                            <asp:TextBox ID="txtCFSwineDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtCFDesc_Swine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="dvContractGrowers" runat="server">
            <div style="margin-left:20px">
                <asp:CheckBox ID="chkContractGrower" runat="server" />
                <asp:Label ID="lblContractGrowers" runat="server" Text="Contract Growers CCC"></asp:Label>
            </div>
            <div id="dvContractGrowersLimit" runat="server" style="display:none">
                <table style="width:100%">
                    <tr>
                        <td style="width:8%"></td>
                        <td>
                            <asp:Label ID="lblLimitCCC" runat="server" Text="Limit"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:8%"></td>
                        <td>
                            <asp:DropDownList ID="ddContractGrowerLimit" runat="server">
                                <asp:ListItem Value="55">250,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

    <br />
    <div id="optionalProperty" runat="server">
        <asp:Label ID="lblOptProperty" runat="server" Text="OPTIONAL PROPERTY COVERAGES"></asp:Label>
        <div id="dvCosmeticDamageExclusion" runat="server" style="margin-left:20px;">
            <asp:CheckBox ID="chkCosmeticDamageExclusion" runat="server" />
            <asp:Label ID="lblCDE" runat="server" Text="Cosmetic Damage Exclusion"></asp:Label>
            <div id="dvCosmeticDamageExclusionData" style="text-indent:25px;display:none;" runat="server">
                <table style="border-collapse:collapse;">
                    <tr id="trCDEValidationMessage" runat="server" style="display:none;">
                        <td colspan="2" style="text-align:center;">
                            <asp:label ID="lblCDEValidationMessage" runat="server" ForeColor="Red" Font-Bold="true" Text="Must select at least one Cosmetic Damage Exclusion Option"></asp:label>
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
        <div id="dvAddlPerils" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkAdditionalPerils" runat="server" />
            <asp:Label ID="lblAddlPerils" runat="server" Text="Additional Perils"></asp:Label>
        </div>
        <div id="dvEarthContents" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkEarthQuake_contents" runat="server" />
            <asp:Label ID="lblEarthContents" runat="server" Text="Earthquake (contents)"></asp:Label>
        </div>
        <div id="dvEarthStructure" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkEarthQuake_structure" runat="server" />
            <asp:Label ID="lblEarthStructure" runat="server" Text="Earthquake (structure)"></asp:Label>
        </div>
        <div id="dvReplacementCost" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkReplacement" runat="server" />
            <asp:Label ID="lblReplacement" runat="server" Text="Replacement Cost – Farm Barns, Buildings and Structures"></asp:Label>
        </div>
        <div id="dvSpecialForm" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkSpecialForm" runat="server" />
            <asp:Label ID="lblSpecialForm" runat="server" Text="Special Form"></asp:Label>
        </div>
        <div style="margin-left: 20px;">
            <asp:CheckBox ID="chkLossIncome" runat="server" />
            <asp:Label ID="lblLossIncome" runat="server"></asp:Label>
        </div>
        <div id="dvLossIncomeLimit" runat="server" style="display:none">
            <table style="width:100%">
                <tr>
                    <td style="width:8%"></td>
                    <td>
                        <asp:Label ID="lblLossIncomeLimit" runat="server" Text="Limit"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:8%"></td>
                    <td>
                        <asp:TextBox ID="txtLossIncomeLimit" Width="90px" ToolTip="Loss of Income Limit" MaxLength="11" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dvLossIncomeData" runat="server" style="display:none">
            <table style="width:100%">
                <tr>
                    <td style="width:15%"></td>
                    <td>
                        <asp:Label ID="lblLossExt" runat="server" Text="Period of Loss Extension"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlLossExt" runat="server" Width="80px">
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                            <asp:ListItem Value="1" Text="30 DAYS"></asp:ListItem>
                            <asp:ListItem Value="2" Text="60 DAYS"></asp:ListItem>
                            <asp:ListItem Value="3" Text="90 DAYS"></asp:ListItem>
                            <asp:ListItem Value="5" Text="120 DAYS"></asp:ListItem>
                            <asp:ListItem Value="6" Text="150 DAYS"></asp:ListItem>
                            <asp:ListItem Value="7" Text="180 DAYS"></asp:ListItem>
                            <asp:ListItem Value="8" Text="270 DAYS"></asp:ListItem>
                            <asp:ListItem Value="9" Text="360 DAYS"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width:15%"></td>
                    <td>
                        <asp:Label ID="lblCoInsurance" runat="server" Text="Coinsurance"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCoInsurance" runat="server" Width="80px">
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                            <asp:ListItem Value="1" Text="Waived"></asp:ListItem>
                            <asp:ListItem Value="2" Text="50%"></asp:ListItem>
                            <asp:ListItem Value="3" Text="60%"></asp:ListItem>
                            <asp:ListItem Value="4" Text="70%"></asp:ListItem>
                            <asp:ListItem Value="5" Text="80%"></asp:ListItem>
                            <asp:ListItem Value="6" Text="90%"></asp:ListItem>
                            <asp:ListItem Value="7" Text="100%"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dvBuildingMaterials" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkBuildingMaterials" runat="server" />
            <asp:Label ID="lblBuildingMaterials" runat="server" Text="Theft of Building Materials"></asp:Label>
        </div>

        <div id="dvSuffocationNEW" runat="server">
            <div style="margin-left:20px;">
                <asp:CheckBox ID="chkSuffocationOfLivestockOrPoultry" CssClass="chkSuffocation" runat="server" />
                <asp:Label ID="lblSuffCheckboxLabel" runat="server" Text="Suffocation of Livestock or Poultry"></asp:Label>
            </div>
            <div id="dvSuffocationOfLivestockOrPoultryData" runat="server" style="display:none;" class="divSuffData">
                <table id="tblSuffocationOfLivestockOrPoultry" runat="server" style="width:100%;">
                    <tr>
                        <td style="width:25%;text-indent:17px;">
                            <asp:Label runat="server" ID="lblSuffCattleLabel" Text="Cattle Limit"></asp:Label>
                        </td>
                        <td style="width:25%;">
                            <asp:TextBox ID="txtSuffCattleLimit" runat="server" Width="90%" CssClass="txtSuffLimit_Cattle"></asp:TextBox>
                        </td>
                        <td style="width:50%;">
                            <asp:TextBox ID="txtSuffCattleDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtSuffDesc_Cattle"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%;text-indent:17px;">
                            <asp:Label runat="server" ID="lblSuffEquineLabel" Text="Equine Limit"></asp:Label>
                        </td>
                        <td style="width:25%;">
                            <asp:TextBox ID="txtSuffEquineLimit" runat="server" Width="90%" CssClass="txtSuffLimit_Equine"></asp:TextBox>
                        </td>
                        <td style="width:50%;">
                            <asp:TextBox ID="txtSuffEquineDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtSuffDesc_Equine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%;text-indent:17px;">
                            <asp:Label runat="server" ID="lblSuffPoultryLabel" Text="Poultry Limit"></asp:Label>
                        </td>
                        <td style="width:25%;">
                            <asp:TextBox ID="txtSuffPoultryLimit" runat="server" Width="90%" CssClass="txtSuffLimit_Poultry"></asp:TextBox>
                        </td>
                        <td style="width:50%;">
                            <asp:TextBox ID="txtSuffPoultryDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtSuffDesc_Poultry"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%;text-indent:17px;">
                            <asp:Label runat="server" ID="lblSuffSwineLabel" Text="Swine Limit"></asp:Label>
                        </td>
                        <td style="width:25%;">
                            <asp:TextBox ID="txtSuffSwineLimit" runat="server" Width="90%" CssClass="txtSuffLimit_Swine"></asp:TextBox>
                        </td>
                        <td style="width:50%;">
                            <asp:TextBox ID="txtSuffSwineDesc" runat="server" Width="100%" placeholder="Enter Description..." CssClass="txtSuffDesc_Swine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="dvSuffocationOLD" runat="server">
            <div style="margin-left: 20px;">
                <asp:CheckBox ID="chkSuffocation" runat="server" />
                <asp:Label ID="lblSuffocation" runat="server" Text="Suffocation of Livestock"></asp:Label>
            </div>
            <div id="dvSufficationLimit" runat="server" style="display:none">
                <table style="width:100%">
                    <tr>
                        <td style="width:8%"></td>
                        <td>
                            <asp:Label ID="lblSufficationLimit" runat="server" Text="Limit"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:8%"></td>
                        <td>
                            <asp:TextBox ID="txtSuffocationLimit" Width="90px" ToolTip="Suffocation of Livestock Limit" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="divMineSub" runat="server" style="margin-left: 20px;">
            <asp:CheckBox ID="chkMineBuilding" runat="server" />
            <asp:Label ID="lblMine" runat="server" Text="Mine Subsidence"></asp:Label>
        </div>
        <div id="divMineSubReqHelpInfo" runat="server" style="margin-left: 20px;display: none;" class="informationalText">
            <asp:Label ID="lblMineSubReqHelpInfo" runat="server" Text="">Mine Subsidence is required for this location, if you would like to opt out, please contact your underwriter. We require the completed, signed form <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("FAR_Help_MineSubReject")%>" style="text-decoration-color:blue;"><font style="color:blue;">PHN-ML-0002</font></a> when removing this coverage.</asp:Label>
        </div>
        <div id="divMineSubReqHelpInfo_OH" runat="server" style="margin-left: 20px;display: none;" class="informationalText">
            <asp:Label ID="lblMineSubReqHelpInfo_OH" runat="server" Text="Mine Subsidence is required for this location."></asp:Label>
        </div>
        <div id="divMineSubLimitInfo" runat="server" style="margin-left: 20px;display: none;" class="informationalText">
            <asp:Label ID="lblMineSubLimitInfo" runat="server" Text="Building limit over 300,000. Mine subsidence limit 300,000."></asp:Label>
        </div>
        <div id="divMineSubNotReqHelpInfo" runat="server" style="margin-left: 20px;display: none;" class="informationalText">
            <asp:Label ID="lblMineSubNotReqHelpInfo" runat="server" Text="Mine Subsidence must be applied to all structures on one location if selected on another structure. It has been automatically applied to all buildings on this location."></asp:Label>
        </div>
<%--        <table style="width: 100%">
            <tr>
                <td style="width: 340px;">OPTIONAL LIABILITY COVERAGES</td>
                <td>Limit</td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkContractGrower" runat="server" Text="Contract Growers CCC" /></div>
                </td>
                <td>
                    <asp:DropDownList ID="ddContractGrowerLimit" runat="server">
                        <asp:ListItem Value="55">250,000</asp:ListItem>
                        <asp:ListItem Value="34">500,000</asp:ListItem>
                        <asp:ListItem Value="56">1,000,000</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr style="height: 20px;">
                <td colspan="2"></td>
            </tr>
            <tr>
                <td>OPTIONAL PROPERTY COVERAGES</td>
                <td>Limit</td>
            </tr>
            <tr runat="server" id="trAdditionalPerils">
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkAdditionalPerils" runat="server" Text="Additional Perils" />
                    </div>
                </td>
                <td></td>
            </tr>
            <tr runat="server" id="trEQ_Contents">
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkEarthQuake_contents" runat="server" Text="Earthquake (contents)" />
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkEarthQuake_structure" runat="server" Text="Earthquake (structure)" />
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkReplacement" runat="server" Text="Replacement Cost" />
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkSpecialForm" runat="server" Text="Special Form" />
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkLossIncome" runat="server" Text="Loss of Income" />
                    </div>
                    <div id="dvLossIncome" runat="server">
                        <table style="width:100%">
                            <tr>
                                <td style="width:15%"></td>
                                <td>
                                    <asp:Label ID="lblLossExt" runat="server" Text="Period of Loss Extension"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlLossExt" runat="server" Width="80px"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width:15%"></td>
                                <td>
                                    <asp:Label ID="lblCoInsurance" runat="server" Text="Coinsurance"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlCoInsurance" runat="server" Width="80px"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td style="vertical-align:top">
                    <asp:TextBox ID="txtLossIncomeLimit" Width="90px" ToolTip="Loss of Income Limit" MaxLength="11" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkBuildingMaterials" MaxLength="11" runat="server" Text="Theft of Building Materials" />
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkSuffocation" runat="server" Text="Suffocation of Livestock" />
                    </div>
                </td>
                <td>
                    <asp:TextBox ID="txtSuffocationLimit" Width="90px" ToolTip="Suffocation of Livestock Limit" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr runat="server" id="trMine">
                <td>
                    <div style="margin-left: 20px;">
                        <asp:CheckBox ID="chkMine" runat="server" Text="Mine subsidence" />
                    </div>
                </td>
                <td></td>
            </tr>
        </table>--%>
    </div>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="hiddenMineSubBldgChecked" runat="server" />
<asp:HiddenField ID="hiddenMineSubBldgEnabled" runat="server" />

<style type="text/css">
    .CFColumn1 {
        width:22%;
        text-indent: 17px;
    }
    .CFColumn2 {
        width:33%;
    }
    .CFColumn3 {
        width:45%;
    }
</style>
