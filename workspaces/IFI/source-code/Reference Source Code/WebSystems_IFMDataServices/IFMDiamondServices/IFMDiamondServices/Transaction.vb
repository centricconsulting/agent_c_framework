Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Transaction
    Public Module Transaction
        Public Function AdjustExpDateToMatchOriginalInceptionDate(EffDate As Date,
                                                                  ExpDate As Date,
                                                                  FirstEffDate As Date,
                                                                  GuaranteedExpDate As Date,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.TransactionService.AdjustExpDateToMatchOriginalInceptionDate.ResponseData
            Dim req As New DCSM.TransactionService.AdjustExpDateToMatchOriginalInceptionDate.Request
            Dim res As New DCSM.TransactionService.AdjustExpDateToMatchOriginalInceptionDate.Response

            With req.RequestData
                .EffDate = EffDate
                .ExpDate = ExpDate
                .FirstEffDate = FirstEffDate
                .GuaranteedExpDate = GuaranteedExpDate
            End With

            If (IFMS.Transaction.AdjustExpDateToMatchOriginalInceptionDate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function CancelStatusSearchResults(Agencycode As String,
                                                  AgencyId As Integer,
                                                  CompanyId As Integer,
                                                  EndDate As Date,
                                                  IsCancelled As Boolean,
                                                  LobId As Integer,
                                                  StartDate As Date,
                                                  StateId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Transactions.CancelStatus)
            Dim req As New DCSM.TransactionService.CancelStatusSearchResults.Request
            Dim res As New DCSM.TransactionService.CancelStatusSearchResults.Response

            With req.RequestData
                .AgencyCode = Agencycode
                .AgencyId = AgencyId
                .CompanyId = CompanyId
                .EndDate = EndDate
                .IsCancelled = IsCancelled
                .LobId = LobId
                .StartDate = StartDate
                .StateId = StateId
            End With

            If (IFMS.Transaction.CancelStatusSearchResults(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.CancelStatusCollection
                End If
            End If

            Return Nothing
        End Function

        Public Function DoesPolicyImageStillExist(PolicyId As Integer,
                                                  PolicyImageId As Integer,
                                                  PolicyImageNum As Integer,
                                                  PolicyStatusCodeId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.TransactionService.DoesPolicyImageStillExist.Request
            Dim res As New DCSM.TransactionService.DoesPolicyImageStillExist.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageId = PolicyImageId
                .PolicyImageNum = PolicyImageNum
                .PolicyStatusCodeId = PolicyStatusCodeId
            End With

            If (IFMS.Transaction.DoesPolicyImageStillExist(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyStatusCodeId
                End If
            End If

            Return Nothing
        End Function

        Public Function GetFirstNonProRataEffDate(PolicyId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Date
            Dim req As New DCSM.TransactionService.GetFirstNonProRataEffDate.Request
            Dim res As New DCSM.TransactionService.GetFirstNonProRataEffDate.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Transaction.GetFirstNonProRataEffDate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.FirstEffDate
                End If
            End If

            Return Nothing
        End Function

        Public Function GetOutOfSeqInfo(IsAutomaticTrans As Boolean,
                                        IsOutOfSeq As Boolean,
                                        PolicyId As Integer,
                                        STI As DCO.Transactions.SubmitTransInfo,
                                        TEffDate As Date,
                                        TEffTime As Date,
                                        TransTypeId As Integer,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.TransactionService.GetOutOfSeqInfo.ResponseData
            Dim req As New DCSM.TransactionService.GetOutOfSeqInfo.Request
            Dim res As New DCSM.TransactionService.GetOutOfSeqInfo.Response

            With req.RequestData
                .IsAutomaticTrans = IsAutomaticTrans
                .IsOutOfSeq = IsOutOfSeq
                .PolicyId = PolicyId
                .STI = STI
                .TEffDate = TEffDate
                .TEffTime = TEffTime
                .TransTypeId = TransTypeId
            End With

            If (IFMS.Transaction.GetOutOfSeqInfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSubmitAppInfo(Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Transactions.SubmitAppInfo
            Dim req As New DCSM.TransactionService.GetSubmitAppInfo.Request
            Dim res As New DCSM.TransactionService.GetSubmitAppInfo.Response

            With req.RequestData
            End With

            If (IFMS.Transaction.GetSubmitAppInfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.SubmitAppInto
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSubmitTransInfo(IsQuote As Boolean,
                                           PolicyId As Integer,
                                           PolicyNo As String,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Transactions.SubmitTransInfoStatic
            Dim req As New DCSM.TransactionService.GetSubmitTransInfo.Request
            Dim res As New DCSM.TransactionService.GetSubmitTransInfo.Response

            With req.RequestData
                .IsQuote = IsQuote
                .PolicyId = PolicyId
                .PolicyNo = PolicyNo
            End With

            If (IFMS.Transaction.GetSubmitTransInfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.STIS
                End If
            End If

            Return Nothing
        End Function

        Public Function IsNewExpDateValid(Errors As DCO.DiamondValidation,
                                          PolicyId As Integer,
                                          PolicyImageNum As Integer,
                                          TransactionExpirationDate As Date,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.IsNewExpDateValid.Request
            Dim res As New DCSM.TransactionService.IsNewExpDateValid.Response

            With req.RequestData
                .Errors = Errors
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .TransactionExpirationDate = TransactionExpirationDate
            End With

            If (IFMS.Transaction.IsNewExpDateValid(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function IsTransactionOutOfSequence(PolicyId As Integer,
                                                   PolicyImageNumber As Integer,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.IsTransactionOutOfSequence.Request
            Dim res As New DCSM.TransactionService.IsTransactionOutOfSequence.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNumber = PolicyImageNumber
            End With

            If (IFMS.Transaction.IsTransactionOutOfSequence(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Result
                End If
            End If

            Return Nothing
        End Function

        Public Function NewTEffDateWillResultInVersionChange(PolicyId As Integer,
                                                             PolicyImageNum As Integer,
                                                             TransactionEffectiveDate As Date,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.NewTEffDateWillResultInVersionChange.Request
            Dim res As New DCSM.TransactionService.NewTEffDateWillResultInVersionChange.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .TransactionEffectiveDate = TransactionEffectiveDate
            End With

            If (IFMS.Transaction.NewTEffDateWillResultInVersionChange(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessAutomaticNonRenewals(Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.ProcessAutomaticNonRenewals.Request
            Dim res As New DCSM.TransactionService.ProcessAutomaticNonRenewals.Response

            With req.RequestData
            End With

            If (IFMS.Transaction.ProcessAutomaticNonRenewals(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessCompanySpecificWarningsAtSubmission(STI As DCO.Transactions.SubmitTransInfo,
                                                                   Optional ByRef e As Exception = Nothing,
                                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.TransactionService.ProcessCompanySpecificWarningsAtSubmission.Request
            Dim res As New DCSM.TransactionService.ProcessCompanySpecificWarningsAtSubmission.Response

            With req.RequestData
                .STI = STI
            End With

            IFMS.Transaction.ProcessCompanySpecificWarningsAtSubmission(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ProcessPolicyPurge(IsAutomaticProcess As Boolean,
                                           PurgeDate As Date,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.ProcessPolicyPurge.Request
            Dim res As New DCSM.TransactionService.ProcessPolicyPurge.Response

            With req.RequestData
                .IsAutomaticProcess = IsAutomaticProcess
                .PurgeDate = PurgeDate
            End With

            If (IFMS.Transaction.ProcessPolicyPurge(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessQuotePurge(IsAutomaticProcess As Boolean,
                                          PurgeDate As Date,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.ProcessQuotePurge.Request
            Dim res As New DCSM.TransactionService.ProcessQuotePurge.Response

            With req.RequestData
                .IsAutomaticProcess = IsAutomaticProcess
                .PurgeDate = PurgeDate
            End With

            If (IFMS.Transaction.ProcessQuotePurge(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessSpecificDateABT(ABTDate As Date,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.TransactionService.ProcessSpecificDateABT.Request
            Dim res As New DCSM.TransactionService.ProcessSpecificDateABT.Response

            With req.RequestData
                .ABTDate = ABTDate
            End With

            IFMS.Transaction.ProcessSpecificDateABT(res, req, e, dv)

            Return Nothing
        End Function

        Public Function RecalculateWrittenPremium(PolicyId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.RecalculateWrittenPremium.Request
            Dim res As New DCSM.TransactionService.RecalculateWrittenPremium.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Transaction.RecalculateWrittenPremium(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function RecalculateWrittenPremiumForAllPolicies(Optional ByRef e As Exception = Nothing,
                                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.RecalculateWrittenPremiumForAllPolicies.Request
            Dim res As New DCSM.TransactionService.RecalculateWrittenPremiumForAllPolicies.Response

            With req.RequestData
            End With

            If (IFMS.Transaction.RecalculateWrittenPremiumForAllPolicies(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function ReferredByChangeSave(PolicyImage As DCO.Policy.Image,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.TransactionService.ReferredByChangeSave.ResponseData
            Dim req As New DCSM.TransactionService.ReferredByChangeSave.Request
            Dim res As New DCSM.TransactionService.ReferredByChangeSave.Response

            With req.RequestData
                .PolicyImage = PolicyImage
            End With

            If (IFMS.Transaction.ReferredByChangeSave(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function ReferredByChangeWillResultInVersionChange(PolicyImage As DCO.Policy.Image,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.TransactionService.ReferredByChangeWillResultInVersionChange.ResponseData
            Dim req As New DCSM.TransactionService.ReferredByChangeWillResultInVersionChange.Request
            Dim res As New DCSM.TransactionService.ReferredByChangeWillResultInVersionChange.Response

            With req.RequestData
                .PolicyImage = PolicyImage
            End With

            If (IFMS.Transaction.ReferredByChangeWillResultInVersionChange(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function ShouldAskToAllowImageToBeDeleted(PolicyId As Integer,
                                                         PolicyImageNum As Integer,
                                                         TransTypeId As Integer,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.DiamondValidation
            Dim req As New DCSM.TransactionService.ShouldAskToAllowImageToBeDeleted.Request
            Dim res As New DCSM.TransactionService.ShouldAskToAllowImageToBeDeleted.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .TransTypeId = TransTypeId
            End With

            If (IFMS.Transaction.ShouldAskToAllowImageToBeDeleted(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DiamondValidation
                End If
            End If

            Return Nothing
        End Function

        Public Function ShouldReferredByBeEnabled(PolicyImage As DCO.Policy.Image,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.ShouldReferredByBeEnabled.Request
            Dim res As New DCSM.TransactionService.ShouldReferredByBeEnabled.Response

            With req.RequestData
                .PolicyImage = PolicyImage
            End With

            If (IFMS.Transaction.ShouldReferredByBeEnabled(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="TransEffDate"></param>
        ''' <param name="TransReasonID"></param>
        ''' <param name="Remark"></param>
        ''' <param name="TransSourceID"></param>
        ''' <param name="TransTypeID"></param>
        ''' <param name="TransUserID"></param>
        ''' <param name="ReturnImage"></param>
        ''' <param name="RequestDate"></param>
        ''' <param name="IsQuote"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks>Tested on QA with Success. Not tested since move to this project</remarks>
        Public Function SubmitTransaction(PolicyID As Integer, PolicyImageNum As Integer, TransEffDate As Date,
                                          TransReasonID As Integer,
                                          Remark As String,
                                          TransSourceID As DCE.TransSource,
                                          TransTypeID As DCE.TransType,
                                          TransUserID As Integer,
                                          Optional ByRef ReturnImage As DCO.Policy.Image = Nothing,
                                          Optional ByVal RequestDate As Date = Nothing,
                                          Optional IsQuote As Boolean = True,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Return SubmitTransaction(New Models.Transaction With {
                .PolicyID = PolicyID, .PolicyImageNum = PolicyImageNum, .TransactionEffectiveDate = TransEffDate,
                .TransactionReasonID = TransReasonID, .TransactionSourceID = TransSourceID, .TransactionTypeID = TransTypeID,
                .TransactionUserID = TransUserID, .Remark = Remark},
                ReturnImage, RequestDate, IsQuote, e)

        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trans"></param>
        ''' <param name="ReturnImage"></param>
        ''' <param name="RequestDate"></param>
        ''' <param name="IsQuote"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubmitTransaction(trans As Models.Transaction,
                                          Optional ByRef ReturnImage As DCO.Policy.Image = Nothing,
                                          Optional ByVal RequestDate As Date = Nothing,
                                          Optional IsQuote As Boolean = True,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean

            Dim img As DCO.Policy.Image = Nothing
            Dim Req As New DCS.Messages.PolicyService.LoadImage.Request
            Dim Resp As New DCS.Messages.PolicyService.LoadImage.Response
            Req.RequestData.PolicyId = trans.PolicyID
            Req.RequestData.ImageNumber = trans.PolicyImageNum
            If Diamond.Policy.LoadImage(Resp, Req) Then
                If Resp.ResponseData.Image IsNot Nothing Then
                    img = Resp.ResponseData.Image
                End If
            End If

            If img IsNot Nothing Then
                With img
                    .TransactionEffectiveDate = trans.TransactionEffectiveDate
                    .TransactionReasonId = trans.TransactionReasonID
                    .TransactionRemark = trans.Remark
                    .TransactionSourceId = trans.TransactionSourceID
                    .TransactionTypeId = trans.TransactionTypeID
                    .TransactionUsersId = trans.TransactionUserID
                End With
                Return SubmitTransaction(img, ReturnImage, RequestDate, IsQuote, e)
            End If
            Return False
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="img"></param>
        ''' <param name="ReturnImage"></param>
        ''' <param name="RequestDate"></param>
        ''' <param name="IsQuote"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SubmitTransaction(img As DCO.Policy.Image,
                                           Optional ByRef ReturnImage As DCO.Policy.Image = Nothing,
                                           Optional ByVal RequestDate As Date = Nothing,
                                           Optional IsQuote As Boolean = True,
                                           Optional ByRef e As System.Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.TransactionService.SubmitTransaction.Request
            Dim res As New DCSM.TransactionService.SubmitTransaction.Response
            With req.RequestData
                If RequestDate = Nothing Then
                    RequestDate = Date.Now
                End If
                .DateReceived = RequestDate
                .IsQuote = IsQuote
                .PolicyId = img.PolicyId
                .PolicyImageNumber = img.PolicyImageNum
                .ReturnImageInResponse = True
                .TransactionEffectiveDate = img.TransactionEffectiveDate
                .TransactionReasonId = img.TransactionReasonId
                .TransactionRemarks = img.TransactionRemark
                .TransactionSourceId = img.TransactionSourceId
                .TransactionTypeId = img.TransactionTypeId
                .TransactionUserId = img.TransactionUsersId
            End With
            If IFMS.Transaction.SubmitTransaction(res, req, e) Then
                If res.ResponseData.PolicyImage IsNot Nothing Then
                    ReturnImage = res.ResponseData.PolicyImage
                End If
                Return True
            End If
            Return False
        End Function

        Public Function SynchronizePackagePolicy(PDetailSyncVersionId As Integer,
                                                 PolicyImage As DCO.Policy.Image,
                                                 UpdatedPackagePart As DCO.Policy.PackagePart,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Image
            Dim req As New DCSM.TransactionService.SynchronizePackagePolicy.Request
            Dim res As New DCSM.TransactionService.SynchronizePackagePolicy.Response

            With req.RequestData
                .PDetailSyncVersionId = PDetailSyncVersionId
                .PolicyImage = PolicyImage
                .UpdatedPackagePart = UpdatedPackagePart
            End With

            If (IFMS.Transaction.SynchronizePackagePolicy(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyImage
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace

