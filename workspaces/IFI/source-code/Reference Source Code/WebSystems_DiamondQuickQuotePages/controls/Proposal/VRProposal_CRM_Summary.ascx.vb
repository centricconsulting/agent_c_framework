'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_CRM_Summary
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass
    Dim proposalHelper As New ProposalHelperClass

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
    'Private _LinesInControl As Integer = 9 'breaks and rows; will need to adjust if any breaks are added; note 8/17/2015: 7 rows (6 + Spacer... actually 1 additional row for CPP header but only 1 visible at-a-time) and 2 breaks; 5/13/2017 note: does not include Comments Row since logic below will add lines if needed
    'updated 5/17/2017 for OptCovs row
    Private _LinesInControl As Integer = 13 'breaks and rows; will need to adjust if any breaks are added
    Public ReadOnly Property LinesInControl As Integer
        Get
            Return _LinesInControl
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub
    Private Sub SetSummaryLabels()

        If _QuickQuote IsNot Nothing Then
            With _QuickQuote
                'Me.lblQuoteNumber.Text = .QuoteNumber
                'updated 4/5/2017 for Diamond Proposals
                Me.lblQuoteNumber.Text = .PolicyNumber
                'Me.lblTotalPremium.Text = .TotalQuotedPremium
                'Me.lblCRMEmployeeTheft.Text = .EmployeeTheftQuotedPremium
                'Me.lblCRMInsidePremises.Text = .InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium
                'Me.lblCRMOutsideThePremises.Text = .OutsideThePremisesQuotedPremium
                'updated 10/11/2018 for multi-state
                Me.lblCRMEmployeeTheft.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .EmployeeTheftQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCRMInsidePremises.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCRMOutsideThePremises.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .OutsideThePremisesQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCRMForgeryAlteration.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ForgeryAlterationQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCRMComputerFraud.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ComputerFraudQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCRMFundsTransferFraud.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .FundsTransferFraudQuotedPremium, maintainFormattingOrDefaultValue:=True)

                'added 8/17/2015
                If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    'CRM packagePart for CPP lob
                    Me.CPP_header_Row.Visible = True
                    Me.Monoline_header_Row.Visible = False
                    Me.startBreak.Visible = False
                    Me.endBreak.Visible = False
                    _LinesInControl -= 2
                    Me.quoteNumberSection.Visible = False 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    'Me.lblTotalPremium.Text = .CPP_CRM_PackagePart_QuotedPremium
                    'updated 10/11/2018 for multi-state
                    Me.lblTotalPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CRM_PackagePart_QuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Me.lblPremiumText.Text = "Commercial Crime Total Premium"
                    Me.SpacerRow.Visible = True
                    'added 5/17/2017; no longer need calc below
                    'Me.lblCRMAmtToEqualMinPremiumAmount.Text = .CPP_MinPremAdj_CRM
                    'updated 10/11/2018 for multi-state
                    Me.lblCRMAmtToEqualMinPremiumAmount.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_MinPremAdj_CRM, maintainFormattingOrDefaultValue:=True)
                Else
                    'CRM lob
                    Me.Monoline_header_Row.Visible = True
                    Me.CPP_header_Row.Visible = False
                    Me.startBreak.Visible = True
                    Me.endBreak.Visible = True
                    Me.quoteNumberSection.Visible = True 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.lblTotalPremium.Text = .TotalQuotedPremium
                    Me.lblPremiumText.Text = "Total Premium Due" 'should be default
                    Me.SpacerRow.Visible = False
                    _LinesInControl -= 1
                    'added 5/17/2017; no longer need calc below
                    'Me.lblCRMAmtToEqualMinPremiumAmount.Text = .MinimumPremiumAdjustment
                    'updated 10/11/2018 for multi-state
                    Me.lblCRMAmtToEqualMinPremiumAmount.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                End If

                ''Dim totprem As Decimal = 0
                'Dim amtto100 As Decimal = 0
                'If qqHelper.IsNumericString(Me.lblTotalPremium.Text) = True Then
                '    'totprem = CDec(Me.lblTotalPremium.Text)
                '    Dim partsprem As Decimal = 0
                '    If IsNumeric(.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium) Then partsprem += CDec(.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium)
                '    If IsNumeric(.OutsideThePremisesQuotedPremium) Then partsprem += CDec(.OutsideThePremisesQuotedPremium)
                '    If IsNumeric(.EmployeeTheftQuotedPremium) Then partsprem += CDec(.EmployeeTheftQuotedPremium)

                '    If partsprem < 100 Then
                '        amtto100 = 100 - partsprem
                '        lblCRMAmtToEqualMinPremiumText.Text = Format("100", "c")
                '        'trCRMAmtToEqualMinumum.Visible = True
                '        lblCRMAmtToEqualMinPremiumAmount.Text = Format(amtto100, "c")
                '        'totprem = 100
                '    Else
                '        'trCRMAmtToEqualMinumum.Visible = False 'nothing needed here since it's being checked below
                '        '_LinesInControl -= 1
                '    End If
                'End If
                'removed calc logic 5/17/2017; now using props above; also correctly formatted minAmt since it was just showing c
                Me.lblCRMAmtToEqualMinPremiumText.Text = qqHelper.QuotedPremiumFormat("100")

                'added 5/13/2017 so Comments can show in Summary section if there isn't a control for the LOB
                Me.CommentsRow.Visible = False
                '5/15/2017 - removed logic below since we now have a LOB control w/ Comments
                'If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialCrime Then 'as opposed to CPP, which already includes CRM packagePart w/ Comments
                '    Dim hasLobControl As Boolean = False
                '    Dim pHelper As New ProposalHelperClass
                '    pHelper.CheckForProposalLobControlAndAddToPlaceholder(_QuickQuote, hasLobControl:=hasLobControl, addToPlaceholder:=False)
                '    If hasLobControl = False Then
                '        Dim commentsLineCount As Integer = 0
                '        Me.lblComments.Text = qqHelper.HtmlForQuickQuoteProposalCommentsForLobType(.Comments, QuickQuoteObject.QuickQuoteLobType.CommercialCrime, includeBreakAtBeginning:=True, lineCount:=commentsLineCount)
                '        If commentsLineCount > 0 Then
                '            _LinesInControl += commentsLineCount
                '            Me.CommentsRow.Visible = True
                '        Else
                '            Me.CommentsRow.Visible = False 'redundant
                '        End If
                '    End If
                'End If

            End With
        End If

        Dim proposalPrems As String = "" 'added 5/16/2017; note: amtToEqualMinPrem may be incorrect since it's based off of prems that are already taken into account

        If qqHelper.IsZeroPremium(Me.lblCRMEmployeeTheft.Text) Then
            Me.trCRMEmployeeTheft.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMEmployeeTheft.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCRMInsidePremises.Text) Then
            Me.trCRMInsidePremises.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMInsidePremises.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCRMOutsideThePremises.Text) Then
            Me.trCRMOutsideThePremises.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMOutsideThePremises.Text)
        End If

        'added 02/05/2020 to support new coverages
        If qqHelper.IsZeroPremium(Me.lblCRMForgeryAlteration.Text) Then
            Me.trCRMForgeryAlteration.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMForgeryAlteration.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCRMComputerFraud.Text) Then
            Me.trCRMComputerFraud.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMComputerFraud.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCRMFundsTransferFraud.Text) Then
            Me.trCRMFundsTransferFraud.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMFundsTransferFraud.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCRMAmtToEqualMinPremiumAmount.Text) Then
            Me.trCRMAmtToEqualMinumum.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCRMAmtToEqualMinPremiumAmount.Text)
        End If

        'added 5/16/2017
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If qqHelper.IsPositiveDecimalString(Me.lblTotalPremium.Text) = True AndAlso CDec(Me.lblTotalPremium.Text) > CDec(proposalPrems) Then
                'needs optional covs; need to use new logic for MinPremAdj (diff prop for monoline vs CPP)
                Dim estOptCovs As String = qqHelper.getDiff(Me.lblTotalPremium.Text, proposalPrems)
                If qqHelper.IsPositiveDecimalString(estOptCovs) = True Then
                    'looks like valid amount to show; need to set label to estOptCovs and show OptCovsRow; use qqHelper.QuotedPremiumFormat
                    Me.lblOptCovsPremium.Text = qqHelper.QuotedPremiumFormat(estOptCovs) 'added 5/17/2017
                End If
            End If
        End If
        If qqHelper.IsZeroPremium(Me.lblOptCovsPremium.Text) = True Then 'added 5/17/2017
            Me.OptCovsRow.Visible = False
            _LinesInControl -= 1
        End If
    End Sub
End Class
