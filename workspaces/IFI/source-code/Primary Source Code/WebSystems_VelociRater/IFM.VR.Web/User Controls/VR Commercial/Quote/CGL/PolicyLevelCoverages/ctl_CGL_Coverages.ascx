<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_Coverages" %>

<div runat="server" id="divGenInfo">
    <h3>General Information
         <span style="float: right;">        
        <asp:LinkButton ID="btnSaveGenInfo" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
    </h3>
    <div>
        <style type="text/css">
            .CGL_GI_LabelColumn {
                width:50%;
                text-align:left;
            }
            .CGL_GI_DataColumn {
                width:50%;
                text-align:left;
            }
        </style>
        <table id="tblGeneralInfo" runat="server" style="width:100%">
            <tr id="trProgramTypeRow" runat="server" style="display:none;">
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddProgramType.ClientID%>">*Program Type</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddProgramType" runat="server" >
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddOccuranceLibLimit.ClientID%>">*Occurrence Liability Limit</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddOccuranceLibLimit" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddGeneralAgg.ClientID%>">*General Aggregate</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddGeneralAgg" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=txtRented.ClientID%>">Damage to Premises Rented to You</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtRented" Enabled="false" runat="server" value="100,000"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddOperationsAgg.ClientID%>">*Product/Completed Operations Aggregate</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddOperationsAgg" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddMedicalExpense.ClientID%>">Medical Expenses</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddMedicalExpense" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddPersonalInjury.ClientID%>">*Personal and Advertising Injury</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddPersonalInjury" runat="server" Width="137px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CGL_Help_BusinessMasterSummary")%>"><font style="color:blue;" >General Liability Enhancement Endorsement</font></a>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:CheckBox ID="chkGLEnhancement" runat="server" Text="&nbsp;" />
                </td>
            </tr>
             <tr id="trGLPlusEnhancementRow" runat="server" style="display:none;">
                <td class="CGL_GI_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CGL_Help_PlusEnhancementSummary")%>"><font style="color:blue;" >General Liability PLUS Enhancement Endorsement</font></a>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:CheckBox ID="chkGLPlusEnhancement" runat="server" Text="&nbsp;" />
                </td>
            </tr>
            <tr id="trAddGLBlanketWaiverOfSubroRow" runat="server">
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddlAddlBlanketOfSubroOptions.ClientID%>">Add Blanket Waiver of Subrogation?</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropdownList ID="ddlAddlBlanketOfSubroOptions" runat="server">
                        <asp:ListItem Text="No" Value=""></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Yes with Completed Ops" Value="2"></asp:ListItem>                 
                    </asp:DropdownList>                
                </td>
            </tr>
<%--            <tr id="trEnhancementMessageRow" runat="server">
                <td colspan="2">
                    <p class="informationalText">If General Liability Enhancement Endorsement is selected, and Blanket Waiver of Subrogation is yes the Contractors Supplemental Application must be comnpleted and mailed to your underwriter to bind coverage.  Please click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CGL_Help_ContractorsApplication")%>"><font style="color:blue;" >Contractors Application</font></a>.</p>
                </td>
            </tr>--%>
            <tr id="trContractorsGLEnhancementRow" runat="server" style="display:none;">
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=chkContractorsGLEnhancement.ClientID%>">Contractors General Liability Enhancement</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:CheckBox ID="chkContractorsGLEnhancement" Enabled="false" runat="server" Text="&nbsp;" />
                </td>
            </tr>
            <tr id="trManufacturersGLEnhancement" runat="server" style="display:none;">
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=chkManufacturersGLEnhancement.ClientID%>">Manufacturers General Liability Enhancement</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:CheckBox ID="chkManufacturersGLEnhancement" Enabled="false" runat="server" Text="&nbsp;" />
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddlAddAGLDeductible.ClientID%>">Add a General Liability Deductible?</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddlAddAGLDeductible" runat="server">
                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                        <asp:ListItem Text="No" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                    <%--<asp:CheckBox ID="chkAddaGeneralLiabilityDeductible" runat="server" Text="&nbsp;" />--%>
                </td>
            </tr>
        </table>
    </div>
</div>

<div id="divDeductibles" runat="server">
    <h3>
        Deductibles
            <span style="float: right;">
                <asp:LinkButton ID="btnSaveDeductible" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
            </span>
    </h3>
    <div runat="server" id="divDeductiblesData">
        <table>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddType.ClientID%>">*Type</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddType" runat="server" Width="230px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddAmount.ClientID%>">*Amount</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddAmount" runat="server" Width="230px">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="CGL_GI_LabelColumn">
                    <label for="<%=ddBasis.ClientID%>">*Basis</label>
                </td>
                <td class="CGL_GI_DataColumn">
                    <asp:DropDownList ID="ddBasis" runat="server" Width="230px">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
