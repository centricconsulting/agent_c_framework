Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Linq

Public Class ctl_OrderClueAndOrMVR
    Inherits VRControlBase

    Public Event JustFinishedCreditOrder() 'added 9/27/2019

    Public Enum ReportRequestType
        notSpecified = 0
        clue = 1
        mvr = 2
        clueandmv = 3
        credit = 4
    End Enum

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
    End Sub

    ''' <summary>
    ''' Loads the specified report and saves the results to the quote.
    ''' </summary>
    ''' <param name="reporttype"></param>
    ''' <remarks></remarks>
    Public Sub LoadReport(reporttype As ReportRequestType)

        Dim loadMVRAndClue_ForAuto As New Action(Sub()
                                                     Dim qqXml As New QuickQuoteXML()
                                                     Dim mvr As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                                     Dim clue As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                                     qqXml.LoadMvrAndClueAutoForQuote(Me.Quote, mvr, clue)
                                                 End Sub)

        Dim loadMVRA_ForAuto As New Action(Sub()
                                               Dim qqXml As New QuickQuoteXML()
                                               Dim mvr As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                               qqXml.LoadMvrForQuote(Me.Quote, mvr)
                                           End Sub)

        Dim loadClue_ForAuto As New Action(Sub()
                                               Dim qqXml As New QuickQuoteXML()
                                               Dim clue As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                               qqXml.LoadClueAutoForQuote(Me.Quote, clue)
                                           End Sub)

        Dim loadClue_ForProperty As New Action(Sub()

                                                   Dim qqXml As New QuickQuoteXML()
                                                   Dim clue As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                                   qqXml.LoadCluePropertyForQuote(Me.Quote, clue)
                                               End Sub)

        Dim loadCredit As New Action(Sub()

                                         Dim qqXml As New QuickQuoteXML()
                                         Dim credit As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
                                         Dim results As String = ""
                                         Dim errorMsg As String = ""
                                         qqXml.LoadCreditForQuote(Me.Quote, credit, results, errorMsg)
                                         RaiseEvent JustFinishedCreditOrder() 'added 9/27/2019

                                         If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                                             VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.Quote)
                                         ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                                             VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.Quote)
                                         Else
                                             IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId, QuickQuoteXML.QuickQuoteSaveType.Quote)
                                         End If

                                     End Sub)

        Dim okayToOrderReport As Boolean = True
        Dim reportOrdered As Boolean = False
        Dim isRated As Boolean = False
        Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
        If ratedQuote IsNot Nothing Then
            isRated = ratedQuote.Success
        End If

        If isRated = False Then
            ' do Report Lookup
            If Me.Quote IsNot Nothing Then
                Select Case Me.Quote.LobId
                    Case "1"
                        Select Case reporttype
                            Case ReportRequestType.mvr
                                loadMVRA_ForAuto()
                                reportOrdered = True
                            Case ReportRequestType.clue
                                loadClue_ForAuto()
                                reportOrdered = True
                            Case ReportRequestType.clueandmv, ReportRequestType.notSpecified
                                loadMVRAndClue_ForAuto()
                                reportOrdered = True
                        End Select
                    Case "2"
                        Select Case reporttype
                            Case ReportRequestType.clue, ReportRequestType.notSpecified
                                loadClue_ForProperty()
                                reportOrdered = True
                            Case ReportRequestType.credit
                                'Updated 9/4/18 for multi state MLW
                                If Me.GoverningStateQuote IsNot Nothing Then
                                    'If Me.Quote.Applicants IsNot Nothing AndAlso Me.Quote.Applicants.Count > 0 Then
                                    If Me.GoverningStateQuote.Applicants IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.Count > 0 Then
                                        If Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                            ' okayToOrderReport = QQHelper.DiamondImageOrQuickQuoteObjectHasNewApplicants(Nothing, Me.GoverningStateQuote) '6/3/2020 - DJG - Bug 47573 - Business didn't want to order credit on endorsements unless a new applicant was added during the endorsement process.
                                            okayToOrderReport = False '6/8/2020 - DJG - Bug 47573 - Business decided to not order credit at this point anymore for endorsements. We have logic to occur at rate which would pick up any new policyholders added for potential rate changes.
                                        End If

                                        If okayToOrderReport = True Then
                                            loadCredit()
                                            reportOrdered = True
                                        End If
                                    End If
                                End If
                        End Select
                    Case "17" ' Farm Matt A 8-6-15
                        Select Case reporttype
                            Case ReportRequestType.clue, ReportRequestType.notSpecified
                                loadClue_ForProperty()
                                reportOrdered = True
                            Case ReportRequestType.credit
                                'Updated 9/4/18 for multi state MLW
                                If Me.GoverningStateQuote IsNot Nothing Then
                                    If Me.GoverningStateQuote.Applicants IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.Count > 0 Then
                                        If Me.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                            okayToOrderReport = False 'Business decided to not order credit at this point anymore for endorsements. We have logic to occur at rate which would pick up any new policyholders added for potential rate changes.
                                        End If

                                        If okayToOrderReport = True Then
                                            loadCredit()
                                            reportOrdered = True
                                        End If
                                        'loadCredit()
                                        'reportOrdered = True
                                    End If
                                End If
                        End Select
                    Case "3" ' DFR Matt A 10-22-15
                        Select Case reporttype
                            Case ReportRequestType.clue, ReportRequestType.notSpecified
                                loadClue_ForProperty()
                                reportOrdered = True
                            Case ReportRequestType.credit
#If DEBUG Then
                                Debugger.Break() 'credit not allowed for DFR
#End If
                        End Select

                    Case Else
#If DEBUG Then
                        Debugger.Break() 'set a case above for this LOB
#End If
                End Select

            End If

            If reportOrdered = True Then
                Fire_GenericBoardcastEvent(BroadCastEventType.ThirdPartyReportOrdered) ' so treeview gets the newest quote info
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Return False
    End Function

    Private Sub ctlHomeQuote_BroadcastGenericEvent(type As BroadCastEventType) Handles Me.BroadcastGenericEvent
        Select Case type
            Case BroadCastEventType.DoHOMCreditRequest
                'do credit check  --- this will inform the treeview of any changes
                LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.credit)
        End Select
    End Sub

End Class