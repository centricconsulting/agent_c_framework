<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlInlandMarineItem.ascx.vb" Inherits="IFM.VR.Web.ctlInlandMarineItem" %>
<%--<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlIMJewelry.ascx" TagPrefix="uc1" TagName="ctlIMJewelry" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlJewelryVault.ascx" TagPrefix="uc1" TagName="ctlJewelryVault" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlCameras.ascx" TagPrefix="uc1" TagName="ctlCameras" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlComputers.ascx" TagPrefix="uc1" TagName="ctlComputers" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlFarm.ascx" TagPrefix="uc1" TagName="ctlFarm" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlArtsBreak.ascx" TagPrefix="uc1" TagName="ctlArtsBreak" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlArtsNoBreak.ascx" TagPrefix="uc1" TagName="ctlArtsNoBreak" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlFurs.ascx" TagPrefix="uc1" TagName="ctlFurs" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlTractors.ascx" TagPrefix="uc1" TagName="ctlTractors" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlGolfEquip.ascx" TagPrefix="uc1" TagName="ctlGolfEquip" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlGuns.ascx" TagPrefix="uc1" TagName="ctlGuns" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlHearing.ascx" TagPrefix="uc1" TagName="ctlHearing" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlSilverware.ascx" TagPrefix="uc1" TagName="ctlSilverware" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlMobile.ascx" TagPrefix="uc1" TagName="ctlMobile" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlTools.ascx" TagPrefix="uc1" TagName="ctlTools" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/InlandMarine/ctlMusicInstr.ascx" TagPrefix="uc1" TagName="ctlMusicInstr" %>


<div id="dvInlandMarineInput" runat="server" class="standardSubSection">
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
                    <uc1:ctlIMJewelry runat="server" ID="ctlIMJewelry" />
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
                    <uc1:ctlJewelryVault runat="server" id="ctlJewelryVault" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddJewleryInVault" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Jewelry In Vault</asp:LinkButton>
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
                    <uc1:ctlCameras runat="server" ID="ctlCameras" />
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
                    <uc1:ctlComputers runat="server" ID="ctlComputers" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddComputers" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Computers</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlFarm" runat="server">
            <div id="dvFarm">
                <asp:CheckBox ID="chkFarm" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblFarm" runat="server" Text=" Farm Machinery - Scheduled"></asp:Label>
                <div id="dvFarmLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblFarmLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblFarmDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlFarm runat="server" ID="ctlFarm" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddFarm" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Farm Machinery</asp:LinkButton>
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
                    <uc1:ctlArtsBreak runat="server" ID="ctlArtsBreak" />
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
                    <uc1:ctlArtsNoBreak runat="server" ID="ctlArtsNoBreak" />
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
                    <uc1:ctlFurs runat="server" ID="ctlFurs" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddFurs" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Furs</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlGarden" runat="server">
            <div id="dvGarden">
                <asp:CheckBox ID="chkGarden" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblGarden" runat="server" Text=" Garden Tractors"></asp:Label>
                <div id="dvGardenLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblGardenLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblGardenDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlTractors runat="server" ID="ctlTractors" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddGarden" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Garden Tractors</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlGolfers" runat="server">
            <div id="dvGolfers">
                <asp:CheckBox ID="chkGolfers" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblGolfers" runat="server" Text=" Golfers Equipment"></asp:Label>
                <div id="dvGolfersLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblGolfLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblGolfDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlGolfEquip runat="server" ID="ctlGolfEquip" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddGolfers" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Golfers Equipment</asp:LinkButton>
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
                    <uc1:ctlGuns runat="server" ID="ctlGuns" />
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
                    <uc1:ctlHearing runat="server" ID="ctlHearing" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddHearing" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Hearing Aids</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlSilverware" runat="server">
            <div id="dvSilverware">
                <asp:CheckBox ID="chkSilverware" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblSilverware" runat="server" Text=" Silverware"></asp:Label>
                <div id="dvSilverwareLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblSilverwareLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblSilverwareDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlSilverware runat="server" ID="ctlSilverware" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddSilverware" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Silverware</asp:LinkButton>
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
                    <uc1:ctlMobile runat="server" ID="ctlMobile" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddMobile" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Telephone - Car or Mobile</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlTools" runat="server">
            <div id="dvTools">
                <asp:CheckBox ID="chkTools" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblTools" runat="server" Text=" Tools and Equipment"></asp:Label>
                <div id="dvToolsLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblToolsLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblToolsDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlTools runat="server" ID="ctlTools" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddTools" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Tools and Equipment</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlMusic" runat="server">
            <div id="dvMusic">
                <asp:CheckBox ID="chkMusic" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblMusic" runat="server" Text=" Musical Instrument (Non-Professional)"></asp:Label>
                <div id="dvMusicLimit" runat="server">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblMusicLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblMusicDeductible" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlMusicInstr runat="server" id="ctlMusicInstr" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddMusic" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add Additional Musical Instrument (Non-Professional)</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</div>
<asp:HiddenField ID="hiddenSelectedIMCoverage" runat="server" />--%>