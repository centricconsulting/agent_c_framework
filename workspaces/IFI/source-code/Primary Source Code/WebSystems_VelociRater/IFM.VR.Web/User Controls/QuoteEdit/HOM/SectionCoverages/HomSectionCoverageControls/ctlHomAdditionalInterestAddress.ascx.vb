Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Public Class ctlHomAdditionalInterestAddress
    Inherits ctlSectionCoverageControlBase

    'Added 2/21/18 control for HOM Upgrade MLW

    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            AiList = Me.MySectionCoverage.AdditionalInterests
            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Me.Quote.Locations(0).AdditionalInterests = value
        End Set
    End Property

    Public Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            If MySectionCoverage IsNot Nothing Then
                If MySectionCoverage.AdditionalInterests IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests.Count > 0 Then
                    Return MySectionCoverage.AdditionalInterests(aiCounter)
                End If
            End If
            Return Nothing
        End Get
        Set(value As QuickQuoteAdditionalInterest)

        End Set
    End Property

    Public Property aiCounter As Int32
        Get
            If ViewState("vs_aiCounter") Is Nothing Then
                ViewState("vs_aiCounter") = 0
            End If
            Return CInt(ViewState("vs_aiCounter"))
        End Get
        Set(value As Int32)
            ViewState("vs_aiCounter") = value
        End Set
    End Property

    Public Property MyAddAddressLinkClientId As String
        Get
            If ViewState("vs_MyAddAddressLinkClientId") Is Nothing Then
                ViewState("vs_MyAddAddressLinkClientId") = ""
            End If
            Return ViewState("vs_MyAddAddressLinkClientId")
        End Get
        Set(value As String)
            ViewState("vs_MyAddAddressLinkClientId") = value
        End Set
    End Property

    Public ReadOnly Property HasSomeAddressinformation As Boolean
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.MySectionCoverage.IsNotNull Then
            If Me.MySectionCoverage IsNot Nothing Then
                'If MyAdditionalInterest.IsNotNull() Then
                If MyAdditionalInterest IsNot Nothing Then
                    Return IFM.PrimativeExtensions.OneorMoreIsNotEmpty(MyAdditionalInterest.Address.HouseNum, MyAdditionalInterest.Address.StreetName,
                                                                                   MyAdditionalInterest.Address.ApartmentNumber, MyAdditionalInterest.Address.Zip,
                                                                                   MyAdditionalInterest.Address.City, MyAdditionalInterest.Address.County)
                End If
            End If
            Return False
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property HouseNumTextBox As TextBox
        Get
            Return Me.txtStreetNum
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property StreetNameTextBox As TextBox
        Get
            Return Me.txtStreetName
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property ApartmentNumTextBox As TextBox
        Get
            Return Me.txtAptSuite
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property ZipCodeTextBox As TextBox
        Get
            Return Me.txtZipCode
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property CityTextBox As TextBox
        Get
            Return Me.txtCityName
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property StateDropDown As DropDownList
        Get
            Return Me.ddStateAbbrev
        End Get
    End Property

    'Added 03/26/2020 for Home Endorsements Bug 44392 MLW
    Public ReadOnly Property CountyTextBox As TextBox
        Get
            Return Me.txtCounty
        End Get
    End Property

    'added 12/28/17 for HOM Upgrade - MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'Updated 7/3/2019 for Home Endorsements Project Tasks 38911, 38912, 38916, 38925 MLW
            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                'set default text and formatting
                Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtStreetNum.ClientID + "').val().toUpperCase().trim() == ""NEED STREET #"") { $('#" + txtStreetNum.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtStreetNum.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtStreetName, ctlPageStartupScript.JsEventType.onkeyup,
                                                           "if($('#" + txtStreetName.ClientID + "').val().toUpperCase().trim() == ""NEED STREET NAME"") { $('#" + txtStreetName.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtStreetName.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtAptSuite, ctlPageStartupScript.JsEventType.onkeyup,
                                                           "if($('#" + txtAptSuite.ClientID + "').val().toUpperCase().trim() == ""NEED IF APPLICABLE"") { $('#" + txtAptSuite.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtAptSuite.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup,
                                                           "if($('#" + txtZipCode.ClientID + "').val().toUpperCase().trim() == ""00001"") { $('#" + txtZipCode.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtZipCode.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onblur,
                                                           "if($('#" + txtCityName.ClientID + "').val().toUpperCase().trim() == ""NEED CITY"") { $('#" + txtCityName.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCityName.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onblur,
                                                           "if($('#" + ddStateAbbrev.ClientID + "').val().toUpperCase().trim() == ""63"") { $('#" + ddStateAbbrev.ClientID + "').css('color', 'DimGray'); } else { $('#" + ddStateAbbrev.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onblur,
                                                           "if($('#" + txtCounty.ClientID + "').val().toUpperCase().trim() == ""NEED COUNTY"") { $('#" + txtCounty.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCounty.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtCityName, ctlPageStartupScript.JsEventType.onkeyup,
                                                           "if($('#" + txtCityName.ClientID + "').val().toUpperCase().trim() == ""NEED CITY"") { $('#" + txtCityName.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCityName.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange,
                                                           "if($('#" + ddStateAbbrev.ClientID + "').val().toUpperCase().trim() == ""63"") { $('#" + ddStateAbbrev.ClientID + "').css('color', 'DimGray'); } else { $('#" + ddStateAbbrev.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtCounty, ctlPageStartupScript.JsEventType.onkeyup,
                                                           "if($('#" + txtCounty.ClientID + "').val().toUpperCase().trim() == ""NEED COUNTY"") { $('#" + txtCounty.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCounty.ClientID + "').css('color', 'Black'); }", True)

                'clear out fields when activating field - bug 26099 - 4/12/18 MLW
                Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onclick,
                                                           "if($('#" + txtStreetNum.ClientID + "').val().toUpperCase().trim() == ""NEED STREET #"") { $('#" + txtStreetNum.ClientID + "').val(''); }", False)
                Me.VRScript.CreateJSBinding(Me.txtStreetName, ctlPageStartupScript.JsEventType.onclick,
                                                           "if($('#" + txtStreetName.ClientID + "').val().toUpperCase().trim() == ""NEED STREET NAME"") { $('#" + txtStreetName.ClientID + "').val(''); }", False)
                Me.VRScript.CreateJSBinding(Me.txtAptSuite, ctlPageStartupScript.JsEventType.onclick,
                                                           "if($('#" + txtAptSuite.ClientID + "').val().toUpperCase().trim() == ""NEED IF APPLICABLE"") { $('#" + txtAptSuite.ClientID + "').val(''); }", False)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onclick,
                                                           "if($('#" + txtZipCode.ClientID + "').val().toUpperCase().trim() == ""00001"") { $('#" + txtZipCode.ClientID + "').val(''); }", False)
                Me.VRScript.CreateJSBinding(Me.txtCityName, ctlPageStartupScript.JsEventType.onclick,
                                                           "if($('#" + txtCityName.ClientID + "').val().toUpperCase().trim() == ""NEED CITY"") { $('#" + txtCityName.ClientID + "').val(''); }", False)
                'Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onclick,
                '                                           "if($('#" + ddStateAbbrev.ClientID + "').val().toUpperCase().trim() == ""63"") { $('#" + ddStateAbbrev.ClientID + "').val('0'); }", False)
                Me.VRScript.CreateJSBinding(Me.txtCounty, ctlPageStartupScript.JsEventType.onclick,
                                                           "if($('#" + txtCounty.ClientID + "').val().toUpperCase().trim() == ""NEED COUNTY"") { $('#" + txtCounty.ClientID + "').val('');}", False)
            End If
        End If

        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
        'Me.VRScript.CreateJSBinding(Me.btnOK, ctlPageStartupScript.JsEventType.onclick, "$('#" + dvSectionIAddress.ClientID + "').parent().hide();  return false;")
        Me.VRScript.CreateJSBinding(Me.btnCancel, ctlPageStartupScript.JsEventType.onclick, "$('#" + dvSectionIAddress.ClientID + "').parent().hide(); if ($('#" + Me.txtStreetNum.ClientID + "').val() != '' || $('#" + Me.txtStreetName.ClientID + "').val() != '' || $('#" + Me.txtAptSuite.ClientID + "').val() != '' || $('#" + Me.txtCityName.ClientID + "').val() != '' || $('#" + Me.txtZipCode.ClientID + "').val() != '' || $('#" + Me.txtCounty.ClientID + "').text()!= ''){$('#" + Me.MyAddAddressLinkClientId + "').text('View/Edit Address');}else{$('#" + Me.MyAddAddressLinkClientId + "').text('Add Address');}return false;")
    End Sub

    Public Overrides Sub LoadStaticData()
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            LoadStaticData()

            If Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                divBar.Visible = False
                divButtons.Visible = False
            Else
                divBar.Visible = True
                divButtons.Visible = True
            End If

            If MySectionCoverage IsNot Nothing Then
                If MyAdditionalInterest IsNot Nothing Then
                    Me.txtStreetNum.Text = Me.MyAdditionalInterest.Address.HouseNum
                    Me.txtStreetName.Text = Me.MyAdditionalInterest.Address.StreetName
                    Me.txtAptSuite.Text = Me.MyAdditionalInterest.Address.ApartmentNumber
                    Me.txtZipCode.Text = Me.MyAdditionalInterest.Address.Zip.ToMaxLength(5)
                    Me.txtCityName.Text = Me.MyAdditionalInterest.Address.City
                    'Added 4/12/18 for Bug 26103 MLW
                    If Me.MyAdditionalInterest.Address.StateId = "63" Then '63 is country wide (CW) for US
                        Me.ddStateAbbrev.SelectedValue = "63"
                    Else
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, Me.MyAdditionalInterest.Address.StateId)
                    End If
                    Me.txtCounty.Text = Me.MyAdditionalInterest.Address.County
                End If
            Else
                Me.ClearControl()
            End If

            'Updated 7/3/2019 for Home Endorsements Project Tasks 38911, 38912, 38916, 38925 MLW
            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                If Me.txtStreetNum.Text Is Nothing OrElse Me.txtStreetNum.Text = "" Then
                    Me.txtStreetNum.Text = "NEED STREET #"
                End If
                If Me.txtStreetName.Text Is Nothing OrElse Me.txtStreetName.Text = "" Then
                    Me.txtStreetName.Text = "NEED STREET NAME"
                End If
                If Me.txtAptSuite.Text Is Nothing OrElse Me.txtAptSuite.Text = "" Then
                    Me.txtAptSuite.Text = "NEED IF APPLICABLE"
                End If
                If Me.txtZipCode.Text Is Nothing OrElse Me.txtZipCode.Text = "" Then
                    Me.txtZipCode.Text = "00001"
                End If
                If Me.txtCityName.Text Is Nothing OrElse Me.txtCityName.Text = "" Then
                    Me.txtCityName.Text = "NEED CITY"
                End If

                Me.ddStateAbbrev.Items.Add(New ListItem("NEED STATE", "63")) '63 is country wide (CW) for US - Additional Interest address requires a valid id for a foreign key restraint on Diamond's end
                If Me.ddStateAbbrev.Text Is Nothing OrElse Me.ddStateAbbrev.Text = "" OrElse Me.ddStateAbbrev.Text = "0" Then
                    If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
                        Me.ddStateAbbrev.SelectedValue = "16"
                    Else
                        Me.ddStateAbbrev.SelectedValue = "63"
                    End If
                End If
                If Me.txtCounty.Text Is Nothing OrElse Me.txtCounty.Text = "" Then
                    Me.txtCounty.Text = "NEED COUNTY"
                End If
            Else
                If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
                    Me.ddStateAbbrev.SelectedValue = "16"
                End If
                'Added 7/11/2019 for Home Endorsements Project Tasks 38911, 38912, 38916 MLW
                Me.lblCity.CssClass = "showAsRequired"
                Me.lblCounty.CssClass = "showAsRequired"
                Me.lblStreetName.CssClass = "showAsRequired"
                Me.lblStreetNum.CssClass = "showAsRequired"
                Me.lblZipCode.CssClass = "showAsRequired"
                Me.txtState.CssClass = "showAsRequired"
            End If
            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                btnOK.Visible = False
                btnCancel.Visible = False
            End If
            Me.PopulateChildControls()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Public Overrides Function Save() As Boolean
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            If MySectionCoverage IsNot Nothing Then
                If MyAdditionalInterest IsNot Nothing Then
                    Me.MyAdditionalInterest.Address.HouseNum = Me.txtStreetNum.Text.Trim()
                    Me.MyAdditionalInterest.Address.StreetName = Me.txtStreetName.Text.Trim()
                    Me.MyAdditionalInterest.Address.ApartmentNumber = Me.txtAptSuite.Text.Trim()
                    If Me.txtStreetNum.Text.Trim() <> "NEED STREET #" AndAlso Me.txtStreetName.Text.Trim() <> "NEED STREET NAME" AndAlso Me.txtAptSuite.Text = "NEED IF APPLICABLE" Then
                        Me.MyAdditionalInterest.Address.ApartmentNumber = ""
                    End If
                    Me.MyAdditionalInterest.Address.Zip = Me.txtZipCode.Text()
                    Me.MyAdditionalInterest.Address.City = Me.txtCityName.Text.Trim()
                    Me.MyAdditionalInterest.Address.StateId = Me.ddStateAbbrev.SelectedValue
                    Me.MyAdditionalInterest.Address.County = Me.txtCounty.Text.Trim
                End If
            End If
            'Me.SaveChildControls()
        End If
        Return True
    End Function
    Public Sub InjectFormIntoAI(interest As QuickQuoteAdditionalInterest)
        If interest IsNot Nothing Then
            interest.Address.HouseNum = Me.txtStreetNum.Text.Trim()
            interest.Address.StreetName = Me.txtStreetName.Text.Trim()
            interest.Address.ApartmentNumber = Me.txtAptSuite.Text.Trim()
            If Me.txtStreetNum.Text.Trim() <> "NEED STREET #" AndAlso Me.txtStreetName.Text.Trim() <> "NEED STREET NAME" AndAlso Me.txtAptSuite.Text = "NEED IF APPLICABLE" Then
                interest.Address.ApartmentNumber = ""
            End If
            interest.Address.Zip = Me.txtZipCode.Text()
            interest.Address.City = Me.txtCityName.Text.Trim()
            interest.Address.StateId = Me.ddStateAbbrev.SelectedValue
            interest.Address.County = Me.txtCounty.Text.Trim
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            If Me.MySectionCoverage IsNot Nothing Then
                Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AdditionalInterestRelativeValidation(MyAdditionalInterest, valArgs.ValidationType, MySectionCoverage, Quote)
                If valList.Any() Then
                    For Each v In valList
                        Select Case v.FieldId
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressStreetNumber
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressStreetName
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressCity
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressState
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressSatetNotIndiana 'Added for Bug 26103 MLW
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressZipCode
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressCountyID
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCounty, v, accordList)
                        End Select
                    Next

                End If
            End If
            ' This copies the validation item up to its parent so they don't have to be in a sperate group

            'Updated 8/23/18 for multi state MLW
            'If Me.ParentVrControl.IsNotNull Then
            If Me.ParentVrControl IsNot Nothing Then
                Me.ParentVrControl.ValidationHelper.InsertFromOtherValidationHelper(Me.ValidationHelper)
            End If

            ' END OF COPY LOGIC
            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Sub ClearControl()
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            MyBase.ClearControl()
            Me.txtAptSuite.Text = ""
            Me.txtCityName.Text = ""
            Me.txtCounty.Text = ""
            Me.txtStreetName.Text = ""
            Me.txtStreetNum.Text = ""
            Me.txtZipCode.Text = ""
            Me.ddCityName.SelectedIndex = -1
        End If
    End Sub

    Protected Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            Me.Save_FireSaveEvent(True)
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
            Me.Save_FireSaveEvent(False)
        End If
    End Sub
End Class