</div>

<div runat="server" id="divCoverages">
    <h3>Policy Level Coverages
        <span style="float: right;">        
        <asp:LinkButton ID="btnSaveCoverages" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>

    </h3>
    <div>
        <style>
            .CGL_Cov_SpacerColumn {
                width:5%;
            }
            .CGL_Cov_LabelColumn {
                text-align:right;
                width:32%;
            }
            .CGL_Cov_DataColumn {
                width:63%;
                text-align:left;
            }
            .CGL_Cov_ShortTextBox {
                width:10%;
            }
        </style>
        <table id="tblCovs" runat="server">
            <!-- NEW Add'l Insureds -->
            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkAdditionalInsured" runat="server" Text="Additional Insured" />
                </td>
            </tr>
            <tr id="trAdditionalInsuredsRow" runat="server" style="display:none;">
                <td colspan="3">
                    <table>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td class="LabelColumn">
                                How many of the following Additional Insureds Apply?
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlNumberOfAddlInsureds" runat="server" CssClass="MediumControl">
                                    <asp:ListItem Value="0">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                    <asp:ListItem Value="4">4 or more</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2" >
                                <ul>
                                    <li>Vendors</li>
                                    <li>Designated Person or Organization</li>
                                    <li>Engineers, Architects, or Surveyors not Engaged by the Named Insured</li>
                                    <li>Owners, Lessees or Contractors </li>
                                    <li>Lessor of Leased Equipment </li>
                                    <li>Managers or Lessors of Premises </li>
                                </ul>
                            </td>
                        </tr>
                        <tr>
                            <td class="SpacerColumn">&nbsp;</td>
                            <td colspan="2">
                                <table style="width:100%;">
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            Check the boxes if the policy has any of the following AI:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_CoOwnerOfInsuredPremises" runat="server"/>Co-Owner of Insured Premises
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_ControllingInterests" runat="server"/>Controlling Interests
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_EngineersArchitectsOrSurveyors" runat="server"/>Engineers, Architects or Surveyors
                                        </td>
                                    </tr>
