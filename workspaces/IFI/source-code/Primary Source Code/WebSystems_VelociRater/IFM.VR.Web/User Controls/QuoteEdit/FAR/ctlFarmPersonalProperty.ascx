<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmPersonalProperty.ascx.vb" Inherits="IFM.VR.Web.ctlFarmPersonalProperty" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlAnimals.ascx" TagPrefix="uc1" TagName="ctlAnimals" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlMachineDescribed.ascx" TagPrefix="uc1" TagName="ctlMachineDescribed" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlMachineOpen.ascx" TagPrefix="uc1" TagName="ctlMachineOpen" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlIrrigationEquip.ascx" TagPrefix="uc1" TagName="ctlIrrigationEquip" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlLivestock.ascx" TagPrefix="uc1" TagName="ctlLivestock" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlMiscFarmPersonalProperty.ascx" TagPrefix="uc1" TagName="ctlMiscFarmPersonalProperty" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlBorrowed.ascx" TagPrefix="uc1" TagName="ctlBorrowed" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlMachineNotDescribed.ascx" TagPrefix="uc1" TagName="ctlMachineNotDescribed" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlGrainBuilding.ascx" TagPrefix="uc1" TagName="ctlGrainBuilding" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlGrainOpen.ascx" TagPrefix="uc1" TagName="ctlGrainOpen" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlHayBuilding.ascx" TagPrefix="uc1" TagName="ctlHayBuilding" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlHayOpen.ascx" TagPrefix="uc1" TagName="ctlHayOpen" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlReproductiveEquip.ascx" TagPrefix="uc1" TagName="ctlReproductiveEquip" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlBlnktPersProp.ascx" TagPrefix="uc1" TagName="ctlBlnktPersProp" %>

