<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_BuildingCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_BuildingCoverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_PhotographersEquipment.ascx" TagPrefix="uc1" TagName="ctl_PhotographersEquipment" %>

<style>
    .SpacerColumn {
        width:3%;
    }
    .LabelColumn {
        width:47%;
        text-align:left;
    }
    .DataColumn {
        width:50%;
        text-align:left;
    }
</style>

<div runat="server" id="divMain">
    <h3>Optional Building Coverages
         <span style="float: right;">
            <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Policy Level Coverages">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table id="tblOptionalBuildingCoverages" runat="server">
            <tr>
                <td>
                    <asp:CheckBox id="chkAcctsReceivableONPremises" runat="server" Text="Accounts Receivable - On Premises" />
                </td>
            </tr>
            <tr id="trAcctsReceivableONPremisesDataRow" runat="server" style="display:none;">
                <td>
                    <table id="tblAcctsReceivableONPremises" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Total Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtAcctsReceivableOnPremisesTotalLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trAccountsReceivableOnPremInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                The BOP Enhancement Endorsement includes Accounts Receivable coverage with limits of $50,000 on-premises and $25,000 off-premises. Please enter the total amount for limits greater than $50,000 for on-premises coverage. Higher limits for off-premises coverage are not available.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox id="chkValuablePapersOnPremises" runat="server" Text="Valuable Papers - On Premises" />
                </td>
            </tr>
            <tr id="trValuablePapersOnPremisesDataRow" runat="server" style="display:none;">
                <td>
                    <table id="tblValuablePapersOnPremises" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Total Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtValuablePapersOnPremisesTotalLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trValuablePapersOnPremInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                The BOP Enhancement Endorsement includes Valuable Papers and records coverage with limits of $25,000 on-premises and $10,000 off-premises. Please enter the total amount for limits greater than $25,000 for on-premisis coverage. Higher limits for off-premises coverage are not available.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trOrdinanceOrLawCheckBoxRow" runat="server">
                <td>
                    <asp:CheckBox id="chkOrdinanceOrLaw" runat="server" Text="Ordinance or Law" />
                </td>
            </tr>
            <tr id="trOrdinanceOrLawDataRow" runat="server" style="display:none;">
                <td>
                    <table id="tblOrdinanceOrLaw" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Loss to the Undamaged Portion of the Building - Undamaged
                            </td>
                            <td class="DataColumn">
                                <asp:CheckBox ID="chkOrdinanceOrLawUndamaged" runat="server" Checked="true" Enabled="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Demolition Cost Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtDemolitionCostLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Increased Cost of Construction Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtIncreasedCostOfConstructionLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr><td>&nbsp</td><td>&nbsp</td><td class="DataColumn">OR</td></tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Demolition and Increased Cost Combined Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtDemolitionAndIncreasedCostCombinedLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trOrdinanceOrLawInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Applicable to Coverage 3 Increased Cost of Construction - The BOP Enhancement Endorsement includes Increased Cost of Construction with a $25,000 limit. Please enter the amount in excess of the $25,000 limit included in the enhancement.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox id="chkSpoilage" runat="server" Text="Spoilage" />
                </td>
            </tr>
            <tr id="trSpoilageDataRow" runat="server" style="display:none;">
                <td>
                    <table id="tblSpoilage" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Property Classification
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlSpoilagePropertyClassification" runat="server" Width="98%">
                                    <asp:ListItem Value="1">Bakery Goods</asp:ListItem>
                                    <asp:ListItem Value="2">Cheese Goods</asp:ListItem>
                                    <asp:ListItem Value="3">Convenience Food Stores</asp:ListItem>
                                    <asp:ListItem Value="4">Dairy Products, excluding Ice Cream</asp:ListItem>
                                    <asp:ListItem Value="5">Dairy Products, including Ice Cream</asp:ListItem>
                                    <asp:ListItem Value="6">Delicatessens</asp:ListItem>
                                    <asp:ListItem Value="7">Florists</asp:ListItem>
                                    <asp:ListItem Value="8">Fruits and Vegetables</asp:ListItem>
                                    <asp:ListItem Value="9">Grocery Stores</asp:ListItem>
                                    <asp:ListItem Value="10">Meat and Poultry Markets</asp:ListItem>
                                    <asp:ListItem Value="11">Other</asp:ListItem>
                                    <asp:ListItem Value="12">Pharmaceuticals</asp:ListItem>
                                    <asp:ListItem Value="13">Restaurants (limited cooking only)</asp:ListItem>
                                    <asp:ListItem Value="14">Seafood</asp:ListItem>
                                    <asp:ListItem Value="15">Supermarkets</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Total Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtSpoilageTotalLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Refrigeration Maintenance Agreement
                            </td>
                            <td class="DataColumn">
                                <asp:CheckBox ID="chkRefrigeratorMaintenanceAgreement" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Breakdown or Contamination
                            </td>
                            <td class="DataColumn">
                                <asp:CheckBox ID="chkBreakdownOrContamination" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Power Outage
                            </td>
                            <td class="DataColumn">
                                <asp:CheckBox ID="chkPowerOutage" runat="server" />
                            </td>
                        </tr>
                        <tr id="trSpoilageInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                The BOP Enhancement Endorsement includes Spoilage with a $10,000 limit. Please enter the total amount for limits greater than $10,000.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- OPTIONAL CLASS-BASED COVERAGES START HERE --%>
            <%-- Apartments --%>
            <tr id="trApartmentsCheckboxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkApartments" runat="server" Text="Apartments" CssClass="chkApartmentsSelector" />
                </td>
            </tr>
            <tr id="trApartmentsDataRow" runat="server" style="display:none;" class="trApartmentsRow">
                <td>
                    <table id="tblApartments" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn" >
                                Number of Locations with Apartments
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox id="txtAptsNumberOfLocationsWithApts" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtAptsNumberOfLocationsWithApts"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2" style="text-align:left;text-decoration:underline;font-weight:700;">
                                Tenant Auto Legal Liability
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Limit of Liability (per event)
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlAptAutoLiabilityLimit" runat="server" CssClass="ddlAptAutoLiabilityLimit">
                                    <asp:ListItem value="0">N/A</asp:ListItem>
                                    <asp:ListItem value="161">6,000</asp:ListItem>
                                    <asp:ListItem value="6">7,500</asp:ListItem>
                                    <asp:ListItem value="268">9,000</asp:ListItem>
                                    <asp:ListItem value="375">12,000</asp:ListItem>
                                    <asp:ListItem value="48">15,000</asp:ListItem>
                                    <asp:ListItem value="376">18,000</asp:ListItem>
                                    <asp:ListItem value="377">22,500</asp:ListItem>
                                    <asp:ListItem value="62">30,000</asp:ListItem>
                                    <asp:ListItem value="378">37,500</asp:ListItem>
                                    <asp:ListItem value="282">45,000</asp:ListItem>
                                    <asp:ListItem value="324">60,000</asp:ListItem>
                                    <asp:ListItem value="50">75,000</asp:ListItem>
                                    <asp:ListItem value="379">90,000</asp:ListItem>
                                    <asp:ListItem value="380">120,000</asp:ListItem>
                                    <asp:ListItem value="52">150,000</asp:ListItem>
                                    <asp:ListItem value="381">180,000</asp:ListItem>
                                    <asp:ListItem value="54">225,000</asp:ListItem>
                                    <asp:ListItem value="33">300,000</asp:ListItem>
                                    <asp:ListItem value="382">375,000</asp:ListItem>
                                    <asp:ListItem value="383">450,000</asp:ListItem>
                                    <asp:ListItem value="178">600,000</asp:ListItem>
                                    <asp:ListItem value="180">750,000</asp:ListItem>
                                    <asp:ListItem value="182">900,000</asp:ListItem>
                                    <asp:ListItem value="183">1,200,000</asp:ListItem>
                                    <asp:ListItem value="185">1,500,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Deductible (OTC per Auto Ded/ OTC Max per Event Ded/ Collision Ded)
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlAptDeductible" runat="server" CssClass="ddlAptDeductible">
                                    <asp:ListItem value="0">N/A</asp:ListItem>
                                    <asp:ListItem value="43">250/500/500</asp:ListItem>
                                    <asp:ListItem value="44">250/1,000/500</asp:ListItem>
                                    <asp:ListItem value="45">500/2,500/500</asp:ListItem>
                                    <asp:ListItem value="46">500/2,500/1,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trApartmentsInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                If your insured provides a service for parking (ie – valet services), then coverage is available for Tenants autos while in the insured’s care, at this location.<br /><br />This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Barbers --%>
            <tr id="trBarbersLiabilityCheckBoxRow" runat="server" visible="false" >
                <td>
                    <asp:CheckBox id="chkBarbersLiability" runat="server" Text="Barbers Professional Liability" CssClass="chkBarbersSelector" />
                </td>
            </tr>
            <tr id="trBarbersLiabilityDataRow" runat="server" style="display:none;" class="trBarbersRow">
                <td style="width:100%;">
                    <table id="tblBarbersLiability" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Full Time Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtBarberFullTimeEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtBarbersFullTimeEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Part Time Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtBarberPartTimeEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtBarbersPartTimeEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBarbersInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Professional Coverage is rated on number of employees who provide the professional service. Enter the number of applicable employees for all locations here. 
                                <br /><br />
                                This coverage is grayed out on any other buildings, it can only be added once per policy.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            
            <%-- Beauty Shops --%>
            <tr id="trBeautyShopsCheckBoxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkBeautyShops" runat="server" Text="Beauticians Professional Liability" CssClass="chkBeauticiansSelector" />
                </td>
            </tr>
            <tr id="trBeautyShopsDataRow" runat="server" style="display:none;" class="trBeauticiansRow">
                <td>
                    <table id="tblBeautyShops" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Full Time Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtBeautyShopsNumFullTimeEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtBeautyShopsNumFullTimeEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Part Time Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtBeautyShopsNumPartTimeEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtBeautyShopsNumPartTimeEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trBeautyShopsInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Professional Coverage is rated on number of employees who provide the professional service. Enter the number of applicable employees for all locations here.
                                <br /><br />This coverage is grayed out on any other buildings, it can only be added once per policy.                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Fine Arts --%>
            <tr id="trFineArtsCheckboxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkFineArts" runat="server" Text="Fine Arts" CssClass="chkFineArtsSelector" />
                </td>
            </tr>

            <%-- Funeral Directors --%>
            <tr id="trFuneralCheckBoxRow" runat="server" visible="false" class="FuneralClass">
                <td>
                    <asp:CheckBox id="chkFuneralDirectors" runat="server" Text="Funeral Directors Professional Liability" CssClass="chkFuneralSelector" />
                </td>
            </tr>
            <tr id="trFuneralDataRow" runat="server" style="display:none;" class="trFuneralRow">
                <td>
                    <table id="tblFuneral" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtFuneralNumOfEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtFuneralNumOfEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trFuneralDirectorsInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Professional Coverage is rated on number of employees who provide the professional service. Enter the number of applicable employees for all locations here.
                                <br /><br />
                                This coverage is grayed out on any other buildings, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Liquor Liability --%>
            <tr id="trLiquorLiabilityCheckboxRow" runat="server" visible="false" >
                <td>
                    <asp:CheckBox id="chkLiquorLiability" runat="server" Text="Liquor Liability" />
                </td>
            </tr>
            <tr id="trLiquorLiabilityDataRow" runat="server" style="display:none;" >
                <td>
                    <table id="tblLiquorLiability" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                *Limit
                            </td>
                            <td class="DataColumn">
                                <asp:Label ID="lblLiquorLiabilityLimit" runat="server" Width="50%" Text="$1,000,000"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                *Liquor Liability Class Code
                            </td>
                            <td class="DataColumn">
                                <asp:Label ID="lblLiquorLiabilityClassCode" runat="server" Width="90%" Text="58161 - Restaurant includes Package Sales"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                *Annual Gross Alcohol Sales
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtLiquorLiabiltyAnnualGrossAlcoholSales" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trLiquorLiabilityInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Enter total liquor receipts for all locations. <br /><br />
                                This coverage is grayed out on any other buildings, it can only be added once per policy. <br /><br />
                                Click <a href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPLiquorLiabilityApp")%>" target="_blank">this link</a> for Liquor Liability Application and submit to your Underwriter.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Motels --%>
            <tr id="trMotelCheckboxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkMotel" runat="server" Text="Motel" CssClass="chkMotelSelector" />
                </td>
            </tr>
            <tr id="trMotelDataRow" runat="server" style="display:none;" class="trMotelRow">
                <td>
                    <table id="tblMotel" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                *Liability Limit for Guest Property
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlMotelLiabilityLimit" runat="server" CssClass="ddlMotelLiabilityLimit">
                                    <asp:ListItem value="371">$1,000/$25,000</asp:ListItem>
                                    <asp:ListItem value="372">$2,000/$50,000</asp:ListItem>
                                    <asp:ListItem value="373">$3,000/$75,000</asp:ListItem>
                                    <asp:ListItem value="374">$4,000/$100,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2" style="text-align:left;text-decoration:underline;font-weight:700;">
                                Liability for Guest Property in Safe Deposit Box
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Limit for Safe Deposit Box
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlMotelSafeDepositBoxLimit" runat="server" CssClass="ddlMotelSafeDepositBoxLimit">
                                    <asp:ListItem value="0">N/A</asp:ListItem>
                                    <asp:ListItem value="8">25,000</asp:ListItem>
                                    <asp:ListItem value="9">50,000</asp:ListItem>
                                    <asp:ListItem value="10">100,000</asp:ListItem>
                                    <asp:ListItem value="55">250,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Safe Deposit Box Deductible
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlMotelSafeDepositBoxDeductible" runat="server" CssClass="ddlMotelSafeDepositBoxDeductible">
                                    <asp:ListItem value=""></asp:ListItem>
                                    <asp:ListItem value="40">0</asp:ListItem>
                                    <asp:ListItem value="4">250</asp:ListItem>
                                    <asp:ListItem value="8">500</asp:ListItem>
                                    <asp:ListItem value="9">1,000</asp:ListItem>
                                    <asp:ListItem value="15">2,500</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trMotelInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                The motel endorsement includes a $1,000 per guest property damage limit, with an aggregate limit of $25,000. This limit can be increased above. 
                                <br /><br />
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Optical & Hearing Aid --%>
            <tr id="trOpticalAndHearingAidCheckboxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkOpticalAndHearingAid" runat="server" Text="Optical and Hearing Aid Professional Liability" CssClass="chkOpticalSelector" />
                </td>
            </tr>
            <tr id="trOpticalAndHearingDataRow" runat="server" style="display:none;" class="trOpticalRow">
                <td>
                    <table id="tblOpticalAndHearing" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtOpticalAndHearingNumOfEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtOpticalAndHearingNumOfEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trOpticalAndHearingInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Professional Coverage is rated on number of employees who provide the professional service. Enter the number of applicable employees for all locations here.
                                <br /><br />
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Pharmacists --%>
            <tr id="trPharmacistsCheckBoxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkPharmacists" runat="server" Text="Pharmacists Professional Liability" CssClass="chkPharmacistsSelector" />
                </td>
            </tr>
            <tr id="trPharmacistsDataRow" runat="server" style="display:none;" class="trPharmacistsRow">
                <td>
                    <table id="tblPharmacists" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Receipts
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtPharmacistsReceipts" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtPharmacistsReceipts"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trPharmacistsInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Enter total pharmacy receipts for all locations.
                                <br /><br />
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Photography NEW --%>
            <tr id="trPhotographyCheckboxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkPhotography" runat="server" Text="Photography" class="PhotographyClass" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="chkPhotographySelector" />
                </td>
            </tr>
            <tr id="trPhotographyDataRow" runat="server" class="trPhotograhyRow">
                <td>
                    <table id="tblPhotography" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td>
                                <asp:CheckBox ID="chkPhotographyScheduledEquipment" runat="server" Text="Scheduled Equipment" CssClass="chkPhotographyScheduledEquipment" />
                            </td>
                        </tr>
                        <tr id="trPhotogScheduledEquipmentRow" runat="server" class="trPhotogScheduledEquipmentRow" style="display:none">
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Total Scheduled Equipment Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtPhotogScheduledEquipmentLimit" runat="server" Width="50%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="PhotogScheduledEquipmentLimitTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trPhotographyInfoText" runat="server">
                            <td colspan="3" class="informationalText" style="width:100%;text-align:left;">
                                Your binding authority for this coverage is a maximum limit of $1,000,000 with a per item maximum of $150,000
                                <br /><br />
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                        <tr id="trPhotogScheduledItems" runat="server">
                            <td></td>
                            <td colspan="2">
                                <uc1:ctl_PhotographersEquipment runat="server" ID="ctl_PhotogEquip" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr id="trPhotographyMakeupAndHairCheckboxRow" runat="server" style="display:none;" class="trPhotogMakeupAndHairRow">
                <td>
                    <asp:CheckBox id="chkPhotogMakeupAndHair" runat="server" Text="Makeup and Hair" CssClass="chkPhotogMakeupAndHair" />
                </td>
            </tr>

            <%-- Printers --%>
            <tr id="trPrintersCheckBoxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkPrinters" runat="server" Text="Printers Errors & Omissions Liability" CssClass="chkPrintersSelector" />
                </td>
            </tr>
            <tr id="trPrintersDataRow" runat="server" style="display:none;" class="trPrintersRow">
                <td>
                    <table id="tblPrinters" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Locations
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtPrintersNumOfLocations" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtPrintersNumOfLocations"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trPrintersInfoText" runat="server">
                            <td colspan="3" class="informationalText" style="width:100%;text-align:left;">
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Residential Cleaning --%>
            <tr id="trResidentialCleaningCheckboxRow" runat="server" visible="false">
                <td>
                    <asp:CheckBox id="chkResidentialCleaning" runat="server" Text="Residential Cleaning" />
                </td>
            </tr>

            <%-- Restaurants --%>
            <tr id="trRestaurantCheckboxRow" runat="server" visible="false" >
                <td>
                    <asp:CheckBox id="chkRestaurant" runat="server" Text="Restaurant" CssClass="chkRestaurantSelector" />
                </td>
            </tr>
            <tr id="trRestaurantDataRow" runat="server" style="display:none;" class="trRestaurantRow" >
                <td>
                    <table id="tblRestaurant" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2" style="text-align:left;font-weight:700;text-decoration:underline;">
                                Customer Auto Legal Liability
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Limit of Liability (per event)
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlRestaurantsLimitOfLiability" runat="server" CssClass="ddlRestaurantsLimitOfLiability">
                                    <asp:ListItem value="0">N/A</asp:ListItem>
                                    <asp:ListItem value="161">6,000</asp:ListItem>
                                    <asp:ListItem value="6">7,500</asp:ListItem>
                                    <asp:ListItem value="268">9,000</asp:ListItem>
                                    <asp:ListItem value="375">12,000</asp:ListItem>
                                    <asp:ListItem value="48">15,000</asp:ListItem>
                                    <asp:ListItem value="376">18,000</asp:ListItem>
                                    <asp:ListItem value="377">22,500</asp:ListItem>
                                    <asp:ListItem value="62">30,000</asp:ListItem>
                                    <asp:ListItem value="378">37,500</asp:ListItem>
                                    <asp:ListItem value="282">45,000</asp:ListItem>
                                    <asp:ListItem value="324">60,000</asp:ListItem>
                                    <asp:ListItem value="50">75,000</asp:ListItem>
                                    <asp:ListItem value="379">90,000</asp:ListItem>
                                    <asp:ListItem value="380">120,000</asp:ListItem>
                                    <asp:ListItem value="52">150,000</asp:ListItem>
                                    <asp:ListItem value="381">180,000</asp:ListItem>
                                    <asp:ListItem value="54">225,000</asp:ListItem>
                                    <asp:ListItem value="33">300,000</asp:ListItem>
                                    <asp:ListItem value="382">375,000</asp:ListItem>
                                    <asp:ListItem value="383">450,000</asp:ListItem>
                                    <asp:ListItem value="178">600,000</asp:ListItem>
                                    <asp:ListItem value="180">750,000</asp:ListItem>
                                    <asp:ListItem value="182">900,000</asp:ListItem>
                                    <asp:ListItem value="183">1,200,000</asp:ListItem>
                                    <asp:ListItem value="185">1,500,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Deductible (OTC per Auto / OTC Max / Coll)
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlRestaurantsDeductible" runat="server" CssClass="ddlRestaurantsDeductible">
                                    <asp:ListItem value="0">N/A</asp:ListItem>
                                    <asp:ListItem value="43">250/500/500</asp:ListItem>
                                    <asp:ListItem value="44">250/1,000/500</asp:ListItem>
                                    <asp:ListItem value="45">500/2,500/500</asp:ListItem>
                                    <asp:ListItem value="46">500/2,500/1,000</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trRestaurantsInfoText" runat="server">
                            <td colspan="3" class="informationalText" style="width:100%;text-align:left;">
                                If your insured provides a service for parking (ie – valet services), then coverage is available for Customers autos while in the insured’s care, at this location.
                                <br /><br />
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Self Storage --%>
            <tr id="trSelfStorageCheckBoxRow" runat="server" visible="false" >
                <td>
                    <asp:CheckBox id="chkSelfStorage" runat="server" Text="Self Storage Facility" CssClass="chkSelfStorageSelector" />
                </td>
            </tr>
            <tr id="trSelfStorageDataRow" runat="server" style="display:none;" class="trSelfStorageRow" >
                <td>
                    <table id="tblSelfStorage" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Limit
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtStorageLimit" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtStorageLimit"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trSelfStorageInfoText" runat="server">
                            <td colspan="3" class="informationalText" style="width:100%;text-align:left;">
                                $50,000 of Customer's Goods Legal Liability is included
                                <br /><br />
                                This coverage is grayed out on any other buildings or locations once selected, it can only be added once per policy.                            
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <%-- Veterinarians --%>
            <tr id="trVeterinariansCheckBoxRow" runat="server" visible="false" >
                <td>
                    <asp:CheckBox id="chkVeterinarians" runat="server" Text="Veterinarians Professional Liability" CssClass="chkVeterinariansSelector" />
                </td>
            </tr>
            <tr id="trVeterinariansDataRow" runat="server" style="display:none;" class="trVeterinariansRow">
                <td>
                    <table id="tblVeterinarians" runat="server" style="width:100%;">
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                Number of Employees
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtVeterinariansNumOfEmployees" runat="server" Width="25%" onkeypress='return event.charCode >= 48 && event.charCode <= 57' CssClass="txtVeterinariansNumOfEmployees"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trVeterinariansInfoText" runat="server">
                            <td colspan="3" class="informationalText">
                                Professional Coverage is rated on number of employees who provide the professional service.  Enter the number of applicable employees for all locations here.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr><td >&nbsp;</td></tr>
            <tr id="trOtherOptionalCoveragesInfoText" runat="server">
                <td class="informationalTextRed">*Other optional coverages are available. Please contact your Commercial Underwriter for assistance and approval.</td>
            </tr>
        </table>
        <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
</div>