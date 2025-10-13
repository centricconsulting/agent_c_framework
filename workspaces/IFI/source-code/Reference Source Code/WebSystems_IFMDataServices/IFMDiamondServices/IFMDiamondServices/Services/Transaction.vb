Imports Microsoft.VisualBasic
Imports IFM.DiamondServices.Models
Imports IDS = IFM.DiamondServices.Services.Diamond
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCST = Diamond.Common.Services.Messages.TransactionService
Imports DCSP = Diamond.Common.Services.Proxies.Transaction
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.Transaction

    Public Module Transaction
        Public Function AdjustExpDateToMatchOriginalInceptionDate(ByRef res As DCST.AdjustExpDateToMatchOriginalInceptionDate.Response,
                                                                  ByRef req As DCST.AdjustExpDateToMatchOriginalInceptionDate.Request,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AdjustExpDateToMatchOriginalInceptionDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CancelStatusSearchResults(ByRef res As DCST.CancelStatusSearchResults.Response,
                                                  ByRef req As DCST.CancelStatusSearchResults.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CancelStatusSearchResults
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DoesPolicyImageStillExist(ByRef res As DCST.DoesPolicyImageStillExist.Response,
                                                  ByRef req As DCST.DoesPolicyImageStillExist.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DoesPolicyImageStillExist
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetFirstNonProRataEffDate(ByRef res As DCST.GetFirstNonProRataEffDate.Response,
                                                  ByRef req As DCST.GetFirstNonProRataEffDate.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetFirstNonProRataEffDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetOutOfSeqInfo(ByRef res As DCST.GetOutOfSeqInfo.Response,
                                        ByRef req As DCST.GetOutOfSeqInfo.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetOutofSeqInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSubmitAppInfo(ByRef res As DCST.GetSubmitAppInfo.Response,
                                         ByRef req As DCST.GetSubmitAppInfo.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetSubmitAppInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSubmitTransInfo(ByRef res As DCST.GetSubmitTransInfo.Response,
                                           ByRef req As DCST.GetSubmitTransInfo.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetSubmitTransInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsNewExpDateValid(ByRef res As DCST.IsNewExpDateValid.Response,
                                          ByRef req As DCST.IsNewExpDateValid.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsNewExpDateValid
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsTransactionOutOfSequence(ByRef res As DCST.IsTransactionOutOfSequence.Response,
                                                   ByRef req As DCST.IsTransactionOutOfSequence.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsTransactionOutOfSequence
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function NewTEffDateWillResultInVersionChange(ByRef res As DCST.NewTEffDateWillResultInVersionChange.Response,
                                                             ByRef req As DCST.NewTEffDateWillResultInVersionChange.Request,
                                                             Optional ByRef e As Exception = Nothing,
                                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.NewTEffDateWillResultInVersionChange
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessAutomaticNonRenewals(ByRef res As DCST.ProcessAutomaticNonRenewals.Response,
                                                                  ByRef req As DCST.ProcessAutomaticNonRenewals.Request,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessAutomaticNonRenewals
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessCompanySpecificWarningsAtSubmission(ByRef res As DCST.ProcessCompanySpecificWarningsAtSubmission.Response,
                                                                   ByRef req As DCST.ProcessCompanySpecificWarningsAtSubmission.Request,
                                                                   Optional ByRef e As Exception = Nothing,
                                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessCompanySpecificWarningsAtSubmission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessPolicyPurge(ByRef res As DCST.ProcessPolicyPurge.Response,
                                           ByRef req As DCST.ProcessPolicyPurge.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessPolicyPurge
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessQuotePurge(ByRef res As DCST.ProcessQuotePurge.Response,
                                          ByRef req As DCST.ProcessQuotePurge.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessQuotePurge
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessSpecificDateABT(ByRef res As DCST.ProcessSpecificDateABT.Response,
                                               ByRef req As DCST.ProcessSpecificDateABT.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessSpecificDateABT
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RecalculateWrittenPremium(ByRef res As DCST.RecalculateWrittenPremium.Response,
                                                  ByRef req As DCST.RecalculateWrittenPremium.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RecalculateWrittenPremium
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RecalculateWrittenPremiumForAllPolicies(ByRef res As DCST.RecalculateWrittenPremiumForAllPolicies.Response,
                                                                ByRef req As DCST.RecalculateWrittenPremiumForAllPolicies.Request,
                                                                Optional ByRef e As Exception = Nothing,
                                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RecalculateWrittenPremiumForAllPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReferredByChangeSave(ByRef res As DCST.ReferredByChangeSave.Response,
                                             ByRef req As DCST.ReferredByChangeSave.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReferredByChangeSave
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReferredByChangeWillResultInVersionChange(ByRef res As DCST.ReferredByChangeWillResultInVersionChange.Response,
                                                                  ByRef req As DCST.ReferredByChangeWillResultInVersionChange.Request,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReferredByChangeWillResultInVersionChange
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ShouldAskToAllowImageToBeDeleted(ByRef res As DCST.ShouldAskToAllowImageToBeDeleted.Response,
                                                         ByRef req As DCST.ShouldAskToAllowImageToBeDeleted.Request,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ShouldAskToAllowImageToBeDeleted
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ShouldReferredByBeEnabled(ByRef res As DCST.ShouldReferredByBeEnabled.Response,
                                                  ByRef req As DCST.ShouldReferredByBeEnabled.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ShouldReferredByBeEnabled
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SubmitTransaction(ByRef res As DCST.SubmitTransaction.Response,
                                          ByRef req As DCST.SubmitTransaction.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SubmitTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SynchronizePackagePolicy(ByRef res As DCST.SynchronizePackagePolicy.Response,
                                                 ByRef req As DCST.SynchronizePackagePolicy.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.TransactionServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SynchronizePackagePolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

