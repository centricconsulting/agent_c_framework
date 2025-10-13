<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_App_InlandMarine.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_App_InlandMarine" %>

<div id="divCPPAppIM" runat="server">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Inland Marine Coverages"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkRemove" ToolTip="Remove all Inland Marine Coverages" runat="server" CssClass="RemovePanelLink">Remove</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>

    <div>
        <style type="text/css">
            .CPPSummaryFieldVerticalTop{
                vertical-align:top;
            }
            .CPPSummaryDoubleHeightControl{
                height: 60px;
            }
            .CPPAppTextboxLabel {
                width:90%;
                vertical-align:bottom;
                text-align:left;
            }
            .CPPAppDropdown {
                width:99%;
                vertical-align:top;
                text-align:left;
            }
            .CPPAppTD4Column {
                width:25%;
                vertical-align:bottom;
            }
            .CPPAppTD3Column {
                width:33%;
                vertical-align:bottom;
            }
            .CPPAppLabelColumn {
                vertical-align:bottom;
            }
        </style>

        <!-- Builders Risk -->
        <div id="divBuildersRisk" runat="server">
            <h3>
                <b>Builders Risk - Scheduled</b>
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveBuildersRisk" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptBuildersRisk" runat="server">
                    <ItemTemplate>
                        <table id="tblBuildersRisk" runat="server" style="width:100%;">
                            <tr runat="server" id="trBuildersRiskRow1">
                                <td class="CPPAppTD3Column">
                                    Limit
                                    <br />
                                    <asp:TextBox ID="txtBuildersRiskLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td colspan="2">
                                    Jobsite Location Description
                                    <br />
                                    <asp:TextBox ID="txtBuildersRiskJobsiteDescription" runat="server" style="width:96%;background-color:lightgray;" ReadOnly="true" ></asp:TextBox>
                                </td>
                            </tr>
                            <tr runat="server" id="trBuildersRiskRow2">
                                <td class="CPPAppTD3Column">
                                    Additional Interest Name
                                    <br />
                                    <asp:DropDownList ID="ddBuildersRiskAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Additional Interest Type
                                    <br />
                                    <asp:DropDownList ID="ddBuildersRiskAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    ATIMA
                                    <br />
                                    <asp:DropDownList ID="ddBuildersRiskATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Computers -->
        <div id="divComputers" runat="server">
            <h3>
                Computers
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveComputers" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptComputers" runat="server">
                    <ItemTemplate>
                        <table id="tblComputers" runat="server" style="width:100%;">
                            <tr runat="server" id="trComputersRow1">
                                <td class="CPPAppTD4Column" style="text-align:left;">
                                    Loc# - Bldg#
                                    <br />
                                    <asp:TextBox ID="txtComputersLocBldgNum" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true" ></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column">
                                    <asp:Label ID="lblComputersHardwareLimitLabel" runat="server" CssClass="CPPAppDoubleHeightLabel,CPPAppTextboxLabel">Hardware Limit</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtComputersHardwareLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column">
                                    <asp:Label ID="lblComputersPgmsAppAndMediaLimitLabel" runat="server" CssClass="CPPAppDoubleHeightLabel,CPPAppTextboxLabel">Programs Application & Media Limit</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtComputersProgramsAppAndMediaLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column">
                                    <asp:Label ID="lblComputersBusinessIncomeLimitLabel" runat="server" CssClass="CPPAppDoubleHeightLabel,CPPAppTextboxLabel">Business Income Limit</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtComputersBusinessIncomeLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                            </tr>
                            <tr runat="server" id="trComputersRow2">
                                <td class="CPPAppTD4Column">
                                    Additional Interest Name
                                    <br />
                                    <asp:DropDownList ID="ddComputersAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD4Column">
                                    Additional Interest Type
                                    <br />
                                    <asp:DropDownList ID="ddComputersAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value=""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD4Column">
                                    ATIMA
                                    <br />
                                    <asp:DropDownList ID="ddComputersATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD4Column">&nbsp;</td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Contractors Scheduled Equipment -->
        <div id="divContractors" runat="server">
            <h3>
                <asp:Label ID="lblContractorsSectionTitle" runat="server" Text="Contractors Scheduled Equipment" Font-Bold="true"></asp:Label>
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveContractors" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptContractors" runat="server" OnItemCommand="rptContractors_ItemCommand">
                    <ItemTemplate>
                        <table runat="server" id="tblContractors" style="width:100%">
                            <tr id="trContractorsRow1" runat="server">
                                <td class="CPPAppTD3Column">
                                    Limit**
                                    <br />
                                    <asp:TextBox ID="txtContractorsLimit" runat="server" CssClass="CPPAppTextboxLabel" onkeypress='return event.charCode >= 48 && event.charCode <= 57' ></asp:TextBox>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Valuation
                                    <br />
                                    <asp:DropDownList ID="ddContractorsValuation" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    *Description
                                    <br />
                                    <asp:TextBox ID="txtContractorsDescription" runat="server" CssClass="CPPSummaryDoubleHeightControl,CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trContractorsRow2" runat="server">
                                <td class="CPPAppTD3Column">
                                    Additonal Interest Name
                                    <br />
                                    <asp:DropDownList ID="ddContractorsAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Additional Interest Type
                                    <br />
                                    <asp:DropDownList ID="ddContractorsAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value =""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    ATIMA
                                    <br />
                                    <asp:DropDownList ID="ddContractorsATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    &nbsp;
                                </td>
                                <td style="text-align:right;">
                                    <asp:Button ID="btnContractorsDelete" runat="server" CssClass="StandardSaveButton" style="height:20px;" CommandName="DELETE" OnClientClick="return confirm('Delete this Scheduled Tools item?');" Text="Delete" />
