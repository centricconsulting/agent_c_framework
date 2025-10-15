<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlTreeView.ascx.vb" Inherits="IFM.VR.Web.ctlTreeView" %>

<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Email_UW" %>

<%--<asp:UpdatePanel ID="up_TreeView" runat="server">
<ContentTemplate>--%>
<script type="text/javascript">
    //use # when in head and need Page.Header.DataBind() on Page_Load; use = when in body
    var hdnExpandOrCollapseAllFlagId = '<%=hdnExpandOrCollapseAllFlag.ClientID%>';
    //added 1/22/2014
    var hdnDeselectAllListItemsFlagId = '<%=hdnDeselectAllListItemsFlag.ClientID%>';
    var hdnInEditModeFlagId = '<%=hdnInEditModeFlag.ClientID%>';
    var dbpEffectiveDateClientId = '<%=bdpEffectiveDate.ClientID%>';
    var pnlTreeViewClientId = '<%= Me.pnlTreeView.ClientId%>';
    var hdnQuoteTransactionTypeFlagId = '<%=hdnQuoteTransactionTypeFlag.ClientID%>'; //added 2/21/2019

    //  6/29/14 _ Matt A - Fade in tree on load
    $(document).ready(function () {
        // fades in the treeview like the other items on the page
        $("#" + pnlTreeViewClientId).fadeIn();
    });
</script>

<%--&nbsp;--%><%--3/12/2014 - commented out all instances and added 'removed 3/12/2014 to test spacing' comment--%>
<%--3/13/2014 - comments added around spans to prevent extra spaces from being added--%>

<asp:Panel ID="pnlTreeView" runat="server" Visible="false" CssClass="VRtreeview" Style="display: none;">
    <ul style="list-style-type: none; width: 300px;" class="mainList">
        <%--<li style="list-style-type:square;">--%>
        <li runat="server" id="liQuoteNumber" visible="false">
            <div class="sectionHeader overflow">
                <%-- onclick="sectionHeaderClick(this, 'QUOTENUM');"--%>
                <b><span><asp:Label runat="server" ID="lblQuoteOrPolicy" Text="Quote"></asp:Label> Number:&nbsp;</span><asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></b>
            </div>
            <br />
        </li>
        <li>
            <input id="hdnQuoteDescriptionSection_EnabledOrDisabledFlag" name="hdnQuoteDescriptionSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader overflow" onclick="sectionHeaderClick(this, 'QUOTEDESC');">
                <%--added overflow 3/17/2014--%>
                <%--<input id="hdnQuoteDescriptionSection_EnabledOrDisabledFlag" name="hdnQuoteDescriptionSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <input id="hdnIsSelected_QuoteDescription" name="hdnIsSelected_QuoteDescription" type="hidden" runat="server" />
                <input id="hdnQuoteNumber" name="hdnQuoteNumber" type="hidden" runat="server" /><input id="hdnUseQuoteNumberHeader" name="hdnUseQuoteNumberHeader" type="hidden" runat="server" /><%--added hdnUseQuoteNumberHeader 6/26/2014--%><%--<asp:Label runat="server" ID="lblQuoteNumber" Visible="false"></asp:Label>--%>
                <input id="hdnOriginalQuoteDescription" name="hdnOriginalQuoteDescription" type="hidden" runat="server" /><%--<asp:Label runat="server" ID="lblOriginalQuoteDescription" Visible="false"></asp:Label>--%><%--
                --%><span id="QuoteDescriptionHeader" runat="server" class="clickableHeader" title="Edit Quote Description" onclick="QuoteDescriptionOrEffectiveDateHeaderClick(this);"><%--was using onclick="SetClickedElement(this); var showError = false; if (this.className == 'clickable'){showError = true;} if (InEditMode(showError) == false){ToggleTitleAndClickableClass(this, false); SimulateDescOrEffDateEditButtonFromHeader(this);}"--%><%--onclick used to include ClickChildButtonOfRelatedChildElement(this, 'QuoteDescriptionViewSection', 'imgBtnEditQuoteDescription');--%><%--
                --%><input id="hdnTitleFlag_QuoteDescriptionHeader" name="hdnTitleFlag_QuoteDescriptionHeader" type="hidden" runat="server" /><%--
                --%><b id="DescriptionOrRemarksBoldTag"><%--<asp:Label runat="server" ID="lblDescriptionOrRemarks" Text="Description"></asp:Label>--%><span runat="server" id="DescriptionOrRemarksText">Description</span>:</b><%--
                --%></span><%--
                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                --%><input id="hdnQuoteDescriptionViewSect_Display" name="hdnQuoteDescriptionViewSect_Display" type="hidden" runat="server" /><%--
                --%><span runat="server" id="QuoteDescriptionViewSection"><%--
                --%><asp:Label runat="server" ID="lblQuoteDescription" OnClick="QuoteDescriptionOrEffectiveDateLabelClick(this);" ToolTip="Edit Quote Description" CssClass="clickable"></asp:Label><%--3/17/2014 - moved overflow to header div; was previously CssClass="overflow clickable"--%><%--was using OnClick="SetClickedElement(this); if (InEditMode(true) == false){ToggleTitleAndClickableClassForRelatedParentSibling(this, false); SimulateDescOrEffDateEditButtonFromLabel(this);}"--%><%--onclick used to include ClickRelatedChildButton(this, 'imgBtnEditQuoteDescription');--%><%--
                --%><%--&nbsp;--%><%--
                --%><asp:ImageButton runat="server" ID="imgBtnEditQuoteDescription" ImageUrl="../images/test/button_edit_off.gif" ToolTip="Edit Quote Description" OnClientClick="SelectParentListItem(this);" CssClass="hidden" /><%--
                --%></span><%--
                --%><input id="hdnQuoteDescriptionEditSect_Display" name="hdnQuoteDescriptionEditSect_Display" type="hidden" runat="server" /><%--
                --%><span runat="server" id="QuoteDescriptionEditSection"><%--3/5/2014 - removed visible="false"--%><%--
                --%><asp:TextBox runat="server" ID="txtQuoteDescription" MaxLength="200" onblur="OnBlurQuoteDescriptionOrEffDate(this);"></asp:TextBox><%--
                --%><%--&nbsp;--%><%--
                --%><asp:ImageButton runat="server" ID="imgBtnSaveQuoteDescription" ImageUrl="../images/test/button_save_on.gif" ToolTip="Save Quote Description" CssClass="hidden" />
                    <asp:ImageButton runat="server" ID="imgBtnCancelSaveQuoteDescription" ImageUrl="../images/test/button_cancel_on.gif" ToolTip="Cancel Save Quote Description" OnClientClick="RemoveSelectedClassFromAllListItems(); ToggleTitleAndClickableClassForRelatedParentSibling(this, true, 'Edit Quote Description');" CssClass="hidden" />
                </span>
            </div>
            <span runat="server" id="QuoteTypeSection" visible="false"><%--added 9/16/2015--%>
                <ul style="list-style-type: none;" class="subList" runat="server" id="ulQuoteType">
                    <li runat="server" id="liQuoteType">
                        <span runat="server" id="QuoteTypeSectionSubLists_expandCollapseImageArea" style="visibility:hidden;"><%--this just here for spacing--%><%--
                            --%><img id="QuoteTypeSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                            <img id="QuoteTypeSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                            <input id="hdnQuoteTypeSectionSubListsExpandedOrCollapsed" name="hdnQuoteTypeSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                        --%></span><%--
                        --%><span class="clickableHeader" style="visibility: hidden;">X</span>&nbsp;<%--just a placeholder; has visibility hidden so it never shows--%><%--
                        --%><asp:Label ID="lblQuoteType" runat="server"></asp:Label>
                    </li>
                </ul>
            </span>
            <br />
        </li>
        <%--<li style="list-style-type:square;">--%>
        <li>
            <input id="hdnEffectiveDateSection_EnabledOrDisabledFlag" name="hdnEffectiveDateSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'EFFDATE');">
                <%--<input id="hdnEffectiveDateSection_EnabledOrDisabledFlag" name="hdnEffectiveDateSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <input id="hdnIsSelected_EffectiveDate" name="hdnIsSelected_EffectiveDate" type="hidden" runat="server" /><input id="hdnMinimumEffectiveDate" name="hdnMinimumEffectiveDate" type="hidden" runat="server" /><input id="hdnMaximumEffectiveDate" name="hdnMaximumEffectiveDate" type="hidden" runat="server" /><input id="hdnMinimumEffectiveDateAllQuotes" name="hdnMinimumEffectiveDateAllQuotes" type="hidden" runat="server" /><input id="hdnMaximumEffectiveDateAllQuotes" name="hdnMaximumEffectiveDateAllQuotes" type="hidden" runat="server" /><input id="hdnQuoteHasMinimumEffectiveDate" name="hdnQuoteHasMinimumEffectiveDate" type="hidden" runat="server" /><input id="hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes" name="hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes" type="hidden" runat="server" /><input id="hdnBeforeDateMsg" name="hdnBeforeDateMsg" type="hidden" runat="server" /><input id="hdnAfterDateMsg" name="hdnAfterDateMsg" type="hidden" runat="server" /><%--added hdnMinimumEffectiveDate and hdnMaximumEffectiveDate 9/1/2015--%>
                <input id="hdnOriginalEffectiveDate" name="hdnOriginalEffectiveDate" type="hidden" runat="server" /><%--<asp:Label runat="server" ID="lblOriginalEffectiveDate" Visible="false"></asp:Label>--%><%--
                --%><span id="EffectiveDateHeader" runat="server" class="clickableHeader" title="Edit Effective Date" onclick="QuoteDescriptionOrEffectiveDateHeaderClick(this);"><%--was using onclick="SetClickedElement(this); var showError = false; if (this.className == 'clickable'){showError = true;} if (InEditMode(showError) == false){ToggleTitleAndClickableClass(this, false); SimulateDescOrEffDateEditButtonFromHeader(this);}"--%><%--onclick used to include ClickChildButtonOfRelatedChildElement(this, 'EffectiveDateViewSection', 'imgBtnEditEffectiveDate');--%><%--
                --%><input id="hdnTitleFlag_EffectiveDateHeader" name="hdnTitleFlag_EffectiveDateHeader" type="hidden" runat="server" /><%--
                --%><b runat="server" id="lblEffDateTextBoldTag"><asp:Label runat="server" ID="lblEffDateText" Text="Effective Date"></asp:Label>:</b><%--
                --%></span><%--
                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                --%><input id="hdnEffectiveDateViewSect_Display" name="hdnEffectiveDateViewSect_Display" type="hidden" runat="server" /><%--
                --%><span runat="server" id="EffectiveDateViewSection"><%--
                --%><asp:Label runat="server" ID="lblEffectiveDate" OnClick="QuoteDescriptionOrEffectiveDateLabelClick(this);" ToolTip="Edit Effective Date" CssClass="clickable"></asp:Label><%--was using OnClick="SetClickedElement(this); if (InEditMode(true) == false){ToggleTitleAndClickableClassForRelatedParentSibling(this, false); SimulateDescOrEffDateEditButtonFromLabel(this);}"--%><%--onclick used to include ClickRelatedChildButton(this, 'imgBtnEditEffectiveDate');--%><%--
                --%><%--&nbsp;--%><%--
                --%><asp:ImageButton runat="server" ID="imgBtnEditEffectiveDate" ImageUrl="../images/test/button_edit_off.gif" ToolTip="Edit Effective Date" OnClientClick="SelectParentListItem(this);" CssClass="hidden" /><%--
                --%></span><%--
                --%><input id="hdnEffectiveDateEditSect_Display" name="hdnEffectiveDateEditSect_Display" type="hidden" runat="server" /><%--
                --%><span runat="server" id="EffectiveDateEditSection"><%--3/5/2014 - removed visible="false"--%><%--
                --%><%--<asp:TextBox runat="server" ID="txtEffectiveDate"></asp:TextBox>--%><%--
                --%><BDP:BasicDatePicker DisplayType="TextBox" DateFormat="MM/dd/yyyy" ID="bdpEffectiveDate" runat="server" ShowNoneButton="false" OnClientBeforeCalendarClose="BasicDatePickerCalendarClosed" OnClientBeforeCalendarOpen="BasicDatePickerCalendarOpened"></BDP:BasicDatePicker>
                    <input id="hdnIsVisible_BdpEffectiveDateCalendar" name="hdnIsVisible_BdpEffectiveDateCalendar" type="hidden" runat="server" /><%--onfocusout="OnBlurQuoteDescriptionOrEffDate(this);" works on Textbox but not with calendar... like my current TextBox.onblur--%><%--
                    --%><%--&nbsp;--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnSaveEffectiveDate" ImageUrl="../images/test/button_save_on.gif" ToolTip="Save Effective Date" CssClass="hidden" />
                    <asp:ImageButton runat="server" ID="imgBtnCancelSaveEffectiveDate" ImageUrl="../images/test/button_cancel_on.gif" ToolTip="Cancel Save Effective Date" OnClientClick="RemoveSelectedClassFromAllListItems(); ToggleTitleAndClickableClassForRelatedParentSibling(this, true, 'Edit Effective Date');" CssClass="hidden" />
                </span>
            </div>
            <br />
        </li>
        <li>
            <input id="hdnPolicyholderSection_EnabledOrDisabledFlag" name="hdnPolicyholderSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'PH');">
                <%--<input id="hdnPolicyholderSection_EnabledOrDisabledFlag" name="hdnPolicyholderSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="ph_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Policyholder Information Required" title="Policyholder Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainPolicyholderSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from ph_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="ph_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('ph', 'expand');" />
                        <img id="ph_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('ph', 'collapse');" />--%><%--
                        --%><img id="MainPolicyholderSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from ph_expand; updated onclick from 'expandCollapse('ph', 'expand');'--%>
                    <img id="MainPolicyholderSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from ph_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('ph', 'collapse');'--%>
                    <input id="hdnMainPolicyholderSectionSubListsExpandedOrCollapsed" name="hdnMainPolicyholderSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnPhExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="ph_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<img alt="Policyholders" src="images/test/insured.png" />--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnPolicyholders" ImageUrl="../images/test/insured.png" ToolTip="Show Policyholders" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnPolicyholders');" title="Show Policyholders" class="clickableHeader"><b>Policyholders</b>&nbsp;(<asp:Label runat="server" ID="lblNumberOfPolicyholders" Text="0"></asp:Label>)</span><%--added &nbsp; and (#) inside span 3/18/2014--%><%--
                    --%>&nbsp;<%--added &nbsp; and add button and + 3/18/2014--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnAddPolicyholder" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddPolicyholder_Click" ToolTip="Add Policyholder" CssClass="hidden" /><%--
                    --%><span runat="server" id="AddPolicyholderArea" class="clickableHeader" title="Add Policyholder" onclick="ClickRelatedChildButton(this, 'imgBtnAddPolicyholder');">+</span>
            </div>
            <asp:Panel ID="pnlPolicyholders" runat="server" Visible="false">
                <%--note 3/17/2014: overflow here works but no ellipsis--%>
                <asp:Repeater ID="rptPolicyholders" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                            <%--note 3/17/2014: overflow here works but no ellipsis--%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><%--note 3/17/2014: class overflow doesn't work but style does--%>
                            <span runat="server" id="PolicyholderSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="PolicyholderSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="PolicyholderSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="PolicyholderSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="PolicyholderSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnPolicyholderSubListsExpandedOrCollapsed" name="hdnPolicyholderSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanClearPolicyholder" class="clickableHeader" title="Clear Policyholder" onclick="ClickRelatedChildButton(this, 'imgBtnClearPolicyholder');" style="visibility: hidden;">X</span><%--added runat and id 3/18/2014--%><%--added 3/12/2014 to match spacing of other sections; unlike most of them, this one has visibility hidden--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnClearPolicyholder" ImageUrl="../images/test/close.gif" OnClick="imgBtnClearPolicyholder_Click" ToolTip="Clear Policyholder" CssClass="hidden" /><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><input id="hdnIsSelected_Policyholder" name="hdnIsSelected_Policyholder" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditPolicyholder" ImageUrl="../images/test/insured.png" OnClick="imgBtnEditPolicyholder_Click" ToolTip="Edit Policyholder" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblPolicyholderDescription" runat="server" OnClick='if(TreePolicyholderLabelClicked($(this))){ClickRelatedChildButton(this, "imgBtnEditPolicyholder");}' ToolTip="Edit Policyholder" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "PolicyholderDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblPolicyholderNumber" runat="server" Style="display: none;" Text='<%# DataBinder.Eval(Container.DataItem, "PolicyholderNumber") %>'></asp:Label>
                            <ul style="list-style-type: none;" class="subList2" runat="server" id="ulPolicyholderSubItems" visible="false">
                                <%--added 5/21/2015 for FAR--%>
                                <li runat="server" id="liPolicyholderSubItem_Applicant" visible="false">
                                    <asp:Label ID="lblApplicant" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Applicant")%>'></asp:Label><asp:Label ID="lblApplicantNumber" runat="server" Style="display: none;" Text='<%# DataBinder.Eval(Container.DataItem, "ApplicantNumber")%>'></asp:Label></li>
                                <li runat="server" id="liPolicyholderSubItem_Applicant2" visible="false">
                                    <asp:Label ID="lblApplicant2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Applicant2")%>'></asp:Label><asp:Label ID="lblApplicant2Number" runat="server" Style="display: none;" Text='<%# DataBinder.Eval(Container.DataItem, "Applicant2Number")%>'></asp:Label></li>
                                <%--added 6/8/2015 to store applicants 1 and 2 under single PH1--%>
                            </ul>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>

        <!-- NEW CPP SECTIONS FOR PROPERTY AND LIABILITY MGB 2/13/18 -->
        <!-- PROPERTY -->
        <!-- CPP Property Header -->
        <li runat="server" id="liCPPCPRDetailHeader" visible="false">
            <input id="hdnCPPCPRDetailHeaderSection_EnabledOrDisabledFlag" name="hdnCPPCPRDetailHeaderSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CPRPDS');">
                <span title="Property Detail" class="clickableHeader" onclick="ClickRelatedChildButton(this, 'imgBtnCPPCPRHeaderClick');" ><b>Property Detail</b></span>
                <asp:ImageButton runat="server" ID="imgBtnCPPCPRHeaderClick" style="display:none;" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Property Policy Level Coverages" CssClass="hidden" />
