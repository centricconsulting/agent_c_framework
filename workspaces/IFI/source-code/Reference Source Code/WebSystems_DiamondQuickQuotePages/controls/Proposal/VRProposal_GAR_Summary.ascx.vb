Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_GAR_Summary 'added 4/22/2017
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass

    '5/10/2017 note: CAP and GAR Summary controls have the exact same coverages; GAR linesInControl is only bigger because of CPP/monoline functionality, though extra lines are hidden, and they should match for monoline

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

    'Private _LinesInControl As Integer = 6 'breaks and rows; will need to adjust if any breaks are added
    'updated 4/28/2017 for new rows (7); should've been 5 originally since only 1 Monoline/CPP row will show at a time
    'Private _LinesInControl As Integer = 12 'breaks and rows; will need to adjust if any breaks are added
    'updated 5/10/2017 for towingLabor and rentalReimbursement
    'Private _LinesInControl As Integer = 14 'breaks and rows; will need to adjust if any breaks are added; 5/13/2017 note: does not include Comments Row since logic below will add lines if needed
    'updated 5/17/2017 for MinPremAdj
    Private _LinesInControl As Integer = 15 'breaks and rows; will need to adjust if any breaks are added
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
                Me.lblQuoteNumber.Text = .PolicyNumber

                'added 4/28/2017
                'Me.lblLiabilityPremium.Text = .VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium 'limit - .Liability_UM_UIM_Limit
                'Me.lblMedPayPremium.Text = .VehiclesTotal_MedicalPaymentsQuotedPremium 'limit - .MedicalPaymentsLimit
                'Me.lblUmUimPremium.Text = .VehiclesTotal_UM_UIM_CovsQuotedPremium 'limit - .Liability_UM_UIM_Limit
                'Me.lblCompPremium.Text = .VehiclesTotal_ComprehensiveCoverageQuotedPremium
                'Me.lblCollPremium.Text = .VehiclesTotal_CollisionCoverageQuotedPremium
                'Me.lblGarageKeepersPremium.Text = .GarageKeepersTotalPremium 'added 5/9/2017
                'Me.lblOptCovsPremium.Text = .Dec_CAP_OptCovs_Premium
                'updated 5/10/2017 to use new properties; should add in towingLabor and rental like CAP (since they're taken into account for new OptCovs prem)... won't show unless it's on the quote... now there
                'Me.lblLiabilityPremium.Text = .AutoLiabilityTotalPremium
                'Me.lblMedPayPremium.Text = .AutoMedicalPaymentsTotalPremium
                'Me.lblUmUimPremium.Text = .Auto_UM_UIM_TotalPremium
                'Me.lblCompPremium.Text = .AutoComprehensiveTotalPremium
                'Me.lblCollPremium.Text = .AutoCollisionTotalPremium
                'Me.lblTowPremium.Text = .VehiclesTotal_TowingAndLaborQuotedPremium
                'Me.lblRentPremium.Text = .VehiclesTotal_RentalReimbursementQuotedPremium
                'Me.lblGarageKeepersPremium.Text = .GarageKeepersTotalPremium
                'Me.lblOptCovsPremium.Text = .CAP_GAR_OptCovs_Premium
                'updated 10/11/2018 for multi-state
                Me.lblLiabilityPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoLiabilityTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblMedPayPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoMedicalPaymentsTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblUmUimPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Auto_UM_UIM_TotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCompPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoComprehensiveTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCollPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoCollisionTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblTowPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .VehiclesTotal_TowingAndLaborQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblRentPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .VehiclesTotal_RentalReimbursementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblGarageKeepersPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GarageKeepersTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblOptCovsPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CAP_GAR_OptCovs_Premium, maintainFormattingOrDefaultValue:=True)

                If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    'GAR packagePart for CPP lob
                    Me.CPP_header_Row.Visible = True
                    Me.Monoline_header_Row.Visible = False
                    'Me.lblTotalPremium.Text = .CPP_GAR_PackagePart_QuotedPremium
                    'updated 10/11/2018 for multi-state
                    Me.lblTotalPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_GAR_PackagePart_QuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Me.lblPremiumText.Text = "Garage Total Premium"

                    Me.startBreak.Visible = False
                    Me.endBreak.Visible = False
                    _LinesInControl -= 2
                    Me.quoteNumberSection.Visible = False 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.SpacerRow.Visible = True
                    'Me.lblMinPremAdj.Text = .CPP_MinPremAdj_GAR 'added 5/17/2017
                    'updated 10/11/2018 for multi-state
                    Me.lblMinPremAdj.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_MinPremAdj_GAR, maintainFormattingOrDefaultValue:=True)
                Else
                    'GAR lob
                    Me.Monoline_header_Row.Visible = True
                    Me.CPP_header_Row.Visible = False
                    Me.lblTotalPremium.Text = .TotalQuotedPremium
                    Me.lblPremiumText.Text = "Total Premium Due" 'should be default

                    Me.startBreak.Visible = True
                    Me.endBreak.Visible = True
                    Me.quoteNumberSection.Visible = True 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    Me.SpacerRow.Visible = False
                    _LinesInControl -= 1
                    'Me.lblMinPremAdj.Text = .MinimumPremiumAdjustment 'added 5/17/2017
                    'updated 10/11/2018 for multi-state
                    Me.lblMinPremAdj.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                End If

                'added 5/13/2017 so Comments can show in Summary section if there isn't a control for the LOB
                Me.CommentsRow.Visible = False
                '5/15/2017 - removed logic below since we now have a LOB control w/ Comments
                'If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGarage Then 'as opposed to CPP; will just use this and not pass in lobToCheckFor below once Matt has CPP updated; added IF back in 5/15/2017
                '    Dim hasLobControl As Boolean = False
                '    Dim pHelper As New ProposalHelperClass
                '    'note: only passing in lobToCheckFor temporarily since Matt will likely have this working for CPP before code in this project is updated for monoline control; will then just check when monoline
                '    'pHelper.CheckForProposalLobControlAndAddToPlaceholder(_QuickQuote, lobToCheckFor:=QuickQuoteObject.QuickQuoteLobType.CommercialGarage, hasLobControl:=hasLobControl, addToPlaceholder:=False)
                '    'updated 5/15/2017 now that latest CPP control w/ GAR is available
                '    pHelper.CheckForProposalLobControlAndAddToPlaceholder(_QuickQuote, hasLobControl:=hasLobControl, addToPlaceholder:=False)
                '    If hasLobControl = False Then
                '        Dim commentsLineCount As Integer = 0
                '        Me.lblComments.Text = qqHelper.HtmlForQuickQuoteProposalCommentsForLobType(.Comments, QuickQuoteObject.QuickQuoteLobType.CommercialGarage, includeBreakAtBeginning:=True, lineCount:=commentsLineCount)
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

        Dim proposalPrems As String = "" 'added 5/16/2017; note: already has OptCovs based off of prems that QuickQuoteObject takes into account

        'added 4/28/2017
        If qqHelper.IsZeroPremium(Me.lblLiabilityPremium.Text) = True Then
            Me.LiabilityRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblLiabilityPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblMedPayPremium.Text) = True Then
            Me.MedPayRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblMedPayPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblUmUimPremium.Text) = True Then
            Me.UmUimRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblUmUimPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCompPremium.Text) = True Then
            Me.CompRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCompPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCollPremium.Text) = True Then
            Me.CollRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCollPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblTowPremium.Text) = True Then 'added 5/10/2017
            Me.TowRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblTowPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblRentPremium.Text) = True Then 'added 5/10/2017
            Me.RentRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblRentPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblGarageKeepersPremium.Text) = True Then
            Me.GarageKeepersRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblGarageKeepersPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblMinPremAdj.Text) = True Then 'added 5/17/2017
            Me.MinPremAdjRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblMinPremAdj.Text)
        End If
        'If qqHelper.IsZeroPremium(Me.lblOptCovsPremium.Text) = True Then '5/17/2017 - moved below
        '    Me.OptCovsRow.Visible = False
        '    _LinesInControl -= 1
        'End If

        'added 5/16/2017
        Me.lblOptCovsPremium.Text = "" 'resetting as-of 5/17/2017
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If qqHelper.IsPositiveDecimalString(Me.lblTotalPremium.Text) = True AndAlso CDec(Me.lblTotalPremium.Text) > CDec(proposalPrems) Then
                'needs optional covs; see if difference is already the same as Me.lblOptCovsPrem.Text; may need to check MinPremAdj and add row for that too
                Dim estOptCovs As String = qqHelper.getDiff(Me.lblTotalPremium.Text, proposalPrems)
                If qqHelper.IsPositiveDecimalString(estOptCovs) = True Then
                    'looks like valid amount to show
                    'If qqHelper.IsPositiveDecimalString(Me.lblOptCovsPremium.Text) = True Then
                    '    If CDec(Me.lblOptCovsPremium.Text) = CDec(estOptCovs) Then
                    '        'same amount; nothing needed
                    '    Else
                    '        'should probably overwrite label w/ estOptCovs; use qqHelper.QuotedPremiumFormat

                    '    End If
                    'Else
                    '    'likely not showing yet; need to set label to estOptCovs and show OptCovsRow; use qqHelper.QuotedPremiumFormat

                    'End If
                    'updated 5/17/2017 after adding reset code above
                    Me.lblOptCovsPremium.Text = qqHelper.QuotedPremiumFormat(estOptCovs)
                End If
            End If
        End If
        If qqHelper.IsZeroPremium(Me.lblOptCovsPremium.Text) = True Then '5/17/2017 - moved here from above
            Me.OptCovsRow.Visible = False
            _LinesInControl -= 1
        End If

    End Sub
End Class
