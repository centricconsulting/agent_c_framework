<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WCP_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_WCP_Coverages" %>
<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctl_General_Info" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/WCP/ctl_WCP_Classification.ascx" TagPrefix="uc1" TagName="ctl_WCP_Classification" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Email_UW" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/WCP/ctl_WCP_LocationList.ascx" TagPrefix="uc1" TagName="ctl_WCP_Locations" %>

<div runat="server" id="divMain">

    <div runat="server" id="divGeneralInfo">
        <h3>
            General Information
             <span style="float: right;">
                 <asp:LinkButton ID="lnkSaveGeneralInfo" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
             </span>
        </h3>
        <div>
            <table>
                <tr>
                    <td style="width:10%;">
                        &nbsp;
                    </td>
                    <td style="width:40%;text-align:left;">
                        *Employer's Liability
                    </td>
                    <td style="width:50%;text-align:left;">
                        <asp:DropDownList ID="ddlEmployersLiability" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 10%;">&nbsp;
                    </td>
                    <td colspan="2">
                        <span>We require minimum limits of 500/500/500 when quoting an umbrella.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        *Experience Modification
                    </td>
                    <td>
                        <%--<asp:TextBox ID="txtExpMod" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || (event.charCode == 190)'></asp:TextBox>--%>
                        <asp:TextBox ID="txtExpMod" runat="server" AutoPostBack="true"></asp:TextBox>                    
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <asp:Label ID="lblExpModEffDt" runat="server" Text="*Experience Mod.Eff. Date"></asp:Label>
                    </td>
                    <td>
                        <BDP:BasicDatePicker ID="bdpExpModEffDate" runat="server" DateFormat="MM/dd/yyyy" ShowCalendarOnTextBoxFocus="true"></BDP:BasicDatePicker>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="divLocations" runat="server">
        <uc1:ctl_WCP_Locations id="ctl_WCP_LocationList" runat="server"></uc1:ctl_WCP_Locations>
    </div>
<%--    <div id="divLocation" runat="server">
        <h3>
            <asp:Label ID="lblAccordHeader_Location" runat="server" Text="Location # 1"></asp:Label>
             <span style="float: right;">
                 <asp:LinkButton ID="lnkSaveLocation" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
             </span>
        </h3>
        <div>
            <uc1:ctl_General_Info id="ctl_GenInfo" runat="server"></uc1:ctl_General_Info>
        </div>
    </div>--%>

    <div id="divClassification" runat="server">
        <h3>
            Classification Information
             <span style="float: right;">
                 <asp:LinkButton ID="lbAddNewClassification" CssClass="RemovePanelLink" ToolTip="Add a new classification" runat="server">Add New</asp:LinkButton>
                 <asp:LinkButton ID="btnSaveClassifications" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
             </span>
        </h3>
        <div runat="server" id="divClassificationList">
            <asp:Repeater ID="rptClassifications" runat="server">
                <ItemTemplate>
                    <uc1:ctl_WCP_Classification id="ctl_Classification" runat="server" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <div id="divEndorsements" runat="server">
        <h3>
            Endorsements
             <span style="float: right;">
                 <asp:LinkButton ID="lnkSaveEndorsements" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
             </span>
        </h3>
        <div>
            <table>
                <tr>
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkInclusionOfSoleProp" runat="server" Text="&nbsp;" />
                    </td>
                    <td style="width:90%;">
                        <asp:Label ID="lblInclusionOfSoleProp" runat="server" Text="Inclusion of Sole Proprietors, Partners, and LLC Members (WC 00 03 10)(IN/IL)"></asp:Label>
                    </td>
                </tr>
                <tr id="trBlanketWaiverOfSubroRow" runat="server">
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkBlanketWaiverOfSubro" runat="server" Text="&nbsp;" />
                    </td>
                    <td>
                        <asp:Label ID="lblBlanketWaiverOfSubro" runat="server" Text="Blanket Waiver of Subrogation (WCP 1001)(IN/IL)"></asp:Label>
                    </td>
                </tr>
                <tr id="trWaiverOfSubroRow" runat="server">
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkWaiverofSubro" runat="server" Text="&nbsp;" />
                    </td>
                    <td>
                        <asp:Label ID="lblWaiverOfSubro" runat="server" Text="Waiver of Subrogation (WC 00 03 13)(IN/IL)"></asp:Label>
                    </td>
                </tr>
                <tr id="trNumberOfWaiversRow" runat="server">
                    <td colspan="3">
                        <table>
                            <tr>
                                <td style="width:55%;text-align:right;">*Number of Waivers</td>
                                <td>
                                    <asp:TextBox ID="txtNumberOfWaivers" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trExclusionOfAmishWorkers_row" runat="server" style="display:none;">
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkExclusionOfAmishWorkers" runat="server" Text="&nbsp;" />
                    </td>
                    <td>
                        <asp:Label ID="lblExclusionOfAmishWorkers" runat="server" Text="Exclusion of Amish Workers (WC 00 03 08)(IN)"></asp:Label>
                    </td>
                </tr>
                <tr id="trExclusionOfExecutiveOfficer_row" runat="server" style="display:none;">
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkExclusionOfExecutiveOfficer" runat="server" Text="&nbsp;" />
                    </td>
                    <td>
                        <asp:Label ID="lblExclusionOfExecutiveOfficer" runat="server" Text="Exclusion of Executive Officer (WC 00 03 08)(IN)"></asp:Label>
                    </td>
                </tr>
                <tr id="trInfo1" runat="server" >
                    <td style="width:5%;">&nbsp;</td>
                    <td colspan="2" class="informationalText">
                        The State of Indiana requires that you complete and submit form 36097 (Notice For Workers Compensation And Occupational Diseases Coverage) when excluding officers from workers compensation coverage.  For your convenience this form can be found <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("WCP_Help_WorkCompINStateForm")%>"> here.</a>  Please mail the form to the WC board as instructed, also submit a copy to your underwriter.
                    </td>
                </tr>
                <tr id="trExclusionOfSoleProprietorsEtc_IL_row" runat="server" style="display:none;">
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkExclusionOfSoleProprietorsEtc_IL" runat="server" Text="&nbsp;" />
                    </td>
                    <td>
                        <asp:Label ID="lblExclusionOfSoleProprietorsEtc_IL" runat="server" Text="Exclusion of Sole Proprietors, Partners, Officers, LLC Members & others (WC 12 03 07)(IL)"></asp:Label>
                    </td>
                </tr>
                <tr id="trRejectionOfCoverageEndorsementRow" runat="server" style="display:none;">
                    <td style="width:5%;">&nbsp;</td>
                    <td style="width:5%;">
                        <asp:CheckBox ID="chkRejectionOfCoverageEndorsement" runat="server" Text="&nbsp;" />
                    </td>
                    <td>
                        <asp:Label ID="lblRejectionOfCoverageEndorsement" runat="server" Text="Rejection of Coverage Endorsement (WC 16 03 01)(KY)"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>

    </div>

    <div style="margin-top: 20px; width: 100%; text-align:center;">
        <asp:Button ID="btnSave" runat="server" Text="Save Coverages" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" />
        <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
    </div>

    <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
    <asp:HiddenField ID="hdnAccordLoc" runat="server" />
    <asp:HiddenField ID="hdnAccordClass" runat="server" />
    <asp:HiddenField ID="hdnAccordClassList" runat="server" />
    <asp:HiddenField ID="hdnAccordEndorsements" runat="server" />
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Text="&nbsp"></asp:Label>
</div>