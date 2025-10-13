'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_CPP_Summary
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass 'added 5/29/2013 to check for zero premiums
    Dim proposalHelper As New ProposalHelperClass 'added 7/1/2015

    Private _QuickQuote As QuickQuoteObject
    Private _SubQuotes As List(Of QuickQuoteObject) 'added 10/11/2018
    Public Property QuickQuote As QuickQuoteObject
        Get
            Return _QuickQuote
        End Get
        Set(value As QuickQuoteObject)
            _QuickQuote = value
            SetSummaryLabels()
        End Set
    End Property
    Public Property SubQuotes As List(Of QuickQuoteObject) 'added 10/11/2018
        Get
            If (_SubQuotes Is Nothing OrElse _SubQuotes.Count = 0) AndAlso _QuickQuote IsNot Nothing Then
                _SubQuotes = qqHelper.MultiStateQuickQuoteObjects(_QuickQuote)
            End If
            Return _SubQuotes
        End Get
        Set(value As List(Of QuickQuoteObject))
            _SubQuotes = value
        End Set
    End Property
    Public ReadOnly Property SubQuoteFirst As QuickQuoteObject 'added 10/11/2018
        Get
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                Return SubQuotes.Item(0)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property GoverningStateQuote As QuickQuoteObject 'added 10/11/2018
        Get
            Return qqHelper.GoverningStateQuote(_QuickQuote, subQuotes:=Me.SubQuotes)
        End Get
    End Property

    'added 8/19/2015
    Public ReadOnly Property CPR_SummaryControl As controls_Proposal_VRProposal_CPR_Summary
        Get
            Return Me.CPR_Summary
        End Get
    End Property
    Public ReadOnly Property CGL_SummaryControl As controls_Proposal_VRProposal_CGL_Summary
        Get
            Return Me.CGL_Summary
        End Get
    End Property
    Public ReadOnly Property CIM_SummaryControl As controls_Proposal_VRProposal_CIM_Summary
        Get
            Return Me.CIM_Summary
        End Get
    End Property
    Public ReadOnly Property CRM_SummaryControl As controls_Proposal_VRProposal_CRM_Summary
        Get
            Return Me.CRM_Summary
        End Get
    End Property
    Public ReadOnly Property GAR_SummaryControl As controls_Proposal_VRProposal_GAR_Summary 'added 4/22/2017
        Get
            Return Me.GAR_Summary
        End Get
    End Property

    'added 6/28/2013
    'Private _LinesInControl As Integer = 24 'breaks and rows; will need to adjust if any breaks are added
    ' Changed 6/18/15 for CIM/CRM
    'Private _LinesInControl As Integer = 46 'breaks and rows; will need to adjust if any breaks are added
    ' Added 2 lines for Property and CIM Amount to meet minimum
    'Private _LinesInControl As Integer = 48 'breaks and rows; will need to adjust if any breaks are added

    ' Here's the breakdown on lines in control:
    ' Title: 1 line
    ' CPR: Max 12 Lines
    ' CGL: Max 9 Lines
    ' CRM: Max 7 Lines
    ' CIM: Max 16 Lines
    ' Total Line: 1 line
    ' TOTAL LINES ON PAGE:
    'Private _LinesInControl As Integer = 48 'breaks and rows; will need to adjust if any breaks are added
    'will now set initial value on load... based on SUM of lines in child controls
    Private _LinesInControl As Integer = 0
    Public ReadOnly Property LinesInControl As Integer
        Get
            RecalculateLinesInControl()
            Return _LinesInControl
        End Get
    End Property

    'added 8/20/2015
    Private _HeaderLines As Integer = 2 '1 for break and 1 for header row
    Public ReadOnly Property HeaderLines As Integer
        Get
            Return _HeaderLines
        End Get
    End Property
    Private _FooterLines As Integer = 2 '1 for total row and 1 for break
    Public ReadOnly Property FooterLines As Integer
        Get
            Return _FooterLines
        End Get
    End Property
    Public ReadOnly Property CPR_Lines As Integer
        Get
            'Return CPR_SummaryControl.LinesInControl
            'updated 5/12/2017
            If Me.trCommercialProperty.Visible = True Then
                Return CPR_SummaryControl.LinesInControl
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property CGL_Lines As Integer
        Get
            'Return CGL_SummaryControl.LinesInControl
            'updated 5/12/2017
            If Me.trGeneralLiability.Visible = True Then
                Return CGL_SummaryControl.LinesInControl
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property CIM_Lines As Integer
        Get
            If Me.trInlandMarine.Visible = True Then
                Return CIM_SummaryControl.LinesInControl
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property CRM_Lines As Integer
        Get
            If Me.trCommercialCrime.Visible = True Then
                Return CRM_SummaryControl.LinesInControl
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property GAR_Lines As Integer 'added 4/22/2017
        Get
            If Me.trCommercialGarage.Visible = True Then
                Return GAR_SummaryControl.LinesInControl
            Else
                Return 0
            End If
        End Get
    End Property
    'Public ReadOnly Property Lines_CGL_to_end As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.CGL_to_end)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_CIM_to_end As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.CIM_to_end)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_CRM_to_end As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.CRM_to_end)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_start_thru_CPR As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.start_thru_CPR)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_start_thru_CGL As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.start_thru_CGL)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_start_thru_CIM As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.start_thru_CIM)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_CGL_thru_CIM As Integer
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.CGL_thru_CIM)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_start_thru_CRM As Integer 'added 4/22/2017
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.start_thru_CRM)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_CGL_thru_CRM As Integer 'added 4/22/2017
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.CGL_thru_CRM)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_CIM_thru_CRM As Integer 'added 4/22/2017
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.CIM_thru_CRM)
    '    End Get
    'End Property
    'Public ReadOnly Property Lines_GAR_to_end As Integer 'added 4/22/2017
    '    Get
    '        Return GetLines(CPP_SectionToEvaluate.GAR_to_end)
    '    End Get
    'End Property

    'updated 5/12/2017 to move GAR up between CGL and CIM
    Public ReadOnly Property Lines_CGL_to_end As Integer 'after CPR; includes CGL, GAR, CIM, CRM, and footer
        Get
            Return GetLines(CPP_SectionToEvaluate.CGL_to_end)
        End Get
    End Property
    Public ReadOnly Property Lines_GAR_to_end As Integer 'after CPR and CGL; includes GAR, CIM, CRM, and footer
        Get
            Return GetLines(CPP_SectionToEvaluate.GAR_to_end)
        End Get
    End Property
    Public ReadOnly Property Lines_CIM_to_end As Integer 'after CPR, CGL, and GAR; includes CIM, CRM, and footer
        Get
            Return GetLines(CPP_SectionToEvaluate.CIM_to_end)
        End Get
    End Property
    Public ReadOnly Property Lines_CRM_to_end As Integer 'after CPR, CGL, GAR, and CIM; includes CRM and footer
        Get
            Return GetLines(CPP_SectionToEvaluate.CRM_to_end)
        End Get
    End Property
    Public ReadOnly Property Lines_start_thru_CPR As Integer 'header and CPR
        Get
            Return GetLines(CPP_SectionToEvaluate.start_thru_CPR)
        End Get
    End Property
    Public ReadOnly Property Lines_start_thru_CGL As Integer 'header, CPR, and CGL
        Get
            Return GetLines(CPP_SectionToEvaluate.start_thru_CGL)
        End Get
    End Property
    Public ReadOnly Property Lines_start_thru_GAR As Integer 'header, CPR, CGL, and GAR
        Get
            Return GetLines(CPP_SectionToEvaluate.start_thru_GAR)
        End Get
    End Property
    Public ReadOnly Property Lines_start_thru_CIM As Integer 'header, CPR, CGL, GAR, and CIM
        Get
            Return GetLines(CPP_SectionToEvaluate.start_thru_CIM)
        End Get
    End Property
    Public ReadOnly Property Lines_CGL_thru_GAR As Integer 'CGL and GAR
        Get
            Return GetLines(CPP_SectionToEvaluate.CGL_thru_GAR)
        End Get
    End Property
    Public ReadOnly Property Lines_CGL_thru_CIM As Integer 'CGL, GAR, and CIM
        Get
            Return GetLines(CPP_SectionToEvaluate.CGL_thru_CIM)
        End Get
    End Property
    Public ReadOnly Property Lines_GAR_thru_CIM As Integer 'GAR and CIM
        Get
            Return GetLines(CPP_SectionToEvaluate.GAR_thru_CIM)
        End Get
    End Property

    'added 5/13/2017 so other controls can see which controls are visible (i.e. to see which one shows up first, etc.)
    Public ReadOnly Property CPR_IsVisible As Boolean
        Get
            Return Me.trCommercialProperty.Visible
        End Get
    End Property
    Public ReadOnly Property CGL_IsVisible As Boolean
        Get
            Return Me.trGeneralLiability.Visible
        End Get
    End Property
    Public ReadOnly Property GAR_IsVisible As Boolean
        Get
            Return Me.trCommercialGarage.Visible
        End Get
    End Property
    Public ReadOnly Property CIM_IsVisible As Boolean
        Get
            Return Me.trInlandMarine.Visible
        End Get
    End Property
    Public ReadOnly Property CRM_IsVisible As Boolean
        Get
            Return Me.trCommercialCrime.Visible
        End Get
    End Property

    'added 7/1/2015 to handle page breaking inside of CPP control; removed 8/19/2015
    'Private _PageNum As Integer = 0
    'Public Property PageNum As Integer
    '    Get
    '        Return _PageNum
    '    End Get
    '    Set(value As Integer)
    '        _PageNum = value
    '    End Set
    'End Property
    'Private _IsLastControl As Boolean = False
    'Public Property IsLastControl As Boolean
    '    Get
    '        Return _IsLastControl
    '    End Get
    '    Set(value As Boolean)
    '        _IsLastControl = value
    '    End Set
    'End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            RecalculateLinesInControl() 'probably not needed here since it will currently be called whenever somebody requests the LinesInControl
        End If
    End Sub
    Private Sub SetSummaryLabels()
        If _QuickQuote IsNot Nothing Then
            With _QuickQuote
                'Me.lblQuoteNumber.Text = .QuoteNumber
                'updated 4/5/2017 for Diamond Proposals
                Me.lblQuoteNumber.Text = .PolicyNumber
                Me.lblTotalPremium.Text = .TotalQuotedPremium

                'CPR_SummaryControl.QuickQuote = _QuickQuote
                'CGL_SummaryControl.QuickQuote = _QuickQuote
                'updated 5/12/2017 to use new hasPackagePart flags for CPR and CGL; CPR will likely always be there; CGL is usually there too, but it shouldn't be there when GAR is included
                'If .CPP_Has_Property_PackagePart = True Then
                'updated 12/12/2018 for multi-state
                If qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .CPP_Has_Property_PackagePart) = True Then
                    Me.trCommercialProperty.Visible = True
                    CPR_SummaryControl.QuickQuote = _QuickQuote
                    CPR_SummaryControl.SubQuotes = SubQuotes 'added 10/11/2018
                Else
                    Me.trCommercialProperty.Visible = False
                    _LinesInControl -= CPR_SummaryControl.LinesInControl 'not needed if calling RecalculateLinesInControl every time
                End If
                'If .CPP_Has_GeneralLiability_PackagePart = True Then
                'updated 12/12/2018 for multi-state
                If qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .CPP_Has_GeneralLiability_PackagePart) = True Then
                    Me.trGeneralLiability.Visible = True
                    CGL_SummaryControl.QuickQuote = _QuickQuote
                    CGL_SummaryControl.SubQuotes = SubQuotes 'added 10/11/2018
                Else
                    Me.trGeneralLiability.Visible = False
                    _LinesInControl -= CGL_SummaryControl.LinesInControl 'not needed if calling RecalculateLinesInControl every time
                End If
                'If .CPP_Has_InlandMarine_PackagePart = True Then
                'updated 12/12/2018 for multi-state
                If qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .CPP_Has_InlandMarine_PackagePart) = True Then
                    Me.trInlandMarine.Visible = True
                    CIM_SummaryControl.QuickQuote = _QuickQuote
                    CIM_SummaryControl.SubQuotes = SubQuotes 'added 10/11/2018
                Else
                    Me.trInlandMarine.Visible = False
                    _LinesInControl -= CIM_SummaryControl.LinesInControl 'not needed if calling RecalculateLinesInControl every time
                End If
                'If .CPP_Has_Crime_PackagePart = True Then
                'updated 12/12/2018 for multi-state
                If qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .CPP_Has_Crime_PackagePart) = True Then
                    Me.trCommercialCrime.Visible = True
                    CRM_SummaryControl.QuickQuote = _QuickQuote
                    CRM_SummaryControl.SubQuotes = SubQuotes 'added 10/11/2018
                Else
                    Me.trCommercialCrime.Visible = False
                    _LinesInControl -= CRM_SummaryControl.LinesInControl 'not needed if calling RecalculateLinesInControl every time
                End If
                'If .CPP_Has_Garage_PackagePart = True Then 'added 4/22/2017; 5/12/2017 note: GAR will now be going between CGL and CIM instead of at the end
                'updated 12/12/2018 for multi-state
                If qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .CPP_Has_Garage_PackagePart) = True Then
                    Me.trCommercialGarage.Visible = True
                    GAR_SummaryControl.QuickQuote = _QuickQuote
                    GAR_SummaryControl.SubQuotes = SubQuotes 'added 10/11/2018
                Else
                    Me.trCommercialGarage.Visible = False
                    _LinesInControl -= GAR_SummaryControl.LinesInControl 'not needed if calling RecalculateLinesInControl every time
                End If
            End With
        End If
    End Sub
    Private Sub RecalculateLinesInControl() 'added 8/19/2015
        '_LinesInControl = 0 'in case the default above is something else
        ''_LinesInControl += 4 '2 rows (header and total) and 2 breaks (top and bottom)
        ''_LinesInControl += CPR_SummaryControl.LinesInControl
        ''_LinesInControl += CGL_SummaryControl.LinesInControl
        ''If Me.trInlandMarine.Visible = True Then
        ''    _LinesInControl += CIM_SummaryControl.LinesInControl
        ''End If
        ''If Me.trCommercialCrime.Visible = True Then
        ''    _LinesInControl += CRM_SummaryControl.LinesInControl
        ''End If
        ''updated 8/20/2015 to use new properties
        '_LinesInControl += HeaderLines
        '_LinesInControl += CPR_Lines
        '_LinesInControl += CGL_Lines
        '_LinesInControl += CIM_Lines
        '_LinesInControl += CRM_Lines
        '_LinesInControl += FooterLines
        'updated 8/20/2015 to use new function
        _LinesInControl = GetLines() 'could specify All for optional param, but it's the default so it shouldn't matter
    End Sub
    'note: this Enum and GetLines method assume that the order of the CPP packagePart controls is CPR, CGL, CIM, and then CRM; would need to update logic if order changes
    Private Enum CPP_SectionToEvaluate
        'All = 0
        'CGL_to_end = 1
        'CIM_to_end = 2
        'CRM_to_end = 3
        'start_thru_CPR = 4
        'start_thru_CGL = 5
        'start_thru_CIM = 6
        'CGL_thru_CIM = 7
        ''added 4/22/2017
        'start_thru_CRM = 8 'would've been All before
        'CGL_thru_CRM = 9 'would've been CGL_to_end before
        'CIM_thru_CRM = 10 'would've been CIM_to_end
        'GAR_to_end = 11

        'updated 5/12/2017 to move GAR up between CGL and CIM
        All = 0 'header, CPR, CGL, GAR, CIM, CRM, and footer
        CGL_to_end = 1 'after CPR; includes CGL, GAR, CIM, CRM, and footer
        GAR_to_end = 2 'after CPR and CGL; includes GAR, CIM, CRM, and footer
        CIM_to_end = 3 'after CPR, CGL, and GAR; includes CIM, CRM, and footer
        CRM_to_end = 4 'after CPR, CGL, GAR, and CIM; includes CRM and footer
        start_thru_CPR = 5 'header and CPR
        start_thru_CGL = 6 'header, CPR, and CGL
        start_thru_GAR = 7 'header, CPR, CGL, and GAR
        start_thru_CIM = 8 'header, CPR, CGL, GAR, and CIM
        CGL_thru_GAR = 9 'CGL and GAR
        CGL_thru_CIM = 10 'CGL, GAR, and CIM
        GAR_thru_CIM = 11 'GAR and CIM
    End Enum
    Private Function GetLines(Optional ByVal section As CPP_SectionToEvaluate = CPP_SectionToEvaluate.All) As Integer
        Dim lines As Integer = 0

        'Select Case section
        '    Case CPP_SectionToEvaluate.All 'max 57 as-of 8/20/2015 (2 + 14 + 11 + 19 + 9 + 2)
        '        lines += HeaderLines '2 as-of 8/20/2015 (1 for break and 1 for header row)
        '        lines += CPR_Lines 'max 14 as-of 8/20/2015 (12 rows and 2 breaks)
        '        lines += CGL_Lines 'max 11 as-of 8/20/2015 (9 rows and 2 breaks)
        '        lines += CIM_Lines 'max 19 as-of 8/20/2015 (17 rows and 2 breaks)
        '        lines += CRM_Lines 'max 9 as-of 8/20/2015 (7 rows and 2 breaks)
        '        lines += GAR_Lines 'added 4/22/2017
        '        lines += FooterLines '2 as-of 8/20/2015 (1 for total row and 1 for break)
        '    Case CPP_SectionToEvaluate.CGL_to_end
        '        lines += CGL_Lines
        '        lines += CIM_Lines
        '        lines += CRM_Lines
        '        lines += GAR_Lines 'added 4/22/2017
        '        lines += FooterLines
        '    Case CPP_SectionToEvaluate.CIM_to_end
        '        lines += CIM_Lines
        '        lines += CRM_Lines
        '        lines += GAR_Lines 'added 4/22/2017
        '        lines += FooterLines
        '    Case CPP_SectionToEvaluate.CRM_to_end
        '        lines += CRM_Lines
        '        lines += GAR_Lines 'added 4/22/2017
        '        lines += FooterLines
        '    Case CPP_SectionToEvaluate.start_thru_CPR
        '        lines += HeaderLines
        '        lines += CPR_Lines
        '    Case CPP_SectionToEvaluate.start_thru_CGL
        '        lines += HeaderLines
        '        lines += CPR_Lines
        '        lines += CGL_Lines
        '    Case CPP_SectionToEvaluate.start_thru_CIM
        '        lines += HeaderLines
        '        lines += CPR_Lines
        '        lines += CGL_Lines
        '        lines += CIM_Lines
        '    Case CPP_SectionToEvaluate.CGL_thru_CIM
        '        lines += CGL_Lines
        '        lines += CIM_Lines
        '    Case CPP_SectionToEvaluate.start_thru_CRM 'added 4/22/2017
        '        lines += HeaderLines
        '        lines += CPR_Lines
        '        lines += CGL_Lines
        '        lines += CIM_Lines
        '        lines += CRM_Lines
        '    Case CPP_SectionToEvaluate.CGL_thru_CRM 'added 4/22/2017
        '        lines += CGL_Lines
        '        lines += CIM_Lines
        '        lines += CRM_Lines
        '    Case CPP_SectionToEvaluate.CIM_thru_CRM 'added 4/22/2017
        '        lines += CIM_Lines
        '        lines += CRM_Lines
        '    Case CPP_SectionToEvaluate.GAR_to_end 'added 4/22/2017
        '        lines += GAR_Lines 'added 4/22/2017
        '        lines += FooterLines
        'End Select

        'updated 5/12/2017 to move GAR up between CGL and CIM
        Select Case section
            Case CPP_SectionToEvaluate.All 'header, CPR, CGL, GAR, CIM, CRM, and footer; max 71 as-of 5/12/2017 (2 + 14 + 11 + 14 + 19 + 9 + 2)
                lines += HeaderLines '2 as-of 8/20/2015 (1 for break and 1 for header row)
                lines += CPR_Lines 'max 14 as-of 8/20/2015 (12 rows and 2 breaks)
                lines += CGL_Lines 'max 11 as-of 8/20/2015 (9 rows and 2 breaks)
                lines += GAR_Lines 'max 14 as of 5/12/2017 (12 rows with 1 being spacer and 2 breaks)
                lines += CIM_Lines 'max 19 as-of 8/20/2015 (17 rows and 2 breaks)
                lines += CRM_Lines 'max 9 as-of 8/20/2015 (7 rows and 2 breaks)
                lines += FooterLines '2 as-of 8/20/2015 (1 for total row and 1 for break)
            Case CPP_SectionToEvaluate.CGL_to_end 'after CPR; includes CGL, GAR, CIM, CRM, and footer
                lines += CGL_Lines
                lines += GAR_Lines
                lines += CIM_Lines
                lines += CRM_Lines
                lines += FooterLines
            Case CPP_SectionToEvaluate.GAR_to_end 'after CPR and CGL; includes GAR, CIM, CRM, and footer
                lines += GAR_Lines
                lines += CIM_Lines
                lines += CRM_Lines
                lines += FooterLines
            Case CPP_SectionToEvaluate.CIM_to_end 'after CPR, CGL, and GAR; includes CIM, CRM, and footer
                lines += CIM_Lines
                lines += CRM_Lines
                lines += FooterLines
            Case CPP_SectionToEvaluate.CRM_to_end 'after CPR, CGL, GAR, and CIM; includes CRM and footer
                lines += CRM_Lines
                lines += FooterLines
            Case CPP_SectionToEvaluate.start_thru_CPR 'header and CPR
                lines += HeaderLines
                lines += CPR_Lines
            Case CPP_SectionToEvaluate.start_thru_CGL 'header, CPR, and CGL
                lines += HeaderLines
                lines += CPR_Lines
                lines += CGL_Lines
            Case CPP_SectionToEvaluate.start_thru_GAR 'header, CPR, CGL, and GAR
                lines += HeaderLines
                lines += CPR_Lines
                lines += CGL_Lines
                lines += GAR_Lines
            Case CPP_SectionToEvaluate.start_thru_CIM 'header, CPR, CGL, GAR, and CIM
                lines += HeaderLines
                lines += CPR_Lines
                lines += CGL_Lines
                lines += GAR_Lines
                lines += CIM_Lines
            Case CPP_SectionToEvaluate.CGL_thru_GAR 'CGL and GAR
                lines += CGL_Lines
                lines += GAR_Lines
            Case CPP_SectionToEvaluate.CGL_thru_CIM 'CGL, GAR, and CIM
                lines += CGL_Lines
                lines += GAR_Lines
                lines += CIM_Lines
            Case CPP_SectionToEvaluate.GAR_thru_CIM 'GAR and CIM
                lines += GAR_Lines
                lines += CIM_Lines
        End Select

        Return lines
    End Function
    Public Sub EnableOrDisablePageBreak_between_CPR_and_CGL(ByVal enabled As Boolean)
        Me.trPageBreak_between_CPR_and_CGL.Visible = enabled
        If enabled = True AndAlso Me.phPageBreak_between_CPR_and_CGL.HasControls = False Then
            proposalHelper.AddPageBreakToPlaceholder(Me.phPageBreak_between_CPR_and_CGL)
        End If
    End Sub
    Public Sub EnableOrDisablePageBreak_between_CGL_and_GAR(ByVal enabled As Boolean) 'added 5/12/2017
        Me.trPageBreak_between_CGL_and_GAR.Visible = enabled
        If enabled = True AndAlso Me.phPageBreak_between_CGL_and_GAR.HasControls = False Then
            proposalHelper.AddPageBreakToPlaceholder(Me.phPageBreak_between_CGL_and_GAR)
        End If
    End Sub
    'Public Sub EnableOrDisablePageBreak_between_CGL_and_CIM(ByVal enabled As Boolean) 'removed 5/12/2017
    '    Me.trPageBreak_between_CGL_and_CIM.Visible = enabled
    '    If enabled = True AndAlso Me.phPageBreak_between_CGL_and_CIM.HasControls = False Then
    '        proposalHelper.AddPageBreakToPlaceholder(Me.phPageBreak_between_CGL_and_CIM)
    '    End If
    'End Sub
    Public Sub EnableOrDisablePageBreak_between_GAR_and_CIM(ByVal enabled As Boolean) 'added 5/12/2017
        Me.trPageBreak_between_GAR_and_CIM.Visible = enabled
        If enabled = True AndAlso Me.phPageBreak_between_GAR_and_CIM.HasControls = False Then
            proposalHelper.AddPageBreakToPlaceholder(Me.phPageBreak_between_GAR_and_CIM)
        End If
    End Sub
    Public Sub EnableOrDisablePageBreak_between_CIM_and_CRM(ByVal enabled As Boolean)
        Me.trPageBreak_between_CIM_and_CRM.Visible = enabled
        If enabled = True AndAlso Me.phPageBreak_between_CIM_and_CRM.HasControls = False Then
            proposalHelper.AddPageBreakToPlaceholder(Me.phPageBreak_between_CIM_and_CRM)
        End If
    End Sub
    'Public Sub EnableOrDisablePageBreak_between_CRM_and_GAR(ByVal enabled As Boolean) 'added 4/22/2017; removed 5/12/2017
    '    Me.trPageBreak_between_CRM_and_GAR.Visible = enabled
    '    If enabled = True AndAlso Me.phPageBreak_between_CRM_and_GAR.HasControls = False Then
    '        proposalHelper.AddPageBreakToPlaceholder(Me.phPageBreak_between_CRM_and_GAR)
    '    End If
    'End Sub
    'Private Sub SetSummaryLabelsOld() 'removed 8/19/2015
    '    Dim CPRAmt As Decimal = 0
    '    Dim CPRAmtToMeetMin As Decimal = 0
    '    Dim totprem As Decimal = 0
    '    Dim amtto100 As Decimal = 0

    '    If _QuickQuote IsNot Nothing Then
    '        With _QuickQuote
    '            Me.lblQuoteNumber.Text = .QuoteNumber
    '            Me.lblTotalPremium.Text = .TotalQuotedPremium
    '            Me.lblCprPremium.Text = .CPP_CPR_PackagePart_QuotedPremium
    '            Me.lblCglPremium.Text = .CPP_GL_PackagePart_QuotedPremium

    '            'added 5/13/2013 (CPR)
    '            Me.lblBuildingPremium.Text = .CPR_BuildingsTotal_BuildingCovQuotedPremium
    '            Me.lblPersPropPremium.Text = .CPR_BuildingsTotal_PersPropCoverageQuotedPremium
    '            Me.lblPersPropOfOthersPremium.Text = .CPR_BuildingsTotal_PersPropOfOthersQuotedPremium
    '            Me.lblBusIncPremium.Text = .CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium
    '            Me.lblPropInTheOpenPremium.Text = .LocationsTotal_PropertyInTheOpenRecords_QuotedPremium 'added 5/6/2013
    '            Me.lblEnhEndPremium_Cpr.Text = .PackageCPR_EnhancementEndorsementQuotedPremium '5/29/2013 - updated label name to make it easier to distinguish
    '            Me.lblEbPremium.Text = .LocationsTotal_EquipmentBreakdownQuotedPremium
    '            Me.lblEqPremium.Text = .LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium

    '            If .CPP_CPR_PackagePart_QuotedPremium IsNot Nothing AndAlso .CPP_CPR_PackagePart_QuotedPremium <> String.Empty AndAlso IsNumeric(.CPP_CPR_PackagePart_QuotedPremium) Then
    '                totprem = CDec(.CPP_CPR_PackagePart_QuotedPremium)
    '                Dim partsprem As Decimal = 0
    '                If IsNumeric(.CPR_BuildingsTotal_BuildingCovQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_BuildingCovQuotedPremium)
    '                If IsNumeric(.CPR_BuildingsTotal_PersPropCoverageQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_PersPropCoverageQuotedPremium)
    '                If IsNumeric(.CPR_BuildingsTotal_PersPropOfOthersQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_PersPropOfOthersQuotedPremium)
    '                If IsNumeric(.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium) Then partsprem += CDec(.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium)
    '                If IsNumeric(.LocationsTotal_PropertyInTheOpenRecords_QuotedPremium) Then partsprem += CDec(.LocationsTotal_PropertyInTheOpenRecords_QuotedPremium)
    '                If IsNumeric(.PackageCPR_EnhancementEndorsementQuotedPremium) Then partsprem += CDec(.PackageCPR_EnhancementEndorsementQuotedPremium)
    '                If IsNumeric(.LocationsTotal_EquipmentBreakdownQuotedPremium) Then partsprem += CDec(.LocationsTotal_EquipmentBreakdownQuotedPremium)
    '                If IsNumeric(.LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium) Then partsprem += CDec(.LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium)

    '                If partsprem < 100 Then
    '                    amtto100 = 100 - partsprem
    '                    lblCPRAmountToMeetMinimumAmountText.Text = Format(amtto100, "c")
    '                    lblCPRAmountToEqualMinimumPremium.Text = Format(amtto100, "c")
    '                    totprem = 100
    '                Else
    '                    'trCRMAmtToEqualMinumum.Visible = False
    '                    'updated 8/17/2015 to use correct row; nothing needed here since it's being checked below
    '                    'trCPRAmountToMeetMinimumPremiumRow.Visible = False
    '                    '_LinesInControl -= 1
    '                End If
    '            End If

    '            'CPRAmt = CDec(lblCprPremium.Text)
    '            'If CPRAmt < 100 Then
    '            '    CPRAmtToMeetMin = 100 = CPRAmt
    '            '    lblCPRAmountToEqualMinimumPremium.Text = Format(CPRAmtToMeetMin, "c")
    '            '    lblCPRAmountToMeetMinimumAmountText.Text = Format(CPRAmtToMeetMin, "c")
    '            'End If

    '            'added 5/13/2013 (CGL)
    '            Me.lblEnhEndPrem_Cgl.Text = .PackageGL_EnhancementEndorsementQuotedPremium '5/29/2013 - updated label name to make it easier to distinguish
    '            Me.lblPremOps.Text = .GL_PremisesTotalQuotedPremium
    '            Me.lblProdCompOps.Text = .GL_ProductsTotalQuotedPremium
    '            Me.lblOptCovs.Text = .Dec_GL_OptCovs_Premium
    '            Me.lblMinPrem_Prem.Text = .GL_PremisesMinimumQuotedPremium
    '            'Me.lblMinPrem_Prem.Text = .GL_PremisesMinimumPremiumAdjustment 'changed back to original logic 8/18/2015; commented this line and un-commented the line above
    '            Me.lblAmtForMinPrem_Prem.Text = .GL_PremisesMinimumPremiumAdjustment
    '            Me.lblMinPrem_Prod.Text = .GL_ProductsMinimumQuotedPremium
    '            'Me.lblMinPrem_Prod.Text = .GL_ProductsMinimumPremiumAdjustment 'changed back to original logic 8/18/2015; commented this line and un-commented the line above
    '            Me.lblAmtForMinPrem_Prod.Text = .GL_ProductsMinimumPremiumAdjustment

    '            ' SET THE ENHANCEMENTS TO 'CONTRACTOR ENHANCEMENT' OR 'MANUFACTURER EMHANCEMENT" FOR CPR/CGL
    '            ' MGB 6/23/15
    '            ' CPR
    '            If .CPP_CPR_ContractorsEnhancementQuotedPremium IsNot Nothing AndAlso IsNumeric(.CPP_CPR_ContractorsEnhancementQuotedPremium) AndAlso CDec(.CPP_CPR_ContractorsEnhancementQuotedPremium) > 0 Then
    '                lblCPREnhancementEndorsementText.Text = "Contractor Enhancement Endorsement"
    '                lblEnhEndPremium_Cpr.Text = .CPP_CPR_ContractorsEnhancementQuotedPremium
    '            ElseIf .CPP_CPR_ManufacturersEnhancementQuotedPremium IsNot Nothing AndAlso IsNumeric(.CPP_CPR_ManufacturersEnhancementQuotedPremium) AndAlso CDec(.CPP_CPR_ManufacturersEnhancementQuotedPremium) > 0 Then
    '                lblCPREnhancementEndorsementText.Text = "Manufacturer Enhancement Endorsement"
    '                lblEnhEndPremium_Cpr.Text = .CPP_CPR_ManufacturersEnhancementQuotedPremium
    '            End If
    '            ' CGL
    '            If .CPP_CGL_ContractorsEnhancementQuotedPremium IsNot Nothing AndAlso IsNumeric(.CPP_CGL_ContractorsEnhancementQuotedPremium) AndAlso CDec(.CPP_CPR_ContractorsEnhancementQuotedPremium) > 0 Then
    '                lblCGLEnhancementEndorsementText.Text = "Contractor Enhancement Endorsement"
    '                lblEnhEndPrem_Cgl.Text = .CPP_CGL_ContractorsEnhancementQuotedPremium
    '            ElseIf .CPP_CGL_ManufacturersEnhancementQuotedPremium IsNot Nothing AndAlso IsNumeric(.CPP_CGL_ManufacturersEnhancementQuotedPremium) AndAlso CDec(.CPP_CPR_ManufacturersEnhancementQuotedPremium) > 0 Then
    '                lblCGLEnhancementEndorsementText.Text = "Manufacturer Enhancement Endorsement"
    '                lblEnhEndPrem_Cgl.Text = .CPP_CGL_ManufacturersEnhancementQuotedPremium
    '            End If

    '            ' CIM 6/18/15 MGB
    '            ' Note: I did the CRM and CIM sections a little differently:
    '            '       - The LOB (CIM/CRM) sections may or may not be shown depending on whether the quote has them.
    '            '       - Each coverage item within CIM/CRM may or may not be shown depending on whether the coverage has been selected.
    '            '       - I show/hide the rows and decrement the line counter in one block of code instead of two.
    '            If .CPP_Has_InlandMarine_PackagePart Then
    '                totprem = 0
    '                amtto100 = 0
    '                trInlandMarine.Visible = True
    '                If .BuildersRiskQuotedPremium IsNot Nothing AndAlso .BuildersRiskQuotedPremium <> String.Empty Then
    '                    trCIMBuildersRisk.Visible = True
    '                    lblCIMBuildersRisk.Text = .BuildersRiskQuotedPremium
    '                Else
    '                    trCIMBuildersRisk.Visible = False
    '                    _LinesInControl -= 1
    '                    lblCIMBuildersRisk.Text = ""
    '                End If
    '                If .ComputerQuotedPremium IsNot Nothing AndAlso .ComputerQuotedPremium <> String.Empty Then
    '                    trCIMComputer.Visible = True
    '                    lblCIMComputer.Text = .ComputerQuotedPremium
    '                Else
    '                    trCIMComputer.Visible = False
    '                    lblCIMComputer.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .ContractorsEquipmentScheduleQuotedPremium IsNot Nothing AndAlso .ContractorsEquipmentScheduleQuotedPremium <> String.Empty Then
    '                    trCIMContractorsEquipment.Visible = True
    '                    lblCIMContractorsEquipment.Text = .ContractorsEquipmentScheduleQuotedPremium
    '                Else
    '                    trCIMContractorsEquipment.Visible = False
    '                    lblCIMContractorsEquipment.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .ContractorsEquipmentLeasedRentedFromOthersQuotedPremium IsNot Nothing AndAlso .ContractorsEquipmentLeasedRentedFromOthersQuotedPremium <> String.Empty Then
    '                    trCIMEquipmentLeasedRentedFromOthers.Visible = True
    '                    lblCIMEquipmentLeasedRentedFromOthers.Text = .ContractorsEquipmentLeasedRentedFromOthersQuotedPremium
    '                Else
    '                    trCIMEquipmentLeasedRentedFromOthers.Visible = False
    '                    lblCIMEquipmentLeasedRentedFromOthers.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                ' Note that for UNSCHEDULED TOOLS, either one (but not both) of the following premiums
    '                ' could be populated.  MGB 7/8/15
    '                If (.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium IsNot Nothing AndAlso .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium <> String.Empty) _
    '                    OrElse (.SmallToolsQuotedPremium IsNot Nothing AndAlso .SmallToolsQuotedPremium <> String.Empty) Then
    '                    trCIMUnscheduledTools.Visible = True
    '                    If .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium IsNot Nothing AndAlso .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium <> String.Empty Then
    '                        lblCIMUnscheduledTools.Text = .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium
    '                    Else
    '                        lblCIMUnscheduledTools.Text = .SmallToolsQuotedPremium
    '                    End If
    '                Else
    '                    trCIMUnscheduledTools.Visible = False
    '                    lblCIMUnscheduledTools.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .HasContractorsEnhancement Then
    '                    trCIMEnhancementEndorsement.Visible = True
    '                Else
    '                    trCIMEnhancementEndorsement.Visible = False
    '                    _LinesInControl -= 1
    '                End If
    '                If .FineArtsQuotedPremium IsNot Nothing AndAlso .FineArtsQuotedPremium <> String.Empty Then
    '                    trCIMFineArtsFloater.Visible = True
    '                    lblCIMFineArtsFloater.Text = .FineArtsQuotedPremium
    '                Else
    '                    trCIMFineArtsFloater.Visible = False
    '                    lblCIMFineArtsFloater.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .InstallationBlanketQuotedPremium IsNot Nothing AndAlso .InstallationBlanketQuotedPremium <> String.Empty Then
    '                    trCIMInstallationFloater.Visible = True
    '                    lblCIMInstallationFloater.Text = .InstallationBlanketQuotedPremium
    '                Else
    '                    trCIMInstallationFloater.Visible = False
    '                    lblCIMInstallationFloater.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .MotorTruckCargoScheduledVehicleQuotedPremium IsNot Nothing AndAlso .MotorTruckCargoScheduledVehicleQuotedPremium <> String.Empty Then
    '                    trCIMMotorTruckCargo.Visible = True
    '                    lblCIMMotorTruckCargo.Text = .MotorTruckCargoScheduledVehicleQuotedPremium
    '                Else
    '                    trCIMMotorTruckCargo.Visible = False
    '                    lblCIMMotorTruckCargo.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .OwnersCargoAnyOneOwnedVehicleQuotedPremium IsNot Nothing AndAlso .OwnersCargoAnyOneOwnedVehicleQuotedPremium <> String.Empty Then
    '                    trCIMOwnersCargo.Visible = True
    '                    lblCIMOwnersCargo.Text = .OwnersCargoAnyOneOwnedVehicleQuotedPremium
    '                Else
    '                    trCIMOwnersCargo.Visible = False
    '                    lblCIMOwnersCargo.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .ScheduledPropertyQuotedPremium IsNot Nothing AndAlso .ScheduledPropertyQuotedPremium <> String.Empty Then
    '                    trCIMSchedulePropertyFloater.Visible = True
    '                    lblCIMScheduledPropertyFloater.Text = .ScheduledPropertyQuotedPremium
    '                Else
    '                    trCIMSchedulePropertyFloater.Visible = False
    '                    lblCIMScheduledPropertyFloater.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .SignsQuotedPremium IsNot Nothing AndAlso .SignsQuotedPremium <> String.Empty Then
    '                    trCIMSigns.Visible = True
    '                    lblCIMSigns.Text = .SignsQuotedPremium
    '                Else
    '                    trCIMSigns.Visible = False
    '                    lblCIMSigns.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .TransportationCatastropheQuotedPremium IsNot Nothing AndAlso .TransportationCatastropheQuotedPremium <> String.Empty Then
    '                    trCIMTransportation.Visible = True
    '                    lblCIMTransportation.Text = .TransportationCatastropheQuotedPremium
    '                Else
    '                    trCIMTransportation.Visible = False
    '                    lblCIMTransportation.Text = ""
    '                    _LinesInControl -= 1
    '                End If

    '                If .CPP_CIM_PackagePart_QuotedPremium IsNot Nothing AndAlso .CPP_CIM_PackagePart_QuotedPremium <> String.Empty AndAlso IsNumeric(.CPP_CIM_PackagePart_QuotedPremium) Then
    '                    totprem = CDec(.CPP_CIM_PackagePart_QuotedPremium)
    '                    Dim partsprem As Decimal = 0
    '                    If IsNumeric(.BuildersRiskQuotedPremium) Then partsprem += CDec(.BuildersRiskQuotedPremium)
    '                    If IsNumeric(.ComputerQuotedPremium) Then partsprem += CDec(.ComputerQuotedPremium)
    '                    If IsNumeric(.ContractorsEquipmentScheduleQuotedPremium) Then partsprem += CDec(.ContractorsEquipmentScheduleQuotedPremium)
    '                    If IsNumeric(.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium) Then partsprem += CDec(.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)
    '                    If IsNumeric(.SmallToolsQuotedPremium) Then partsprem += CDec(.SmallToolsQuotedPremium)
    '                    If IsNumeric(.FineArtsQuotedPremium) Then partsprem += CDec(.FineArtsQuotedPremium)
    '                    If IsNumeric(.InstallationBlanketQuotedPremium) Then partsprem += CDec(.InstallationBlanketQuotedPremium)
    '                    If IsNumeric(.MotorTruckCargoScheduledVehicleQuotedPremium) Then partsprem += CDec(.MotorTruckCargoScheduledVehicleQuotedPremium)
    '                    If IsNumeric(.OwnersCargoAnyOneOwnedVehicleQuotedPremium) Then partsprem += CDec(.OwnersCargoAnyOneOwnedVehicleQuotedPremium)
    '                    If IsNumeric(.ScheduledPropertyQuotedPremium) Then partsprem += CDec(.ScheduledPropertyQuotedPremium)
    '                    If IsNumeric(.SignsQuotedPremium) Then partsprem += CDec(.SignsQuotedPremium)
    '                    If IsNumeric(.TransportationCatastropheQuotedPremium) Then partsprem += CDec(.TransportationCatastropheQuotedPremium)

    '                    If partsprem < 100 Then
    '                        trCIMAmountToMeetMinimumRow.Visible = True
    '                        amtto100 = 100 - partsprem
    '                        lblCIMAmountToMeetMinimumText.Text = "Amount to Equal Minimum Premium - (" & Format(amtto100, "c") & ")"
    '                        lblCIMAmountToMeetMinimumAmount.Text = Format(amtto100, "c")
    '                        totprem = 100
    '                    Else
    '                        trCIMAmountToMeetMinimumRow.Visible = False
    '                        _LinesInControl -= 1
    '                    End If
    '                End If

    '                lblCIMTotalPremium.Text = Format(totprem, "c")
    '            Else
    '                trInlandMarine.Visible = False
    '                _LinesInControl -= 16
    '            End If
    '            ' CRM 6/18/15 MGB
    '            If .CPP_Has_Crime_PackagePart Then
    '                totprem = 0
    '                amtto100 = 0
    '                trCommercialCrime.Visible = True
    '                If .EmployeeTheftQuotedPremium IsNot Nothing AndAlso .EmployeeTheftQuotedPremium <> String.Empty Then
    '                    trCRMEmployeeTheft.Visible = True
    '                    lblCRMEmployeeTheft.Text = .EmployeeTheftQuotedPremium
    '                Else
    '                    trCRMEmployeeTheft.Visible = False
    '                    lblCRMEmployeeTheft.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium IsNot Nothing AndAlso .InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium <> String.Empty Then
    '                    trCRMInsidePremises.Visible = True
    '                    lblCRMInsidePremises.Text = .InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium
    '                Else
    '                    trCRMInsidePremises.Visible = False
    '                    lblCRMInsidePremises.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .OutsideThePremisesQuotedPremium IsNot Nothing AndAlso .OutsideThePremisesQuotedPremium <> String.Empty Then
    '                    trCRMOutsideThePremises.Visible = True
    '                    lblCRMOutsideThePremises.Text = .OutsideThePremisesQuotedPremium
    '                Else
    '                    trCRMOutsideThePremises.Visible = False
    '                    lblCRMOutsideThePremises.Text = ""
    '                    _LinesInControl -= 1
    '                End If
    '                If .CPP_CRM_PackagePart_QuotedPremium IsNot Nothing AndAlso .CPP_CRM_PackagePart_QuotedPremium <> String.Empty AndAlso IsNumeric(.CPP_CRM_PackagePart_QuotedPremium) Then
    '                    totprem = CDec(.CPP_CRM_PackagePart_QuotedPremium)
    '                    Dim partsprem As Decimal = 0
    '                    If IsNumeric(.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium) Then partsprem += CDec(.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium)
    '                    If IsNumeric(.OutsideThePremisesQuotedPremium) Then partsprem += CDec(.OutsideThePremisesQuotedPremium)
    '                    If IsNumeric(.EmployeeTheftQuotedPremium) Then partsprem += CDec(.EmployeeTheftQuotedPremium)

    '                    If partsprem < 100 Then
    '                        amtto100 = 100 - partsprem
    '                        lblCRMAmtToEqualMinPremiumText.Text = "Amount to Equal Minimum Premium - (" & Format(amtto100, "c") & ")"
    '                        trCRMAmtToEqualMinumum.Visible = True
    '                        lblCRMAmtToEqualMinPremiumAmount.Text = Format(amtto100, "c")
    '                        totprem = 100
    '                    Else
    '                        trCRMAmtToEqualMinumum.Visible = False
    '                        _LinesInControl -= 1
    '                    End If
    '                End If
    '                lblCRMTotalPremium.Text = Format(totprem, "c")
    '            Else
    '                trCommercialCrime.Visible = False
    '                _LinesInControl -= 7
    '            End If
    '        End With
    '    End If
    '    'added 5/29/2013
    '    'CPR
    '    If qqHelper.IsZeroPremium(Me.lblBuildingPremium.Text) = True Then
    '        Me.BuildingRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblPersPropPremium.Text) = True Then
    '        Me.PersPropRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblPersPropOfOthersPremium.Text) = True Then
    '        Me.PersPropOfOthersRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblBusIncPremium.Text) = True Then
    '        Me.BusIncRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblPropInTheOpenPremium.Text) = True Then
    '        Me.PropInTheOpenRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblEnhEndPremium_Cpr.Text) = True Then
    '        Me.EnhEnd_CprRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblEbPremium.Text) = True Then
    '        Me.EbRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblEqPremium.Text) = True Then
    '        Me.EqRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblCPRAmountToEqualMinimumPremium.Text) Then
    '        Me.trCPRAmountToMeetMinimumPremiumRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    'CGL
    '    If qqHelper.IsZeroPremium(Me.lblEnhEndPrem_Cgl.Text) = True Then
    '        Me.EnhEnd_CglRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblPremOps.Text) = True Then
    '        Me.PremOpsRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblProdCompOps.Text) = True Then
    '        Me.ProdCompOpsRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblOptCovs.Text) = True Then
    '        Me.OptCovsRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblAmtForMinPrem_Prem.Text) = True Then
    '        Me.MinPrem_PremRow.Visible = False
    '        _LinesInControl -= 1
    '    End If
    '    If qqHelper.IsZeroPremium(Me.lblAmtForMinPrem_Prod.Text) = True Then
    '        Me.MinPrem_ProdRow.Visible = False
    '        _LinesInControl -= 1
    '    End If

    'End Sub

    'added 7/1/2015
    'Private Sub ResetPageLineCount(ByRef currLines As Integer, Optional ByVal isFirstPage As Boolean = False)
    '    If isFirstPage = True Then
    '        currLines = 18 'everything before adding LOB summaries (assuming 8 lines for ClientAndAgencyInfo control; everything else alloted to logo, headers, and breaks)
    '    Else
    '        currLines = 0
    '    End If

    '    'also add in lines after LOB summaries (combined premium, breaks, and disclaimer) since they'll always be on the last page w/ the 
    '    'currLines += 5 'breaks and combined premium
    '    'currLines += 8 'disclaimer control
    '    'updated to only add into total if there are no remaining controls left
    'End Sub
    'Private Sub AddPageBreakToPlaceholder(ByRef pageNum As Integer, ByRef controlCount As Integer, Optional ByRef currLines As Integer = Nothing, Optional ByVal controlLines As Integer = Nothing)
    '    proposalHelper.AddPageBreakToPlaceholder(Me.phControls)
    '    pageNum += 1
    '    controlCount = 0
    '    If currLines <> Nothing Then
    '        ResetPageLineCount(currLines)
    '        If controlLines <> Nothing AndAlso controlLines > 0 Then 'add lines back in for control that caused break (since it will be on the next page)
    '            currLines += controlLines
    '        End If
    '    End If
    'End Sub
    'Private Sub DetermineIfPageBreakIsNeeded(ByRef pageNum As Integer, ByRef currLines As Integer, ByRef controlCount As Integer, ByVal controlLines As Integer, ByVal isLastControl As Boolean)
    '    currLines += controlLines
    '    If isLastControl = True Then
    '        currLines += 13 '5 for breaks and combined prem and 8 for disclaimer
    '    End If
    '    If controlCount <> Nothing AndAlso controlCount > 0 AndAlso currLines > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '        'updated 7/1/2015 to account for CPP Summary control now having the ability to wrap to extra page by itself (w/o extra quotes/controls)... not going to work since none of the LOB summary controls have built-in page breaks... just breaks in between each quote
    '        'If (currLines >= 30 OrElse (controlCount <> Nothing AndAlso controlCount > 0)) AndAlso currLines > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '        AddPageBreakToPlaceholder(pageNum, controlCount, currLines, controlLines)
    '    End If
    'End Sub
End Class