<%--                                    <asp:LinkButton ID="lbContractorsDelete" runat="server" Text="Delete" OnClick="lbContractorsDelete_Click"></asp:LinkButton>--%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
                <span style="float:left;">
                    <asp:Button ID="btnContractorsAdd" runat="server" CssClass="StandardSaveButton" style="height:25px;" Text="Add Additional Equipment" />
                    <%--<asp:LinkButton ID="lbContractorsAdd" runat="server" Text="Add Additional Equipment"></asp:LinkButton>--%>
                    <br />
                    <br />
                    <asp:Label ID="lblContMsg1" runat="server" CssClass="informationalText" Text="**Entered limit amounts cannot exceed the total Scheduled Equipment amount. If the amount entered here is different than the original Scheduled Equipment amount, the premium will be adjusted accordingly." ></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblContMsg2" runat="server" CssClass="informationalText" Text="Your binding authority for this coverage is a maximum of $1,000,000 with a per item maximum limit of $500,000." ></asp:Label>
                </span>
            </div>
        </div>

        <!-- Fine Arts -->
        <div id="divFineArts" runat="server">
            <h3>
                Fine Arts
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveFineArts" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptFineArts" runat="server">
                    <ItemTemplate>
                        <table runat="server" id="tblFineArts" style="100%">
                            <tr id="trFineArtsRow1" runat="server">
                                <td class="CPPAppTD3Column" style="text-align:left;">
                                    Loc# - Bldg#
                                    <br />
                                    <asp:TextBox ID="txtFineArtsLocBldgNum" runat="server" CssClass="CPPAppTextboxLabe" BackColor="LightGray" ReadOnly="true" ></asp:TextBox>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Limit
                                    <br />
                                    <asp:TextBox ID="txtFineArtsLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD3Column">
                                    *Description
                                    <br />
                                    <asp:TextBox ID="txtFineArtsDescription" runat="server" CssClass="CPPSummaryDoubleHeightControl,CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trFineArtsRow2" runat="server">
                                <td class="CPPAppTD3Column">
                                    Additional Interest Name
                                    <br />
                                    <asp:DropDownList ID="ddFineArtsAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Additional Interest Type
                                    <br />
                                    <asp:DropDownList ID="ddFineArtsAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value =""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    ATIMA
                                    <br />
                                    <asp:DropDownList ID="ddFineArtsATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Installation Floater -->  <!-- REMOVED 6/28/2018 PER KATHY - AI'S FOR INSTALLATION DO NOT WORK IN DIAMOND, NEVER HAVE -->
        <!-- This section does not have a repeater -->
