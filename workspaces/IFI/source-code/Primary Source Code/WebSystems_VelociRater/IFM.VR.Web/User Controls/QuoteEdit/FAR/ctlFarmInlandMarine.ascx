<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmInlandMarine.ascx.vb" Inherits="IFM.VR.Web.ctlFarmInlandMarine" %>
<%@ Register Src="~/User Controls/InlandMarine/ctlInlandMarineIncreasedLimit.ascx" TagPrefix="uc1" TagName="ctlInlandMarineIncreasedLimit" %>



<div id="dvInlandMarineInput" runat="server" class="standardSubSection">
    <h3>
        <asp:Label ID="lblInlandMarineHdr" runat="server" Text="INLAND MARINE"></asp:Label>
        <asp:Label ID="lblIMChosen" runat="server" Text="(0)"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearInland" runat="server" CssClass="RemovePanelLink" OnClientClick="var confirmed = confirm('Clear Inland Marine?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" Style="margin-left: 20px" ToolTip="Clear Inland Marine">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveinland" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>                    
        </span>
    </h3>
    <div>
    <asp:Panel ID="pnlRoundMsg" runat="server">
        <table style="width: 100%">
            <tr>
                <td>
                    <span style="float: right; margin-right: 20px">
                        <asp:Label ID="lblRoundMsg" runat="server" Text="All Limit values will be rounded up to the next $100"></asp:Label>
                    </span>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlInlandMarineCoverages" runat="server">
        <asp:Panel ID="pnlIMJewelry" runat="server">
            <div id="dvIMJewelry">
                <asp:CheckBox ID="chkIMJewelry" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblIMJewelry" runat="server" Text=" Jewelry"></asp:Label>
                <div id="dvIMJewelryLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblIMJewelryLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblIMJewelryDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlJewelry" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddIMJewelryLimit" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Jewelry</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlJewelInVault" runat="server">
            <div id="dvJewelryInVault">
                <asp:CheckBox ID="chkJewelInVault" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblJewelInVault" runat="server" Text=" Jewelry In Vault"></asp:Label>
                <div id="dvJewelInVaultLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblJewelryVaultLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblJewelryVaultDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlJewelInVault" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddJewleryInVault" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Jewelry In Vault</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlAntiquesBreak" runat="server">
            <div id="dvAntiquesBreak">
                <asp:CheckBox ID="chkAntiquesBreak" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblAntiquesBreak" runat="server" Text=" Antiques - with breakage coverage"></asp:Label>
                <div id="dvAntiquesBreakLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblAntiquesBreakLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblAntiquesBreakDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlAntiquesBreak" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddAntiquesBreak" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Antiques</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlAntiquesNoBreak" runat="server">
            <div id="dvAntiquesNoBreak">
                <asp:CheckBox ID="chkAntiquesNoBreak" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblAntiquesNoBreak" runat="server" Text=" Antiques - without breakage coverage"></asp:Label>
                <div id="dvAntiquesNoBreakLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblAntiquesNoBreakLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblAntiquesNoBreakDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlAntiquesNoBreak" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddAntiquesNoBreak" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Antiques</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlCollectBreak" runat="server">
            <div id="dvCollectBreak">
                <asp:CheckBox ID="chkCollectBreak" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblCollectBreak" runat="server" Text=" Collector Items Hobby - with breakage coverage"></asp:Label>
                <div id="dvCollectBreakLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblCollectBreakLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblCollectBreakDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlCollectBreak" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddCollectBreak" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Collector Items Hobby</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlCollectNoBreak" runat="server">
            <div id="dvCollectNoBreak">
                <asp:CheckBox ID="chkCollectNoBreak" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblCollectNoBreak" runat="server" Text=" Collector Items Hobby - without breakage coverage"></asp:Label>
                <div id="dvCollectNoBreakLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblCollectNoBreakLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblCollectNoBreakDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlCollectNoBreak" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddCollectNoBreak" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Collector Items Hobby</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlCameras" runat="server">
            <div id="dvCameras">
                <asp:CheckBox ID="chkCameras" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblCameras" runat="server" Text=" Cameras"></asp:Label>
                <div id="dvCamerasLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblCamerasLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblCamerasDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlCameras" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddCameras" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Cameras</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlComputers" runat="server">
            <div id="dvComputers">
                <asp:CheckBox ID="chkComputers" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblComputers" runat="server" Text=" Computers"></asp:Label>
                <div id="dvComputersLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblComputersLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblComputersDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlComputers" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddComputers" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Computers</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlFABreak" runat="server">
            <div id="dvFABreak">
                <asp:CheckBox ID="chkFABreak" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblFABreak" runat="server" Text=" Fine Arts - with breakage coverage"></asp:Label>
                <div id="dvFABreakLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblArtsBreakLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblArtsBreakDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlArtsBreak" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddFABreak" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Fine Arts</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlFANoBreak" runat="server">
            <div id="dvFANoBreak">
                <asp:CheckBox ID="chkFANoBreak" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblFANoBreak" runat="server" Text=" Fine Arts - without breakage coverage"></asp:Label>
                <div id="dvFANoBreakLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblArtsNoBreakLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblArtsNoBreakDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlArtsNoBreak" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddFANoBreak" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Fine Arts</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlFurs" runat="server">
            <div id="dvFurs">
                <asp:CheckBox ID="chkFurs" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblFurs" runat="server" Text=" Furs"></asp:Label>
                <div id="dvFursLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblFursLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblFursDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlFurs" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddFurs" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Furs</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlGuns" runat="server">
            <div id="dvGuns">
                <asp:CheckBox ID="chkGuns" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblGuns" runat="server" Text=" Guns"></asp:Label>
                <div id="dvGunsLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblGunsLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblGunsDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlGuns" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddGuns" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Guns</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlHearing" runat="server">
            <div id="dvHearing">
                <asp:CheckBox ID="chkHearing" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblHearing" runat="server" Text=" Hearing Aids"></asp:Label>
                <div id="dvHearingLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblHearingLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblHearingDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlHearing" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddHearing" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Hearing Aids</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlIrrigationNamed" runat="server">
            <div id="dvIrrigationNamed">
                <asp:CheckBox ID="chkIrrigationNamed" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblIrrigationNamed" runat="server" Text=" Irrigation Equipment - Named Perils"></asp:Label>
                <div id="dvIrrigationNamedLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblIrrigationNamedLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblIrrigationNamedDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlIrrigationNamed" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddIrrigationNamed" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Irrigation Equipment</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlIrrigationSpecial" runat="server">
            <div id="dvIrrigationSpecial">
                <asp:CheckBox ID="chkIrrigationSpecial" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblIrrigationSpecial" runat="server" Text=" Irrigation Equipment - Special Form"></asp:Label>
                <div id="dvIrrigationSpecialLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblIrrigationSpecialLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblIrrigationSpecialDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlIrrigationSpecial" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddIrrigationSpecial" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Irrigation Equipment</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlRadioCB" runat="server">
            <div id="dvRadioCB">
                <asp:CheckBox ID="chkRadioCB" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblRadioCB" runat="server" Text=" Radios - CB"></asp:Label>
                <div id="dvRadioCBLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblRadioCBLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblRadioCBDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlRadioCB" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddRadioCB" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Radios - CB</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlRadiosFM" runat="server">
            <div id="dvRadiosFM">
                <asp:CheckBox ID="chkRadiosFM" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblRadiosFM" runat="server" Text=" Radios - FM"></asp:Label>
                <div id="dvRadiosFMLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblRadiosFMLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblRadiosFMDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlRadiosFM" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddRadiosFM" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Radios - FM</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlReproductiveNamed" runat="server">
            <div id="dvReproductiveNamed">
                <asp:CheckBox ID="chkReproductiveNamed" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblReproductiveNamed" runat="server" Text=" Reproductive Materials - Named Perils"></asp:Label>
                <div id="dvReproductiveNamedLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblReproductiveNamedLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblReproductiveNamedDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlReproductiveNamed" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddReproductiveNamed" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Reproductive Materials - Named Perils</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlReproductiveSpecial" runat="server">
            <div id="dvReproductiveSpecial">
                <asp:CheckBox ID="chkReproductiveSpecial" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblReproductiveSpecial" runat="server" Text=" Reproductive Materials - Special Form"></asp:Label>
                <div id="dvReproductiveSpecialLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblReproductiveSpecialLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblReproductiveSpecialDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlReproductiveSpecial" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddReproductiveSpecial" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Reproductive Materials - Special Form</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlMobile" runat="server">
            <div id="dvMobile">
                <asp:CheckBox ID="chkMobile" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblMobile" runat="server" Text=" Telephone - Car or Mobile"></asp:Label>
                <div id="dvMobileLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblMobileLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblMobileDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlInlandMarineIncreasedLimit runat="server" id="ctlMobile" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddMobile" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Telephone - Car or Mobile</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
    </div>
    <asp:HiddenField ID="hiddenSelectedIMCoverage" runat="server" />
    <asp:HiddenField ID="hiddenIM" runat="server" />
</div>