<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_Coverages" %>

<div runat="server" id="divGenInfo">
    <h3>General Information
         <span style="float: right;">        
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
    </h3>
    <div>
        <style type="text/css">
            .CPR_GI_LabelColumn {
                width:50%;
                text-align:left;
            }
            .CPR_GI_DataColumn {
                width:50%;
                text-align:left;
            }
            .CPR_StdDdl {
                width:60%;
            }
        </style>
        <table id="tblGeneralInfo" runat="server" style="width:100%">
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddPolicyType.ClientID%>">*Policy Type</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddPolicyType" runat="server" CssClass="CPR_StdDdl" ></asp:DropDownList>
                </td>
            </tr>
            <tr id="trPackageTypeRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddPackageType.ClientID%>">Package Type</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddPackageType" runat="server" CssClass="CPR_StdDdl" ></asp:DropDownList>
                </td>
            </tr>
            <tr id="trPackageModificationAssignmentTypeRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddPackageModificationAssignmentType.ClientID%>">Package Modification Assignment Type</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddPackageModificationAssignmentType" runat="server" CssClass="CPR_StdDdl" ></asp:DropDownList>
                </td>
            </tr>
            <tr id="trPkgModHotelTypeInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    Review <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CL_Help_HotelGuidelines")%>"><font style="color:blue;font-weight:700;" >Hotel Guidelines</font></a>. <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CL_Help_HotelSupplementalApplication")%>"><font style="color:blue;font-weight:700;" >Hotel Supplemental App</font></a> is required.
                </td>
            </tr>
            <tr id="trPkgModApartmentTypeInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    Please review the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPHabitationalGuidelines")%>"><font style="color:blue;font-weight:700;" >Habitational Guidelines</font></a> for Eligibility. If coverage is being requested, this <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPHabitationalSupplementalApplication")%>"><font style="color:blue;font-weight:700;" >Habitational App</font></a> needs to be completed and uploaded.
                </td>
            </tr>
            <tr id="trPkgModContractorsTypeInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPContractorsSupplementalApp")%>"><font style="color:blue;font-weight:700;" >Contractors Supplemental App</font></a> will be required.
                </td>
            </tr>
            <tr id="trPkgModRestaurantTypeInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    If your risk is a restaurant you must fill out and return the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("BOP_Help_BOPRestaurantSupplementalApp")%>"><font style="color:blue;font-weight:700;" >Restaurant Supplemental App</font></a>.
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPR_Help_BusinessMasterSummary")%>"><font style="color:blue;" >Property Enhancement Endorsement</font></a>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:CheckBox ID="chkEnhancement" runat="server" Text="" />
                </td>
            </tr>
           
            <tr id="trPlusEnhancementRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPR_Help_PropPlusEnhancementSummary")%>"><font style="color:blue;" >Property PLUS Enhancement Endorsement</font></a>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:CheckBox ID="chkPlusEnhancement" runat="server" Text="" />
                </td>
            </tr>

            <tr id="trContractorsEnhancementRow" runat="server" style="display:none;">
                <td class="CPP_GIProperty_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPP_Help_ContractorsEnhancementSummary")%>"><font style="color:blue;" >Contractors Enhancement Package</font></a>
                </td>
                <td class="CPP_GIProperty_DataColumn">
                    <asp:CheckBox ID="chkContractorsEnhancementPackage" runat="server" Text="" />
                </td>
            </tr>
            <tr id="trContractorsEnhancementInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    If Contractors Enhancement Package is selected, the Contractors Supplemental Application must be completed and mailed to your underwriter to bind coverage.  Click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPP_Help_ContractorsEnhancementApplication")%>"><font style="color:blue;font-weight:700;" >Contractors Application</font></a>.
                </td>
            </tr>
            <tr id="trManufacturersEnhancementRow" runat="server" style="display:none;">
                <td class="CPP_GIProperty_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPP_Help_ManufacturersEnhancementSummary")%>"><font style="color:blue;" >Manufacturers Enhancement Package</font></a>
                </td>
                <td class="CPP_GIProperty_DataColumn">
                    <asp:CheckBox ID="chkManufacturersEnhancementPackage" runat="server" Text="" />
                </td>
            </tr>
            <tr id="trManufacturersEnhancementInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    If Manufacturers Enhancement Package is selected, the Manufacturers Supplemental Application must be completed and mailed to your underwriter to bind coverage.  Click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPP_Help_ManufacturersApplication")%>"><font style="color:blue;font-weight:700;" >Manufacturers Application</font></a>.
                </td>
            </tr>
            <tr id="trFoodManufacturersEnhancementRow" runat="server" style="display:none;">
                <td class="CPP_GIProperty_LabelColumn">
                    <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPP_Help_FoodManufacturersEnhancementSummary")%>"><font style="color:blue;" >Food Manufacturers Enhancement Endorsement Package</font></a>
                </td>
                <td class="CPP_GIProperty_DataColumn" style="vertical-align:top;">
                    <asp:CheckBox ID="chkFoodManufacturersEnhancementPackage" runat="server" Text="" />
                </td>
            </tr>
            <tr id="trFoodManufacturersEnhancementInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    If Food Manufacturers Enhancement Endorsement Package is selected, the Food Manufacturers Supplemental Application must be completed and mailed to your underwriter to bind coverage.  Click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPP_Help_FoodManufacturersApplication")%>"><font style="color:blue;font-weight:700;" >Food Manufacturers Application</font></a>.
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddBlanketRating.ClientID%>">Blanket Rating</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddBlanketRating" runat="server" ></asp:DropDownList>
                </td>
            </tr>
