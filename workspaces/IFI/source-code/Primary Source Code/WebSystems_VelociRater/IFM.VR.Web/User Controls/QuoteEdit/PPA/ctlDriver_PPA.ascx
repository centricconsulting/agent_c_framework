<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDriver_PPA.ascx.vb" Inherits="IFM.VR.Web.ctlDriver_PPA" %>
<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlViolationList.ascx" TagPrefix="uc1" TagName="ctlViolationList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryItem.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryItem" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Driver" CssClass="RemovePanelLink">Add Driver</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Remove this Driver" runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save all drivers" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <%-- PPA Input Div --%>
    <div id="divPPA" runat="server">
        <table style="width: 100%">
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnCopyFromPh1" CssClass="StandardButton" runat="server" Text="Copy from Policyholder #1" />
                    <asp:Button ID="btnCopyFromPh2" CssClass="StandardButton" Style="margin-left: 40px;" runat="server" Text="Copy from Policyholder #2" />
                </td>
            </tr>
            <tr>
                <td style="width: 260px;">
                    <label for="<%=Me.txtFirstName.ClientID%>">*First Name:</label>
                    <br />
                    <asp:TextBox ID="txtFirstName" TabIndex="1" runat="server" MaxLength="50" autofocus></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.ddMaritialStatus.ClientID%>">*Marital Status:</label>
                    <br />
                    <asp:DropDownList ID="ddMaritialStatus" TabIndex="7" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name:</label>
                    <br />
                    <asp:TextBox ID="txtMiddleName" TabIndex="2" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.txtDLNumber.ClientID%>"><asp:Label ID="DLRequired" runat="server" Text="*" visible="false"></asp:Label>DL Number:</label>
                    <br />
                    <asp:TextBox ID="txtDLNumber" runat="server" TabIndex="8" MaxLength="19"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtLastname.ClientID%>">*Last Name:</label>
                    <br />
                    <asp:TextBox ID="txtLastname" TabIndex="3" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.ddDLState.ClientID%>"><asp:Label ID="DLStateRequired" runat="server" Text="*" visible="false"></asp:Label>DL State:</label>
                    <br />
                    <asp:DropDownList ID="ddDLState" TabIndex="9" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddSuffix.ClientID%>">Suffix:</label>
                    <br />
                    <asp:DropDownList ID="ddSuffix" TabIndex="4" runat="server"></asp:DropDownList>
                </td>
                <td>
                    <label for="<%=Me.txtDLDate.ClientID%>"><asp:Label ID="DLDateRequired" runat="server" Text="*" visible="false"></asp:Label>DL Date:</label>
                    <br />
                    <asp:TextBox ID="txtDLDate" onblur="$(this).val(dateFormat($(this).val())); ifm.vr.ui.LockTree_Freeze(); " TabIndex="10" MaxLength="19" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date:</label>
                    <br />
                    <asp:TextBox ID="txtBirthDate" TabIndex="5" MaxLength="10" runat="server"></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.ddRelationToPolicyHolder.ClientID%>">*Relation to Policyholder:</label>
                    <br />
                    <asp:DropDownList ID="ddRelationToPolicyHolder" TabIndex="11" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.ddSex.ClientID%>">*Gender:</label>
                    <br />
                    <asp:DropDownList ID="ddSex" TabIndex="6" runat="server"></asp:DropDownList>
                </td>
                <td class="auto-style1">
                    <label for="<%=Me.ddRatedOrExcludedDriver.ClientID%>">*Rated/Excluded Driver:</label>
                    <br />
                    <asp:DropDownList ID="ddRatedOrExcludedDriver" TabIndex="12" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtDefensiveDriverDate.ClientID%>">Defensive Driver Course Date:</label>
                    <br />
                    <asp:TextBox ID="txtDefensiveDriverDate" onblur="$(this).val(dateFormat($(this).val()));" MaxLength="10" TabIndex="13" runat="server"></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.txtAccidentPreventionCourse.ClientID%>">Mature Driver Course Date:</label>
                    <br />
                    <asp:TextBox ID="txtAccidentPreventionCourse" onblur="$(this).val(dateFormat($(this).val()));" MaxLength="10" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>

        <div id="divGoodStudentContainer" class="standardSubSection" runat="server">
            <div id="divGoodStudent" runat="server">
                <h3 id="h3GoodStudent" runat="server">Good Student <span style="float: right;">
                    <asp:LinkButton ID="lnkBtnSaveGoodStudent" ToolTip="Save all Drivers" runat="server">Save</asp:LinkButton></span>
                </h3>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 300px;">
                                <label for="<%=Me.chkGoodStudent.ClientID%>">Good Student</label></td>
                            <td>
                                <asp:CheckBox ID="chkGoodStudent" ToolTip="Good Student" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.chkDistantStudent.ClientID%>">Distant Student</label></td>
                            <td>
                                <asp:CheckBox ID="chkDistantStudent" ToolTip="Distant Student" runat="server" />
                            </td>
                        </tr>
                        <tr runat="server" id="trDistanceToSchool">
                            <td>
                                <label for="<%=Me.txtMilesSchool.ClientID%>">Distance to School(miles)</label></td>
                            <td>
                                <asp:TextBox ID="txtMilesSchool" onkeyup='$(this).val(FormatAsNumber($(this).val()));' runat="server" MaxLength="5"></asp:TextBox></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <div id="divMotorCycle" runat="server" class="standardSubSection">
            <h3 id="h3MotorCycle" runat="server">Motorcycle<span style="float: right;">
                <asp:LinkButton ID="lnkBtnSaveMotor" ToolTip="Save all Drivers" runat="server">Save</asp:LinkButton></span></h3>
            <div>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 300px;">
                            <label for="<%=Me.txtMotorcycleTrainingDate.ClientID%>">Motorcycle Training/Membership Date</label></td>
                        <td>
                            <asp:TextBox ID="txtMotorcycleTrainingDate" onblur="$(this).val(dateFormat($(this).val()));" MaxLength="10" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkMotorcycleClubMember.ClientID%>">Motorcycle Club Member</label></td>
                        <td>
                            <asp:CheckBox ID="chkMotorcycleClubMember" ToolTip="Motorcycle Club Member" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.txtMotorCycleYearsOfExperience.ClientID%>">Years of Motorcycle Experience</label></td>
                        <td>
                            <asp:TextBox ID="txtMotorCycleYearsOfExperience" MaxLength="3" onkeyup='$(this).val(FormatAsNumber($(this).val()));' runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkMotorcycleDiscount.ClientID%>">Motorcycle Safety Course Discount</label></td>
                        <td>
                            <asp:CheckBox ID="chkMotorcycleDiscount" ToolTip="Motorcycle Safety Course Discount" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="divNonOwned" runat="server" class="standardSubSection">
            <h3>Non-Owned <span style="float: right;">
                <asp:LinkButton ID="lnkBtnSaveNonOwned" ToolTip="Save all Drivers" runat="server">Save</asp:LinkButton></span></h3>
            <div>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 300px;">
                            <label for="<%=Me.chkExtendedNonOwned.ClientID%>">Extended Non-Owned</label>
                        </td>
                        <td>
                            <asp:CheckBox ToolTip="Extended Non-Owned " ID="chkExtendedNonOwned" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkPrimaryGarageLiability.ClientID%>">Primary/Garage Liability Insurance Provided</label></td>
                        <td>
                            <asp:CheckBox ToolTip="Primary/Garage Liability Insurance Provided" ID="chkPrimaryGarageLiability" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkUsedInGov.ClientID%>">Used In Government Business</label></td>
                        <td>
                            <asp:CheckBox ToolTip="Used In Government Business" ID="chkUsedInGov" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkFurnishedForUse.ClientID%>">Vehicle Furnished For Regular Use</label></td>
                        <td>
                            <asp:CheckBox ID="chkFurnishedForUse" ToolTip="Vehicle Furnished For Regular Use" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkEmployedByGarage.ClientID%>">Employed By Garage</label></td>
                        <td>
                            <asp:CheckBox ID="chkEmployedByGarage" ToolTip="Employed By Garage" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.radNamedInd_AndResident.ClientID%>">Relationship to the Insured</label></td>
                        <td>
                            <asp:RadioButton ID="radNamedOnly" runat="server" ToolTip="Relationship to the Insured" GroupName="radRelation" Text="Named Individuals Only" />
                            <br />
                            <asp:RadioButton ID="radNamedInd_AndResident" ToolTip="Relationship to the Insured" runat="server" GroupName="radRelation" Text="Named Individuals and Resident Relatives" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="<%=Me.chkExcludedMedical.ClientID%>">Excluded Medical Payment</label></td>
                        <td>
                            <asp:CheckBox ID="chkExcludedMedical" ToolTip="Excluded Medical Payment" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

    <%-- CAP Input Div --%>
    <div id="divCAP" runat="server">
        <table style="width: 100%">
            <tr>
                <td style="width: 260px;">
                    <label for="<%=Me.txtCAPFirstName.ClientID%>">*First Name:</label>
                    <br />
                    <asp:TextBox ID="txtCAPFirstName" TabIndex="1" runat="server" MaxLength="50" autofocus></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.txtCAPBirthDate.ClientID%>">*Birth Date:</label>
                    <br />
                    <asp:TextBox ID="txtCAPBirthDate" TabIndex="5" MaxLength="10" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtCAPMiddleName.ClientID%>">Middle Name:</label>
                    <br />
                    <asp:TextBox ID="txtCAPMiddleName" TabIndex="2" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.txtCAPDLNumber.ClientID%>">DL Number:</label>
                    <br />
                    <asp:TextBox ID="txtCAPDLNumber" runat="server" TabIndex="8" MaxLength="19"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtCAPLastname.ClientID%>">*Last Name:</label>
                    <br />
                    <asp:TextBox ID="txtCAPLastName" TabIndex="3" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.ddCAPDLState.ClientID%>">DL State:</label>
                    <br />
                    <asp:DropDownList ID="ddCAPDLState" TabIndex="9" runat="server"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>

    <uc1:ctlAccidentHistoryList runat="server" ID="ctlAccidentHistoryList" />

    <uc1:ctlViolationList runat="server" ID="ctlViolationList" />
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <asp:HiddenField ID="HiddenField2" runat="server" />
    <asp:HiddenField ID="HiddenField3" runat="server" />
    <br />
</div>