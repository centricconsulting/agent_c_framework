Imports IFM.PrimativeExtensions

Public Class ctl_CGL_Classcode
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property
    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Public Property ClassCodeIndex As Int32
        Get
            Return ViewState.GetInt32("vs_ccIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_ccIndex") = value
        End Set
    End Property

    Public ReadOnly Property IsAtPolicyLevel As Boolean
        Get
            Return LocationIndex < 0
        End Get
    End Property

    Public ReadOnly Property IsAtLocationLevel As Boolean
        Get
            Return LocationIndex >= 0 AndAlso BuildingIndex < 0
        End Get
    End Property

    Public ReadOnly Property IsAtBuildingLevel As Boolean
        Get
            Return IsAtLocationLevel AndAlso BuildingIndex >= 0
        End Get
    End Property

    Public ReadOnly Property MyClassCode As QuickQuote.CommonObjects.QuickQuoteGLClassification
        Get
            If IsAtPolicyLevel Then
                'CGL
                Return Me.Quote.GLClassifications.GetItemAtIndex(Me.ClassCodeIndex) ' IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetAllPolicyAndLocationClassCodes(Me.Quote).GetItemAtIndex(Me.ClassCodeIndex)
            Else
                If IsAtBuildingLevel Then
                    If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(LocationIndex) AndAlso Me.Quote.Locations(LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                        ' ONly Personal Property and Personal Property of Others contain a class code Info
                        ' Need to know which it is
                    End If
                Else
                    If IsAtLocationLevel Then
                        If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(LocationIndex) AndAlso Me.Quote.Locations(LocationIndex).GLClassifications.HasItemAtIndex(Me.ClassCodeIndex) Then
                            Return Me.Quote.Locations(LocationIndex).GLClassifications(Me.ClassCodeIndex)
                        End If
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

        'Me.VRScript.CreateJSBinding(Me.ddAssignment, ctlPageStartupScript.JsEventType.onchange,
        '                            "if($('#" + Me.ddAssignment.ClientID + "').val() == '2'){$('#" + Me.ddAssignmentLocation.ClientID + "').show();}else{$('#" + Me.ddAssignmentLocation.ClientID + "').hide();}", True)



        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
        Me.divContents.Visible = Not Me.HideFromParent
        If Me.HideFromParent Then
            Me.lnkRemove.Visible = False
        Else
            Me.VRScript.StopEventPropagation(Me.lnkRemove.ClientID)
            Me.VRScript.CreatePseudoDisabledTextBox(Me.txtClassCode) ' you can't truely disable because you need the text that is set by script
            Me.VRScript.CreatePseudoDisabledTextBox(Me.txtClassCodeDescription) ' you can't truely disable because you need the text that is set by script
            Me.VRScript.CreatePseudoDisabledTextBox(Me.txtBasis) ' you can't truely disable because you need the text that is set by script

            If HttpContext.Current.Items("ccUiBindingIndex") Is Nothing Then
                HttpContext.Current.Items("ccUiBindingIndex") = 0
            Else
                HttpContext.Current.Items("ccUiBindingIndex") = CInt(HttpContext.Current.Items("ccUiBindingIndex")) + 1
            End If

            Me.VRScript.CreateJSBinding(Me.btnSearch, ctlPageStartupScript.JsEventType.onclick, "VrGlClassCodes.PerformSearch(" + HttpContext.Current.Items("ccUiBindingIndex").ToString() + ",$('#" + Me.ddearchType.ClientID + "').val(),$('#" + Me.txtSearchClassCode.ClientID + "').val(),'" + CInt(Me.Quote.LobType).ToString() + "','" + If(Me.Quote.ProgramTypeId.IsNumeric = False, "54", Me.Quote.ProgramTypeId) + "','" + Me.divSearchResults.ClientID + "'); return false;")

            Me.VRScript.AddVariableLine("VrGlClassCodes.UiBindings.push(new VrGlClassCodes.ClassCodeUiBinding(" + Me.ClassCodeIndex.ToString() + ",'" + Me.txtClassCode.ClientID + "','" + Me.txtClassCodeDescription.ClientID + "','" + Me.txtExposure.ClientID + "','" + Me.txtBasis.ClientID + "','" + Me.lblpremBaseShort.ClientID + "','" + Me.divArating.ClientID + "','" + Me.txtARatePrem.ClientID + "','" + Me.txtARateProd.ClientID + "','" + Me.txtFootNote.ClientID + "'));")

            Me.VRScript.AddScriptLine("VrGlClassCodes.PopulateClassCodeByClassCodeNumber_Limited({0},$('#{1}').val(),{2},{3});".FormatIFM(HttpContext.Current.Items("ccUiBindingIndex").ToString(), Me.txtClassCode.ClientID, CInt(Me.Quote.LobType).ToString(), If(Me.Quote.ProgramTypeId.IsNumeric = False, "54", Me.Quote.ProgramTypeId)), True)

        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.MyClassCode.IsNotNull Then
            If Me.IsAtPolicyLevel Then
                Me.lblAccordHeader.Text = "Class Code #{0} - Policy Level".FormatIFM(Me.ClassCodeIndex + 1)
            Else
                If IsAtBuildingLevel Then
                    Me.lblAccordHeader.Text = "Class Code #{0} - Location #{1} Building #{2}".FormatIFM(Me.ClassCodeIndex + 1, Me.LocationIndex + 1, Me.BuildingIndex + 1)
                Else
                    'Is At Location
                    Me.lblAccordHeader.Text = "Class Code #{0} - Location #{1}".FormatIFM(Me.ClassCodeIndex + 1, Me.LocationIndex + 1)
                End If
            End If

            Me.txtClassCode.Text = MyClassCode.ClassCode
            Me.txtClassCodeDescription.Text = MyClassCode.ClassDescription
            Me.txtClassCodeDescription.ToolTip = MyClassCode.ClassDescription
            Me.txtExposure.Text = MyClassCode.PremiumExposure
            Me.txtBasis.Text = MyClassCode.PremiumBase
            Me.txtBasis.ToolTip = MyClassCode.PremiumBase
            Me.lblpremBaseShort.Text = MyClassCode.PremiumBaseShort
            Me.txtARatePrem.Text = MyClassCode.ManualElpaRate_Premises
            Me.txtARateProd.Text = MyClassCode.ManualElpaRate_Products
            Me.txtFootNote.Text = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetFootNote(MyClassCode.ClassCode, Me.Quote.ProgramTypeId, Me.Quote.LobType)

            'Me.ddAssignmentLocation.Items.Clear()
            'Dim i As Int32 = 0
            'If Me.Quote.Locations.IsLoaded() Then
            '    For Each l In Me.Quote.Locations
            '        Me.ddAssignmentLocation.Items.Add(New ListItem(l.Address.ToIFMAddressString(), i))
            '        i += 1
            '    Next
            'End If


        End If

        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()

        If Me.MyClassCode.IsNull Then
            AddNew()
        End If

        MyClassCode.ClassCode = Me.txtClassCode.Text.Trim()
        MyClassCode.ClassDescription = Me.txtClassCodeDescription.Text.Trim()
        MyClassCode.PremiumExposure = Me.txtExposure.Text.Trim()
        MyClassCode.PremiumBase = Me.txtBasis.Text.Trim()
        MyClassCode.PremiumBaseShort = Me.lblpremBaseShort.Text.Trim()
        MyClassCode.ManualElpaRate_Premises = Me.txtARatePrem.Text.Trim()
        MyClassCode.ManualElpaRate_Products = Me.txtARateProd.Text.Trim()

        Return True
    End Function

    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        Me.Save_FireSaveEvent(False)
        AddNew()
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub

    Private Sub AddNew()
        If Me.IsAtPolicyLevel Then
            Me.Quote.GLClassifications.AddNew()
        Else
            If Me.IsAtLocationLevel Then
                If Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                    Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).GLClassifications.AddNew()
                End If
            Else
                'BUILDING LEVEL
                ' what Location then what Building then what coverage
                If Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                    If Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                        'Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex).[CoverageType].AddNew()
                    End If
                End If

            End If
        End If
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        Me.Save_FireSaveEvent(False)
        If Me.MyClassCode.IsNotNull Then
            If Me.IsAtPolicyLevel Then
                Me.Quote.GLClassifications.RemoveAt(Me.ClassCodeIndex)
            Else
                If IsAtLocationLevel Then
                    Me.Quote.Locations(Me.LocationIndex).GLClassifications.RemoveAt(Me.ClassCodeIndex)
                Else
                    If IsAtBuildingLevel Then
                        'have to know the coverage  you are using to delete it
                        'Me.Quote.Locations(Me.LocationIndex).Buildings(Me.BuildingIndex).PersPropOfOthers_ClassificationCode = Nothing
                    End If
                End If
            End If
        End If
        Me.Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent(False)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

End Class