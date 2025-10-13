Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL
Imports IFM.VR.Common.Helpers.CL
Imports IFM.VR.Common.Helpers.QuickQuoteObjectHelper
Imports QuickQuote.CommonObjects

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

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            Return Quote.Locations(LocationIndex)
        End Get
    End Property

    Public Property ClassCodeType As String
        Get
            Return ViewState("vs_ccType")?.ToString
        End Get
        Set(value As String)
            ViewState("vs_ccType") = value
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
            '  If there's no class code type then it's a new class code and we're at policy level
            If ClassCodeType Is Nothing Then Return True
            Return ClassCodeType.Substring(0, 1).ToUpper = "P"
        End Get
    End Property

    Public ReadOnly Property IsAtLocationLevel As Boolean
        Get
            If ClassCodeType Is Nothing Then Return False
            Return ClassCodeType.Substring(0, 1).ToUpper = "L"
        End Get
    End Property

    Public Property OriginalLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_ccOriginalLocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_ccOriginalLocationIndex") = value
        End Set
    End Property

    Private ReadOnly Property NumberOfStatesOnQuote() As Integer
        Get
            Dim cnt As Integer = 0
            Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
            Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)

            If INQuote IsNot Nothing Then cnt += 1
            If ILQuote IsNot Nothing Then cnt += 1

            Return cnt
        End Get
    End Property

    Public ReadOnly Property MyClassCode As QuickQuote.CommonObjects.QuickQuoteGLClassification
        Get
            If IsAtPolicyLevel Then
                'CGL
                Return Me.Quote.GLClassifications.GetItemAtIndex(Me.ClassCodeIndex) ' IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetAllPolicyAndLocationClassCodes(Me.Quote).GetItemAtIndex(Me.ClassCodeIndex)
            Else
                If IsAtLocationLevel Then
                    If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(LocationIndex) AndAlso Me.Quote.Locations(LocationIndex).GLClassifications.HasItemAtIndex(Me.ClassCodeIndex) Then
                        Return Me.Quote.Locations(LocationIndex).GLClassifications(Me.ClassCodeIndex)
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property MyClassCodeState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Get
            If MyClassCode IsNot Nothing Then
                If MyClassCode.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                    Return MyClassCode.QuickQuoteState
                Else
                    If MyClassCode.QuoteStateTakenFrom <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        Return MyClassCode.QuoteStateTakenFrom
                    End If
                End If
            End If
            ' If we got here state was not found on either property so return none
            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
        End Get
    End Property

    Public Property FirstClassificationInList As Boolean
        Get
            Dim val As Boolean = False
            Boolean.TryParse(ViewState("vs_ccFirstInList")?.ToString, val)
            Return val
        End Get
        Set(value As Boolean)
            ViewState("vs_ccFirstInList") = value
        End Set
    End Property

    Public Property SwitchedType As Boolean
        Get
            If ViewState("vs_SwitchedType") IsNot Nothing Then
                Return CBool(ViewState("vs_SwitchedType"))
            Else
                Return False
            End If
        End Get
        Set(value As Boolean)
            ViewState("vs_SwitchedType") = value
        End Set
    End Property

    Public Event AddNewClassCode()
    Public Event DeleteClassCode(ByVal PolicyOrLocation As String, ByVal UniqueId As String)
    Public Event RepopulateClassCodes()

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateJSBinding(Me.ddAssignment, ctlPageStartupScript.JsEventType.onchange,
                                    "if($('#" + Me.ddAssignment.ClientID + "').val() == '2'){$('#" + Me.divLoc.ClientID + "').show();}else{$('#" + Me.divLoc.ClientID + "').hide();}", True)

        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnAdd.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID)

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

            Me.VRScript.CreateJSBinding(ddAssignment.ClientID, ctlPageStartupScript.JsEventType.onchange, "Cgl.AssignmentDDLChanged('" + ddAssignment.ClientID & "','" & trAssignmentInfoRow.ClientID & "','" & ddState.ClientID & "','" & ddAssignmentLocation.ClientID & "');")
            'Me.VRScript.CreateJSBinding(ddAssignment.ClientID, ctlPageStartupScript.JsEventType.onchange, "Cgl.AssignmentDDLChanged('" + ddAssignment.ClientID & "','" & trAssignmentInfoRow.ClientID + "');")

            Dim IsIneligibleMotelHotelCodes As Boolean = HotelMotelRemovedRisks.IsHotelMotelRemovedRisksAvailable(Quote)
            Dim isClassCodeAssignmentAvailable As Boolean = ClassCodeAssignmentHelper.IsClassCodeAssignmentAvailable(Quote)
            Me.VRScript.CreateJSBinding(Me.btnSearch, ctlPageStartupScript.JsEventType.onclick, "VrGlClassCodes.PerformSearch(" & HttpContext.Current.Items("ccUiBindingIndex").ToString() & ",$('#" & Me.ddSearchType.ClientID & "').val(),$('#" & Me.txtSearchClassCode.ClientID & "').val(),'" & CInt(Me.Quote.LobType).ToString() & "','" & If(Me.Quote.ProgramTypeId.IsNumeric = False, "54", Me.Quote.ProgramTypeId) & "','" & Me.divSearchResults.ClientID & "','" & IsIneligibleMotelHotelCodes & "','" & isClassCodeAssignmentAvailable.ToString().ToLower() & "'); return false;")
            Me.VRScript.AddVariableLine("VrGlClassCodes.UiBindings.push(new VrGlClassCodes.ClassCodeUiBinding(" + Me.ClassCodeIndex.ToString() + ",'" + Me.txtClassCode.ClientID + "','" + Me.txtClassCodeDescription.ClientID + "','" + Me.ddAssignment.ClientID + "','" + Me.txtExposure.ClientID + "','" + Me.txtBasis.ClientID + "','" + Me.lblpremBaseShort.ClientID + "','" + Me.trAratingRow.ClientID + "','" + Me.txtARatePrem.ClientID + "','" + Me.txtARateProd.ClientID + "','" + txtPremiumDescription.ClientID + "','" + divClassInfo.ClientID + "','" + hdnCheckAratePrem.ClientID + "','" + hdnCheckArateProd.ClientID + "','" + divFootnote.ClientID + "','" + tdAratePremColumn.ClientID + "','" + tdArateProdColumn.ClientID + "','" + trAratingInfoRow.ClientID + "','" + hdnRemoveEPLI.ClientID + "'));")
            Me.VRScript.AddScriptLine("VrGlClassCodes.PopulateClassCodeByClassCodeNumber_Limited({0},$('#{1}').val(),{2},{3});".FormatIFM(HttpContext.Current.Items("ccUiBindingIndex").ToString(), Me.txtClassCode.ClientID, CInt(Me.Quote.LobType).ToString(), If(Me.Quote.ProgramTypeId.IsNumeric = False, "54", Me.Quote.ProgramTypeId)), True)

            Me.VRScript.CreateJSBinding(Me.txtSearchClassCode, ctlPageStartupScript.JsEventType.onkeydown, "VrGlClassCodes.EnterLogic(event, '" + Me.btnSearch.ClientID + "');")
            Me.VRScript.CreateJSBinding(ddAssignmentLocation.ClientID, ctlPageStartupScript.JsEventType.onchange, "Cgl.ClassCodeAssignedLocationChanged('" + ddAssignment.ClientID & "','" & trAssignmentInfoRow.ClientID & "','" & hdnStateId.ClientID & "');")

            ddState.Attributes.Add("onchange", "Cgl.ClassCodeStateDropdownChanged('" & ddState.ClientID & "','" & hdnStateId.ClientID & "', '', '', '');")
            ddAssignmentLocation.Attributes.Add("onchange", "Cgl.ClassCodeAssignedLocationChanged('" & ddAssignmentLocation.ClientID & "','" & ddState.ClientID & "','" & hdnStateId.ClientID & "');")
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Function ClassCodeIsSaved() As Boolean
        If Quote IsNot Nothing Then
            ' Check quote level classifications
            If Quote.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                    If cc.ClassCode <> "" AndAlso cc.ClassCode <> "9999999" AndAlso
                                    cc.ClassDescription <> "" AndAlso
                                    cc.PremiumExposure <> "" AndAlso IsNumeric(cc.PremiumExposure) Then
                        Return True
                    End If
                Next
            End If

            ' Check Location level classifications
            If Quote.Locations IsNot Nothing Then
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If LOC.GLClassifications IsNot Nothing Then
                        For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In LOC.GLClassifications
                            If cc.ClassCode <> "" AndAlso cc.ClassCode <> "9999999" AndAlso
                            cc.ClassDescription <> "" AndAlso
                            cc.PremiumExposure <> "" AndAlso IsNumeric(cc.PremiumExposure) Then
                                Return True
                            End If
                        Next
                    End If
                Next
            End If
        End If

        Return False
    End Function

    Public Sub HideAddButton()
        If ClassCodeIsSaved() Then lnkBtnAdd.Visible = True Else lnkBtnAdd.Visible = False
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Sub HideRemoveButton()
        Me.lnkRemove.Visible = False
    End Sub
    Public Sub ShowRemoveButton()
        Me.lnkRemove.Visible = True
    End Sub

    Private Sub UpdateAccordHeader()
        Dim st As String = Nothing
        Select Case hdnStateId.Value
            Case "15"
                st = "(IL)"
                Exit Select
            Case "16"
                st = "(IN)"
                Exit Select
            Case "36"
                st = "(OH)"
                Exit Select
            Case Else
                st = ""
        End Select
        If Me.IsAtPolicyLevel Then
            'Me.lblAccordHeader.Text = "Class Code #{0} - Policy " & st.FormatIFM(Me.ClassCodeIndex + 1)
            Me.lblAccordHeader.Text = "Class Code #" & (ClassCodeIndex + 1).ToString & " - Policy " & st
        Else
            'Is At Location
            'Me.lblAccordHeader.Text = "Class Code #{0} - Location #{1} " & st.FormatIFM(Me.ClassCodeIndex + 1, Me.LocationIndex + 1)
            Me.lblAccordHeader.Text = "Class Code #" & (ClassCodeIndex + 1).ToString & " - Location #" & (LocationIndex + 1).ToString & st
        End If
    End Sub

    Private Sub LoadStatesDropdown()
        Dim li As ListItem = Nothing
        Dim selectedid As String = Nothing

        ' Save the current selection
        If ddState.SelectedItem IsNot Nothing Then selectedid = ddState.SelectedValue

        ' Load the state dropdown
        ddState.Items.Clear()

        ' Add a blank item if more than one state on the quote
        If NumberOfStatesOnQuote > 1 Then
            li = New ListItem()
            li.Value = ""
            li.Text = ""
            ddState.Items.Add(li)
        End If

        If SubQuotesContainsState("IL") Then
            li = New ListItem()
            li.Value = "15"
            li.Text = "IL - Illinois"
            ddState.Items.Add(li)
        End If

        If SubQuotesContainsState("IN") Then
            li = New ListItem()
            li.Value = "16"
            li.Text = "IN - Indiana"
            ddState.Items.Add(li)
        End If

        If SubQuotesContainsState("OH") Then
            li = New ListItem()
            li.Value = "36"
            li.Text = "OH - Ohio"
            ddState.Items.Add(li)
        End If

        If MyClassCode.ClassCode = "9999999" Then
            ' Needs to have a blank option if New and doesn't have one
            If ddState.Items.Count < 3 Then
                li = New ListItem()
                li.Value = ""
                li.Text = ""
                ddState.Items.Add(li)
            End If

            ddState.SelectedIndex = -1
            ddState.SelectedValue = ""
        Else
            ' restore the original selection if appropriate
            If selectedid IsNot Nothing Then
                ddState.SelectedIndex = -1
                ddState.SelectedValue = selectedid
            End If
        End If

        hdnStateId.Value = ddState.SelectedValue

        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        If Me.MyClassCode.IsNotNull Then
            If ddState.Items.Count = 0 Then LoadStatesDropdown() ' Only load states when they have't been loaded

            If MyClassCode.ClassCode <> "9999999" Then Me.txtClassCode.Text = MyClassCode.ClassCode
            If MyClassCode.ClassCode = "9999999" Then hdnClassCode.Value = MyClassCode.ClassCode Else hdnClassCode.Value = Nothing
            If MyClassCode.ClassCode = "9999999" Then
                divClassInfo.Attributes.Add("style", "display:none")
            Else
                divClassInfo.Attributes.Add("style", "display:''")
            End If

            Me.txtClassCodeDescription.Text = MyClassCode.ClassDescription
            Me.txtClassCodeDescription.ToolTip = MyClassCode.ClassDescription
            Me.txtExposure.Text = MyClassCode.PremiumExposure
            Me.txtBasis.Text = MyClassCode.PremiumBase
            Me.txtBasis.ToolTip = MyClassCode.PremiumBase
            Me.lblpremBaseShort.Text = MyClassCode.PremiumBaseShort
            Me.txtARatePrem.Text = MyClassCode.ManualElpaRate_Premises
            Me.txtARateProd.Text = MyClassCode.ManualElpaRate_Products
            'Me.txtFootNote.Text = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetFootNote(MyClassCode.ClassCode, Me.Quote.ProgramTypeId, Me.Quote.LobType)
            Select Case Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    Me.divFootnote.InnerHtml = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetFootNote(MyClassCode.ClassCode, SubQuoteFirst.ProgramTypeId.TryToGetInt32, Me.Quote.LobType, currentPremiumBaseShort:=Me.lblpremBaseShort.Text)
                    'Me.divFootnote.InnerHtml = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetFootNote(MyClassCode.ClassCode, Me.Quote.ProgramTypeId, Me.Quote.LobType)
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Me.divFootnote.InnerHtml = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetFootNote(MyClassCode.ClassCode, 55, Quote.LobType, currentPremiumBaseShort:=Me.lblpremBaseShort.Text)  ' CGL Standard = 54; CGL Preferred = 55
                    'Me.divFootnote.InnerHtml = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetFootNote(MyClassCode.ClassCode, 55, Me.Quote.LobType)  ' CGL Standard = 54; CGL Preferred = 55
                    Exit Select
            End Select
            txtPremiumDescription.Text = MyClassCode.PremiumBase

            LoadLocationsList()
        End If

        If IsAtLocationLevel Then
            ' LOCATION LEVEL 
            ddAssignment.SelectedValue = "2"
            If ClassCodeAssignmentHelper.IsClassCodeAssignmentAvailable(Quote) Then
                'Force Class Code Assignment drop down to Location and disable if Premium Exposure Description begins with "Area". Full description would be "Area, Products/Completed Operations". Sometimes this comes back truncated at Opera or Op. Do not know if there are more variations.
                If MyClassCode IsNot Nothing AndAlso MyClassCode.PremiumBase.Length > 3 AndAlso Left(MyClassCode.PremiumBase, 4).ToUpper = "AREA" Then
                    ddAssignment.Enabled = False
                Else
                    ddAssignment.Enabled = True
                End If
            Else
                ddAssignment.Enabled = True
            End If
            divLoc.Attributes.Add("style", "display:''")
            ddAssignmentLocation.SelectedIndex = LocationIndex
            OriginalLocationIndex = ddAssignmentLocation.SelectedIndex
            trAssignmentInfoRow.Attributes.Add("style", "display:''")
            ' Set the state dropdown based on the selected location
            ddState.SelectedValue = MyLocation.Address.StateId
            ddState.Attributes.Add("disabled", "true")
            hdnStateId.Value = ddState.SelectedValue
        Else
            ' POLICY LEVEL
            ddAssignment.SelectedValue = "1"
            ddAssignment.Enabled = True
            divLoc.Attributes.Add("style", "display:none")
            ddAssignmentLocation.SelectedIndex = -1
            trAssignmentInfoRow.Attributes.Add("style", "display:none")
            OriginalLocationIndex = -1

            Select Case MyClassCodeState
                Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                    ddState.SelectedIndex = -1
                    ddState.SelectedValue = "15"
                    hdnStateId.Value = "15"
                    Exit Select
                Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                    ddState.SelectedIndex = -1
                    ddState.SelectedValue = "16"
                    hdnStateId.Value = "16"
                Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                    ddState.SelectedIndex = -1
                    ddState.SelectedValue = "36"
                    hdnStateId.Value = "36"
                Case Else
                    Exit Select
            End Select
            ddState.Attributes.Remove("disabled")
        End If

        UpdateAccordHeader()

        Me.PopulateChildControls()
    End Sub

    Public Sub LoadLocationsList()
        Dim selndx = ddAssignmentLocation.SelectedIndex

        Me.ddAssignmentLocation.Items.Clear()
        Dim i As Int32 = 0
        If Me.Quote.Locations.IsLoaded() Then
            For Each l In Me.Quote.Locations
                Me.ddAssignmentLocation.Items.Add(New ListItem(l.Address.ToIFMAddressString(), i.ToString))
                i += 1
            Next
        End If

        ' Re-select the same item that was selected before we updated the list
        If selndx >= 0 AndAlso ddAssignment.Items.Count >= selndx + 1 Then
            ddAssignmentLocation.SelectedIndex = selndx
        End If

        LoadStatesDropdown()

        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()

        If Me.MyClassCode.IsNull Then Return False

        ' DETERMINE IF THE LOCATION CHANGED
        If IsAtLocationLevel() AndAlso OriginalLocationIndex <> -1 Then
            If OriginalLocationIndex <> ddAssignmentLocation.SelectedIndex Then
                ' Location changed
                Dim NCC As QuickQuote.CommonObjects.QuickQuoteGLClassification = Nothing

                ' Remove the original Location ClassCode
                If Quote.Locations(OriginalLocationIndex).GLClassifications.HasItemAtIndex(ClassCodeIndex) Then
                    NCC = Quote.Locations(OriginalLocationIndex).GLClassifications(ClassCodeIndex)
                    Quote.Locations(OriginalLocationIndex).GLClassifications.RemoveAt(ClassCodeIndex)
                End If

                ' Create the new class code on the selected location (IF LOCATION ASSIGNMENT IS SELECTED)
                ' We handle the switch from location to Policy below
                If ddAssignment.SelectedValue.TryToGetInt32 = 2 Then
                    If Quote.Locations(ddAssignmentLocation.SelectedIndex).GLClassifications Is Nothing Then Quote.Locations(ddAssignmentLocation.SelectedIndex).GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
                    Quote.Locations(ddAssignmentLocation.SelectedIndex).GLClassifications.Add(NCC)
                End If
            End If
        End If

        ' DETERMINE OF THE TYPE (POLICY/LOCATION) CHANGED
        SwitchedType = False
        If ddAssignment.SelectedValue = "2" AndAlso ClassCodeType = "P" Then
            ' LOCATION
            ' Class Code Type of Location is assigned but Policy is on the class code - 
            ' Switch it from a Policy to Location class code

            ' Remove the new class code from the policy level class codes
            For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications
                If cc.UniqueIdentifier = MyClassCode.UniqueIdentifier Then
                    Quote.GLClassifications.RemoveAt(ClassCodeIndex)
                    Exit For
                End If
            Next

            ' Make a new Location Class Code
            If Quote.Locations(ddAssignmentLocation.SelectedValue.TryToGetInt32).GLClassifications Is Nothing Then Quote.Locations(ddAssignmentLocation.SelectedValue.TryToGetInt32).GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
            Quote.Locations(ddAssignmentLocation.SelectedValue.TryToGetInt32).GLClassifications.AddNew()

            ' Set the class code type to Location
            ClassCodeType = "L"

            ' Set the Classification Index
            ClassCodeIndex = Quote.Locations(ddAssignmentLocation.SelectedValue.TryToGetInt32).GLClassifications.Count - 1

            ' Set the Location Index
            LocationIndex = ddAssignmentLocation.SelectedValue.TryToGetInt32

            SwitchedType = True
        Else
            If ddAssignment.SelectedValue = "1" AndAlso ClassCodeType = "L" Then
                ' POLICY
                ' Class Code Type of Policy is assigned but Location is on the class code - 
                ' Switch it from a Location to Policy class code

                ' Remove the new class code from the location level class codes
                For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.Locations(LocationIndex).GLClassifications
                    If MyClassCode.UniqueIdentifier IsNot Nothing AndAlso cc.UniqueIdentifier = MyClassCode.UniqueIdentifier Then
                        Quote.Locations(LocationIndex).GLClassifications.RemoveAt(ClassCodeIndex)
                        Exit For
                    End If
                Next

                ' Make a new Policy Class Code
                If Quote.GLClassifications Is Nothing Then Quote.GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
                Quote.GLClassifications.AddNew()

                ' Set the class code type to Policy
                ClassCodeType = "P"

                ' Set the Classification Index
                ClassCodeIndex = Quote.GLClassifications.Count - 1

                ' Set the Location Index to -1
                LocationIndex = -1

                SwitchedType = True
            End If
        End If

        ' If the class code can't have EPLI, remove it from the quote
        If hdnRemoveEPLI.Value IsNot Nothing AndAlso hdnRemoveEPLI.Value.Trim <> "" Then
            IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(Me.Quote, False)
        End If

        ' Populate the class code fields with the values on the form

        ' If at location level be sure to set the location index
        If IsAtLocationLevel() Then
            LocationIndex = ddAssignmentLocation.SelectedValue.TryToGetInt32
        Else
            LocationIndex = -1
        End If

        ' If at the location level we need to set the state dropdowns again here because they lose their value for some reason when they're disabled
        If IsAtLocationLevel() AndAlso ddState.Text.Trim = "" Then
            If hdnStateId.Value IsNot Nothing AndAlso hdnStateId.Value <> "" Then
                ddState.SelectedValue = hdnStateId.Value
            End If
        End If

        If txtClassCode.Text = "" AndAlso (hdnClassCode.Value IsNot Nothing AndAlso hdnClassCode.Value <> "") Then
            ' If this is an empty class code, the hidden field will contain '9999999' and should be saved back to the object that way 
            ' because if you don't save a class code in the ClassCode on the quote it won't save.
            MyClassCode.ClassCode = hdnClassCode.Value.ToString
        Else
            MyClassCode.ClassCode = Me.txtClassCode.Text.Trim()
        End If
        MyClassCode.ClassDescription = Me.txtClassCodeDescription.Text.Trim()
        MyClassCode.PremiumExposure = Me.txtExposure.Text.Trim()
        MyClassCode.PremiumBase = Me.txtBasis.Text.Trim()
        MyClassCode.PremiumBaseShort = Me.lblpremBaseShort.Text.Trim()
        MyClassCode.ManualElpaRate_Premises = Me.txtARatePrem.Text.Trim()
        MyClassCode.ManualElpaRate_Products = Me.txtARateProd.Text.Trim()
        Select Case hdnStateId.Value
            Case "15"
                MyClassCode.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                Exit Select
            Case "16"
                MyClassCode.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                Exit Select
            Case "36"
                MyClassCode.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                Exit Select
            Case Else
                'MyClassCode.QuickQuoteState = Nothing
        End Select

        If FirstClassificationInList Then
            If ClassCodeIsSaved() Then lnkBtnAdd.Visible = True Else lnkBtnAdd.Visible = False
        Else
            lnkBtnAdd.Visible = True
        End If

        UpdateAccordHeader()

        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Dim PolicyOrLoc As String = "Policy"
        If IsAtLocationLevel Then PolicyOrLoc = "Location"
        Me.ValidationHelper.GroupName = PolicyOrLoc & " Classification #" & ClassCodeIndex + 1

        ' State
        If hdnStateId.Value Is Nothing OrElse hdnStateId.Value = "" Then
            'If ddState.SelectedValue = "" Then
            Me.ValidationHelper.AddError(ddState, "Missing State", accordList)
        End If

        If txtClassCode.Text.Trim = String.Empty Then
            Me.ValidationHelper.AddError("No Class Code selected")
            'Me.ValidationHelper.AddError(txtSearchClassCode, "No Class Code selected", accordList) this method pushes the find button down
            Exit Sub
        End If

        If txtExposure.Text = String.Empty Then
            Me.ValidationHelper.AddError(txtExposure, "Missing Premium Exposure", accordList)
        Else
            If Not IsNumeric(txtExposure.Text) OrElse CDec(txtExposure.Text) < 0 Then
                Me.ValidationHelper.AddError(txtExposure, "Invalid Premium Exposure", accordList)
            End If
        End If

        ' Assignment should always be there, but just in case
        If ddAssignment.SelectedIndex < 0 Then
            Me.ValidationHelper.AddError(ddAssignment, "Missing Assignment", accordList)
        End If

        If hdnCheckAratePrem.Value IsNot Nothing AndAlso hdnCheckAratePrem.Value <> String.Empty Then
            If txtARatePrem.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtARatePrem, "Missing Premises/Ops Manual Rate", accordList)
            Else
                If Not IsNumeric(txtARatePrem.Text) OrElse CDec(txtARatePrem.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtARatePrem, "Invalid Premises/Ops Manual Rate", accordList)
                End If
            End If
        End If
        If hdnCheckArateProd.Value IsNot Nothing AndAlso hdnCheckArateProd.Value <> String.Empty Then
            If txtARateProd.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtARateProd, "Missing Products/Comp Manual Rate", accordList)
            Else
                If Not IsNumeric(txtARateProd.Text) OrElse CDec(txtARateProd.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtARateProd, "Invalid Products/Comp Manual Rate", accordList)
                End If
            End If
        End If

        Me.ValidateChildControls(valArgs)

        Exit Sub
    End Sub

    Protected Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkBtnAdd.Click
        RaiseEvent AddNewClassCode()
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        RaiseEvent DeleteClassCode(ClassCodeType, MyClassCode.UniqueIdentifier)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
        If SwitchedType Then
            RaiseEvent RepopulateClassCodes()
            SwitchedType = False
        End If
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        ddSearchType.SelectedIndex = 0
        txtSearchClassCode.Text = ""
        txtClassCode.Text = ""
        txtClassCodeDescription.Text = ""
        ddAssignment.SelectedIndex = 0
        If ddAssignmentLocation.Items.Count > 0 Then ddAssignmentLocation.SelectedIndex = 0
        trAssignmentInfoRow.Attributes.Add("style", "display:none")
        txtExposure.Text = ""
        txtPremiumDescription.Text = ""
        txtBasis.Text = ""
        trAratingInfoRow.Attributes.Add("style", "display:none")
        trAratingRow.Attributes.Add("style", "display:none")
        txtARatePrem.Text = ""
        txtARateProd.Text = ""
        divFootnote.InnerHtml = ""

        ' Clear the values in MyClassCode
        MyClassCode.ClassCode = "9999999"
        MyClassCode.ClassDescription = ""
        MyClassCode.PremiumBase = ""
        MyClassCode.PremiumExposure = ""
        MyClassCode.PremiumBaseShort = ""
        MyClassCode.ManualElpaRate_Premises = ""
        MyClassCode.ManualElpaRate_Products = ""
    End Sub
End Class