<%--            <tr id="trBlanketRatingInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    <b>When blanket is selected, co-insurance must be at least 90% on items which the blanket applies.  </b>If blanket is unchecked you may select a co-insurance of less than 90% at the location/building coverage/Property in the Open.
                </td>
            </tr>--%>
            <tr id="trBlanketCauseOfLossRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddBlanketCauseOfLoss.ClientID%>">Blanket Cause of Loss</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddBlanketCauseOfLoss" runat="server" CssClass="CPR_StdDdl"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trBlanketCoinsuranceRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddBlanketCoinsurance.ClientID%>">Blanket Coinsurance</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddBlanketCoinsurance" runat="server" CssClass="CPR_StdDdl"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trBlanketValuationRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddBlanketValuation.ClientID%>">Blanket Valuation</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddBlanketValuation" runat="server" CssClass="CPR_StdDdl"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trDeductibleRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddDeductible.ClientID%>">Blanket Deductible</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddDeductible" runat="server" CssClass="CPR_StdDdl"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trAgreedAmountRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=chkAgreedAmount.ClientID%>">Agreed Amount</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:CheckBox ID="chkAgreedAmount" runat="server" Text="" />
                </td>
            </tr>
            <tr id="trAgreedAmountInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    Blanket and/or Agreed Amount require a signed statement of values.  Please forward this to your underwriter upon binding coverage.
                </td>
            </tr>
            <tr>
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=chkBusinessIncomeALS.ClientID%>">Business Income ALS</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:CheckBox ID="chkBusinessIncomeALS" runat="server" Text="" />
                </td>
            </tr>
            <tr id="trBIALSLimitRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=txtBusinessIncomeALSLimit.ClientID%>">*Business Income ALS Limit</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:TextBox ID="txtBusinessIncomeALSLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                </td>
            </tr>
            <tr id="trBIALSWaitingPeriodRow" runat="server" style="display:none;">
                <td class="CPR_GI_LabelColumn">
                    <label for="<%=ddBusinessIncomeALSWaitingPeriod.ClientID%>">*Waiting Period</label>
                </td>
                <td class="CPR_GI_DataColumn">
                    <asp:DropDownList ID="ddBusinessIncomeALSWaitingPeriod" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trBIALSInfoRow" runat="server" style="display:none;">
                <td colspan="2" class="informationalText">
                    If ALS coverage is desired, the limit of insurance will appear on the declaration page as BUSINESS INCOME - 12 MONTHS - ALS.  However, to determine the appropriate value for rating purposes please click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPR_Help_ALSWorksheet")%>"><font style="color:blue;font-weight:700;" >ALS Worksheet</font></a>.
                </td>
            </tr>
        </table>
    </div>
</div>

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 400px;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Policy Level Coverages and continue to Locations." Text="Save Policy Level Coverages" />
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/> 
    <asp:Button ID="btnSaveAndGotoLocations" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Location" /><br />
</div>

<asp:HiddenField ID="hdnAccord" runat="server" />
<asp:HiddenField ID="hdnAgreedAmountValue" runat="server" />
<asp:HiddenField ID="hdnEffDate" runat="server" />
