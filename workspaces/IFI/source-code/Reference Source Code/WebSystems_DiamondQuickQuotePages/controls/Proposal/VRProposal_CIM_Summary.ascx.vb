'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_CIM_Summary
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
    'Private _LinesInControl As Integer = 19 'breaks and rows; will need to adjust if any breaks are added; note 8/17/2015: 17 rows (16 + Spacer... actually 1 additional row for CPP header but only 1 visible at-a-time) and 2 breaks; 5/13/2017 note: does not include Comments Row since logic below will add lines if needed
    'updated 5/15/2017 for Golf Cart/Course rows
    'Private _LinesInControl As Integer = 21
    'updated 5/17/2017 for OptCovs row
    Private _LinesInControl As Integer = 22
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
                'Me.lblCIMBuildersRisk.Text = .BuildersRiskQuotedPremium
                'Me.lblCIMComputer.Text = .ComputerQuotedPremium
                'Me.lblCIMContractorsEquipment.Text = .ContractorsEquipmentScheduleQuotedPremium
                'Me.lblCIMEquipmentLeasedRentedFromOthers.Text = .ContractorsEquipmentLeasedRentedFromOthersQuotedPremium
                'If qqHelper.IsZeroPremium(.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium) = False Then
                '    Me.lblCIMUnscheduledTools.Text = .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium
                'Else
                '    Me.lblCIMUnscheduledTools.Text = .SmallToolsQuotedPremium
                'End If
                'If .HasContractorsEnhancement = True Then
                '    Me.lblCIMEnhancementEndorsementText.Text = "INCLUDED" 'may eventually use premium
                'Else
                '    Me.lblCIMEnhancementEndorsementText.Text = ""
                'End If
                'Me.lblCIMFineArtsFloater.Text = .FineArtsQuotedPremium
                'Me.lblCIMInstallationFloater.Text = .InstallationBlanketQuotedPremium
                'Me.lblCIMMotorTruckCargo.Text = .MotorTruckCargoScheduledVehicleQuotedPremium
                'Me.lblCIMOwnersCargo.Text = .OwnersCargoAnyOneOwnedVehicleQuotedPremium
                'Me.lblCIMScheduledPropertyFloater.Text = .ScheduledPropertyQuotedPremium
                'Me.lblCIMSigns.Text = .SignsQuotedPremium
                'Me.lblCIMTransportation.Text = .TransportationCatastropheQuotedPremium

                ''added 5/15/2017
                'Me.lblCIMGolfCart.Text = .GolfCartQuotedPremium
                'Me.lblCIMGolfCourse.Text = .GolfCourseQuotedPremium
                'updated 10/11/2018 for multi-state
                Me.lblCIMBuildersRisk.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .BuildersRiskQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMComputer.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ComputerQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMContractorsEquipment.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ContractorsEquipmentScheduleQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMEquipmentLeasedRentedFromOthers.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ContractorsEquipmentLeasedRentedFromOthersQuotedPremium, maintainFormattingOrDefaultValue:=True)
                If qqHelper.IsZeroPremium(qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium, maintainFormattingOrDefaultValue:=True)) = False Then
                    Me.lblCIMUnscheduledTools.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Else
                    Me.lblCIMUnscheduledTools.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .SmallToolsQuotedPremium, maintainFormattingOrDefaultValue:=True)
                End If
                If qqHelper.HasAnyTruePropertyValues(SubQuotes, Function() .HasContractorsEnhancement) = True Then
                    Me.lblCIMEnhancementEndorsementText.Text = "INCLUDED" 'may eventually use premium
                Else
                    Me.lblCIMEnhancementEndorsementText.Text = ""
                End If
                Me.lblCIMFineArtsFloater.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .FineArtsQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMInstallationFloater.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .InstallationBlanketQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMMotorTruckCargo.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MotorTruckCargoScheduledVehicleQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMOwnersCargo.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .OwnersCargoAnyOneOwnedVehicleQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMScheduledPropertyFloater.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .ScheduledPropertyQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMSigns.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .SignsQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMTransportation.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .TransportationCatastropheQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMGolfCart.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GolfCartQuotedPremium, maintainFormattingOrDefaultValue:=True)
                Me.lblCIMGolfCourse.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .GolfCourseQuotedPremium, maintainFormattingOrDefaultValue:=True)

                'added 8/17/2015
                If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    'CIM packagePart for CPP lob
                    Me.CPP_header_Row.Visible = True
                    Me.Monoline_header_Row.Visible = False
                    Me.startBreak.Visible = False
                    Me.endBreak.Visible = False
                    _LinesInControl -= 2
                    Me.quoteNumberSection.Visible = False 'no longer needed since there are 2 separate header rows (since the font size varies between CPP and monoline)
                    'Me.lblTotalPremium.Text = .CPP_CIM_PackagePart_QuotedPremium
                    'updated 10/11/2018 for multi-state
                    Me.lblTotalPremium.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_CIM_PackagePart_QuotedPremium, maintainFormattingOrDefaultValue:=True)
                    Me.lblPremiumText.Text = "Inland Marine Total Premium"
                    Me.SpacerRow.Visible = True
                    'added 5/17/2017; no longer need calc below
                    'Me.lblCIMAmountToMeetMinimumAmount.Text = .CPP_MinPremAdj_CIM
                    'updated 10/11/2018 for multi-state
                    Me.lblCIMAmountToMeetMinimumAmount.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .CPP_MinPremAdj_CIM, maintainFormattingOrDefaultValue:=True)
                Else
                    'CIM lob
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
                    'Me.lblCIMAmountToMeetMinimumAmount.Text = .MinimumPremiumAdjustment
                    'updated 10/11/2018 for multi-state
                    Me.lblCIMAmountToMeetMinimumAmount.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() .MinimumPremiumAdjustment, maintainFormattingOrDefaultValue:=True)
                End If

                ''Dim totprem As Decimal = 0
                'Dim amtto100 As Decimal = 0
                'If qqHelper.IsNumericString(Me.lblTotalPremium.Text) = True Then
                '    'totprem = CDec(Me.lblTotalPremium.Text)
                '    Dim partsprem As Decimal = 0
                '    If IsNumeric(.BuildersRiskQuotedPremium) Then partsprem += CDec(.BuildersRiskQuotedPremium)
                '    If IsNumeric(.ComputerQuotedPremium) Then partsprem += CDec(.ComputerQuotedPremium)
                '    If IsNumeric(.ContractorsEquipmentScheduleQuotedPremium) Then partsprem += CDec(.ContractorsEquipmentScheduleQuotedPremium)
                '    If IsNumeric(.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium) Then partsprem += CDec(.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)
                '    If IsNumeric(.SmallToolsQuotedPremium) Then partsprem += CDec(.SmallToolsQuotedPremium)
                '    If IsNumeric(.FineArtsQuotedPremium) Then partsprem += CDec(.FineArtsQuotedPremium)
                '    If IsNumeric(.InstallationBlanketQuotedPremium) Then partsprem += CDec(.InstallationBlanketQuotedPremium)
                '    If IsNumeric(.MotorTruckCargoScheduledVehicleQuotedPremium) Then partsprem += CDec(.MotorTruckCargoScheduledVehicleQuotedPremium)
                '    If IsNumeric(.OwnersCargoAnyOneOwnedVehicleQuotedPremium) Then partsprem += CDec(.OwnersCargoAnyOneOwnedVehicleQuotedPremium)
                '    If IsNumeric(.ScheduledPropertyQuotedPremium) Then partsprem += CDec(.ScheduledPropertyQuotedPremium)
                '    If IsNumeric(.SignsQuotedPremium) Then partsprem += CDec(.SignsQuotedPremium)
                '    If IsNumeric(.TransportationCatastropheQuotedPremium) Then partsprem += CDec(.TransportationCatastropheQuotedPremium)

                '    If partsprem < 100 Then
                '        amtto100 = 100 - partsprem
                '        lblCIMAmountToMeetMinimumText.Text = Format("100", "c")
                '        lblCIMAmountToMeetMinimumAmount.Text = Format(amtto100, "c")
                '        'totprem = 100
                '    Else
                '        'trCIMAmountToMeetMinimumRow.Visible = False 'nothing needed here since it's being checked below
                '        '_LinesInControl -= 1
                '    End If
                'End If
                'removed calc logic 5/17/2017; now using props above; also correctly formatted minAmt since it was just showing c
                Me.lblCIMAmountToMeetMinimumText.Text = qqHelper.QuotedPremiumFormat("100")


                'added 5/13/2017 so Comments can show in Summary section if there isn't a control for the LOB
                Me.CommentsRow.Visible = False
                '5/15/2017 - removed logic below since we now have a LOB control w/ Comments
                'If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine Then 'as opposed to CPP, which already includes CIM packagePart w/ Comments
                '    Dim hasLobControl As Boolean = False
                '    Dim pHelper As New ProposalHelperClass
                '    pHelper.CheckForProposalLobControlAndAddToPlaceholder(_QuickQuote, hasLobControl:=hasLobControl, addToPlaceholder:=False)
                '    If hasLobControl = False Then
                '        Dim commentsLineCount As Integer = 0
                '        Me.lblComments.Text = qqHelper.HtmlForQuickQuoteProposalCommentsForLobType(.Comments, QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine, includeBreakAtBeginning:=True, lineCount:=commentsLineCount)
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

        Dim proposalPrems As String = "" 'added 5/16/2017

        If qqHelper.IsZeroPremium(Me.lblCIMBuildersRisk.Text) Then
            Me.trCIMBuildersRisk.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMBuildersRisk.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMComputer.Text) Then
            Me.trCIMComputer.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMComputer.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMContractorsEquipment.Text) Then
            Me.trCIMContractorsEquipment.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMContractorsEquipment.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMEquipmentLeasedRentedFromOthers.Text) Then
            Me.trCIMEquipmentLeasedRentedFromOthers.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMEquipmentLeasedRentedFromOthers.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMUnscheduledTools.Text) Then
            Me.trCIMUnscheduledTools.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMUnscheduledTools.Text)
        End If
        If UCase(Me.lblCIMEnhancementEndorsementText.Text) <> "INCLUDED" AndAlso qqHelper.IsZeroPremium(Me.lblCIMEnhancementEndorsementText.Text) Then
            Me.trCIMEnhancementEndorsement.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMEnhancementEndorsementText.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMFineArtsFloater.Text) Then
            Me.trCIMFineArtsFloater.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMFineArtsFloater.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMInstallationFloater.Text) Then
            Me.trCIMInstallationFloater.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMInstallationFloater.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMMotorTruckCargo.Text) Then
            Me.trCIMMotorTruckCargo.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMMotorTruckCargo.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMOwnersCargo.Text) Then
            Me.trCIMOwnersCargo.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMOwnersCargo.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMScheduledPropertyFloater.Text) Then
            Me.trCIMSchedulePropertyFloater.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMScheduledPropertyFloater.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMSigns.Text) Then
            Me.trCIMSigns.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMSigns.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMTransportation.Text) Then
            Me.trCIMTransportation.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMTransportation.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMGolfCart.Text) Then 'added 5/15/2017
            Me.trCIMGolfCart.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMGolfCart.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMGolfCourse.Text) Then 'added 5/15/2017
            Me.trCIMGolfCourse.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMGolfCourse.Text)
        End If
        If qqHelper.IsZeroPremium(Me.lblCIMAmountToMeetMinimumAmount.Text) Then
            Me.trCIMAmountToMeetMinimumRow.Visible = False
            _LinesInControl -= 1
        Else 'added 5/16/2017
            proposalPrems = qqHelper.getSum(proposalPrems, Me.lblCIMAmountToMeetMinimumAmount.Text)
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
