Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CL
Public Class ctl_BOP_ENDO_App_Building
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

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property


    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return BuildingIndex
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainDiv.ClientID, Me.hdnAccordList, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
    End Sub

    Private Sub LoadPayeeDDLs()
        'Dim li As New ListItem()

        '' Need to save first so all additonal interests already added will be saved to the quote
        'Save_FireSaveEvent(False)

        'ddlBuildingLimitLossPayeeName.Items.Clear()
        'ddlBuildingLimitLossPayeeType.Items.Clear()

        'If Quote IsNot Nothing Then
        '    ' Building Limit Loss Payee - Load with additional interests

        '    If Quote.AdditionalInterests IsNot Nothing Then
        '        For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
        '            li = New ListItem()
        '            li.Text = AI.Name.DisplayName
        '            li.Value = AI.ListId
        '            ddlBuildingLimitLossPayeeName.Items.Add(li)
        '        Next
        '    End If

        '    ' Personal Property Limit Loss Payee  - Load with additional interests
        '    If Quote.AdditionalInterests IsNot Nothing Then
        '        For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
        '            li = New ListItem()
        '            li.Text = AI.Name.DisplayName
        '            li.Value = AI.ListId
        '            ddlPersonalPropertyLimitLossPayeeName.Items.Add(li)
        '        Next
        '    End If
        'End If

        'updated 6/1/2017; note: similar logic in ctl_App_CTEQ.ascx.vb
        'ddlBuildingLimitLossPayeeName.Items.Clear()
        'ddlPersonalPropertyLimitLossPayeeName.Items.Clear()

        'Dim li As New ListItem()
        'li.Text = "N/A"
        'li.Value = ""
        'ddlBuildingLimitLossPayeeName.Items.Add(li)
        'Dim li2 As New ListItem()
        'li2.Text = "N/A"
        'li2.Value = ""
        'ddlPersonalPropertyLimitLossPayeeName.Items.Add(li2)

        'If Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then
        '    For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
        '        li = New ListItem
        '        li.Text = ai.Name.DisplayName
        '        li.Value = ai.ListId
        '        ddlBuildingLimitLossPayeeName.Items.Add(li)
        '        li2 = New ListItem
        '        li2.Text = ai.Name.DisplayName
        '        li2.Value = ai.ListId
        '        ddlPersonalPropertyLimitLossPayeeName.Items.Add(li2)
        '    Next
        'End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If (IsQuoteEndorsement() OrElse IsQuoteReadOnly()) AndAlso
            (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP) _
            AndAlso IsNewCo() Then
            trOldCo.Visible = False
            trNewCo.Visible = True
            Me.trPrefillInfoTextEndoNewCo.Attributes.Add("style", "display:''")
            Me.trPrefillInfoTextEndoOldCo.Attributes.Add("style", "display:none")
        Else
            trOldCo.Visible = True
            trNewCo.Visible = False
            If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                Me.trPrefillInfoTextEndoOldCo.Attributes.Add("style", "display:none")
            Else
                Me.trPrefillInfoTextEndoOldCo.Attributes.Add("style", "display:''")
            End If
            Me.trPrefillInfoTextEndoNewCo.Attributes.Add("style", "display:none")
        End If
        If (IsQuoteEndorsement() OrElse IsQuoteReadOnly()) AndAlso MyBuilding.IsNotNull Then 'Note: Need IsQuoteEndorsement(), IsQuoteReadOnly() because this control was added to the quote side for endorsements. We do not want this firing for new business.
            'Me.lblAccordHeader.Text = "Building #{0} - {1}".FormatIFM(Me.BuildingIndex + 1, Me.MyBuilding.Description).Ellipsis(55)
            Me.lblAccordHeader.Text = "Building Application Info"
            'lblBuildingLimit.Text = "Building Limit: " & MyBuilding.Limit
            'lblPersonalPropertyLimit.Text = "Personal Property Limit: " & MyBuilding.PersonalPropertyLimit

            'added 6/8/2017; originally in ctl_BOP_ENDO_App_Location
            If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP) _
                AndAlso IsNewCo() Then
                'NewCo
                ' Square Feet
                If MyBuilding.SquareFeet <> "0" Then txtSquareFeetNewCo.Text = MyBuilding.SquareFeet Else txtSquareFeetNewCo.Text = ""
                ' Year Built
                If MyBuilding.YearBuilt <> "0" Then txtYearBuiltNewCo.Text = MyBuilding.YearBuilt Else txtYearBuiltNewCo.Text = ""
                ' Number of Stories
                If MyBuilding.NumberOfStories <> "0" Then txtNumOfStoriesNewCo.Text = MyBuilding.NumberOfStories Else txtNumOfStoriesNewCo.Text = ""
                Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                If endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then
                    txtSquareFeetNewCo.Enabled = False
                    txtYearBuiltNewCo.Enabled = False
                    txtNumOfStoriesNewCo.Enabled = False
                End If
            Else
                ' Square Feet
                If MyBuilding.SquareFeet <> "0" Then txtSquareFeet.Text = MyBuilding.SquareFeet Else txtSquareFeet.Text = ""
                ' Year Built
                If MyBuilding.YearBuilt <> "0" Then txtYearBuilt.Text = MyBuilding.YearBuilt Else txtYearBuilt.Text = ""
            End If
            ' Year Roof Updated
            If MyBuilding.Updates.RoofUpdateYear <> "0" Then txtYearRoofUpdated.Text = MyBuilding.Updates.RoofUpdateYear Else txtYearRoofUpdated.Text = ""
            ' Year Plumbing Updated
            If MyBuilding.Updates.PlumbingUpdateYear <> "0" Then txtYearPlumbingUpdated.Text = MyBuilding.Updates.PlumbingUpdateYear Else txtYearPlumbingUpdated.Text = ""
            ' Year Wiring Updated
            If MyBuilding.Updates.ElectricUpdateYear <> "0" Then txtYearWiringUpdated.Text = MyBuilding.Updates.ElectricUpdateYear Else txtYearWiringUpdated.Text = ""
            ' Year Heat Updated
            If MyBuilding.Updates.CentralHeatUpdateYear <> "0" Then txtYearHeatUpdated.Text = MyBuilding.Updates.CentralHeatUpdateYear Else txtYearHeatUpdated.Text = ""

            'ddlBuildingLimitLossPayeeName.Items.Clear()
            'ddlPersonalPropertyLimitLossPayeeName.Items.Clear()

            'If Quote.AdditionalInterests IsNot Nothing AndAlso Quote.AdditionalInterests.Count > 0 Then
            '    For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
            '        Dim li As New ListItem()
            '        Dim li2 As New ListItem()
            '        If AI.Name.CommercialName1.Trim <> "" OrElse AI.Name.CommercialName2.Trim <> "" Then
            '            If AI.Name.CommercialName1.Trim <> "" Then
            '                li.Text = AI.Name.CommercialName1
            '                li.Value = AI.ListId
            '                li2.Text = AI.Name.CommercialName1
            '                li2.Value = AI.ListId
            '            Else
            '                li.Text = AI.Name.CommercialName2
            '                li2.Text = AI.Name.CommercialName2
            '            End If
            '            ddlBuildingLimitLossPayeeName.Items.Add(li)
            '            ddlPersonalPropertyLimitLossPayeeName.Items.Add(li2)
            '        Else
            '            If AI.Name.FirstName.Trim <> "" OrElse AI.Name.LastName.Trim <> "" Then
            '                Dim txt As String = ""
            '                If AI.Name.FirstName.Trim <> "" Then txt = AI.Name.FirstName
            '                If AI.Name.LastName.Trim <> "" Then
            '                    If txt <> "" Then txt += " "
            '                    txt += AI.Name.LastName
            '                End If
            '                li.Text = txt
            '                li.Value = AI.ListId
            '                li2.Text = txt
            '                li2.Value = AI.ListId
            '                ddlBuildingLimitLossPayeeName.Items.Add(li)
            '                ddlPersonalPropertyLimitLossPayeeName.Items.Add(li2)
            '            End If
            '        End If
            '    Next
            'End If

            'updated 6/1/2017
            'LoadPayeeDDLs()

            '6/7/2017 note on AIs: this control will just work w/ the specific Building AIs that it needs, but the full list, which contains Contractors Equipment AIs, is still in tact; the Contractors Equipment control will just work w/ those specific AIs, which are copied back and forth from the Building AIs in the QuickQuote library on Retrieval and Save

            'added 6/6/2017
            'If ddlBuildingLimitLossPayeeName.Items IsNot Nothing AndAlso ddlBuildingLimitLossPayeeName.Items.Count > 0 Then
            '    Dim buildingAIs As List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest) = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestsForDescription(MyBuilding.AdditionalInterests, "Building", cloneList:=True, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing)
            '    If buildingAIs IsNot Nothing AndAlso buildingAIs.Count > 0 Then
            '        For Each bAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In buildingAIs
            '            If bAI IsNot Nothing Then
            '                If QQHelper.IsPositiveIntegerString(bAI.ListId) = True AndAlso ddlBuildingLimitLossPayeeName.Items.FindByValue(bAI.ListId) IsNot Nothing Then
            '                    ddlBuildingLimitLossPayeeName.SelectedValue = bAI.ListId
            '                    SetBuildingAdditionalInterestFieldsFromAI(bAI, setLossPayeeName:=False)
            '                    Exit For
            '                End If
            '            End If
            '        Next
            '    End If
            'End If
            'updated 6/7/2017 to just get 1 buildingAI and set everything that we can
            'Dim buildingAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForDescription(MyBuilding.AdditionalInterests, "Building", cloneAI:=False, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing)
            'If buildingAI IsNot Nothing Then
            '    SetBuildingAdditionalInterestFieldsFromAI(buildingAI, setLossPayeeName:=True)
            'End If

            'If ddlPersonalPropertyLimitLossPayeeName.Items IsNot Nothing AndAlso ddlPersonalPropertyLimitLossPayeeName.Items.Count > 0 Then
            '    Dim persPropAIs As List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest) = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestsForDescription(MyBuilding.AdditionalInterests, "Personal property", cloneList:=True, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing)
            '    If persPropAIs IsNot Nothing AndAlso persPropAIs.Count > 0 Then
            '        For Each ppAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In persPropAIs
            '            If ppAI IsNot Nothing Then
            '                If QQHelper.IsPositiveIntegerString(ppAI.ListId) = True AndAlso ddlPersonalPropertyLimitLossPayeeName.Items.FindByValue(ppAI.ListId) IsNot Nothing Then
            '                    ddlPersonalPropertyLimitLossPayeeName.SelectedValue = ppAI.ListId
            '                    SetPersonalPropertyAdditionalInterestFieldsFromAI(ppAI, setLossPayeeName:=False)
            '                    Exit For
            '                End If
            '            End If
            '        Next
            '    End If
            'End If
            'updated 6/7/2017 to just get 1 persPropAI and set everything that we can
            'Dim persPropAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForDescription(MyBuilding.AdditionalInterests, "Personal property", cloneAI:=False, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing)
            'If persPropAI IsNot Nothing Then
            '    SetPersonalPropertyAdditionalInterestFieldsFromAI(persPropAI, setLossPayeeName:=True)
            'End If

        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        '6/7/2017 note on AIs: this control will just work w/ the specific Building AIs that it needs, but the full list, which contains Contractors Equipment AIs, is still in tact; the Contractors Equipment control will just work w/ those specific AIs, which are copied back and forth from the Building AIs in the QuickQuote library on Retrieval and Save
        'Added 10/18/2021 for BOP Endorsements task 61660 MLW
        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        If Me.Visible AndAlso Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then 'Note: Need Me.Visible because this control was added to the quote side for endorsements. We do not want this firing for new business.
            If MyBuilding IsNot Nothing Then
                'added 6/8/2017; originally in ctl_BOP_ENDO_App_Location
                If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP) AndAlso IsNewCo() Then
                    MyBuilding.SquareFeet = txtSquareFeetNewCo.Text
                    MyBuilding.YearBuilt = txtYearBuiltNewCo.Text
                    MyBuilding.NumberOfStories = txtNumOfStoriesNewCo.Text
                Else
                    MyBuilding.SquareFeet = txtSquareFeet.Text
                    MyBuilding.YearBuilt = txtYearBuilt.Text
                End If
                MyBuilding.Updates.RoofUpdateYear = txtYearRoofUpdated.Text
                MyBuilding.Updates.PlumbingUpdateYear = txtYearPlumbingUpdated.Text
                MyBuilding.Updates.ElectricUpdateYear = txtYearWiringUpdated.Text
                MyBuilding.Updates.CentralHeatUpdateYear = txtYearHeatUpdated.Text

                'added 6/8/2017
                'save/remove building AI
                'If ddlBuildingLimitLossPayeeName.SelectedIndex > 0 Then
                '    Dim buildingAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForDescription(MyBuilding.AdditionalInterests, "Building", cloneAI:=False, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing, returnNewIfNothing:=True)
                '    If buildingAI IsNot Nothing Then
                '        Dim isSameListId As Boolean = False
                '        With buildingAI
                '            If .ListId = Me.ddlBuildingLimitLossPayeeName.SelectedValue Then
                '                isSameListId = True
                '            Else
                '                isSameListId = False 'redundant
                '            End If
                '            .ListId = Me.ddlBuildingLimitLossPayeeName.SelectedValue
                '            .TypeId = Me.ddlBuildingLimitLossPayeeType.SelectedValue
                '            Select Case Me.ddlBuildingLimitATMA.SelectedValue
                '                Case "3"
                '                    .ATIMA = True
                '                    .ISAOA = True
                '                Case "1"
                '                    .ATIMA = True
                '                    .ISAOA = False
                '                Case "2"
                '                    .ISAOA = True
                '                    .ATIMA = False
                '                Case Else '0
                '                    .ATIMA = False
                '                    .ISAOA = False
                '            End Select
                '            'If isSameListId = False Then 'will just run every time
                '            If Me.Quote IsNot Nothing AndAlso Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                '                Dim sourceAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(Me.Quote.AdditionalInterests, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuote.CommonMethods.QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                '                If sourceAI IsNot Nothing Then
                '                    QQHelper.CopyQuickQuoteAdditionalInterestNameAddressEmailsAndPhones(sourceAI, buildingAI)
                '                End If
                '            End If
                '            'End If
                '        End With
                '    End If
                'Else
                '    QuickQuote.CommonMethods.QuickQuoteHelperClass.RemoveQuickQuoteAdditionalInterestsForDescription(MyBuilding.AdditionalInterests, "Building", matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing)
                'End If

                'save/remove persProp AI
                'If ddlPersonalPropertyLimitLossPayeeName.SelectedIndex > 0 Then
                '    Dim persPropAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForDescription(MyBuilding.AdditionalInterests, "Personal property", cloneAI:=False, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing, returnNewIfNothing:=True)
                '    If persPropAI IsNot Nothing Then
                '        Dim isSameListId As Boolean = False
                '        With persPropAI
                '            If .ListId = Me.ddlPersonalPropertyLimitLossPayeeName.SelectedValue Then
                '                isSameListId = True
                '            Else
                '                isSameListId = False 'redundant
                '            End If
                '            .ListId = Me.ddlPersonalPropertyLimitLossPayeeName.SelectedValue
                '            .TypeId = Me.ddlPersonalPropertyLimitLossPayeeType.SelectedValue
                '            Select Case Me.ddlPersonalPropertyLimitATMA.SelectedValue
                '                Case "3"
                '                    .ATIMA = True
                '                    .ISAOA = True
                '                Case "1"
                '                    .ATIMA = True
                '                    .ISAOA = False
                '                Case "2"
                '                    .ISAOA = True
                '                    .ATIMA = False
                '                Case Else '0
                '                    .ATIMA = False
                '                    .ISAOA = False
                '            End Select
                '            'If isSameListId = False Then 'will just run every time
                '            If Me.Quote IsNot Nothing AndAlso Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                '                Dim sourceAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(Me.Quote.AdditionalInterests, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuote.CommonMethods.QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                '                If sourceAI IsNot Nothing Then
                '                    QQHelper.CopyQuickQuoteAdditionalInterestNameAddressEmailsAndPhones(sourceAI, persPropAI)
                '                End If
                '            End If
                '            'End If
                '        End With
                '    End If
                'Else
                '    QuickQuote.CommonMethods.QuickQuoteHelperClass.RemoveQuickQuoteAdditionalInterestsForDescription(MyBuilding.AdditionalInterests, "Personal property", matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing)
                'End If
            End If

            Me.SaveChildControls()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 10/18/2021 for BOP Endorsements task 61660 MLW
        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        If Me.Visible AndAlso Not endorsementsPreexistHelper.IsPreexistingLocation(LocationIndex) Then 'Note: Need Me.Visible because this control was added to the quote side for endorsements. We do not want this firing for new business.
            MyBase.ValidateControl(valArgs)

            'added 6/8/2017; originally in ctl_BOP_ENDO_App_Location
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            ValidationHelper.GroupName = "Location #" & (LocationIndex + 1).ToString & " Building #" & (BuildingIndex + 1).ToString

            If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP) AndAlso IsNewCo() Then
                If txtSquareFeetNewCo.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtSquareFeetNewCo, "Missing Square Feet", accordList)
                Else
                    If Not IsNumeric(txtSquareFeetNewCo.Text) Then
                        Me.ValidationHelper.AddError(txtSquareFeetNewCo, "Invalid Square Feet", accordList)
                    Else
                        Dim sf As Integer = CInt(txtSquareFeetNewCo.Text)
                        If sf <= 0 Then
                            Me.ValidationHelper.AddError(txtSquareFeetNewCo, "Invalid Square Feet", accordList)
                        End If
                    End If
                End If

                If txtYearBuiltNewCo.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtYearBuiltNewCo, "Missing Year Built", accordList)
                Else
                    If Not YearBuiltValidationHelper.ValidYearBuilt(txtYearBuiltNewCo.Text, Quote) Then
                        Me.ValidationHelper.AddError(txtYearBuiltNewCo, "Invalid Year Built", accordList)
                    End If
                End If

                If IsNullEmptyorWhitespace(txtNumOfStoriesNewCo.Text) Then
                    Me.ValidationHelper.AddError(txtNumOfStoriesNewCo, "Invalid Number of Stories", accordList)
                End If
            Else
                If txtSquareFeet.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtSquareFeet, "Missing Square Feet", accordList)
                Else
                    If Not IsNumeric(txtSquareFeet.Text) Then
                        Me.ValidationHelper.AddError(txtSquareFeet, "Invalid Square Feet", accordList)
                    Else
                        Dim sf As Integer = CInt(txtSquareFeet.Text)
                        If sf <= 0 Then
                            Me.ValidationHelper.AddError(txtSquareFeet, "Invalid Square Feet", accordList)
                        End If
                    End If
                End If

                If txtYearBuilt.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtYearBuilt, "Missing Year Built", accordList)
                Else
                    If Not YearBuiltValidationHelper.ValidYearBuilt(txtYearBuilt.Text, Quote) Then
                        Me.ValidationHelper.AddError(txtYearBuilt, "Invalid Year Built", accordList)
                    End If
                End If
            End If

            If txtYearRoofUpdated.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtYearRoofUpdated, "Missing Year Roof Updated", accordList)
            Else
                If Not ValidYear(txtYearRoofUpdated.Text) Then
                    Me.ValidationHelper.AddError(txtYearRoofUpdated, "Invalid Year Roof Updated", accordList)
                End If
            End If

            If txtYearPlumbingUpdated.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtYearPlumbingUpdated, "Missing Year Plumbing Updated", accordList)
            Else
                If Not ValidYear(txtYearPlumbingUpdated.Text) Then
                    Me.ValidationHelper.AddError(txtYearPlumbingUpdated, "Invalid Year Plumbing Updated", accordList)
                End If
            End If


            If txtYearWiringUpdated.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtYearWiringUpdated, "Missing Year Wiring Updated", accordList)
            Else
                If Not ValidYear(txtYearWiringUpdated.Text) Then
                    Me.ValidationHelper.AddError(txtYearWiringUpdated, "Invalid Year Wiring Updated", accordList)
                End If
            End If

            If txtYearHeatUpdated.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtYearHeatUpdated, "Missing Year Heat Updated", accordList)
            Else
                If Not ValidYear(txtYearHeatUpdated.Text) Then
                    Me.ValidationHelper.AddError(txtYearHeatUpdated, "Invalid Year Heat Updated", accordList)
                End If
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent()
    End Sub

    'added 6/6/2017
    'Private Sub SetBuildingAdditionalInterestFieldsFromAI(ByVal buildingAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, Optional ByVal setLossPayeeName As Boolean = True)
    '    If buildingAI IsNot Nothing Then
    '        If setLossPayeeName = True Then
    '            If QQHelper.IsPositiveIntegerString(buildingAI.ListId) = True Then
    '                If Me.ddlBuildingLimitLossPayeeName.Items IsNot Nothing Then
    '                    If Me.ddlBuildingLimitLossPayeeName.Items.Count > 0 AndAlso Me.ddlBuildingLimitLossPayeeName.Items.FindByValue(buildingAI.ListId) IsNot Nothing Then
    '                        Me.ddlBuildingLimitLossPayeeName.SelectedValue = buildingAI.ListId
    '                    Else
    '                        'add name to dropdown and set

    '                    End If
    '                End If
    '            End If
    '        End If
    '        If QQHelper.IsPositiveIntegerString(buildingAI.TypeId) = True Then
    '            If Me.ddlBuildingLimitLossPayeeType.Items IsNot Nothing Then
    '                If Me.ddlBuildingLimitLossPayeeType.Items.Count > 0 AndAlso Me.ddlBuildingLimitLossPayeeType.Items.FindByValue(buildingAI.TypeId) IsNot Nothing Then
    '                    Me.ddlBuildingLimitLossPayeeType.SelectedValue = buildingAI.TypeId
    '                Else
    '                    'add type to dropdown and set

    '                End If
    '            End If
    '        End If
    '        If buildingAI.ATIMA = True AndAlso buildingAI.ISAOA = True Then
    '            Me.ddlBuildingLimitATMA.SelectedValue = "3"
    '        ElseIf buildingAI.ATIMA = True Then
    '            Me.ddlBuildingLimitATMA.SelectedValue = "1"
    '        ElseIf buildingAI.ISAOA = True Then
    '            Me.ddlBuildingLimitATMA.SelectedValue = "2"
    '        Else 'ElseIf buildingAI.ATIMA = False AndAlso buildingAI.ISAOA = False Then
    '            Me.ddlBuildingLimitATMA.SelectedValue = "0"
    '        End If
    '    End If
    'End Sub
    'Private Sub SetPersonalPropertyAdditionalInterestFieldsFromAI(ByVal persPropAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, Optional ByVal setLossPayeeName As Boolean = True)
    '    If persPropAI IsNot Nothing Then
    '        If setLossPayeeName = True Then
    '            If QQHelper.IsPositiveIntegerString(persPropAI.ListId) = True Then
    '                If Me.ddlPersonalPropertyLimitLossPayeeName.Items IsNot Nothing Then
    '                    If Me.ddlPersonalPropertyLimitLossPayeeName.Items.Count > 0 AndAlso Me.ddlPersonalPropertyLimitLossPayeeName.Items.FindByValue(persPropAI.ListId) IsNot Nothing Then
    '                        Me.ddlPersonalPropertyLimitLossPayeeName.SelectedValue = persPropAI.ListId
    '                    Else
    '                        'add name to dropdown and set

    '                    End If
    '                End If
    '            End If
    '        End If
    '        If QQHelper.IsPositiveIntegerString(persPropAI.TypeId) = True Then
    '            If Me.ddlPersonalPropertyLimitLossPayeeType.Items IsNot Nothing Then
    '                If Me.ddlPersonalPropertyLimitLossPayeeType.Items.Count > 0 AndAlso Me.ddlPersonalPropertyLimitLossPayeeType.Items.FindByValue(persPropAI.TypeId) IsNot Nothing Then
    '                    Me.ddlPersonalPropertyLimitLossPayeeType.SelectedValue = persPropAI.TypeId
    '                Else
    '                    'add type to dropdown and set

    '                End If
    '            End If
    '        End If
    '        If persPropAI.ATIMA = True AndAlso persPropAI.ISAOA = True Then
    '            Me.ddlPersonalPropertyLimitATMA.SelectedValue = "3"
    '        ElseIf persPropAI.ATIMA = True Then
    '            Me.ddlPersonalPropertyLimitATMA.SelectedValue = "1"
    '        ElseIf persPropAI.ISAOA = True Then
    '            Me.ddlPersonalPropertyLimitATMA.SelectedValue = "2"
    '        Else 'ElseIf persPropAI.ATIMA = False AndAlso persPropAI.ISAOA = False Then
    '            Me.ddlPersonalPropertyLimitATMA.SelectedValue = "0"
    '        End If
    '    End If
    'End Sub

    'moved to VrControlBaseEssentials
    ''added 6/8/2017 from ctl_BOP_ENDO_App_Location
    'Private Function ValidYear(ByVal testYear As String) As Boolean
    '    If Not IsNumeric(testYear) Then Return False
    '    Dim yr As Integer = CInt(testYear)
    '    If yr > DateTime.Now.Year Then Return False
    '    If yr < 1900 Then Return False
    '    Return True
    'End Function

End Class