<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCoverage_PPA.ascx.vb" Inherits="IFM.VR.Web.ctlCoverage_PPA" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlCoverage_PPA_Vehicle_List.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA_Vehicle_List" %>



<div id="divPolicyCoverage" runat="server">
    <div id="dvCoverageType" runat="server">
        <h3>
            <asp:Label ID="lblLiabType" runat="server" Text="Liability Type"></asp:Label>
            <asp:DropDownList ID="ddLiabType" runat="server" autofocus="True"></asp:DropDownList>
            <span style="float: right">
                <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" Style="margin-left: 18px;" OnClientClick="var confirmed = confirm('Clear Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Coverages to Default Values" runat="server">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" Style="margin-left: 18px;" ToolTip="Save all vehicles." runat="server">Save</asp:LinkButton>
            </span>
        </h3>
    </div>
    <div id="dvPolicyCoverage" runat="server">
        <table style="width: 100%" title="Vehicle Coverages (applies to all listed vehicles)">
            <tr>
                <td style="vertical-align: top; width: 50%">
                    <div id="dvR1C1" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td class="auto-style1">
                                    <asp:Label ID="lblBodilyInjury" runat="server" Text="Bodily Injury"></asp:Label>
                                </td>
                                <td class="auto-style2">
                                    <asp:DropDownList ID="ddBodilyInjury" Width="145px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblPropertyDamage" runat="server" Text="Property Damage"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddPropertyDamage" Width="90px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR2C1" runat="server" style="display:none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblSingleLimitLib" runat="server" Text="Single Limit Liability"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddSingleLimitLib" Width="90px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR3C1" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblMedicalPayments" runat="server" Text="Medical Payments"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddmedicalPayments" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                                    <div id="PersonalLiabilitylimitTextPPACSL" runat="server" style="display:none">
                        <table style="width: 100%">
                            <tr>
                                <td colspan="2">
                                    <span>We require a combined single limit of $300,000
                                        <br />
                                        when quoting an umbrella.</span>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="PersonalLiabilitylimitTextPPASPLIT" runat="server" style="display: none">
                        <table style="width: 100%">
                            <tr>
                                <td colspan="2">
                                    <span>We require minimum split limits of 250/5000<br />
                                        when quoting an umbrella.</span>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td style="vertical-align: top">
                    <div id="dvR1C2" runat="server" style="display:none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUmUimSSl" runat="server" Text="UM/UIM CSL"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUmUimSSl" Width="90px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR1C2SE" runat="server" style="display:none">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUMCSL" runat="server" Text="UM CSL"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUMCSL" Width="90px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUIMCSL" runat="server" Text="UIM CSL"></asp:Label>
                                </td>
                                <td>
                                    <asp:Textbox ID="txtUIMCSL" Width="90px" runat="server"></asp:Textbox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR2C2" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUmUmiBi" runat="server" Text="UM/UIM BI"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUmUmiBi" Width="145px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR2C2SE" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUMBI" runat="server" Text="UM BI"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUMBI" Width="145px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUIMBI" runat="server" Text="UIM BI"></asp:Label>
                                </td>
                                <td>
                                    <asp:Textbox ID="txtUIMBI" Width="130px" runat="server"></asp:Textbox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR3C2" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUmPd" runat="server" Text="UM PD"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUmPd" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvR4C2" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="lblUmPdDeduct" runat="server" Text="UM PD Deductible"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUmPdDeductible" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvPriorBi" runat="server">
                         <table style="width: 100%">
                            <tr>
                                <td style="width: 40%">
                                    <asp:Label ID="Label1" runat="server" Text="Prior BI Limit"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddPriorBiLimit" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
       
        <br />
        <div id="dvAutoEnhance" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 75.5%">
                        <asp:LinkButton ID="lnkAutoEnhance" runat="server" OnClientClick="return false;">Auto Enhancement Endorsement</asp:LinkButton>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAutoEnhance" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="dvAutoPlusEnhance" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 75.5%">
                        <asp:LinkButton ID="lnkAutoPlusEnhance" runat="server" OnClientClick="return false;">Auto Plus Enhancement</asp:LinkButton>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAutoPlusEnhance" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="dvMultiPolicy" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 75.5%">
                        <asp:Label ID="lblMultiPolicy" runat="server" Text="Related Homeowners/Farmowners Quote"></asp:Label><br />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkMultiPolicyDiscount" runat="server" Checked="false" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="dvMarketCredit" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 75.5%">
                        <asp:Label ID="lblMarketCredit" runat="server" Text="NO children of any age in the household"></asp:Label><br />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkSelectMarketCredit" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divParachuteDiscounts" runat="server">
             <table style="width: 100%">
                <tr>
                    <td style="width: 75.5%">
                        <asp:Label ID="lblAutoHomeDiscount" runat="server" Text="Home/Auto discount (HOM or FAR w/primary residence)"></asp:Label><br />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAutoHomeDiscount_Parachute" runat="server"  />
                    </td>
                </tr>
                <tr id="tr_RelatedHomeFarm" runat="server">
                     <td colspan="2">
                        <div style="margin-left: 30px;">
                            <span style="margin-bottom: 5px;">Quote/Policy Info</span>
                            <br />
                            <asp:TextBox ID="txtMoreInfo" Width="390px" Heigth="50px;" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </td>
                 </tr>
            </table>
             <table style="width: 100%">
                <tr>
                    <td style="width: 75.5%">
                        <asp:Label ID="lblMultiLineDiscount" runat="server" Text="MultiLine Discount (A total of your commercial polices, umbrellas, and non-primary residence personal policies)"></asp:Label><br />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddMultiLineDiscount_Parachute" runat="server">
                    <asp:ListItem Text="0" Value="4"></asp:ListItem>
                    <asp:ListItem Text="1" Value="5"></asp:ListItem>
                    <asp:ListItem Text="2" Value="6"></asp:ListItem>
                    <asp:ListItem Text="3+" Value="7"></asp:ListItem>
                </asp:DropDownList>
                    </td>
                </tr>
            </table>
           
        </div>
    </div>
    <asp:HiddenField ID="hiddenPolicyCoverage" runat="server" />
    <asp:HiddenField ID="hiddenAutoEnhancement" runat="server" Value="True" />
    <asp:HiddenField ID="hidden_VehicleCoverageList" Value="0" runat="server" />
    <asp:HiddenField ID="hiddenCompOnly" runat="server" />
    <asp:HiddenField ID="hiddenVehCovPlanList" runat="server" />
    <asp:HiddenField ID="hiddenAutoPlusEnhancementEffectiveDate" runat="server" />
</div>
<div id="dvCompOnlyPopup" runat="server" style="display: none">
    <table style="width:100%">
        <tr>
            <td align="center" style="min-height:225px; height:225px">
                <asp:Label ID="lblCompOnlyMsg" runat="server" Text="PL/PD coverages do not apply to selected vehicle" Font-Bold="true" Font-Size="Large"></asp:Label>
            </td>
        </tr>
    </table>
</div>
<uc1:ctlCoverage_PPA_Vehicle_List runat="server" ID="ctlCoverage_PPA_Vehicle_List" />