<div id="dvFarmPersProp" runat="server">
    <div id="dvFarmPersPropDeduct" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
        <h3>
            <asp:Label ID="lblPPHeader" runat="server" Text="Farm Personal Property"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkClearPersProp" runat="server" OnClientClick="var confirmed = confirm('Clear Personal Property Deductible?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Personal Property Deductible" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSavePersProp" runat="server" ToolTip="Save Personal Property" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <asp:Panel ID="pnlPersProp" runat="server">
                <div id="dvPersPropHdr" runat="server">
                    <asp:Label ID="lblPPDeduct" runat="server" Text="Deductible"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlPPDeduct" runat="server"></asp:DropDownList>
                </div>
            </asp:Panel>
        </div>
    </div>
    <asp:HiddenField ID="hiddenFarmPPCoverage" runat="server" />
    <div id="dvFarmPersPropSched" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
        <h3>
            <asp:Label ID="lblPPSHeader" runat="server" Text="Scheduled - Farm Personal Property"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkClearPersPropSched" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Scheduled Personal Property Coverage?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Clear ALL Scheduled Personal Property Coverage" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSavePersPropSched" runat="server" ToolTip="Save Scheduled Personal Property" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <div id="dvAnimals" class="div">
                <asp:CheckBox ID="chkAnimals" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblAnimals" runat="server" Text=" 4H and FFA Animals"></asp:Label>
                <div id="dvAnimalsLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblAnimalsLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblAnimalsDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlAnimals runat="server" ID="ctlAnimals" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddAnimalsLimit" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvMachineryDescribed" class="div">
                <asp:CheckBox ID="chkMachineryDescribed" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblMachineryDescribed" runat="server" Text=" Farm Machinery Described"></asp:Label>
                <div id="dvMachineryDescribedLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblMachineryDescribedLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblMachineryDescribedDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlMachineDescribed runat="server" ID="ctlMachineDescribed" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddMachineryDescribed" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvMachineryOpen" class="div">
                <asp:CheckBox ID="chkMachineryOpen" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblMachineryOpen" runat="server" Text=" Farm Machinery Described - Open Perils"></asp:Label>
                <div id="dvMachineryOpenLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblMachineryOpenLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblMachineryOpenDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlMachineOpen runat="server" ID="ctlMachineOpen" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddMachineryOpen" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvIrrigation" class="div">
                <asp:CheckBox ID="chkIrrigation" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblIrrigation" runat="server" Text=" Irrigation Equipment"></asp:Label>
                <div id="dvIrrigationLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblIrrigationLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblIrrigationDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlIrrigationEquip runat="server" ID="ctlIrrigationEquip" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddIrrigation" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvLivestock" class="div">
                <asp:CheckBox ID="chkLivestock" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblLivestock" runat="server" Text=" Livestock"></asp:Label>
                <div id="dvLivestockLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblLivestockLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblLivestockDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlLivestock runat="server" ID="ctlLivestock" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddLivestock" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvMiscFarmPersProperty" class="div" runat="server">
                <asp:CheckBox ID="chkMiscFarmPersProperty" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblMiscFarmPersProperty" runat="server" Text=" Miscellaneous Farm Personal Property"></asp:Label>
                <div id="dvMiscFarmPersPropertyLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblMiscFarmPersPropertyLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblMiscFarmPersPropertyDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlMiscFarmPersonalProperty runat="server" ID="ctlMiscFarmPersonalProperty" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddMiscFarmPersProperty" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <div id="dvBorrowed" class="div">
                <asp:CheckBox ID="chkBorrowed" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblBorrowed" runat="server" Text=" Rented or Borrowed Equipment"></asp:Label>
                <div id="dvBorrowedLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20px"></td>
                            <td style="width: 85px">
                                <asp:Label ID="lblBorrowedLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td style="width: 100px">
                                <asp:Label ID="lblBorrowedDesc" runat="server" Text="Description"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <uc1:ctlBorrowed runat="server" ID="ctlBorrowed" />
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkAddBorrowed" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                    </span>
                    <br />
                </div>
            </div>
            <br />
            <hr />
            <asp:LinkButton ID="lnkMoreLess" runat="server" ForeColor="Blue"></asp:LinkButton>
            <div id="dvMoreLess" runat="server" style="display: none">
                <div id="dvMachineryNotDescribed" class="div">
                    <asp:CheckBox ID="chkMachineryNotDescribed" runat="server" AutoPostBack="true" />
                    <asp:Label ID="lblMachineryNotDescribed" runat="server" Text=" Farm Machinery - Not Described"></asp:Label>
                    <div id="dvMachineryNotDescribedLimit" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20px"></td>
                                <td style="width: 85px">
                                    <asp:Label ID="lblMachineryNotDescribedLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td style="width: 100px">
                                    <asp:Label ID="lblMachineryNotDescribedDesc" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <uc1:ctlMachineNotDescribed runat="server" ID="ctlMachineNotDescribed" />
                        <%--                        <span style="float: right;">
                            <asp:LinkButton ID="lnkAddMachineryNotDescribed" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                        </span>--%>
                        <br />
                    </div>
                </div>
                <div id="dvGrainBuild" class="div">
                    <asp:CheckBox ID="chkGrainBuild" runat="server" AutoPostBack="true" />
                    <asp:Label ID="lblGrainBuild" runat="server" Text=" Grain in Buildings"></asp:Label>
                    <div id="dvGrainBuildLimit" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20px"></td>
                                <td style="width: 85px">
                                    <asp:Label ID="lblGrainBuildLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td style="width: 100px">
                                    <asp:Label ID="lblGrainBuildDesc" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <uc1:ctlGrainBuilding runat="server" ID="ctlGrainBuilding" />
                        <%--                        <span style="float: right;">
                            <asp:LinkButton ID="lnkAddGrainBuild" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                        </span>--%>
                        <br />
                    </div>
                </div>
                <div id="dvGrainOpen" class="div">
                    <asp:CheckBox ID="chkGrainOpen" runat="server" AutoPostBack="true" />
                    <asp:Label ID="lblGrainOpen" runat="server" Text=" Grain in the Open"></asp:Label>
                    <div id="dvGrainOpenLimit" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20px"></td>
                                <td style="width: 85px">
                                    <asp:Label ID="lblGrainOpenLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td style="width: 100px">
                                    <asp:Label ID="lblGrainOpenDesc" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <uc1:ctlGrainOpen runat="server" ID="ctlGrainOpen" />
                        <%--                        <span style="float: right;">
                            <asp:LinkButton ID="lnkAddGrainOpen" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                        </span>--%>
                        <br />
                    </div>
                </div>
                <div id="dvHayBuilding" class="div">
                    <asp:CheckBox ID="chkHayBuild" runat="server" AutoPostBack="true" />
                    <asp:Label ID="lblHayBuild" runat="server" Text=" Hay in Buildings"></asp:Label>
                    <div id="dvHayBuildLimit" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20px"></td>
                                <td style="width: 85px">
                                    <asp:Label ID="lblHayBuildLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td style="width: 100px">
                                    <asp:Label ID="lblHayBuildDesc" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <uc1:ctlHayBuilding runat="server" ID="ctlHayBuilding" />
                        <%--                        <span style="float: right;">
                            <asp:LinkButton ID="lnkAddHayBuilding" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                        </span>--%>
                        <br />
                    </div>
                </div>
                <div id="dvHayOpen" class="div">
                    <asp:CheckBox ID="chkHayOpen" runat="server" AutoPostBack="true" />
                    <asp:Label ID="lblHayOpen" runat="server" Text=" Hay in the Open"></asp:Label>
                    <div id="dvHayOpenLimit" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20px"></td>
                                <td style="width: 85px">
                                    <asp:Label ID="lblHayOpenLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td style="width: 100px">
                                    <asp:Label ID="lblHayOpenDesc" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <uc1:ctlHayOpen runat="server" ID="ctlHayOpen" />
                        <%--                        <span style="float: right;">
                            <asp:LinkButton ID="lnkAddHayOpen" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                        </span>--%>
                        <br />
                    </div>
                </div>
                <div id="dvReproductive" class="div">
                    <asp:CheckBox ID="chkReproductive" runat="server" AutoPostBack="true" />
                    <asp:Label ID="lblReproductive" runat="server" Text=" Reproductive Equipment"></asp:Label>
                    <div id="dvReproductiveLimit" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20px"></td>
                                <td style="width: 85px">
                                    <asp:Label ID="lblReproductiveLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td style="width: 100px">
                                    <asp:Label ID="lblReproductiveDesc" runat="server" Text="Description"></asp:Label>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                        <uc1:ctlReproductiveEquip runat="server" ID="ctlReproductiveEquip" />
                        <span style="float: right;">
                            <asp:LinkButton ID="lnkAddReproductive" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Scheduled</asp:LinkButton>
                        </span>
                        <br />
                    </div>
                </div>
            </div>
            <asp:HiddenField ID="hiddenMoreLessCnt" runat="server" />
        </div>
    </div>
    <asp:HiddenField ID="hiddenPPSched" runat="server" />
    <div id="dvFarmBlanket" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
        <h3>
            <asp:Label ID="lblBlanket" runat="server" Text="Blanket - Unscheduled Farm Personal Property"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkSaveBlanket" runat="server" ToolTip="Save Farm Blanket" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <uc1:ctlBlnktPersProp runat="server" ID="ctlBlnktPersProp" />
        </div>
    </div>
    <asp:HiddenField ID="hiddenFarmBlanket" runat="server" />
    <div id="dvFarmOptional" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
        <h3>
            <asp:Label ID="lblFarmOpt" runat="server" Text="Optional Coverages (F and G)"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkClearOptional" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Farm Optional Coverage?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Optional Coverage to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSaveOptional" runat="server" ToolTip="Save Optional Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <div id="dvSheepPerils" runat="server">
                <asp:CheckBox ID="chkSheepPerils" runat="server" Enabled="false" />
                <asp:Label ID="lblSheepPerils" runat="server" Text=" Sheep Additional Perils"></asp:Label>
                <div id="dvSheepPerilsLimit" runat="server" style="display: none">
                    <table style="width: 100%">
                        <tr>
                            <td></td>
                            <td style="width: 96%">
                                <asp:Label ID="lblSheepPerilsLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <div id="dvSheepLimit" runat="server" class="div">
                        <table style="width: 100%" class="tableBorder">
                            <tr>
                                <td style="width: 4%"></td>
                                <td vertical-align="central" class="CovTableColumn">
                                    <asp:TextBox ID="txtSheep_LimitData" runat="server" CssClass="CovTableItem" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));'></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                </div>
            </div>
            <div id="dvFarmMachinery" runat="server" style="display: block;">
                <asp:CheckBox ID="chkFarmMachinery" runat="server" />
                <asp:Label AssociatedControlID="chkFarmMachinery" ID="lblFarmMachinery" runat="server" Text=" Farm Machinery - Special Coverage - Coverage G"></asp:Label>
            </div>
            <div id="dvPropertyInTransit" runat="server" style="display: block;">
                <asp:CheckBox ID="chkPropertyInTransit" runat="server" />
                <asp:Label AssociatedControlID="chkPropertyInTransit" ID="lblPropertyInTransit" runat="server" Text=" Property in Transit"></asp:Label>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hiddenOptional" runat="server" />
    <div id="dvFarmIncidentalLimits" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
        <h3>
            <asp:Label ID="Label1" runat="server" Text="Farm Incidental Limits"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkClearFarmIncidental" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Farm Optional Coverage?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Optional Coverage to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSaveFarmIncidental" runat="server" ToolTip="Save Optional Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <div id="dvGlassBreakageCabs" runat="server">
                <asp:CheckBox ID="chkGlassBreakageCabs" runat="server" />
                <asp:Label ID="lblGlassBreakageCabs" runat="server" Text=" Glass Breakage in Cabs"></asp:Label>
                <table id="tblGlassBreakageCabs" runat="server" style="display: none">
                    <tr>
                        <td>
                            <label for="<%=txtGlassBreakageIncludedLimit.ClientID%>">Included Limit</label>
                            <br />
                            <asp:TextBox ID="txtGlassBreakageIncludedLimit" Width="100" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <label for="<%=txtGlassBreakageIncreaseLimit.ClientID%>">Increased Limit</label>
                            <br />
                            <asp:TextBox ID="txtGlassBreakageIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <label for="<%=txtGlassBreakageTotalLimit.ClientID%>">Total Limit</label>
                            <br />
                            <asp:TextBox ID="txtGlassBreakageTotalLimit" Width="100" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="HiddenFarmIncidentalLimits" runat="server" />

    <div>
        <table style="width: 100%" runat="server" id="divActionButtons">
            <tr>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Saves Policy Level Coverages." />
                </td>
                <td>
                    <asp:Button ID="btnIMRVPage" runat="server" Text="IM and RV Watercraft Page" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="IM/RV Watercraft Page" />
                </td>
                <td>
                    <asp:Button ID="btnRate" runat="server" Text="Rate this Quote" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Rate Quote" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hiddenMoreLess" runat="server" />
    </div>
    <asp:HiddenField ID="hiddenSheepPerilsExist" runat="server" />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" />
    <asp:Button ID="btnIMRVPageEndo" runat="server" Text="IM and RV Watercraft Page" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="IM/RV Watercraft Page" />
</div>
