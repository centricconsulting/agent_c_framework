Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCPS = Diamond.Common.Services.Messages.PolicyService
Imports DCSP = Diamond.Common.Services.Proxies.PolicyServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.Policy
    Public Module Policy
        Public Function AcceptRejectQuote(ByRef res As DSCPS.AcceptRejectQuote.Response,
                                          ByRef req As DSCPS.AcceptRejectQuote.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.AcceptRejectQuote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function AcquirePendingImage(ByRef res As DSCPS.AcquirePendingImage.Response,
                                            ByRef req As DSCPS.AcquirePendingImage.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.AcquirePendingImage
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ApplyExperienceModificationFactor(ByRef res As DSCPS.ApplyExperienceModificationFactor.Response,
                                                          ByRef req As DSCPS.ApplyExperienceModificationFactor.Request,
                                                          Optional ByRef e As Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ApplyExperienceModificationFactor
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function AttachForms(ByRef res As DSCPS.AttachForms.Response,
                                    ByRef req As DSCPS.AttachForms.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.AttachForms
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Binder(ByRef res As DSCPS.Binder.Response,
                               ByRef req As DSCPS.Binder.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Binder
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ConvertPolicyForRenewal(ByRef res As DSCPS.ConvertPolicyForRenewal.Response,
                                                ByRef req As DSCPS.ConvertPolicyForRenewal.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ConvertPolicyForRenewal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ConvertPolicyFromQuoteToApplication(ByRef res As DSCPS.ConvertPolicyFromQuoteToApplication.Response,
                                                            ByRef req As DSCPS.ConvertPolicyFromQuoteToApplication.Request,
                                                            Optional ByRef e As Exception = Nothing,
                                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ConvertPolicyFromQuoteToApplication
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CopyQuoteToNewQuote(ByRef res As DSCPS.CopyQuoteToNewQuote.Response,
                                            ByRef req As DSCPS.CopyQuoteToNewQuote.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CopyQuoteToNewQuote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CoverageDeleteWarning(ByRef res As DSCPS.CoverageDeleteWarning.Response,
                                              ByRef req As DSCPS.CoverageDeleteWarning.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CoverageDeleteWarning
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CreateNewPackageParts(ByRef res As DSCPS.CreateNewPackageParts.Response,
                                              ByRef req As DSCPS.CreateNewPackageParts.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateNewPackageParts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DataFill(ByRef res As DSCPS.DataFill.Response,
                                 ByRef req As DSCPS.DataFill.Request,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DataFill
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DefaultFirstLocation(ByRef res As DSCPS.DefaultFirstLocation.Response,
                                             ByRef req As DSCPS.DefaultFirstLocation.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DefaultFirstLocation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DefaultSubmitInfo(ByRef res As DSCPS.DefaultSubmitInfo.Response,
                                          ByRef req As DSCPS.DefaultSubmitInfo.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DefaultSubmitInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteClient(ByRef res As DSCPS.DeleteClient.Response,
                                     ByRef req As DSCPS.DeleteClient.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteFutureEvents(ByRef res As DSCPS.DeleteFutureEvents.Response,
                                           ByRef req As DSCPS.DeleteFutureEvents.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteFutureEvents
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteOutsideAuditor(ByRef res As DSCPS.DeleteOutsideAuditor.Response,
                                             ByRef req As DSCPS.DeleteOutsideAuditor.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteOutsideAuditor
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeletePendingImage(ByRef res As DSCPS.DeletePendingImage.Response,
                                           ByRef req As DSCPS.DeletePendingImage.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePendingImage
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeletePolicySearchTypeViewableByUserCategory(ByRef res As DSCPS.DeletePolicySearchTypeViewableByUserCategory.Response,
                                                                     ByRef req As DSCPS.DeletePolicySearchTypeViewableByUserCategory.Request,
                                                                     Optional ByRef e As Exception = Nothing,
                                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeletePolicySearchTypeViewableByUserCategory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteQuote(ByRef res As DSCPS.DeleteQuote.Response,
                                    ByRef req As DSCPS.DeleteQuote.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteQuote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DemotePendingToQuote(ByRef res As DSCPS.DemotePendingToQuote.Response,
                                             ByRef req As DSCPS.DemotePendingToQuote.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DemotePendingToQuote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetActiveRisksByClientId(ByRef res As DSCPS.GetActiveRisksByClientId.Response,
                                                 ByRef req As DSCPS.GetActiveRisksByClientId.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetActiveRisksByClientId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetAddress(ByRef res As DSCPS.GetAddress.Response,
                                   ByRef req As DSCPS.GetAddress.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetAgencyData(ByRef res As DSCPS.GetAgencyData.Response,
                                      ByRef req As DSCPS.GetAgencyData.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetAgencyData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientPolicies(ByRef res As DSCPS.GetClientPolicies.Response,
                                          ByRef req As DSCPS.GetClientPolicies.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetClientPolicyInfo(ByRef res As DSCPS.GetClientPolicyInfo.Response,
                                            ByRef req As DSCPS.GetClientPolicyInfo.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetClientPolicyInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetFirstWrittenDate(ByRef res As DSCPS.GetFirstWrittenDate.Response,
                                            ByRef req As DSCPS.GetFirstWrittenDate.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetFirstWrittenDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNAICSData(ByRef res As DSCPS.GetNAICSData.Response,
                                     ByRef req As DSCPS.GetNAICSData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNAICSData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNewPolicyLinkNumber(ByRef res As DSCPS.GetNewPolicyLinkNumber.Response,
                                               ByRef req As DSCPS.GetNewPolicyLinkNumber.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNewPolicyLinkNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNewPolicyOrQuoteNumber(ByRef res As DSCPS.GetNewPolicyOrQuoteNumber.Response,
                                                  ByRef req As DSCPS.GetNewPolicyOrQuoteNumber.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNewPolicyOrQuoteNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPendingOrQuoteImageByPolicyIdAndOrImageNum(ByRef res As DSCPS.GetPendingOrQuoteImageByPolicyIdAndOrImageNum.Response,
                                                                      ByRef req As DSCPS.GetPendingOrQuoteImageByPolicyIdAndOrImageNum.Request,
                                                                      Optional ByRef e As Exception = Nothing,
                                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPendingOrQuoteImageByPolicyIdAndOrImageNum
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyHistory(ByRef res As DSCPS.GetPolicyHistory.Response,
                                         ByRef req As DSCPS.GetPolicyHistory.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyIdAndNumForBillingAccount(ByRef res As DSCPS.GetPolicyIdAndNumForBillingAccount.Response,
                                                           ByRef req As DSCPS.GetPolicyIdAndNumForBillingAccount.Request,
                                                           Optional ByRef e As Exception = Nothing,
                                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyIdAndNumForBillingAccount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyIdAndNumForPolicyNumber(ByRef res As DSCPS.GetPolicyIdAndNumForPolicyNumber.Response,
                                                         ByRef req As DSCPS.GetPolicyIdAndNumForPolicyNumber.Request,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyIdAndNumForPolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPolicyMessage(ByRef res As DSCPS.GetPolicyMessage.Response,
                                         ByRef req As DSCPS.GetPolicyMessage.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyMessage
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetPreviousPolicyHistory(ByRef res As DSCPS.GetPreviousPolicyHistory.Response,
                                                 ByRef req As DSCPS.GetPreviousPolicyHistory.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPreviousPolicyHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetTerritoryData(ByRef res As DSCPS.GetTerritoryData.Response,
                                         ByRef req As DSCPS.GetTerritoryData.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetTerritoryData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetValidPackagePartTypes(ByRef res As DSCPS.GetValidPackagePartTypes.Response,
                                                 ByRef req As DSCPS.GetValidPackagePartTypes.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetValidPackagePartTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVersionIdByPolicyId(ByRef res As DSCPS.GetVersionIdByPolicyId.Response,
                                               ByRef req As DSCPS.GetVersionIdByPolicyId.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVersionIdByPolicyId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVersionIdByPolicyNumber(ByRef res As DSCPS.GetVersionIdByPolicyNumber.Response,
                                                   ByRef req As DSCPS.GetVersionIdByPolicyNumber.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVersionIdByPolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVersionIdForPackagePart(ByRef res As DSCPS.GetVersionIdForPackagePart.Response,
                                                   ByRef req As DSCPS.GetVersionIdForPackagePart.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVersionIdForPackagePart
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVersionToSubmit(ByRef res As DSCPS.GetVersionToSubmit.Response,
                                           ByRef req As DSCPS.GetVersionToSubmit.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVersionToSubmit
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ImportPackagePart(ByRef res As DSCPS.ImportPackagePart.Response,
                                          ByRef req As DSCPS.ImportPackagePart.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ImportPackagePart
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ImportUnderlyingPolicies(ByRef res As DSCPS.ImportUnderlyingPolicies.Response,
                                                 ByRef req As DSCPS.ImportUnderlyingPolicies.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ImportUnderlyingPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsExternalPolicy(ByRef res As DSCPS.IsExternalPolicy.Response,
                                         ByRef req As DSCPS.IsExternalPolicy.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsExternalPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsNewTEffDateValid(ByRef res As DSCPS.IsNewTEffDateValid.Response,
                                           ByRef req As DSCPS.IsNewTEffDateValid.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsNewTEffDateValid
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Issue(ByRef res As DSCPS.Issue.Response,
                              ByRef req As DSCPS.Issue.Request,
                              Optional ByRef e As Exception = Nothing,
                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Issue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IssueByPolicyId(ByRef res As DSCPS.IssueByPolicyId.Response,
                                        ByRef req As DSCPS.IssueByPolicyId.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IssueByPolicyId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadClient(ByRef res As DSCPS.LoadClient.Response,
                                   ByRef req As DSCPS.LoadClient.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadCoveragePlanDefaults(ByRef res As DSCPS.LoadCoveragePlanDefaults.Response,
                                                 ByRef req As DSCPS.LoadCoveragePlanDefaults.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadCoveragePlanDefaults
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadCoveragePlans(ByRef res As DSCPS.LoadCoveragePlans.Response,
                                          ByRef req As DSCPS.LoadCoveragePlans.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadCoveragePlans
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadExperienceModification(ByRef res As DSCPS.LoadExperienceModification.Response,
                                                   ByRef req As DSCPS.LoadExperienceModification.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadExperienceModification
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadFilingHistory(ByRef res As DSCPS.LoadFilingHistory.Response,
                                          ByRef req As DSCPS.LoadFilingHistory.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadFilingHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadFilingInfo(ByRef res As DSCPS.LoadFilingInfo.Response,
                                       ByRef req As DSCPS.LoadFilingInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadFilingInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadFutureEvents(ByRef res As DSCPS.LoadFutureEvents.Response,
                                         ByRef req As DSCPS.LoadFutureEvents.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadFutureEvents
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadImage(ByRef res As DSCPS.LoadImage.Response,
                                  ByRef req As DSCPS.LoadImage.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            'Dim p As New DCSP.PolicyServiceProxy
            Using p As New DCSP.PolicyServiceProxy
                Dim m As Services.Common.pMethod = AddressOf p.LoadImage
                res = RunDiamondService(m, req, e, dv)
                If res IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Using
        End Function

        Public Function LoadImagesByPolicyNumber(ByRef res As DSCPS.LoadImagesByPolicyNumber.Response,
                                                 ByRef req As DSCPS.LoadImagesByPolicyNumber.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadImagesByPolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadInspection(ByRef res As DSCPS.LoadInspection.Response,
                                       ByRef req As DSCPS.LoadInspection.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadInspection
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadMiscPolicyStatic(ByRef res As DSCPS.LoadMiscPolicyStatic.Response,
                                             ByRef req As DSCPS.LoadMiscPolicyStatic.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadMiscPolicyStatic
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadOutsideAuditor(ByRef res As DSCPS.LoadOutsideAuditor.Response,
                                           ByRef req As DSCPS.LoadOutsideAuditor.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadOutsideAuditor
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadOutsideAuditorList(ByRef res As DSCPS.LoadOutsideAuditorList.Response,
                                               ByRef req As DSCPS.LoadOutsideAuditorList.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadOutsideAuditorList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadOutsideAuditorNames(ByRef res As DSCPS.LoadOutsideAuditorNames.Response,
                                                ByRef req As DSCPS.LoadOutsideAuditorNames.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadOutsideAuditorNames
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPackagePartList(ByRef res As DSCPS.LoadPackagePartList.Response,
                                            ByRef req As DSCPS.LoadPackagePartList.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPackagePartList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyAuditList(ByRef res As DSCPS.LoadPolicyAuditList.Response,
                                            ByRef req As DSCPS.LoadPolicyAuditList.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyAuditList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyAudits(ByRef res As DSCPS.LoadPolicyAudits.Response,
                                         ByRef req As DSCPS.LoadPolicyAudits.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyAudits
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyDetail(ByRef res As DSCPS.LoadPolicyDetail.Response,
                                         ByRef req As DSCPS.LoadPolicyDetail.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicySearchTypeViewableByUserCategoryList(ByRef res As DSCPS.LoadPolicySearchTypeViewableByUserCategoryList.Response,
                                                                       ByRef req As DSCPS.LoadPolicySearchTypeViewableByUserCategoryList.Request,
                                                                       Optional ByRef e As Exception = Nothing,
                                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicySearchTypeViewableByUserCategoryList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadProblemPolicyAccounts(ByRef res As DSCPS.LoadProblemPolicyAccounts.Response,
                                                  ByRef req As DSCPS.LoadProblemPolicyAccounts.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadProblemPolicyAccounts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadRecentBillingListForUser(ByRef res As DSCPS.LoadRecentBillingListForUser.Response,
                                                     ByRef req As DSCPS.LoadRecentBillingListForUser.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadRecentBillingListForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadRecentPolicyListForUser(ByRef res As DSCPS.LoadRecentPolicyListForUser.Response,
                                                    ByRef req As DSCPS.LoadRecentPolicyListForUser.Request,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadRecentPolicyListForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadRenewalUnderwriting(ByRef res As DSCPS.LoadRenewalUnderwriting.Response,
                                                ByRef req As DSCPS.LoadRenewalUnderwriting.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadRenewalUnderwriting
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadSummaryOfChanges(ByRef res As DSCPS.LoadSummaryOfChanges.Response,
                                             ByRef req As DSCPS.LoadSummaryOfChanges.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadSummaryOfChanges
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadTransactions(ByRef res As DSCPS.LoadTransactions.Response,
                                         ByRef req As DSCPS.LoadTransactions.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadValidation(ByRef res As DSCPS.LoadValidation.Response,
                                       ByRef req As DSCPS.LoadValidation.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadValidation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LookupMultiPolicyDiscountPolicies(ByRef res As DSCPS.LookupMultiPolicyDiscountPolicies.Response,
                                                          ByRef req As DSCPS.LookupMultiPolicyDiscountPolicies.Request,
                                                          Optional ByRef e As Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LookupMultiPolicyDiscountPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LookupUnderlyingPolicies(ByRef res As DSCPS.LookupUnderlyingPolicies.Response,
                                                 ByRef req As DSCPS.LookupUnderlyingPolicies.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LookupUnderlyingPolicies
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessAACS(ByRef res As DSCPS.ProcessAACS.Response,
                                    ByRef req As DSCPS.ProcessAACS.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessAACS
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function PromoteQuoteToPending(ByRef res As DSCPS.PromoteQuoteToPending.Response,
                                              ByRef req As DSCPS.PromoteQuoteToPending.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PromoteQuoteToPending
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function QueryForBillingAccountNumber(ByRef res As DSCPS.QueryForBillingAccountNumber.Response,
                                                     ByRef req As DSCPS.QueryForBillingAccountNumber.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.QueryForBillingAccountNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function QueryForPolicyNumber(ByRef res As DSCPS.QueryForPolicyNumber.Response,
                                             ByRef req As DSCPS.QueryForPolicyNumber.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.QueryForPolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function QueuePolicyForExport(ByRef res As DSCPS.QueuePolicyForExport.Response,
                                             ByRef req As DSCPS.QueuePolicyForExport.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.QueuePolicyForExport
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Rate(ByRef res As DSCPS.Rate.Response,
                             ByRef req As DSCPS.Rate.Request,
                             Optional ByRef e As Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Rate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function RateOnly(ByRef res As DSCPS.RateOnly.Response,
                                 ByRef req As DSCPS.RateOnly.Request,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RateOnly
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function RouteQuoteToUnderwriting(ByRef res As DSCPS.RouteQuoteToUnderwriting.Response,
                                                 ByRef req As DSCPS.RouteQuoteToUnderwriting.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RouteQuoteToUnderwriting
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveClient(ByRef res As DSCPS.SaveClient.Response,
                                   ByRef req As DSCPS.SaveClient.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing
                                   ) As Boolean

            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveClientInfo(ByRef res As DSCPS.SaveClientInfo.Response,
                                       ByRef req As DSCPS.SaveClientInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveClientInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveExperienceModification(ByRef res As DSCPS.SaveExperienceModification.Response,
                                                   ByRef req As DSCPS.SaveExperienceModification.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveExperienceModification
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveFilingHistory(ByRef res As DSCPS.SaveFilingHistory.Response,
                                          ByRef req As DSCPS.SaveFilingHistory.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveFilingHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveFilingInfo(ByRef res As DSCPS.SaveFilingInfo.Response,
                                       ByRef req As DSCPS.SaveFilingInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveFilingInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveInspection(ByRef res As DSCPS.SaveInspection.Response,
                                       ByRef req As DSCPS.SaveInspection.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveInspection
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveOutsideAuditor(ByRef res As DSCPS.SaveOutsideAuditor.Response,
                                           ByRef req As DSCPS.SaveOutsideAuditor.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveOutsideAuditor
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SavePolicyAudits(ByRef res As DSCPS.SavePolicyAudits.Response,
                                         ByRef req As DSCPS.SavePolicyAudits.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SavePolicyAudits
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SavePolicySearchTypeViewableByUserCategory(ByRef res As DSCPS.SavePolicySearchTypeViewableByUserCategory.Response,
                                                                   ByRef req As DSCPS.SavePolicySearchTypeViewableByUserCategory.Request,
                                                                   Optional ByRef e As Exception = Nothing,
                                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SavePolicySearchTypeViewableByUserCategory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveProblemPolicyAccounts(ByRef res As DSCPS.SaveProblemPolicyAccounts.Response,
                                                  ByRef req As DSCPS.SaveProblemPolicyAccounts.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveProblemPolicyAccounts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveRate(ByRef res As DSCPS.SaveRate.Response,
                                 ByRef req As DSCPS.SaveRate.Request,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveRate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveRateIssue(ByRef res As DSCPS.SaveRateIssue.Response,
                                      ByRef req As DSCPS.SaveRateIssue.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveRateIssue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveRecentBillingForUser(ByRef res As DSCPS.SaveRecentBillingForUser.Response,
                                                 ByRef req As DSCPS.SaveRecentBillingForUser.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveRecentBillingForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveRecentPolicyForUser(ByRef res As DSCPS.SaveRecentPolicyForUser.Response,
                                                ByRef req As DSCPS.SaveRecentPolicyForUser.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveRecentPolicyForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveTransRemark(ByRef res As DSCPS.SaveTransRemark.Response,
                                        ByRef req As DSCPS.SaveTransRemark.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveTransRemark
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByAdditionalPolicyHolder(ByRef res As DSCPS.SearchByAdditionalPolicyHolder.Response,
                                                       ByRef req As DSCPS.SearchByAdditionalPolicyHolder.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByAdditionalPolicyHolder
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByAddress(ByRef res As DSCPS.SearchByAddress.Response,
                                        ByRef req As DSCPS.SearchByAddress.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByAddress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByClient(ByRef res As DSCPS.SearchByClient.Response,
                                       ByRef req As DSCPS.SearchByClient.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByDoingBusinessAs(ByRef res As DSCPS.SearchByDoingBusinessAs.Response,
                                                ByRef req As DSCPS.SearchByDoingBusinessAs.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByDoingBusinessAs
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByFullName(ByRef res As DSCPS.SearchByFullName.Response,
                                         ByRef req As DSCPS.SearchByFullName.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByFullName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByManuallyCreatedClientIdentifier(ByRef res As DSCPS.SearchByManuallyCreatedClientIdentifier.Response,
                                                                ByRef req As DSCPS.SearchByManuallyCreatedClientIdentifier.Request,
                                                                Optional ByRef e As Exception = Nothing,
                                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByManuallyCreatedClientIdentifier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByName(ByRef res As DSCPS.SearchByName.Response,
                                     ByRef req As DSCPS.SearchByName.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByPhoneNumber(ByRef res As DSCPS.SearchByPhoneNumber.Response,
                                            ByRef req As DSCPS.SearchByPhoneNumber.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByPhoneNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByPolicyNumber(ByRef res As DSCPS.SearchByPolicyNumber.Response,
                                             ByRef req As DSCPS.SearchByPolicyNumber.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByPolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SearchByQuoteNumber(ByRef res As DSCPS.SearchByQuoteNumber.Response,
                                            ByRef req As DSCPS.SearchByQuoteNumber.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SearchByQuoteNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SetPolicyLock(ByRef res As DSCPS.SetPolicyLock.Response,
                                      ByRef req As DSCPS.SetPolicyLock.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SetPolicyLock
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SmallLoad(ByRef res As DSCPS.SmallLoad.Response,
                                  ByRef req As DSCPS.SmallLoad.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SmallLoad
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SmallSave(ByRef res As DSCPS.SmallSave.Response,
                                  ByRef req As DSCPS.SmallSave.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SmallSave
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SR2226Import(ByRef res As DSCPS.SR2226Import.Response,
                                     ByRef req As DSCPS.SR2226Import.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SR2226Import
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SubmitApplication(ByRef res As DSCPS.SubmitApplication.Response,
                                          ByRef req As DSCPS.SubmitApplication.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SubmitApplication
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SubmitRate(ByRef res As DSCPS.SubmitRate.Response,
                                   ByRef req As DSCPS.SubmitRate.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SubmitRate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Tiering(ByRef res As DSCPS.Tiering.Response,
                                ByRef req As DSCPS.Tiering.Request,
                                Optional ByRef e As Exception = Nothing,
                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Tiering
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TransferPolicyToClient(ByRef res As DSCPS.TransferPolicyToClient.Response,
                                               ByRef req As DSCPS.TransferPolicyToClient.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TransferPolicyToClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UndenyPolicy(ByRef res As DSCPS.UndenyPolicy.Response,
                                     ByRef req As DSCPS.UndenyPolicy.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.UndenyPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UpdateReferredBy(ByRef res As DSCPS.UpdateReferredBy.Response,
                                         ByRef req As DSCPS.UpdateReferredBy.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdateReferredBy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ValidateApplication(ByRef res As DSCPS.ValidateApplication.Response,
                                            ByRef req As DSCPS.ValidateApplication.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ValidateApplication
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function VerifyUserCanSaveIssue(ByRef res As DSCPS.VerifyUserCanSaveIssue.Response,
                                               ByRef req As DSCPS.VerifyUserCanSaveIssue.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.VerifyUserCanSaveIssue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