<%--                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_EngineersArchitectsOrSurveyorsNotEngaged" runat="server"/>Engineers, Architects or Surveyors Not Engaged by the Named Insured
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_MortgageeAssigneeOrReceiver" runat="server"/>Mortgagee, Assignee or Receiver
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SpacerColumn"></td>
                                        <td colspan="2">
                                            <asp:CheckBox ID="chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased" runat="server"/>Owner or Other Interests from Whom Land has been Leased
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!-- OLD Add'l Insured -->
<%--            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkAdditional" Text="Additional Insured" runat="server" />
                </td>
            </tr>
            <tr id="trAddlInsuredDataRow" runat="server">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn">
                    Number of Additional Insureds
                </td>
                <td>
                    <asp:TextBox ID="txtNumberOfAddlInsureds" runat="server" CssClass="CGL_Cov_ShortTextBox"  onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="trAddlInsuredsInfoRow" runat="server">
                <td colspan="3" class="informationalText">
                    Up to 4 Additional Insureds available. If more than 4 are requested please contact your underwriter.
                </td>
            </tr>--%>
            <tr id="trCondoDAndORow" runat="server" style="display:none;">
                <td colspan="3">
                    <asp:CheckBox ID="chkCondoDAndO" Text="Condo Directors and Officers - Claims-Made Basis" runat="server" />
                </td>
            </tr>
            <tr id="trCondoDAndODataRow1" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtNamedAssociation.ClientID%>">Named Association (when different from named insured)</label></td>
                <td class="CGL_Cov_DataColumn">
                    <asp:TextBox ID="txtNamedAssociation" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr id="trCondoDAndODataRow2" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtCondoDAndOLimit.ClientID%>">Limit</label></td>
                <td class="CGL_Cov_DataColumn">
                    <asp:TextBox ID="txtCondoDAndOLimit" runat="server" Enabled="false" Text="$1,000,000"></asp:TextBox>
                </td>
            </tr>
            <tr id="trCondoDAndODataRow3" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=ddCondoDAndODeductible.ClientID%>">Deductible</label></td>
                <td class="CGL_Cov_DataColumn">
                    <asp:DropDownList ID="ddCondoDAndODeductible" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <%--CLI start--%>
            <tr id="chkCLIWrapper" runat="server">
                <td colspan="3">
                    <asp:CheckBox ID="chkCLI" Text="Cyber Liability" runat="server" />
                </td>
            </tr>
            <tr id="trCLIInfoRow1" runat="server">
                <td colspan="3">
                    <table style="width:100%;">
                        <tr>
                            <td style="width:3%;">&nbsp;</td>
                            <td style="width:12%;text-align:left;">
                                Aggregate Limit
                            </td>
                            <td style="width:12%;text-align:left;">$50,000</td>
                            <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                            <td style="width:12%;text-align:left;">
                                Deductible
                            </td>
                            <td style="width:12%;text-align:left;">$2,500</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trCLIInfoRow2" runat="server">
                <td colspan="3"  class="informationalText">
                    *For higher limits please contact your Underwriter
                </td>
            </tr>
            <%--CLI end--%>
            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkEmployee" Text="Employee Benefits Liability" runat="server" />
                </td>
            </tr>
            <tr id="trEmployeeBenefitsLiabilityDataRow1" runat="server">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtEmployeeOccurrenceLimit.ClientID%>">Each Employee Limit</label></td>
                <td class="CGL_Cov_DataColumn">
                    <asp:TextBox ID="txtEmployeeOccurrenceLimit" Enabled="false" Text="500,000" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr id="trEmployeeBenefitsLiabilityDataRow2" runat="server">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtEmployeeNumberOfEmployees.ClientID%>">*Number of Employees</label></td>
                <td class="CGL_Cov_DataColumn">
                    <asp:TextBox ID="txtEmployeeNumberOfEmployees" runat="server" CssClass="CGL_Cov_ShortTextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="trEmployeeBenefitsLiabilityInfoRow" runat="server">
                <td colspan="3" class="informationalText">
                    *Please contact your underwriter if a lower Employee Benefits Liability limit is requested.
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkEPLI" Text="Employment Practices Liability - Claims-Made Basis" runat="server" />
                </td>
            </tr>
            <tr id="trEPLIInfoRow1" runat="server">
                <td colspan="3">
                    <table style="width:100%;">
                        <tr>
                            <td style="width:3%;">&nbsp;</td>
                            <td style="width:12%;text-align:left;">
                                Each "Claim" Limit
                            </td>
                            <td style="width:12%;text-align:left;">$100,000</td>
                            <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                            <td style="width:12%;text-align:left;">
                                Aggregate Limit
                            </td>
                            <td style="width:12%;text-align:left;">$100,000</td>
                        </tr>
                        <tr>
                            <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                            <td style="width:12%;text-align:left;">
                                Deductible
                            </td>
                            <td style="width:12%;text-align:left;">$5,000</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trEPLIInfoRow2" runat="server">
                <td colspan="3"  class="informationalText">
                    *For Underwritten product please contact your Underwriter.
                </td>
            </tr>
            <tr id="trStopGapCheckboxRow" runat="server" style="display:none;width:100%;">
                <td colspan="3">
                    <asp:CheckBox ID="chkStopGap" runat="server" Text="Stop Gap (OH)" />&nbsp;
                </td>
            </tr>
            <tr id="trStopGapDataRow" runat="server" style="width:100%;text-indent:15px;">
                <td>
                    <table style="width:100%;">
                        <tr>
                            <td>
                                <asp:Label ID="lblStopGapLimit" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblStopGapPayroll" runat="server" Text="Ohio Only Payroll"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddStopGapLimit" runat="server" ></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtStopGapPayroll" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkHired" Text="Hired/Non-Owned Autos" runat="server" />
                </td>
            </tr>
            <tr id="trLiquorLiabilityCheckBoxRow_IN" runat="server">
                <td colspan="3">
                    <asp:CheckBox ID="chkLiquor_IN" Text="Liquor Liability (Indiana/Ohio Coverage only)" runat="server" />
                </td>
            </tr>
            <tr id="trLiquorInfoRow_IN" runat="server">
                <td colspan="3" class="informationalText">
                    If liquor coverage is desired, please complete the <a href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPLiquorLiabilityApp")%>" target="_blank"><b>Liquor Liability Application</b></a> or liquor section of the <a href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPRestaurantSupplementalApp")%>" target="_blank"><b>Restaurant Supplemental App</b></a>, and send to your underwriter prior to binding coverage.
                </td>
            </tr>
            <tr id="trLiquorDataRow1_IN" runat="server">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtLiquorOccurrenceLimit_IN.ClientID%>">Occurrence Limit</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtLiquorOccurrenceLimit_IN" Text="" Enabled="false" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr id="chkManufacturerLiquorSalesTableRow_IN" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkManufacturerLiquorSales_IN" Text="Manufacturer, WholeSalers & Distributors" runat="server"/>
                </td>
            </tr>
            <tr id="txtManufacturerLiquorSalesTableRow_IN" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtManufacturerLiquorSales_IN.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtManufacturerLiquorSales_IN" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="chkRestaurantLiquorSalesTableRow_IN" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkRestaurantLiquorSales_IN" Text="Restaurants Or Hotels" runat="server"/>
                </td>
            </tr>
            <tr id="txtRestaurantLiquorSalesTableRow_IN" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtRestaurantLiquorSales_IN.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtRestaurantLiquorSales_IN" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="chkPackageStoreLiquorSalesTableRow_IN" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkPackageStoreLiquorSales_IN" Text="Package Stores" runat="server"/>
                </td>
            </tr>
            <tr id="txtPackageStoreLiquorSalesTableRow_IN" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtPackageStoreLiquorSales_IN.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtPackageStoreLiquorSales_IN" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="chkClubLiquorSalesTableRow_IN" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkClubLiquorSales_IN" Text="Clubs" runat="server"/>
                </td>
            </tr>
            <tr id="txtClubLiquorSalesTableRow_IN" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtClubLiquorSales_IN.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtClubLiquorSales_IN" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="trLiquorInfoRow2_IN" runat="server">
                <td colspan="3" class="informationalText">
                    Please contact your underwriter if a lower Liquor Liability limit is requested.
                </td>
            </tr>
