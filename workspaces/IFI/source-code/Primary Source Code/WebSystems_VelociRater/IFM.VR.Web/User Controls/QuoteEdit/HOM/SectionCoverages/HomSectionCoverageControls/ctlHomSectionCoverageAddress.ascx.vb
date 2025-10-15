Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctlHomSectionCoverageAddress
    Inherits ctlSectionCoverageControlBase

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
                'If Me.MySectionCoverage.Address.IsNotNull() Then
                If Me.MySectionCoverage.Address IsNot Nothing Then
                    Return IFM.PrimativeExtensions.OneorMoreIsNotEmpty(Me.MySectionCoverage.Address.HouseNum, Me.MySectionCoverage.Address.StreetName,
                                                                                   Me.MySectionCoverage.Address.ApartmentNumber, Me.MySectionCoverage.Address.Zip,
                                                                                   Me.MySectionCoverage.Address.City, Me.MySectionCoverage.Address.County)
                End If
            End If
            Return False
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
        'Added 2/23/18 for HOM Upgrade MLW
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'Updated 7/3/2019 for Home Endorsements Project Tasks 38903, 38904, 38905, 38907, 38909, 38910, 38925 MLW
            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.SectionCoverageIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None Then
                'set default text and formatting
                Me.VRScript.CreateJSBinding(Me.txtStreetNum, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtStreetNum.ClientID + "').val().toUpperCase().trim() == ""NEED STREET #"") { $('#" + txtStreetNum.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtStreetNum.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtStreetName, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtStreetName.ClientID + "').val().toUpperCase().trim() == ""NEED STREET NAME"") { $('#" + txtStreetName.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtStreetName.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtAptSuite, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtAptSuite.ClientID + "').val().toUpperCase().trim() == ""NEED IF APPLICABLE"") { $('#" + txtAptSuite.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtAptSuite.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtZipCode.ClientID + "').val().toUpperCase().trim() == ""00001"") { $('#" + txtZipCode.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtZipCode.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onchange,
                                                       "if($('#" + txtCityName.ClientID + "').val().toUpperCase().trim() == ""NEED CITY"") { $('#" + txtCityName.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCityName.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onchange,
                                                       "if($('#" + ddStateAbbrev.ClientID + "').val().toUpperCase().trim() == ""999"") { $('#" + ddStateAbbrev.ClientID + "').css('color', 'DimGray'); } else { $('#" + ddStateAbbrev.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onchange,
                                                       "if($('#" + txtCounty.ClientID + "').val().toUpperCase().trim() == ""NEED COUNTY"") { $('#" + txtCounty.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCounty.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtCityName, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtCityName.ClientID + "').val().toUpperCase().trim() == ""NEED CITY"") { $('#" + txtCityName.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCityName.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange,
                                                       "if($('#" + ddStateAbbrev.ClientID + "').val().toUpperCase().trim() == ""999"") { $('#" + ddStateAbbrev.ClientID + "').css('color', 'DimGray'); } else { $('#" + ddStateAbbrev.ClientID + "').css('color', 'Black'); }", True)
                Me.VRScript.CreateJSBinding(Me.txtCounty, ctlPageStartupScript.JsEventType.onkeyup,
                                                       "if($('#" + txtCounty.ClientID + "').val().toUpperCase().trim() == ""NEED COUNTY"") { $('#" + txtCounty.ClientID + "').css('color', 'DimGray'); } else { $('#" + txtCounty.ClientID + "').css('color', 'Black'); }", True)
                'clear out fields when activating field - bug 26099, 26092 - 4/12/18 MLW
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
                '                                           "if($('#" + ddStateAbbrev.ClientID + "').val().toUpperCase().trim() == ""999"") { $('#" + ddStateAbbrev.ClientID + "').val('0'); }", False)
                Me.VRScript.CreateJSBinding(Me.txtCounty, ctlPageStartupScript.JsEventType.onclick,
                                                       "if($('#" + txtCounty.ClientID + "').val().toUpperCase().trim() == ""NEED COUNTY"") { $('#" + txtCounty.ClientID + "').val('');}", False)
            End If
        End If

        Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
        'Me.VRScript.CreateJSBinding(Me.btnOK, ctlPageStartupScript.JsEventType.onclick, "$('#" + dvSectionIAddress.ClientID + "').parent().hide();  return false;")
        Me.VRScript.CreateJSBinding(Me.btnCancel, ctlPageStartupScript.JsEventType.onclick, "$('#" + dvSectionIAddress.ClientID + "').parent().hide(); if ($('#" + Me.txtStreetNum.ClientID + "').val() != '' || $('#" + Me.txtStreetName.ClientID + "').val() != '' || $('#" + Me.txtAptSuite.ClientID + "').val() != '' || $('#" + Me.txtCityName.ClientID + "').val() != '' || $('#" + Me.txtZipCode.ClientID + "').val() != '' || $('#" + Me.txtCounty.ClientID + "').text()!= ''){$('#" + Me.MyAddAddressLinkClientId + "').text('View/Edit Address');}else{$('#" + Me.MyAddAddressLinkClientId + "').text('Add Address');}return false;")
    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If MySectionCoverage IsNot Nothing Then
            Me.txtStreetNum.Text = Me.MySectionCoverage.Address.HouseNum
            Me.txtStreetName.Text = Me.MySectionCoverage.Address.StreetName
            Me.txtAptSuite.Text = Me.MySectionCoverage.Address.ApartmentNumber
            Me.txtZipCode.Text = Me.MySectionCoverage.Address.Zip.ToMaxLength(5)
            Me.txtCityName.Text = Me.MySectionCoverage.Address.City
            'Added 4/12/18 for Bug 26099 MLW
            If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If MySectionCoverage.Address.StateId = "999" Then
                    Me.ddStateAbbrev.SelectedValue = "999"
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, Me.MySectionCoverage.Address.StateId)
                End If
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, Me.MySectionCoverage.Address.StateId)
            End If
            Me.txtCounty.Text = Me.MySectionCoverage.Address.County
        Else
            Me.ClearControl()
        End If

        'Added 2/23/18 MLW for HOM Upgrade MLW
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'Updated 7/12/2019 for Home Endorsements Project Tasks 38903, 38904, 38905, 38907, 38909, 38910, 38925 MLW
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
                Me.ddStateAbbrev.Items.Add(New ListItem("NEED STATE", "999"))
                If Me.ddStateAbbrev.Text Is Nothing OrElse Me.ddStateAbbrev.Text = "" OrElse Me.ddStateAbbrev.Text = "0" Then
                    Me.ddStateAbbrev.SelectedValue = "999"
                    Me.ddStateAbbrev.ForeColor = Drawing.Color.DimGray
                Else
                    Me.ddStateAbbrev.ForeColor = Drawing.Color.Black
                End If
                If Me.txtCounty.Text Is Nothing OrElse Me.txtCounty.Text = "" Then
                    Me.txtCounty.Text = "NEED COUNTY"
                End If
            Else
                'Me.lblAptSuite.CssClass = "showAsRequired" this is not required on app or endorsement
                Me.lblCity.CssClass = "showAsRequired"
                Me.lblCounty.CssClass = "showAsRequired"
                Me.lblStreetName.CssClass = "showAsRequired"
                Me.lblStreetNum.CssClass = "showAsRequired"
                Me.lblZipCode.CssClass = "showAsRequired"
                Me.txtState.CssClass = "showAsRequired" 'why a label is prefixed with txt I don't know Matt A 7-1-2019
            End If
        End If
        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            btnOK.Visible = False
            btnCancel.Visible = False
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If MySectionCoverage IsNot Nothing Then
            Me.MySectionCoverage.Address.HouseNum = Me.txtStreetNum.Text.Trim()
            Me.MySectionCoverage.Address.StreetName = Me.txtStreetName.Text.Trim()
            Me.MySectionCoverage.Address.ApartmentNumber = Me.txtAptSuite.Text.Trim()
            'Added 2/23/18 MLW for HOM Upgrade MLW
            If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If Me.txtStreetNum.Text.Trim() <> "NEED STREET #" AndAlso Me.txtStreetName.Text.Trim() <> "NEED STREET NAME" AndAlso Me.txtAptSuite.Text = "NEED IF APPLICABLE" Then
                    Me.MySectionCoverage.Address.ApartmentNumber = ""
                End If
            End If
            Me.MySectionCoverage.Address.Zip = Me.txtZipCode.Text()
            Me.MySectionCoverage.Address.City = Me.txtCityName.Text.Trim()
            Me.MySectionCoverage.Address.StateId = Me.ddStateAbbrev.SelectedValue
            Me.MySectionCoverage.Address.County = Me.txtCounty.Text.Trim
        End If
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = Me.CoverageName
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If Me.MySectionCoverage IsNot Nothing Then
            Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
            If valList.Any() Then
                For Each v In valList
                    Select Case v.FieldId
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressStreetNumber
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressStreetName
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressAptNumber
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAptSuite, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressCity
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressState
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressSatetNotIndiana
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressZipCode
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                        Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressCountyID
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

    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtAptSuite.Text = ""
        Me.txtCityName.Text = ""
        Me.txtCounty.Text = ""
        Me.txtStreetName.Text = ""
        Me.txtStreetNum.Text = ""
        Me.txtZipCode.Text = ""
        Me.ddCityName.SelectedIndex = -1
    End Sub

    Protected Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Save_FireSaveEvent(False)
    End Sub
End Class