<%--                <span runat="server" id="CPPCPRHeaderLists_expandCollapseImageArea">
                    <img id="CPPCPRHeaderSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="CPPCPRHeaderSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" />
                    <input id="hdnCPPCPRHeaderSectonSubListsExpandedOrCollapsed" name="hdnCPPCPRHeaderSectonSubListsExpandedOrCollapsed" type="hidden" runat="server" />
                </span>--%>
            </div>
            <br />
        </li>
        <!-- CPP Property Coverages -->
        <li runat="server" id="liCPPCPRCoverages" visible="false">
            <input id="hdnCPPCPRPolicyLevelCoverageSection_EnabledOrDisabledFlag" name="hdnCPPCPRPolicyLevelCoverageSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CPPCPRCOVS');">
                <span runat="server" id="CPPCPRCoverages_xImageArea" visible="false" style="float: right;">
                    <img src="images/incomplete.png" alt="Policy Level Coverage Information Required" title="Policy Level Coverage Information Required" />
                </span>
<%--                <span runat="server" id="CPPCPRCoverageSectionSubLists_expandCollapseImageArea">
                    <img id="CPPCPRCoverageSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="CPPCPRCoverageSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" />
                    <input id="hdnCPPCPRCoverageSectionSubListsExpandedOrCollapsed" name="hdnCPPCPRCoverageSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" />
                </span>--%>
                <span runat="server" id="CPPCPRCoverages_checkMarkArea" visible="false" style="float: right;">
                <%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%>
                </span>
                &nbsp;
                <asp:ImageButton runat="server" ID="imgBtnCPPCPRCovs" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Property Policy Level Coverages" CssClass="hidden" />
                <span style="margin-left:20px;" onclick="ClickRelatedChildButton(this, 'imgBtnCPPCPRCovs');" title="Show Property Policy Level Coverages" class="clickableHeader"><b>Policy Level Coverages</b></span>
            </div>
            <br />
        </li>        
        <!-- CPP Property Locations -->
        <li runat="server" id="liCPPCPRLocations" visible="false">
            <input id="hdnCPPCPRLocationsSection_EnabledOrDisabledFlag" name="hdnCPPCPRLocationsSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'LOC');">
                <%--<input id="hdnLocationSection_EnabledOrDisabledFlag" name="hdnLocationSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="CPPCPRLocations_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Location Information Required" title="Location Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="CPPCPRLocationsSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from l_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="l_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('l', 'expand');" />
                        <img id="l_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('l', 'collapse');" />--%><%--
                        --%><img id="CPPCPRLocationSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from l_expand; updated onclick from 'expandCollapse('l', 'expand');'--%>
                    <img id="CPPCPRLocationSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from l_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('l', 'collapse');'--%>
                    <input id="hdnCPPCPRLocationsSectionSubListsExpandedOrCollapsed" name="hdnCPPCPRLocationsSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnLExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="CPPCPRLocations_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<img alt="Locations" src="images/test/home.png" />--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnCPPCPRLocations" ImageUrl="../images/test/home.png" ToolTip="Show Locations" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnCPPCPRLocations');" title="Show Locations" class="clickableHeader"><b runat="server" id="CPPCPRLocationsTitle">Locations</b>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%>(<asp:Label runat="server" ID="lblCPPCPRNumberOfLocations" Text="0"></asp:Label>)</span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnAddCPPCPRLocation" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddCPPCPRLocation_Click1" ToolTip="Add Location" CssClass="hidden" /><%--
                    --%><span class="clickableHeader" title="Add Location" onclick="ClickRelatedChildButton(this, 'imgBtnAddCPPCPRLocation');" runat="server" id="AddCPPCPRLocationSection">+</span><%--
                    --%><span runat="server" id="CPPCPRLocationsPremiumSection" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblCPPCPRLocationsPremium" Text="$0"></asp:Label></span>
            </div>
            <asp:Panel ID="pnlCPPCPRLocations" runat="server" Visible="false">
                <asp:Repeater ID="rptCPPCPRLocations" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="CPPCPRLocationSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="LocationSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="LocationSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="CPPCPRLocationSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="CPPCPRLocationSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnCPPCPRLocationSubListsExpandedOrCollapsed" name="hdnCPPCPRLocationSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveCPPCPRLocation" class="clickableHeader" title="Remove Location" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveCPPCPRLocation');">X</span><%--added runat and id 3/18/2014--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnRemoveCPPCPRLocation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveCPPCPRLocation_Click1" ToolTip="Remove Location" CssClass="hidden" /><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                                --%><input id="hdnIsSelected_CPPCPRLocation" name="hdnIsSelected_CPPCPRLocation" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditCPPCPRLocation" ImageUrl="../images/test/home.png" OnClick="imgBtnEditCPPCPRLocation_Click1" ToolTip="Edit Location" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblCPPCPRLocationDescription" runat="server" OnClick="ClickRelatedChildButton(this, 'imgBtnEditCPPCPRLocation');" ToolTip="Edit Location" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "LocationDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblCPPCPRLocationNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "LocationNumber") %>'></asp:Label>
                            <%--&nbsp;--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocation" ImageUrl="../images/test/button_delete_on.gif" OnClick="imgBtnRemoveLocation_Click" ToolTip="Remove Location" />--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocation_Click" ToolTip="Remove Location" />--%>
                            <asp:Panel ID="pnlCPPCPRLocationBuildings" runat="server" Visible="false">
                                <asp:Repeater ID="rptCPPCPRLocationBuildings" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveCPPCPRLocationBuilding" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveCPPCPRLocationBuilding_Click1" ToolTip="Remove Location Building" CssClass="hidden" /><span runat="server" id="xSpanRemoveCPPCPRLocationBuilding" class="clickableHeader" title="Remove Location Building" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveCPPCPRLocationBuilding');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<input id="hdnIsSelected_CPPCPRLocationBuilding" name="hdnIsSelected_CPPCPRLocationBuilding" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditCPPCPRLocationBuilding" ImageUrl="../images/test/home.png" OnClick="imgBtnEditCPPCPRLocationBuilding_Click1" ToolTip="Edit Location Building" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--5/28/2015 - added functionality to edit Location Building--%><asp:Label ID="lblCPPCPRLocationBuildingDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LocationBuildingDescription") %>' OnClick="ClickRelatedChildButton(this, 'imgBtnEditCPPCPRLocationBuilding');" ToolTip="Edit Location Building" CssClass="clickable"></asp:Label><%--5/28/2015 - added OnClick, ToolTip, and CssClass--%><asp:Label ID="lblCPPCPRBuildingNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "BuildingNumber") %>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" CssClass="hidden" /><span class="clickableHeader" title="Remove Location Building" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocationBuilding');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        <%--<li><asp:ImageButton runat="server" ID="imgBtnAddLocation" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddLocation_Click" ToolTip="Add Location" /></li>--%>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>


        <!-- CPP LIABILITY -->
        <!-- CPP Liability Header -->
        <li runat="server" id="liCPPCGLDetailHeader" visible="false">
            <input id="hdnCPPCGLDetailHeaderSection_EnabledOrDisabledFlag" name="hdnCPPCGLDetailHeaderSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CPRLDS');">
                <span title="Liability Detail" class="clickableHeader" onclick="ClickRelatedChildButton(this, 'imgBtnCPPCGLHeaderClick');"><b>Liability Detail</b></span>
                <asp:ImageButton runat="server" ID="imgBtnCPPCGLHeaderClick" style="display:none;" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Liability Policy Level Coverages" CssClass="hidden" />
            </div>
            <br />
        </li>
        <!-- CPP Liability Coverages -->
        <li runat="server" id="liCPPCGLCoverages" visible="false">
            <input id="hdnCPPCGLPolicyLevelCoverageSection_EnabledOrDisabledFlag" name="hdnCPPCGLPolicyLevelCoverageSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CPPCPRCOVS');">
                <span runat="server" id="CPPCGLCoverages_xImageArea" visible="false" style="float: right;">
                    <img src="images/incomplete.png" alt="Policy Level Coverage Information Required" title="Policy Level Coverage Information Required" />
                </span>
