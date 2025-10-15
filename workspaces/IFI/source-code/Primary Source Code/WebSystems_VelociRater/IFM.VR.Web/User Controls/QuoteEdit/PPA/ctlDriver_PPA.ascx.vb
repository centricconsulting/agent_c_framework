Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions

Public Class ctlDriver_PPA
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Event DriverIndexRemoving(index As Int32)
    Public Event NewDriverRequested()

    Public ReadOnly Property MyDriver As QuickQuoteDriver
        Get
            'Updated 9/17/18 for multi state MLW
            'If Me.Quote.IsNotNull Then
            If Me.Quote IsNot Nothing Then
                'Return Me.Quote.Drivers.GetItemAtIndex(Me.DriverIndex)
                Return Me.GoverningStateQuote.Drivers.GetItemAtIndex(Me.DriverIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property DriverIndex As Int32
        Get
            Return ViewState.GetInt32("vs_driverNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
            SetHeaderTextJavascript()
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.DriverIndex
        End Get
    End Property

    Private ReadOnly Property HasPolicyholderTwoAvailable As Boolean
        Get
            'Updated 9/17/18 for multi state MLW
            'If Me.Quote.IsNotNull AndAlso Me.Quote.Policyholder2.IsNotNull AndAlso Me.Quote.Policyholder2.Name.IsNotNull Then
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Policyholder2.IsNotNull AndAlso Me.Quote.Policyholder2.Name.IsNotNull Then
                If NoneAreNullEmptyorWhitespace(Me.Quote.Policyholder2.Name.FirstName, Me.Quote.Policyholder2.Name.LastName) Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property

    'added 6/18/2020
    Public ReadOnly Property ShouldGenericLossHistoryControlBeUsed As Boolean
        Get
            If IsOnAppPage = False AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso QuickQuoteHelperClass.PPA_CheckDictionaryKeyToOrderClueAtQuoteRate() = True AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData() ' probably not needed
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddSex.Items.Count < 1 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddSuffix, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.SuffixName, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddSex, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.SexId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddMaritialStatus, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.MaritalStatusId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddRelationToPolicyHolder, QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.RelationshipTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddRatedOrExcludedDriver, QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.DriverExcludeTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddDLState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            If Quote.LobType = QuickQuoteLobType.AutoPersonal Then
                Dim Item = New ListItem("NON-US", "306")
                If Me.ddDLState.Items.Contains(Item) Then
                ddDLState.Items.Remove(Item)
                End If
            End If
        End If
    End Sub


    Private Sub SetHeaderTextJavascript()
        Dim scriptHeaderUpdate As String = "updateDriverHeaderText(""" + Me.lblAccordHeader.ClientID + """,""" + (Me.DriverIndex + 1).ToString() + """,""" + Me.txtFirstName.ClientID + """,""" + Me.txtLastname.ClientID + """,""" + Me.txtMiddleName.ClientID + """," + Me.DriverIndex.ToString() + "); "
        Me.VRScript.CreateJSBinding({Me.txtFirstName, Me.txtMiddleName, Me.txtLastname}, ctlPageStartupScript.JsEventType.onkeyup, scriptHeaderUpdate)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()

        divPPA.Attributes.Add("style", "display:none")
        divCAP.Attributes.Add("style", "display:none")

        'added 6/18/2020
        If ShouldGenericLossHistoryControlBeUsed = True Then
            Me.ctlAccidentHistoryList.Visible = False
        Else
            Me.ctlAccidentHistoryList.Visible = True
        End If

        If Quote IsNot Nothing AndAlso MyDriver IsNot Nothing Then
            Select Case Quote.LobType
                Case QuickQuoteLobType.AutoPersonal
                    divPPA.Attributes.Add("style", "display:''")

                    Me.lblAccordHeader.Text = "Driver #{0} - {1} {2}".FormatIFM(Me.DriverIndex + 1, MyDriver.Name.FirstName, MyDriver.Name.LastName)
                    If MyDriver.Name.MiddleName.IsNullEmptyorWhitespace = False Then
                        Me.lblAccordHeader.Text = "Driver #{0} - {1} {2} {3}".FormatIFM(Me.DriverIndex + 1, MyDriver.Name.FirstName, MyDriver.Name.MiddleName, MyDriver.Name.LastName)
                    End If

                    Me.lblAccordHeader.Text = Me.lblAccordHeader.Text.Ellipsis(40)

                    Me.txtFirstName.Text = MyDriver.Name.FirstName
                    Me.txtMiddleName.Text = MyDriver.Name.MiddleName
                    Me.txtLastname.Text = MyDriver.Name.LastName

                    SetHeaderTextJavascript()

                    'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddSuffix, MyDriver.Name.SuffixName.Replace(".", ""))
                    SetdropDownFromValue_ForceSeletion(Me.ddSuffix, MyDriver.Name.SuffixName, MyDriver.Name.SuffixName) 'CAH B41922
                    If IsDate(MyDriver.Name.BirthDate) Then
                        Me.txtBirthDate.Text = MyDriver.Name.BirthDate.ReturnEmptyIfDefaultDiamondDate
                    Else
                        ' keep what ever was there
                    End If

                    Me.ddSex.SetFromValue(MyDriver.Name.SexId)
                    Me.ddMaritialStatus.SetFromValue(MyDriver.Name.MaritalStatusId)
                    Me.txtDLNumber.Text = MyDriver.Name.DriversLicenseNumber



                    Dim _dl As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, "IN", Me.Quote.LobType)

                    If MyDriver.Name.DriversLicenseStateId.EqualsAny("", "0") Then
                        SetdropDownFromValue(Me.ddDLState, "16")
                    Else
                        'Me.ddDLState.SetFromValue(MyDriver.Name.DriversLicenseStateId)
                        SetDropDownValue_ForceDiamondValue(Me.ddDLState, MyDriver.Name.DriversLicenseStateId, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId)
                    End If
                    If IsDate(MyDriver.Name.DriversLicenseDate) Then
                        Me.txtDLDate.Text = MyDriver.Name.DriversLicenseDate.ReturnEmptyIfDefaultDiamondDate
                    Else
                        'keep what ever was there
                    End If

                    Me.ddRelationToPolicyHolder.SetFromValue(MyDriver.RelationshipTypeId)
                    If IsQuoteEndorsement() AndAlso IsNewDriverOnEndorsement(MyDriver) = False AndAlso (MyDriver.RelationshipTypeId = "8" OrElse MyDriver.RelationshipTypeId = "5") Then
                        Me.lnkRemove.Visible = False
                        Me.ddRelationToPolicyHolder.Enabled = False
                    End If

                    If MyDriver.DriverExcludeTypeId.IsNullEmptyorWhitespace Then
                        Me.ddRatedOrExcludedDriver.SetFromValue(QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteDriver, QuickQuotePropertyName.DriverExcludeTypeId, "Rated", Me.Quote.LobType))
                    Else
                        Me.ddRatedOrExcludedDriver.SetFromValue(MyDriver.DriverExcludeTypeId)
                    End If

                    If IsDate(MyDriver.DefDriverDate) Then
                        Me.txtDefensiveDriverDate.Text = MyDriver.DefDriverDate.ReturnEmptyIfDefaultDiamondDate
                    Else
                        ' just keep whatever is there
                    End If
                    If IsDate(MyDriver.AccPreventionCourse) Then
                        Me.txtAccidentPreventionCourse.Text = MyDriver.AccPreventionCourse.ReturnEmptyIfDefaultDiamondDate
                    Else
                        'just keep whatever is there now
                    End If

                    If Me.txtBirthDate.Text.IsDate Then
                        Dim dob As Date = Date.Parse(Me.txtBirthDate.Text)
                        Dim ageInYrs As Integer = Date.Now.Year - dob.Year
                        If Date.Now.Month < dob.Month _
                        OrElse (Date.Now.Month = dob.Month AndAlso Date.Now.Day < dob.Day) Then
                            ageInYrs -= 1
                        End If
                        If (ageInYrs < 25) Then
                            'only available to drivers under 25 years of age
                            Me.chkGoodStudent.Checked = MyDriver.GoodStudent
                            Me.chkDistantStudent.Checked = MyDriver.DistantStudent

                            Me.txtMilesSchool.Text = MyDriver.SchoolDistance.Trim().ReturnEmptyIfEqualsAny("0")


                        Else
                            Me.chkGoodStudent.Checked = False
                            Me.chkDistantStudent.Checked = False
                            Me.txtMilesSchool.Text = ""
                        End If
                    Else
                        'can't tell age so this is not valid
                        Me.chkGoodStudent.Checked = False
                        Me.chkDistantStudent.Checked = False
                        Me.txtMilesSchool.Text = ""
                    End If

                    If IsDate(MyDriver.MotorMembershipDate) Then
                        Me.txtMotorcycleTrainingDate.Text = MyDriver.MotorMembershipDate.ReturnEmptyIfDefaultDiamondDate
                    Else
                        ' just keeps whatever is there now
                    End If

                    Me.chkMotorcycleClubMember.Checked = MyDriver.MotorClub

                    Me.txtMotorCycleYearsOfExperience.Text = MyDriver.MotorcycleYearsExperience.Trim().ReturnEmptyIfEqualsAny("0")

                    Me.chkMotorcycleDiscount.Checked = MyDriver.MotorcycleTrainingDisc

                    Me.chkExtendedNonOwned.Checked = MyDriver.ExtendedNonOwned
                    Me.chkPrimaryGarageLiability.Checked = MyDriver.EnolPrimaryLiability
                    Me.chkUsedInGov.Checked = MyDriver.EnolGovtBusiness
                    Me.chkFurnishedForUse.Checked = MyDriver.EnolRegularUse
                    Me.chkEmployedByGarage.Checked = MyDriver.EnolEmployedByGarage

                    Me.radNamedOnly.Checked = MyDriver.EnolNamedInsured
                    Me.radNamedInd_AndResident.Checked = Not MyDriver.EnolNamedInsured

                    Me.chkExcludedMedical.Checked = MyDriver.EnolMedpayExclude

                    Me.ctlViolationList.DriverIndex = Me.DriverIndex
                    Me.ctlAccidentHistoryList.DriverIndex = Me.DriverIndex

                    'CAH 05/07/2019
                    If IsQuoteEndorsement() Then
                        DLRequired.Visible = True
                        DLStateRequired.Visible = True
                        DLDateRequired.Visible = True
                    Else
                        DLRequired.Visible = False
                        DLStateRequired.Visible = False
                        DLDateRequired.Visible = False
                    End If

                    Me.PopulateChildControls()
                    Exit Select
                Case QuickQuoteLobType.CommercialAuto
                    divCAP.Attributes.Add("style", "display:''")
                    Exit Select
                Case Else
                    Exit Select
            End Select
        End If

        If MyDriver IsNot Nothing Then

        End If

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Driver #{0}", Me.DriverIndex + 1)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim accordListPlusGoodStudent As New List(Of VRAccordionTogglePair)
        accordListPlusGoodStudent.AddRange(accordList)
        accordListPlusGoodStudent.Add(New VRAccordionTogglePair(Me.divGoodStudent.ClientID, "0"))

        Dim accordListPlusMotorCycle As New List(Of VRAccordionTogglePair)
        accordListPlusMotorCycle.AddRange(accordList)
        accordListPlusMotorCycle.Add(New VRAccordionTogglePair(Me.divMotorCycle.ClientID, "0"))

        Dim valItems = DriverValidator.ValidateDriver(Me.DriverIndex, Me.Quote, valArgs.ValidationType)
        If valItems.Any() Then

            For Each v In valItems
                Select Case v.FieldId
                    Case DriverValidator.DriverFirstname
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, accordList)
                    Case DriverValidator.DriverLastname
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastname, v, accordList)
                    Case DriverValidator.DriverBirthDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBirthDate, v, accordList)
                    Case DriverValidator.DriverGender
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddSex, v, accordList)
                    Case DriverValidator.DriverMaritalStatus
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddMaritialStatus, v, accordList)
                    Case DriverValidator.DriverDLNumber
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDLNumber, v, accordList)
                    Case DriverValidator.DriverDLDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDLDate, v, accordList)
                    Case DriverValidator.DriverRelationship
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddRelationToPolicyHolder, v, accordList)
                    Case DriverValidator.DriverRelationshipPh1Duplicated
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddRelationToPolicyHolder, v, accordList)
                    Case DriverValidator.DriverRelationshipPh2Duplicated
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddRelationToPolicyHolder, v, accordList)
                    Case DriverValidator.DriverDefensiveDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDefensiveDriverDate, v, accordList)
                    Case DriverValidator.DriverMatureDriverDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAccidentPreventionCourse, v, accordList)
                    Case DriverValidator.DriverGoodStudentAge ' gets this if the age is 25 or more and distant driver/good driver/ or distance is set
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMilesSchool, v, accordListPlusGoodStudent)
                    Case DriverValidator.DriverDistanceToSchool
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMilesSchool, v, accordListPlusGoodStudent)
                    Case DriverValidator.DriverMotorCycleTrainingDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMotorcycleTrainingDate, v, accordListPlusMotorCycle)
                    Case DriverValidator.DriverMotorCycleYearsOfExperience
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMotorCycleYearsOfExperience, v, accordListPlusMotorCycle)
                End Select
            Next
        End If
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        ' START - this crazy stuff is needed because of the custom tab orders required
        Me.VRScript.CreateJSBinding(Me.txtDefensiveDriverDate, ctlPageStartupScript.JsEventType.onkeydown, "TabLogicTxtDefensiveDriver(event,'" + Me.txtAccidentPreventionCourse.ClientID + "','" + Me.h3GoodStudent.ClientID + "','" + Me.h3MotorCycle.ClientID + "');")
        ' END - this crazy stuff is needed because of the custom tab orders required

        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove this Driver?")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        Me.VRScript.StopEventPropagation(Me.lnkBtnSaveGoodStudent.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSaveMotor.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSaveNonOwned.ClientID)

        Me.VRScript.CreateAccordion(Me.divGoodStudent.ClientID, HiddenField1, "false")
        Me.VRScript.CreateAccordion(Me.divMotorCycle.ClientID, HiddenField2, "false")
        Me.VRScript.CreateAccordion(Me.divNonOwned.ClientID, HiddenField3, "false")

        Me.VRScript.CreateDatePicker(Me.txtDLDate.ClientID, True)
        Me.VRScript.CreateDatePicker(Me.txtDefensiveDriverDate.ClientID, True)
        Me.VRScript.CreateDatePicker(Me.txtAccidentPreventionCourse.ClientID, True)
        Me.VRScript.CreateDatePicker(Me.txtMotorcycleTrainingDate.ClientID, True)

        Dim goodStudentScript As String = "ToggleGoodDriverDiv(""" + Me.divGoodStudentContainer.ClientID + """,""" + Me.txtBirthDate.ClientID + """);"
        Dim accidentCourse As String = "ToggleAccidentCourse('" + Me.txtAccidentPreventionCourse.ClientID + "','" + Me.txtBirthDate.ClientID + "');"
        Dim calcDL_Date As String = "DLDateCalc('" + Me.txtDLDate.ClientID + "','" + Me.txtBirthDate.ClientID + "');"

        Me.VRScript.CreateJSBinding(Me.txtBirthDate, ctlPageStartupScript.JsEventType.onkeyup, "$(this).val(dateFormat($(this).val())); ifm.vr.ui.LockTree_Freeze(); " + goodStudentScript + accidentCourse)
        Me.VRScript.CreateJSBinding(Me.txtBirthDate, ctlPageStartupScript.JsEventType.onblur, calcDL_Date)

        ' need to run it on page load as well
        Me.VRScript.AddScriptLine(goodStudentScript + accidentCourse)

        ' distant student check event
        Me.VRScript.CreateJSBinding(Me.chkDistantStudent, ctlPageStartupScript.JsEventType.onchange, "ToggleDistantDriver('" + Me.chkDistantStudent.ClientID + "','" + trDistanceToSchool.ClientID + "','" + Me.txtMilesSchool.ClientID + "');")

        ' distant student page load run once
        Me.VRScript.AddScriptLine("ToggleDistantDriver('" + Me.chkDistantStudent.ClientID + "','" + trDistanceToSchool.ClientID + "','" + Me.txtMilesSchool.ClientID + "');")

        Me.VRScript.CreateJSBinding(Me.ddRatedOrExcludedDriver, ctlPageStartupScript.JsEventType.onchange, "if($(this).val() == '3'){alert('Excluded driver accepted but will require Underwriting review prior to issuance.');}")

        ' Task 40520
        If QQHelper.IsQuickQuoteDriverNewToImage(MyDriver, Quote) Then
            Dim disablePolicholder2FromRelationshipDropdownScript = If(HasPolicyholderTwoAvailable = False, "$(""#" + Me.ddRelationToPolicyHolder.ClientID + " option[value='5']"").attr('disabled', 'disabled');", "")
            Me.VRScript.AddScriptLine(disablePolicholder2FromRelationshipDropdownScript)

            Me.VRScript.AddScriptLine("ddRelationships.push('" + Me.ddRelationToPolicyHolder.ClientID + "');$(""#" + Me.ddRelationToPolicyHolder.ClientID + """).change(function(){CheckDriverRelationshipToPolicyHolder();});")

        End If



        If Me.Quote IsNot Nothing Then
            Me.btnCopyFromPh1.Visible = False
            Me.btnCopyFromPh2.Visible = False
            If Me.Quote.Policyholder.Name IsNot Nothing Then
                Me.btnCopyFromPh1.Visible = Me.Quote.Policyholder.Name.FirstName.Trim() <> "" AndAlso Me.Quote.Policyholder.Name.LastName.Trim() <> ""
            End If

            If Me.Quote.Policyholder2.Name IsNot Nothing Then
                Me.btnCopyFromPh2.Visible = Me.Quote.Policyholder2.Name.FirstName.Trim() <> "" AndAlso Me.Quote.Policyholder2.Name.LastName.Trim() <> ""
            End If

            ' for copy from PH buttons
            If Me.Quote IsNot Nothing Then
                If Me.Quote.Policyholder.Name IsNot Nothing Then
                    Dim copyPH1Script As New StringBuilder()
                    copyPH1Script.Append("$(""#" + Me.txtFirstName.ClientID + """).val('" + Me.Quote.Policyholder.Name.FirstName.ToUpper().Replace("'", "\'") + "');")
                    copyPH1Script.Append("$(""#" + Me.txtMiddleName.ClientID + """).val('" + Me.Quote.Policyholder.Name.MiddleName.ToUpper().Replace("'", "\'") + "');")
                    copyPH1Script.Append("$(""#" + Me.txtLastname.ClientID + """).val('" + Me.Quote.Policyholder.Name.LastName.ToUpper().Replace("'", "\'") + "');")
                    copyPH1Script.Append("$(""#" + Me.txtBirthDate.ClientID + """).val('" + Me.Quote.Policyholder.Name.BirthDate + "');")

                    copyPH1Script.Append("$(""#" + Me.ddSex.ClientID + " option[value='" + Me.Quote.Policyholder.Name.SexId + "']"").prop('selected', true);")
                    copyPH1Script.Append("$(""#" + Me.ddSuffix.ClientID + "  option:contains('" + Me.Quote.Policyholder.Name.SuffixName.ToUpper().Replace(".", "") + "')"").each(function(){if( $(this).text() == '" + Me.Quote.Policyholder.Name.SuffixName.ToUpper().Replace(".", "") + "' ){$(this).attr(""selected"",""selected"");}});")
                    copyPH1Script.Append("$(""#" + Me.ddRelationToPolicyHolder.ClientID + " option[value='8']"").prop('selected', true);")

                    ' dldatecalc will not fire if dldate isn't empty to always empty it
                    copyPH1Script.Append("$(""#" + Me.txtDLDate.ClientID + """).val(''); DLDateCalc('" + Me.txtDLDate.ClientID + "','" + Me.txtBirthDate.ClientID + "');")
                    copyPH1Script.Append("ifm.vr.ui.LockTree_Freeze(); ")
                    copyPH1Script.Append("updateDriverHeaderText(""" + Me.lblAccordHeader.ClientID + """,""" + (Me.DriverIndex + 1).ToString() + """,""" + Me.txtFirstName.ClientID + """,""" + Me.txtLastname.ClientID + """,""" + Me.txtMiddleName.ClientID + """," + Me.DriverIndex.ToString() + "); ")
                    copyPH1Script.Append("return false;")
                    Me.btnCopyFromPh1.OnClientClick = copyPH1Script.ToString()
                End If

                If Me.Quote.Policyholder2.Name IsNot Nothing Then
                    Dim copyPH2Script As New StringBuilder()
                    copyPH2Script.Append("$(""#" + Me.txtFirstName.ClientID + """).val('" + Me.Quote.Policyholder2.Name.FirstName.ToUpper().Replace("'", "\'") + "');")
                    copyPH2Script.Append("$(""#" + Me.txtMiddleName.ClientID + """).val('" + Me.Quote.Policyholder2.Name.MiddleName.ToUpper().Replace("'", "\'") + "');")
                    copyPH2Script.Append("$(""#" + Me.txtLastname.ClientID + """).val('" + Me.Quote.Policyholder2.Name.LastName.ToUpper().Replace("'", "\'") + "');")
                    copyPH2Script.Append("$(""#" + Me.txtBirthDate.ClientID + """).val('" + Me.Quote.Policyholder2.Name.BirthDate + "');")

                    copyPH2Script.Append("$(""#" + Me.ddSex.ClientID + " option[value='" + Me.Quote.Policyholder2.Name.SexId + "']"").prop('selected', true);")
                    copyPH2Script.Append("$(""#" + Me.ddSuffix.ClientID + "  option:contains('" + Me.Quote.Policyholder2.Name.SuffixName.ToUpper().Replace(".", "") + "')"").each(function(){if( $(this).text() == '" + Me.Quote.Policyholder2.Name.SuffixName.ToUpper().Replace(".", "") + "' ){$(this).attr(""selected"",""selected"");}});")
                    copyPH2Script.Append("$(""#" + Me.ddRelationToPolicyHolder.ClientID + " option[value='5']"").prop('selected', true);")

                    ' dldatecalc will not fire if dldate isn't empty to always empty it
                    copyPH2Script.Append("$(""#" + Me.txtDLDate.ClientID + """).val(''); DLDateCalc('" + Me.txtDLDate.ClientID + "','" + Me.txtBirthDate.ClientID + "');")
                    copyPH2Script.Append("ifm.vr.ui.LockTree_Freeze(); ")
                    copyPH2Script.Append("updateDriverHeaderText(""" + Me.lblAccordHeader.ClientID + """,""" + (Me.DriverIndex + 1).ToString() + """,""" + Me.txtFirstName.ClientID + """,""" + Me.txtLastname.ClientID + """,""" + Me.txtMiddleName.ClientID + """," + Me.DriverIndex.ToString() + "); ")
                    copyPH2Script.Append("return false;")
                    Me.btnCopyFromPh2.OnClientClick = copyPH2Script.ToString()
                End If

            End If

        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            If MyDriver IsNot Nothing Then
                MyDriver.Name.FirstName = Me.txtFirstName.Text.ToUpper().Trim()
                MyDriver.Name.MiddleName = Me.txtMiddleName.Text.ToUpper().Trim()
                MyDriver.Name.LastName = Me.txtLastname.Text.ToUpper().Trim()

                MyDriver.Name.SuffixName = Me.ddSuffix.Text

                MyDriver.Name.BirthDate = Me.txtBirthDate.Text
                MyDriver.Name.SexId = Me.ddSex.SelectedValue

                MyDriver.Name.MaritalStatusId = Me.ddMaritialStatus.SelectedValue 'straight text no ID
                MyDriver.Name.DriversLicenseNumber = Me.txtDLNumber.Text.Trim().Replace("-", "")
                MyDriver.Name.DriversLicenseStateId = Me.ddDLState.Text 'straight text no ID
                MyDriver.Name.DriversLicenseDate = Me.txtDLDate.Text.Trim()
                MyDriver.RelationshipTypeId = Me.ddRelationToPolicyHolder.SelectedValue
                MyDriver.DriverExcludeTypeId = Me.ddRatedOrExcludedDriver.SelectedValue

                MyDriver.DefDriverDate = Me.txtDefensiveDriverDate.Text
                MyDriver.AccPreventionCourse = Me.txtAccidentPreventionCourse.Text.Trim()

                If Date.TryParse(Me.txtBirthDate.Text, Nothing) Then
                    Dim dob As Date = Date.Parse(Me.txtBirthDate.Text)
                    Dim nowDate As Date = Nothing
                    If Date.TryParse(Me.Quote.EffectiveDate, nowDate) = False Then
                        nowDate = DateTime.Now
                    End If

                    Dim ageInYrs As Integer = nowDate.Year - dob.Year
                    If nowDate.Month < dob.Month _
                     OrElse (nowDate.Month = dob.Month AndAlso nowDate.Day < dob.Day) Then
                        ageInYrs -= 1
                    End If
                    If (ageInYrs < 25) Then
                        'only available to drivers under 24 years of age
                        MyDriver.GoodStudent = Me.chkGoodStudent.Checked
                        MyDriver.DistantStudent = Me.chkDistantStudent.Checked
                        If Me.chkDistantStudent.Checked Then
                            MyDriver.SchoolDistance = Me.txtMilesSchool.Text.Trim()
                        Else
                            MyDriver.SchoolDistance = ""
                        End If


                    Else
                        MyDriver.GoodStudent = False
                        MyDriver.DistantStudent = False
                        MyDriver.SchoolDistance = ""
                        If Me.chkDistantStudent.Checked Or Me.chkGoodStudent.Checked Then
                            Me.chkGoodStudent.Checked = False
                            Me.chkDistantStudent.Checked = False
                        End If
                    End If
                Else
                    'can't tell age so this is not valid
                    MyDriver.GoodStudent = False
                    MyDriver.DistantStudent = False
                    MyDriver.SchoolDistance = ""
                End If

                MyDriver.MotorMembershipDate = Me.txtMotorcycleTrainingDate.Text.Trim
                MyDriver.MotorClub = Me.chkMotorcycleClubMember.Checked
                MyDriver.MotorcycleYearsExperience = Me.txtMotorCycleYearsOfExperience.Text.Trim()
                MyDriver.MotorcycleTrainingDisc = Me.chkMotorcycleDiscount.Checked

                MyDriver.ExtendedNonOwned = Me.chkExtendedNonOwned.Checked
                MyDriver.EnolPrimaryLiability = Me.chkPrimaryGarageLiability.Checked
                MyDriver.EnolGovtBusiness = Me.chkUsedInGov.Checked
                MyDriver.EnolRegularUse = Me.chkFurnishedForUse.Checked
                MyDriver.EnolEmployedByGarage = Me.chkEmployedByGarage.Checked

                MyDriver.EnolNamedInsured = Me.radNamedOnly.Checked
                'driver.EnolNamedInsured = Me.radNamedInd_AndResident.Checked

                MyDriver.EnolMedpayExclude = Me.chkExcludedMedical.Checked

                ' need to tell the violation list to save itself
                'need to tell the loss history list to save itself
                Me.SaveChildControls()

                Return True
            End If
        End If
        Return False
    End Function

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        RaiseEvent DriverIndexRemoving(Me.DriverIndex)
    End Sub

    Protected Sub lnkBtnSav_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click, lnkBtnSaveMotor.Click, lnkBtnSaveNonOwned.Click, lnkBtnSaveGoodStudent.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub btnCopyFromPh1_Click(sender As Object, e As EventArgs) Handles btnCopyFromPh1.Click, btnCopyFromPh2.Click
        ' this should never execute do to client side logic

        If Me.Quote IsNot Nothing Then
            If sender Is btnCopyFromPh1 Then
                If Me.Quote.Policyholder.Name IsNot Nothing Then
                    Me.txtFirstName.Text = Me.Quote.Policyholder.Name.FirstName
                    Me.txtMiddleName.Text = Me.Quote.Policyholder.Name.MiddleName
                    Me.txtLastname.Text = Me.Quote.Policyholder.Name.LastName
                    Me.txtBirthDate.Text = Me.Quote.Policyholder.Name.BirthDate
                    Me.ddSex.SetFromValue(Me.Quote.Policyholder.Name.SexId)
                    Me.ddSuffix.SetFromValue(Me.Quote.Policyholder.Name.SuffixName.Replace(".", ""))
                    Me.ddRelationToPolicyHolder.SetFromValue("8")

                    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

                    Me.VRScript.AddScriptLine("DLDateCalc('" + Me.txtDLDate.ClientID + "','" + Me.txtBirthDate.ClientID + "');")
                End If
            Else
                If Me.Quote.Policyholder2.Name IsNot Nothing Then
                    Me.txtFirstName.Text = Me.Quote.Policyholder2.Name.FirstName
                    Me.txtMiddleName.Text = Me.Quote.Policyholder2.Name.MiddleName
                    Me.txtLastname.Text = Me.Quote.Policyholder2.Name.LastName
                    Me.txtBirthDate.Text = Me.Quote.Policyholder2.Name.BirthDate
                    Me.ddSex.SetFromValue(Me.Quote.Policyholder2.Name.SexId)
                    Me.ddSuffix.SetFromValue(Me.Quote.Policyholder2.Name.SuffixName.Replace(".", ""))
                    Me.ddRelationToPolicyHolder.SetFromValue("5")
                    If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing Then
                        If Me.DriverIndex = 1 Then
                            Try
                                If Me.Quote.Drivers(0).Name.MaritalStatusId = "2" Then
                                    Me.ddMaritialStatus.SetFromValue("2")
                                End If
                            Catch ex As Exception

                            End Try
                        End If
                    End If

                    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

                    Me.VRScript.AddScriptLine("DLDateCalc('" + Me.txtDLDate.ClientID + "','" + Me.txtBirthDate.ClientID + "');")

                End If
            End If
        End If
    End Sub

    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        RaiseEvent NewDriverRequested()
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtAccidentPreventionCourse.Text = ""
        Me.txtBirthDate.Text = ""
        Me.txtDefensiveDriverDate.Text = ""
        Me.txtDLDate.Text = ""
        Me.txtDLNumber.Text = ""
        Me.txtFirstName.Text = ""
        Me.txtLastname.Text = ""
        Me.txtMiddleName.Text = ""
        Me.txtMilesSchool.Text = ""
        Me.txtMotorcycleTrainingDate.Text = ""
        Me.txtMotorCycleYearsOfExperience.Text = ""
        Me.ddDLState.SelectedIndex = -1
        Me.ddMaritialStatus.SelectedIndex = -1
        Me.ddRatedOrExcludedDriver.SelectedIndex = -1
        Me.ddRelationToPolicyHolder.SelectedIndex = -1
        Me.ddSex.SelectedIndex = -1
        Me.ddSuffix.SelectedIndex = -1
        Me.chkDistantStudent.Checked = False
        Me.chkEmployedByGarage.Checked = False
        Me.chkExcludedMedical.Checked = False
        Me.chkExtendedNonOwned.Checked = False
        Me.chkFurnishedForUse.Checked = False
        Me.chkGoodStudent.Checked = False
        Me.chkMotorcycleClubMember.Checked = False
        Me.chkMotorcycleDiscount.Checked = False
        Me.chkPrimaryGarageLiability.Checked = False
        Me.chkUsedInGov.Checked = False
        MyBase.ClearControl()
    End Sub

End Class