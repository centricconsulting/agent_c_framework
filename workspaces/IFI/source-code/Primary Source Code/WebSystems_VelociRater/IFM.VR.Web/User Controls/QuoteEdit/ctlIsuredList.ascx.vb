Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Web.Helpers
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports PublicQuotingLib.Models
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlIsuredList
    Inherits VRControlBase

    'Added 1/8/2019 for Bug 30790 MLW
    Public Event AboutToPopulateForStateChange(ByVal isFarmWithCommercialName As Boolean)

    'This could be removed once we change to a checkbox list for addition state selections
    Public ReadOnly Property QuoteWasCreatedInThisSession As Boolean
        Get
            If DirectCast(Me.Page.Master, VelociRater).GetQuotesStartedThisSession() IsNot Nothing And Me.QuoteId.IsNumeric() Then
                Return DirectCast(Me.Page.Master, VelociRater).GetQuotesStartedThisSession.Contains(Me.QuoteId.TryToGetInt32)
            End If
            Return False
        End Get
    End Property

    'This could be removed once we change to a checkbox list for addition state selections
    Public Property PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession As Boolean
        Get
            Dim sessionKey = $"ph_WasSaved_{Me.QuoteId}"
            If Session(sessionKey) Is Nothing Then
                Session(sessionKey) = False
            End If
            Return DirectCast(Session(sessionKey), Boolean)
        End Get
        Set(value As Boolean)
            Session($"ph_WasSaved_{Me.QuoteId}") = value
        End Set
    End Property

    Public Property ActiveInsuredIndex As String
        Get
            Return Me.visibleTabIndex.Value
        End Get
        Set(value As String)
            Me.visibleTabIndex.Value = value
        End Set
    End Property

    Public Property ShowActionButtons As Boolean
        Get
            Return Me.divActionButtons.Visible
        End Get
        Set(value As Boolean)
            Me.divActionButtons.Visible = value
        End Set
    End Property

    Public Property ShowMultiState As Boolean
        Get
            Return Me.MultiStateSection.Visible
        End Get
        Set(value As Boolean)
            Me.MultiStateSection.Visible = value
        End Set
    End Property

    Private _hasMultiStateOption As Nullable(Of Boolean)
    Public Property hasMultiStateOption() As Nullable(Of Boolean)
        Get
            If _hasMultiStateOption Is Nothing Then
                _hasMultiStateOption = IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote)
            End If
            Return _hasMultiStateOption
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _hasMultiStateOption = value
        End Set
    End Property

    Public Property HasOtherStateSelection() As String
        Get
            If String.IsNullOrWhiteSpace(Me.hdnHasOtherStateSelection?.Value) = False Then
                Return Me.hdnHasOtherStateSelection.Value
            End If
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Me.hdnHasOtherStateSelection.Value = value
        End Set
    End Property

    Private ReadOnly Property IsOhioEffective As Boolean
        Get
            'If CDate(Quote.EffectiveDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then Return True
            'updated 5/3/2021
            If Quote IsNot Nothing AndAlso QQHelper.IsValidDateString(Quote.EffectiveDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(Quote.EffectiveDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then Return True
            Return False
        End Get
    End Property

    Private Property chkState1_State As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Get
            If ViewState("vs_chkState1State") Is Nothing Then ViewState("vs_chkState1State") = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
            Return ViewState("vs_chkState1State")
        End Get
        Set(value As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            ViewState("vs_chkState1State") = value
        End Set
    End Property

    Private Property chkState2_State As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Get
            If ViewState("vs_chkState2State") Is Nothing Then ViewState("vs_chkState2State") = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
            Return ViewState("vs_chkState2State")
        End Get
        Set(value As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            ViewState("vs_chkState2State") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = ""
            Me.ListAccordionDivId = accInsuredList.ClientID
            'Me.Populate() 'removed 10/22/2019 (12/11/2019 in Sprint 10); should be called on-demand by individual LOB workflowMgrs
            If Me.Quote IsNot Nothing Then
                Me.btnSubmit.Text = "Save Policyholders"
                Select Case Me.Quote.LobId.TryToGetInt32
                    Case 1 ' auto
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves any policyholders then navigates to the drivers page."
                        Me.btnSaveAndGotoDrivers.Text = "Drivers Page"
                        Me.btnViewGotoDrivers.Text = "Drivers Page"
                    Case 2 ' home
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves any policyholders then navigates to the location page."
                        Me.btnSaveAndGotoDrivers.Text = "Property Page"
                        Me.btnViewGotoDrivers.Text = "Property Page"
                    Case 3 ' DFR
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves any policyholders then navigates to the location page."
                        Me.btnSaveAndGotoDrivers.Text = "Property Page"
                        Me.btnViewGotoDrivers.Text = "Property Page"
                    Case 17, 14 'Farm / Umbrella
                        If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing Then
                            ' no applicants
                            ' you don't set button text here. Farm quotes use a wrapper control to wrap policyholders and applicants into one workflow. That wrapper control has the buttons.
                            If Me.Quote.Policyholder.Name.TypeId = "2" Then
                                ' has applicants
                                Me.ctlInsured1.Visible = False ' hide PH#2 because commercial only have one policyholders
                            End If
                        End If
                        Me.btnViewGotoDrivers.Text = "Policy Level Coverages Page"
                    Case 9 ' CGL
                        Me.ctlInsured1.Visible = False
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves policyholder then navigates to the coverages page."
                        Me.btnSaveAndGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnViewGotoDrivers.Text = "Policy Level Coverages"
                    Case 20 ' CAP
                        ' Commercial Lines don't need policyholder2
                        Me.ctlInsured1.Visible = False
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves policyholder then navigates to the Policy Level Coverages page."
                        Me.btnSaveAndGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnViewGotoDrivers.Text = "Policy Level Coverages"
                    Case 25 ' BOP
                        ' Commercial Lines don't need policyholder2
                        Me.ctlInsured1.Visible = False

                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves policyholder then navigates to the Locations page."
                        Me.btnSaveAndGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnViewGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnSubmit.Text = "Save Policyholder"
                    Case 21 ' WCP
                        ' Commercial Lines don't need policyholder2
                        Me.ctlInsured1.Visible = False

                        'Me.btnSaveAndGotoDrivers.ToolTip = "Saves policyholder then navigates to the Locations page."
                        Me.btnSaveAndGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnViewGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnSubmit.Text = "Save Policyholder"
                    Case 28 ' CPR
                        Me.ctlInsured1.Visible = False
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves policyholder then navigates to the Policy Level Coverages page."
                        Me.btnSaveAndGotoDrivers.Text = "Policy Level Coverages"
                        Me.btnViewGotoDrivers.Text = "Policy Level Coverages"
                    Case 23 ' CPP
                        Me.ctlInsured1.Visible = False
                        Me.btnSaveAndGotoDrivers.ToolTip = "Saves policyholder then navigates to the Policy Level Coverages page."
                        Me.btnSaveAndGotoDrivers.Text = "Property Policy Level Coverages"
                        Me.btnViewGotoDrivers.Text = "Property Policy Level Coverages"
                    Case Else
                End Select



            End If

#If Not DEBUG Then
            Me.txtClientId_Lookup.Attributes.Add("style", "display:none;")
#End If

        End If

        'Updated 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        ''Added 01/23/2020 for eSignature Project MLW - do not show eSignature control on endorsements, for NB only
        'If Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly Then
        '    ctl_Esignature.Visible = False
        'End If
        If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) OrElse Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    ''' <summary>
    ''' Formats the multistate section based on the effective date on the quote
    ''' </summary>
    Private Sub ShowHideMultistate()
        'Me.MultiStateSection.Visible = IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote)
        'ShowMultiState = IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote)
        ShowMultiState = hasMultiStateOption


        ' MGB 4-16-19 Added code for KY WC
        Me.divMultiDefaultOLD.Visible = False
        divMultiDefaultNEW.Visible = False
        Me.divMultiKYWC.Visible = False

        Select Case Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                ' Only show the KY section if the effective date is greater that the KY WC start date
                If Quote IsNot Nothing AndAlso IsDate(Quote.EffectiveDate) Then
                    'Updated 3/8/2022 for KY WCP Task 73087 MLW
                    Select Case Me.Quote.QuickQuoteState
                        Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                            Me.divMultiDefaultNEW.Visible = True
                            Exit Select
                        Case Else
                            If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                                Me.divMultiKYWC.Visible = True
                            Else
                                If IsOhioEffective Then
                                    Me.divMultiDefaultNEW.Visible = True
                                Else
                                    Me.divMultiDefaultOLD.Visible = True
                                End If
                            End If
                            Exit Select
                    End Select
                Else
                    If IsOhioEffective Then
                        Me.divMultiDefaultNEW.Visible = True
                    Else
                        Me.divMultiDefaultOLD.Visible = True
                    End If
                End If
                Exit Select
            Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                'Added 4/7/2022 for bug 74156 MLW
                If GoverningStateQuote.ProgramTypeId IsNot Nothing AndAlso GoverningStateQuote.ProgramTypeId = "5" Then
                    If IsOhioEffective Then
                        Me.divMultiDefaultNEW.Visible = True
                    Else
                        Me.divMultiDefaultOLD.Visible = True
                    End If
                Else
                    ShowMultiState = False
                End If
                Exit Select
            Case Else
                If IsOhioEffective Then
                    Me.divMultiDefaultNEW.Visible = True
                Else
                    Me.divMultiDefaultOLD.Visible = True
                End If
                Exit Select
        End Select
    End Sub

    ''' <summary>
    ''' This differs from Populate in that it uses the passed NewDate parameter for it's date checking instead of quote.effectivedate
    ''' If you make changes to populate you probably need to make them here as well
    ''' 
    ''' This is needed because at the time the treeview triggers the EffectiveDateChanging event the effective date has not been changed on 
    ''' the quote object yet and we need to populate on the new date and not the old date on the quote.
    ''' </summary>
    ''' <param name="NewDate"></param>
    Public Sub PopulateAfterEffectiveDateChange(ByVal NewDate As String)
        Me.txtClientId_Lookup.ToolTip = "Debug Client Id"

        If Me.Quote IsNot Nothing AndAlso Me.Quote.Client IsNot Nothing Then
            Me.txtClientId_Lookup.Text = Me.Quote.Client.ClientId
        End If

        'Me.MultiStateSection.Visible = IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote)
        ShowMultiState = hasMultiStateOption

        Me.divMultiDefaultOLD.Visible = False
        Me.divMultiDefaultNEW.Visible = False
        Me.divMultiKYWC.Visible = False

        'If IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote) Then
        If hasMultiStateOption Then
            Select Case Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    'Updated 3/8/2022 for KY WCP Task 73087 MLW
                    Select Case Me.Quote.QuickQuoteState
                        Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                            Me.divMultiDefaultNEW.Visible = True
                            Exit Select
                        Case Else
                            ' Only show the KY section if the effective date is greater that the KY WC start date
                            If CDate(NewDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                                Me.divMultiKYWC.Visible = True
                            Else
                                If IsOhioEffective Then
                                    Me.divMultiDefaultNEW.Visible = True
                                Else
                                    Me.divMultiDefaultOLD.Visible = True
                                End If
                            End If
                            Exit Select
                    End Select
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                    'Added 4/7/2022 for bug 74156 MLW
                    If GoverningStateQuote.ProgramTypeId IsNot Nothing AndAlso GoverningStateQuote.ProgramTypeId = "5" Then
                        If IsOhioEffective Then
                            Me.divMultiDefaultNEW.Visible = True
                        Else
                            Me.divMultiDefaultOLD.Visible = True
                        End If
                    Else
                        ShowMultiState = False
                    End If
                    Exit Select
                Case Else
                    If IsOhioEffective Then
                        Me.divMultiDefaultNEW.Visible = True
                    Else
                        Me.divMultiDefaultOLD.Visible = True
                    End If
                    Exit Select
            End Select
        End If

        'added 8/8/2018; note: this will change once we have more than 2 states; would likely just load CheckboxList based on QuoteStates... could also use QuoteStateIds if we want to use the stateIds as the listItem values
        'If IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote) Then
        If hasMultiStateOption Then
            Me.lblGoverningState.Text = Me.Quote.State

            Select Case Me.Quote.QuickQuoteState
                Case QuickQuoteHelperClass.QuickQuoteState.Indiana
                    ' GOVERNING STATE INDIANA
                    Select Case Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            ' WORK COMP
                            'Me.divMultiKYWC.Attributes.Add("style", "display:block")
                            If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                                ' With the addition of KY WC, Work Comp has it's own multistate section
                                ' Note that OH does not support WC
                                ' Set the 'Other State' radio button and value  
                                chkOtherState.Visible = True
                                chkOtherState.Text = "Illinois"

                                ' Check or uncheck the state checkboxes
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                    chkOtherState.Checked = True
                                Else
                                    chkOtherState.Checked = False
                                End If
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky) = True Then
                                    chkKentucky.Checked = True
                                Else
                                    chkKentucky.Checked = False
                                End If
                            Else
                                ' Pre-kentucky
                                Me.lblOtherState.Text = "Illinois"
                                If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                        Me.rblHasOtherState.SelectedValue = "Yes"
                                    Else
                                        Me.rblHasOtherState.SelectedValue = "No"
                                    End If
                                End If
                            End If
                            Exit Select
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                            'Added 4/7/2022 for bug 74156 MLW
                            If GoverningStateQuote.ProgramTypeId IsNot Nothing AndAlso GoverningStateQuote.ProgramTypeId = "5" Then
                                If IsOhioEffective Then
                                    ' Use the NEW default layout
                                    'divMultiDefaultNEW.Attributes.Add("style", "display:block")
                                    ' All but Work Comp - default layout: table of checkboxes
                                    Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                                    Me.chkState1.Text = "Illinois"
                                    Me.chkState1.Checked = False
                                    Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Ohio
                                    Me.chkState2.Text = "Ohio"
                                    Me.chkState2.Checked = False

                                    If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                            Me.chkState1.Checked = True
                                        End If
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio) = True Then
                                            Me.chkState2.Checked = True
                                        End If
                                    End If
                                Else
                                    ' Use the OLD default layout
                                    'divMultiDefaultOLD.Attributes.Add("style", "display:block")
                                    Me.lblOtherState.Text = "Illinois"
                                    If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                            Me.rblHasOtherState.SelectedValue = "Yes"
                                        Else
                                            Me.rblHasOtherState.SelectedValue = "No"
                                        End If
                                    End If
                                End If
                            End If
                            Exit Select
                        Case Else
                            ' GOVERNINIG STATE INDIANA - ALL OTHER LOBs
                            If IsOhioEffective Then
                                ' Use the NEW default layout
                                'divMultiDefaultNEW.Attributes.Add("style", "display:block")
                                ' All but Work Comp - default layout: table of checkboxes
                                Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                                Me.chkState1.Text = "Illinois"
                                Me.chkState1.Checked = False
                                Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Ohio
                                Me.chkState2.Text = "Ohio"
                                Me.chkState2.Checked = False

                                If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                        Me.chkState1.Checked = True
                                    End If
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio) = True Then
                                        Me.chkState2.Checked = True
                                    End If
                                End If
                            Else
                                ' Use the OLD default layout
                                'divMultiDefaultOLD.Attributes.Add("style", "display:block")
                                Me.lblOtherState.Text = "Illinois"
                                If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                        Me.rblHasOtherState.SelectedValue = "Yes"
                                    Else
                                        Me.rblHasOtherState.SelectedValue = "No"
                                    End If
                                End If
                            End If
                            Exit Select
                    End Select
                    Exit Select
                Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                    ' GOVERNING STATE ILLINOIS
                    Select Case Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            ' WORK COMP
                            'Me.divMultiKYWC.Attributes.Add("style", "display:block")
                            If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                                ' With the addition of KY WC, Work Comp has it's own multistate section
                                chkOtherState.Visible = True
                                chkOtherState.Text = "Indiana"

                                ' Check or uncheck the state checkboxes
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                    chkOtherState.Checked = True
                                Else
                                    chkOtherState.Checked = False
                                End If
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky) = True Then
                                    chkKentucky.Checked = True
                                Else
                                    chkKentucky.Checked = False
                                End If
                            Else
                                ' Effective date less than the KY WC start date, use the old logic 
                                Me.lblOtherState.Text = "Indiana"
                                If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                        Me.rblHasOtherState.SelectedValue = "Yes"
                                    Else
                                        Me.rblHasOtherState.SelectedValue = "No"
                                    End If
                                End If
                            End If
                            Exit Select
                        Case Else
                            ' ALL OTHER LOBs
                            If CDate(Quote.EffectiveDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
                                ' After OH start date, use the NEW default layout
                                divMultiDefaultNEW.Attributes.Add("style", "display:block")
                                ' All but Work Comp - default layout: table of checkboxes
                                Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Indiana
                                Me.chkState1.Text = "Indiana"
                                Me.chkState1.Checked = False
                                Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Ohio
                                Me.chkState2.Text = "Ohio"
                                Me.chkState2.Checked = False

                                If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                        Me.chkState1.Checked = True
                                    End If
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio) = True Then
                                        Me.chkState2.Checked = True
                                    End If
                                End If
                            Else
                                ' Pre OH start date, use the OLD default layout
                                'divMultiDefaultOLD.Attributes.Add("style", "display:block")
                                Me.lblOtherState.Text = "Indiana"
                                If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                    If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                        Me.rblHasOtherState.SelectedValue = "Yes"
                                    Else
                                        Me.rblHasOtherState.SelectedValue = "No"
                                    End If
                                End If
                                Exit Select
                            End If
                            Exit Select
                    End Select
                    Exit Select
                Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                    ' GOVERNING STATE OHIO
                    ' Use the NEW default layout
                    'divMultiDefaultNEW.Attributes.Add("style", "display:block")
                    Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                    Me.chkState1.Text = "Illinois"
                    Me.chkState1.Checked = False
                    Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Indiana
                    Me.chkState2.Text = "Indiana"
                    Me.chkState2.Checked = False

                    If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                            Me.chkState1.Checked = True
                        End If
                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                            Me.chkState2.Checked = True
                        End If
                    End If
                    Exit Select
                Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                    'Added 3/8/2022 for KY WCP Task 73087 MLW
                    Select Case Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                            Me.chkState1.Text = "Illinois"
                            Me.chkState1.Checked = False
                            Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Indiana
                            Me.chkState2.Text = "Indiana"
                            Me.chkState2.Checked = False

                            If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                    Me.chkState1.Checked = True
                                End If
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                    Me.chkState2.Checked = True
                                End If
                            End If
                            Exit Select
                        Case Else
                            'No other KY Governing State LOBs yet
                            Exit Select
                    End Select
                    Exit Select
            End Select
        End If
        Me.PopulateChildControls()
        NewCoMessage.Visible = False
        If NewCompanyIdHelper.isNewCompanyLocked(Quote) Then
            NewCoMessage.Text = "You must contact your Underwriter for assistance or start a new quote."
            NewCoMessage.Visible = True
            DisableMultistateSelectionsByState_NewCo()
        End If
    End Sub

    Public Overrides Sub Populate()
#If DEBUG Then
        Me.txtClientId_Lookup.ToolTip = "Debug Client Id"
#End If
        'Added 04/12/2021 for CAP Endorsements Task 52971 MLW
        ctl_RouteToUw.Visible = False
        If Me.Quote IsNot Nothing Then 'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
            'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
            If AllowPopulate() Then
                If WebHelper_Personal.ControlVisibilityIsOkayForCommercialDataPrefillFirmographicsPreload(Me.Visible) = True AndAlso Me.HasAttemptedCommercialDataPrefillFirmographicsPreload = False AndAlso WebHelper_Personal.WorkflowIsOkayForCommercialDataPrefillCalls(Me.Quote, request:=Request) = True Then
                    'note: will just call preload now if needed so we know it will be called before the actual Prefill call and we don't have to wait until a button click
                    Dim ih As New IntegrationHelper
                    Dim attemptedServiceCall As Boolean = False
                    Dim caughtUnhandledException As Boolean = False
                    Dim unhandledExceptionToString As String = ""
                    ih.CallCommercialDataPrefill_FirmographicsOnly_Preload_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                    Me.HasAttemptedCommercialDataPrefillFirmographicsPreload = True
                    If caughtUnhandledException = True Then
                        Dim preloadError As New IFM.ErrLog_Parameters_Structure()
                        Dim addPreloadInfo As String = WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
                        With preloadError
                            .ApplicationName = "Velocirater Personal"
                            .ClassName = "ctlIsuredList"
                            .ErrorMessage = unhandledExceptionToString
                            .LogDate = DateTime.Now
                            .RoutineName = "Populate"
                            .StackTrace = "at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly_Preload at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly_Preload_IfNeeded"
                            .AdditionalInfo = addPreloadInfo
                        End With
                        WriteErrorLogRecord(preloadError, "")
                    End If
                End If

                'If Me.Quote IsNot Nothing AndAlso Me.Quote.Client IsNot Nothing Then
                '    Me.txtClientId_Lookup.Text = Me.Quote.Client.ClientId
                'End If
                'updated 5/1/2019
                'If Me.Quote IsNot Nothing Then
                Dim hasClientId As Boolean = False
                If Me.Quote.Client IsNot Nothing AndAlso Me.Quote.Client.HasValidClientId() = True Then
                    hasClientId = True
                End If
                If hasClientId = True Then
                    Me.txtClientId_Lookup.Text = Me.Quote.Client.ClientId
                    IFM.VR.Common.QuoteSave.QuoteSaveHelpers.AddToClientIdsInSessionFromPopulate(QQHelper.IntegerForString(Me.Quote.Client.ClientId))
                Else
                    Me.txtClientId_Lookup.Text = ""
                    IFM.VR.Common.QuoteSave.QuoteSaveHelpers.AddToQuoteIdsInSessionWithoutClientId(QQHelper.IntegerForString(Me.QuoteId))
                End If
                'End If

                ' Shoe the appropriate multistate section format
                ShowHideMultistate()

                'added 8/8/2018; note: this will change once we have more than 2 states; would likely just load CheckboxList based on QuoteStates... could also use QuoteStateIds if we want to use the stateIds as the listItem values
                ' If IFM.VR.Common.Helpers.MultiState.General.QuoteHasMultistateOptionsAvailable(Me.Quote) Then 'ShowMultiState was set in call above.
                'If ShowMultiState Then
                If hasMultiStateOption Then
                    Me.lblGoverningState.Text = Me.Quote.State

                    Select Case Me.Quote.QuickQuoteState
                        Case QuickQuoteHelperClass.QuickQuoteState.Indiana
                            ' GOVERNING STATE INDIANA
                            Select Case Quote.LobType
                                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                                    ' WORK COMP
                                    'Me.divMultiKYWC.Attributes.Add("style", "display:block")
                                    If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                                        ' With the addition of KY WC, Work Comp has it's own multistate section
                                        ' Note that OH does not support WC
                                        ' Set the 'Other State' radio button and value  
                                        chkOtherState.Visible = True
                                        chkOtherState.Text = "Illinois"

                                        ' Check or uncheck the state checkboxes
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                            chkOtherState.Checked = True
                                        Else
                                            chkOtherState.Checked = False
                                        End If
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky) = True Then
                                            chkKentucky.Checked = True
                                        Else
                                            chkKentucky.Checked = False
                                        End If
                                    Else
                                        ' Pre-kentucky
                                        Me.lblOtherState.Text = "Illinois"
                                        If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                                Me.rblHasOtherState.SelectedValue = "Yes"
                                            Else
                                                Me.rblHasOtherState.SelectedValue = "No"
                                            End If
                                        End If
                                    End If
                                    Exit Select
                                Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                                    'Added 4/7/2022 for bug 74156 MLW
                                    If GoverningStateQuote.ProgramTypeId IsNot Nothing AndAlso GoverningStateQuote.ProgramTypeId = "5" Then
                                        If IsOhioEffective Then
                                            ' Use the NEW default layout
                                            'divMultiDefaultNEW.Attributes.Add("style", "display:block")
                                            ' All but Work Comp - default layout: table of checkboxes
                                            Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                                            Me.chkState1.Text = "Illinois"
                                            ' Me.chkState1.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/22/2021
                                            Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Ohio
                                            Me.chkState2.Text = "Ohio"
                                            ' Me.chkState2.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/22/2021

                                            If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                                    Me.chkState1.Checked = True
                                                End If
                                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio) = True Then
                                                    Me.chkState2.Checked = True
                                                End If
                                            End If
                                        Else
                                            ' Use the OLD default layout
                                            'divMultiDefaultOLD.Attributes.Add("style", "display:block")
                                            Me.lblOtherState.Text = "Illinois"
                                            If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                                    Me.rblHasOtherState.SelectedValue = "Yes"
                                                Else
                                                    Me.rblHasOtherState.SelectedValue = "No"
                                                End If
                                            End If
                                        End If
                                    End If
                                    Exit Select
                                Case Else
                                    ' GOVERNINIG STATE INDIANA - ALL OTHER LOBs
                                    If IsOhioEffective Then
                                        ' Use the NEW default layout
                                        'divMultiDefaultNEW.Attributes.Add("style", "display:block")
                                        ' All but Work Comp - default layout: table of checkboxes
                                        Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                                        Me.chkState1.Text = "Illinois"
                                        ' Me.chkState1.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/22/2021
                                        Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Ohio
                                        Me.chkState2.Text = "Ohio"
                                        ' Me.chkState2.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/22/2021

                                        If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                                Me.chkState1.Checked = True
                                            End If
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio) = True Then
                                                Me.chkState2.Checked = True
                                            End If
                                        End If
                                    Else
                                        ' Use the OLD default layout
                                        'divMultiDefaultOLD.Attributes.Add("style", "display:block")
                                        Me.lblOtherState.Text = "Illinois"
                                        If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                                Me.rblHasOtherState.SelectedValue = "Yes"
                                            Else
                                                Me.rblHasOtherState.SelectedValue = "No"
                                            End If
                                        End If
                                    End If
                                    Exit Select
                            End Select
                            Exit Select
                        Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                            ' GOVERNING STATE ILLINOIS
                            Select Case Quote.LobType
                                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                                    ' WORK COMP
                                    'Me.divMultiKYWC.Attributes.Add("style", "display:block")
                                    If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                                        ' With the addition of KY WC, Work Comp has it's own multistate section
                                        chkOtherState.Visible = True
                                        chkOtherState.Text = "Indiana"

                                        ' Check or uncheck the state checkboxes
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                            chkOtherState.Checked = True
                                        Else
                                            chkOtherState.Checked = False
                                        End If
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky) = True Then
                                            chkKentucky.Checked = True
                                        Else
                                            chkKentucky.Checked = False
                                        End If
                                    Else
                                        ' Effective date less than the KY WC start date, use the old logic 
                                        Me.lblOtherState.Text = "Indiana"
                                        If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                                Me.rblHasOtherState.SelectedValue = "Yes"
                                            Else
                                                Me.rblHasOtherState.SelectedValue = "No"
                                            End If
                                        End If
                                    End If
                                    Exit Select
                                Case Else
                                    ' ALL OTHER LOBs
                                    If CDate(Quote.EffectiveDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
                                        ' After OH start date, use the NEW default layout
                                        divMultiDefaultNEW.Attributes.Add("style", "display:block")
                                        ' All but Work Comp - default layout: table of checkboxes
                                        Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Indiana
                                        Me.chkState1.Text = "Indiana"
                                        ' Me.chkState1.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/22/2021
                                        Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Ohio
                                        Me.chkState2.Text = "Ohio"
                                        ' Me.chkState2.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/22/2021

                                        If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                                Me.chkState1.Checked = True
                                            End If
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio) = True Then
                                                Me.chkState2.Checked = True
                                            End If
                                        End If
                                    Else
                                        ' Pre OH start date, use the OLD default layout
                                        'divMultiDefaultOLD.Attributes.Add("style", "display:block")
                                        Me.lblOtherState.Text = "Indiana"
                                        If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                            If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                                Me.rblHasOtherState.SelectedValue = "Yes"
                                            Else
                                                Me.rblHasOtherState.SelectedValue = "No"
                                            End If
                                        End If
                                        Exit Select
                                    End If
                                    Exit Select
                            End Select
                            Exit Select
                        Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                            ' GOVERNING STATE OHIO
                            ' Use the NEW default layout
                            'divMultiDefaultNEW.Attributes.Add("style", "display:block")
                            Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                            Me.chkState1.Text = "Illinois"
                            'Me.chkState1.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/21/2021
                            Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Indiana
                            Me.chkState2.Text = "Indiana"
                            'Me.chkState2.Checked = False 'Bug 59833:OH - Additional state checkbox gets unchecked for Multi-state farm policy BB 04/21/2021

                            If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                    Me.chkState1.Checked = True
                                End If
                                If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                    Me.chkState2.Checked = True
                                End If
                            End If
                            Exit Select
                        Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                            'Added 3/8/2022 for KY WCP Task 73087 MLW
                            Select Case Quote.LobType
                                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                                    Me.chkState1_State = QuickQuoteHelperClass.QuickQuoteState.Illinois
                                    Me.chkState1.Text = "Illinois"
                                    Me.chkState2_State = QuickQuoteHelperClass.QuickQuoteState.Indiana
                                    Me.chkState2.Text = "Indiana"

                                    If Me.QuoteWasCreatedInThisSession = False Or (Me.QuoteWasCreatedInThisSession AndAlso Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession) Then
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                                            Me.chkState1.Checked = True
                                        End If
                                        If Me.Quote.QuoteStates.Contains(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                                            Me.chkState2.Checked = True
                                        End If
                                    End If
                                    Exit Select
                                Case Else
                                    'No other KY Governing State LOBs yet
                                    Exit Select
                            End Select
                    End Select
                End If

                'If Me.Quote IsNot Nothing Then
                If IsQuoteReadOnly() Then
                    Dim policyNumber As String = Me.Quote.PolicyNumber
                    Dim imageNum As Integer = 0
                    Dim policyId As Integer = 0
                    Dim toolTip As String = "Make a change to this policy"
                    'Dim qqHelper As New QuickQuoteHelperClass
                    Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                    'QuickQuoteHelperClass.configAppSettingValueAsString("")  'Unused CAH 07/21/2020
                    If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                        readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                    End If

                    divActionButtons.Visible = False
                    divEndorsementButtons.Visible = False

                    If Quote.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                        divEndorsementButtons.Visible = True
                        btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                        readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                        btnMakeAChange.ToolTip = toolTip
                        btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
                    End If
                End If

                If IsQuoteEndorsement() Then
                    Select Case Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                            divEndosementMessage.Visible = True
                            endoMessage.Text = "If you are adding locations in a state you previously did not have locations within, please contact your farm underwriter for assistance."
                            divMultiDefaultOLD.Disabled = True
                        Case Else
                            divEndosementMessage.Visible = False
                            endoMessage.Text = ""
                    End Select
                End If

                'Added 11/16/2020 for CAP Endorsements task 52971 MLW
                If Me.IsQuoteEndorsement Then
                    Select Case Me.Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                            Me.rblHasOtherState.Enabled = False
                            btnRatePolicyholder.Visible = True
                            btnSaveAndGotoDrivers.Visible = False

                            'Added 04/12/2021 for CAP Endorsements task 52971 MLW
                            Dim hasDiamondError As Boolean = False
                            If Me.Quote.ValidationItems IsNot Nothing AndAlso Me.Quote.ValidationItems.Count > 0 Then
                                Dim diaErrors = (From v In Me.Quote.ValidationItems Where v.ValidationSeverityType = QuickQuote.CommonObjects.QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError Select v)
                                If diaErrors IsNot Nothing AndAlso diaErrors.Count > 0 Then
                                    For Each ve In diaErrors
                                        If ve.Message.Contains("Invalid county entered (COOK)") Then
                                            hasDiamondError = True
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                            ctl_RouteToUw.Visible = hasDiamondError
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                            DisableMultistateSelections()
                            btnRatePolicyholder.Visible = True
                            btnSaveAndGotoDrivers.Visible = False
                            DisableMultistateSelections()
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                            btnRatePolicyholder.Visible = True
                            btnSaveAndGotoDrivers.Visible = False
                            DisableMultistateSelections()





                        Case Else
                            'show all
                    End Select


                Else
                    btnRatePolicyholder.Visible = False
                    btnSaveAndGotoDrivers.Visible = True
                End If
                'End If

                Me.PopulateChildControls()
            End If
            NewCoMessage.Visible = False
            If NewCompanyIdHelper.isNewCompanyLocked(Quote) Then
                NewCoMessage.Text = "You must contact your Underwriter for assistance or start a new quote."
                NewCoMessage.Visible = True
                DisableMultistateSelectionsByState_NewCo()
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
        If AllowValidateAndSave() Then
            MyBase.ValidateControl(valArgs)
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Me.ValidationHelper.GroupName = "Policyholders"

            If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                ' WORK COMP
                If divMultiKYWC.Visible Then
                    ' for WCP we don't care whether they select an additional state or not
                    'Dim hasSelectedMultstateValue = Me.chkOtherState.Checked Or Me.chkKentucky.Checked
                    'If Not hasSelectedMultstateValue Then
                    '    Me.ValidationHelper.AddError(Me.rblHasOtherState, "No additional state risk selection made.", accordList)
                    'End If
                End If
            Else
                ' ALL OTHER LOBs & Pre-KY WCP
                If IsOhioEffective Then
                    ' In the case of the new post-ohio format the user is not required to check a state checkbox
                    ' so no validation is necessary
                Else
                    ' In the case of a pre-ohio non work-comp quote we need to make sure either yes or no is selected for the other state
                    If MultiStateSection.Visible Then
                        Dim hasSelectedMultstateValue = Me.rblHasOtherState.SelectedValue = "Yes" Or Me.rblHasOtherState.SelectedValue = "No"
                        If hasSelectedMultstateValue = False Then
                            Me.ValidationHelper.AddError(Me.rblHasOtherState, "No additional state risk selection made.", accordList)
                        End If
                    End If
                End If
            End If

            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                'Updated 11/30/18 for multi state MLW - Bug 29997
                If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                    'If Me.GoverningStateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                    If IFM.VR.Common.Helpers.States.DoesStateIdExist(CInt(Me.Quote?.Policyholder?.Address?.StateId)) Then
                        If Me.Quote.StateId <> Me.Quote?.Policyholder?.Address?.StateId Then
                            Me.ValidationHelper.AddError($"The Policyholder state entered does not match the governing state selected.")
                        End If
                    End If
                    'End If
                End If
            End If
            Me.ValidateChildControls(valArgs)

            If Me.ValidationSummmary.HasErrors() = False Then
                Me.PolicyholderSaveHasBeenCompletedForThisQuoteInThisSession = True
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddVariableLine("var InsuredsListControlDivTopMost = '" + Me.InsuredsListControlDivTopMost.ClientID + "';")
        Me.VRScript.AddVariableLine("function ShowPolicyHolder(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}")
        Me.VRScript.CreateAccordion(ListAccordionDivId, visibleTabIndex, "0")
        If CAP.UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            chkState1.Attributes.Add("onchange", "MultistateCheckboxChanged('State1','" & chkState1.ClientID & "','" & chkState2.ClientID & "');")
            chkState2.Attributes.Add("onchange", "MultistateCheckboxChanged('State2','" & chkState1.ClientID & "','" & chkState2.ClientID & "');")
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 12/22/2020 for CAP Endorsements Task 52971 MLW
        If AllowValidateAndSave() Then
            ' THESE MUST BE FIRST
            Me.SaveChildControls()
            'Me.ctlInsured.Save()
            'Me.ctlInsured1.Save()
            ' THESE MUST BE FIRST


            '  holding sensitive info like this is only OK because we are on agents-only - on the public site this is to dangerous
            Dim clientSetFromExisting As Boolean = False '8-1-14
            If Me.Quote IsNot Nothing Then
                Me.InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave = False

                If Me.Quote.Client IsNot Nothing Then
                    If String.IsNullOrWhiteSpace(Me.txtClientId_Lookup.Text) = False Then
                        If Me.Quote.Client.ClientId <> Me.txtClientId_Lookup.Text Then
                            ' only do this if it has changed 8-1-14
                            clientSetFromExisting = True
                        End If
                    End If
                    Me.Quote.Client.ClientId = Me.txtClientId_Lookup.Text '5-30-14
                End If
                Select Case Me.Quote.LobId.TryToGetInt32
                    Case 1 ' auto
                        'Me.Quote.CopyPolicyholder1NameAddressEmailsAndPhonesToClient() 'removed 7-21-14
                        Me.Quote.CopyPolicyholdersToClients()
                    Case 2 ' home
                        'Me.Quote.CopyPolicyholder1NameAddressEmailsAndPhonesToClient() 'removed 7-21-14
                        Me.Quote.CopyPolicyholdersToClients()
                        Me.Quote.CopyPolicyholdersToApplicants()
                        Me.Quote.CopyPolicyholdersToOperators() ' 11-3-14

                        'MatureHomowner is given to those over 50 years old
                        If IsQuoteEndorsement() = False Then 'CAH B51742 Do not change an endorsement's value.
                            Me.Quote.Locations(0).MatureHomeownerDiscount = False ' default to false
                            Dim MatureCreditMinAge = 50
                            If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing Then
                                If IsDate(Me.Quote.Policyholder.Name.BirthDate) Then
                                    If CDate(Me.Quote.Policyholder.Name.BirthDate) <= DateTime.Now.AddYears(-MatureCreditMinAge) Then
                                        Me.Quote.Locations(0).MatureHomeownerDiscount = True
                                    End If
                                End If
                            End If

                            If Me.Quote.Policyholder2 IsNot Nothing AndAlso Me.Quote.Policyholder2.Name IsNot Nothing Then
                                If IsDate(Me.Quote.Policyholder2.Name.BirthDate) Then
                                    If CDate(Me.Quote.Policyholder2.Name.BirthDate) <= DateTime.Now.AddYears(-MatureCreditMinAge) Then
                                        Me.Quote.Locations(0).MatureHomeownerDiscount = True
                                    End If
                                End If
                            End If
                        End If


                        If clientSetFromExisting Then
                            ' defaults the multipolicy discount if it applies 8-1-14
                            HasExistingAutoPolicyorQuote()
                        End If
                ' and copy to applicants

                'Case 17 'farm
                'updated 10/1/2021 for Umbrella
                    Case 17, 14 'farm, Umbrella Personel
                        Me.Quote.CopyPolicyholdersToClients()

                        If Me.Quote.Policyholder IsNot Nothing Then
                            If Me.Quote.Policyholder.Name IsNot Nothing Then
                                If Me.Quote.Policyholder.Name.TypeId <> "2" Then
                                    ' copy clients to applicants if they are personal policyholder names ??? Matt A -2-18-15
                                    Me.Quote.CopyPolicyholdersToApplicants()
                                Else
                                    'comm - this logic is handled on the applicant controls as they create the applicants rather than copy them from policyholders

                                End If
                            End If
                        End If

                    Case 3 'DFR
                        Me.Quote.CopyPolicyholdersToClients()
                        Me.Quote.CopyPolicyholdersToApplicants()

                    Case 9 ' CGL
                        Me.Quote.CopyPolicyholdersToClients()

                    Case Else
                        'Me.Quote.CopyPolicyholder1NameAddressEmailsAndPhonesToClient() 'removed 7-21-14
                        Me.Quote.CopyPolicyholdersToClients()
                End Select

                'added 5/1/2019; updated 5/2/2019 to only include clientIds that came from Populate as to exclude ones loaded from Client Search (though it could have come from Client Search and then showed up on Populate later due to reload)... more logic added but may not need it all (depends on whether or not we want to try to limit updates to client created for current quote); IsNewClientIdInSessionFromDiamondSave should ensure that the clientId was created during the current user session; IsQuoteIdInSessionWithoutClientId just indicates that the quote was populated without a clientId during the current session; IsClientIdInSessionFromPopulate indicates that the clientId was populated for some quote during the user session (not necessarily this quote)
                'If Me.Quote.Client IsNot Nothing AndAlso Me.Quote.Client.HasValidClientId() = True AndAlso IsQuoteIdInSessionWithoutClientId(QQHelper.IntegerForString(Me.QuoteId)) = True AndAlso IsClientIdInSessionFromPopulate(QQHelper.IntegerForString(Me.Quote.Client.ClientId)) = True AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.IsNewClientIdInSessionFromDiamondSave(QQHelper.IntegerForString(Me.Quote.Client.ClientId)) = True Then
                '    Me.Quote.Client.OverwriteClientInfoForDiamondId = True
                'End If
                'updated 5/6/2019 to call method from QuoteSaveHelpers
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.CheckQuoteForClientOverwrite(Me.Quote, quoteId:=QQHelper.IntegerForString(Me.QuoteId))

                'added 8/8/2018; note: need to make sure other screens are re-populated if states change
                ' If Me.MultiStateSection.Visible = True Then
                If ShowMultiState Then
                    Dim previousStateIds As List(Of Integer) = Me.Quote.QuoteStateIds 'added 1/8/2019 for Bug 30790 MLW
                    Dim qqStates As New List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
                    qqStates.Add(Me.Quote.QuickQuoteState)

                    Select Case Quote.LobType
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            Select Case Me.Quote.QuickQuoteState
                                Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                                    'Added 3/8/2022 for KY WCP Task 73087 MLW - WCP KY Gov State
                                    If Me.chkState1.Checked Then qqStates.Add(chkState1_State)
                                    If Me.chkState2.Checked Then qqStates.Add(chkState2_State)
                                    Exit Select
                                Case Else
                                    If divMultiKYWC.Visible Then
                                        ' KY Multistate is in effect
                                        If chkOtherState.Visible AndAlso chkOtherState.Text.ToUpper = "INDIANA" Then
                                            If chkOtherState.Checked Then
                                                ' Add Indiana
                                                qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                                            End If
                                        End If
                                        If chkOtherState.Visible AndAlso chkOtherState.Text.ToUpper = "ILLINOIS" Then
                                            If chkOtherState.Checked Then
                                                ' Add Illinois
                                                qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                                            End If
                                        End If
                                        If chkKentucky.Visible Then
                                            If chkKentucky.Checked Then
                                                ' Add Kentucky
                                                qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky)
                                            End If
                                        End If
                                    Else
                                        ' KY multistate is NOT in effect
                                        If Me.rblHasOtherState.SelectedValue = "Yes" Then
                                            Select Case Me.Quote.QuickQuoteState
                                                Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                                    qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                                                Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                                    qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                                            End Select
                                        End If
                                    End If
                                    Exit Select
                            End Select
                            Exit Select
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                            'Bug 59833:Additional state checkbox gets unchecked for Multi-state farm policy 04/06/2021 'BB
                            If IsOhioEffective Then
                                If Me.chkState1.Checked Then qqStates.Add(chkState1_State)
                                If Me.chkState2.Checked Then qqStates.Add(chkState2_State)


                            Else
                                If Me.rblHasOtherState.SelectedValue = "Yes" OrElse (IsQuoteEndorsement() AndAlso HasOtherStateSelection = "Yes") Then
                                    Select Case Me.Quote.QuickQuoteState
                                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                            qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                            qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                                    End Select
                                End If
                                Exit Select
                            End If
                        Case Else
                            ' All LOB's except Work Comp
                            If IsOhioEffective Then
                                ' Post-Ohio default format
                                If Me.chkState1.Checked Then qqStates.Add(chkState1_State)
                                If Me.chkState2.Checked Then qqStates.Add(chkState2_State)
                            Else
                                ' Original (pre-ohio) default format
                                If Me.rblHasOtherState.SelectedValue = "Yes" Then
                                    Select Case Me.Quote.QuickQuoteState
                                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                            qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                            qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                                    End Select
                                End If
                            End If
                            Exit Select
                    End Select

                    Me.Quote.Set_QuoteStates(qqStates)
                    Dim newStateIds As List(Of Integer) = Me.Quote.QuoteStateIds 'added 1/8/2019 for Bug 30790 MLW
                    'updated 1/8/2019 for Bug 30790 MLW
                    'If Not IsOnAppPage Then
                    If Not IsOnAppPage AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.IntegerListsMatch(previousStateIds, newStateIds) = False Then
                        Dim isFarmWithCommercialName As Boolean = False 'Added 1/8/2019 for Bug 30790 MLW
                        'Added 1/8/2019 for Bug 30790 MLW
                        If Me.Quote.LobId.TryToGetInt32 = 17 AndAlso Me.Quote.Policyholder.Name.TypeId = "2" Then
                            isFarmWithCommercialName = True
                        End If
                        RaiseEvent AboutToPopulateForStateChange(isFarmWithCommercialName) 'Added 1/8/2019 for Bug 30790 MLW
                        Populate_FirePopulateEvent() ' 99% of the time this is bad inside of a Save() this is that 1% exception
                    End If

                    'Me.quote.Set_QuoteStateIds(listOfInt) 'may want to use this once we have more than 2 states and load list with checkboxList values (stateIds)
                Else
                    'remove other state parts
                End If


                ''added 8/8/2018; note: need to make sure other screens are re-populated if states change
                'If Me.MultiStateSection.Visible = True Then
                '    Dim previousStateIds As List(Of Integer) = Me.Quote.QuoteStateIds 'added 1/8/2019 for Bug 30790 MLW
                '    Dim qqStates As New List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
                '    qqStates.Add(Me.Quote.QuickQuoteState)

                '    Select Case Quote.LobType
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                '            If divMultiKYWC.Visible Then
                '                ' KY Multistate is in effect
                '                If chkOtherState.Visible AndAlso chkOtherState.Text.ToUpper = "INDIANA" Then
                '                    If chkOtherState.Checked Then
                '                        ' Add Indiana
                '                        qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                '                    End If
                '                End If
                '                If chkOtherState.Visible AndAlso chkOtherState.Text.ToUpper = "ILLINOIS" Then
                '                    If chkOtherState.Checked Then
                '                        ' Add Illinois
                '                        qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                '                    End If
                '                End If
                '                If chkKentucky.Visible Then
                '                    If chkKentucky.Checked Then
                '                        ' Add Kentucky
                '                        qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky)
                '                    End If
                '                End If
                '            Else
                '                ' KY multistate is NOT in effect
                '                If IsOhioEffective Then
                '                    If chkState1.Checked OrElse chkState2.Checked Then

                '                    End If
                '                Else
                '                    If Me.rblHasOtherState.SelectedValue = "Yes" Then
                '                        Select Case Me.Quote.QuickQuoteState
                '                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                '                                qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                '                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                '                                qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                '                        End Select
                '                    End If
                '                End If
                '            End If
                '            Exit Select
                '        Case Else
                '            If Me.rblHasOtherState.SelectedValue = "Yes" Then
                '                Select Case Me.Quote.QuickQuoteState
                '                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                '                        qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                '                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                '                        qqStates.Add(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                '                End Select
                '            End If
                '            Exit Select
                '    End Select

                '    Me.Quote.Set_QuoteStates(qqStates)
                '    Dim newStateIds As List(Of Integer) = Me.Quote.QuoteStateIds 'added 1/8/2019 for Bug 30790 MLW
                '    'updated 1/8/2019 for Bug 30790 MLW
                '    'If Not IsOnAppPage Then
                '    If Not IsOnAppPage AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.IntegerListsMatch(previousStateIds, newStateIds) = False Then
                '        Dim isFarmWithCommercialName As Boolean = False 'Added 1/8/2019 for Bug 30790 MLW
                '        'Added 1/8/2019 for Bug 30790 MLW
                '        If Me.Quote.LobId.TryToGetInt32 = 17 AndAlso Me.Quote.Policyholder.Name.TypeId = "2" Then
                '            isFarmWithCommercialName = True
                '        End If
                '        RaiseEvent AboutToPopulateForStateChange(isFarmWithCommercialName) 'Added 1/8/2019 for Bug 30790 MLW
                '        Populate_FirePopulateEvent() ' 99% of the time this is bad inside of a Save() this is that 1% exception
                '    End If

                '    'Me.quote.Set_QuoteStateIds(listOfInt) 'may want to use this once we have more than 2 states and load list with checkboxList values (stateIds)
                'Else
                '    'remove other state parts
                'End If

                If WebHelper_Personal.ControlVisibilityIsOkayForCommercialDataPrefillFirmographicsPrefill(Me.Visible) = True AndAlso WebHelper_Personal.WorkflowIsOkayForCommercialDataPrefillCalls(Me.Quote, request:=Request) = True Then
                    'note: may need to check for unhandledException info stored in session 1st... so we don't try the same thing over and over if LN/SnapLogic is down
                    Dim ih As New IntegrationHelper
                    Dim attemptedServiceCall As Boolean = False
                    Dim caughtUnhandledException As Boolean = False
                    Dim unhandledExceptionToString As String = ""
                    Dim setAnyMods As Boolean = False
                    ih.CallCommercialDataPrefill_FirmographicsOnly_IfNeeded(Me.Quote, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
                    If caughtUnhandledException = True Then
                        'maybe do something like store flag in session similar to what we do from ctlCommercialDataPrefillEntry
                        Dim prefillError As New IFM.ErrLog_Parameters_Structure()
                        Dim addPrefillInfo As String = WebHelper_Personal.AdditionalInfoTextForCommercialDataPrefillError(Me.Quote, qqHelper:=QQHelper)
                        With prefillError
                            .ApplicationName = "Velocirater Personal"
                            .ClassName = "ctlIsuredList"
                            .ErrorMessage = unhandledExceptionToString
                            .LogDate = DateTime.Now
                            .RoutineName = "Save"
                            .StackTrace = "at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly at IntegrationHelper.CallCommercialDataPrefill_FirmographicsOnly_IfNeeded"
                            .AdditionalInfo = addPrefillInfo
                        End With
                        WriteErrorLogRecord(prefillError, "")
                    ElseIf setAnyMods = True Then
                        'should re-populate pertinent fields
                        'Me.PopulateChildControls() 'should handle fields that we'd update
                        'note: shouldn't need to do anything since ctlIsuredList.btnSubmit_Click calls Save_FireSaveEvent and then Populate_FirePopulateEvent
                        Me.InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave = True
                    End If
                End If
            End If

            'Me.ctlInsured.Populate() - Matt A - You can not do this here
            ' Me.ctlInsured1.Populate()
        End If
        'Updates CompanyType for what it qualifies for
        NewCompanyIdHelper.UpdateCompanyType(Quote, ValidationHelper.ValidationErrors)
        Return True
    End Function

    Public Sub HasExistingAutoPolicyorQuote() '8-1-14
        If String.IsNullOrWhiteSpace(Me.Quote.Client.ClientId) = False Then
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "usp_HOM_MultiPolicyDiscountCheck"
                    cmd.Parameters.AddWithValue("@clientId", Me.Quote.Client.ClientId)
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        ' only change if you know that it does have a policy or quote
                        If reader.HasRows Then
                            Me.Quote.Locations(0).MultiPolicyDiscount = True
                        End If
                    End Using
                End Using
            End Using
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnSaveAndGotoDrivers.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' fires Base save requested event
        Populate_FirePopulateEvent()
        '
        ' you don't want the treeview to be able to invoke a prefill request at the wrong time so only do it if they clicked save button on insured list
        If Me.ValidationSummmary.HasErrors = False Then
            ' you only want to attempt prefill only if you have enough information
            ' it will control if it is actually invoked based on if it has been fetched already
            If Me.Quote IsNot Nothing Then
                Select Case Me.Quote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal ' auto
                        Fire_GenericBoardcastEvent(BroadCastEventType.PreFillRequested)
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal ' home
                        Me.Fire_GenericBoardcastEvent(BroadCastEventType.DoHOMCreditRequest)
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm ' farm
                        'RaiseEvent DoHOMCreditRequest()
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            'RaiseEvent RequestNavigationToProperty()
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability ' CGL
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto  ' CAP
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP ' BOP
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation ' WCP
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty ' CPR
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage ' CPP
                        If sender Is btnSaveAndGotoDrivers Then ' navigate as well
                            Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages, "0")
                        End If
                    Case Else

                End Select
            End If

        End If
    End Sub
    Private Sub btnRatePolicyholder_Click(sender As Object, e As EventArgs) Handles btnRatePolicyholder.Click
        'Save_FireSaveEvent(False) 'Removed 01/28/2021 MLW - Causes duplicate validation messages
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoDrivers_Click(sender As Object, e As EventArgs) Handles btnViewGotoDrivers.Click
        If Me.Quote IsNot Nothing Then
            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal ' auto
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal ' home
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm ' farm
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability ' CGL
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto  ' CAP
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP ' BOP
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation ' WCP
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty ' CPR
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage ' CPP
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages, "0")
                Case Else

            End Select
        End If
    End Sub

    Private Sub ctlInsured_PolicyHolderCleared() Handles ctlInsured.PolicyHolderCleared, ctlInsured1.PolicyHolderCleared
        Me.txtClientId_Lookup.Text = ""
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtClientId_Lookup.Text = ""
        Me.ClearChildControls()
    End Sub

    Private Sub ctlIsuredList_BroadcastGenericEvent(type As BroadCastEventType) Handles Me.BroadcastGenericEvent

    End Sub

    'added 9/18/2019
    Public Sub SetClientIdTextboxFromQuoteIfNeeded()
        If Me.Quote IsNot Nothing AndAlso Me.Quote.Client IsNot Nothing AndAlso Me.Quote.Client.HasValidClientId() = True Then
            If QQHelper.IsPositiveIntegerString(Me.txtClientId_Lookup.Text) = False Then
                Me.txtClientId_Lookup.Text = Me.Quote.Client.ClientId
            End If
        End If
    End Sub

    'added 5/1/2019; 5/6/2019 - moved to QuoteSaveHelpers
    'Private Function QuoteIdsInSessionWithoutClientId() As List(Of Integer)
    '    Dim quoteIds As List(Of Integer) = Nothing

    '    If Session("QuoteIdsWithoutClientId") IsNot Nothing Then
    '        quoteIds = Session("QuoteIdsWithoutClientId")
    '    End If

    '    Return quoteIds
    'End Function
    'Private Sub SetQuoteIdsInSessionWithoutClientId(ByVal quoteIds As List(Of Integer))
    '    If quoteIds IsNot Nothing Then
    '        If Session("QuoteIdsWithoutClientId") IsNot Nothing Then
    '            Session("QuoteIdsWithoutClientId") = quoteIds
    '        Else
    '            Session.Add("QuoteIdsWithoutClientId", quoteIds)
    '        End If
    '    Else
    '        If Session("QuoteIdsWithoutClientId") IsNot Nothing Then
    '            Session("QuoteIdsWithoutClientId") = Nothing
    '            Session.Remove("QuoteIdsWithoutClientId")
    '        End If
    '    End If
    'End Sub
    'Private Sub AddToQuoteIdsInSessionWithoutClientId(ByVal qId As Integer)
    '    If qId > 0 AndAlso IsQuoteIdInSessionWithoutClientId(qId) = False Then
    '        Dim quoteIds As List(Of Integer) = QuoteIdsInSessionWithoutClientId()
    '        QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(qId, quoteIds)
    '        SetQuoteIdsInSessionWithoutClientId(quoteIds)
    '    End If
    'End Sub
    'Private Function IsQuoteIdInSessionWithoutClientId(ByVal qId As Integer) As Boolean
    '    Dim isInList As Boolean = False

    '    Dim quoteIds As List(Of Integer) = QuoteIdsInSessionWithoutClientId()
    '    If quoteIds IsNot Nothing AndAlso quoteIds.Count > 0 AndAlso quoteIds.Contains(qId) = True Then
    '        isInList = True
    '    End If

    '    Return isInList
    'End Function
    ''added 5/2/2019
    'Private Function ClientIdsInSessionFromPopulate() As List(Of Integer)
    '    Dim quoteIds As List(Of Integer) = Nothing

    '    If Session("ClientIdsFromPopulate") IsNot Nothing Then
    '        quoteIds = Session("ClientIdsFromPopulate")
    '    End If

    '    Return quoteIds
    'End Function
    'Private Sub SetClientIdsInSessionFromPopulate(ByVal clientIds As List(Of Integer))
    '    If clientIds IsNot Nothing Then
    '        If Session("ClientIdsFromPopulate") IsNot Nothing Then
    '            Session("ClientIdsFromPopulate") = clientIds
    '        Else
    '            Session.Add("ClientIdsFromPopulate", clientIds)
    '        End If
    '    Else
    '        If Session("ClientIdsFromPopulate") IsNot Nothing Then
    '            Session("ClientIdsFromPopulate") = Nothing
    '            Session.Remove("ClientIdsFromPopulate")
    '        End If
    '    End If
    'End Sub
    'Private Sub AddToClientIdsInSessionFromPopulate(ByVal cId As Integer)
    '    If cId > 0 AndAlso IsClientIdInSessionFromPopulate(cId) = False Then
    '        Dim clientIds As List(Of Integer) = ClientIdsInSessionFromPopulate()
    '        QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(cId, clientIds)
    '        SetClientIdsInSessionFromPopulate(clientIds)
    '    End If
    'End Sub
    'Private Function IsClientIdInSessionFromPopulate(ByVal cId As Integer) As Boolean
    '    Dim isInList As Boolean = False

    '    Dim clientIds As List(Of Integer) = ClientIdsInSessionFromPopulate()
    '    If clientIds IsNot Nothing AndAlso clientIds.Count > 0 AndAlso clientIds.Contains(cId) = True Then
    '        isInList = True
    '    End If

    '    Return isInList
    'End Function

    'Added 02/02/2021 for CAP Endorsements Task 52971 MLW
    Private Function AllowPopulate() As Boolean
        Select Case Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
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

    'Added 02/02/2021 for CAP Endorsements Task 52971 MLW
    Private Function AllowValidateAndSave() As Boolean
        Select Case Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
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

    Private Sub DisableMultistateSelections()
        chkOtherState.Enabled = False
        chkKentucky.Enabled = False
        chkState1.Enabled = False
        chkState2.Enabled = False
        chkState3.Enabled = False
        chkState4.Enabled = False
        Me.rblHasOtherState.Enabled = False
    End Sub

    Private Sub DisableMultistateSelectionsByState_NewCo()
        Dim StatesAllowed As New List(Of String)
        For Each stateId As String In NewCompanyIdHelper.GoverningStatesAllowed
            StatesAllowed.Add([Enum].GetName(GetType(States.StateNames), stateId.TryToGetInt32).Trim.ToLower)
        Next

        Dim ChkStates As New List(Of WebControls.CheckBox) _
                        From {chkState1, chkState2, chkState3, chkState4}
        For Each chkState As WebControls.CheckBox In ChkStates
            If StatesAllowed.Contains(chkState.Text.Trim.ToLower) Then
                chkState.Enabled = True
            Else
                chkState.Checked = False
                chkState.Enabled = False
            End If
        Next

    End Sub

End Class

