Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteObject

Public Class RequestRate
    Public Shared Sub RateWasRequested(ByRef quote As QuickQuoteObject, ByRef requesterMe As VRMasterControlBase, ctlTreeView As ctlTreeView, ValidationSummary As ctlValidationSummary, Optional summaryControl As VRControlBase = Nothing, Optional locationListControl As VRControlBase = Nothing)
        Dim QQHelper As New QuickQuoteHelperClass
        If ValidationSummary.HasErrors() = False Then
            'good to rate
            ' do rate
            Dim saveErr As String = Nothing
            Dim loadErr As String = Nothing
            Dim IsAppRating As Boolean
            Dim preRateCompanyId As Integer = 1
            Dim postRateCompanyId As Integer = 1

            requesterMe.Session("ShowNewCoLockMessage") = ""

            'Check if we are App rating
            IsAppRating = requesterMe.IsOnAppPage


            If Not IsAppRating Then
                If quote IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(quote.Database_DiaCompanyId) Then
                    preRateCompanyId = CInt(quote.Database_DiaCompanyId)
                End If
            End If

            If quote.LobType = QuickQuoteLobType.CommercialBOP Then
                'check for AIs that won't pass validation
                IFM.VR.Common.Helpers.AdditionalInterest.RemoveIncompleteAIs_BOP_CAP(quote)
            End If

            'updated 2/18/2019
            Dim ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If String.IsNullOrWhiteSpace(requesterMe.ReadOnlyPolicyIdAndImageNum) = False Then
                'no rate
            ElseIf String.IsNullOrWhiteSpace(requesterMe.EndorsementPolicyIdAndImageNum) = False Then
                Dim successfulEndorsementRate As Boolean = Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedEndorsementQuoteFromContext(requesterMe.EndorsementPolicyId, requesterMe.EndorsementPolicyImageNum, qqEndorsementResults:=ratedQuote, errorMessage:=saveErr)
            Else
                If IsAppRating Then
                    ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(requesterMe.QuoteId, saveErr, loadErr, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                Else
                    ratedQuote = Common.QuoteSave.QuoteSaveHelpers.SaveAndRate(requesterMe.QuoteId, saveErr, loadErr)
                End If

            End If

            ' Check for quote stop or kill - DM 8/30/2017
            If quote IsNot Nothing AndAlso (quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse quote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled) Then
                IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(quote, requesterMe.Page, requesterMe.Response, requesterMe.Session)
            End If

            If IsAppRating Then
                'IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
                'updated 2/18/2019
                If String.IsNullOrWhiteSpace(requesterMe.ReadOnlyPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(requesterMe.ReadOnlyPolicyId, requesterMe.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                ElseIf String.IsNullOrWhiteSpace(requesterMe.EndorsementPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(requesterMe.EndorsementPolicyId, requesterMe.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                Else
                    Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(requesterMe.QuoteId, saveType:=QuickQuoteXML.QuickQuoteSaveType.AppGap)
                End If
            ElseIf quote.LobType = QuickQuoteLobType.CommercialBOP Then
                If String.IsNullOrWhiteSpace(requesterMe.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(requesterMe.ReadOnlyPolicyId, requesterMe.ReadOnlyPolicyImageNum)
                ElseIf String.IsNullOrWhiteSpace(requesterMe.EndorsementPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(requesterMe.EndorsementPolicyId, requesterMe.EndorsementPolicyImageNum)
                Else
                    IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(requesterMe.QuoteId)
                End If

                If TypeOf locationListControl Is ctl_BOP_LocationList Then
                    CType(locationListControl, ctl_BOP_LocationList).Populate()
                End If
            End If

            ' set this per page life cycle cache with newest - 6-3-14
            If ratedQuote IsNot Nothing Then

                'Turning this off 02/12/2025 - When other lines of business are brought into the new company, this code will need to be reinstated for that LOB only.
                'If Not IsAppRating AndAlso QQHelper.IsPositiveIntegerString(ratedQuote.CompanyId) Then
                '    postRateCompanyId = CInt(ratedQuote.CompanyId)
                '    If postRateCompanyId > preRateCompanyId Then
                '        requesterMe.Session("ShowNewCoLockMessage") = True
                '    End If
                'End If

                DirectCast(requesterMe.Page.Master, VelociRater).GetRatedQuotefromCache(False, ratedQuote) 'sets the rated quote cache
            Else
                ' you can't set a Nothing quote with this method you'll just have to let it find out for itself that the last rated quote was nothing - should never happen
                DirectCast(requesterMe.Page.Master, VelociRater).GetRatedQuotefromCache(True)
            End If

            If String.IsNullOrWhiteSpace(saveErr) = False Or String.IsNullOrWhiteSpace(loadErr) = False Then
                'failed
                If String.IsNullOrWhiteSpace(saveErr) = False Then
                    requesterMe.ValidationHelper.AddError(saveErr)
                End If
                If String.IsNullOrWhiteSpace(loadErr) = False Then
                    requesterMe.ValidationHelper.AddError(loadErr)
                End If

            Else
                ' did not fail to call service but may have validation Items
                If ratedQuote IsNot Nothing Then
                    WebHelper_Personal.GatherRatingErrorsAndWarnings(ratedQuote, requesterMe.ValidationHelper)
                    If ratedQuote.Success Then
                        ''Me.ctl_CPP_QuoteSummary.Populate()

                        If ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse ratedQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                            'stay where you are - don't show summary - stop message will be contained in validation messages
                        Else
                            If TypeOf summaryControl Is ctl_CGL_QuoteSummary Then
                                CType(summaryControl, ctl_CGL_QuoteSummary).Populate()
                            End If
                            If TypeOf summaryControl Is ctl_BOP_QuoteSummary Then
                                CType(summaryControl, ctl_BOP_QuoteSummary).Populate()
                            End If
                            requesterMe.SetCurrentWorkFlow(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "")
                            ctlTreeView.RefreshRatedQuote()
                        End If
                    Else
                        'stay where you are - probably coverages
                    End If

                End If
            End If
        End If
    End Sub

End Class