<%--                <span runat="server" id="CPPCPRCoverageSectionSubLists_expandCollapseImageArea">
                    <img id="CPPCPRCoverageSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="CPPCPRCoverageSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" />
                    <input id="hdnCPPCPRCoverageSectionSubListsExpandedOrCollapsed" name="hdnCPPCPRCoverageSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" />
                </span>--%>
                <span runat="server" id="CPPCGLCoverages_checkMarkArea" visible="false" style="float: right;">
                <%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%>
                </span>
                &nbsp;
                <asp:ImageButton runat="server" ID="imgBtnCPPCGLCovs" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Property Policy Level Coverages" CssClass="hidden" />
                <span style="margin-left:20px;" onclick="ClickRelatedChildButton(this, 'imgBtnCPPCGLCovs');" title="Show Liability Policy Level Coverages" class="clickableHeader"><b>Policy Level Coverages</b></span>
            </div>
            <br />
        </li>        
        <!-- CPP Liability Locations -->
        <li runat="server" id="liCPPCGLLocations" visible="false">
            <input id="hdnCPPCGLLocationSection_EnabledOrDisabledFlag" name="hdnCPPCGLLocationsSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'LOC');">
                <%--<input id="hdnLocationSection_EnabledOrDisabledFlag" name="hdnLocationSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="CPPCGLLocations_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Location Information Required" title="Location Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="CPPCGLLocationsSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from l_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="l_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('l', 'expand');" />
                        <img id="l_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('l', 'collapse');" />--%><%--
                        --%><img id="CPPCGLLocationSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from l_expand; updated onclick from 'expandCollapse('l', 'expand');'--%>
                    <img id="CPPCGLLocationSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from l_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('l', 'collapse');'--%>
                    <input id="hdnCPPCGLLocationsSectionSubListsExpandedOrCollapsed" name="hdnCPPCGLLocationsSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnLExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="CPPCGLLocations_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<img alt="Locations" src="images/test/home.png" />--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnCPPCGLLocations" ImageUrl="../images/test/home.png" ToolTip="Show Locations" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnCPPCGLLocations');" title="Show Locations" class="clickableHeader" runat="server" id="CPPCGLLocationsTip"><b runat="server" id="CPPCGLLocationsTitle">Locations</b>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%>(<asp:Label runat="server" ID="lblCPPCGLNumberOfLocations" Text="0"></asp:Label>)</span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<asp:ImageButton runat="server" ID="imgBtnAddCPPCGLLocation" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddCPPCGLLocation_Click" ToolTip="Add Location" CssClass="hidden" />--%><%--
                    --%><%--<span class="clickableHeader" title="Add Location" onclick="ClickRelatedChildButton(this, 'imgBtnAddCPPCGLLocation');">+</span>--%><%--
                    --%><span runat="server" id="CPPCGLLocationsPremiumSection" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblCPPCGLLocationsPremium" Text="$0"></asp:Label></span>
            </div>
            <asp:Panel ID="pnlCPPCGLLocations" runat="server" Visible="false">
                <asp:Repeater ID="rptCPPCGLLocations" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="CPPCGLLocationSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="LocationSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="LocationSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="CPPCGLLocationSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="CPPCGLLocationSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnCPPCGLLocationSubListsExpandedOrCollapsed" name="hdnCPPCGLLocationSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><!--<span runat="server" id="xSpanRemoveCPPCGLLocation" class="clickableHeader" title="Remove Location" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveCPPCGLLocation');">X</span>--><%--added runat and id 3/18/2014--%><%--
                                --%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveCPPCGLLocation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveCPPCGLLocation_Click" ToolTip="Remove Location" CssClass="hidden" />--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                                --%><input id="hdnIsSelected_CPPCGLLocation" name="hdnIsSelected_CPPCGLLocation" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditCPPCGLLocation" ImageUrl="../images/test/home.png" OnClick="imgBtnEditCPPCGLLocation_Click" ToolTip="Edit Location" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblCPPCGLLocationDescription" runat="server" OnClick="ClickRelatedChildButton(this, 'imgBtnEditCPPCGLLocation');" ToolTip="Edit Location" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "LocationDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblCPPCGLLocationNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "LocationNumber") %>'></asp:Label>
                            <asp:Panel ID="pnlCPPCGLLocationBuildings" runat="server" Visible="false">
                                <asp:Repeater ID="rptCPPCGLLocationBuildings" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveCPPCGLLocationBuilding" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveCPPCGLLocationBuilding_Click" ToolTip="Remove Location Building" CssClass="hidden" /><span runat="server" id="xSpanRemoveCPPCGLLocationBuilding" class="clickableHeader" title="Remove Location Building" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveCPPCGLLocationBuilding');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<input id="hdnIsSelected_CPPCGLLocationBuilding" name="hdnIsSelected_CPPCGLLocationBuilding" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditCPPCGLLocationBuilding" ImageUrl="../images/test/home.png" OnClick="imgBtnEditCPPCGLLocationBuilding_Click" ToolTip="Edit Location Building" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--5/28/2015 - added functionality to edit Location Building--%><asp:Label ID="lblCPPCGLLocationBuildingDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LocationBuildingDescription") %>' OnClick="ClickRelatedChildButton(this, 'imgBtnEditCPPCGLLocationBuilding');" ToolTip="Edit Location Building" CssClass="clickable"></asp:Label><%--5/28/2015 - added OnClick, ToolTip, and CssClass--%><asp:Label ID="lblCPPCGLBuildingNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "BuildingNumber") %>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" CssClass="hidden" /><span class="clickableHeader" title="Remove Location Building" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocationBuilding');">X</span>--%></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        <%--<li><asp:ImageButton runat="server" ID="imgBtnAddLocation" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddLocation_Click" ToolTip="Add Location" /></li>--%>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <!-- END OF NEW CPP SECTIONS FOR PROPERTY AND LIABILITY MGB 2/13/18 -->

        <li runat="server" id="liPolicyLevelCoverages" visible="false"><%--added 7/27/2015 for Farm--%>
            <input id="hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag" name="hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'PLC');">
                <%--<input id="hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag" name="hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="plc_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Policy Level Coverage Information Required" title="Policy Level Coverage Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainPolicyLevelCoverageSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="plc_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="plc_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainPolicyLevelCoverageSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainPolicyLevelCoverageSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainPolicyLevelCoverageSectionSubListsExpandedOrCollapsed" name="hdnMainPolicyLevelCoverageSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="plc_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnPolicyLevelCoverages" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Policy Level Coverages" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnPolicyLevelCoverages');" title="Show Policy Level Coverages" class="clickableHeader"><b>Policy Level Coverages</b></span>
            </div>
            <br />
        </li>
        <li runat="server" id="liDrivers" visible="false">
            <input id="hdnDriverSection_EnabledOrDisabledFlag" name="hdnDriverSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'DRV');">
                <%--<input id="hdnDriverSection_EnabledOrDisabledFlag" name="hdnDriverSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="d_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Driver Information Required" title="Driver Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainDriverSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from d_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="d_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('d', 'expand');" />
                        <img id="d_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('d', 'collapse');" />--%><%--
                        --%><img id="MainDriverSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from d_expand; updated onclick from 'expandCollapse('d', 'expand');'--%>
                    <img id="MainDriverSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from d_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('d', 'collapse');'--%>
                    <input id="hdnMainDriverSectionSubListsExpandedOrCollapsed" name="hdnMainDriverSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnDExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="d_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<img alt="Drivers" src="images/test/driver.png" />--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnDrivers" ImageUrl="../images/test/driver.png" ToolTip="Show Drivers" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnDrivers');" title="Show Drivers" class="clickableHeader"><b>Drivers</b>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%>(<asp:Label runat="server" ID="lblNumberOfDrivers" Text="0"></asp:Label>)</span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnAddDriver" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddDriver_Click" ToolTip="Add Driver" CssClass="hidden" /><%--
                    --%><span class="clickableHeader" title="Add Driver" onclick="ClickRelatedChildButton(this, 'imgBtnAddDriver');" runat="server" id="AddDriverSection">+</span>
            </div>
            <asp:Panel ID="pnlDrivers" runat="server" Visible="false">
                <asp:Repeater ID="rptDrivers" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="DriverSubLists_expandCollapseImageArea"><%--3/12/2014 - removed ' visible="false"'--%><%--
                            --%><%--<img id="DriverSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="DriverSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="DriverSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="DriverSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnDriverSubListsExpandedOrCollapsed" name="hdnDriverSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveDriver" class="clickableHeader" title="Remove Driver" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveDriver');">X</span><%--added runat and id 3/18/2014--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnRemoveDriver" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveDriver_Click" ToolTip="Remove Driver" CssClass="hidden" /><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                                --%><input id="hdnIsSelected_Driver" name="hdnIsSelected_Driver" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditDriver" ImageUrl="../images/test/driver.png" OnClick="imgBtnEditDriver_Click" ToolTip="Edit Driver" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblDriverDescription" runat="server" OnClick='if(TreeDriverLabelClicked($(this))){ClickRelatedChildButton(this, "imgBtnEditDriver");}' ToolTip="Edit Driver" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "DriverDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblDriverNumber" runat="server" Style="display: none;" Text='<%# DataBinder.Eval(Container.DataItem, "DriverNumber") %>'></asp:Label>
                            <%--&nbsp;--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveDriver" ImageUrl="../images/test/button_delete_on.gif" OnClick="imgBtnRemoveDriver_Click" ToolTip="Remove Driver" />--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveDriver" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveDriver_Click" ToolTip="Remove Driver" />--%>
                            <asp:Panel ID="pnlDriverLossHistories" runat="server" Visible="false">
                                <asp:Repeater ID="rptDriverLossHistories" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveDriverLossHistory" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveDriverLossHistory_Click" ToolTip="Remove Driver Loss History" CssClass="hidden" /><span runat="server" id="xSpanRemoveDriverLossHistory" class="clickableHeader" title="Remove Driver Loss History" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveDriverLossHistory');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<asp:Label ID="lblDriverLossHistoryDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DriverLossHistoryDescription") %>'></asp:Label><asp:Label ID="lblDriverLossHistoryNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "DriverLossHistoryNumber") %>'></asp:Label><asp:Label ID="lblDriverLossHistorySourceId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "DriverLossHistorySourceId")%>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveDriverLossHistory" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveDriverLossHistory_Click" ToolTip="Remove Driver Loss History" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveDriverLossHistory" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveDriverLossHistory_Click" ToolTip="Remove Driver Loss History" CssClass="hidden" /><span class="clickableHeader" title="Remove Driver Loss History" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveDriverLossHistory');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                            <asp:Panel ID="pnlDriverAccidentViolations" runat="server" Visible="false">
                                <asp:Repeater ID="rptDriverAccidentViolations" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveDriverAccidentViolation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveDriverAccidentViolation_Click" ToolTip="Remove Driver Accident/Violation" CssClass="hidden" /><span runat="server" id="xSpanRemoveDriverAccidentViolation" class="clickableHeader" title="Remove Driver Accident/Violation" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveDriverAccidentViolation');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<asp:Label ID="lblDriverAccidentViolationDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DriverAccidentViolationDescription") %>'></asp:Label><asp:Label ID="lblDriverAccidentViolationNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "DriverAccidentViolationNumber") %>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveDriverAccidentViolation" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveDriverAccidentViolation_Click" ToolTip="Remove Driver Accident/Violation" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveDriverAccidentViolation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveDriverAccidentViolation_Click" ToolTip="Remove Driver Accident/Violation" CssClass="hidden" /><span class="clickableHeader" title="Remove Driver Accident/Violation" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveDriverAccidentViolation');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        <%--<li><asp:ImageButton runat="server" ID="imgBtnAddDriver" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddDriver_Click" ToolTip="Add Driver" /></li>--%>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liVehicles" visible="false">
            <input id="hdnVehicleSection_EnabledOrDisabledFlag" name="hdnVehicleSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'VEH');">
                <%--<input id="hdnVehicleSection_EnabledOrDisabledFlag" name="hdnVehicleSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="v_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Vehicle Information Required" title="Vehicle Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainVehicleSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from v_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="v_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('v', 'expand');" />
                        <img id="v_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('v', 'collapse');" />--%><%--
                        --%><img id="MainVehicleSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from v_expand; updated onclick from 'expandCollapse('v', 'expand');'--%>
                    <img id="MainVehicleSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from v_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('v', 'collapse');'--%>
                    <input id="hdnMainVehicleSectionSubListsExpandedOrCollapsed" name="hdnMainVehicleSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnVExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="v_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<img alt="Vehicles" src="images/test/vehicle.png" />--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnVehicles" ImageUrl="../images/test/vehicle.png" ToolTip="Show Vehicles" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnVehicles');" title="Show Vehicles" class="clickableHeader"><b>Vehicles</b>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%>(<asp:Label runat="server" ID="lblNumberOfVehicles" Text="0"></asp:Label>)</span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnAddVehicle" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddVehicle_Click" ToolTip="Add Vehicle" CssClass="hidden" /><%--
                    --%><span class="clickableHeader" title="Add Vehicle" onclick="ClickRelatedChildButton(this, 'imgBtnAddVehicle');" runat="server" id="AddVehicleSection">+</span><%--
                    --%><span runat="server" id="VehiclesPremiumSection" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblVehiclesPremium" Text="$0"></asp:Label></span>
            </div>
            <asp:Panel ID="pnlVehicles" runat="server" Visible="false">
                <asp:Repeater ID="rptVehicles" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="VehicleSubLists_expandCollapseImageArea"><%--3/12/2014 - removed ' visible="false"'--%><%--
                            --%><%--<img id="VehicleSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="VehicleSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="VehicleSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="VehicleSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnVehicleSubListsExpandedOrCollapsed" name="hdnVehicleSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveVehicle" class="clickableHeader" title="Remove Vehicle" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveVehicle');">X</span><%--added runat and id 3/18/2014--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnRemoveVehicle" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveVehicle_Click" ToolTip="Remove Vehicle" CssClass="hidden" /><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                                --%><input id="hdnIsSelected_Vehicle" name="hdnIsSelected_Vehicle" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditVehicle" ImageUrl="../images/test/vehicle.png" OnClick="imgBtnEditVehicle_Click" ToolTip="Edit Vehicle" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblVehicleDescription" runat="server" OnClick='if(TreeVehicleClicked($(this))){ClickRelatedChildButton(this, "imgBtnEditVehicle");}' ToolTip="Edit Vehicle" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblVehicleNumber" runat="server" Style="display: none;" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleNumber") %>'></asp:Label>
                            <%--&nbsp;--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveVehicle" ImageUrl="../images/test/button_delete_on.gif" OnClick="imgBtnRemoveVehicle_Click" ToolTip="Remove Vehicle" />--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveVehicle" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveVehicle_Click" ToolTip="Remove Vehicle" />--%>
                            <asp:Panel ID="pnlVehicleDrivers" runat="server" Visible="false">
                                <asp:Repeater ID="rptVehicleDrivers" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveVehicleDriver" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveVehicleDriver_Click" ToolTip="Remove Vehicle Driver" CssClass="hidden" /><span runat="server" id="xSpanRemoveVehicleDriver" class="clickableHeader" title="Remove Vehicle Driver" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveVehicleDriver');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<asp:Label ID="lblVehicleDriverDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleDriverDescription") %>'></asp:Label><asp:Label ID="lblVehicleDriverIdentifier" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "VehicleDriverIdentifier") %>'></asp:Label><asp:Label ID="lblDriverNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "DriverNumber") %>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveVehicleDriver" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveVehicleDriver_Click" ToolTip="Remove Vehicle Driver" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveVehicleDriver" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveVehicleDriver_Click" ToolTip="Remove Vehicle Driver" CssClass="hidden" /><span class="clickableHeader" title="Remove Vehicle Driver" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveVehicleDriver');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        <%--<li><asp:ImageButton runat="server" ID="imgBtnAddVehicle" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddVehicle_Click" ToolTip="Add Vehicle" /></li>--%>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liLocations" visible="false">
            <input id="hdnLocationSection_EnabledOrDisabledFlag" name="hdnLocationSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'LOC');">
                <%--<input id="hdnLocationSection_EnabledOrDisabledFlag" name="hdnLocationSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="l_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Location Information Required" title="Location Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainLocationSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from l_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="l_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('l', 'expand');" />
                        <img id="l_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('l', 'collapse');" />--%><%--
                        --%><img id="MainLocationSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from l_expand; updated onclick from 'expandCollapse('l', 'expand');'--%>
                    <img id="MainLocationSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from l_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('l', 'collapse');'--%>
                    <input id="hdnMainLocationSectionSubListsExpandedOrCollapsed" name="hdnMainLocationSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnLExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="l_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<img alt="Locations" src="images/test/home.png" />--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnLocations" ImageUrl="../images/test/home.png" ToolTip="Show Locations" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnLocations');" title="Show Locations" class="clickableHeader"><b>Locations</b>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%>(<asp:Label runat="server" ID="lblNumberOfLocations" Text="0"></asp:Label>)</span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnAddLocation" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddLocation_Click" ToolTip="Add Location" CssClass="hidden" /><%--
                    --%><span class="clickableHeader" title="Add Location" onclick="ClickRelatedChildButton(this, 'imgBtnAddLocation');" runat="server" id="AddLocationSection">+</span><%--
                    --%><span runat="server" id="LocationsPremiumSection" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblLocationsPremium" Text="$0"></asp:Label></span>
            </div>
            <asp:Panel ID="pnlLocations" runat="server" Visible="false">
                <asp:Repeater ID="rptLocations" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="LocationSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="LocationSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="LocationSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="LocationSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="LocationSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnLocationSubListsExpandedOrCollapsed" name="hdnLocationSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveLocation" class="clickableHeader" title="Remove Location" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocation');">X</span><%--added runat and id 3/18/2014--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnRemoveLocation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocation_Click" ToolTip="Remove Location" CssClass="hidden" /><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                                --%><input id="hdnIsSelected_Location" name="hdnIsSelected_Location" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditLocation" ImageUrl="../images/test/home.png" OnClick="imgBtnEditLocation_Click" ToolTip="Edit Location" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblLocationDescription" runat="server" OnClick="ClickRelatedChildButton(this, 'imgBtnEditLocation');" ToolTip="Edit Location" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "LocationDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblLocationNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "LocationNumber") %>'></asp:Label>
                            <%--&nbsp;--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocation" ImageUrl="../images/test/button_delete_on.gif" OnClick="imgBtnRemoveLocation_Click" ToolTip="Remove Location" />--%>
                            <%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocation" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocation_Click" ToolTip="Remove Location" />--%>
                            <asp:Panel ID="pnlLocationDwellings" runat="server" Visible="false">
                                <%--added 7/21/2015 for Farm dwellings (location w/ valid form type)--%>
                                <asp:Repeater ID="rptLocationDwellings" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveLocationDwelling" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocationDwelling_Click" ToolTip="Remove Location Dwelling" CssClass="hidden" /><span runat="server" id="xSpanRemoveLocationDwelling" class="clickableHeader" title="Remove Location Dwelling" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocationDwelling');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<input id="hdnIsSelected_LocationDwelling" name="hdnIsSelected_LocationDwelling" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditLocationDwelling" ImageUrl="../images/test/home.png" OnClick="imgBtnEditLocationDwelling_Click" ToolTip="Edit Location Dwelling" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--5/28/2015 - added functionality to edit Location Building--%><asp:Label ID="lblLocationDwellingDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LocationDwellingDescription")%>' OnClick="ClickRelatedChildButton(this, 'imgBtnEditLocationDwelling');" ToolTip="Edit Location Dwelling" CssClass="clickable"></asp:Label><%--5/28/2015 - added OnClick, ToolTip, and CssClass--%><asp:Label ID="lblDwellingNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "DwellingNumber")%>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationDwelling" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveLocationDwelling_Click" ToolTip="Remove Location Dwelling" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationDwelling" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocationDwelling_Click" ToolTip="Remove Location Dwelling" CssClass="hidden" /><span class="clickableHeader" title="Remove Location Dwelling" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocationDwelling');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                            <asp:Panel ID="pnlLocationBuildings" runat="server" Visible="false">
                                <asp:Repeater ID="rptLocationBuildings" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" CssClass="hidden" /><span runat="server" id="xSpanRemoveLocationBuilding" class="clickableHeader" title="Remove Location Building" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocationBuilding');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<input id="hdnIsSelected_LocationBuilding" name="hdnIsSelected_LocationBuilding" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnEditLocationBuilding" ImageUrl="../images/test/home.png" OnClick="imgBtnEditLocationBuilding_Click" ToolTip="Edit Location Building" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--5/28/2015 - added functionality to edit Location Building--%><asp:Label ID="lblLocationBuildingDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LocationBuildingDescription") %>' OnClick="ClickRelatedChildButton(this, 'imgBtnEditLocationBuilding');" ToolTip="Edit Location Building" CssClass="clickable"></asp:Label><%--5/28/2015 - added OnClick, ToolTip, and CssClass--%><asp:Label ID="lblBuildingNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "BuildingNumber") %>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveLocationBuilding" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveLocationBuilding_Click" ToolTip="Remove Location Building" CssClass="hidden" /><span class="clickableHeader" title="Remove Location Building" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveLocationBuilding');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        <%--<li><asp:ImageButton runat="server" ID="imgBtnAddLocation" ImageUrl="../images/test/button_add_on.gif" OnClick="imgBtnAddLocation_Click" ToolTip="Add Location" /></li>--%>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liResidences" visible="false">
            <input id="hdnResidenceSection_EnabledOrDisabledFlag" name="hdnResidenceSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'RES');">
                <%--<input id="hdnResidenceSection_EnabledOrDisabledFlag" name="hdnResidenceSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="r_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Property Information Required" title="Property Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--9/23/2014 - changed from Residence to Property--%><%--
                --%></span><%--
                --%><span runat="server" id="MainResidenceSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from r_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="r_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('c', 'expand');" />
                        <img id="r_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('c', 'collapse');" />--%><%--
                        --%><img id="MainResidenceSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from r_expand; updated onclick from 'expandCollapse('c', 'expand');'--%>
                    <img id="MainResidenceSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from r_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('c', 'collapse');'--%>
                    <input id="hdnMainResidenceSectionSubListsExpandedOrCollapsed" name="hdnMainResidenceSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnCExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="r_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnResidence" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Residence" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnResidence');" title="Show Property" class="clickableHeader"><b>Property</b></span><%--9/25/2014 - changed from Residence to Property--%></div>
            <asp:Panel ID="pnlResidences" runat="server" Visible="false">
                <asp:Repeater ID="rptResidences" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="ResidenceSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="ResidenceSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="ResidenceSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="ResidenceSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="ResidenceSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnResidenceSubListsExpandedOrCollapsed" name="hdnResidenceSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanClearResidence" class="clickableHeader" title="Clear Property" onclick="ClickRelatedChildButton(this, 'imgBtnClearResidence');" style="visibility: hidden;">X</span><%--added runat and id 3/18/2014--%><%--added 3/12/2014 to match spacing of other sections; unlike most of them, this one has visibility hidden--%><%--9/25/2014 - changed from Residence to Property--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnClearResidence" ImageUrl="../images/test/close.gif" OnClick="imgBtnClearResidence_Click" ToolTip="Clear Property" CssClass="hidden" /><%--added 3/12/2014 to match spacing of other sections--%><%--9/25/2014 - changed from Residence to Property--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><asp:Label ID="lblResidenceDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ResidenceDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblLocationNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "LocationNumber") %>'></asp:Label><%--
                                --%><%--added Protection Class 10/1/2014; doesn't have indention like normal list items if just using div or br tags--%><%--<div>Protection Class: </div>--%>
                            <ul style="list-style-type: none;" class="subList2" runat="server" id="ulPropertySubItems" visible="false">
                                <li runat="server" id="liPropertySubItem_FormType" visible="false" onclick="FormTypeClick(this);"><%--added onclick 1/7/2015--%><span class="clickableHeader" style="visibility: hidden;">X</span><%--just a placeholder; has visibility hidden so it never shows; onClick wouldn't work anyway since there's no button and there isn't code-behind logic for it--%>&nbsp;<span>Form Type:&nbsp;</span><input id="hdnOriginalFormType" name="hdnOriginalFormType" type="hidden" runat="server" /><asp:Label ID="lblFormType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FormType")%>'></asp:Label><%--<asp:Label ID="lblFormTypeId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "FormTypeId")%>'></asp:Label><asp:DropDownList runat="server" ID="ddlFormType" Visible="false"></asp:DropDownList>--%></li>
                                <li runat="server" id="liPropertySubItem_FormTypeId" visible="false" style="display: none;"><span class="clickableHeader" style="visibility: hidden;">X</span><%--just a placeholder; has visibility hidden so it never shows; onClick wouldn't work anyway since there's no button and there isn't code-behind logic for it--%>&nbsp;<%--<span>Form Type:&nbsp;</span>--%><input id="hdnOriginalFormTypeId" name="hdnOriginalFormTypeId" type="hidden" runat="server" /><asp:Label ID="lblFormTypeId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "FormTypeId")%>'></asp:Label><asp:DropDownList runat="server" ID="ddlFormType" Width="200px" onblur="PropertyFormTypeDropdownOnblur(this);"></asp:DropDownList><asp:ImageButton runat="server" ID="imgBtnSaveFormType" ImageUrl="../images/test/button_save_on.gif" OnClick="imgBtnSaveFormType_Click" ToolTip="Save Form Type" CssClass="hidden" /><%--1/7/2015 - added imgBtnSaveFormType--%></li>
                                <li runat="server" id="liPropertySubItem_ProtectionClass" visible="false"><span class="clickableHeader" style="visibility: hidden;">X</span><%--just a placeholder; has visibility hidden so it never shows; onClick wouldn't work anyway since there's no button and there isn't code-behind logic for it--%>&nbsp;<span>Protection Class:&nbsp;</span><asp:Label ID="lblProtectionClass" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ProtectionClass")%>'></asp:Label></li>
                                <li runat="server" id="liPropertySubItem_ProtectionClass_VeriskReport" visible="false" OnClick="ClickChildButton(this, 'imgBtnViewVeriskReport');" title="View Verisk Report" class="clickable"><span class="clickableHeader" style="visibility: hidden;">X</span><%--just a placeholder; has visibility hidden so it never shows; onClick wouldn't work anyway since there's no button and there isn't code-behind logic for it--%>&nbsp;<span>Protection Class:&nbsp;</span><asp:Label ID="lblProtectionClass_VeriskReport" runat="server"></asp:Label><asp:Label ID="lblLocationNumber_VeriskReport" runat="server" Visible="false"></asp:Label><asp:Label ID="lblProtectionClassSystemGenerated_VeriskReport" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ProtectionClassSystemGenerated")%>'></asp:Label><asp:Label ID="lblProtectionClassId_VeriskReport" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ProtectionClassId")%>'></asp:Label><asp:Label ID="lblProtectionClassSystemGeneratedId_VeriskReport" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ProtectionClassSystemGeneratedId")%>'></asp:Label><asp:ImageButton runat="server" ID="imgBtnViewVeriskReport" ImageUrl="../images/test/dark_account_maint.png" OnClick="imgBtnViewVeriskReport_Click" ToolTip="View Verisk Report" CssClass="hidden" /></li><!--added 10/17/2016 for Verisk Protection Class-->
                            </ul>
                            <asp:Panel ID="pnlPropertyLossHistories" runat="server" Visible="false">
                                <%--added 11/6/2014--%>
                                <asp:Repeater ID="rptPropertyLossHistories" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                            <%--3/13/2014 removed bullets--%>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:ImageButton runat="server" ID="imgBtnRemovePropertyLossHistory" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemovePropertyLossHistory_Click" ToolTip="Remove Property Loss History" CssClass="hidden" /><span runat="server" id="xSpanRemovePropertyLossHistory" class="clickableHeader" title="Remove Property Loss History" onclick="ClickRelatedChildButton(this, 'imgBtnRemovePropertyLossHistory');">X</span><%--added runat and id 3/18/2014--%>&nbsp;<asp:Label ID="lblPropertyLossHistoryDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PropertyLossHistoryDescription") %>'></asp:Label><asp:Label ID="lblPropertyLossHistoryCounter" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "PropertyLossHistoryCounter")%>'></asp:Label><asp:Label ID="lblPropertyLossHistoryLevel" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "PropertyLossHistoryLevel")%>'></asp:Label><asp:Label ID="lblPropertyLossHistoryLevelNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "PropertyLossHistoryLevelNumber")%>'></asp:Label><asp:Label ID="lblPropertyLossHistoryLevelCounter" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "PropertyLossHistoryLevelCounter")%>'></asp:Label><asp:Label ID="lblPropertyLossHistorySourceId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "PropertyLossHistorySourceId")%>'></asp:Label><%--&nbsp;--%><%--removed 3/12/2014 to test spacing; then later un-commented--%><%--<asp:ImageButton runat="server" ID="imgBtnRemovePropertyLossHistory" ImageUrl="../images/test/command_button_delete_on.gif" OnClick="imgBtnRemovePropertyLossHistory_Click" ToolTip="Remove Property Loss History" />--%><%--<asp:ImageButton runat="server" ID="imgBtnRemovePropertyLossHistory" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemovePropertyLossHistory_Click" ToolTip="Remove Property Loss History" CssClass="hidden" /><span class="clickableHeader" title="Remove Property Loss History" onclick="ClickRelatedChildButton(this, 'imgBtnRemovePropertyLossHistory');">X</span>--%></li>
                                        <%--moved &nbsp; and button and x to left-side 3/18/2014 to work w/ truncating on right-side--%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                        <!--<li>
                                <span runat="server" id="Span1"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                                    --%><%--<img id="ResidenceSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="ResidenceSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                    --%><img id="Img1" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display:none;" />
                                    <img id="Img2" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display:inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                    <input id="Hidden1" name="hdnResidenceSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="Span2" class="clickableHeader" title="Clear Property" onclick="ClickRelatedChildButton(this, 'imgBtnClearResidence');" style="visibility:hidden;">X</span><%--added runat and id 3/18/2014--%><%--added 3/12/2014 to match spacing of other sections; unlike most of them, this one has visibility hidden--%><%--9/25/2014 - changed from Residence to Property--%><%--
                                --%><asp:ImageButton runat="server" ID="ImageButton1" ImageUrl="../images/test/close.gif" OnClick="imgBtnClearResidence_Click" ToolTip="Clear Property" CssClass="hidden" /><%--added 3/12/2014 to match spacing of other sections--%><%--9/25/2014 - changed from Residence to Property--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>Protection Class:
                            </li>-->
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liCoverages" visible="false">
            <input id="hdnCoverageSection_EnabledOrDisabledFlag" name="hdnCoverageSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'COV');">
                <%--<input id="hdnCoverageSection_EnabledOrDisabledFlag" name="hdnCoverageSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="c_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Coverage Information Required" title="Coverage Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainCoverageSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from c_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="c_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('c', 'expand');" />
                        <img id="c_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('c', 'collapse');" />--%><%--
                        --%><img id="MainCoverageSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from c_expand; updated onclick from 'expandCollapse('c', 'expand');'--%>
                    <img id="MainCoverageSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from c_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('c', 'collapse');'--%>
                    <input id="hdnMainCoverageSectionSubListsExpandedOrCollapsed" name="hdnMainCoverageSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnCExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="c_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnCoverages" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Coverages" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnCoverages');" title="Show Coverages" class="clickableHeader"><b>Coverages</b></span>
            </div>
            <asp:Panel ID="pnlCoverages" runat="server" Visible="false">
                <asp:Repeater ID="rptCoverages" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                            <%--3/12/2014 note: may need 'style="list-style-type:none;"' to line up w/ other sections; added 3/14/2014 for consistency--%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="CoverageSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="CoverageSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="CoverageSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="CoverageSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="CoverageSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnCoverageSubListsExpandedOrCollapsed" name="hdnCoverageSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveCoverage" class="clickableHeader" title="Remove Coverage" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveCoverage');" style="visibility: hidden;">X</span><%--added runat and id 3/18/2014--%><%--added 3/12/2014 to match spacing of other sections; unlike most of them, this one has visibility hidden--%><%--
                                --%><asp:ImageButton runat="server" ID="imgBtnRemoveCoverage" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveCoverage_Click" ToolTip="Remove Coverage" CssClass="hidden" /><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><asp:Label ID="lblCoverageDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CoverageDescription") %>'></asp:Label>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>        

        <li runat="server" id="liFarmPersonalProperty" visible="false"><%--added 7/27/2015 for Farm--%>
            <input id="hdnFarmPersonalPropertySection_EnabledOrDisabledFlag" name="hdnFarmPersonalPropertySection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'FPP');">
                <%--<input id="hdnFarmPersonalPropertySection_EnabledOrDisabledFlag" name="hdnFarmPersonalPropertySection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="fpp_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Farm Personal Property Coverage Information Required" title="Farm Personal Property Coverage Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainFarmPersonalPropertySectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="fpp_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="fpp_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainFarmPersonalPropertySectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainFarmPersonalPropertySectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainFarmPersonalPropertySectionSubListsExpandedOrCollapsed" name="hdnMainFarmPersonalPropertySectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="fpp_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnFarmPersonalProperty" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Farm Personal Property Coverages (F and G)" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnFarmPersonalProperty');" title="Show Farm Personal Property Coverages (F and G)" class="clickableHeader"><b>Farm Personal Property (F and G)</b></span>
            </div>
            <br />
        </li>
        <!-- Note that this Inland Marine section is for non-cpp lob's that require it -->
        <li runat="server" id="liInlandMarineAndRvWatercraft" visible="false"><%--added 7/27/2015 for Farm--%>
            <input id="hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag" name="hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'IMRV');">
                <%--<input id="hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag" name="hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="imrv_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Inland Marine and Rv/Watercraft Information Required" title="Inland Marine and Rv/Watercraft Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainInlandMarineAndRvWatercraftSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="imrv_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="imrv_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainInlandMarineAndRvWatercraftSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainInlandMarineAndRvWatercraftSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainInlandMarineAndRvWatercraftSectionSubListsExpandedOrCollapsed" name="hdnMainInlandMarineAndRvWatercraftSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="imrv_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnInlandMarineAndRvWatercraft" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Inland Marine and Rv/Watercraft" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnInlandMarineAndRvWatercraft');" title="Show Inland Marine and Rv/Watercraft" class="clickableHeader"><b>Inland Marine / Rv/Watercraft</b><%--added count label 8/12/2015--%>&nbsp;(<asp:Label runat="server" ID="lblNumberOfInlandMarinesAndRvWatercrafts" Text="0"></asp:Label>)</span>
            </div>
            <br />
        </li>

        <!-- ADDED INLAND MARINE FOR CPP MGB 3-7-18 -->
        <!-- CPP IM ONLY! -->
        <li runat="server" id="liInlandMarine" visible="false">
            <input id="hdnInlandMarineSection_EnabledOrDisabledFlag" name="hdnInlandMarineSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'IMRV');">
                <span runat="server" id="im_xImageArea" visible="false" style="float: right;"><%--
                --%><img src="images/incomplete.png" alt="Inland Marine Information Required" title="Inland Marine Information Required" /><%--
                --%></span><%--
                --%><span runat="server" id="im_DeleteButtonArea" visible="false" style="float: right;"><%--
                --%><asp:button ID="btnDeleteIM" runat="server" Text="Delete" Height="18px" Font-Size="Smaller" CssClass="StandardSaveButton btnTreeCIMDelete" OnClick="btnDeleteIM_Click" /> <%--
                --%></span><%--
                --%>&nbsp;<%--
                --%><asp:ImageButton runat="server" ID="imgBtnInlandMarine" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Inland Marine" CssClass="hidden" /><%--
                --%><%--&nbsp;--%><%--
                --%><span onclick="ClickRelatedChildButton(this, 'imgBtnInlandMarine');" title="Show Inland Marine" class="clickableHeader"><b>Inland Marine</b></span>
            </div>
            <br />
        </li>
        <!-- END OF INLAND MARINE FOR CPP -->

        <!-- ADDED CRIME FOR CPP MGB 3-7-18 -->
        <li runat="server" id="liCrime" visible="false">
            <input id="hdnCrimeSection_EnabledOrDisabledFlag" name="hdnCrimeSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CRIME');">
                <%--<input id="hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag" name="hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="crime_xImageArea" visible="false" style="float: right;"><%--
                --%><img src="images/incomplete.png" alt="Crime Information Required" title="Crime Information Required" /><%--
                --%></span><%--
                    --%><span runat="server" id="crime_DeleteButtonArea" visible="false" style="float: right;"><%--
                    --%><asp:button ID="btnDeleteCrime" runat="server" Text="Delete" Height="18px" Font-Size="Smaller" CssClass="StandardSaveButton btnTreeCRMDelete" OnClick="btnDeleteCrime_Click" /> <%--
                    --%></span><%--
                    --%>&nbsp;<%--
                    --%><asp:ImageButton runat="server" ID="imgBtnCrime" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Crime" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnCrime');" title="Show Crime" class="clickableHeader"><b>Crime</b></span>
            </div>
            <br />
        </li>
        <!-- END OF CRIME SECTION -->

        <li runat="server" id="liBillingInformation" visible="false">
            <input id="hdnBillingInfoSection_EnabledOrDisabledFlag" name="hdnBillingInfoSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'BILLINFO');">
                <%--<input id="hdnBillingInfoSection_EnabledOrDisabledFlag" name="hdnBillingInfoSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="billInfo_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Billing Information Required" title="Billing Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainBillingInfoSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="billInfo_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="billInfo_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainBillingInfoSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainBillingInfoSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainBillingInfoSectionSubListsExpandedOrCollapsed" name="hdnMainBillingInfoSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="billInfo_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnBillingInformation" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Billing Information" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnBillingInformation');" title="Show Billing Information" class="clickableHeader"><b>Billing Information</b></span>
            </div>
            <br />
        </li>

        <li runat="server" id="liPrintHistory" visible="false">
            <input id="hdnPrintHistSection_EnabledOrDisabledFlag" name="hdnPrintHistSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'PRTHIST');">
                <%--<input id="hdnPrintHistSection_EnabledOrDisabledFlag" name="hdnPrintHistSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="prtHist_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Print History Required" title="Print History Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainPrintHistSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="prtHist_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="prtHist_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainPrintHistSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainPrintHistSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainPrintHistSectionSubListsExpandedOrCollapsed" name="hdnMainPrintHistSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="prtHist_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnPrintHistory" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Print History" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnPrintHistory');" title="Show Print History" class="clickableHeader"><b>Print History</b></span>
            </div>
            <br />
        </li>

        <li runat="server" id="liPolicyHistory" visible="false">
            <input id="hdnPolicyHistSection_EnabledOrDisabledFlag" name="hdnPolicyHistSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'POLHIST');">
                <%--<input id="hdnPolicyHistSection_EnabledOrDisabledFlag" name="hdnPolicyHistSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="polHist_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Policy History Required" title="Policy History Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainPolicyHistSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="polHist_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="polHist_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainPolicyHistSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainPolicyHistSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainPolicyHistSectionSubListsExpandedOrCollapsed" name="hdnMainPolicyHistSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="polHist_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnPolicyHistory" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Policy History" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnPolicyHistory');" title="Show Policy History" class="clickableHeader"><b>Policy History</b></span>
            </div>
            <br />
        </li>

        <li runat="server" id="liQuoteSummary" visible="false">
            <input id="hdnQuoteSummarySection_EnabledOrDisabledFlag" name="hdnQuoteSummarySection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <input id="hdnQuoteSummarySection_SuccessfullyRatedFlag" name="hdnQuoteSummarySection_SuccessfullyRatedFlag" type="hidden" runat="server" /><%--added 6/3/2014--%>
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'QS');">
                <%--<input id="hdnQuoteSummarySection_EnabledOrDisabledFlag" name="hdnQuoteSummarySection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="q_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img runat="server" id="qs_ValidationErrorImage" src="~/images/incomplete.png" alt="Quote Summary Validation Errors Found" title="Quote Summary Validation Errors Found" /><%--updated 3/13/2014 from images/test/warning.gif; 3/19/2020: added "~/" to file path since it runs at server--%><%--
                --%></span><%--
                --%><span runat="server" id="MainQuoteSummarySectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="q_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('q', 'expand');" />
                        <img id="q_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('q', 'collapse');" />--%><%--
                        --%><img id="MainQuoteSummarySectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainQuoteSummarySectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainQuoteSummarySectionSubListsExpandedOrCollapsed" name="hdnMainQuoteSummarySectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="q_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnQuoteSummary" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Quote Summary" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span runat="server" id="qs_Header" onclick="ClickRelatedChildButtonFromSummaryHeader(this, 'imgBtnQuoteSummary');" title="Show Quote Summary" class="clickableHeader"><b>Quote Summary</b></span><%--6/3/2014 - updated span onclick from ClickRelatedChildButton--%><%--
                    --%><span runat="server" id="TotalPremiumSection" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblTotalPremium" Text="$0"></asp:Label></span>
            </div>
            <asp:Panel ID="pnlQuoteSummaryItems" runat="server" Visible="false">
                <%--added 4/3/2014--%>
                <asp:Repeater ID="rptQuoteSummaryItems" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="QuoteSummaryItemSubLists_expandCollapseImageArea"><%--
                            --%><%--<img id="QuoteSummaryItemSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="QuoteSummaryItemSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="QuoteSummaryItemSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="QuoteSummaryItemSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnQuoteSummaryItemSubListsExpandedOrCollapsed" name="hdnQuoteSummaryItemSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveQuoteSummaryItem" class="clickableHeader" title="Remove QuoteSummaryItem" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveQuoteSummaryItem');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%><%--
                                --%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveQuoteSummaryItem" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveQuoteSummaryItem_Click" ToolTip="Remove QuoteSummaryItem" CssClass="hidden" />--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><asp:Label ID="lblQuoteSummaryItemDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "QuoteSummaryItemDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblQuoteSummaryItemIdentifier" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "QuoteSummaryItemIdentifier")%>'></asp:Label><%--
                                --%><asp:Label ID="lblQuoteSummaryItemCount" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "QuoteSummaryItemCount")%>'></asp:Label>
                            <asp:Panel ID="pnlQuoteSummarySubItems" runat="server" Visible="false">
                                <asp:Repeater ID="rptQuoteSummarySubItems" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li><%--<asp:ImageButton runat="server" ID="imgBtnRemoveQuoteSummarySubItem" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveQuoteSummarySubItem_Click" ToolTip="Remove Quote Summary SubItem" CssClass="hidden" />--%><span runat="server" id="xSpanRemoveQuoteSummarySubItem" class="clickableHeader" title="Remove Quote Summary SubItem" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveQuoteSummarySubItem');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%>&nbsp;<asp:Label ID="lblQuoteSummarySubItemDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "QuoteSummarySubItemDescription") %>'></asp:Label></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liIRPM" visible="false"><%--added 7/27/2015 for Farm--%>
            <input id="hdnIRPMSection_EnabledOrDisabledFlag" name="hdnIRPMSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'IRPM');">
                <%--<input id="hdnIRPMSection_EnabledOrDisabledFlag" name="hdnIRPMSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="irpm_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="~/images/incomplete.png" alt="IRPM Information Required" title="IRPM Information Required" runat="server" id="warningGif"/><%--updated 3/13/2014 from images/test/warning.gif; 3/19/2020: added "~/" to file path since it runs at server--%><%--
                --%></span><%--
                --%><span runat="server" id="MainIRPMSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="irpm_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="irpm_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainIRPMSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainIRPMSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainIRPMSectionSubListsExpandedOrCollapsed" name="hdnMainIRPMSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="irpm_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnIRPM" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show IRPM" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnIRPM');" title="Show IRPM" class="clickableHeader" runat="server" id="IrpmTip"><b runat="server" id="IrpmTitle">IRPM</b></span>
            </div>
            <br />
        </li>
        <li runat="server" id="liCreditReports" visible="false">
            <input id="hdnCreditReportSection_EnabledOrDisabledFlag" name="hdnCreditReportSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CR');">
                <%--<input id="hdnCreditReportSection_EnabledOrDisabledFlag" name="hdnCreditReportSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="cr_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Credit/Tiering Information Required" title="Credit/Tiering Information Required" /><%--updated 3/13/2014 from images/test/warning.gif; 11/7/2014 - updated text from Credit Report to Credit/Tiering--%><%--
                --%></span><%--
                --%><span runat="server" id="MainCreditReportSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from cr_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="cr_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('c', 'expand');" />
                        <img id="cr_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('c', 'collapse');" />--%><%--
                        --%><img id="MainCreditReportSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from cr_expand; updated onclick from 'expandCollapse('c', 'expand');'--%>
                    <img id="MainCreditReportSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from cr_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('c', 'collapse');'--%>
                    <input id="hdnMainCreditReportSectionSubListsExpandedOrCollapsed" name="hdnMainCreditReportSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnCExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="cr_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<asp:ImageButton runat="server" ID="imgBtnCreditReports" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Credit Reports" CssClass="hidden" />--%><%--not currently being used; no code-behind logic--%><%--
                    --%><%--&nbsp;--%><%--
                    --%><%--<span onclick="ClickRelatedChildButton(this, 'imgBtnCreditReports');" title="Show Credit Reports" class="clickableHeader"><b>Credit Reports</b></span>--%><%--not currently being used--%><%--
                    --%><span title="Credit/Tiering" style="cursor: inherit;"><b>Credit/Tiering</b></span><%--updated 6/3/2014 to inherit cursor from section header... should either be default or hand... didn't work... still text cursor or hand; 11/7/2014 - updated text from Credit Reports to Credit/Tiering--%><%--
                    --%><span runat="server" id="RatedTierSection" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblRatedTier" Text=""></asp:Label></span>
            </div>
            <asp:Panel ID="pnlCreditReports" runat="server" Visible="false">
                <asp:Repeater ID="rptCreditReports" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                            <%--3/12/2014 note: may need 'style="list-style-type:none;"' to line up w/ other sections; added 3/14/2014 for consistency--%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="CreditReportSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="CreditReportSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="CreditReportSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="CreditReportSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="CreditReportSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnCreditReportSubListsExpandedOrCollapsed" name="hdnCreditReportSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveCreditReport" class="clickableHeader" title="Remove Credit Report" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveCreditReport');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%><%--
                                --%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveCreditReport" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveCreditReport_Click" ToolTip="Remove Credit Report" CssClass="hidden" />--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><input id="hdnIsSelected_CreditReport" name="hdnIsSelected_CreditReport" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnViewCreditReport" ImageUrl="../images/test/dark_account_maint.png" OnClick="imgBtnViewCreditReport_Click" ToolTip="View Credit Report" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblCreditReportDescription" runat="server" OnClick="ClickRelatedChildButton(this, 'imgBtnViewCreditReport');" ToolTip="View Credit Report" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "CreditReportDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblCreditReportEntityType" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "CreditReportEntityType") %>'></asp:Label><%--
                                --%><asp:Label ID="lblCreditReportEntityNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "CreditReportEntityNumber")%>'></asp:Label><%--
                                --%><asp:Label ID="lblCreditReportUnitNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "CreditReportUnitNumber") %>'></asp:Label>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liUnderwritingQuestions" visible="false">
            <input id="hdnUnderwritingQuestionSection_EnabledOrDisabledFlag" name="hdnUnderwritingQuestionSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'UW');">
                <%--<input id="hdnUnderwritingQuestionSection_EnabledOrDisabledFlag" name="hdnUnderwritingQuestionSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="u_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Underwriting Question Information Required" title="Underwriting Question Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainUnderwritingQuestionSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="u_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="u_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainUnderwritingQuestionSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainUnderwritingQuestionSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainUnderwritingQuestionSectionSubListsExpandedOrCollapsed" name="hdnMainUnderwritingQuestionSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="u_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnUnderwritingQuestions" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Underwriting Questions" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnUnderwritingQuestions');" title="Show Underwriting Questions" class="clickableHeader"><b>Underwriting Questions</b></span>
            </div>
            <br />
        </li>


        <li runat="server" id="liApplication" visible="false">
            <input id="hdnApplicationSection_EnabledOrDisabledFlag" name="hdnApplicationSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'APP');">
                <%--<input id="hdnApplicationSection_EnabledOrDisabledFlag" name="hdnApplicationSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="a_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Application Information Required" title="Application Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainApplicationSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="a_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="a_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainApplicationSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainApplicationSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainApplicationSectionSubListsExpandedOrCollapsed" name="hdnMainApplicationSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="a_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnApplication" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Application" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnApplication');" title="Show Application" class="clickableHeader"><b>Application</b></span>
            </div>
            <br />
        </li>

      

        <li runat="server" id="liMvrReports" visible="false">
            <input id="hdnMvrReportSection_EnabledOrDisabledFlag" name="hdnMvrReportSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'MVR');">
                <%--<input id="hdnMvrReportSection_EnabledOrDisabledFlag" name="hdnMvrReportSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="mvr_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="MVR Report Information Required" title="MVR Report Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainMvrReportSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from mvr_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="mvr_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('c', 'expand');" />
                        <img id="mvr_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('c', 'collapse');" />--%><%--
                        --%><img id="MainMvrReportSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from mvr_expand; updated onclick from 'expandCollapse('c', 'expand');'--%>
                    <img id="MainMvrReportSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from mvr_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('c', 'collapse');'--%>
                    <input id="hdnMainMvrReportSectionSubListsExpandedOrCollapsed" name="hdnMainMvrReportSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnCExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="mvr_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<asp:ImageButton runat="server" ID="imgBtnMvrReports" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show MVR Reports" CssClass="hidden" />--%><%--not currently being used; no code-behind logic--%><%--
                    --%><%--&nbsp;--%><%--
                    --%><%--<span onclick="ClickRelatedChildButton(this, 'imgBtnMvrReports');" title="Show MVR Reports" class="clickableHeader"><b>MVR Reports</b></span>--%><%--not currently being used--%><%--
                    --%><span title="MVR Reports" style="cursor: inherit;"><b>MVR Reports</b></span><%--updated 6/3/2014 to inherit cursor from section header... should either be default or hand... didn't work... still text cursor or hand--%></div>
            <asp:Panel ID="pnlMvrReports" runat="server" Visible="false">
                <asp:Repeater ID="rptMvrReports" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                            <%--3/12/2014 note: may need 'style="list-style-type:none;"' to line up w/ other sections; added 3/14/2014 for consistency--%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="MvrReportSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="MvrReportSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="MvrReportSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="MvrReportSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="MvrReportSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnMvrReportSubListsExpandedOrCollapsed" name="hdnMvrReportSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveMvrReport" class="clickableHeader" title="Remove MVR Report" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveMvrReport');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%><%--
                                --%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveMvrReport" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveMvrReport_Click" ToolTip="Remove MVR Report" CssClass="hidden" />--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><input id="hdnIsSelected_MvrReport" name="hdnIsSelected_MvrReport" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnViewMvrReport" ImageUrl="../images/test/dark_account_maint.png" OnClick="imgBtnViewMvrReport_Click" ToolTip="View MVR Report" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblMvrReportDescription" runat="server" OnClick="ClickRelatedChildButton(this, 'imgBtnViewMvrReport');" ToolTip="View MVR Report" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "MvrReportDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblMvrReportEntityType" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "MvrReportEntityType") %>'></asp:Label><%--
                                --%><asp:Label ID="lblMvrReportEntityNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "MvrReportEntityNumber")%>'></asp:Label><%--
                                --%><asp:Label ID="lblMvrReportUnitNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "MvrReportUnitNumber") %>'></asp:Label>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liClueReports" visible="false">
            <input id="hdnClueReportSection_EnabledOrDisabledFlag" name="hdnClueReportSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'CLUE');">
                <%--<input id="hdnClueReportSection_EnabledOrDisabledFlag" name="hdnClueReportSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="clue_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="CLUE Report Information Required" title="CLUE Report Information Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainClueReportSectionSubLists_expandCollapseImageArea"><%--renamed 3/13/2014 from clue_expandCollapseImageArea; removed ' visible="false"'--%><%--
                --%><%--<img id="clue_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('c', 'expand');" />
                        <img id="clue_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('c', 'collapse');" />--%><%--
                        --%><img id="MainClueReportSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" /><%--renamed 3/13/2014 from clue_expand; updated onclick from 'expandCollapse('c', 'expand');'--%>
                    <img id="MainClueReportSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--renamed 3/13/2014 from clue_collapse; updated to default to expanded w/ collapse image visible; updated onclick from 'expandCollapse('c', 'collapse');'--%>
                    <input id="hdnMainClueReportSectionSubListsExpandedOrCollapsed" name="hdnMainClueReportSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--renamed 3/13/2014 from hdnCExpandedOrCollapsed;--%><%--
                    --%></span><%--
                    --%><span runat="server" id="clue_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><%--<asp:ImageButton runat="server" ID="imgBtnClueReports" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show CLUE Reports" CssClass="hidden" />--%><%--not currently being used; no code-behind logic--%><%--
                    --%><%--&nbsp;--%><%--
                    --%><%--<span onclick="ClickRelatedChildButton(this, 'imgBtnClueReports');" title="Show CLUE Reports" class="clickableHeader"><b>CLUE Reports</b></span>--%><%--not currently being used--%><%--
                    --%><span title="CLUE Report" style="cursor: inherit;"><b>CLUE Report</b></span><%--updated 6/3/2014 to inherit cursor from section header... should either be default or hand... didn't work... still text cursor or hand--%></div>
            <asp:Panel ID="pnlClueReports" runat="server" Visible="false">
                <asp:Repeater ID="rptClueReports" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                            <%--3/12/2014 note: may need 'style="list-style-type:none;"' to line up w/ other sections; added 3/14/2014 for consistency--%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="ClueReportSubLists_expandCollapseImageArea"><%--added 3/12/2014; removed ' visible="false"' to match others--%><%--
                            --%><%--<img id="ClueReportSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="ClueReportSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="ClueReportSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="ClueReportSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnClueReportSubListsExpandedOrCollapsed" name="hdnClueReportSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveClueReport" class="clickableHeader" title="Remove CLUE Report" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveClueReport');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%><%--
                                --%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveClueReport" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveClueReport_Click" ToolTip="Remove CLUE Report" CssClass="hidden" />--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><input id="hdnIsSelected_ClueReport" name="hdnIsSelected_ClueReport" type="hidden" runat="server" /><asp:ImageButton runat="server" ID="imgBtnViewClueReport" ImageUrl="../images/test/dark_account_maint.png" OnClick="imgBtnViewClueReport_Click" ToolTip="View CLUE Report" CssClass="hidden" OnClientClick="SelectParentListItem(this);" /><%--
                                --%><%--&nbsp;--%><%--
                                --%><asp:Label ID="lblClueReportDescription" runat="server" OnClick="ClickRelatedChildButton(this, 'imgBtnViewClueReport');" ToolTip="View CLUE Report" CssClass="clickable" Text='<%# DataBinder.Eval(Container.DataItem, "ClueReportDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblClueReportEntityType" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ClueReportEntityType") %>'></asp:Label><%--
                                --%><asp:Label ID="lblClueReportUnitNumber" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ClueReportUnitNumber") %>'></asp:Label>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>
        <li runat="server" id="liApplicationSummary" visible="false">
            <input id="hdnApplicationSummarySection_EnabledOrDisabledFlag" name="hdnApplicationSummarySection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <input id="hdnApplicationSummarySection_SuccessfullyRatedFlag" name="hdnApplicationSummarySection_SuccessfullyRatedFlag" type="hidden" runat="server" /><%--added 6/3/2014--%>
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'AS');">
                <%--<input id="hdnApplicationSummarySection_EnabledOrDisabledFlag" name="hdnApplicationSummarySection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="as_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="Application Summary Validation Errors Found" title="Application Summary Validation Errors Found" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainApplicationSummarySectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="as_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('q', 'expand');" />
                        <img id="as_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('q', 'collapse');" />--%><%--
                        --%><img id="MainApplicationSummarySectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainApplicationSummarySectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainApplicationSummarySectionSubListsExpandedOrCollapsed" name="hdnMainApplicationSummarySectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="as_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnApplicationSummary" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show Application Summary" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButtonFromSummaryHeader(this, 'imgBtnApplicationSummary');" title="Show Application Summary" class="clickableHeader"><b>Application Summary</b></span><%--6/3/2014 - updated span onclick from ClickRelatedChildButton--%><%--
                    --%><span runat="server" id="TotalPremiumSection_Application" visible="false" style="float: right;" class="sectionHeaderPremium"><asp:Label runat="server" ID="lblTotalPremium_Application" Text="$0"></asp:Label></span>
            </div>
            <asp:Panel ID="pnlApplicationSummaryItems" runat="server" Visible="false">
                <%--added 4/3/2014--%>
                <asp:Repeater ID="rptApplicationSummaryItems" runat="server">
                    <HeaderTemplate>
                        <ul style="list-style-type: none;" class="subList">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <span runat="server" id="ApplicationSummaryItemSubLists_expandCollapseImageArea"><%--
                            --%><%--<img id="ApplicationSummaryItemSubLists_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapseSubLists(this, 'expand');" />
                                        <img id="ApplicationSummaryItemSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapseSubLists(this, 'collapse');" />--%><%--
                                        --%><img id="ApplicationSummaryItemSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                                <img id="ApplicationSummaryItemSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--updated 3/13/2014 to default to expanded w/ collapse image visible--%>
                                <input id="hdnApplicationSummaryItemSubListsExpandedOrCollapsed" name="hdnApplicationSummaryItemSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                                --%></span><%--
                                --%><span runat="server" id="xSpanRemoveApplicationSummaryItem" class="clickableHeader" title="Remove ApplicationSummaryItem" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveApplicationSummaryItem');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%><%--
                                --%><%--<asp:ImageButton runat="server" ID="imgBtnRemoveApplicationSummaryItem" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveApplicationSummaryItem_Click" ToolTip="Remove ApplicationSummaryItem" CssClass="hidden" />--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--added 3/12/2014 to match spacing of other sections--%><%--
                                --%><asp:Label ID="lblApplicationSummaryItemDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ApplicationSummaryItemDescription") %>'></asp:Label><%--
                                --%><asp:Label ID="lblApplicationSummaryItemIdentifier" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ApplicationSummaryItemIdentifier")%>'></asp:Label><%--
                                --%><asp:Label ID="lblApplicationSummaryItemCount" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ApplicationSummaryItemCount")%>'></asp:Label>
                            <asp:Panel ID="pnlApplicationSummarySubItems" runat="server" Visible="false">
                                <asp:Repeater ID="rptApplicationSummarySubItems" runat="server">
                                    <HeaderTemplate>
                                        <ul style="list-style-type: none;" class="subList2">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li><%--<asp:ImageButton runat="server" ID="imgBtnRemoveApplicationSummarySubItem" ImageUrl="../images/test/close.gif" OnClick="imgBtnRemoveApplicationSummarySubItem_Click" ToolTip="Remove Application Summary SubItem" CssClass="hidden" />--%><span runat="server" id="xSpanRemoveApplicationSummarySubItem" class="clickableHeader" title="Remove Application Summary SubItem" onclick="ClickRelatedChildButton(this, 'imgBtnRemoveApplicationSummarySubItem');" style="visibility: hidden;">X</span><%--just a placehold; has visibility hidden so it never shows; onClick wouldn't work anyway since button is commented out and there isn't code-behind logic for it--%>&nbsp;<asp:Label ID="lblApplicationSummarySubItemDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ApplicationSummarySubItemDescription") %>'></asp:Label></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </li>
                    </ItemTemplate>
                    <%--<AlternatingItemTemplate>
                    </AlternatingItemTemplate>--%>
                    <%--<SeparatorTemplate>
                    </SeparatorTemplate>--%>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
            <br />
        </li>

          <li runat="server" id="liFileUpload" visible="false">
            <input id="hdnFileUploadSection_EnabledOrDisabledFlag" name="hdnFileUploadSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="sectionHeader" onclick="sectionHeaderClick(this, 'FILEUPLOAD');">
                <%--<input id="hdnUnderwritingQuestionSection_EnabledOrDisabledFlag" name="hdnUnderwritingQuestionSection_EnabledOrDisabledFlag" type="hidden" runat="server" />--%>
                <span runat="server" id="file_xImageArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                --%><img src="images/incomplete.png" alt="File Upload Required" title="File Upload Required" /><%--updated 3/13/2014 from images/test/warning.gif--%><%--
                --%></span><%--
                --%><span runat="server" id="MainFileUploadSectionSubLists_expandCollapseImageArea"><%--
                --%><%--<img id="u_expand" class="clickable" alt="expand" title="expand" src="images/test/expand.png" onclick="expandCollapse('u', 'expand');" />
                        <img id="u_collapse" class="clickable" alt="collapse" title="collapse" src="images/test/collapse.png" onclick="expandCollapse('u', 'collapse');" />--%><%--
                        --%><img id="MainFileUploadSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainFileUploadSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" /><%--3/13/2014 - updated to default to expanded w/ collapse image visible--%>
                    <input id="hdnMainFileUploadSectionSubListsExpandedOrCollapsed" name="hdnMainFileUploadSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" /><%--
                    --%></span><%--
                    --%><span runat="server" id="file_checkMarkArea" visible="false" style="float: right;"><%--added style 3/13/2014--%><%--
                    --%><%--<img src="images/complete.png" />--%><%--updated 3/14/2014 from images/test/button_checkmark_on.gif--%><%--
                    --%></span><%--
                    --%>&nbsp;<%--removed 3/12/2014 to test spacing; then later un-commented--%><%--
                    --%><asp:ImageButton runat="server" ID="imgBtnFileUpload" ImageUrl="../images/test/dark_account_maint.png" ToolTip="Show File Upload" CssClass="hidden" /><%--
                    --%><%--&nbsp;--%><%--
                    --%><span onclick="ClickRelatedChildButton(this, 'imgBtnFileUpload');" title="Show Upload a File" class="clickableHeader"><b>Upload a File (<asp:Label ID="lblFileUploadCount" ClientIDMode="Static" runat="server" Text="0"></asp:Label>)</b></span>
                <asp:HiddenField ID="hdnTreeFileUploadCount" ClientIDMode="Static" Value="0" runat="server" />
            </div>
            <br />
        </li>

        <li runat="server" id="liRouteToUW" visible="false">
            <input id="hdnRouteToUWSection_EnabledOrDisabledFlag" name="hdnRouteToUWSection_EnabledOrDisabledFlag" type="hidden" runat="server" />
            <div class="StandardButton" title="Route to Underwriting" onclick="ClickRouteButton(this, 'DIV');" style="padding:2px; height:auto;">
                <span runat="server" id="routeToUW_xImageArea" visible="false" style="float: right;">
                    <img src="images/incomplete.png" alt="Route to Underwriting Required" title="Route to Underwriting Required" />
                </span>
                <span runat="server" id="MainRouteToUWSectionSubLists_expandCollapseImageArea">
                    <img id="MainRouteToUWSectionSubLists_expand" class="clickable" alt="expand" title="expand" src="images/colaspe.png" onclick="expandCollapseSubLists(this, 'expand');" style="display: none;" />
                    <img id="MainRouteToUWSectionSubLists_collapse" class="clickable" alt="collapse" title="collapse" src="images/expand.png" onclick="expandCollapseSubLists(this, 'collapse');" style="display: inline;" />
                    <input id="hdnMainRouteToUWSectionSubListsExpandedOrCollapsed" name="hdnMainRouteToUWSectionSubListsExpandedOrCollapsed" type="hidden" runat="server" />
                </span>
                <span runat="server" id="routeToUW_checkMarkArea" visible="false" style="float: right;">

                </span>
                &nbsp;
                <%--<span onclick="ClickRouteButton(this, 'SPAN');" title="Route to Underwriting" class="clickableHeader"><b>Route to Underwriting</b></span>--%>
                <span><b>Route to Underwriting</b></span>
            </div>
            <div class="hidden">
                <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" HideFromParent="true" />
                <uc1:ctl_Email_UW ID="ctl_EmailUW" runat="server" />
            </div>            
            <input id="hdnRouteCommOrPers" name="hdnRouteCommOrPers" type="hidden" runat="server" />
            <br />
        </li>

    </ul>
    <div class="hidden">
        <asp:Button runat="server" ID="btnExpandAll" Text="Expand All (server-side)" />
        <input type="button" id="btnCollapseAll" name="btnCollapseAll" value="Collapse All (client-side)" onclick="CollapseAll();" />
        <asp:Button runat="server" ID="btnToggleViewMode" Text="Toggle View Mode" />
    </div>
    <%--testing UpdatePanel 4/1/2014... hits code-behind and raises event, but page needs postback to update view... Save happens on save buttons, but textbox is still visible like when in edit mode--%>
    <%--<asp:UpdatePanel ID="up_TreeButtons" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="imgBtnSaveQuoteDescription" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnSaveEffectiveDate" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnPolicyholders" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnDrivers" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnVehicles" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnLocations" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnPolicyholders" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnCoverages" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnUnderwritingQuestions" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnQuoteSummary" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnDiscounts" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="imgBtnSurcharges" EventName="Click" />
        </Triggers>
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Panel>
<asp:Panel ID="pnlTreeViewError" runat="server" Visible="false">
    <div>Nothing found!</div>
