Imports QuickQuote.CommonObjects
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports IFM.VR.Web.Helpers

Public Class ctlInsured
    Inherits VRControlBase

    Public Event PolicyHolderCleared()

    Public Property IsPolicyHolderNum1 As Boolean
        Get
            Return ViewState.GetBool("vs_IsPolHoldNum1")
        End Get
        Set(value As Boolean)
            ViewState("vs_IsPolHoldNum1") = value
        End Set
    End Property

    Public Property showCareOf As Boolean
        Get
            Return ViewState.GetBool("vs_showCareOf")
        End Get
        Set(value As Boolean)
            ViewState("vs_showCareOf") = value
        End Set
    End Property

    Public ReadOnly Property PhoneTypeID() As String
        Get
            Return Me.ddPhoneType.ClientID
        End Get
    End Property

    Dim helper As New CommonHelperClass()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ValidationHelper.GroupName = "Policyholder #{0}".FormatIFM(If(Me.IsPolicyHolderNum1, 1, 2))
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddStateAbbrev.Items.Count < 1 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddSuffix, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.SuffixName, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddSex, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.SexId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddPhoneType, QuickQuoteClassName.QuickQuotePhone, QuickQuotePropertyName.TypeId, SortBy.None, Me.Quote.LobType)

            'Add Business TYpe
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddBusinessType, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.EntityTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

            If Me.Quote IsNot Nothing Then
                If GoverningStateQuote.ProgramTypeId IsNot "5" AndAlso GoverningStateQuote.UmbrellaSelfInsuredRetentionLimitId IsNot "286" AndAlso GoverningStateQuote.LobType = QuickQuoteLobType.UmbrellaPersonal Then
                    ddBusinessType.Items.Remove(New ListItem("Individual", "1"))
                End If
            End If
            ' CAH 11/01/2017 - No need for 2 Pulls; Static Data changed to use LobType in the above statement
            'If Me.Quote IsNot Nothing Then
            '    Dim atts As New List(Of QuickQuoteStaticDataAttribute)
            '    Dim att As New QuickQuoteStaticDataAttribute
            '    With att
            '        .nvp_propertyName = QuickQuotePropertyName.LobId
            '        .nvp_value = Me.Quote.LobId
            '    End With
            '    atts.Add(att)
            '    QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddBusinessType, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.EntityTypeId, atts, SortBy.None, Me.Quote.LobType)
            'End If

        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
            If IsQuoteEndorsement() = False _
                AndAlso (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty _
                OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP) Then
                Me.divPrefillInfoText.Visible = True
            Else
                Me.divPrefillInfoText.Visible = False
            End If
            If AllowPopulate() Then
                LoadStaticData()
                Dim ph As QuickQuotePolicyholder = Nothing
                Me.liBusinessStarted.Attributes.Add("style", "display:none")
                Me.liYearsExperience.Attributes.Add("style", "display:none")

                Me.trCopyFromDriver.Visible = False '7-20-14 Driver Copy
                If Me.IsPolicyHolderNum1 Then
                    ' use policyholder #1
                    ph = Quote.Policyholder
                    Me.tblAddressInfo.Visible = True
                Else
                    ' use policyholder #2
                    ph = Quote.Policyholder2
                End If

                Me.divPersName.Visible = False
                Me.divCommName.Visible = False

                'B28756 CAH
                Select Case Me.Quote.LobType
                    Case QuickQuoteLobType.Farm
                        helper.AddCSSClassToControl(dbaNameFields, "hide")
                        Exit Select 'why?
                    Case QuickQuoteLobType.UmbrellaPersonal
                        helper.AddCSSClassToControl(dbaNameFields, "hide")
                        helper.AddCSSClassToControl(liTaxIdType, "hide")
                        helper.AddCSSClassToControl(liBusinessStarted, "hide")
                        helper.AddCSSClassToControl(liDescriptionOfOperations, "hide")
                        Exit Select 'why?
                    Case Else
                        helper.RemoveCSSClassFromControl(dbaNameFields, "hide")
                End Select

                'Updated 9/4/18 for multi state MLW
                'If ph.Name.IsNotNull Then
                If ph.Name IsNot Nothing Then
                    If ph.Name.TypeId <> "2" Then
                        'Personal Name
                        Me.divPersName.Visible = True
                        Me.lblInsuredTitle.Text = "Policyholder #{4} - {0} {1} {2} {3}".FormatIFM(ph.Name.FirstName, ph.Name.MiddleName, ph.Name.LastName, ph.Name.SuffixName, If(IsPolicyHolderNum1, "1", "2")).ToUpper().Replace("  ", " ")

                        Me.lblInsuredTitle.Text = Me.lblInsuredTitle.Text.Ellipsis(55)

                        Me.txtFirstName.Text = ph.Name.FirstName
                        Me.txtMiddleName.Text = ph.Name.MiddleName
                        Me.txtLastName.Text = ph.Name.LastName

                        'SetdropDownFromValue(Me.ddSuffix, ph.Name.SuffixName.Replace(".", ""))
                        SetdropDownFromValue_ForceSeletion(Me.ddSuffix, ph.Name.SuffixName, ph.Name.SuffixName) 'CAH B41922
                        SetdropDownFromValue(Me.ddSex, ph.Name.SexId)

                        If ph.Name.TaxNumber_Hyphens.RemoveAny("000-00-0000", "000000000").IsNullEmptyorWhitespace Then
                            Me.txtSSN.Text = "" ' was either empty or 000-00-0000 so just show empty
                        Else
                            Me.txtSSN.Text = ph.Name.TaxNumber_Hyphens
                        End If

                        If ph.Name.BirthDate.IsDate Then
                            Me.txtBirthDate.Text = RemovePossibleDefaultedDateValue(ph.Name.BirthDate)
                        Else
                            ' just show whatever was there
                        End If
                    Else
                        'Commercial Name
                        Me.divCommName.Visible = True
                        Me.lblInsuredTitle.Text = "Policyholder #{1} - {0}".FormatIFM(ph.Name.CommercialName1, If(IsPolicyHolderNum1, "1", "2")).ToUpper().Replace("  ", " ")

                        Me.lblInsuredTitle.Text = Me.lblInsuredTitle.Text.Ellipsis(55)
                        Me.txtBusinessName.Text = ph.Name.CommercialName1
                        Me.txtDBA.Text = ph.Name.DoingBusinessAsName
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso IFM.VR.Common.Helpers.FARM.IndividualLegalEntityHelper.IsIndividualLegalEntityAvailable(Quote) = False Then
                            Dim removeIndividual As ListItem = ddBusinessType.Items.FindByValue("1")
                            If removeIndividual IsNot Nothing Then
                                Me.ddBusinessType.Items.Remove(removeIndividual)
                            End If
                        End If
                        'SetdropDownFromValue(Me.ddBusinessType, ph.Name.EntityTypeId)
                        SetDropDownValue_ForceDiamondValue(Me.ddBusinessType, ph.Name.EntityTypeId, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.EntityTypeId)
                        'Added 2/14/2022 for bug 63511 MLW
                        If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63511_PolicyholderOtherEntityType")) = True Then
                            Select Case Me.Quote.LobType
                                Case QuickQuoteLobType.CommercialAuto,
                                     QuickQuoteLobType.CommercialBOP,
                                     QuickQuoteLobType.CommercialGeneralLiability,
                                     QuickQuoteLobType.CommercialPackage,
                                     QuickQuoteLobType.CommercialProperty,
                                     QuickQuoteLobType.WorkersCompensation
                                    If ph.Name.EntityTypeId <> String.Empty AndAlso QQHelper.IsPositiveIntegerString(ph.Name.EntityTypeId) Then
                                        If ddBusinessType.SelectedValue = "5" Then 'Other = 5
                                            liOtherEntity.Attributes.Add("style", "display:''")
                                            txtOtherEntity.Text = ph.Name.OtherLegalEntityDescription
                                        Else
                                            liOtherEntity.Attributes.Add("style", "display:none")
                                            txtOtherEntity.Text = ""
                                        End If
                                    Else 'added to handle for re-populate after prefill re-order where this was previously there from original order but should now be hidden due to NoHit
                                        liOtherEntity.Attributes.Add("style", "display:none")
                                        txtOtherEntity.Text = ""
                                    End If
                                    If IsQuoteEndorsement() Then
                                        txtOtherEntity.Enabled = False
                                    End If
                            End Select
                        End If
                        If ph.Name.TaxTypeId <> String.Empty AndAlso IsNumeric(ph.Name.TaxTypeId) AndAlso (CInt(ph.Name.TaxTypeId) > 0) Then
                            ddTaxIDType.SelectedValue = ph.Name.TaxTypeId
                            If ddTaxIDType.SelectedIndex >= 1 Then
                                If ddTaxIDType.SelectedValue = "1" Then
                                    liSSNBusiness.Attributes.Add("style", "display:''")
                                    Me.txtSSNBusiness.Text = ph.Name.TaxNumber_Hyphens
                                Else
                                    liFEIN.Attributes.Add("style", "display:''")
                                    Me.txtFEIN.Text = ph.Name.TaxNumber_Hyphens
                                End If
                            End If
                        Else
                            ddTaxIDType.SelectedIndex = 0
                        End If

                        ' Don't show these fields for commercial farm!
                        Me.liBusinessStarted.Attributes.Add("style", "display:none")
                        liYearsExperience.Attributes.Add("style", "display:none")
                        Me.liDescriptionOfOperations.Attributes.Add("style", "display:none")

                        If Quote.LobType <> QuickQuoteLobType.Farm Then
                            Me.liBusinessStarted.Attributes.Add("style", "display:''")
                            Dim bsdt As String = FormatDate(ph.Name.DateBusinessStarted)
                            'Added 12/30/2020 for CAP Endorsements Task 52971 MLW
                            If IsQuoteEndorsement() Then
                                If bsdt = "01/01/1800" Then
                                    bsdt = ""
                                End If
                            End If
                            Me.txtBusinessStarted.Text = bsdt
                            'Updated 9/20/2022 for bug 67839 MLW
                            'If DateBusinessStartedLessThan3YearsAgo(txtBusinessStarted.Text) Then
                            If DateBusinessStartedLessThan3YearsAgo(txtBusinessStarted.Text) AndAlso DateBusinessStartedNotInFuture(txtBusinessStarted.Text) Then
                                liYearsExperience.Attributes.Add("style", "display:''")
                                Me.txtYearsOfExperience.Text = ph.Name.YearsOfExperience.Trim()
                            Else
                                liYearsExperience.Attributes.Add("style", "display:none")
                                Me.txtYearsOfExperience.Text = ""
                            End If
                            Me.liDescriptionOfOperations.Attributes.Add("style", "display:''")
                            Me.txtDescriptionOfOperations.Text = ph.Name.DescriptionOfOperations
                        End If

                        'Added 11/16/2020 for CAP Endorsements task 52971 MLW
                        If Me.IsQuoteEndorsement() Then
                            Select Case Me.Quote.LobType
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                                    Me.txtBusinessName.Enabled = False
                                    Me.txtDBA.Enabled = False
                                    Me.ddBusinessType.Enabled = False
                                    Me.txtDescriptionOfOperations.Enabled = False
                                    Me.ddTaxIDType.Enabled = False
                                    Me.txtFEIN.Enabled = False
                                    Me.txtSSNBusiness.Enabled = False
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                                    Me.txtBusinessName.Enabled = False
                                    Me.txtDBA.Enabled = False
                                    Me.ddBusinessType.Enabled = False
                                    Me.txtDescriptionOfOperations.Enabled = False
                                    Me.ddTaxIDType.Enabled = False
                                    Me.txtFEIN.Enabled = False
                                    Me.txtSSNBusiness.Enabled = False
                                    Me.txtBusinessStarted.Enabled = False
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                    Me.txtBusinessName.Enabled = False
                                    Me.txtDBA.Enabled = False
                                    Me.ddBusinessType.Enabled = False
                                    Me.txtDescriptionOfOperations.Enabled = False
                                    Me.ddTaxIDType.Enabled = False
                                    Me.txtFEIN.Enabled = False
                                    Me.txtSSNBusiness.Enabled = False
                                    Me.txtBusinessStarted.Enabled = False
                                Case Else
                                    'show all
                            End Select


                        End If
                    End If
                End If

                If ph.Emails.IsLoaded() Then
                    Me.txtEmail.Text = ph.Emails(0).Address
                    Me.hdnEmailTypeId.Value = ph.Emails(0).TypeId 'added 10/7/2017
                End If

                If ph.Phones.IsLoaded() Then
                    Me.txtPhone.Text = ph.Phones(0).Number
                End If

                If ph.Phones.IsLoaded() Then
                    If ph.Phones(0).Extension.Trim() <> "0" Then
                        Me.txtPhoneExt.Text = ph.Phones(0).Extension
                    Else
                        Me.txtPhoneExt.Text = ""
                    End If

                    SetdropDownFromValue(Me.ddPhoneType, ph.Phones(0).TypeId)
                End If

                ' Address Info is always pulled from Ph#1
                'Updated 9/4/18 for multi state MLW
                'If Me.Quote.Policyholder.Address.IsNotNull Then
                If Me.Quote.Policyholder.Address IsNot Nothing Then
                    Me.txtStreetNum.Text = Quote.Policyholder.Address.HouseNum
                    Me.txtStreetName.Text = Quote.Policyholder.Address.StreetName
                    Me.txtAptNum.Text = Quote.Policyholder.Address.ApartmentNumber
                    Me.txtPOBox.Text = Quote.Policyholder.Address.POBox
                    Me.txtCityName.Text = Quote.Policyholder.Address.City
                    If Quote.Policyholder.Address.StateId.EqualsAny("", "0") Then
                        SetdropDownFromValue(Me.ddStateAbbrev, "16")
                    Else
                        SetdropDownFromValue(Me.ddStateAbbrev, Quote.Policyholder.Address.StateId)
                    End If


                    Me.txtZipCode.Text = Quote.Policyholder.Address.Zip.RemoveAny("00000", "-0000")

                    Me.txtGaragedCounty.Text = Quote.Policyholder.Address.County


                    If String.IsNullOrWhiteSpace(Quote.Policyholder.Address.Other) Then
                        Me.OtherSection.Visible = False
                        Me.OtherPrefixSection.Visible = True
                        showCareOf = True
                    Else
                        Dim AddressOtherField = New AddressOtherField(Quote.Policyholder.Address.Other)
                        If AddressOtherField.PrefixType = Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Other Then
                            Me.OtherSection.Visible = True
                            Me.OtherPrefixSection.Visible = False
                            showCareOf = False
                        Else
                            Me.OtherSection.Visible = False
                            Me.OtherPrefixSection.Visible = True
                            Me.OtherPrefix.SelectedValue = AddressOtherField.PrefixType
                            Me.txtPrefixCO.Text = AddressOtherField.NameWithoutPrefix
                            showCareOf = True
                        End If

                    End If
                End If

                ' Address Info is always pulled from Ph#1
                Me.txtStreetNum.Enabled = Me.IsPolicyHolderNum1
                Me.txtStreetName.Enabled = Me.IsPolicyHolderNum1
                Me.txtAptNum.Enabled = Me.IsPolicyHolderNum1
                Me.txtCO.Enabled = Me.IsPolicyHolderNum1
                Me.txtPOBox.Enabled = Me.IsPolicyHolderNum1
                Me.txtCityName.Enabled = Me.IsPolicyHolderNum1
                Me.ddStateAbbrev.Enabled = Me.IsPolicyHolderNum1
                Me.txtZipCode.Enabled = Me.IsPolicyHolderNum1
                Me.txtGaragedCounty.Enabled = Me.IsPolicyHolderNum1


                If IsOnAppPage Then
                    Me.lblEmail.Text = "*Email:"
                    If IFM.VR.Common.Helpers.AllLines.RequiredEmailHelper.IsRequiredEmailAvailable(Quote) = True Then
                        Me.lblPhone.Text = "Phone:"
                    Else
                        Me.lblPhone.Text = "*Phone:"
                    End If

                    Me.lnkRemove.Visible = False
                    Me.trCopyFromDriver.Visible = False
                    Me.divAddressMessage.Visible = False
                    Me.ddCityName.Visible = False

                    If ph.Name.TypeId <> "2" Then
                        'Personal Name
                    Else
                        'comm
                    End If

                End If

                'No "Clear" on Endorsements - CAH Bug 36053
                If IsQuoteEndorsement() Then
                    lnkRemove.Visible = False
                End If
            End If
        End If
    End Sub

    'Added 9/20/2022 for bug 67839 MLW
    Private Function DateBusinessStartedNotInFuture(DateBusinessStarted As String) As Boolean
        If IsDate(DateBusinessStarted) Then
            Dim dbs As Date = CDate(DateBusinessStarted)
            Dim today As Date = DateTime.Now

            If dbs <= today Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Private Function FormatDate(ByVal dt As String) As String
        If Not IsDate(dt) Then Return dt
        Dim myDate As DateTime = CDate(dt)
        Dim newDt As String = myDate.Month.ToString.PadLeft(2, "0"c) & "/" & myDate.Day.ToString.PadLeft(2, "0"c) & "/" & myDate.Year.ToString
        Return newDt
    End Function

    Private Function DateBusinessStartedLessThan3YearsAgo(ByVal DateBusinessStarted As String) As Boolean
        If IsDate(DateBusinessStarted) Then
            Dim dbs As Date = CDate(DateBusinessStarted)
            Dim threeyearsago As Date = DateTime.Now.AddYears(-3)

            If dbs >= threeyearsago Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
        If AllowValidateAndSave() Then
            MyBase.ValidateControl(valArgs)

            Dim myaccordClientId As String = If(Me.ParentVrControl IsNot Nothing, Me.ParentVrControl.ListAccordionDivId, "")
            Dim paneIndex As Int32 = If(Me.IsPolicyHolderNum1, 0, 1)

            Dim ph As QuickQuotePolicyholder = If(Me.IsPolicyHolderNum1, Me.Quote.Policyholder, Me.Quote.Policyholder2)

            Dim valItems = InsuredValidator.ValidateInsured(If(Me.IsPolicyHolderNum1, 0, 1), Me.Quote, valArgs.ValidationType)
            If valItems.Any() Then

                For Each v In valItems
                    Select Case v.FieldId

                        Case InsuredValidator.PolicyHolderBusinessStartedDate ' Matt A - 12/21/2015
                            If ph.Name.TypeId = "2" Then
                                If Me.Quote.LobType = QuickQuoteLobType.CommercialGeneralLiability Or Me.Quote.LobType = QuickQuoteLobType.CommercialBOP Or Me.Quote.LobType = QuickQuoteLobType.CommercialAuto Or Quote.LobType = QuickQuoteLobType.WorkersCompensation Or Quote.LobType = QuickQuoteLobType.CommercialProperty Or Quote.LobType = QuickQuoteLobType.CommercialPackage Then
                                    'Updated 9/20/2022 for task 67839 MLW
                                    If IsQuoteEndorsement() = False Then
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessStarted, v, myaccordClientId, paneIndex)
                                    End If
                                    ''Updated 12/30/2020 for CAP Endorsements Task 52971 MLW
                                    'If Me.Quote.LobType <> QuickQuoteLobType.CommercialAuto AndAlso Not IsQuoteEndorsement() Then
                                    '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessStarted, v, myaccordClientId, paneIndex)
                                    'End If
                                    ''Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessStarted, v, myaccordClientId, paneIndex)
                                End If
                            End If

                        Case InsuredValidator.PolicyHolderYearsOfExperience ' Matt A - 12/21/2015
                            If ph.Name.TypeId = "2" Then
                                If Me.Quote.LobType = QuickQuoteLobType.CommercialGeneralLiability Or Me.Quote.LobType = QuickQuoteLobType.CommercialBOP Or Quote.LobType = QuickQuoteLobType.CommercialAuto Or Quote.LobType = QuickQuoteLobType.WorkersCompensation Or Quote.LobType = QuickQuoteLobType.CommercialProperty Or Quote.LobType = QuickQuoteLobType.CommercialPackage Then
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtYearsOfExperience, v, myaccordClientId, paneIndex)
                                End If
                            End If

                        Case InsuredValidator.CommercialName
                            Try
                                v.Message = v.Message.Split(" "c)(0) + " Business Name"
                            Catch ex As Exception
