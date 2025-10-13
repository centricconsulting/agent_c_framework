'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_CAP_Summary
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass 'added 5/29/2013 to check for zero premiums

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
    'added 6/28/2013
    'Private _LinesInControl As Integer = 12 'breaks and rows; will need to adjust if any breaks are added
    'updated 5/10/2017 for GarageKeepers
    'Private _LinesInControl As Integer = 13 'breaks and rows; will need to adjust if any breaks are added
    'updated 5/17/2017 for MinPremAdj
    'Private _LinesInControl As Integer = 14 'breaks and rows; will need to adjust if any breaks are added
    'updated 6/20/2017 for Enhancement Endorsement
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
                'Me.lblQuoteNumber.Text = .QuoteNumber
                'updated 4/5/2017 for Diamond Proposals
                Me.lblQuoteNumber.Text = .PolicyNumber
                Me.lblTotalPremium.Text = .TotalQuotedPremium
                Dim ExpDate As Date = Date.Today
                If IsDate(.EffectiveDate) Then
                    ExpDate = DateAdd(DateInterval.Year, +1, CDate(.EffectiveDate))
                End If                               
                Me.lblEffectiveDate.Text = .EffectiveDate
                Me.lblExpirationDate.Text = ExpDate
                'Me.lblLiabilityPremium.Text = .VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium 'limit - .Liability_UM_UIM_Limit
                'Me.lblMedPayPremium.Text = .VehiclesTotal_MedicalPaymentsQuotedPremium 'limit - .MedicalPaymentsLimit
                'Me.lblUmUimPremium.Text = .VehiclesTotal_UM_UIM_CovsQuotedPremium 'limit - .Liability_UM_UIM_Limit
                'Me.lblCompPremium.Text = .VehiclesTotal_ComprehensiveCoverageQuotedPremium
                'Me.lblCollPremium.Text = .VehiclesTotal_CollisionCoverageQuotedPremium
                'Me.lblTowPremium.Text = .VehiclesTotal_TowingAndLaborQuotedPremium
                'Me.lblRentPremium.Text = .VehiclesTotal_RentalReimbursementQuotedPremium
                'Me.lblOptCovsPremium.Text = .Dec_CAP_OptCovs_Premium
                'updated 5/10/2017 to use new properties; also added GarageKeepers
                'Me.lblLiabilityPremium.Text = .AutoLiabilityTotalPremium
                'Me.lblMedPayPremium.Text = .AutoMedicalPaymentsTotalPremium
                'Me.lblUmUimPremium.Text = .Auto_UM_UIM_TotalPremium
                'Me.lblCompPremium.Text = .AutoComprehensiveTotalPremium
                'Me.lblCollPremium.Text = .AutoCollisionTotalPremium
                'Me.lblTowPremium.Text = .VehiclesTotal_TowingAndLaborQuotedPremium
                'Me.lblRentPremium.Text = .VehiclesTotal_RentalReimbursementQuotedPremium
                'Me.lblGarageKeepersPremium.Text = .GarageKeepersTotalPremium
                'Me.lblEnhEndPrem.Text = .BusinessMasterEnhancementQuotedPremium 'added 6/20/2017
                'Me.lblOptCovsPremium.Text = .CAP_GAR_OptCovs_Premium
                'Me.lblMinPremAdj.Text = .MinimumPremiumAdjustment 'added 5/17/2017
                'updated 10/11/2018 for multi-state
                Me.lblLiabilityPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoLiabilityTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblMedPayPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoMedicalPaymentsTotalPremium, maintainFormattingOrDefaultValue:=True)
                If CAP_UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(_QuickQuote) Then
                    Me.lblUmPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Me.lblUimPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium, maintainFormattingOrDefaultValue:=True)
                    If ((_QuickQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana AndAlso qqHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId)) OrElse (_QuickQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois AndAlso qqHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId))) Then
                        Me.lblUmpdPremium.Text = "Included"
                    End If
                Else
                    Me.lblUmUimPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .Auto_UM_UIM_TotalPremium, maintainFormattingOrDefaultValue:=True)
                End If
                Me.lblCompPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoComprehensiveTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCollPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .AutoCollisionTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblTowPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .VehiclesTotal_TowingAndLaborQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblRentPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .VehiclesTotal_RentalReimbursementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblGarageKeepersPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GarageKeepersTotalPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblEnhEndPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .BusinessMasterEnhancementQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblOptCovsPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CAP_GAR_OptCovs_Premium, maintainFormattingOrDefaultValue:=True)
                Me.lblMinPremAdj.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
            End With
        End If

        Dim proposalPrems As String = "" 'added 5/16/2017; note: already has OptCovs based off of prems that QuickQuoteObject takes into account

        'added 5/29/2013
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
        If CAP_UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(_QuickQuote) Then
            Me.UmUimRow.Visible = False
            _LinesInControl -= 1
            If qqHelper.IsZeroPremium(Me.lblUmPremium.Text) = True Then
                Me.UmRow.Visible = False
            Else
                Me.UmRow.Visible = True
                proposalPrems = qqHelper.getSum(proposalPrems, Me.lblUmPremium.Text)
                _LinesInControl += 1
            End If
            If ((_QuickQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana AndAlso qqHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId)) OrElse (_QuickQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois AndAlso qqHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId))) Then
                Me.UmpdRow.Visible = True
                _LinesInControl += 1
            Else
                Me.UmpdRow.Visible = False
            End If
            If qqHelper.IsZeroPremium(Me.lblUimPremium.Text) = True Then
                Me.UimRow.Visible = False
            Else
                Me.UimRow.Visible = True
                proposalPrems = qqHelper.getSum(proposalPrems, Me.lblUimPremium.Text)
                _LinesInControl += 1
            End If
        Else
            If qqHelper.IsZeroPremium(Me.lblUmUimPremium.Text) = True Then
                Me.UmUimRow.Visible = False
                _LinesInControl -= 1
            Else 'added 5/16/2017
                proposalPrems = qqHelper.getSum(proposalPrems, Me.lblUmUimPremium.Text)
            End If
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
        If qqHelper.IsZeroPremium(Me.lblTowPremium.Text) = True Then
            Me.TowRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblTowPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblRentPremium.Text) = True Then
            Me.RentRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblRentPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblGarageKeepersPremium.Text) = True Then 'added 5/10/2017
            Me.GarageKeepersRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblGarageKeepersPremium.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblEnhEndPrem.Text) = True Then 'added 6/20/2017
            Me.EnhEndRow.Visible = False
            _LinesInControl -= 1
        Else
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblEnhEndPrem.Text)
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


        Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"
        Dim LPDP_Coverage As QuickQuoteCoverage = Nothing
        Dim lpdpValue As Decimal
        If Me.GoverningStateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            LPDP_Coverage = ProposalHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)

            If LPDP_Coverage IsNot Nothing Then
                lpdpValue = ProposalHelperClass.TryToGetDec(LPDP_Coverage.FullTermPremium)
                
                'show LPDP section
                trLargePremiumDiscount.Visible = True
                lblTotalLargePremiumDiscount.Text = qqHelper.QuotedPremiumFormat(lpdpValue)
                _LinesInControl += 1
            End If
        End If

        'added 5/16/2017
        Me.lblOptCovsPremium.Text = "" 'resetting as-of 5/17/2017
        If qqHelper.IsPositiveDecimalString(proposalPrems) = True Then
            If lpdpValue < 0 Then
                proposalPrems = qqHelper.getSum(proposalPrems, lpdpValue) 'Added 10/07/2022 for bug 65640 MLW
            End If
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
                    'If LPDP_Coverage IsNot Nothing Then
                    '    estOptCovs = qqHelper.getDiff(estOptCovs, lpdpValue.ToString())
                    'End If
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
