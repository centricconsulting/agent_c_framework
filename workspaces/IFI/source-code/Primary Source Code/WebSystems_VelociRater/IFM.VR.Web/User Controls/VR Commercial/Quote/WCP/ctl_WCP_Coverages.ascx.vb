Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports Diamond.Common.ChoicePoint.Library.ProductTransactionRecords.Report.Ncf

Public Class ctl_WCP_Coverages
    Inherits VRControlBase

#Region "Declarations"

    Private Structure ClassIficationItem_enum
        Public Classification As QuickQuote.CommonObjects.QuickQuoteClassification
        Public QuickQuoteState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Public ClassificationIndex
    End Structure

    Public Event RequestClassificationPopulate()

    ''' <summary>
    ''' Returns the KY effective date as a date object
    ''' </summary>
    ''' <returns></returns>
    'Private ReadOnly Property WCKYEffDate As DateTime
    '    Get
    '        If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate")) Then
    '            If IsDate(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate")) Then
    '                Return CDate(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate"))
    '            Else
    '                Return DateTime.Now.AddDays(1)
    '            End If
    '        Else
    '            Return DateTime.Now.AddDays(1)
    '        End If
    '    End Get
    'End Property

    Private ReadOnly Property TotalNumberOfClassificationsOnQuote() As Integer
        Get
            Dim cnt As Integer = 0
            'If SubQuotes IsNot Nothing Then
            '    For Each q As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            '        If q.Locations IsNot Nothing AndAlso q.Locations.HasItemAtIndex(0) AndAlso q.Locations(0).Classifications IsNot Nothing Then
            '            For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In q.Locations(0).Classifications
            '                If cls.ClassCode IsNot Nothing AndAlso cls.ClassCode.Trim <> "" Then
            '                    cnt += 1
            '                End If
            '            Next
            '        End If
            '    Next
            'End If
            'updated 12/28/2018
            Dim quoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates
            If quoteStates IsNot Nothing AndAlso quoteStates.Count > 0 Then
                For Each qs As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qs)
                    If locsForState IsNot Nothing AndAlso locsForState.Count > 0 AndAlso locsForState(0) IsNot Nothing AndAlso locsForState(0).Classifications IsNot Nothing Then
                        cnt += locsForState(0).Classifications.Count
                    End If
                Next
            Else
                If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 AndAlso Quote.Locations(0) IsNot Nothing AndAlso Quote.Locations(0).Classifications IsNot Nothing Then
                    cnt = Quote.Locations(0).Classifications.Count
                End If
            End If
            Return cnt
        End Get
    End Property