#If DEBUG Then
                                Debugger.Break()
#End If
                            End Try
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessName, v, myaccordClientId, paneIndex)

                        Case InsuredValidator.CommAndPersNameComponentsEmpty
                            'Updated 9/4/18 for multi state MLW
                            'If ph.Name.IsNotNull AndAlso ph.Name.TypeId <> "2" Then
                            If ph.Name IsNot Nothing AndAlso ph.Name.TypeId <> "2" Then
                                'Personal Name
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, myaccordClientId, paneIndex)
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, myaccordClientId, paneIndex)
                            Else
                                'comm name
                                v.Message = "Missing Business Name"
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessName, v, myaccordClientId, paneIndex)
                            End If

                        Case InsuredValidator.CommAndPersNameComponentsAllSet
                            'Updated 9/4/18 for multi state MLW
                            'If ph.Name.IsNotNull AndAlso ph.Name.TypeId <> "2" Then
                            If ph.Name IsNot Nothing AndAlso ph.Name.TypeId <> "2" Then
                                'Personal Name
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, myaccordClientId, paneIndex)
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, myaccordClientId, paneIndex)
                            Else
                                'comm name
                                ' should not be possible
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessName, v, myaccordClientId, paneIndex)
                            End If

                        Case InsuredValidator.EntityTypeId ' matt a 2-26-15
                            If Not IsQuoteEndorsement() Then 'CAH 10/8/2020 Task51953
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBusinessType, v, myaccordClientId, paneIndex)
                            End If
                        Case InsuredValidator.OtherEntityType 'Added 2/14/2022 for bug 63511 MLW
                            If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63511_PolicyholderOtherEntityType")) = True Then
                                Select Case Me.Quote.LobType
                                    Case QuickQuoteLobType.CommercialAuto,
                                         QuickQuoteLobType.CommercialBOP,
                                         QuickQuoteLobType.CommercialGeneralLiability,
                                         QuickQuoteLobType.CommercialPackage,
                                         QuickQuoteLobType.CommercialProperty,
                                         QuickQuoteLobType.WorkersCompensation
                                        If Not IsQuoteEndorsement() Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtOtherEntity, v, myaccordClientId, paneIndex)
                                        End If
                                End Select
                            End If

                        Case InsuredValidator.PolicyHolderFirstNameID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderLastNameID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderGenderID
                            If Not IsQuoteEndorsement() Then 'CAH 9/6/2019 Task40294
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddSex, v, myaccordClientId, paneIndex)
                            End If
                        Case InsuredValidator.PolicyHolderSSNID  ' Changed 3-20-17 to handle Commercial vs Personal SSN
                            'Updated 9/4/18 for multi state MLW
                            'If ph.Name.IsNotNull AndAlso ph.Name.TypeId <> "2" Then
                            If ph.Name IsNot Nothing AndAlso ph.Name.TypeId <> "2" Then
                                'Personal Name
                                If String.IsNullOrWhiteSpace(Me.txtSSN.Text) = False Then
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSSN, v, myaccordClientId, paneIndex)
                                End If

                            Else
                                If Not IsQuoteEndorsement() Then
                                    'comm name
                                    If String.IsNullOrWhiteSpace(txtSSNBusiness.Text) = False OrElse Me.Quote.LobType = QuickQuoteLobType.WorkersCompensation Then
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSSNBusiness, v, myaccordClientId, paneIndex)
                                    End If
                                End If
                            End If
                        Case InsuredValidator.PolicyHolderFEINID  ' MGB 3-20-2017 (re-added 10/5/17 MGB)
                            If Not IsQuoteEndorsement() Then 'CAH 10/8/2020 Task51953
                                'If String.IsNullOrWhiteSpace(txtFEIN.Text) = False OrElse Me.Quote.LobType = QuickQuoteLobType.WorkersCompensation Then
                                If Me.Quote.LobType = QuickQuoteLobType.WorkersCompensation OrElse (String.IsNullOrWhiteSpace(txtFEIN.Text) = False AndAlso PolicyholderTaxNumEmptyAfterCommercialDataPrefillFirmographics(ph) = False) Then
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFEIN, v, myaccordClientId, paneIndex)
                                End If
                            End If
                        Case InsuredValidator.PolicyHolderBirthDate
                            If Not IsQuoteEndorsement() Then 'CAH 9/6/2019 Task40294
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBirthDate, v, myaccordClientId, paneIndex)
                            End If
                        Case InsuredValidator.PolicyHolderEmail
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEmail, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderPhoneNumber
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPhone, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderPhoneExtension
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPhoneExt, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderPhoneType
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddPhoneType, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderStreetAndPoBoxEmpty
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, myaccordClientId, paneIndex)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, myaccordClientId, paneIndex)
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBox, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderStreetAndPoxBoxAreSet
                            If IsQuoteEndorsement() = False Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, myaccordClientId, paneIndex)
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, myaccordClientId, paneIndex)
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBox, v, myaccordClientId, paneIndex)
                            End If
                        Case InsuredValidator.PolicyHolderHouseNumberID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderStreetNameID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderPOBOXID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPOBox, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderZipCodeID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderCityID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderStateID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderCountyID
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedCounty, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderEmailAndPhoneIsEmpty
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEmail, v, myaccordClientId, paneIndex)
                        Case InsuredValidator.PolicyHolderDescriptionOfOperations
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescriptionOfOperations, v, myaccordClientId, paneIndex)
                    End Select

                Next
            End If
            Populate()
        End If
    End Sub
    Private Function PolicyholderTaxNumEmptyAfterCommercialDataPrefillFirmographics(ByVal ph As QuickQuotePolicyholder) As Boolean
        Dim isIt As Boolean = False

        If Me.InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave = True AndAlso ph IsNot Nothing AndAlso ph.Name IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ph.Name.TaxNumber) = True Then
            isIt = True
        End If

        Return isIt
    End Function

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        If IsOnAppPage Then
            Me.txtEmail.CreateWatermark("User@Site.com")
            Me.txtPhone.CreateMask("(000)000-0000")
            Me.txtPhone.CreateWatermark("(000)000-0000")
            Return
        End If

        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Clear Policyholder?")

        Me.txtSSN.CreateMask("000-00-0000")
        Me.txtSSNBusiness.CreateMask("000-00-0000")
        Me.txtFEIN.CreateMask("00-0000000")
        Me.txtBirthDate.CreateWatermark("MM/DD/YYYY")
        'Me.txtBusinessStarted.CreateWatermark("MM/DD/YYYY")
        Me.txtBusinessStarted.CreateMask("00/00/0000")
        Me.txtEmail.CreateWatermark("User@Site.com")
        Me.txtPhone.CreateMask("(000)000-0000")
        Me.txtPhone.CreateWatermark("(000)000-0000")

        Me.txtCityName.CreateAutoComplete("INCities")

        ' Tax ID Type DDL
        'Me.ddTaxIDType.Attributes.Add("onchange", "Bop.TaxIdChanged('" & ddTaxIDType.ClientID & "','" & liSSNBusiness.ClientID & "','" & liFEIN.ClientID & "');")
        Me.ddTaxIDType.Attributes.Add("onchange", "var ddtyp = document.getElementById('" & ddTaxIDType.ClientID & "');var liSSN = document.getElementById('" & liSSNBusiness.ClientID & "');var liFEIN = document.getElementById('" & liFEIN.ClientID & "');if (ddtyp.selectedIndex == '1') {liSSN.style.display = 'none';liFEIN.style.display = '';} else {if (ddtyp.selectedIndex == '2') {liSSN.style.display = '';liFEIN.style.display = 'none';} else {liSSN.style.display = 'none';liFEIN.style.display = 'none';}}")

        'Me.VRScript.CreateTextBoxFormatter(Me.txtFirstName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur) 'removed 12-16-2015 should allow anything
        'Me.VRScript.CreateTextBoxFormatter(Me.txtMiddleName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur) 'removed 12-16-2015 should allow anything
        'Me.VRScript.CreateTextBoxFormatter(Me.txtLastName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur) 'removed 12-16-2015 should allow anything

        Me.VRScript.CreateTextBoxFormatter(Me.txtPhoneExt, ctlPageStartupScript.FormatterType.PositiveNumberNoCommas, ctlPageStartupScript.JsEventType.onblur)
        'Me.VRScript.CreateTextBoxFormatter(Me.txtStreetNum, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur)
        'Me.VRScript.CreateTextBoxFormatter(Me.txtStreetName, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur) 'removed 12-16-2015 should allow anything
        'Me.VRScript.CreateTextBoxFormatter(Me.txtAptNum, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur)
        'Me.VRScript.CreateTextBoxFormatter(Me.txtPOBox, ctlPageStartupScript.FormatterType.AlphaNumeric, ctlPageStartupScript.JsEventType.onblur)


        Me.VRScript.CreateTextBoxFormatter(Me.txtCityName, ctlPageStartupScript.FormatterType.AlphabeticOnly, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtGaragedCounty, ctlPageStartupScript.FormatterType.AlphabeticOnly, ctlPageStartupScript.JsEventType.onblur)
        'Me.VRScript.AddScriptLine("$(""#" + Me.txtCityName.ClientID + """).autocomplete({ source: INCities });")

        Me.txtGaragedCounty.CreateAutoComplete("indiana_Counties")
        'Me.VRScript.AddScriptLine("$(""#" + Me.txtGaragedCounty.ClientID + """).autocomplete({ source: indiana_Counties });")

        Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();")
        Me.VRScript.CreateJSBinding(Me.ddCityName.ClientID, "change", "CopyAddressFromPolicyHolder();if($(this).val() == '0'){$(""#" + Me.txtCityName.ClientID + """).show(); } else {$(""#" + Me.txtCityName.ClientID + """).val($(this).val()); $(""#" + Me.txtCityName.ClientID + """).hide();}")

        ' Add logic to hide or show the Years Experience row based on the entered Business Started date
        'Dim bsJs As String = "var dv = document.getElementById('" & txtBusinessStarted.ClientID & "').value;var MyDt = new Date(dv);var earliestdate = new Date('1/1/1900'); if (MyDt < earliestdate){return false;} else {var tody = new Date(); var yr = tody.getFullYear(); var mo = tody.getMonth() +1;var dy = tody.getDate();var CurrentDateMinus3Years = new Date(yr - 3, mo, dy); if (CurrentDateMinus3Years<MyDt){document.getElementById('" & liYearsExperience.ClientID & "').style.display='block'; } else {document.getElementById('" & liYearsExperience.ClientID & "').style.display='none';} };"
        'Me.VRScript.CreateJSBinding(txtBusinessStarted.ClientID, "input", bsJs)

        ' WHAT ACTUALLY NEEDS TO HAPPEN WITH DATE BUSINESS STARTED is that when the date entered is greater than 3 years a dialog will be shown then the years experience row will display and be required
        'Dim bsJs As String = "var dv = document.getElementById('" & txtBusinessStarted.ClientID & "').value;var MyDt = new Date(dv);var earliestdate = new Date('1/1/1900'); if (MyDt < earliestdate){return false;} else {var tody = new Date(); var yr = tody.getFullYear(); var mo = tody.getMonth() +1;var dy = tody.getDate();var CurrentDateMinus3Years = new Date(yr - 3, mo, dy); if (CurrentDateMinus3Years<MyDt){alert('Date business started is less than 3 years ago. Please enter years of experience in this type of business operation.');document.getElementById('" & liYearsExperience.ClientID & "').style.display='block'; } else {document.getElementById('" & liYearsExperience.ClientID & "').style.display='none';} };"
        'Dim bsJs As String = "BusinessStartedLessThanThreeYearsAgo()"
        'Me.VRScript.CreateJSBinding(txtBusinessStarted.ClientID, "input", bsJs)

        ' CLEAR CLIENT ID TEXTBOX ON KEY UP - START; removed 5/1/2019 so we can maintain the same client after it's established (changes will just be made to PH records)
        'Dim clearClientIdScript As String = "$(""#txtClientId_Lookup"").val('');"
        'If Me.Quote.Policyholder.Name.TypeId <> "2" Then
        '    Me.VRScript.CreateJSBinding({txtFirstName, txtMiddleName, txtLastName, txtSSN, txtBirthDate}, ctlPageStartupScript.JsEventType.onkeyup, clearClientIdScript)
        'Else
        '    Me.VRScript.CreateJSBinding({txtBusinessName, txtDBA, txtFEIN}, ctlPageStartupScript.JsEventType.onkeyup, clearClientIdScript)
        'End If

        'Me.VRScript.CreateJSBinding({txtEmail, txtPhone, ddPhoneType, txtStreetNum, txtStreetName, txtAptNum, txtPOBox, txtCityName}, ctlPageStartupScript.JsEventType.onkeyup, clearClientIdScript)


        ' CLEAR CLIENT ID TEXTBOX ON KEY UP - END

        If Me.Quote.Policyholder.Name.TypeId <> "2" Then
            'Me.VRScript.CreateJSBinding(Me.ddSuffix, ctlPageStartupScript.JsEventType.onchange, clearClientIdScript)
            'Me.VRScript.CreateJSBinding(Me.ddSex, ctlPageStartupScript.JsEventType.onchange, clearClientIdScript)
            Me.VRScript.CreateTextBoxFormatter(Me.txtBirthDate, ctlPageStartupScript.FormatterType.DateFormat, ctlPageStartupScript.JsEventType.onblur)

        Else
            'Me.VRScript.CreateJSBinding(Me.ddBusinessType, ctlPageStartupScript.JsEventType.onchange, clearClientIdScript) ' Matt A
            Select Case Me.Quote.LobType
                Case QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteLobType.CommercialBOP, QuickQuoteLobType.CommercialAuto, QuickQuoteLobType.WorkersCompensation, QuickQuoteLobType.CommercialProperty, QuickQuoteLobType.CommercialPackage ' Zach S
                    txtBusinessStarted.Attributes.Add("onkeyup", "Bop.BusinessStartedLessThanThreeYearsAgo('" & txtBusinessStarted.ClientID & "','" & liYearsExperience.ClientID & "','" & txtYearsOfExperience.ClientID & "');")
                    'Added 2/14/2022 for bug 63511 MLW
                    If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63511_PolicyholderOtherEntityType")) = True AndAlso (Not IsEndorsementRelated()) Then
                        Me.ddBusinessType.Attributes.Add("onchange", "ShowHideOtherEntityType('" & ddBusinessType.ClientID & "','" & liOtherEntity.ClientID & "','" & txtOtherEntity.ClientID & "');")
                    End If
                    Exit Select
            End Select
        End If

        'Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange, clearClientIdScript)
        'Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, clearClientIdScript + "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtGaragedCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtGaragedCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
        'Me.VRScript.CreateJSBinding(Me.txtGaragedCounty, ctlPageStartupScript.JsEventType.onkeyup, clearClientIdScript)

        ' do this on focus
        Dim addresswarningScript As String = "DoAddressWarning(true,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreetName.ClientID + "','" + txtPOBox.ClientID + "');"
        Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtPOBox}, ctlPageStartupScript.JsEventType.onfocus, addresswarningScript)

        'do this onblur
        Dim addresswarningScriptOff As String = "DoAddressWarning(false,'" + Me.divAddressMessage.ClientID + "','" + Me.txtStreetNum.ClientID + "','" + txtStreetName.ClientID + "','" + txtPOBox.ClientID + "');"
        Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtPOBox}, ctlPageStartupScript.JsEventType.onblur, "CopyAddressFromPolicyHolder();" + addresswarningScriptOff)

        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onblur, "formatPostalcode($(this).val()); CopyAddressFromPolicyHolder();")

        If Me.IsPolicyHolderNum1 Then

            ' use policyholder #1

            If Me.Quote.Policyholder.Name.TypeId <> "2" Then
                'Personal Name
                'Updated 1/18/2022 for bug 67521 MLW - added eSigEmail
                ''Me.VRScript.AddScriptLine("ClientSearch.SetBindings(true,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """);")
                ''updated 10/7/2017
                'Me.VRScript.AddScriptLine("ClientSearch.SetBindings(true,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """);")

                'If hasEsigOption() AndAlso Not IsEndorsementRelated() Then 'Updated 01/24/2022 for Bug 72362 MLW
                '    Me.VRScript.AddScriptLine("ClientSearch.SetBindings(true,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,esigEmail);")
                'Else
                '    Me.VRScript.AddScriptLine("ClientSearch.SetBindings(true,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,);")
                'End If

                If hasEsigOption() AndAlso Not IsEndorsementRelated() Then 'Updated 01/24/2022 for Bug 72362 MLW; CAH 5/1/2023 ws-656
                    Me.VRScript.AddScriptLine("ClientSearch.SetBindings(true,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,esigEmail,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                Else
                    Me.VRScript.AddScriptLine("ClientSearch.SetBindings(true,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + ""","""",""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                End If

                'bindings to copy address
                Dim binding As String = "streetNumClientID1 = '" + Me.txtStreetNum.ClientID + "';"
                binding += "streetNameClientID1 = '" + Me.txtStreetName.ClientID + "';"
                binding += "aptNumClientID1 = '" + Me.txtAptNum.ClientID + "';"
                binding += "COClientID1 = '" + Me.txtPrefixCO.ClientID + "';"
                binding += "POBoxClientID1 = '" + Me.txtPOBox.ClientID + "';"
                binding += "CityClientID1 = '" + Me.txtCityName.ClientID + "';"
                binding += "StateClientID1 = '" + Me.ddStateAbbrev.ClientID + "';"
                binding += "ZipClientID1 = '" + Me.txtZipCode.ClientID + "';"
                binding += "CountyClientID1 = '" + Me.txtGaragedCounty.ClientID + "';"
                Me.VRScript.AddScriptLine(binding)

                Dim scriptHeaderUpdate As String = "updateInsuredHeaderText(""" + Me.lblInsuredTitle.ClientID + """,""1"",""" + Me.txtFirstName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,0,""" + Me.ddSuffix.ClientID + """); "

                Me.VRScript.AddVariableLine("function Ph1UpdateHeaderText(){" + scriptHeaderUpdate + "}")
                Me.VRScript.AddScriptLine("Ph1UpdateHeaderText();")

                Dim clientSearchScript As String = "ClientSearch.DoSearchWithEleNames(e.keyCode,$(this).attr('id'),""" + Me.txtFirstName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtSSN.ClientID + """);"

                Me.VRScript.CreateJSBinding(Me.txtFirstName, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript + scriptHeaderUpdate)
                Me.VRScript.CreateJSBinding(Me.txtMiddleName, ctlPageStartupScript.JsEventType.onkeyup, scriptHeaderUpdate)
                Me.VRScript.CreateJSBinding(Me.txtLastName, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript + scriptHeaderUpdate)

                Me.VRScript.CreateJSBinding(Me.ddSuffix, ctlPageStartupScript.JsEventType.onchange, scriptHeaderUpdate)

                Me.VRScript.CreateJSBinding({Me.txtSSN, Me.txtCityName, Me.txtZipCode}, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript)

            Else
                'Commercial Name
                'Updated 2/22/2022 for bug 63511 MLW - added otherEntityDescription - only for BOP, CAP, CPP, CPR, CGL, WCP
                ''Updated 1/19/2022 for bug 67521 MLW - added eSigEmail
                '''Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"");")
                '''updated 10/5/2017 and 10/6/2017; had to add phoneType too; emailType, phoneExt, descOfOps, busStartedDt, yrsExp, liYrsExp 10/7/2017
                ''Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """);")
                'If hasEsigOption() AndAlso Not IsEndorsementRelated() Then 'Updated 01/24/2022 for Bug 72362 MLW
                '    Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,esigEmail);")
                'Else
                '    Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,);")
                'End If
                If hasEsigOption() AndAlso Not IsEndorsementRelated() Then 'Updated 01/24/2022 for Bug 72362 MLW
                    If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63511_PolicyholderOtherEntityType")) = True Then
                        Select Case Me.Quote.LobType
                            Case QuickQuoteLobType.CommercialAuto, QuickQuoteLobType.CommercialBOP, QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteLobType.CommercialPackage, QuickQuoteLobType.CommercialProperty, QuickQuoteLobType.WorkersCompensation
                                Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,esigEmail,""" + Me.txtOtherEntity.ClientID + """,""" + Me.liOtherEntity.ClientID + """,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                            Case Else
                                Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,esigEmail,null,null,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                        End Select
                    Else
                        Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,esigEmail,null,null,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                    End If
                Else
                    If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63511_PolicyholderOtherEntityType")) = True AndAlso Not IsEndorsementRelated() Then
                        Select Case Me.Quote.LobType
                            Case QuickQuoteLobType.CommercialAuto, QuickQuoteLobType.CommercialBOP, QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteLobType.CommercialPackage, QuickQuoteLobType.CommercialProperty, QuickQuoteLobType.WorkersCompensation
                                Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,null,""" + Me.txtOtherEntity.ClientID + """,""" + Me.liOtherEntity.ClientID + """,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                            Case Else
                                Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,null,null,null,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                        End Select
                    Else
                        Me.VRScript.AddScriptLine("ClientSearch.SetBindingsComm(""" + Me.txtBusinessName.ClientID + """,""" + Me.txtDBA.ClientID + """,""" + Me.ddBusinessType.ClientID + """,""" + Me.txtFEIN.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.txtSSNBusiness.ClientID + """,""" + Me.ddTaxIDType.ClientID + """,""" + Me.liFEIN.ClientID + """,""" + Me.liSSNBusiness.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """,""" + Me.txtDescriptionOfOperations.ClientID + """,""" + Me.txtBusinessStarted.ClientID + """,""" + Me.txtYearsOfExperience.ClientID + """,""" + Me.liYearsExperience.ClientID + """,null,null,null,""" + Me.txtPrefixCO.ClientID + """,""" + Me.OtherPrefix.ClientID + """);")
                    End If
                End If
                'bindings to copy address
                Dim binding As String = "streetNumClientID1 = '" + Me.txtStreetNum.ClientID + "';"
                binding += "streetNameClientID1 = '" + Me.txtStreetName.ClientID + "';"
                binding += "aptNumClientID1 = '" + Me.txtAptNum.ClientID + "';"
                binding += "COClientID1 = '" + Me.txtPrefixCO.ClientID + "';"
                binding += "POBoxClientID1 = '" + Me.txtPOBox.ClientID + "';"
                binding += "CityClientID1 = '" + Me.txtCityName.ClientID + "';"
                binding += "StateClientID1 = '" + Me.ddStateAbbrev.ClientID + "';"
                binding += "ZipClientID1 = '" + Me.txtZipCode.ClientID + "';"
                binding += "CountyClientID1 = '" + Me.txtGaragedCounty.ClientID + "';"
                Me.VRScript.AddScriptLine(binding)

                Dim scriptHeaderUpdate As String = "updateInsuredHeaderText(""" + Me.lblInsuredTitle.ClientID + """,""1"",""" + Me.txtBusinessName.ClientID + ""","""","""",0,""""); "

                Me.VRScript.AddVariableLine("function Ph1UpdateHeaderText(){" + scriptHeaderUpdate + "}")

                Me.VRScript.AddScriptLine("Ph1UpdateHeaderText();")

                Dim clientSearchScript As String = "ClientSearch.DoSearchWithEleNamesComm(e.keyCode,$(this).attr('id'),""" + Me.txtBusinessName.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtFEIN.ClientID + """);"

                'Me.VRScript.CreateJSBinding(Me.txtBusinessName, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript + clientSearchScript + scriptHeaderUpdate)
                'updated 10/6/2017 to only register clientSearchScript once
                Me.VRScript.CreateJSBinding(Me.txtBusinessName, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript + scriptHeaderUpdate)
                Me.VRScript.CreateJSBinding({Me.txtDBA, Me.txtFEIN, Me.txtCityName, Me.txtZipCode}, ctlPageStartupScript.JsEventType.onkeyup, clientSearchScript)

            End If

            ' copy address from PH1 to PH2
            Me.VRScript.CreateJSBinding({Me.txtAptNum, Me.txtCityName, Me.txtGaragedCounty}, ctlPageStartupScript.JsEventType.onblur, "CopyAddressFromPolicyHolder();")

        Else
            ' use policyholder #2
            'Dim bindingCode As String = "ClientSearch.SetBindings(false,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """);"
            'updated 10/7/2017
            Dim bindingCode As String = "ClientSearch.SetBindings(false,""" + Me.txtFirstName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.ddSuffix.ClientID + """,""" + Me.ddSex.ClientID + """,""" + Me.txtSSN.ClientID + """,""" + Me.txtBirthDate.ClientID + """,""" + Me.txtEmail.ClientID + """,""" + Me.txtPhone.ClientID + """,""" + Me.txtStreetNum.ClientID + """,""" + Me.txtStreetName.ClientID + """,""" + Me.txtAptNum.ClientID + """,""" + Me.txtPOBox.ClientID + """,""" + Me.txtCityName.ClientID + """,""" + Me.ddStateAbbrev.ClientID + """,""" + Me.txtZipCode.ClientID + """,""" + Me.txtGaragedCounty.ClientID + """,""txtClientId_Lookup"",""" + Me.ddPhoneType.ClientID + """,""" + Me.hdnEmailTypeId.ClientID + """,""" + Me.txtPhoneExt.ClientID + """);"
            Me.VRScript.AddScriptLine(bindingCode)

            'bindings to copy address
            Dim binding As String = "streetNumClientID2 = '" + Me.txtStreetNum.ClientID + "';"
            binding += "streetNameClientID2 = '" + Me.txtStreetName.ClientID + "';"
            binding += "aptNumClientID2 = '" + Me.txtAptNum.ClientID + "';"
            binding += "COClientID2 = '" + Me.txtPrefixCO.ClientID + "';"
            binding += "POBoxClientID2 = '" + Me.txtPOBox.ClientID + "';"
            binding += "CityClientID2 = '" + Me.txtCityName.ClientID + "';"
            binding += "StateClientID2 = '" + Me.ddStateAbbrev.ClientID + "';"
            binding += "ZipClientID2 = '" + Me.txtZipCode.ClientID + "';"
            binding += "CountyClientID2 = '" + Me.txtGaragedCounty.ClientID + "';"
            Me.VRScript.AddScriptLine(binding)

            Dim scriptHeaderUpdate As String = "updateInsuredHeaderText(""" + Me.lblInsuredTitle.ClientID + """,""2"",""" + Me.txtFirstName.ClientID + """,""" + Me.txtLastName.ClientID + """,""" + Me.txtMiddleName.ClientID + """,1,""" + Me.ddSuffix.ClientID + """); "
            Me.VRScript.AddVariableLine("function Ph2UpdateHeaderText(){" + scriptHeaderUpdate + "}")

            Me.VRScript.AddScriptLine("Ph2UpdateHeaderText();")


            Me.VRScript.CreateJSBinding({Me.txtFirstName, Me.txtMiddleName, Me.txtLastName}, ctlPageStartupScript.JsEventType.onkeyup, scriptHeaderUpdate + "CopyAddressFromPolicyHolder();")

            Me.VRScript.CreateJSBinding(Me.ddSuffix, ctlPageStartupScript.JsEventType.onchange, scriptHeaderUpdate)

            Me.tblAddressInfo.Visible = True
            Me.tblAddressInfo.Attributes.Add("title", "Read-only. Copied from Policyholder #1")

            '7-20-14 DRiver copy
            'Updated 9/4/18 for multi state MLW
            'If Me.Quote IsNot Nothing AndAlso Me.Quote.LobId = 1 AndAlso Me.Quote.Drivers IsNot Nothing Then
            If Me.Quote IsNot Nothing AndAlso CInt(Me.Quote.LobId) = 1 AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers IsNot Nothing Then
                'Updated 9/4/18 for multi state MLW
                'Me.trCopyFromDriver.Visible = Me.Quote.Drivers IsNot Nothing AndAlso Me.Quote.Drivers.Any()
                Me.trCopyFromDriver.Visible = Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Any()
                Me.VRScript.AddVariableLine("var ph2FirstNameId = '" + Me.txtFirstName.ClientID + "';")
                Me.VRScript.AddVariableLine("var ph2MiddleNameId = '" + Me.txtMiddleName.ClientID + "';")
                Me.VRScript.AddVariableLine("var ph2LastNameId = '" + Me.txtLastName.ClientID + "';")
                Me.VRScript.AddVariableLine("var ph2SuffixId = '" + Me.ddSuffix.ClientID + "';")
                Me.VRScript.AddVariableLine("var ph2BirthDateId = '" + Me.txtBirthDate.ClientID + "';")
                Me.VRScript.AddVariableLine("var ph2GenderId = '" + Me.ddSex.ClientID + "';")
            Else
                Me.trCopyFromDriver.Visible = False
            End If
        End If

        'Added 12/4/2019 for eSignature project task 41686 MLW - esigEmail defined in ctl_Esignature.ascx
        If Me.IsPolicyHolderNum1 AndAlso Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
            If QQDevDictionary_GetItem("Sign_Application_Using_eSignature") Is Nothing OrElse UCase(QQDevDictionary_GetItem("Sign_Application_Using_eSignature")) <> "YES" Then
                Dim copyPH1EmailToEsigEmail As String = "$(""#"" + esigEmail).val($(""#" + Me.txtEmail.ClientID + """).val());"
                Me.VRScript.CreateJSBinding({Me.txtEmail}, ctlPageStartupScript.JsEventType.onchange, copyPH1EmailToEsigEmail)
            End If
            Me.VRScript.AddVariableLine("var phEmail = '" + Me.txtEmail.ClientID + "';") 'used on ctl_Esignature.ascx to copy email
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then

            'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
            If AllowValidateAndSave() Then
                'Dim isDifferent As Boolean = False

                Dim ph As QuickQuotePolicyholder = Nothing
                If Me.IsPolicyHolderNum1 Then
                    ' use policyholder #1
                    ph = Me.Quote.Policyholder
                Else
                    ' use policyholder #2
                    ph = Me.Quote.Policyholder2
                    If String.IsNullOrWhiteSpace(Me.txtFirstName.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtLastName.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtBusinessName.Text.ToUpper().Trim()) Then ' And String.IsNullOrWhiteSpace(Me.txtMiddleName.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtSSN.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtBirthDate.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtEmail.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtPhone.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.txtPhoneExt.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.ddSuffix.SelectedItem.Text.ToUpper().Trim()) And String.IsNullOrWhiteSpace(Me.ddSex.SelectedItem.Text.ToUpper().Trim()) Then
                        'empty Ph#2
                        Me.lblInsuredTitle.Text = String.Format("Policyholder #{0} - ", If(IsPolicyHolderNum1, "1", "2")).ToUpper().Replace("  ", " ")
                        Me.Quote.Policyholder2 = New QuickQuotePolicyholder() 'empty
                        Return False
                    End If
                End If

                If ph.Name Is Nothing Then
                    ph.Name = New QuickQuoteName()
                End If

                If ph.Name.TypeId <> "2" Then
                    'Personal Name
                    ph.Name.TypeId = "1"
                    ph.Name.FirstName = Me.txtFirstName.Text.ToUpper().Trim()
                    ph.Name.MiddleName = Me.txtMiddleName.Text.ToUpper().Trim()
                    ph.Name.LastName = Me.txtLastName.Text.ToUpper().Trim()
                    ph.Name.SuffixName = Me.ddSuffix.SelectedValue.ToUpper().Trim()

                    ph.Name.SexId = Me.ddSex.SelectedValue
                    ph.Name.TaxNumber = Me.txtSSN.Text.ToUpper().Trim()
                    ph.Name.TaxTypeId = "1"
                    ph.Name.BirthDate = Me.txtBirthDate.Text

                    ph.Name.EntityTypeId = "1" 'added 6-18-15 as result of Bug 5039
                    'Updated 9/4/18 for multi state MLW
                    'Me.Quote.EntityTypeId = "1" 'added 6-18-15 as result of Bug 5039
                    If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                            sq.EntityTypeId = "1" 'added 6-18-15 as result of Bug 5039
                        Next
                    End If

                Else
                    'Added 11/16/2020 for CAP Endorsements task 52971 MLW
                    If Not Me.IsQuoteEndorsement OrElse Me.Quote.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        ph.Name.CommercialName1 = Me.txtBusinessName.Text.ToUpper().Trim()
                        ph.Name.DoingBusinessAsName = Me.txtDBA.Text.ToUpper().Trim()
                        ph.Name.EntityTypeId = Me.ddBusinessType.SelectedValue
                        'Updated 9/4/18 for multi state MLW
                        'Me.Quote.EntityTypeId = Me.ddBusinessType.SelectedValue 'added 6-18-15 as result of Bug 5039
                        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                sq.EntityTypeId = Me.ddBusinessType.SelectedValue 'added 6-18-15 as result of Bug 5039
                            Next
                        End If

                        'Added 2/14/2022 for bug 63511 MLW - need this in the SubQuotes section above?
                        If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63511_PolicyholderOtherEntityType")) = True Then
                            Select Case Me.Quote.LobType
                                Case QuickQuoteLobType.CommercialAuto,
                                     QuickQuoteLobType.CommercialBOP,
                                     QuickQuoteLobType.CommercialGeneralLiability,
                                     QuickQuoteLobType.CommercialPackage,
                                     QuickQuoteLobType.CommercialProperty,
                                     QuickQuoteLobType.WorkersCompensation
                                    If ph.Name.EntityTypeId = "5" Then
                                        ph.Name.OtherLegalEntityDescription = txtOtherEntity.Text
                                    End If
                            End Select
                        End If


                        ph.Name.DescriptionOfOperations = txtDescriptionOfOperations.Text.Trim

                        If ddTaxIDType.SelectedIndex > 0 Then
                            If CInt(ddTaxIDType.SelectedValue) = 1 Then
                                ph.Name.TaxNumber = Me.txtSSNBusiness.Text.ToUpper().Trim()
                                Me.txtFEIN.Text = String.Empty
                                ph.Name.TaxTypeId = "1"
                            Else
                                ph.Name.TaxNumber = Me.txtFEIN.Text.ToUpper().Trim()
                                Me.txtSSNBusiness.Text = String.Empty
                                ph.Name.TaxTypeId = "2"
                            End If
                        End If
                    End If

                    ph.Name.DateBusinessStarted = Me.txtBusinessStarted.Text.Trim()
                    If txtYearsOfExperience.Text.Trim <> "" Then
                        ph.Name.YearsOfExperience = Me.txtYearsOfExperience.Text.Trim()
                        liYearsExperience.Attributes.Add("style", "display:''")
                    Else
                        ph.Name.YearsOfExperience = String.Empty
                        liYearsExperience.Attributes.Add("style", "display:none")
                    End If
                End If

                If String.IsNullOrWhiteSpace(Me.txtEmail.Text) = False Then
                    If ph.Emails Is Nothing OrElse ph.Emails.Count() = 0 Then
                        ph.Emails = New List(Of QuickQuoteEmail)()
                        ph.Emails.Add(New QuickQuoteEmail())
                    End If

                    ph.Emails(0).Address = Me.txtEmail.Text.ToUpper().Trim()
                    'ph.Emails(0).TypeId = "1"
                    'updated 10/7/2017
                    If QQHelper.IsPositiveIntegerString(Me.hdnEmailTypeId.Value) = False Then
                        If ph.Name.TypeId <> "2" Then
                            'Pers; set as Home
                            Me.hdnEmailTypeId.Value = "1"
                        Else
                            'Comm; set as Bus
                            Me.hdnEmailTypeId.Value = "2"
                        End If
                    End If
                    ph.Emails(0).TypeId = Me.hdnEmailTypeId.Value
                Else
                    ph.Emails = Nothing
                End If

                If String.IsNullOrWhiteSpace(Me.txtPhone.Text) And String.IsNullOrWhiteSpace(Me.txtPhoneExt.Text) And Me.ddPhoneType.SelectedIndex = 0 Then
                    ph.Phones = Nothing
                Else
                    If ph.Phones Is Nothing OrElse ph.Phones.Count() = 0 Then
                        ph.Phones = New List(Of QuickQuotePhone)()
                        ph.Phones.Add(New QuickQuotePhone())
                    End If

                    ph.Phones(0).Number = Me.txtPhone.Text.ToUpper().Trim()

                    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtPhoneExt.Text.ToUpper()) Then
                        ph.Phones(0).Extension = Me.txtPhoneExt.Text.ToUpper().Trim()
                    Else
                        If IFM.Common.InputValidation.CommonValidations.IsNonNegativeWholeNumber(Me.txtPhoneExt.Text) Then
                            ph.Phones(0).Extension = If(Int32.TryParse(Me.txtPhoneExt.Text, Nothing), Math.Abs(CInt(Me.txtPhoneExt.Text)), "")
                        Else
                            '  it must be a number or it will bomb out 6-5-14
                            ph.Phones(0).Extension = ""
                        End If
                    End If

                    ph.Phones(0).TypeId = Me.ddPhoneType.SelectedValue
                End If

                If Me.IsPolicyHolderNum1 Then
                    If ph.Address Is Nothing Then
                        ph.Address = New QuickQuoteAddress()
                    End If

                    Dim AOtype As AddressOtherFieldPrefixHelper.OtherFieldPrefix
                    Dim AOName As String
                    If showCareOf = True Then
                        AOtype = AddressOtherFieldPrefixHelper.GetPrefixType(Me.OtherPrefix.Text)
                        AOName = Me.txtPrefixCO.Text.Trim
                    Else
                        AOtype = AddressOtherFieldPrefixHelper.OtherFieldPrefix.Other
                        AOName = Me.txtCO.Text.Trim
                    End If

                    Dim AddressOtherField As AddressOtherField = New AddressOtherField()
                    AddressOtherField.UpdateNameAndType(AOName, AOtype)

                    If Quote.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote Then
                        Dim hasChanges As Boolean = False
                        SetValueIfDifferent(ph.Address.HouseNum, Me.txtStreetNum.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.StreetName, Me.txtStreetName.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.ApartmentNumber, Me.txtAptNum.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.Other, AddressOtherField.NameWithPrefix.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.POBox, Me.txtPOBox.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.City, Me.txtCityName.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.StateId, Me.ddStateAbbrev.SelectedValue, HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.Zip, Me.txtZipCode.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        SetValueIfDifferent(ph.Address.County, Me.txtGaragedCounty.Text.ToUpper().Trim(), HasChange:=hasChanges, DontResetHasChangedFlagToFalse:=True, AlwaysSetValue:=True)
                        ph.Address.IsChanged = hasChanges
                    Else
                        ph.Address.HouseNum = Me.txtStreetNum.Text.ToUpper().Trim()
                        ph.Address.StreetName = Me.txtStreetName.Text.ToUpper().Trim()
                        ph.Address.ApartmentNumber = Me.txtAptNum.Text.ToUpper().Trim()
                        ph.Address.Other = AddressOtherField.NameWithPrefix.ToUpper.Trim
                        ph.Address.POBox = Me.txtPOBox.Text.ToUpper().Trim()
                        ph.Address.City = Me.txtCityName.Text.ToUpper().Trim()
                        ph.Address.StateId = Me.ddStateAbbrev.SelectedValue
                        ph.Address.Zip = Me.txtZipCode.Text.ToUpper().Trim()
                        ph.Address.County = Me.txtGaragedCounty.Text.ToUpper().Trim()
                    End If

                    Select Case Me.Quote.LobId '7-28-14
                        Case "2" ' HOME
                            If Me.Quote.Locations Is Nothing Then ' there should already be a location from the Kill Questions Logic
                                Me.Quote.Locations = New List(Of QuickQuoteLocation)
                                Me.Quote.Locations.Add(New QuickQuoteLocation())
                            End If
                            If String.IsNullOrWhiteSpace(Me.Quote.Locations(0).Address.HouseNum) AndAlso String.IsNullOrWhiteSpace(Me.Quote.Locations(0).Address.StreetName) Then ' AndAlso String.IsNullOrWhiteSpace(Me.Quote.Locations(0).Address.Zip)
                                ' real address copy it to the Home Location 0 Address
                                Me.Quote.Locations(0).Address.HouseNum = ph.Address.HouseNum
                                Me.Quote.Locations(0).Address.StreetName = ph.Address.StreetName
                                Me.Quote.Locations(0).Address.ApartmentNumber = ph.Address.ApartmentNumber
                                Me.Quote.Locations(0).Address.Other = ph.Address.Other
                                Me.Quote.Locations(0).Address.City = ph.Address.City
                                Me.Quote.Locations(0).Address.StateId = ph.Address.StateId
                                Me.Quote.Locations(0).Address.Zip = ph.Address.Zip
                                Me.Quote.Locations(0).Address.County = ph.Address.County
                            End If
                    End Select

                End If

                'it is strange to have this here but it was needed
                If ph.Name IsNot Nothing Then
                    If ph.Name.TypeId <> "2" Then
                        'Personal Name
                        Me.lblInsuredTitle.Text = String.Format("Policyholder #{4} - {0} {1} {2} {3}", ph.Name.FirstName, ph.Name.MiddleName, ph.Name.LastName, ph.Name.SuffixName, If(IsPolicyHolderNum1, "1", "2")).ToUpper().Replace("  ", " ")
                        Me.lblInsuredTitle.Text = EllipsisText(Me.lblInsuredTitle.Text, 55)
                    Else
                        'Commercial Name
                        Me.lblInsuredTitle.Text = String.Format("Policyholder #{1} - {0}", ph.Name.CommercialName1, If(IsPolicyHolderNum1, "1", "2")).ToUpper().Replace("  ", " ")
                        Me.lblInsuredTitle.Text = EllipsisText(Me.lblInsuredTitle.Text, 55)
                    End If
                End If

                Return True
            End If
        End If
        Return False
    End Function

    Public Overrides Sub ClearControl()
        Me.txtBusinessName.Text = ""
        Me.txtDBA.Text = ""
        Me.txtFEIN.Text = String.Empty
        Me.txtSSNBusiness.Text = String.Empty
        Me.ddTaxIDType.SelectedIndex = -1
        Me.ddBusinessType.SelectedIndex = -1

        Me.txtAptNum.Text = ""
        Me.txtCO.Text = ""
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
        Me.ddSex.SelectedIndex = -1
        Me.ddStateAbbrev.SelectedIndex = -1
        Me.ddSuffix.SelectedIndex = -1
        Me.txtBusinessStarted.Text = String.Empty
        Me.txtYearsOfExperience.Text = String.Empty
        Me.txtDescriptionOfOperations.Text = String.Empty
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
        RaiseEvent PolicyHolderCleared()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))  ' to save and informs the tree of the change
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub ddTaxIDType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddTaxIDType.SelectedIndexChanged
        If ddTaxIDType.SelectedIndex < 0 Then Exit Sub

        Select Case ddTaxIDType.SelectedValue.ToUpper
            Case ""
                liFEIN.Attributes.Add("style", "display:none")
                liSSNBusiness.Attributes.Add("style", "display:none")
                Exit Select
            Case "2"
                liFEIN.Attributes.Add("style", "display:''")
                liSSNBusiness.Attributes.Add("style", "display:none")
                Exit Select
            Case "1"
                liFEIN.Attributes.Add("style", "display:none")
                liSSNBusiness.Attributes.Add("style", "display:''")
                Exit Select
            Case Else
                liFEIN.Attributes.Add("style", "display:none")
                liSSNBusiness.Attributes.Add("style", "display:none")
                Exit Sub
        End Select
    End Sub

    'Added 02/02/2021 for CAP Endorsements task 52971 MLW
    Private Function AllowPopulate() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteLobType.CommercialAuto
                If Not IsQuoteEndorsement() Then
                    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Amend Mailing Address" Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function

    'Added 02/02/2021 for CAP Endorsements task 52971 MLW
    Private Function AllowValidateAndSave() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteLobType.CommercialAuto
                If Not IsQuoteEndorsement() Then
                    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Amend Mailing Address" Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function
End Class