</asp:Panel>
<div id="TreeTheme" class="TreeTheme">
    <%--<img title="The Holiday Tree(view)" style="margin-left: auto; margin-right: auto; width: 160px; height: 260px;" src="images/VRThemes/5-global-christmas-tree_small.png" />--%>
</div>
<input id="hdnExpandOrCollapseAllFlag" name="hdnExpandOrCollapseAllFlag" type="hidden" runat="server" />
<input id="hdnDeselectAllListItemsFlag" name="hdnDeselectAllListItemsFlag" type="hidden" runat="server" />
<input id="hdnInEditModeFlag" name="hdnInEditModeFlag" type="hidden" runat="server" />
<input id="hdnQuoteTransactionTypeFlag" name="hdnQuoteTransactionTypeFlag" type="hidden" runat="server" /><%--added 2/21/2019--%>

<%--<asp:Panel ID="pnlLOBVersionFields" runat="server" style="display:none;"></asp:Panel>--%>

<script type="text/javascript">
    InitializeTree(); //changed from InitializePage 5/15/2014
</script>

<script type="text/javascript">
    // Delete IM button Warnings
    $(function () {
        $('.btnTreeCIMDelete').on('click', function (event) {
            if (confirm('Are you sure you want to completely delete the Inland Marine Section from this quote?')) { return true; } else { return false; }
        });
    });
    // Delete Crime button Warnings
    $(function () {
        $('.btnTreeCRMDelete').on('click', function (event) {
            if (confirm('Are you sure you want to completely delete the Crime Section from this quote?” ')) { return true; } else { return false; }
        });
    });
</script>

<%--</ContentTemplate>
</asp:UpdatePanel>--%>