<%--        <div id="divInstallationFloater" runat="server">
            <h3>
                Installation Floater
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveInstallation" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <table style="width:100%;" >
                    <tr>
                        <td class="CPPAppTD3Column">
                            Included Limit
                            <br />
                            <asp:TextBox ID="txtInstallationFloaterIncludedLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="CPPAppTD3Column">
                            Increased Limit
                            <br />
                            <asp:TextBox ID="txtInstallationFloaterIncreasedLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="CPPAppTD3Column">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="CPPAppTD3Column">
                            Additional Interest Name
                            <br />
                            <asp:DropDownList ID="ddInstallationFloaterAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                        </td>
                        <td class="CPPAppTD3Column">
                            Additional Interest Type
                            <br />
                            <asp:DropDownList ID="ddInstallationFloaterAddlInterestType" runat="server" CssClass="CPPDropdown">
                                <asp:ListItem Value =""></asp:ListItem>
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="CPPAppTD3Column">
                            ATIMA
                            <br />
                            <asp:DropDownList ID="ddInstallationFloaterATIMA" runat="server" CssClass="CPPAppDropdown">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>--%>

        <!-- Motor Truck Cargo -->
        <div id="divMotorTruckCargo" runat="server">
            <h3>
                Motor Truck Cargo
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveMotorTruckCargo" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptMotorTruckCargo" runat="server">
                    <ItemTemplate>
                        <table id="tblMotorTruckCargo" runat="server" style="width:100%">
                            <tr id="trMotorTruckCargoRow1" runat="server">
                                <td class="CPPAppTD4Column">
                                    Limit
                                    <br />
                                    <asp:TextBox ID="txtMotorTruckCargoLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column">
                                    *Year
                                    <br />
                                    <asp:TextBox ID="txtYear" runat="server" CssClass="CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column">
                                    *Make
                                    <br />
                                    <asp:TextBox ID="txtMake" runat="server" CssClass="CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column">
                                    *Model
                                    <br />
                                    <asp:TextBox ID="txtModel" runat="server" CssClass="CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                            </tr>
                            <tr runat="server" id="trMotorTruckCargoRow2">
                                <td class="CPPAppTD4Column" style="vertical-align:bottom;">
                                    <asp:Label ID="lblMTCVIN" runat="server" CssClass="CPPAppTextboxLabel">*VIN</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtVIN" runat="server" CssClass="CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                                <td class="CPPAppTD4Column" style="vertical-align:bottom;">
                                    <asp:Label ID="lblMTCAdditionalInterestName" runat="server" CssClass="CPPAppDoubleHeightLabel,CPPAppTextboxLabel">Additional Interest Name</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="ddMotorTruckCargoAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD4Column" style="vertical-align:bottom;">
                                    <asp:Label ID="lblMTCAdditionalInterestType" runat="server" CssClass="CPPAppDoubleHeightLabel,CPPAppDropdown">Additional Interest Type</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="ddMotorTruckCargoAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value =""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD4Column" style="vertical-align:bottom;">
                                    <asp:Label ID="lblMTCATIMA" runat="server" CssClass="CPPAppDoubleHeightLabel,CPPAppTextboxLabel">ATIMA</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="ddMotorTruckCargoATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <!-- Motor Truck Cargo section for MTC UnScheduled MarkUp-->
        <div id="divUnScheduledMotorTruckCargo" runat="server">
            <h3>Motor Truck Cargo
        <span style="float: right;">
            <asp:LinkButton ID="lbSaveUnScheduledMotorTruckCargo" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
            </h3>
            <div>
                <table id="tblUnScheduledMotorTruckCargo" runat="server" style="width: 100%">
                    <tr>
                        <td class="CPPAppTD4Column">Limit</td>
                        <td class="CPPAppTD4Column">Additional Interest Name</td>
                        <td class="CPPAppTD4Column">Additional Interest Type</td>
                        <td class="CPPAppTD4Column">ATIMA</td>
                    </tr>
                    <tr>
                        <td class="CPPAppTD4Column">
                            <asp:TextBox ID="txtUnScheduledMotorTruckCargoLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddUnScheduledMotorTruckCargoAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                        </td>
                        <td class="CPPAppTD4Column" style="vertical-align: bottom;">
                            <asp:DropDownList ID="ddUnScheduledMotorTruckCargoAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                <asp:ListItem Value=""></asp:ListItem>
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddUnScheduledMotorTruckCargoATIMA" runat="server" CssClass="CPPAppDropdown">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- Owners Cargo -->
        <!-- This section does not have a repeater -->
        <div id="divOwnersCargo" runat="server">
            <h3>
                Owners Cargo
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveOwnersCargo" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <table style="width:100%;">
                    <tr>
                        <td class="CPPAppTD4Column">Limit</td>
                        <td class="CPPAppTD4Column">Additional Interest Name</td>
                        <td class="CPPAppTD4Column">Additional Interest Type</td>
                        <td class="CPPAppTD4Column">ATIMA</td>
                    </tr>
                    <tr>
                        <td class="CPPAppTD4Column">
                            <asp:TextBox ID="txtOwnersCargoLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddOwnersCargoAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddOwnersCargoAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                <asp:ListItem Value =""></asp:ListItem>
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddOwnersCargoATIMA" runat="server" CssClass="CPPAppDropdown">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Scheduled Property Floater -->
        <div id="divSchedulePropertyFloater" runat="server">
            <h3>
                Scheduled Property Floater
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveScheduledProperty" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptScheduledPropertyFloater" runat="server">
                    <ItemTemplate>
                        <table id="tblScheduledPropertyFloater" runat="server" style="width:100%;">
                            <tr runat="server" id="trScheduledPropertyRow1" >
                                <td class="CPPAppTD3Column">
                                    Limit
                                    <br />
                                    <asp:TextBox ID="txtScheduledPropertyLimit" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true"></asp:TextBox>
                                </td>
                                <td colspan="2">
                                    *Scheduled Property Description
                                    <br />
                                    <asp:TextBox ID="txtScheduledPropertyDescription" runat="server" CssClass="CPPSummaryDoubleHeightControl,CPPAppTextboxLabel" Style="width:95%;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr runat="server" id="trScheduledPropertyRow2">
                                <td class="CPPAppTD3Column">
                                    Additional Interest Name
                                    <br />
                                    <asp:DropDownList ID="ddScheduledPropertyAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Additional Interest Type
                                    <br />
                                    <asp:DropDownList ID="ddScheduledPropertyInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value =""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    ATIMA
                                    <br />
                                    <asp:DropDownList ID="ddScheduledPropertyATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Signs -->
        <div id="divSigns" runat="server">
            <h3>
                Signs
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveSigns" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <asp:Repeater ID="rptSigns" runat="server">
                    <ItemTemplate>
                        <table id="tblSigns" runat="server" style="width:100%;">
                            <tr id="trSignsRow1" runat="server">
                                <td class="CPPAppTD3Column" style="text-align:left;">
                                    Loc# - Bldg#
                                    <br />
                                    <asp:TextBox ID="txtSignsLocBldgNum" runat="server" CssClass="CPPAppTextboxLabel" BackColor="LightGray" ReadOnly="true" ></asp:TextBox>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Limit
                                    <br />
                                    <asp:TextBox ID="txtSignsLimit" runat="server" CssClass="CPPAppTextboxLabel" ReadOnly="true" BackColor="LightGray" ></asp:TextBox>
                                </td>
                                <td>
                                    *Sign Description
                                    <br />
                                    <asp:TextBox ID="txtSignsDescription" runat="server" CssClass="CPPSummaryDoubleHeightControl,CPPAppTextboxLabel"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trSignsRow2" runat="server">
                                <td class="CPPAppTD3Column">
                                    Additional Interest Name
                                    <br />
                                    <asp:DropDownList ID="ddSignsAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    Additional Interest Type
                                    <br />
                                    <asp:DropDownList ID="ddSignsAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value =""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="CPPAppTD3Column">
                                    ATIMA
                                    <br />
                                    <asp:DropDownList ID="ddSignsATIMA" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value="0">None</asp:ListItem>
                                        <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                        <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                        <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Transportation -->
        <!-- This section does not have a repeater -->
        <div id="divTransportation" runat="server">
            <h3>
                Transportation
                <span style="float: right;">
                    <asp:LinkButton ID="lbSaveTransportation" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </span>
            </h3>
            <div>
                <table style="width:100%;" id="tblTransportation" runat="server">
                    <tr>
                        <td class="CPPAppTD4Column">*Cargo Description</td>
                        <td class="CPPAppTD4Column">Additional Interest Name</td>
                        <td class="CPPAppTD4Column">Additional Interest Type</td>
                        <td class="CPPAppTD4Column">ATIMA</td>
                    </tr>
                    <tr>
                        <td class="CPPAppTD4Column">
                            <asp:TextBox ID="txtTransportationDescription" runat="server" CssClass="CPPSummaryDoubleHeightControl,CPPAppControl"></asp:TextBox>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddTransportationAddlInterestName" runat="server" CssClass="CPPAppDropdown"></asp:DropDownList>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddTransportationAddlInterestType" runat="server" CssClass="CPPAppDropdown">
                                        <asp:ListItem Value =""></asp:ListItem>
                                        <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                        <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                        <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="CPPAppTD4Column">
                            <asp:DropDownList ID="ddTransportationATIMA" runat="server" CssClass="CPPAppDropdown">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnBuildersRiskAccord" runat="server" />
    <asp:HiddenField ID="hdnComputersAccord" runat="server" />
    <asp:HiddenField ID="hdnContractorsAccord" runat="server" />
    <asp:HiddenField ID="hdnFineArtsAccord" runat="server" />
    <asp:HiddenField ID="hdnInstallationAccord" runat="server" />
    <asp:HiddenField ID="hdnMotorTrucCargoAccord" runat="server" />
    <asp:HiddenField ID="hdnOwnersCargoAccord" runat="server" />
    <asp:HiddenField ID="hdnScheduledPropertyAccord" runat="server" />
    <asp:HiddenField ID="hdnSignsAccord" runat="server" />
    <asp:HiddenField ID="hdnTransportationAccord" runat="server" />
</div>
