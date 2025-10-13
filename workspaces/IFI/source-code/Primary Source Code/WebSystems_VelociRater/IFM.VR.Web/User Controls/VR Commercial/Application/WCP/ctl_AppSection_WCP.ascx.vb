Public Class ctl_AppSection_WCP
    Inherits VRControlBase

    Public Event QuoteRated()
    Public Event App_Rate_ApplicationRatedSuccessfully()
    Public Event AIChanged()

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.div_master_wcp_app.ClientID
        Me.ListAccordionDivId = Me.divWorkplacesList.ClientID

        AttachControlEvents()

        'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        If Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSaveNI.ClientID)
        Me.VRScript.StopEventPropagation(Me.lbSaveWorkplaces.ClientID)
        Me.VRScript.StopEventPropagation(Me.lbAddNewWorkplace.ClientID)

        ' Create the main accordion
        Me.VRScript.CreateAccordion(MainAccordionDivId, hdn_master_wcp_app, "0")
        ' Create the Workplaces main accordion
        Me.VRScript.CreateAccordion(divWorkplaces.ClientID, hdnAccordWorkplace, "0")
        ' Create the workplace item accordion
        Me.VRScript.CreateAccordion(divWorkplacesList.ClientID, hdnAccordWorkplaceList, "0")

        ' Create the Named Individuals main accordion
        Me.VRScript.CreateAccordion(divNamedIndividuals.ClientID, hdnAccordNamedIndiv, "0")

        ' Create the accordions for each of the named inidividual types
        Me.VRScript.CreateAccordion(divInclOfSoleProprietersEtc.ClientID, hdnInclSoleProprieterList, "0")
        Me.VRScript.CreateAccordion(divWaiverOfSubro.ClientID, hdnWaiverOfSubroList, "0")
        Me.VRScript.CreateAccordion(divExclOfAmish.ClientID, hdnExclOfAmishList, "0")
        Me.VRScript.CreateAccordion(divExclOfSoleOfficer.ClientID, hdnExclOfSoleOfficerList, "0")
        Me.VRScript.CreateAccordion(divExclOfSoleProprietorsEtc_IL.ClientID, hdnExclOfSoleProprietorList_IL, "0") 'added 11/27/2018
        Me.VRScript.CreateAccordion(divRejectionOfCoverageEndorsement.ClientID, hdnRejectionOfCoverageEndorsement, "0") 'added 11/27/2018
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Dim ndx As Integer = 0

        ' *** NAMED INDIVIDUALS
        ' Reset the Named Individuals repeaters
        rptInclOfSoleProprieters.DataSource = Nothing
        rptInclOfSoleProprieters.DataBind()
        rptWaiverOfSubro.DataSource = Nothing
        rptWaiverOfSubro.DataBind()
        rptExclOfAmish.DataSource = Nothing
        rptExclOfAmish.DataBind()
        rptExclOfSoleOfficer.DataSource = Nothing
        rptExclOfSoleOfficer.DataBind()
        rptExclOfSoleProprietorsEtc_IL.DataSource = Nothing 'added 11/28/2018
        rptExclOfSoleProprietorsEtc_IL.DataBind() 'added 11/28/2018

        ' Update the number of workplaces in the Workplaces header
        lblWorkplacesAccordHeader.Text = "Workplace Addresses "
        If Quote.Locations IsNot Nothing Then
            lblWorkplacesAccordHeader.Text = "Workplace Addresses (" & Quote.Locations.Count & ")"
        End If

        ' Hide the Named Individuals sections
        divInclOfSoleProprietersEtc.Attributes.Add("style", "display:none")
        divWaiverOfSubro.Attributes.Add("style", "display:none")
        divExclOfAmish.Attributes.Add("style", "display:none")
        divExclOfSoleOfficer.Attributes.Add("style", "display:none")
        divExclOfSoleProprietorsEtc_IL.Attributes.Add("style", "display:none") 'added 11/27/2018

        ' Only show the named individuals sections if they were selected on the quote side
        'updated 11/27/2018
        Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
        If govStateQuote IsNot Nothing Then 'Governing State
            If govStateQuote.HasInclusionOfSoleProprietorsPartnersOfficersAndOthers Then
                rptInclOfSoleProprieters.DataSource = govStateQuote.InclusionOfSoleProprietorRecords
                rptInclOfSoleProprieters.DataBind()
                divInclOfSoleProprietersEtc.Attributes.Add("style", "display:''")

                ' Set the control properties for each coverage
                ndx = 0
                For Each ri As RepeaterItem In rptInclOfSoleProprieters.Items
                    For Each ctl As Control In ri.Controls
                        If ctl.ClientID.Contains("ctl_NI_InclOfSoleProprietersEtc") Then
                            Dim myctl As ctl_WCP_NamedIndividual = ctl
                            myctl.NamedIndividualIndex = ndx
                            myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.InclusionOfSoleProprietersEtc
                            ndx += 1
                        End If
                    Next
                Next
            End If
            If govStateQuote.HasWaiverOfSubrogation Then
                rptWaiverOfSubro.DataSource = GoverningStateQuote.WaiverOfSubrogationRecords
                rptWaiverOfSubro.DataBind()
                divWaiverOfSubro.Attributes.Add("style", "display:''")

                ' Set the control properties
                ndx = 0
                For Each ri As RepeaterItem In rptWaiverOfSubro.Items
                    For Each ctl As Control In ri.Controls
                        If ctl.ClientID.Contains("ctl_NI_WaiverOfSubro") Then
                            Dim myctl As ctl_WCP_NamedIndividual = ctl
                            myctl.NamedIndividualIndex = ndx
                            myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.WaiverOfSubrogation
                            ndx += 1
                        End If
                    Next
                Next
            End If
        End If

        ' IN/KY
        If SubQuotesContainsState("IN") OrElse SubQuotesContainsState("KY") Then
            ' Exclusion of Sole Proprietors, Partners, Officers and Others
            ' This coverage should be on both IN and KY state parts if they exist
            Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
            If INQuote IsNot Nothing Then
                If INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers Then
                    rptExclOfSoleOfficer.DataSource = INQuote.ExclusionOfSoleProprietorRecords
                    rptExclOfSoleOfficer.DataBind()
                    divExclOfSoleOfficer.Attributes.Add("style", "display:''")

                    ' Set the control properties for each coverage
                    ndx = 0
                    For Each ri As RepeaterItem In rptExclOfSoleOfficer.Items
                        For Each ctl As Control In ri.Controls
                            If ctl.ClientID.Contains("ctl_NI_ExclOfSoleOfficer") Then
                                Dim myctl As ctl_WCP_NamedIndividual = ctl
                                myctl.NamedIndividualIndex = ndx
                                myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfSoleOfficer
                                ndx += 1
                            End If
                        Next
                    Next
                End If
            Else
                Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky)
                If KYQuote IsNot Nothing Then
                    If KYQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers Then
                        rptExclOfSoleOfficer.DataSource = KYQuote.ExclusionOfSoleProprietorRecords
                        rptExclOfSoleOfficer.DataBind()
                        divExclOfSoleOfficer.Attributes.Add("style", "display:''")

                        ' Set the control properties for each coverage
                        ndx = 0
                        For Each ri As RepeaterItem In rptExclOfSoleOfficer.Items
                            For Each ctl As Control In ri.Controls
                                If ctl.ClientID.Contains("ctl_NI_ExclOfSoleOfficer") Then
                                    Dim myctl As ctl_WCP_NamedIndividual = ctl
                                    myctl.NamedIndividualIndex = ndx
                                    myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfSoleOfficer
                                    ndx += 1
                                End If
                            Next
                        Next
                    End If
                End If
            End If
        End If

        ' IN ONLY
        If SubQuotesContainsState("IN") Then
            Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
            If INQuote IsNot Nothing Then
                If INQuote.HasExclusionOfAmishWorkers Then
                    rptExclOfAmish.DataSource = INQuote.ExclusionOfAmishWorkerRecords
                    rptExclOfAmish.DataBind()
                    divExclOfAmish.Attributes.Add("style", "display:''")

                    ' Set the control properties for each coverage
                    ndx = 0
                    For Each ri As RepeaterItem In rptExclOfAmish.Items
                        For Each ctl As Control In ri.Controls
                            If ctl.ClientID.Contains("ctl_NI_ExclOfAmish") Then
                                Dim myctl As ctl_WCP_NamedIndividual = ctl
                                myctl.NamedIndividualIndex = ndx
                                myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfAmishWorkers
                                ndx += 1
                            End If
                        Next
                    Next
                End If
                'If INQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers Then
                '    rptExclOfSoleOfficer.DataSource = INQuote.ExclusionOfSoleProprietorRecords
                '    rptExclOfSoleOfficer.DataBind()
                '    divExclOfSoleOfficer.Attributes.Add("style", "display:''")

                '    ' Set the control properties for each coverage
                '    ndx = 0
                '    For Each ri As RepeaterItem In rptExclOfSoleOfficer.Items
                '        For Each ctl As Control In ri.Controls
                '            If ctl.ClientID.Contains("ctl_NI_ExclOfSoleOfficer") Then
                '                Dim myctl As ctl_WCP_NamedIndividual = ctl
                '                myctl.NamedIndividualIndex = ndx
                '                myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfSoleOfficer
                '                ndx += 1
                '            End If
                '        Next
                '    Next
                'End If
            End If
        End If

        ' IL ONLY
        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
            Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
            If ILQuote IsNot Nothing Then
                If ILQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL Then
                    rptExclOfSoleProprietorsEtc_IL.DataSource = ILQuote.ExclusionOfSoleProprietorRecords_IL
                    rptExclOfSoleProprietorsEtc_IL.DataBind()
                    divExclOfSoleProprietorsEtc_IL.Attributes.Add("style", "display:''")

                    ' Set the control properties for each coverage
                    ndx = 0
                    For Each ri As RepeaterItem In rptExclOfSoleProprietorsEtc_IL.Items
                        For Each ctl As Control In ri.Controls
                            If ctl.ClientID.Contains("ctl_NI_ExclOfSoleProprietor_IL") Then
                                Dim myctl As ctl_WCP_NamedIndividual = ctl
                                myctl.NamedIndividualIndex = ndx
                                myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfSoleProprietor_IL
                                ndx += 1
                            End If
                        Next
                    Next
                End If
            End If
        End If

        ' KY ONLY
        ' Added Rejection of Coverage Endorsement for KY.  MGB 5-2-2019
        If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date AndAlso SubQuotesContainsState("KY") Then
            Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky)
            If KYQuote IsNot Nothing Then
                If KYQuote.HasKentuckyRejectionOfCoverageEndorsement Then
                    rptRejectionOfCoverageEndorsement.DataSource = KYQuote.KentuckyRejectionOfCoverageEndorsementRecords
                    rptRejectionOfCoverageEndorsement.DataBind()
                    divRejectionOfCoverageEndorsement.Attributes.Add("style", "display:''")

                    ' Set the control properties for each coverage
                    ndx = 0
                    For Each ri As RepeaterItem In rptRejectionOfCoverageEndorsement.Items
                        For Each ctl As Control In ri.Controls
                            If ctl.ClientID.Contains("ctl_NI_RejectionOfCoverageEndorsement") Then
                                Dim myctl As ctl_WCP_NamedIndividual = ctl
                                myctl.NamedIndividualIndex = ndx
                                myctl.NIType = ctl_WCP_NamedIndividual.NIType_Enum.RejectionOfCoverageEndorsement
                                ndx += 1
                            End If
                        Next
                    Next
                End If
            End If
        End If

        ' *** WORKPLACES
        rptWorkplaces.DataSource = Nothing
        rptWorkplaces.DataBind()

        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                rptWorkplaces.DataSource = Quote.Locations
                rptWorkplaces.DataBind()

                Me.FindChildVrControls()
                Dim LocIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_WCP_Workplace)
                    cnt.WorkplaceIndex = LocIndex
                    cnt.Populate()
                    LocIndex += 1
                Next
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub AttachControlEvents()
        For Each cntrl As RepeaterItem In Me.rptWorkplaces.Items
            Dim WPControl As ctl_WCP_Workplace = cntrl.FindControl("ctl_Workplaces")
            AddHandler WPControl.AddWorkplaceRequested, AddressOf AddNewWorkplace
            AddHandler WPControl.ClearWorkplaceRequested, AddressOf ClearWorkplace
            AddHandler WPControl.DeleteWorkplaceRequested, AddressOf DeleteWorkplace
        Next
        For Each cntrl As RepeaterItem In Me.rptInclOfSoleProprieters.Items
            Dim NICtl As ctl_WCP_NamedIndividual = cntrl.FindControl("ctl_NI_InclOfSoleProprietersEtc")
            If NICtl IsNot Nothing Then AddHandler NICtl.ChangeNIActiveAccordion, AddressOf SetNamedIndividualCurrentAccordion
        Next
        For Each cntrl As RepeaterItem In Me.rptWaiverOfSubro.Items
            Dim NICtl As ctl_WCP_NamedIndividual = cntrl.FindControl("ctl_NI_WaiverOfSubro")
            If NICtl IsNot Nothing Then AddHandler NICtl.ChangeNIActiveAccordion, AddressOf SetNamedIndividualCurrentAccordion
        Next
        For Each cntrl As RepeaterItem In Me.rptExclOfAmish.Items
            Dim NICtl As ctl_WCP_NamedIndividual = cntrl.FindControl("ctl_NI_ExclOfAmish")
            If NICtl IsNot Nothing Then AddHandler NICtl.ChangeNIActiveAccordion, AddressOf SetNamedIndividualCurrentAccordion
        Next
        For Each cntrl As RepeaterItem In Me.rptExclOfSoleOfficer.Items
            Dim NICtl As ctl_WCP_NamedIndividual = cntrl.FindControl("ctl_NI_ExclOfSoleOfficer")
            If NICtl IsNot Nothing Then AddHandler NICtl.ChangeNIActiveAccordion, AddressOf SetNamedIndividualCurrentAccordion
        Next
        For Each cntrl As RepeaterItem In Me.rptExclOfSoleProprietorsEtc_IL.Items 'added 11/27/2018
            Dim NICtl As ctl_WCP_NamedIndividual = cntrl.FindControl("ctl_NI_ExclOfSoleProprietor_IL")
            If NICtl IsNot Nothing Then AddHandler NICtl.ChangeNIActiveAccordion, AddressOf SetNamedIndividualCurrentAccordion
        Next
        For Each cntrl As RepeaterItem In Me.rptRejectionOfCoverageEndorsement.Items 'added 5/2/2019
            Dim NICtl As ctl_WCP_NamedIndividual = cntrl.FindControl("ctl_NI_RejectionOfCoverageEndorsement")
            If NICtl IsNot Nothing Then AddHandler NICtl.ChangeNIActiveAccordion, AddressOf SetNamedIndividualCurrentAccordion
        Next
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Application"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub AddNewWorkplace()
        ' Note that workplaces are added as locations
        If Quote IsNot Nothing Then
            If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            If Quote.Locations.Count <= 0 Then Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)

            Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)

            Me.Populate()
            Save_FireSaveEvent(False)
            Me.Populate()

            Me.hdnAccordWorkplaceList.Value = Quote.Locations.Count - 1.ToString
        End If
        Exit Sub
    End Sub

    Private Sub DeleteWorkplace(ByVal WorkplaceIndex As String)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= WorkplaceIndex + 1 Then
            Quote.Locations.RemoveAt(WorkplaceIndex)
            Populate()
            Save_FireSaveEvent(False)
            Me.Populate()
        End If
    End Sub

    Private Sub ClearWorkplace(ByVal WorkplaceIndex As String)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= WorkplaceIndex + 1 Then
            Quote.Locations(WorkplaceIndex).Address = New QuickQuote.CommonObjects.QuickQuoteAddress
            Populate()
            Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Private Sub SetNamedIndividualCurrentAccordion(ByVal NIType As ctl_WCP_NamedIndividual.NIType_Enum, ByVal ItemIndex As Integer)
        Select Case NIType
            Case ctl_WCP_NamedIndividual.NIType_Enum.InclusionOfSoleProprietersEtc
                'Me.hdnInclSoleProprieterList.Value = FirstStateQuoteThatIsEligibleForInclusionExclusionCoverages.InclusionOfSoleProprietorRecords.Count - 1.ToString
                'updated 11/28/2018
                Dim val = "0"
                Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                If govStateQuote IsNot Nothing Then
                    val = govStateQuote.InclusionOfSoleProprietorRecords.Count - 1.ToString
                End If
                Me.hdnInclSoleProprieterList.Value = val
                Exit Select
            Case ctl_WCP_NamedIndividual.NIType_Enum.WaiverOfSubrogation
                'Me.hdnWaiverOfSubroList.Value = GoverningStateQuote.WaiverOfSubrogationRecords.Count - 1.ToString
                'updated 11/28/2018
                Dim val = "0"
                Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                If govStateQuote IsNot Nothing Then
                    val = govStateQuote.WaiverOfSubrogationRecords.Count - 1.ToString
                End If
                Me.hdnWaiverOfSubroList.Value = val
                Exit Select
            Case ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfAmishWorkers
                'Me.hdnExclOfAmishList.Value = FirstStateQuoteThatIsEligibleForInclusionExclusionCoverages.ExclusionOfAmishWorkerRecords.Count - 1.ToString
                'updated 11/28/2018
                Dim val = "0"
                Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
                If INQuote IsNot Nothing Then
                    val = INQuote.ExclusionOfAmishWorkerRecords.Count - 1.ToString
                End If
                Me.hdnExclOfAmishList.Value = val
                Exit Select
            Case ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfSoleOfficer
                'Me.hdnExclOfSoleOfficerList.Value = FirstStateQuoteThatIsEligibleForInclusionExclusionCoverages.ExclusionOfSoleProprietorRecords.Count - 1.ToString
                'updated 11/28/2018
                Dim val = "0"
                Dim INQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IN")
                If INQuote IsNot Nothing Then
                    val = INQuote.ExclusionOfSoleProprietorRecords.Count - 1.ToString
                End If
                Me.hdnExclOfSoleOfficerList.Value = val
                Exit Select
            Case ctl_WCP_NamedIndividual.NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                Dim val = "0"
                Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IL")
                If ILQuote IsNot Nothing Then
                    val = ILQuote.ExclusionOfSoleProprietorRecords_IL.Count - 1.ToString
                End If
                Me.hdnExclOfSoleProprietorList_IL.Value = val
                Exit Select
            Case Else
                Exit Select
        End Select

        Exit Sub
    End Sub

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctlApp_Rate.ApplicationRated
        RaiseEvent QuoteRated()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctlApp_Rate.ApplicationRatedSuccessfully
        RaiseEvent App_Rate_ApplicationRatedSuccessfully()
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, lbSaveWorkplaces.Click, lnkSaveNI.Click
        Save_FireSaveEvent()
    End Sub

    Private Sub lbAddNewWorkplace_Click(sender As Object, e As EventArgs) Handles lbAddNewWorkplace.Click
        AddNewWorkplace()
    End Sub

End Class