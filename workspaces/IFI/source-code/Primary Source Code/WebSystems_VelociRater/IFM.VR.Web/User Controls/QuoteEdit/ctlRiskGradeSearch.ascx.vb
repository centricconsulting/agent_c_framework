Imports IFM.PrimativeExtensions
Imports PopupMessageClass
Imports IFM.VR.Common.Helpers.MultiState.General
Imports IFM.VR.Common.Helpers.CL

Public Class ctlRiskGradeSearch
    Inherits VRControlBase

    Public Event populateCPPCoverageForCondoDandO() 'Added 09/02/2021 for bug 51550 MLW

    Private Property HasStateDropdown As Boolean
        Get
            Return ViewState.GetBool("vs_HasStateDropdown")
        End Get
        Set(value As Boolean)
            ViewState("vs_HasStateDropdown") = value
        End Set
    End Property


    Public Property AllowSave As Boolean
        Get
            Return ViewState.GetBool("vs_allowSave")
        End Get
        Set(value As Boolean)
            ViewState("vs_allowSave") = value
        End Set
    End Property

    Private ReadOnly Property NumberOfStatesOnQuote() As Integer
        Get
            Dim numLocStates As Integer = 0

            Dim lStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = LocationStates
            If lStates IsNot Nothing AndAlso lStates.Count > 0 Then
                numLocStates = lStates.Count
            End If

            Return numLocStates
        End Get
    End Property
    Private ReadOnly Property LocationStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) 'added 12/28/2018
        Get
            Dim lStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = Nothing

            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If l IsNot Nothing AndAlso l.Address IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), l.Address.QuickQuoteState) = True AndAlso l.Address.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        QuickQuote.CommonMethods.QuickQuoteHelperClass.AddQuickQuoteStateToList(l.Address.QuickQuoteState, lStates, onlyAddIfUnique:=True)
                    End If
                Next
            End If

            Return lStates
        End Get
    End Property
    Private Sub LoadStateDropdown()
        If ddState IsNot Nothing AndAlso ddState.Items IsNot Nothing Then
            Dim lStates As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = LocationStates
            If lStates IsNot Nothing AndAlso lStates.Count > 0 Then
                If ddState.Items.Count > 0 Then
                    ddState.Items.Clear()
                End If
                Dim initialItem As New ListItem
                initialItem.Text = ""
                initialItem.Value = "0"
                ddState.Items.Add(initialItem)
                For Each s As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState In lStates
                    Dim newItem As New ListItem
                    newItem.Text = QQHelper.appendText(QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForQuickQuoteState(s), System.Enum.GetName(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), s), splitter:=" ")
                    newItem.Value = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(s).ToString
                    ddState.Items.Add(newItem)
                Next
            End If
        End If
    End Sub

    Public Event SelectedClassCodeChanged(ByVal ClassCode As String, ByVal Desc As String, ByVal DiaClass_Id As String, ByVal StateID As String)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation _
            AndAlso Session("WCP_ClassCodeLookup_" & Session.SessionID) IsNot Nothing Then
            ' Used to determine whether or not we need to validate the state on the class code lookup
            Dim ValidateState As String = "TRUE"
            If (Not IsMultistateCapableEffectiveDate(Quote.EffectiveDate)) OrElse NumberOfStatesOnQuote = 1 Then ValidateState = "FALSE"

            ' Perform class code lookup
            Me.VRScript.CreatePopupForm(Me.MainAccordionDivId, "Class Code Selection ", 750, 550, True, True, False, Me.txtRiskGradeFilterValue.ClientID, "")
            'Me.VRScript.CreateJSBinding(Me.btnRisksearch.ClientID, "click", "VRClassCode.PerformClassCodeLookup(" + Me.Quote.LobId + ",'#" + Me.ddlRiskGradeFilterBy.ClientID + "','#" + Me.txtRiskGradeFilterValue.ClientID + "','#" + Me.divResults.ClientID + "','#" + Me.HiddenClassCode.ClientID + "','#" + Me.HiddenDescription.ClientID + "','#" & Me.HiddenDIAClass_Id.ClientID & "'); return false;")
            Me.VRScript.CreateJSBinding(Me.btnRisksearch.ClientID, "click", "VRClassCode.PerformClassCodeLookup(" + Me.Quote.LobId + ",'#" + Me.ddlRiskGradeFilterBy.ClientID + "','#" + Me.txtRiskGradeFilterValue.ClientID + "','#" + Me.divResults.ClientID + "','#" + Me.HiddenClassCode.ClientID + "','#" + Me.HiddenDescription.ClientID + "','#" & Me.HiddenDIAClass_Id.ClientID & "','" & ddState.ClientID & "','" & ValidateState & "'); return false;")
        Else
            ' Perform Risk Grade lookup
            Me.VRScript.CreatePopupForm(Me.MainAccordionDivId, "Risk Grade Lookup", 750, 550, True, True, False, Me.txtRiskGradeFilterValue.ClientID, "")
            If HotelMotelRemovedRisks.IsHotelMotelRemovedRisksAvailable(Quote) = True 
                Me.VRScript.CreateJSBinding(Me.btnRisksearch.ClientID, "click", "VRRiskGrade.PerformRiskGradeLookupNoMotelHotel(" + Me.Quote.LobId + ",'#" + Me.ddlRiskGradeFilterBy.ClientID + "','#" + Me.txtRiskGradeFilterValue.ClientID + "','" + Me.Quote.VersionId + "','#" + Me.divResults.ClientID + "','#" + Me.hiddenRiskCodeId.ClientID + "','#" + Me.HiddenRiskCodeId2.ClientID + "','#" + Me.hiddenRiskCodeLookupId.ClientID + "'); return false;")
            Else
                Me.VRScript.CreateJSBinding(Me.btnRisksearch.ClientID, "click", "VRRiskGrade.PerformRiskGradeLookup(" + Me.Quote.LobId + ",'#" + Me.ddlRiskGradeFilterBy.ClientID + "','#" + Me.txtRiskGradeFilterValue.ClientID + "','" + Me.Quote.VersionId + "','#" + Me.divResults.ClientID + "','#" + Me.hiddenRiskCodeId.ClientID + "','#" + Me.HiddenRiskCodeId2.ClientID + "','#" + Me.hiddenRiskCodeLookupId.ClientID + "'); return false;")
            End If

        End If
        Me.VRScript.AddVariableLine("function SubmitRiskGradeForm(){$('#" + Me.btnSubmit.ClientID + "').click();}")
        Me.VRScript.AddScriptLine("$('#" + Me.btnSubmit.ClientID + "').hide();")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        HasStateDropdown = False

        ' Only show if the quote has Kill Questions answered and there is no qso.RiskGrade or qso.RiskGradeLookupId on the quote
        Me.Visible = False
        Me.AllowSave = False

        Me.btnCancel.Visible = False

        If Me.Quote IsNot Nothing Then
            ' Change the code lookup search type description based on LOB
            ' THIS CHANGE WAS RESCINDED BY BUG 22390 MGB 9/25/2017
            'Select Case Quote.LobType
            '    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
            '        ddlRiskGradeFilterBy.Items(1).Text = "WC Class Code"  ' Change the class code description for WCP - MGB Bug 22005
            '        Exit Select
            '    Case Else
            '        ddlRiskGradeFilterBy.Items(1).Text = "GL Class Code"
            '        Exit Select
            'End Select

            ddlRiskGradeFilterBy.Items(1).Text = "GL Class Code"

            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso Session("WCP_ClassCodeLookup_" & Session.SessionID) IsNot Nothing Then
                ' Don't show the risk preference guide or link for Class Code Lookup
                divRiskGradeInfo.Attributes.Add("style", "display:none;")
                divRiskPrefGuide.Attributes.Add("style", "display:none;")

                ' Change the lookup description from "GL Class Code" to "WC Class Code" when in Class Code Lookup mode
                ddlRiskGradeFilterBy.Items(1).Text = "WC Class Code"

                ' Show the state dropdown if appropriate
                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso NumberOfStatesOnQuote > 1 Then
                    LoadStateDropdown() 'added 12/28/2018
                    tdStateLabelRow.Attributes.Add("style", "display:''")
                    tdStateddRow.Attributes.Add("style", "display:''")
                    HasStateDropdown = True
                Else
                    ' Quote is not multistate and/or number of states > 1, don't show state dropdown
                    tdStateLabelRow.Attributes.Add("style", "display:none")
                    tdStateddRow.Attributes.Add("style", "display:none")
                    HasStateDropdown = False
                End If

                ' If a state was passed in ViewState("WCP_CCLookup_StateId") then need to set the state dropdown to the passed state then disable the dropdown
                ddState.Enabled = True
                If HasStateDropdown() Then
                    'If Session("WCP_CCLookup_StateId") IsNot Nothing Then
                    '    Select Case Session("WCP_CCLookup_StateId")
                    '        Case "15", "16"
                    '            ddState.SelectedValue = Session("WCP_CCLookup_StateId")
                    '            ddState.Enabled = False
                    '            Exit Select
                    '        Case Else
                    '            Throw New Exception("ctlRiskGradeSearch.Populate: unsupported state")
                    '            Exit Select
                    '    End Select
                    'End If
                    'updated 12/28/2018
                    Dim sessionStateId As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.sessionVariableValueAsString("WCP_CCLookup_StateId")
                    If QQHelper.IsPositiveIntegerString(sessionStateId) = True Then
                        Dim sessionState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(QQHelper.IntegerForString(sessionStateId))
                        If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), sessionState) = True AndAlso sessionState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                            If ddState IsNot Nothing AndAlso ddState.Items IsNot Nothing AndAlso ddState.Items.Count > 0 AndAlso ddState.Items.FindByValue(sessionStateId) IsNot Nothing Then
                                ddState.SelectedValue = sessionStateId
                                ddState.Enabled = False
                            End If
                        Else
                            Throw New Exception("ctlRiskGradeSearch.Populate: unsupported state")
                        End If
                    End If
                End If
            End If

            If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial).ContainsValue(CInt(Me.Quote.LobId)) Then
                'updated 9/5/2018 to use SubQuoteFirst instead of Quote
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Me.hiddenRiskCodeLookupId.Value = Me.SubQuoteFirst.CPP_CPR_RiskGradeLookupId

                    Me.hiddenRiskCodeId.Value = Me.SubQuoteFirst.CPP_CPR_RiskGrade
                    Me.HiddenRiskCodeId2.Value = Me.SubQuoteFirst.CPP_CGL_RiskGrade
                Else
                    Me.hiddenRiskCodeLookupId.Value = Me.SubQuoteFirst.RiskGradeLookupId
                    Me.hiddenRiskCodeId.Value = Me.SubQuoteFirst.RiskGrade
                End If

                'If Me.Quote.PolicyUnderwritings.IsLoaded() Then ' only show after Kill questions have been answered
                'updated 9/22/2018 to use SubQuoteFirst
                If Me.SubQuoteFirst.PolicyUnderwritings.IsLoaded() Then ' only show after Kill questions have been answered
                    If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                        'CPP
                        'If Me.Quote.CPP_CPR_RiskGrade.IsNullEmptyorWhitespace() OrElse Me.Quote.CPP_CGL_RiskGrade.IsNullEmptyorWhitespace() OrElse Me.Quote.CPP_CPR_RiskGradeLookupId.IsNullEmptyorWhitespace() OrElse Me.Quote.CPP_CGL_RiskGradeLookupId.IsNullEmptyorWhitespace() Then
                        ' Don't check the RiskGrade field, only the RiskGrade lookup
                        'updated 9/5/2018 to use SubQuoteFirst instead of Quote
                        'If Me.SubQuoteFirst.CPP_CPR_RiskGradeLookupId.IsNullEmptyorWhitespace() OrElse Me.SubQuoteFirst.CPP_CGL_RiskGradeLookupId.IsNullEmptyorWhitespace() Then
                        'updated 9/22/2018 to also handle for 0
                        If Me.SubQuoteFirst.CPP_CPR_RiskGradeLookupId.IsNullEmptyorWhitespace() OrElse Me.SubQuoteFirst.CPP_CGL_RiskGradeLookupId.IsNullEmptyorWhitespace() OrElse QQHelper.IntegerForString(Me.SubQuoteFirst.CPP_CPR_RiskGradeLookupId) = 0 OrElse QQHelper.IntegerForString(Me.SubQuoteFirst.CPP_CGL_RiskGradeLookupId) = 0 Then                        'show
                            Me.Visible = True
                            Me.AllowSave = True
                            Me.PopulateChildControls()
                        End If
                    Else
                        'ALL NON CPP
                        If MainAccordionDivId = "" Then MainAccordionDivId = divMain.ClientID
                        'updated 9/5/2018 to use SubQuoteFirst instead of Quote
                        'If Me.SubQuoteFirst.RiskGradeLookupId.IsNullEmptyorWhitespace() Then
                        'updated 9/22/2018 to also handle for 0
                        If Me.SubQuoteFirst.RiskGradeLookupId.IsNullEmptyorWhitespace() OrElse QQHelper.IntegerForString(Me.SubQuoteFirst.RiskGradeLookupId) = 0 Then
                            'show
                            Me.Visible = True
                            Me.AllowSave = True
                            Me.PopulateChildControls()
                        End If
                        ' WCP - If this session variable is set, show the control
                        If Session("WCP_ClassCodeLookup_" & Session.SessionID) IsNot Nothing Then
                            'show
                            Me.btnCancel.Visible = True
                            Me.Visible = True
                            Me.AllowSave = True
                            Me.PopulateChildControls()
                        End If
                    End If
                End If
            End If
        End If
        Me.RiskGradeSearchIsVisible = Me.Visible
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMain.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If AllowSave Then
            If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial).ContainsValue(CInt(Me.Quote.LobId)) Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso Session("WCP_ClassCodeLookup_" & Session.SessionID) IsNot Nothing Then
                        ' WCP CLASS CODE LOOKUP - Called from the WCP Coverages page, class code control
                        ' We know we need to execute the class code lookup when the 'Session("WCP_ClassCodeLookup_" & Session.SessionID)' value is set
                        Session("WCP_ClassCodeLookup_" & Session.SessionID) = Nothing
                        'RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Me.hdnStateId.Value)
                        If HasStateDropdown Then
                            RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Me.ddState.SelectedValue)
                        Else
                            RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Quote.Locations(0).Address.StateId)
                        End If
                        AllowSave = False
                        Me.Visible = False
                    Else
                        'If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                        '    Me.Quote.CPP_CPR_RiskGradeLookupId = Me.hiddenRiskCodeLookupId.Value
                        '    Me.Quote.CPP_CGL_RiskGradeLookupId = Me.hiddenRiskCodeLookupId.Value

                        '    Me.Quote.CPP_CPR_RiskGrade = Me.hiddenRiskCodeId.Value
                        '    Me.Quote.CPP_CGL_RiskGrade = Me.HiddenRiskCodeId2.Value
                        '    AllowSave = False
                        '    Me.Visible = False
                        'Else
                        '    Me.Quote.RiskGradeLookupId = Me.hiddenRiskCodeLookupId.Value
                        '    Me.Quote.RiskGrade = Me.hiddenRiskCodeId.Value
                        '    AllowSave = False
                        '    Me.Visible = False
                        'End If
                        'updated 9/5/2018 for multi-state
                        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                            For Each stateQuote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                                    stateQuote.CPP_CPR_RiskGradeLookupId = Me.hiddenRiskCodeLookupId.Value
                                    stateQuote.CPP_CGL_RiskGradeLookupId = Me.hiddenRiskCodeLookupId.Value

                                    stateQuote.CPP_CPR_RiskGrade = Me.hiddenRiskCodeId.Value
                                    stateQuote.CPP_CGL_RiskGrade = Me.HiddenRiskCodeId2.Value
                                    AllowSave = False
                                    Me.Visible = False
                                Else
                                    stateQuote.RiskGradeLookupId = Me.hiddenRiskCodeLookupId.Value
                                    stateQuote.RiskGrade = Me.hiddenRiskCodeId.Value
                                    AllowSave = False
                                    Me.Visible = False
                                End If
                            Next
                        End If
                    End If
                End If
            End If

            Me.SaveChildControls()

        End If
        Me.RiskGradeSearchIsVisible = Me.Visible
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        MyBase.ValidateControl(valArgs)

        ' Validate State
        If HasStateDropdown Then
            If ddState.SelectedValue = "" Then
                Me.ValidationHelper.AddError(ddState, "Missing State", accordList)
            End If
        End If

        ' Notes:
        ' If CPP will use different properties on quote
        ' If the grade is "P" (on cpp check both grades) then you can't even quote it message 'Not authorized to quote/bind this risk'
        ' If grade is "3" (on cpp check both grades) then you need to popup a warning 'The user does not have the authority to quote, bind or issue coverage for this risk.  Please refer to your Commercial Underwriter.'

        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Quote IsNot Nothing Then
            'If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso Session("WCP_ClassCodeLookup_" & Session.SessionID) IsNot Nothing Then
            '    ' Class code lookup
            '    If HasStateDropdown Then
            '        RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Me.ddState.SelectedValue)
            '    Else
            '        RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Quote.Locations(0).Address.StateId)
            '    End If
            '    Me.Save_FireSaveEvent(False)
            'Else
            '    ' Risk grade lookup
            '    Me.Save_FireSaveEvent()
            'End If

            If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                Me.Save_FireSaveEvent(False)  ' Don't fire validation when WCP because we just did a class code lookup and payroll is not filled in yet
            ElseIf Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                'Added 09/02/2021 for bug 51550 MLW
                Dim condoDandONeedsReset As Boolean = False
                If Me.SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.HasCondoDandO = True Then
                    condoDandONeedsReset = True
                End If

                Me.Save_FireSaveEvent()

                If condoDandONeedsReset Then
                    Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = QQHelper.MultiStateQuickQuoteObjects(Quote) 'should always return at least qq in the list
                    If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                            sq.HasCondoDandO = True
                        Next
                    End If

                    RaiseEvent populateCPPCoverageForCondoDandO()
                End If
            Else
                Me.Save_FireSaveEvent()
            End If
        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        'RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value)
        Me.HiddenClassCode.Value = "9999999"
        Me.HiddenDescription.Value = "NO INFORMATION"
        Me.HiddenDIAClass_Id.Value = "9999999"
        If HasStateDropdown Then
            RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Me.ddState.SelectedValue)
        Else
            RaiseEvent SelectedClassCodeChanged(Me.HiddenClassCode.Value, Me.HiddenDescription.Value, Me.HiddenDIAClass_Id.Value, Quote.Locations(0).Address.StateId)
        End If
        Me.Save_FireSaveEvent(False)
    End Sub
End Class