<!-- ILLINOIS LIQUOR LIAB -->
            <tr id="trLiquorLiabilityCheckboxRow_IL" runat="server">
                <td colspan="3" >
                    <asp:CheckBox ID="chkLiquor_IL" Text="Liquor Liability (Illinois Coverage only)" runat="server" />
                </td>
            </tr>
            <tr id="trLiquorInfoRow_IL" runat="server" >
                <td colspan="3" class="informationalText">
                    If liquor coverage is desired, please complete the <a href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPLiquorLiabilityApp")%>" target="_blank"><b>Liquor Liability Application</b></a> or liquor section of the <a href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPRestaurantSupplementalApp")%>" target="_blank"><b>Restaurant Supplemental App</b></a>, and send to your underwriter prior to binding coverage.
                </td>
            </tr>
            <tr id="trLiquorInfoRow2_IL">
                <td colspan="3" style="text-align:center">
                    Each Person BI Limit/Each Person PD Limit/Loss of Means of Support
                </td>
            </tr>
            <tr id="trLiquorDataRow1_IL" runat="server">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtLiquorLiabilityLimit_IL.ClientID%>">Occurrence Limit</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtLiquorLiabilityLimit_IL" Text="" Enabled="false" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr id="chkManufacturerLiquorSalesTableRow_IL" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkManufacturerLiquorSales_IL" Text="Manufacturer, WholeSalers & Distributors" runat="server"/>
                </td>
            </tr>
            <tr id="txtManufacturerLiquorSalesTableRow_IL" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtManufacturerLiquorSales_IL.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtManufacturerLiquorSales_IL" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="chkRestaurantLiquorSalesTableRow_IL" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkRestaurantLiquorSales_IL" Text="Restaurants Or Hotels" runat="server"/>
                </td>
            </tr>
            <tr id="txtRestaurantLiquorSalesTableRow_IL" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtRestaurantLiquorSales_IL.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtRestaurantLiquorSales_IL" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="chkPackageStoreLiquorSalesTableRow_IL" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkPackageStoreLiquorSales_IL" Text="Package Stores" runat="server"/>
                </td>
            </tr>
            <tr id="txtPackageStoreLiquorSalesTableRow_IL" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtPackageStoreLiquorSales_IL.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtPackageStoreLiquorSales_IL" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="chkClubLiquorSalesTableRow_IL" runat="server">
                <td class="SpacerColumn"></td>
                <td colspan="2">
                    <asp:CheckBox ID="chkClubLiquorSales_IL" Text="Clubs" runat="server"/>
                </td>
            </tr>
            <tr id="txtClubLiquorSalesTableRow_IL" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td class="CGL_Cov_LabelColumn"><label for="<%=txtClubLiquorSales_IL.ClientID%>">Liquor Sales</label></td>
                <td class="CGL_GI_DataColumn">
                    <asp:TextBox ID="txtClubLiquorSales_IL" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="trLiquorInfoRow3_IL" runat="server">
                <td colspan="3" class="informationalText">
                    Please contact your underwriter if a lower Liquor Liability limit is requested.
                </td>
            </tr>
<!-- ILLINOIS LIQUOR LIAB -->

            <tr id="trContractorsHomeRepairAndRemodelingRow" runat="server" style="display:none;">
                <td colspan="3">
                    <asp:CheckBox ID="chkContractorsHomeRepairAndRemodeling_IL" Text="IL Contractors - Home Repair & Remodeling" runat="server" />
                </td>
            </tr>
            <tr id="trContractorsHomeRepairAndRemodelingInfoRow" runat="server" style="display:none;">
                <td class="CGL_Cov_SpacerColumn">&nbsp;</td>
                <td colspan="2">
                    Limit: $10,000
                </td>
            </tr>
        </table>
    </div>
</div>

<asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
<asp:HiddenField ID="hdnAccordCoverages" runat="server" />
<asp:HiddenField ID="hdnAccordDeductible" runat="server" />
