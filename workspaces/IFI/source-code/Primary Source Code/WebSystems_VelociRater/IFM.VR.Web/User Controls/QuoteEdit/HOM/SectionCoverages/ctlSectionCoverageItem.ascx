<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlSectionCoverageItem.ascx.vb" Inherits="IFM.VR.Web.ctlSectionCoverageItem" %>

<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSpecifiedStructureList.ascx" TagPrefix="uc1" TagName="ctlHomSpecifiedStructureList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalResidenceList.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalResidenceList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomFarmLandList.ascx" TagPrefix="uc1" TagName="ctlHomFarmLandList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomMultipleNamesList.ascx" TagPrefix="uc1" TagName="ctlHomMultipleNamesList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomOtherMembersList.ascx" TagPrefix="uc1" TagName="ctlHomOtherMembersList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInterestsList.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInterestsList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomTrustList.ascx" TagPrefix="uc1" TagName="ctlHomTrustList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInsuredList.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomLocation.ascx" TagPrefix="uc1" TagName="ctlHomLocation" %>
<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>

<div>    
    <asp:CheckBox ID="chkCov" runat="server" CssClass="homOptionalCovChk" />    
    <asp:Label AssociatedControlID="chkCov" ID="lblHeader" runat="server" Text=""></asp:Label>
    <asp:HyperLink ID="lnkHelp" Target="_blank" runat="server"></asp:HyperLink>
    <div id="divDetails" runat="server">
        <table style="width:100%">
            <tr id="trStandard" runat="server">
                <td>
                    <label for="<%=txtIncludedLimit.ClientID%>">Included Limit</label>
                    <br />
                    <asp:TextBox ID="txtIncludedLimit" Width="100" runat="server"></asp:TextBox>
                    <div runat="server" visible="false" id="divHdnCoverageCode" style="display:inline-block;"><!-- 2/12/18 added for HOM Upgrade MLW, used in js -->
                        <asp:HiddenField ID="hdnCoverageCode" Value="0" runat="server" />
                    </div>
                </td>
                <td>
                    <label for="<%=txtIncreaseLimit.ClientID%>">Increased Limit</label>
                    <br />
                    <asp:TextBox ID="txtIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="ddIncreasedLimit" Width="100" runat="server"></asp:DropDownList>
                </td>
                <td>
                    <label for="<%=txtTotalLimit.ClientID%>">Total Limit</label>
                    <br />
                    <asp:TextBox ID="txtTotalLimit" Width="100" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr id="trBatteryBackupText" runat="server" style="display:none;">
                <td colspan="3" class="informationalText">If limit is $25,000 or greater. Battery Backup is required on all sump pumps.</td>
            </tr>
            <tr id="trSingleInputs" runat="server">
                <td colspan="3">
                    <div runat="server" visible="false" id="divDeductible" style="display:inline-block;">
                        Deductible
                        <br />
                        <asp:DropDownList ID="ddDeductible" Width="100" runat="server"></asp:DropDownList>
                    </div>
                    <div runat="server" id="divEffectiveDate" visible="false" style="display:inline-block;">
                         <asp:Label ID="lblEffectiveDate" runat="server">Effective Date</asp:Label>
                        <br />
                        <asp:TextBox ID="txtEffectiveDate" Width="100" runat="server"></asp:TextBox>
                    </div>   
                    <div runat="server" id="divExpirationDate" visible="false" style="display:inline-block;"><!-- Added 1/15/18 for HOM Upgrade MLW -->
                        <asp:Label ID="lblExpirationDate" runat="server">Expiration Date</asp:Label>
                        <br />
                        <asp:TextBox ID="txtExpirationDate" Width="100" runat="server"></asp:TextBox>
                    </div>                                     
                    <div runat="server" id="divLimit" visible="false" style="display:inline-block;">
                        <asp:Label ID="lblLimit" runat="server">Limit</asp:Label>
                        <br />
                        <asp:TextBox ID="txtLimit" Width="100" runat="server"></asp:TextBox>
                    </div>
                    <div runat="server" id="divDescription" visible="false" style="display:inline-block;">
                         <asp:Label ID="lblDescription" runat="server">Description</asp:Label><!-- Updated 7/9/19 asterisk for Home Endorsements Project Task 38908 and 38915 MLW -->
                        <br />
                        <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="300" runat="server"></asp:TextBox>
                        <!-- Added 7/8/19 max chars for Home Endorsements Project Task 38908 MLW -->
                        <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                        <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
                        <asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
                    </div>
                </td>
            </tr>
            <tr id="trBusinessPursuits" runat="server">
                <td colspan="3"><!-- Added 1/24/18 for HOM Upgrade MLW -->
                    <table style="width:100%">
                        <tr>
                            <td>
                                <div runat="server" visible="false" id="divInsuredFirstName" style="display:block;">
                                    *Insured First Name:
                                    <br />
                                    <asp:TextBox ID="txtInsuredFirstName" Width="125" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                                <div runat="server" visible="false" id="divInsuredMiddleName" style="display:block;">
                                    Insured Middle Name:
                                    <br />
                                    <asp:TextBox ID="txtInsuredMiddleName" Width="125" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                                <div runat="server" visible="false" id="divInsuredLastName" style="display:block;">
                                    *Insured Last Name:
                                    <br />
                                    <asp:TextBox ID="txtInsuredLastName" Width="125" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                                <div runat="server" visible="false" id="divInsuredSuffixName" style="display:block;">
                                    Suffix
                                    <br />
                                    <asp:DropDownList ID="ddInsuredSuffixName" runat="server"></asp:DropDownList>
                                </div>
                            </td>
                            <td style="vertical-align:top;">
                                <div runat="server" visible="false" id="divInsuredBusinessName" style="display:inline-block;">
                                    *Name of Business
                                    <br />
                                    <asp:TextBox ID="txtInsuredBusinessName" Width="300" runat="server"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>       
            </tr>
            <tr id="trGreenUpgrades" runat="server">
                <td colspan="3"><!-- Added 1/25/18 for HOM Upgrade MLW -->
                    <table style="width:100%">
                        <tr>
                            <td>
                                <div runat="server" visible="false" id="divMaximumAmount" style="display:block;">
                                    *Maximum Amount
                                    <br />
                                    <asp:TextBox ID="txtMaximumAmount" Width="125" runat="server" MaxLength="50"></asp:TextBox>
                                </div> 
                            </td>
                            <td style="vertical-align:top;">
                                <div runat="server" visible="false" id="divIncreasedCostOfLoss" style="display:block;">
                                    *Increased Cost of Loss
                                    <br />
                                    <asp:DropDownList ID="ddIncreasedCostOfLoss" width="125" runat="server"></asp:DropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div runat="server" id="divGreenCheck" visible="false" style="display:block;">
                                    <asp:CheckBox ID="chkVegetatedRoofApplies" runat="server" CssClass="homOptionalCovChk" />  
                                    <asp:Label AssociatedControlID="chkVegetatedRoofApplies" ID="lblVegetatedRoofApplies" runat="server" Text="Vegetated Roof Applies"></asp:Label> 
                                    <br />
                                    <asp:CheckBox ID="chkExpRelCov" runat="server" CssClass="homOptionalCovChk" />  
                                    <asp:Label AssociatedControlID="chkExpRelCov" ID="lblExpRelCov" runat="server" Text="Expense Related Coverage"></asp:Label> 
                                    <br />
                                </div>
                                <div runat="server" id="divExpRelCovLimit" visible="false" style="display:block;">
                                    <asp:Label ID="lblExpRelCovLimitt" runat="server">*Limit</asp:Label><br />
                                    <asp:TextBox ID="txtExpRelCovLimit" Width="125" runat="server"></asp:TextBox>
                                </div>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </td>            
            </tr>
            <tr id="trBusinessStructure" runat="server">
                <td colspan="3"><!-- Added 1/29/18 for HOM Upgrade MLW -->
                    <table style="width:100%">
                        <tr>
                            <td>
                                <div runat="server" visible="false" id="divBusinessDescription" style="display:block;">
                                    *Business Description
                                    <br />
                                    <asp:TextBox ID="txtBusinessDescription" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </div> 
                            </td>
                            <td style="vertical-align:top;">
                                
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div runat="server" id="divOtherStructures" visible="false" style="display:block;">
                                    <asp:CheckBox ID="chkOtherStructures" runat="server" CssClass="homOptionalCovChk" />  
                                    <asp:Label AssociatedControlID="chkOtherStructures" ID="lblOtherStructures" runat="server" Text="Other Structures"></asp:Label> 
                                    <br />
                                </div>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <div runat="server" id="divBuildingDescription" visible="false" style="display:block;">
                                    *Building Description
                                    <br />
                                    <asp:TextBox ID="txtBuildingDescription" Width="150" runat="server"></asp:TextBox>
                                    <br />
                                </div>
                            </td>
                            <td>
                                <div runat="server" id="divBuildingLimit" visible="false" style="display:block;">
                                    *Total Limit
                                    <br />
                                    <asp:TextBox ID="txtBuildingLimit" Width="125" runat="server"></asp:TextBox>
                                    <br />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>            
            </tr>
            
             
            
            <tr id="trStructures" runat="server">
                <td colspan="3">
                    <uc1:ctlHomSpecifiedStructureList runat="server" id="ctlHomSpecifiedStructureList" />
                </td>
                           
            </tr>
            <tr runat="server" id="trFarmLand">
                <td colspan="3">
                    <uc1:ctlHomFarmLandList runat="server" id="ctlHomFarmLandList" />
                </td>
            </tr>

            <tr runat="server" id="trAdditionalResidenceRenterToOthers">
                <td colspan="3">
                    <uc1:ctlHomAdditionalResidenceList runat="server" id="ctlHomAdditionalResidenceList" />
                </td>
            </tr>

            <tr runat="server" id="trMultipleNames">
                <td colspan="3"><!-- Added 1/23/18 for HOM Upgrade MLW -->
                    <uc1:ctlHomMultipleNamesList runat="server" id="ctlHomMultipleNamesList" />
                </td>
            </tr>
            
            <tr runat="server" id="trOtherMembers">
                <td colspan="3"><!-- Added 1/31/18 for HOM Upgrade MLW -->
                    <uc1:ctlHomOtherMembersList runat="server" id="ctlHomOtherMembersList" />
                </td>
            </tr>
            <tr runat="server" id="trAdditionalInterests">
                <td colspan="3"><!-- Added 1/31/18 for HOM Upgrade MLW -->
                    <div id="divAILimits" runat="server">
                        <table style="width:100%" id="tableAILimits" runat="server">
                            <tr id="trPropertyLabel" runat="server"><td colspan="3">Property:</td></tr>
                            <tr id="trPropertyLimits" runat="server">
                                <td>
                                    <label for="<%=txtPropIncludedLimit.ClientID%>">Included Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtPropIncludedLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtPropIncreaseLimit.ClientID%>">Increased Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtPropIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                                    <asp:DropDownList ID="ddPropIncreaseLimit" Width="100" runat="server"></asp:DropDownList>
                                </td>
                                <td>
                                    <label for="<%=txtPropTotalLimit.ClientID%>">Total Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtPropTotalLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trLiabilityLabel" runat="server"><td colspan="3">Liability:</td></tr>
                            <tr id="trLiabilityLimits" runat="server">
                                <td>
                                    <label for="<%=txtLiabIncludedLimit.ClientID%>">Included Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtLiabIncludedLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <label for="<%=txtLiabIncreaseLimit.ClientID%>">Increased Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtLiabIncreaseLimit" Width="100" runat="server"></asp:TextBox>
                                    <asp:DropDownList ID="ddLiabIncreaseLimit" Width="100" runat="server"></asp:DropDownList>
                                </td>
                                <td>
                                    <label for="<%=txtLiabTotalLimit.ClientID%>">Total Limit</label>
                                    <br />
                                    <asp:TextBox ID="txtLiabTotalLimit" Width="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <uc1:ctlHomAdditionalInterestsList runat="server" id="ctlHomAdditionalInterestsList" />
                </td>
            </tr>
            <tr runat="server" id="trAdditionalInsured">
                <td colspan="3"><!-- Added 4/30/18 for HOM Upgrade Bug 26102 MLW -->
                    <uc1:ctlHomAdditionalInsuredList runat="server" id="ctlHomAdditionalInsuredList" />
                </td>
            </tr>
            <tr runat="server" id="trTrust">
                <td colspan="3"><!-- Added 2/14/18 for HOM Upgrade MLW -->
                    <uc1:ctlHomTrustList runat="server" id="ctlHomTrustList" />
                </td>
            </tr>
            <tr runat="server" id="trSpecialEventCoverage">
                <td colspan="3">
                    <table style="width:100%;border-collapse:collapse;">
                        <tr>
                            <td colspan="2" class="informationalText">
                                If alcohol is to be served, a licensed provider must be serving. If not, the event is ineligible for this coverage. If the event takes place at a location other than the residence premises, we require proof of insurance from that venue.
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:60%;">
                                *Description <br />
                                <asp:TextBox runat="server" ID="txtSpecialEventDescription" Width="95%" Height="40px" MaxLength="250" TextMode="MultiLine"></asp:TextBox>
                                <br />
                                <asp:Label ID="Label1" runat="server" Text="Max Characters: 250" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                                <br />
                                <br />
                            </td>
                            <td>
                                &nbsp;
                                <asp:LinkButton ID="lbSpecialEventAddAddress" runat="server" Text="Add Address" OnClientClick="return false;" ></asp:LinkButton>
                                &nbsp;
                                <asp:LinkButton ID="lbSpecialEventDeleteAddress" runat="server" Text="Delete" ></asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table style="width:100%;border-collapse:collapse;">
                                    <tr>
                                        <td style="width:30%;">Event From</td>
                                        <td>
                                            <BDP:BasicDatePicker DisplayType="TextBox" ID="bdpSpecialEventFromDate" runat="server" DateFormat="MM/dd/yyyy" ShowCalendarOnTextBoxFocus="true"></BDP:BasicDatePicker>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table style="width:100%;border-collapse:collapse;">
                                    <tr>
                                        <td style="width:30%;">Event To</td>
                                        <td>
                                            <BDP:BasicDatePicker DisplayType="TextBox" ID="bdpSpecialEventToDate" runat="server" DateFormat="MM/dd/yyyy" ShowCalendarOnTextBoxFocus="true"></BDP:BasicDatePicker>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr id="trSpecialEventAddress" runat="server">
                            <td colspan="2">
                                <table id="tblSpecialEventAddress" style="width:100%;border-collapse:collapse;">
                                    <tr>
                                        <td style="width:30%;">
                                            Street #
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtSpecialEventStreetNumber"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Street Name</td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtSpecialEventStreetName"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Apt/Suite Number</td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtSpecialEventAptSuiteNumber"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>ZIP</td>
                                        <td>
                                            <asp:TextBox ID="txtSpecialEventZip" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>City</td>
                                        <td>
                                            <asp:TextBox ID="txtSpecialEventCity" runat="server"></asp:TextBox>
                                            <asp:DropDownList ID="ddlSpecialEventCity" CausesValidation="false" runat="server" ></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>State</td>
                                        <td>
                                            <asp:DropDownList ID="ddlSpecialEventState" runat="server"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>County</td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtSpecialEventCounty"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center;align-content:center;">
                                            <br />
                                            <asp:Button runat="server" ID="btnSpecialEventOK" Text="OK" CssClass="StandardSaveButton" />
                                            &nbsp;
                                            <asp:Button runat="server" ID="btnSpecialEventCancel" Text="Cancel" CssClass="StandardSaveButton" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>                      
                    </table>
                </td>
            </tr>
            <tr id="trLocation" runat="server">
                <td colspan="3">
                    <uc1:ctlHomLocation runat="server" id="ctlHomLocation" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hndhomOptionalCovChk" Value="0" runat="server" />
</div>