#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(divGeneralInfo.ClientID, hdnAccordGenInfo, "0")
        'Me.VRScript.CreateAccordion(divLocation.ClientID, hdnAccordLoc, "0")
        Me.VRScript.CreateAccordion(divClassification.ClientID, hdnAccordClass, "0")
        Me.VRScript.CreateAccordion(ListAccordionDivId, hdnAccordClassList, "0")
        Me.VRScript.CreateAccordion(divEndorsements.ClientID, hdnAccordEndorsements, "0")

        Me.VRScript.StopEventPropagation(Me.lnkSaveGeneralInfo.ClientID)
        'Me.VRScript.StopEventPropagation(Me.lnkSaveLocation.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSaveEndorsements.ClientID)
        Me.VRScript.StopEventPropagation(Me.btnSaveClassifications.ClientID)

        Me.VRScript.StopEventPropagation(Me.lbAddNewClassification.ClientID)

        ' Coverage checkboxes script
        chkInclusionOfSoleProp.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('INCLSOLE', '" & chkInclusionOfSoleProp.ClientID & "','', '', '','','');")
        chkBlanketWaiverOfSubro.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('BLNKTW', '" & chkBlanketWaiverOfSubro.ClientID & "', '', '', '', '','');")
        chkWaiverofSubro.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('WAIVERSUBRO', '" & chkWaiverofSubro.ClientID & "', '" & trNumberOfWaiversRow.ClientID & "', '', '', '','');")
        chkExclusionOfAmishWorkers.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('AMISH', '" & chkExclusionOfAmishWorkers.ClientID & "', '', '', '', '','');")
        ddlEmployersLiability.Attributes.Add("onchange", "Wcp.EmployersLiabilityLimitsChanged('" & ddlEmployersLiability.ClientID & "');")
        'chkExclusionOfExecutiveOfficer.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('EXCLEXEC', '" & chkExclusionOfExecutiveOfficer.ClientID & "', '', '', '', '','');")
        chkExclusionOfSoleProprietorsEtc_IL.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('EXCLSOLE_IL', '" & chkExclusionOfSoleProprietorsEtc_IL.ClientID & "', '', '', '', '','');")

        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' If INDIANA is on the quote then show the exclusions message when the checkbox is checked or unchecked
            ' We do this by passing the info row
            ' Always show the info message when Indiana is on the state; do not show it if Illinois or Kentucky are the only states on the quote MGB 2-1-2019 Bug 31215
            If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(SubQuotes, "IN") Then
                chkExclusionOfExecutiveOfficer.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('EXCLEXEC', '" & chkExclusionOfExecutiveOfficer.ClientID & "', '', '', '', '','" & trInfo1.ClientID & "');")
                'chkExclusionOfSoleProprietorsEtc_IL.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('EXCLSOLE_IL', '" & chkExclusionOfSoleProprietorsEtc_IL.ClientID & "', '', '', '', '','" & trInfo1.ClientID & "');")
            End If
        Else
            ' If INDIANA is not on the quote then do not show the exclusions message when the checkbox is checked or unchecked
            ' We do this by NOT passing the info row
            chkExclusionOfExecutiveOfficer.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('EXCLEXEC', '" & chkExclusionOfExecutiveOfficer.ClientID & "', '', '', '', '','');")
            'chkExclusionOfSoleProprietorsEtc_IL.Attributes.Add("onchange", "Wcp.CoverageCheckboxChanged('EXCLSOLE_IL', '" & chkExclusionOfSoleProprietorsEtc_IL.ClientID & "', '', '', '', '','');")
        End If

        ' Experience Modifcation
        txtExpMod.Attributes.Add("onkeyup", "Wcp.ExperienceModificationValueChanged('" & txtExpMod.ClientID & "','" & bdpExpModEffDate.ClientID & "');")

        'VRScript.AddScriptLine("function EmpLiabilityMessage() {alert('The Employers Liability limit defaulted to 500/500/500, which is the minimum limit required to quote an umbrella.');}")

        ResetEmpLiabilityCheck()
        Exit Sub
    End Sub

    Private Sub ResetEmpLiabilityCheck()
        If Me.SubQuoteFirst IsNot Nothing Then
            Dim requireReset = EffectiveDateHelper.doesRequireAPolicyHolderRestart(Quote)
            If SubQuoteFirst.EmployersLiabilityId <> "" AndAlso requireReset = False Then
                ddlEmployersLiability.SelectedValue = SubQuoteFirst.EmployersLiabilityId
            Else
                If SubQuoteFirst.EmployersLiabilityId = "311" Then '100/500/100
                    ddlEmployersLiability.SelectedValue = "313" '500/500/500
                    If requireReset Then
                        VRScript.AddScriptLine("alert('The Employers Liability limit defaulted to 500/500/500, which is the minimum limit required to quote an umbrella.');")
                        'VRScript.AddScriptLine("EmpLiabilityMessage();")
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub AttachControlEvents()
        For Each cntrl As RepeaterItem In Me.rptClassifications.Items
            Dim ClassControl As ctl_WCP_Classification = cntrl.FindControl("ctl_Classification")
            AddHandler ClassControl.AddClassificationRequested, AddressOf AddNewClassification
            'AddHandler ClassControl.ClearClassificationRequested, AddressOf ClearClassification
            AddHandler ClassControl.DeleteClassificationRequested, AddressOf DeleteClassification
            AddHandler ClassControl.RequestClassificationPopulate, AddressOf HandleClassificationPopulateRequest
        Next

        ' TODO: Add property address changed logic for locations???
        'AddHandler Me.ctl_GenInfo.PropertyAddressChanged, AddressOf HandleAddressChange

    End Sub

    Private Sub HandleClassificationPopulateRequest()
        RaiseEvent RequestClassificationPopulate()
    End Sub

    'Private Sub HandleAddressChange()
    '    UpdateLocationAddressHeader()
    'End Sub

    Public Overrides Sub LoadStaticData()
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()

        ' We only want to load the ddl's once
        If Me.ddlEmployersLiability.Items IsNot Nothing AndAlso ddlEmployersLiability.Items.Count > 0 Then Exit Sub

        ' Employers Liability
        QQHelper.LoadStaticDataOptionsDropDown(ddlEmployersLiability, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.EmployersLiabilityId, , Quote.LobType)

        ' SET DDL DEFAULTS
        ' Employers Liability - 100/500/100
        ' New Default 500/500/500 - CAH 10/11/2013 WS-1775
        For Each li As ListItem In ddlEmployersLiability.Items
            If li.Text.Trim <> "" Then
                If li.Text = "500/500/500" Then
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        Exit Sub
    End Sub

    'Private Sub UpdateLocationAddressHeader()
    '    lblAccordHeader_Location.Text = "Location #1"
    '    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 AndAlso Quote.Locations(0).Address IsNot Nothing Then
    '        If Quote.Locations(0).Address.DisplayAddress <> "" Then
    '            Dim adr As String = Quote.Locations(0).Address.DisplayAddress.ToUpper
    '            If adr.Length > 50 Then adr = adr.Substring(0, 50) & "..."
    '            lblAccordHeader_Location.Text = "Location #1 - " & adr
    '        End If
    '    End If
    '    Exit Sub
    'End Sub

    ''' <summary>
    ''' Returns the number of locations in the passed state
    ''' </summary>
    ''' <param name="qqstate"></param>
    ''' <returns></returns>
    Private Function NumberOfLocationsOnState(qqstate As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Integer
        Dim count As Integer = 0
        'Dim foundit As Boolean = False 'removed 12/28/2018
        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                ' Loop through all the locations and count the number of locations in the passed state
                For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If LOC.Address IsNot Nothing AndAlso LOC.Address.QuickQuoteState = qqstate Then
                        count += 1
                        Exit For
                    End If
                Next
                Return count
            Else
                ' No locations - Locations is nothing
                Return 0
            End If
        End If
        Return 0
    End Function

    Public Overrides Sub Populate()
        Dim CompletedOpsCount As Integer = 0
        LoadStaticData()
        Me.PopulateChildControls()

        If Quote IsNot Nothing Then
            'KY WCP Gov State versus KY WCP subquote state
            'Updated 3/10/2022 for KY WCP Task 73087 MLW
            Select Case Me.Quote.QuickQuoteState
                Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                    'KY Gov State
                    'When KY WCP Gov State, we need to change checkbox labels and hide some checkboxes based on subquotes
                    lblInclusionOfSoleProp.Text = "Inclusion of Sole Proprietors, Partners, and LLC Members (WC 00 03 10)(IN/IL/KY)"
                    lblExclusionOfExecutiveOfficer.Text = "Exclusion of Executive Officer (WC 00 03 08)(IN/KY)"
                    'KY specific coverage
                    trRejectionOfCoverageEndorsementRow.Attributes.Add("style", "display:''")
                    'Non KY specific coverages
                    If SubQuotesContainsState("IN") OrElse SubQuotesContainsState("IL") Then
                        trBlanketWaiverOfSubroRow.Attributes.Add("style", "display:''") 'IN/IL
                        trWaiverOfSubroRow.Attributes.Add("style", "display:''") 'IN/IL
                    Else
                        trBlanketWaiverOfSubroRow.Attributes.Add("style", "display:none") 'IN/IL
                        trWaiverOfSubroRow.Attributes.Add("style", "display:none") 'IN/IL
                    End If
                    Exit Select
                Case Else
                    'KY Subquote, IN or IL Gov State
                    ' When KY WC is enabled we need to change a couple of checkbox labels 
                    If IsDate(Quote.EffectiveDate) AndAlso CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                        ' Add KY to a couple of labels
                        lblInclusionOfSoleProp.Text = "Inclusion of Sole Proprietors, Partners, and LLC Members (WC 00 03 10)(IN/IL/KY)"
                        lblExclusionOfExecutiveOfficer.Text = "Exclusion of Executive Officer (WC 00 03 08)(IN/KY)"
                        ' Display the KY specific coverages
                        trRejectionOfCoverageEndorsementRow.Attributes.Add("style", "display:''")
                    Else
                        ' Don't add KY to a couple of labels
                        lblInclusionOfSoleProp.Text = "Inclusion of Sole Proprietors, Partners, and LLC Members (WC 00 03 10)(IN/IL)"
                        lblExclusionOfExecutiveOfficer.Text = "Exclusion of Executive Officer (WC 00 03 08)(IN)"
                        ' Hide the KY specific coverages
                        trRejectionOfCoverageEndorsementRow.Attributes.Add("style", "display:none")
                    End If
                    Exit Select
            End Select

            '11/27/2018 - top-level stuff; moved/copied here from below since it's not all applied at state-level
            If Me.Quote.ExperienceModificationFactor <> "" AndAlso Me.Quote.ExperienceModificationFactor <> "0" Then
                txtExpMod.Text = Me.Quote.ExperienceModificationFactor
            Else
                txtExpMod.Text = "1"
            End If
            If Me.Quote.RatingEffectiveDate <> "" AndAlso IsDate(Me.Quote.RatingEffectiveDate) Then
                bdpExpModEffDate.SelectedDate = CDate(Me.Quote.RatingEffectiveDate)
            Else
                bdpExpModEffDate.SelectedDate = DateTime.Today
            End If

            ' Exp Mod Date is disabled when Exp Mod is 1, enabled otherwise
            If IsNumeric(txtExpMod.Text) Then
                Dim val As Decimal = 0
                val = CDec(txtExpMod.Text)
                If val = 1 Then
                    bdpExpModEffDate.Enabled = False
                Else
                    bdpExpModEffDate.Enabled = True
                End If
            End If

            PopulateClassifications()

            If SubQuoteFirst.EmployersLiabilityId.IsNumeric Then

                ddlEmployersLiability.SelectedValue = SubQuoteFirst.EmployersLiabilityId
            End If




            '11/27/2018 - state-specific stuff; moved here from above
            Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                If govStateQuote IsNot Nothing Then 'Governing State
                    chkInclusionOfSoleProp.Checked = govStateQuote.HasInclusionOfSoleProprietorsPartnersOfficersAndOthers
                    If govStateQuote.BlanketWaiverOfSubrogation = "4" Then
                        chkBlanketWaiverOfSubro.Checked = True
                    Else
                        chkBlanketWaiverOfSubro.Checked = False
                    End If
                    If govStateQuote.HasWaiverOfSubrogation Then
                        chkWaiverofSubro.Checked = True
                        txtNumberOfWaivers.Text = govStateQuote.WaiverOfSubrogationNumberOfWaivers
                    Else
                        chkWaiverofSubro.Checked = False
                        txtNumberOfWaivers.Text = ""
                    End If
                Else
                    chkInclusionOfSoleProp.Checked = False
                    chkBlanketWaiverOfSubro.Checked = False
                    chkWaiverofSubro.Checked = False
                    txtNumberOfWaivers.Text = ""
                End If
                CheckWaiverOfSubro()

                ' IN/KY
                If SubQuotesContainsState("IN") OrElse SubQuotesContainsState("KY") Then
                    ' Exclusion of executive officer should always show when IN or KY is on the quote
                    trExclusionOfExecutiveOfficer_row.Attributes.Add("style", "display:''")

                    ' The value should be set the same on both the IN and KY quotes if both states are on the quote
                    Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
                    If INQuote IsNot Nothing Then
                        chkExclusionOfExecutiveOfficer.Checked = INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers
                    Else
                        Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("KY")
                        If KYQuote IsNot Nothing Then
                            chkExclusionOfExecutiveOfficer.Checked = KYQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers
                        Else
                            chkExclusionOfExecutiveOfficer.Checked = False
                        End If
                    End If
                Else
                    trExclusionOfExecutiveOfficer_row.Attributes.Add("style", "display:none")
                End If

                ' IN only
                If SubQuotesContainsState("IN") Then
                    trExclusionOfAmishWorkers_row.Attributes.Add("style", "display:''")
                    'trExclusionOfExecutiveOfficer_row.Attributes.Add("style", "display:''")
                    Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
                    If INQuote IsNot Nothing Then
                        chkExclusionOfAmishWorkers.Checked = INQuote.HasExclusionOfAmishWorkers
                    Else
                        chkExclusionOfAmishWorkers.Checked = False
                    End If
                Else
                    trExclusionOfAmishWorkers_row.Attributes.Add("style", "display:none")
                End If

                ' IL Only
                ' Exclusion of sole prop... (IL coverage) MGB 9-27-18
                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
                    trExclusionOfSoleProprietorsEtc_IL_row.Attributes.Add("style", "display:''")
                    Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IL")
                    If ILQuote IsNot Nothing Then
                        chkExclusionOfSoleProprietorsEtc_IL.Checked = ILQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL
                    Else
                        chkExclusionOfSoleProprietorsEtc_IL.Checked = False
                    End If
                Else
                    trExclusionOfSoleProprietorsEtc_IL_row.Attributes.Add("style", "display:none")
                End If

                ' Info row - show all the time for pre-multistate or show it only when exclusion of executive officer... is selected
                trInfo1.Attributes.Add("style", "display:none")
                If chkExclusionOfExecutiveOfficer.Checked = True OrElse IsMultistateCapableEffectiveDate(Quote.EffectiveDate) = False Then
                    'If chkExclusionOfSoleProprietorsEtc_IL.Checked = True OrElse (Not IsMultistateCapableEffectiveDate(Quote.EffectiveDate)) Then
                    ' Only show the info message if Indiana is on the policy MGB 2-6-19
                    If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(SubQuotes, "IN") Then
                        trInfo1.Attributes.Add("style", "display:''")
                    Else
                        trInfo1.Attributes.Add("style", "display:none")
                    End If
                Else
                    trInfo1.Attributes.Add("style", "display:none")
                End If

                ' Rejection of Coverage Endorsement (KY)  added 4/16/19 MGB
                If (IsDate(Quote.EffectiveDate) AndAlso (CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date) AndAlso SubQuotesContainsState("KY")) OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Kentucky Then
                    Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky)
                    trRejectionOfCoverageEndorsementRow.Attributes.Add("style", "display:''")
                    If KYQuote.HasKentuckyRejectionOfCoverageEndorsement Then
                        chkRejectionOfCoverageEndorsement.Checked = True
                    Else
                        chkRejectionOfCoverageEndorsement.Checked = False
                    End If
                Else
                    trRejectionOfCoverageEndorsementRow.Attributes.Add("style", "display:none")
                    chkRejectionOfCoverageEndorsement.Checked = False
                End If

            End If

            Exit Sub
    End Sub

    Private Sub PopulateClassifications()
        ' Classifications
        If Me.Quote.IsNotNull Then
            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' MULTISTATE
                Dim Classifications As List(Of ClassIficationItem_enum) = GetMultistateClassifications()
                rptClassifications.DataSource = Classifications
                rptClassifications.DataBind()

                Dim controlindex As Integer = -1
                Dim clsndx As Integer = -1
                Dim currState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = Nothing
                If Classifications IsNot Nothing AndAlso Classifications.Count > 0 Then currState = Classifications(0).QuickQuoteState

                For Each ci As ClassIficationItem_enum In Classifications
                    controlindex += 1
                    If currState <> ci.QuickQuoteState Then
                        ' State changed
                        currState = ci.QuickQuoteState
                        clsndx = -1
                    End If
                    clsndx += 1

                    Me.FindChildVrControls()
                    Dim ctls As List(Of ctl_WCP_Classification) = GatherChildrenOfType(Of ctl_WCP_Classification)()
                    Dim myWCPControl As ctl_WCP_Classification = ctls(controlindex)
                    'myWCPControl.ClassificationIndex = clsndx
                    'Select Case currState
                    '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                    '        myWCPControl.ClassificationStateId = "15"
                    '        Exit Select
                    '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                    '        myWCPControl.ClassificationStateId = "16"
                    '        Exit Select
                    'End Select
                    'updated 12/28/2018
                    Dim diaStateId As Integer = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(currState)
                    If diaStateId > 0 Then
                        myWCPControl.ClassificationStateId = diaStateId.ToString
                    End If
                    myWCPControl.StateQuoteClassificationIndex = clsndx
                    myWCPControl.ClassificationIndex = controlindex
                    myWCPControl.Populate()
                Next
            Else
                ' SINGLE STATE
                If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                If Quote.Locations.Count = 0 Then
                    Dim newloc As New QuickQuote.CommonObjects.QuickQuoteLocation()
                    newloc.Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                    newloc.Address.StateId = Quote.StateId
                    Quote.Locations.Add(newloc)
                End If
                If Quote.Locations(0).Classifications Is Nothing Then Quote.Locations(0).Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)

                If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing AndAlso Quote.Locations(0).Classifications.Count > 0 Then
                    rptClassifications.DataSource = Quote.Locations(0).Classifications
                    rptClassifications.DataBind()
                Else
                    rptClassifications.DataSource = Nothing
                    rptClassifications.DataBind()
                End If

                Me.FindChildVrControls()
                Dim ClassificationIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_WCP_Classification)
                    cnt.ClassificationIndex = ClassificationIndex
                    cnt.StateQuoteClassificationIndex = ClassificationIndex
                    cnt.ClassificationStateId = Quote.Locations(0).Address.StateId
                    cnt.Populate()
                    ClassificationIndex += 1
                Next
            End If
        End If

        Exit Sub
    End Sub

    ''' <summary>
    ''' Returns a list of all classifications on the quote for multistate
    ''' Classifications are stored at Location 0 on each state quote
    ''' </summary>
    ''' <returns></returns>
    Private Function GetMultistateClassifications() As List(Of ClassIficationItem_enum)
        Dim CLassificationList As New List(Of ClassIficationItem_enum)
        'Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
        'Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
        'Dim INLoc0 As QuickQuote.CommonObjects.QuickQuoteLocation = Nothing
        'Dim ILLoc0 As QuickQuote.CommonObjects.QuickQuoteLocation = Nothing

        'If INQuote IsNot Nothing AndAlso INQuote.Locations IsNot Nothing AndAlso INQuote.Locations.HasItemAtIndex(0) Then
        '    INLoc0 = INQuote.Locations(0)
        'End If
        'If ILQuote IsNot Nothing AndAlso ILQuote.Locations IsNot Nothing AndAlso ILQuote.Locations.HasItemAtIndex(0) Then
        '    ILLoc0 = ILQuote.Locations(0)
        'End If

        '' Add Indiana Classifications
        'If INLoc0 IsNot Nothing Then
        '    If INLoc0.Classifications IsNot Nothing Then
        '        For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In INLoc0.Classifications
        '            Dim clsItem As New ClassIficationItem_enum()
        '            clsItem.Classification = cls
        '            clsItem.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
        '            CLassificationList.AddItem(clsItem)
        '        Next
        '    End If
        'End If

        '' Add Illinois Classifications
        'If ILLoc0 IsNot Nothing Then
        '    If ILLoc0.Classifications IsNot Nothing Then
        '        For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In ILLoc0.Classifications
        '            Dim clsItem As New ClassIficationItem_enum()
        '            clsItem.Classification = cls
        '            clsItem.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
        '            CLassificationList.AddItem(clsItem)
        '        Next
        '    End If
        'End If

        'updated 12/28/2018
        Dim quoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates
        If quoteStates IsNot Nothing AndAlso quoteStates.Count > 0 Then
            For Each qs As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In quoteStates
                Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qs)
                If locsForState IsNot Nothing AndAlso locsForState.Count > 0 AndAlso locsForState(0) IsNot Nothing Then
                    If locsForState(0).Classifications IsNot Nothing AndAlso locsForState(0).Classifications.Count > 0 Then
                        For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In locsForState(0).Classifications
                            Dim clsItem As New ClassIficationItem_enum()
                            clsItem.Classification = cls
                            clsItem.QuickQuoteState = qs
                            CLassificationList.AddItem(clsItem)
                        Next
                    End If
                End If
            Next
        End If

        Return CLassificationList
    End Function

    Private Sub CheckWaiverOfSubro()
        If chkWaiverofSubro.Checked Then
            trNumberOfWaiversRow.Attributes.Add("style", "display:''")
        Else
            trNumberOfWaiversRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured = Nothing
        Dim stateWithExclusionOfSoleProprietorsPartnersOfficersAndOthers As QuickQuoteHelperClass.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.None

        Dim HasAI As Boolean = False

        lblMsg.Text = ""

        If Me.Quote IsNot Nothing Then
            '11/27/2018 - top-level stuff; moved/copied here from below since it's not all applied at state-level
            Me.Quote.ExperienceModificationFactor = txtExpMod.Text '11/27/2018 - moved to top-level section above since it's not set at state-level
            Me.Quote.RatingEffectiveDate = DateTime.Now.ToShortDateString '11/27/2018 - moved to top-level section above since it's not set at state-level
            Me.Quote.ModificationProductionDate = DateTime.Now.ToShortDateString '11/27/2018 - moved to top-level section above since it's not set at state-level
            'sq.AnniversaryRatingEffectiveDate = DateTime.Now.ToShortDateString
            If bdpExpModEffDate.SelectedValue IsNot Nothing Then '11/27/2018 - copied to top-level section above since it's not set at state-level
                Me.Quote.RatingEffectiveDate = bdpExpModEffDate.SelectedDate.ToShortDateString '11/27/2018 note: top-level
                Me.Quote.ModificationProductionDate = ""  ' Dont set Production Date per bug 23129; 11/27/2018 note: top-level
                'sq.AnniversaryRatingEffectiveDate = bdpExpModEffDate.SelectedDate.ToShortDateString '11/27/2018 note: state-level
            Else
                ' Added ELSE 11/7/17 MGB
                Me.Quote.RatingEffectiveDate = "" '11/27/2018 note: top-level
                Me.Quote.ModificationProductionDate = "" '11/27/2018 note: top-level
                'sq.AnniversaryRatingEffectiveDate = "" '11/27/2018 note: state-level
            End If

            ' Clear out dates when exp mod factor is 1; 11/27/2018 - copied from below since it's not all set at state-level
            If IsNumeric(txtExpMod.Text) AndAlso (CDec(txtExpMod.Text) = 0 Or CDec(txtExpMod.Text) = 1) Then
                Me.Quote.RatingEffectiveDate = "" '11/27/2018 note: top-level
                Me.Quote.ModificationProductionDate = "" '11/27/2018 note: top-level
                'sq.AnniversaryRatingEffectiveDate = "" '11/27/2018 note: state-level
            End If
            
            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In Me.SubQuotes
                ' General Info
                sq.EmployersLiabilityId = ddlEmployersLiability.SelectedValue
                'sq.ExperienceModificationFactor = txtExpMod.Text '11/27/2018 - moved to top-level section above since it's not set at state-level
                'sq.RatingEffectiveDate = DateTime.Now.ToShortDateString '11/27/2018 - moved to top-level section above since it's not set at state-level
                'sq.ModificationProductionDate = DateTime.Now.ToShortDateString '11/27/2018 - moved to top-level section above since it's not set at state-level
                sq.AnniversaryRatingEffectiveDate = DateTime.Now.ToShortDateString
                If bdpExpModEffDate.SelectedValue IsNot Nothing Then '11/27/2018 - copied to top-level section above since it's not set at state-level
                    'sq.RatingEffectiveDate = bdpExpModEffDate.SelectedDate.ToShortDateString '11/27/2018 note: top-level
                    'sq.ModificationProductionDate = ""  ' Dont set Production Date per bug 23129; 11/27/2018 note: top-level
                    sq.AnniversaryRatingEffectiveDate = bdpExpModEffDate.SelectedDate.ToShortDateString '11/27/2018 note: state-level
                Else
                    ' Added ELSE 11/7/17 MGB
                    'sq.RatingEffectiveDate = "" '11/27/2018 note: top-level
                    'sq.ModificationProductionDate = "" '11/27/2018 note: top-level
                    sq.AnniversaryRatingEffectiveDate = "" '11/27/2018 note: state-level
                End If

                ' Clear out dates when exp mod factor is 1; 11/27/2018 - copied to top-level section above since it's not set at state-level
                If IsNumeric(txtExpMod.Text) AndAlso (CDec(txtExpMod.Text) = 0 Or CDec(txtExpMod.Text) = 1) Then
                    'sq.RatingEffectiveDate = "" '11/27/2018 note: top-level
                    'sq.ModificationProductionDate = "" '11/27/2018 note: top-level
                    sq.AnniversaryRatingEffectiveDate = "" '11/27/2018 note: state-level
                End If

                ' If the exclusion of sole proprietors... is set, note which state has it for use below  Bug 39704
                If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), stateWithExclusionOfSoleProprietorsPartnersOfficersAndOthers) = False OrElse stateWithExclusionOfSoleProprietorsPartnersOfficersAndOthers = QuickQuoteHelperClass.QuickQuoteState.None Then 'only check for existing state if it hasn’t already been set
                    If (sq.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana OrElse sq.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Kentucky) AndAlso sq.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = True Then 'only checking for it on IN or KY
                        stateWithExclusionOfSoleProprietorsPartnersOfficersAndOthers = sq.QuickQuoteState
                    End If
                End If
                sq.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = False               
            Next

            '11/27/2018 - state-specific stuff; moved here from above
            Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
            If govStateQuote IsNot Nothing Then 'Governing State
                govStateQuote.HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = chkInclusionOfSoleProp.Checked

                If chkBlanketWaiverOfSubro.Checked Then
                    govStateQuote.BlanketWaiverOfSubrogation = "4"
                Else
                    govStateQuote.BlanketWaiverOfSubrogation = ""
                End If

                If chkWaiverofSubro.Checked Then
                    govStateQuote.HasWaiverOfSubrogation = True
                    If IsNumeric(txtNumberOfWaivers.Text) Then
                        govStateQuote.WaiverOfSubrogationNumberOfWaivers = txtNumberOfWaivers.Text
                    Else
                        govStateQuote.WaiverOfSubrogationNumberOfWaivers = 0
                    End If
                    govStateQuote.WaiverOfSubrogationPremiumId = "3"  ' $50
                Else
                    govStateQuote.HasWaiverOfSubrogation = False
                    govStateQuote.WaiverOfSubrogationNumberOfWaivers = 0
                    govStateQuote.WaiverOfSubrogationPremiumId = ""
                End If
            End If

            ' IN/KY
            ' Exclusion of Sole Proprietors, Partners, Officers and Others  Bug 39704
            If chkExclusionOfExecutiveOfficer.Checked = True Then
                Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
                Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("KY")
                Dim QuoteHasIN As Boolean = QuickQuoteHelperClass.QuickQuoteState.Indiana AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteHasState(Quote, QuickQuoteHelperClass.QuickQuoteState.Indiana)
                Dim QuoteHasKY As Boolean = QuickQuoteHelperClass.QuickQuoteState.Indiana AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteHasState(Quote, QuickQuoteHelperClass.QuickQuoteState.Kentucky)

                If stateWithExclusionOfSoleProprietorsPartnersOfficersAndOthers = QuickQuoteHelperClass.QuickQuoteState.Indiana AndAlso QuoteHasIN Then
                    INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = True
                ElseIf stateWithExclusionOfSoleProprietorsPartnersOfficersAndOthers = QuickQuoteHelperClass.QuickQuoteState.Kentucky AndAlso QuoteHasKY = True Then
                    KYQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = True
                ElseIf QuoteHasIN = True Then
                    INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = True
                ElseIf QuoteHasKY = True Then
                    KYQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = True
                End If
            End If

            ' IN/KY
            'If SubQuotesContainsState("IN") OrElse SubQuotesContainsState("KY") Then 'IN & KY only
            '    ' Exclusion of Executive Officer WC 00 03 08
            '    ' Set only on Indiana if it exists
            '    Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
            '    Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("KY")
            '    If INQuote IsNot Nothing Then INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            '    If KYQuote IsNot Nothing Then KYQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            '    If INQuote IsNot Nothing Then
            '        INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = chkExclusionOfExecutiveOfficer.Checked
            '    Else
            '        ' If no IN quote, set on KY if it exists
            '        If KYQuote IsNot Nothing Then
            '            KYQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = chkExclusionOfExecutiveOfficer.Checked
            '        End If
            '    End If
            'End If

            ' IN only
            If SubQuotesContainsState("IN") Then 'IN only
                Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
                If INQuote IsNot Nothing Then
                    INQuote.HasExclusionOfAmishWorkers = chkExclusionOfAmishWorkers.Checked
                End If
            End If

            ' IL only
            If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then 'IL only
                Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IL")
                If ILQuote IsNot Nothing Then
                    ILQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL = chkExclusionOfSoleProprietorsEtc_IL.Checked
                End If
            End If

            ' Added KY MGB 4-16-19
            If (CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date AndAlso SubQuotesContainsState("KY")) OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Kentucky Then 'KY only
                Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("KY")
                If KYQuote IsNot Nothing Then
                    KYQuote.HasKentuckyRejectionOfCoverageEndorsement = chkRejectionOfCoverageEndorsement.Checked
                End If
            End If
        End If

        Me.SaveChildControls()

        If Not IsQuoteEndorsement() Then
            Dim IsFarmIndicator As Boolean = False
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                Dim wcpClassCodesToFind As New List(Of String) From {"0005", "0008", "0016", "0034", "0036", "0037", "0050", "0079", "0083", "0113", "0170", "8279"}
                For Each l As QuickQuoteLocation In Quote.Locations
                    If l IsNot Nothing AndAlso l.Classifications IsNot Nothing AndAlso l.Classifications.Count > 0 Then
                        For Each c As QuickQuoteClassification In l.Classifications
                            If c IsNot Nothing AndAlso String.IsNullOrWhiteSpace(c.ClassCode) = False Then
                                If wcpClassCodesToFind.Contains(c.ClassCode.Trim()) Then
                                    IsFarmIndicator = True
                                    Exit For ' Exit the loop once the target value is found
                                End If
                            End If
                        Next
                    End If
                    If IsFarmIndicator = True Then
                        Exit For '
                    End If
                Next
            End If
            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In Me.SubQuotes
                sq.HasFarmIndicator = IsFarmIndicator
            Next
            Me.Quote.HasFarmIndicator = IsFarmIndicator
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Policy Level Coverages"

        If ddlEmployersLiability.SelectedIndex < 0 Then
            Me.ValidationHelper.AddError(ddlEmployersLiability, "Missing Employers Liability", accordList)
        End If
        If txtExpMod.Text = "" Then
            Me.ValidationHelper.AddError(txtExpMod, "Missing Experience Modification", accordList)
        Else
            ' Exp Mod can be any value > 0
            If Not IsNumeric(txtExpMod.Text) OrElse CDec(txtExpMod.Text) <= 0 Then
                Me.ValidationHelper.AddError(txtExpMod, "Invalid Experience Modification", accordList)
            Else
                ' Exp Mod Eff Date only required if the Exp Mod is greater than 1
                If CDec(txtExpMod.Text) > 1 Then
                    If bdpExpModEffDate.SelectedValue Is Nothing Then
                        Me.ValidationHelper.AddError(bdpExpModEffDate, "Missing Experience Mod. Eff. Date", accordList)
                    End If
                End If
            End If
        End If
        If Me.chkWaiverofSubro.Checked Then
            If Me.txtNumberOfWaivers.Text = "" Then
                Me.ValidationHelper.AddError(txtNumberOfWaivers, "Missing Number of Waivers", accordList)
            Else
                If Not IsNumeric(txtNumberOfWaivers.Text) OrElse CInt(txtNumberOfWaivers.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtNumberOfWaivers, "Invalid Number of Waivers", accordList)
                End If
            End If
        End If
        CheckWaiverOfSubro()

        ' LOCATIONS
        'If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
        '    ' Multistate - must have at least one location per state
        '    If SubQuotesContainsState("IL") Then
        '        If NumberOfLocationsOnState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = 0 Then
        '            Me.ValidationHelper.AddError("You must have at least one location for each state on the quote (IL)")
        '        End If
        '    End If
        '    If SubQuotesContainsState("IN") Then
        '        If NumberOfLocationsOnState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = 0 Then
        '            Me.ValidationHelper.AddError("You must have at least one location for each state on the quote (IN)")
        '        End If
        '    End If
        'Else
        '    ' Single State - must have at least one location
        '    If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 Then
        '        Me.ValidationHelper.AddError("You must have at least one location on the quote")
        '    End If
        'End If

        '' CLASSIFICATIONS
        '' Each state on the quote must have a classification - only applies to Multistate
        'If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
        '    If NumberOfStatesOnQuote > 1 Then
        '        Dim qq As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        '        qq = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
        '        If qq IsNot Nothing Then
        '            If Not StateQuoteHasClassification(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) Then
        '                Me.ValidationHelper.AddError("You have entered a multistate quote but have not entered a classification for each state.  Each state must have at least one classification.")
        '            End If
        '        End If
        '        qq = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
        '        If qq IsNot Nothing Then
        '            If Not StateQuoteHasClassification(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) Then
        '                Me.ValidationHelper.AddError("You have entered a multistate quote but have not entered a classification for each state.  Each state must have at least one classification.")
        '            End If
        '        End If
        '    End If
        'Else
        '    ' Not Multistate - make sure there's at least one classification
        '    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing Then
        '        If Quote.Locations(0).Classifications.Count <= 0 Then
        '            Me.ValidationHelper.AddError("At least one classification is required.")
        '        End If
        '    End If
        'End If

        'updated 12/28/2018
        Dim quoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates
        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
            'multi-state
            Dim statesWithoutLoc As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Nothing
            Dim statesWithoutCls As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Nothing
            For Each qs As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In quoteStates
                If NumberOfLocationsOnState(qs) = 0 Then
                    QuickQuote.CommonMethods.QuickQuoteHelperClass.AddQuickQuoteStateToList(qs, statesWithoutLoc)
                End If
                If StateQuoteHasClassification(qs) = False Then
                    QuickQuote.CommonMethods.QuickQuoteHelperClass.AddQuickQuoteStateToList(qs, statesWithoutCls)
                End If
            Next
            If statesWithoutLoc IsNot Nothing AndAlso statesWithoutLoc.Count > 0 Then
                Me.ValidationHelper.AddError("You must have at least one location for each state on the quote (missing: " & QuickQuote.CommonMethods.QuickQuoteHelperClass.StringOfQuickQuoteStates(statesWithoutLoc, splitter:=", ") & ")")
            End If
            If statesWithoutCls IsNot Nothing AndAlso statesWithoutCls.Count > 0 Then
                Me.ValidationHelper.AddError("You have entered a multistate quote but have not entered a classification for each state.  Each state must have at least one classification.")
            End If
        Else
            'single-state
            If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 OrElse Quote.Locations(0) Is Nothing Then
                Me.ValidationHelper.AddError("You must have at least one location and classification on the quote")
            Else
                If Quote.Locations(0).Classifications Is Nothing OrElse Quote.Locations(0).Classifications.Count <= 0 Then
                    Me.ValidationHelper.AddError("At least one classification is required.")
                End If
            End If
        End If

        ValidateChildControls(valArgs)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Function to determine of the state quote for the passed qqState has classifications.
    ''' Returns true if so, false if not.
    ''' </summary>
    ''' <param name="qqState"></param>
    ''' <returns></returns>
    Private Function StateQuoteHasClassification(ByVal qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Boolean
        'Dim stQt As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(qqState)
        'If stQt IsNot Nothing Then
        '    If stQt.Locations IsNot Nothing AndAlso stQt.Locations.HasItemAtIndex(0) AndAlso stQt.Locations(0).Classifications IsNot Nothing Then
        '        If stQt.Locations(0).Classifications.Count > 0 Then Return True
        '    End If
        'Else
        '    Return False
        'End If
        'updated 12/28/2018
        Dim hasCls As Boolean = False

        Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qqState)
        If locsForState IsNot Nothing AndAlso locsForState.Count > 0 AndAlso locsForState(0) IsNot Nothing AndAlso locsForState(0).Classifications IsNot Nothing AndAlso locsForState(0).Classifications.Count > 0 Then
            hasCls = True
        End If

        Return hasCls
    End Function

    Private Sub AddNewClassification(Optional ByVal qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None) 'added optional event param 12/28/2018
        'Dim qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None 'removed 12/28/2018 after adding event param
        If Quote IsNot Nothing Then
            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' MULTISTATE
                ' We always want to intially add the new classification to whatever is the last state on the last classification in the list
                ' so that the new blank classification is always at the end of the control list

                '' Determine what the state is on the last classification 
                'Dim Classifications As List(Of ClassIficationItem_enum) = GetMultistateClassifications()
                'If Classifications IsNot Nothing AndAlso Classifications.Count > 0 Then
                '    Select Case Classifications(Classifications.Count - 1).QuickQuoteState
                '        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                '            qqState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                '            Exit Select
                '        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                '            qqState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                '            Exit Select
                '    End Select
                'Else
                '    ' No classifcations returned - Add new one to the governing state
                '    If GoverningStateQuote() IsNot Nothing Then
                '        qqState = GoverningStateQuote.QuickQuoteState
                '    Else
                '        ' We should never get here...No Classifications found and no GoverningStateQuote
                '        Throw New Exception("ctl_WCP_Coverage:AddNewClassification - No Classifications or Governing State - Unable to create new classification.")
                '    End If
                'End If

                'Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(qqState)
                'If StateQuote IsNot Nothing Then
                '    If StateQuote.Locations Is Nothing Then StateQuote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                '    If StateQuote.Locations.Count <= 0 Then StateQuote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)
                '    If StateQuote.Locations(0).Classifications Is Nothing Then StateQuote.Locations(0).Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)

                '    Dim newcls As New QuickQuote.CommonObjects.QuickQuoteClassification()
                '    newcls.ClassificationTypeId = "9999999"
                '    StateQuote.Locations(0).Classifications.Add(newcls)

                '    Me.Populate()
                '    Save_FireSaveEvent(False)

                '    Me.hdnAccordClassList.Value = TotalNumberOfClassificationsOnQuote.ToString
                'End If

                'updated 12/28/2018
                '1st default state if needed
                If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), qqState) = False OrElse qqState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                    ' Determine what the state is on the last classification 
                    Dim Classifications As List(Of ClassIficationItem_enum) = GetMultistateClassifications()
                    If Classifications IsNot Nothing AndAlso Classifications.Count > 0 Then
                        qqState = Classifications(Classifications.Count - 1).QuickQuoteState
                    End If

                    If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), qqState) = False OrElse qqState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        qqState = Quote.QuickQuoteState
                    End If

                    If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), qqState) = False OrElse qqState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        ' We should never get here...No Classifications found and no GoverningStateQuote
                        Throw New Exception("ctl_WCP_Coverage:AddNewClassification - No Classifications or Governing State - Unable to create new classification.")
                    End If
                End If

                Dim locForState As QuickQuote.CommonObjects.QuickQuoteLocation = Nothing
                Dim locAddressJustInitialized As Boolean = False
                Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qqState)
                If locsForState IsNot Nothing AndAlso locsForState.Count > 0 Then
                    If locsForState(0) Is Nothing Then
                        locsForState(0) = New QuickQuote.CommonObjects.QuickQuoteLocation
                        locAddressJustInitialized = True
                    End If
                    locForState = locsForState(0)
                Else
                    'add loc and class for state
                    If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                    locForState = New QuickQuote.CommonObjects.QuickQuoteLocation
                    locAddressJustInitialized = True
                    Quote.Locations.Add(locForState)
                End If
                If locForState Is Nothing Then 'redundant
                    locForState = New QuickQuote.CommonObjects.QuickQuoteLocation
                    locAddressJustInitialized = True
                End If
                If locForState.Address Is Nothing Then
                    locForState.Address = New QuickQuote.CommonObjects.QuickQuoteAddress
                    locAddressJustInitialized = True
                End If
                If locAddressJustInitialized = True OrElse QQHelper.IsPositiveIntegerString(locForState.Address.StateId) = False Then
                    locForState.Address.QuickQuoteState = qqState
                End If
                If locForState.Classifications Is Nothing Then
                    locForState.Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)
                End If
                Dim newcls As New QuickQuote.CommonObjects.QuickQuoteClassification()
                newcls.ClassificationTypeId = "9999999"
                locForState.Classifications.Add(newcls)
            Else
                ' SINGLE STATE
                If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                If Quote.Locations.Count <= 0 Then Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)
                If Quote.Locations(0).Classifications Is Nothing Then Quote.Locations(0).Classifications = New List(Of QuickQuote.CommonObjects.QuickQuoteClassification)

                Dim newcls As New QuickQuote.CommonObjects.QuickQuoteClassification()
                newcls.ClassificationTypeId = "9999999"
                Quote.Locations(0).Classifications.Add(newcls)

                'removed 12/28/2018 (and added below) so code could be in the same spot for multiState and singleState
                'Save_FireSaveEvent(False)
                'Me.Populate()

                'Me.hdnAccordClassList.Value = Quote.Locations(0).Classifications.Count - 1.ToString
            End If

            'added 12/28/2018; previously happening above; code now in one spot for multiState and singleState; note this code came from singleState... multiState was calling Populate before Save
            Save_FireSaveEvent(False)
            Me.Populate()

            Me.hdnAccordClassList.Value = TotalNumberOfClassificationsOnQuote.ToString
        End If
        Exit Sub
    End Sub

    Private Sub DeleteClassification(ByVal qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, ByVal ClassificationIndex As String)
        Dim errr As String = Nothing

        'If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
        '    ' MULTISTATE
        '    ' Note that for multistate, the classifcationindex passed should be the index of the classification on the state quote
        '    Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(qqState)
        '    If StateQuote IsNot Nothing Then
        '        If StateQuote.Locations IsNot Nothing AndAlso StateQuote.Locations.HasItemAtIndex(0) Then
        '            If StateQuote.Locations(0).Classifications IsNot Nothing AndAlso StateQuote.Locations(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
        '                StateQuote.Locations(0).Classifications.RemoveAt(ClassificationIndex)
        '                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, errr)
        '                Populate()
        '                'PopulateClassifications()
        '                'Save_FireSaveEvent(False)
        '                'Populate()
        '            End If
        '        End If
        '    End If
        'Else
        '    ' SINGLE STATE
        '    ' For single state quotes classificationindex will be the index of the classification as it appears in the repaeater
        '    If Quote IsNot Nothing Then
        '        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing AndAlso Quote.Locations(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
        '            Quote.Locations(0).Classifications.RemoveAt(ClassificationIndex)
        '            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, errr)
        '            Populate()
        '            'PopulateClassifications()
        '            'Save_FireSaveEvent()
        '            'Populate()
        '        End If
        '    End If
        'End If

        'updated 12/28/2018
        If Quote IsNot Nothing Then
            Dim trySingleStateLogic As Boolean = False
            Dim quoteStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Quote.QuoteStates
            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso quoteStates IsNot Nothing AndAlso quoteStates.Count > 0 AndAlso quoteStates.Contains(qqState) = True Then
                Dim locsForState As List(Of QuickQuote.CommonObjects.QuickQuoteLocation) = QQHelper.LocationsForQuickQuoteState(Quote.Locations, qqState)
                If locsForState IsNot Nothing AndAlso locsForState.Count > 0 AndAlso locsForState(0) IsNot Nothing AndAlso locsForState(0).Classifications IsNot Nothing AndAlso locsForState(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
                    locsForState(0).Classifications.RemoveAt(ClassificationIndex)
                    If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                        'no save
                    ElseIf Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=errr)
                    Else
                        IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, errr)
                    End If
                    Populate()
                    'PopulateClassifications()
                    'Save_FireSaveEvent(False)
                    'Populate()
                Else
                    trySingleStateLogic = True
                End If
            Else
                trySingleStateLogic = True
            End If

            If trySingleStateLogic = True Then
                If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing AndAlso Quote.Locations(0).Classifications.HasItemAtIndex(ClassificationIndex) Then
                    Quote.Locations(0).Classifications.RemoveAt(ClassificationIndex)
                    If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                        'no save
                    ElseIf Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=errr)
                    Else
                        IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, errr)
                    End If
                    Populate()
                    'PopulateClassifications()
                    'Save_FireSaveEvent()
                    'Populate()
                End If
            End If
        End If

    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMain.ClientID
            Me.ListAccordionDivId = Me.divClassificationList.ClientID
        End If

        AttachControlEvents()

        Exit Sub
    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkSaveGeneralInfo.Click, lnkSaveEndorsements.Click, btnSave.Click, btnSaveClassifications.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub lbAddNewClassification_Click(sender As Object, e As EventArgs) Handles lbAddNewClassification.Click
        AddNewClassification()
    End Sub

    Private Sub btnSaveAndRate_Click(sender As Object, e As EventArgs) Handles btnSaveAndRate.Click
        Session("valuationValue") = "False"
        Save_FireSaveEvent(False)
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    Private Sub txtExpMod_TextChanged(sender As Object, e As EventArgs) Handles txtExpMod.TextChanged
        If IsNumeric(txtExpMod.Text) Then
            Dim val As Decimal = CDec(txtExpMod.Text)
            If val = 1 Then
                bdpExpModEffDate.Enabled = False
                lblExpModEffDt.Text = "Experience Mod.Eff. Date"
            Else
                bdpExpModEffDate.Enabled = True
                lblExpModEffDt.Text = "*Experience Mod.Eff. Date"
            End If
        End If
    End Sub

#End Region

End Class