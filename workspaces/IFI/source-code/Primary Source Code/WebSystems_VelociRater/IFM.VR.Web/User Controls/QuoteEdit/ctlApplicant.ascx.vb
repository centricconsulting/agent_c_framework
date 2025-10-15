Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions

Public Class ctlApplicant
    Inherits VRControlBase

    Public Property ApplicantIndex As Int32
        Get
            If ViewState("vs_appIndex") Is Nothing Then
                ViewState("vs_appIndex") = -1
            End If
            Return CInt(ViewState("vs_appIndex"))
        End Get
        Set(value As Int32)
            ViewState("vs_appIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyApplicant As QuickQuoteApplicant
        Get
            'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
            'If Me.Quote IsNot Nothing AndAlso Me.Quote.Applicants.HasItemAtIndex(ApplicantIndex) Then
            'If Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.HasItemAtIndex(ApplicantIndex) Then
            '    'Return Me.Quote.Applicants(ApplicantIndex)
            '    Return Me.GoverningStateQuote.Applicants(ApplicantIndex)
            'End If
            'Return Nothing

            If Me.Quote IsNot Nothing Then
                'If Quote.LobType = QuickQuoteLobType.UmbrellaPersonal Then
                '    Dim test = GoverningStateQuote.Applicants
                '    If Quote.Applicants.HasItemAtIndex(ApplicantIndex) Then
                '        Return Me.Quote.Applicants(ApplicantIndex)
                '    End If
                'Else
                '    If Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.HasItemAtIndex(ApplicantIndex) Then
                '        Return Me.GoverningStateQuote.Applicants(ApplicantIndex)
                '    End If
                'End If
                'updated/reverted 10/1/2021 to treat the same for all LOBs (backend can handle Umbrella)
                If Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.HasItemAtIndex(ApplicantIndex) Then
                    Return Me.GoverningStateQuote.Applicants(ApplicantIndex)
                End If
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddStateAbbrev.Items.Count < 1 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddPhoneType, QuickQuoteClassName.QuickQuotePhone, QuickQuotePropertyName.TypeId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            LoadStaticData()
            Dim app As QuickQuoteApplicant = Me.MyApplicant

            'to fix quotes that were saved without top-level applicants 
            'removed 10/1/2021; will treat applicants the same for all LOBs (backend can handle Umbrella)
            'If app Is Nothing Then
            '    Dim gqApp = Me.GoverningStateQuote?.Applicants?.FirstOrDefault()

            '    If gqApp IsNot Nothing Then
            '        Me.Quote.Applicants.Add(gqApp)
            '        app = gqApp
            '    Else
            '        app = Me.Quote.Applicants.AddNew()
            '    End If
            'End If

            If app IsNot Nothing Then '10/1/2021: brought IF statement back to go w/ code removal above
                'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
                'If Me.Quote.Applicants IsNot Nothing Then
                If Me.GoverningStateQuote?.Applicants IsNot Nothing Then
                    'Me.lnkAdd.Visible = Me.Quote.Applicants.Count < 2
                    Me.lnkAdd.Visible = Me.GoverningStateQuote.Applicants.Count < 2
                End If

                If IsQuoteEndorsement() Then
                    Me.lnkRemove.Visible = False
                End If

                ''TODO: remove references to Me.MyApplicant in following code, use app instead
                If app.Name IsNot Nothing Then
                    Me.lblInsuredTitle.Text = "Applicant #{4} - {0} {1} {2} {3}".FormatIFM(app.Name.FirstName, app.Name.MiddleName, app.Name.LastName, app.Name.SuffixName, (Me.ApplicantIndex + 1).ToString()).ToUpper().Replace("  ", " ")

                    Me.lblInsuredTitle.Text = Me.lblInsuredTitle.Text.Ellipsis(60)

                    Me.txtFirstName.Text = app.Name.FirstName
                    Me.txtMiddleName.Text = app.Name.MiddleName
                    Me.txtLastName.Text = app.Name.LastName

                    'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddSuffix, app.Name.SuffixName)
                    'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddSex, app.Name.SexId)

                    Me.txtSSN.Text = app.Name.TaxNumber_Hyphens.RemoveAny("000-00-0000", "000000000")

                    If app.Name.BirthDate.IsDate Then
                        Me.txtBirthDate.Text = RemovePossibleDefaultedDateValue(app.Name.BirthDate)
                    Else
                        ' just show whatever was there
                    End If

                    If app.Emails.IsLoaded Then
                        Me.txtEmail.Text = app.Emails(0).Address
                    End If

                    If app.Phones.IsLoaded Then
                        Me.txtPhone.Text = app.Phones(0).Number
                    End If

                    If app.Phones.IsLoaded Then
                        Me.txtPhoneExt.Text = app.Phones(0).Extension.ReturnEmptyIfEqualsAny("0")
                        SetdropDownFromValue(Me.ddPhoneType, app.Phones(0).TypeId)
                    End If




                    ' Address Info is always pulled from Ph#1
                    If Me.MyApplicant.Address.IsNotNull Then
                        Me.txtStreetNum.Text = Me.MyApplicant.Address.HouseNum
                        Me.txtStreetName.Text = Me.MyApplicant.Address.StreetName
                        Me.txtAptNum.Text = Me.MyApplicant.Address.ApartmentNumber
                        Me.txtPOBox.Text = Me.MyApplicant.Address.POBox
                        Me.txtCityName.Text = Me.MyApplicant.Address.City
                        If Me.MyApplicant.Address.StateId.EqualsAny("", "0") Then
                            SetdropDownFromValue(Me.ddStateAbbrev, "16")
                        Else
                            SetdropDownFromValue(Me.ddStateAbbrev, Me.MyApplicant.Address.StateId)
                        End If



                        If Me.MyApplicant.Address.Zip.RemoveAny("00000", "-0000").IsNullEmptyorWhitespace Then
                            Me.txtZipCode.Text = ""
                        Else
                            Me.txtZipCode.Text = Me.MyApplicant.Address.Zip
                        End If

                        Me.txtGaragedCounty.Text = Me.MyApplicant.Address.County
                    End If
                End If
            End If
        End If

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Applicant #{0}", (Me.ApplicantIndex + 1).ToString())

        Dim myaccordClientId As String = If(Me.ParentVrControl IsNot Nothing, Me.ParentVrControl.ListAccordionDivId, "")
        Dim paneIndex As Int32 = Me.ApplicantIndex

        'Endorsements - Diamond returns the applicant with the commercial name.  This causes a Comm/personal name false positive.
        Dim applicantInfo = QQHelper.CloneObject(Me.MyApplicant)
        If IsQuoteEndorsement() Then
            applicantInfo.Name.CommercialName1 = String.Empty
        End If

        Dim valItems = ApplicantValidator.ValidateApplicant(applicantInfo, valArgs.ValidationType)
        If valItems.Any() Then

            For Each v In valItems
                Select Case v.FieldId
                    Case ApplicantValidator.CommAndPersNameComponentsEmpty
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, myaccordClientId, paneIndex)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.CommAndPersNameComponentsAllSet
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, myaccordClientId, paneIndex)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantFirstNameID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantLastNameID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantGenderID
                        'Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddSex, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantSSNID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSSN, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantBirthDate
                        If Not IsQuoteEndorsement() Then 'CAH 10/8/2020 Task51953
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBirthDate, v, myaccordClientId, paneIndex)
                        End If
                    Case ApplicantValidator.ApplicantEmail
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEmail, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantPhoneNumber
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPhone, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantPhoneExtension
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPhoneExt, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantPhoneType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddPhoneType, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantStreetAndPoBoxEmpty
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, myaccordClientId, paneIndex)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, myaccordClientId, paneIndex)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBox, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantStreetAndPoxBoxAreSet
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, myaccordClientId, paneIndex)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, myaccordClientId, paneIndex)
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBox, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantHouseNumberID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantStreetNameID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantPOBOXID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBox, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantZipCodeID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantCityID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, myaccordClientId, paneIndex)
                    Case ApplicantValidator.ApplicantStateID
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, myaccordClientId, paneIndex)

                    Case ApplicantValidator.ApplicantCountyID ' don't care if county is empty
                        'Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedCounty, v, myaccordClientId, paneIndex)

                        'Case Else
                        '    If v.IsWarning Then
                        '        Me.ValidationHelper.AddWarning(v.Message)
                        '    Else
                        '        Me.ValidationHelper.AddError(v.Message)
                        '    End If

                End Select

            Next
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkAdd.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove Applicant?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        Me.VRScript.CreateTextboxMask(Me.txtSSN, "000-00-0000")
        Me.VRScript.CreateTextboxWaterMark(Me.txtBirthDate, "MM/DD/YYYY")
        Me.VRScript.CreateTextboxWaterMark(Me.txtEmail, "User@Site.com")
        Me.VRScript.CreateTextboxMask(Me.txtPhone, "(000)000-0000")
        Me.VRScript.CreateTextboxWaterMark(Me.txtPhone, "(000)000-0000")

        Me.VRScript.AddScriptLine("$(""#" + Me.txtCityName.ClientID + """).autocomplete({ source: INCities });")
        Me.VRScript.AddScriptLine("$(""#" + Me.txtGaragedCounty.ClientID + """).autocomplete({ source: indiana_Counties });")

        Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();")
        Me.VRScript.CreateJSBinding(Me.ddCityName, ctlPageStartupScript.JsEventType.onchange, "if($(this).val() == '0'){$(""#" + Me.txtCityName.ClientID + """).show(); } else {$(""#" + Me.txtCityName.ClientID + """).val($(this).val()); $(""#" + Me.txtCityName.ClientID + """).hide();}")

        Me.VRScript.CreateTextBoxFormatter(Me.txtBirthDate, ctlPageStartupScript.FormatterType.DateFormat, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtGaragedCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
        Me.VRScript.CreateTextBoxFormatter(Me.txtGaragedCounty, ctlPageStartupScript.FormatterType.AlphabeticOnly, ctlPageStartupScript.JsEventType.onkeyup)


        ' do this on focus
        Dim addresswarningScript As String = "DoAddressWarning(true,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreetName.ClientID + "','" + txtPOBox.ClientID + "');"
        Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtPOBox}, ctlPageStartupScript.JsEventType.onfocus, addresswarningScript)

        'do this onblur
        Dim addresswarningScriptOff As String = "DoAddressWarning(false,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreetName.ClientID + "','" + txtPOBox.ClientID + "');"
        Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtPOBox}, ctlPageStartupScript.JsEventType.onblur, addresswarningScriptOff)

        Me.VRScript.CreateTextBoxFormatter(Me.txtZipCode, ctlPageStartupScript.FormatterType.ZipCode, ctlPageStartupScript.JsEventType.onblur)

        Dim bindings As New StringBuilder()
        bindings.Append("applicantUiBindings.push(new ApplicantBinding(")
        bindings.Append(String.Format("{0}", Me.ApplicantIndex.ToString()))
        bindings.Append(String.Format(",'{0}'", Me.lblInsuredTitle.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtFirstName.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtMiddleName.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtLastName.ClientID))
        bindings.Append(String.Format(",'{0}'", "")) 'bindings.Append(String.Format(",'{0}'", Me.ddSuffix.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtSSN.ClientID))
        bindings.Append(String.Format(",'{0}'", "")) 'bindings.Append(String.Format(",'{0}'", Me.ddSex.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtBirthDate.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtStreetNum.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtStreetName.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtAptNum.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtPOBox.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtCityName.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.ddStateAbbrev.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtZipCode.ClientID))
        bindings.Append(String.Format(",'{0}'", Me.txtGaragedCounty.ClientID))
        bindings.Append("));")

        Me.VRScript.AddVariableLine(bindings.ToString())

        Dim scriptHeaderUpdate As String = String.Format("ApplicantSearch.updateApplicantHeaderText({0});", Me.ApplicantIndex)
        Dim clientSearchScript As String = "ApplicantSearch.doApplicantSearch($(this).attr('id'),e.keyCode," + Me.ApplicantIndex.ToString() + ");"

        Me.VRScript.CreateJSBinding({Me.txtFirstName, Me.txtLastName}, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript + scriptHeaderUpdate)
        Me.VRScript.CreateJSBinding(Me.txtMiddleName, ctlPageStartupScript.JsEventType.onkeyup, scriptHeaderUpdate)
        Me.VRScript.CreateJSBinding({Me.txtSSN, Me.txtCityName, Me.txtZipCode}, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript)


    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then

            'If Me.Quote.LobType = QuickQuoteLobType.Farm Then
            'updated 10/1/2021 for Umbrella
            If Me.Quote.LobType = QuickQuoteLobType.Farm OrElse Me.Quote.LobType = QuickQuoteLobType.UmbrellaPersonal Then
                If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing Then
                    If Me.Quote.Policyholder.Name.TypeId <> "2" Then
                        Return False ' do not save
                    End If
                End If
            End If
            'Dim isDifferent As Boolean = False

            Dim app As QuickQuoteApplicant = Me.MyApplicant

            If app IsNot Nothing Then 'added IF 10/1/2021
                If app.Name Is Nothing Then
                    app.Name = New QuickQuoteName()
                End If
                If Me.ApplicantIndex = 0 Then
                    app.RelationshipTypeId = "8"
                Else
                    app.RelationshipTypeId = "5"
                End If

                app.Name.FirstName = Me.txtFirstName.Text.ToUpper().Trim()
                app.Name.MiddleName = Me.txtMiddleName.Text.ToUpper().Trim()
                app.Name.LastName = Me.txtLastName.Text.ToUpper().Trim()
                'app.Name.SuffixName = Me.ddSuffix.SelectedValue.ToUpper().Trim()

                'app.Name.SexId = Me.ddSex.SelectedValue
                app.Name.TaxNumber = Me.txtSSN.Text.ToUpper().Trim()
                app.Name.BirthDate = Me.txtBirthDate.Text

                If String.IsNullOrWhiteSpace(Me.txtEmail.Text) = False Then
                    If app.Emails Is Nothing OrElse app.Emails.Count() = 0 Then
                        app.Emails = New List(Of QuickQuoteEmail)()
                        app.Emails.Add(New QuickQuoteEmail())
                    End If

                    app.Emails(0).Address = Me.txtEmail.Text.ToUpper().Trim()
                    app.Emails(0).TypeId = "1"
                Else
                    app.Emails = Nothing
                End If

                If String.IsNullOrWhiteSpace(Me.txtPhone.Text) And String.IsNullOrWhiteSpace(Me.txtPhoneExt.Text) And Me.ddPhoneType.SelectedIndex = 0 Then
                    app.Phones = Nothing
                Else
                    If app.Phones Is Nothing OrElse app.Phones.Count() = 0 Then
                        app.Phones = New List(Of QuickQuotePhone)()
                        app.Phones.Add(New QuickQuotePhone())
                    End If

                    app.Phones(0).Number = Me.txtPhone.Text.ToUpper().Trim()

                    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtPhoneExt.Text.ToUpper()) Then
                        app.Phones(0).Extension = Me.txtPhoneExt.Text.ToUpper().Trim()
                    Else
                        If IFM.Common.InputValidation.CommonValidations.IsNonNegativeWholeNumber(Me.txtPhoneExt.Text) Then
                            app.Phones(0).Extension = If(Int32.TryParse(Me.txtPhoneExt.Text, Nothing), Math.Abs(CInt(Me.txtPhoneExt.Text)).ToString(), "")
                        Else
                            '  it must be a number or it will bomb out 6-5-14
                            app.Phones(0).Extension = ""
                        End If
                    End If

                    app.Phones(0).TypeId = Me.ddPhoneType.SelectedValue
                End If

                If app.Address Is Nothing Then
                    app.Address = New QuickQuoteAddress()
                End If

                If Quote.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote Then
                    Dim hasChanges As Boolean = False
                    SetValueIfDifferent(app.Address.HouseNum, Me.txtStreetNum.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.StreetName, Me.txtStreetName.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.ApartmentNumber, Me.txtAptNum.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.POBox, Me.txtPOBox.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.City, Me.txtCityName.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.StateId, Me.ddStateAbbrev.SelectedValue, HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.Zip, Me.txtZipCode.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    SetValueIfDifferent(app.Address.County, Me.txtGaragedCounty.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                    app.Address.IsChanged = hasChanges
                Else
                    app.Address.HouseNum = Me.txtStreetNum.Text.ToUpper().Trim()
                    app.Address.StreetName = Me.txtStreetName.Text.ToUpper().Trim()
                    app.Address.ApartmentNumber = Me.txtAptNum.Text.ToUpper().Trim()
                    app.Address.POBox = Me.txtPOBox.Text.ToUpper().Trim()
                    app.Address.City = Me.txtCityName.Text.ToUpper().Trim()
                    app.Address.StateId = Me.ddStateAbbrev.SelectedValue
                    app.Address.Zip = Me.txtZipCode.Text.ToUpper().Trim()
                    app.Address.County = Me.txtGaragedCounty.Text.ToUpper().Trim()
                    'app.Address.IsNew = True 'Could add this here but current code should already be assuming this is the case. Leaving here in case we ever need to start using it for some reason
                End If

                Return True
            End If
        End If

        Return False
    End Function

    Public Overrides Sub ClearControl()
        Me.txtAptNum.Text = ""
        Me.txtBirthDate.Text = ""
        Me.txtCityName.Text = ""
        Me.txtEmail.Text = ""
        Me.txtFirstName.Text = ""
        Me.txtGaragedCounty.Text = ""
        Me.txtLastName.Text = ""
        Me.txtMiddleName.Text = ""
        Me.txtPhone.Text = ""
        Me.txtPhoneExt.Text = ""
        Me.txtPOBox.Text = ""
        Me.txtSSN.Text = ""
        Me.txtStreetName.Text = ""
        Me.txtStreetNum.Text = ""
        Me.txtZipCode.Text = ""
        Me.ddCityName.SelectedIndex = -1
        Me.ddPhoneType.SelectedIndex = -1
        'Me.ddSex.SelectedIndex = -1
        Me.ddStateAbbrev.SelectedIndex = -1
        'Me.ddSuffix.SelectedIndex = -1
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
        'save current state
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
        ' remove
        'If Me.Quote.Applicants.Count > 1 Then
        If Me.GoverningStateQuote.Applicants.Count > 1 Then ' don't let them remove the last applicant as it is required. just clear if it is the last one
            'Me.Quote.Applicants.RemoveAt(Me.ApplicantIndex)
            Me.GoverningStateQuote.Applicants.RemoveAt(Me.ApplicantIndex)
        End If
        'repopulate
        Me.ParentVrControl.Populate()
        ' save at current state
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        'force edit mode so they have to save at some point before leaving
        Me.Save_FireSaveEvent(False)
        Me.LockTree()
        If Me.Quote IsNot Nothing Then
            'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
            'Me.Quote.Applicants.AddNew()
            Me.GoverningStateQuote.Applicants.AddNew()
        End If
        Me.ParentVrControl.Populate()
        Me.Save_FireSaveEvent(False)
        'Me.FindFirstVRParentOfType(Of ctlApplicantList)().ActiveApplicantPane = (Me.Quote.Applicants.Count - 1).ToString()
        Me.FindFirstVRParentOfType(Of ctlApplicantList)().ActiveApplicantPane = (Me.GoverningStateQuote.Applicants.Count - 1).ToString()
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(Me.DefaultValidationType)))
    End Sub
End Class