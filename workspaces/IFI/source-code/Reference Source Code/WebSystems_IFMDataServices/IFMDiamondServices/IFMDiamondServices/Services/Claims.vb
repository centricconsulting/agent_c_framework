Imports Microsoft.VisualBasic
Imports DCSC = Diamond.Common.Services.Messages.ClaimsService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Claims
    Public Module Claims
        Public Function AddPayeeName(ByRef res As DCSC.AddPayeeName.Response,
                                     ByRef req As DCSC.AddPayeeName.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddPayeeName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddReserves(ByRef res As DCSC.AddReserves.Response,
                                     ByRef req As DCSC.AddReserves.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddReserves
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AdjustSplitTransactionAmount(ByRef res As DCSC.AdjustSplitTransactionAmount.Response,
                                     ByRef req As DCSC.AdjustSplitTransactionAmount.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AdjustSplitTransactionAmount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AdjustSplitTransactionDeductibles(ByRef res As DCSC.AdjustSplitTransactionDeductibles.Response,
                                     ByRef req As DCSC.AdjustSplitTransactionDeductibles.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AdjustSplitTransactionDeductibles
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ApproveTransaction(ByRef res As DCSC.ApproveTransaction.Response,
                                     ByRef req As DCSC.ApproveTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ApproveTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BifurcateClaim(ByRef res As DCSC.BifurcateClaim.Response,
                                     ByRef req As DCSC.BifurcateClaim.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.BifurcateClaim
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ChangeLossDate(ByRef res As DCSC.ChangeLossDate.Response,
                                     ByRef req As DCSC.ChangeLossDate.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ChangeLossDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ChangeScheduledPaymentCycleStatus(ByRef res As DCSC.ChangeScheduledPaymentCycleStatus.Response,
                                     ByRef req As DCSC.ChangeScheduledPaymentCycleStatus.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ChangeScheduledPaymentCycleStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckforDeductWaived(ByRef res As DCSC.CheckforDeductWaived.Response,
                                     ByRef req As DCSC.CheckforDeductWaived.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckforDeductWaived
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckforSameDeductibleApplied(ByRef res As DCSC.CheckforSameDeductibleApplied.Response,
                                     ByRef req As DCSC.CheckforSameDeductibleApplied.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckforSameDeductibleApplied
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckStopPayment(ByRef res As DCSC.CheckStopPayment.Response,
                                     ByRef req As DCSC.CheckStopPayment.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckStopPayment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddPayeeName(ByRef res As DCSC.ClaimIsRecordOnly.Response,
                                     ByRef req As DCSC.ClaimIsRecordOnly.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ClaimIsRecordOnly
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CloseClaimantRecord(ByRef res As DCSC.CloseClaimantRecord.Response,
                                     ByRef req As DCSC.CloseClaimantRecord.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CloseClaimantRecord
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CloseClaimRecord(ByRef res As DCSC.CloseClaimRecord.Response,
                                     ByRef req As DCSC.CloseClaimRecord.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CloseClaimRecord
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CloseFeatureRecord(ByRef res As DCSC.CloseFeatureRecord.Response,
                                     ByRef req As DCSC.CloseFeatureRecord.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CloseFeatureRecord
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CountNotifyUnderwriting(ByRef res As DCSC.CountNotifyUnderwriting.Response,
                                     ByRef req As DCSC.CountNotifyUnderwriting.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CountNotifyUnderwriting
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateDCPolicy(ByRef res As DCSC.CreateDCPolicy.Response,
                                     ByRef req As DCSC.CreateDCPolicy.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateDCPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateDCPolicyXML(ByRef res As DCSC.CreateDCPolicyXML.Response,
                                     ByRef req As DCSC.CreateDCPolicyXML.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateDCPolicyXML
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateMedicareClaimFile(ByRef res As DCSC.CreateMedicareClaimFile.Response,
                                     ByRef req As DCSC.CreateMedicareClaimFile.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateMedicareClaimFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateMedicareQueryFile(ByRef res As DCSC.CreateMedicareQueryFile.Response,
                                     ByRef req As DCSC.CreateMedicareQueryFile.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateMedicareQueryFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateMedicareTINReferenceFile(ByRef res As DCSC.CreateMedicareTINReferenceFile.Response,
                                     ByRef req As DCSC.CreateMedicareTINReferenceFile.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateMedicareTINReferenceFile
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAppraiser(ByRef res As DCSC.DeleteAppraiser.Response,
                                     ByRef req As DCSC.DeleteAppraiser.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAppraiser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCatastrophe(ByRef res As DCSC.DeleteCatastrophe.Response,
                                     ByRef req As DCSC.DeleteCatastrophe.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCatastrophe
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteClaimant(ByRef res As DCSC.DeleteClaimant.Response,
                                     ByRef req As DCSC.DeleteClaimant.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteClaimant
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteClaimControlProperty(ByRef res As DCSC.DeleteClaimControlProperty.Response,
                                     ByRef req As DCSC.DeleteClaimControlProperty.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteClaimControlProperty
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteClaimControlVehicle(ByRef res As DCSC.DeleteClaimControlVehicle.Response,
                                     ByRef req As DCSC.DeleteClaimControlVehicle.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteClaimControlVehicle
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteClaimNumberFields(ByRef res As DCSC.DeleteClaimNumberFields.Response,
                                     ByRef req As DCSC.DeleteClaimNumberFields.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteClaimNumberFields
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteDashboardTemplate(ByRef res As DCSC.DeleteDashboardTemplate.Response,
                                     ByRef req As DCSC.DeleteDashboardTemplate.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteDashboardTemplate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteFeatureDefault(ByRef res As DCSC.DeleteFeatureDefault.Response,
                                     ByRef req As DCSC.DeleteFeatureDefault.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteFeatureDefault
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteInitialReserve(ByRef res As DCSC.DeleteInitialReserve.Response,
                                     ByRef req As DCSC.DeleteInitialReserve.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteInitialReserve
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteLevy(ByRef res As DCSC.DeleteLevy.Response,
                                     ByRef req As DCSC.DeleteLevy.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteLevy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteNotifyUnderwriting(ByRef res As DCSC.DeleteNotifyUnderwriting.Response,
                                     ByRef req As DCSC.DeleteNotifyUnderwriting.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteNotifyUnderwriting
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePayee(ByRef res As DCSC.DeletePayee.Response,
                                     ByRef req As DCSC.DeletePayee.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePayee
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePendingClaim(ByRef res As DCSC.DeletePendingClaim.Response,
                                     ByRef req As DCSC.DeletePendingClaim.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePendingClaim
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePTTOOItem(ByRef res As DCSC.DeletePTTOOItem.Response,
                                     ByRef req As DCSC.DeletePTTOOItem.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePTTOOItem
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteRepairShop(ByRef res As DCSC.DeleteRepairShop.Response,
                                     ByRef req As DCSC.DeleteRepairShop.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteRepairShop
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteScheduledPaymentCycle(ByRef res As DCSC.DeleteScheduledPaymentCycle.Response,
                                     ByRef req As DCSC.DeleteScheduledPaymentCycle.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteScheduledPaymentCycle
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteWitness(ByRef res As DCSC.DeleteWitness.Response,
                                     ByRef req As DCSC.DeleteWitness.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteWitness
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetClaimNumberData(ByRef res As DCSC.GetClaimNumberData.Response,
                                     ByRef req As DCSC.GetClaimNumberData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetClaimNumberData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetClaimPersonnel(ByRef res As DCSC.GetClaimPersonnel.Response,
                                     ByRef req As DCSC.GetClaimPersonnel.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetClaimPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetClaimPersonnelByType(ByRef res As DCSC.GetClaimPersonnelByType.Response,
                                     ByRef req As DCSC.GetClaimPersonnelByType.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetClaimPersonnelByType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetClaimPersonnelForOffice(ByRef res As DCSC.GetClaimPersonnelForOffice.Response,
                                     ByRef req As DCSC.GetClaimPersonnelForOffice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetClaimPersonnelForOffice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetDeductAmtForClaimFeature(ByRef res As DCSC.GetDeductAmtForClaimFeature.Response,
                                     ByRef req As DCSC.GetDeductAmtForClaimFeature.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDeductAmtForClaimFeature
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetFeatureStatus(ByRef res As DCSC.GetFeatureStatus.Response,
                                     ByRef req As DCSC.GetFeatureStatus.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetFeatureStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetLatestPayeeVersion(ByRef res As DCSC.GetLatestPayeeVersion.Response,
                                     ByRef req As DCSC.GetLatestPayeeVersion.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetLatestPayeeVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetNextClaimNumber(ByRef res As DCSC.GetNextClaimNumber.Response,
                                     ByRef req As DCSC.GetNextClaimNumber.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetNextClaimNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPayeeForReason(ByRef res As DCSC.GetPayeeForReason.Response,
                                     ByRef req As DCSC.GetPayeeForReason.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPayeeForReason
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPolicyIdNumForClaimControlId(ByRef res As DCSC.GetPolicyIdNumForClaimControlId.Response,
                                     ByRef req As DCSC.GetPolicyIdNumForClaimControlId.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPolicyIdNumForClaimControlId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPolicyIdNumForPolicyNoLossDate(ByRef res As DCSC.GetPolicyIdNumForPolicyNoLossDate.Response,
                                     ByRef req As DCSC.GetPolicyIdNumForPolicyNoLossDate.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPolicyIdNumForPolicyNoLossDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetTransactionForPayeePhrase(ByRef res As DCSC.GetTransactionForPayeePhrase.Response,
                                     ByRef req As DCSC.GetTransactionForPayeePhrase.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetTransactionForPayeePhrase
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetVersionIdForClaimControlId(ByRef res As DCSC.GetVersionIdForClaimControlId.Response,
                                     ByRef req As DCSC.GetVersionIdForClaimControlId.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetVersionIdForClaimControlId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetVersionIdForPolicyIdNum(ByRef res As DCSC.GetVersionIdForPolicyIdVer.Response,
                                     ByRef req As DCSC.GetVersionIdForPolicyIdVer.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetVersionIdForPolicyIdNum
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetVersionIdForPolicyNoLossDate(ByRef res As DCSC.GetVersionIdForPolicyNoLossDate.Response,
                                     ByRef req As DCSC.GetVersionIdForPolicyNoLossDate.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetVersionIdForPolicyNoLossDate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function InitializeInitialReserveSetup(ByRef res As DCSC.InitializeInitialReserveSetup.Response,
                                     ByRef req As DCSC.InitializeInitialReserveSetup.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.InitializeInitialReserveSetup
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function InterpretExternalPolicyRecords(ByRef res As DCSC.InterpretExternalPolicyRecords.Response,
                                     ByRef req As DCSC.InterpretExternalPolicyRecords.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.InterpretExternalPolicyRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function InterpretPolicyRecords(ByRef res As DCSC.InterpretPolicyRecords.Response,
                                     ByRef req As DCSC.InterpretPolicyRecords.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.InterpretPolicyRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsOffsetPayment(ByRef res As DCSC.IsOffsetPayment.Response,
                                     ByRef req As DCSC.IsOffsetPayment.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsOffsetPayment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsTransactionReissued(ByRef res As DCSC.IsTransactionReissued.Response,
                                     ByRef req As DCSC.IsTransactionReissued.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsTransactionReissued
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadActivity(ByRef res As DCSC.LoadActivity.Response,
                                     ByRef req As DCSC.LoadActivity.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadActivity
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdjustCoveragesExposures(ByRef res As DCSC.LoadAdjustCoveragesExposures.Response,
                                     ByRef req As DCSC.LoadAdjustCoveragesExposures.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdjustCoveragesExposures
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdjustCoveragesSubExposures(ByRef res As DCSC.LoadAdjustCoveragesSubExposures.Response,
                                     ByRef req As DCSC.LoadAdjustCoveragesSubExposures.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdjustCoveragesSubExposures
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdjustCoveragesTreeData(ByRef res As DCSC.LoadAdjustCoveragesTreeData.Response,
                                     ByRef req As DCSC.LoadAdjustCoveragesTreeData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdjustCoveragesTreeData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdjuster(ByRef res As DCSC.LoadAdjuster.Response,
                                     ByRef req As DCSC.LoadAdjuster.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdjuster
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdjusterList(ByRef res As DCSC.LoadAdjusterList.Response,
                                     ByRef req As DCSC.LoadAdjusterList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdjusterList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAdjusterMaintenance(ByRef res As DCSC.LoadAdjusterMaintenance.Response,
                                     ByRef req As DCSC.LoadAdjusterMaintenance.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdjusterMaintenance
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyInfo(ByRef res As DCSC.LoadAgencyInfo.Response,
                                     ByRef req As DCSC.LoadAgencyInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyProducerInfo(ByRef res As DCSC.LoadAgencyProducerInfo.Response,
                                     ByRef req As DCSC.LoadAgencyProducerInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyProducerInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllFeatureDefaults(ByRef res As DCSC.LoadAllFeatureDefaults.Response,
                                     ByRef req As DCSC.LoadAllFeatureDefaults.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllFeatureDefaults
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAppraiser(ByRef res As DCSC.LoadAppraiser.Response,
                                     ByRef req As DCSC.LoadAppraiser.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAppraiser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAppraiserDetails(ByRef res As DCSC.LoadAppraiserDetails.Response,
                                     ByRef req As DCSC.LoadAppraiserDetails.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAppraiserDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAppraiserList(ByRef res As DCSC.LoadAppraiserList.Response,
                                     ByRef req As DCSC.LoadAppraiserList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAppraiserList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAppraiserNames(ByRef res As DCSC.LoadAppraiserNames.Response,
                                     ByRef req As DCSC.LoadAppraiserNames.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAppraiserNames
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAttorneys(ByRef res As DCSC.LoadAttorneys.Response,
                                     ByRef req As DCSC.LoadAttorneys.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAttorneys
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBifurcatedLink(ByRef res As DCSC.LoadBifurcatedLink.Response,
                                     ByRef req As DCSC.LoadBifurcatedLink.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBifurcatedLink
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBuildPTTOO(ByRef res As DCSC.LoadBuildPTTOO.Response,
                                     ByRef req As DCSC.LoadBuildPTTOO.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBuildPTTOO
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCareProvider(ByRef res As DCSC.LoadCareProvider.Response,
                                     ByRef req As DCSC.LoadCareProvider.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCareProvider
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCareProviderList(ByRef res As DCSC.LoadCareProviderList.Response,
                                     ByRef req As DCSC.LoadCareProviderList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCareProviderList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCareProviderMaintenance(ByRef res As DCSC.LoadCareProviderMaintenance.Response,
                                     ByRef req As DCSC.LoadCareProviderMaintenance.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCareProviderMaintenance
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCarrier(ByRef res As DCSC.LoadCarrier.Response,
                                     ByRef req As DCSC.LoadCarrier.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCarrier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCarrierList(ByRef res As DCSC.LoadCarrierList.Response,
                                     ByRef req As DCSC.LoadCarrierList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCarrierList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCarrierMaintenance(ByRef res As DCSC.LoadCarrierMaintenance.Response,
                                     ByRef req As DCSC.LoadCarrierMaintenance.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCatastrophe
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCatastrophe(ByRef res As DCSC.LoadCatastrophe.Response,
                                     ByRef req As DCSC.LoadCatastrophe.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCatastrophe
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCatastropheCombo(ByRef res As DCSC.LoadCatastropheCombo.Response,
                                     ByRef req As DCSC.LoadCatastropheCombo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCatastropheCombo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCatastropheEdit(ByRef res As DCSC.LoadCatastropheEdit.Response,
                                     ByRef req As DCSC.LoadCatastropheEdit.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCatastropheEdit
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCatastropheList(ByRef res As DCSC.LoadCatastropheList.Response,
                                     ByRef req As DCSC.LoadCatastropheList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCatastropheList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCatastrophesForYear(ByRef res As DCSC.LoadCatastrophesForYear.Response,
                                     ByRef req As DCSC.LoadCatastrophesForYear.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCatastrophesForYear
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCheckInfo(ByRef res As DCSC.LoadCheckInfo.Response,
                                     ByRef req As DCSC.LoadCheckInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCheckInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimant(ByRef res As DCSC.LoadClaimant.Response,
                                     ByRef req As DCSC.LoadClaimant.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimant
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimantInfo(ByRef res As DCSC.LoadClaimantInfo.Response,
                                     ByRef req As DCSC.LoadClaimantInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimantInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimantInjury(ByRef res As DCSC.LoadClaimantInjury.Response,
                                     ByRef req As DCSC.LoadClaimantInjury.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimantInjury
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimantList(ByRef res As DCSC.LoadClaimantList.Response,
                                     ByRef req As DCSC.LoadClaimantList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimantList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlListedAIData(ByRef res As DCSC.LoadClaimControlListedAIData.Response,
                                     ByRef req As DCSC.LoadClaimControlListedAIData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlListedAIData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlListedAIDataAll(ByRef res As DCSC.LoadClaimControlListedAIDataAll.Response,
                                     ByRef req As DCSC.LoadClaimControlListedAIDataAll.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlListedAIDataAll
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlListedProperties(ByRef res As DCSC.LoadClaimControlListedProperties.Response,
                                     ByRef req As DCSC.LoadClaimControlListedProperties.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlListedProperties
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlListedVehicles(ByRef res As DCSC.LoadClaimControlListedVehicles.Response,
                                     ByRef req As DCSC.LoadClaimControlListedVehicles.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlListedVehicles
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlPersonnel(ByRef res As DCSC.LoadClaimControlPersonnel.Response,
                                     ByRef req As DCSC.LoadClaimControlPersonnel.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlPersonnelByType(ByRef res As DCSC.LoadClaimControlPersonnelByType.Response,
                                     ByRef req As DCSC.LoadClaimControlPersonnelByType.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlPersonnelByType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlProperties(ByRef res As DCSC.LoadClaimControlProperties.Response,
                                     ByRef req As DCSC.LoadClaimControlProperties.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlProperties
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlPropertyAppraisal(ByRef res As DCSC.LoadClaimControlPropertyAppraisal.Response,
                                     ByRef req As DCSC.LoadClaimControlPropertyAppraisal.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlPropertyAppraisal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlPropertyAppraisals(ByRef res As DCSC.LoadClaimControlPropertyAppraisals.Response,
                                     ByRef req As DCSC.LoadClaimControlPropertyAppraisals.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlPropertyAppraisals
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlScreenData(ByRef res As DCSC.LoadClaimControlScreenData.Response,
                                     ByRef req As DCSC.LoadClaimControlScreenData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlScreenData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlVehicleAppraisal(ByRef res As DCSC.LoadClaimControlVehicleAppraisal.Response,
                                     ByRef req As DCSC.LoadClaimControlVehicleAppraisal.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlVehicleAppraisal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlVehicleAppraisals(ByRef res As DCSC.LoadClaimControlVehicleAppraisals.Response,
                                     ByRef req As DCSC.LoadClaimControlVehicleAppraisals.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlVehicleAppraisals
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimControlVehicles(ByRef res As DCSC.LoadClaimControlVehicles.Response,
                                     ByRef req As DCSC.LoadClaimControlVehicles.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimControlVehicles
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimCoverages(ByRef res As DCSC.LoadClaimCoverages.Response,
                                     ByRef req As DCSC.LoadClaimCoverages.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimCoverages
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimCoveragesForFeatureDefault(ByRef res As DCSC.LoadClaimCoveragesForFeatureDefault.Response,
                                     ByRef req As DCSC.LoadClaimCoveragesForFeatureDefault.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimCoveragesForFeatureDefault
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimDetailInfo(ByRef res As DCSC.LoadClaimDetailInfo.Response,
                                     ByRef req As DCSC.LoadClaimDetailInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimDetailInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimExposure(ByRef res As DCSC.LoadClaimExposure.Response,
                                     ByRef req As DCSC.LoadClaimExposure.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimExposure
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimFirmAttorney(ByRef res As DCSC.LoadClaimFirmAttorney.Response,
                                     ByRef req As DCSC.LoadClaimFirmAttorney.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimFirmAttorney
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimFirmAttorneyList(ByRef res As DCSC.LoadClaimFirmAttorneyList.Response,
                                     ByRef req As DCSC.LoadClaimFirmAttorneyList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimFirmAttorneyList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimFirmInfo(ByRef res As DCSC.LoadClaimFirmInfo.Response,
                                     ByRef req As DCSC.LoadClaimFirmInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimFirmInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimFirmList(ByRef res As DCSC.LoadClaimFirmList.Response,
                                     ByRef req As DCSC.LoadClaimFirmList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimFirmList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimFirmListByType(ByRef res As DCSC.LoadClaimFirmListByType.Response,
                                     ByRef req As DCSC.LoadClaimFirmListByType.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimFirmListByType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimForId(ByRef res As DCSC.LoadClaimForId.Response,
                                     ByRef req As DCSC.LoadClaimForId.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimForId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function LoadClaimForms(ByRef res As DCSC.LoadClaimForms.Response,
        '                             ByRef req As DCSC.LoadClaimForms.Request,
        '                             Optional ByRef e As Exception = Nothing,
        '                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.ClaimsServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.LoadClaimForms
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadClaimInquiry(ByRef res As DCSC.LoadClaimInquiry.Response,
                                     ByRef req As DCSC.LoadClaimInquiry.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimInquiry
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimOfficeListForAdmin(ByRef res As DCSC.LoadClaimOfficeListForAdmin.Response,
                                     ByRef req As DCSC.LoadClaimOfficeListForAdmin.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimOfficeListForAdmin
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimPayeeChangeReason(ByRef res As DCSC.LoadClaimPayeeChangeReason.Response,
                                     ByRef req As DCSC.LoadClaimPayeeChangeReason.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimPayeeChangeReason
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimPayeePhrase(ByRef res As DCSC.LoadClaimPayeePhrase.Response,
                                     ByRef req As DCSC.LoadClaimPayeePhrase.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimPayeePhrase
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimPersonnelDisplay(ByRef res As DCSC.LoadClaimPersonnelDisplay.Response,
                                     ByRef req As DCSC.LoadClaimPersonnelDisplay.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimPersonnelDisplay
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimPersonnelDisplayList(ByRef res As DCSC.LoadClaimPersonnelDisplayList.Response,
                                     ByRef req As DCSC.LoadClaimPersonnelDisplayList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimPersonnelDisplayList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaims(ByRef res As DCSC.LoadClaims.Response,
                                     ByRef req As DCSC.LoadClaims.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaims
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimsByUser(ByRef res As DCSC.LoadClaimsByUser.Response,
                                     ByRef req As DCSC.LoadClaimsByUser.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimsByUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimsList(ByRef res As DCSC.LoadClaimsList.Response,
                                     ByRef req As DCSC.LoadClaimsList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimsList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadClaimsListForPolicyNumber(ByRef res As DCSC.LoadClaimsListForPolicyNumber.Response,
                                     ByRef req As DCSC.LoadClaimsListForPolicyNumber.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadClaimsListForPolicyNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        ''' <summary>
        ''' Is not implemented...
        ''' </summary>
        ''' <param name="res"></param>
        ''' <param name="req"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        ''' <remarks>Diamond responses/requests are not inherited from response/request bases</remarks>
        Public Function LoadClaimSubCoverages(ByRef res As DCSC.LoadClaimSubCoverages.Response,
                                     ByRef req As DCSC.LoadClaimSubCoverages.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            'Dim p As New DCSP.ClaimsServiceProxy
            'Dim m As Services.common.pMethod = AddressOf p.LoadClaimSubCoverages
            'res = RunDiamondService(m, req, e, dv)
            'If res IsNot Nothing Then
            '    Return True
            'Else
            Return False
            'End If
        End Function
        Public Function LoadCloseClaimSummary(ByRef res As DCSC.LoadCloseClaimSummary.Response,
                                     ByRef req As DCSC.LoadCloseClaimSummary.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCloseClaimSummary
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompanyStateLOBs(ByRef res As DCSC.LoadCompanyStateLOBs.Response,
                                     ByRef req As DCSC.LoadCompanyStateLOBs.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompanyStateLOBs
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadContact(ByRef res As DCSC.LoadContact.Response,
                                     ByRef req As DCSC.LoadContact.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadContact
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCoverageList(ByRef res As DCSC.LoadCoverageList.Response,
                                     ByRef req As DCSC.LoadCoverageList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCoverageList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCoverages(ByRef res As DCSC.LoadCoverages.Response,
                                     ByRef req As DCSC.LoadCoverages.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCoverages
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDashboardControls(ByRef res As DCSC.LoadDashboardControls.Response,
                                     ByRef req As DCSC.LoadDashboardControls.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDashboardControls
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDashboardTemplates(ByRef res As DCSC.LoadDashboardTemplates.Response,
                                     ByRef req As DCSC.LoadDashboardTemplates.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDashboardTemplates
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDeptClaimFire(ByRef res As DCSC.LoadDeptClaimFire.Response,
                                     ByRef req As DCSC.LoadDeptClaimFire.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDeptClaimFire
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDeptClaimPolice(ByRef res As DCSC.LoadDeptClaimPolice.Response,
                                     ByRef req As DCSC.LoadDeptClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDeptClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDeptListClaimFire(ByRef res As DCSC.LoadDeptListClaimFire.Response,
                                     ByRef req As DCSC.LoadDeptListClaimFire.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDeptListClaimFire
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDeptListClaimPolice(ByRef res As DCSC.LoadDeptListClaimPolice.Response,
                                     ByRef req As DCSC.LoadDeptListClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDeptListClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDeptMaintenanceClaimFire(ByRef res As DCSC.LoadDeptMaintenanceClaimFire.Response,
                                     ByRef req As DCSC.LoadDeptMaintenanceClaimFire.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDeptMaintenanceClaimFire
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDeptMaintenanceClaimPolice(ByRef res As DCSC.LoadDeptMaintenanceClaimPolice.Response,
                                     ByRef req As DCSC.LoadDeptMaintenanceClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDeptMaintenanceClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadExposureList(ByRef res As DCSC.LoadExposureList.Response,
                                     ByRef req As DCSC.LoadExposureList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadExposureList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFeature(ByRef res As DCSC.LoadFeature.Response,
                                     ByRef req As DCSC.LoadFeature.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFeature
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFeatureDefault(ByRef res As DCSC.LoadFeatureDefault.Response,
                                     ByRef req As DCSC.LoadFeatureDefault.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFeatureDefault
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFeatureInitialReserves(ByRef res As DCSC.LoadFeatureInitialReserves.Response,
                                     ByRef req As DCSC.LoadFeatureInitialReserves.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFeatureInitialReserves
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFeatureList(ByRef res As DCSC.LoadFeatureList.Response,
                                     ByRef req As DCSC.LoadFeatureList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFeatureList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFeaturePersonnel(ByRef res As DCSC.LoadFeaturePersonnel.Response,
                                     ByRef req As DCSC.LoadFeaturePersonnel.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFeaturePersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFinancials(ByRef res As DCSC.LoadFinancials.Response,
                                     ByRef req As DCSC.LoadFinancials.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFinancials
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFinancialsForClaimant(ByRef res As DCSC.LoadFinancialsForClaimant.Response,
                                     ByRef req As DCSC.LoadFinancialsForClaimant.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFinancialsForClaimant
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFinancialsForClaimControl(ByRef res As DCSC.LoadFinancialsForClaimControl.Response,
                                     ByRef req As DCSC.LoadFinancialsForClaimControl.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFinancialsForClaimControl
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadFinancialsForFeature(ByRef res As DCSC.LoadFinancialsForFeature.Response,
                                     ByRef req As DCSC.LoadFinancialsForFeature.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadFinancialsForFeature
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInitialReserve(ByRef res As DCSC.LoadInitialReserve.Response,
                                     ByRef req As DCSC.LoadInitialReserve.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInitialReserve
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInitialReserveList(ByRef res As DCSC.LoadInitialReserveList.Response,
                                     ByRef req As DCSC.LoadInitialReserveList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInitialReserveList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInsured(ByRef res As DCSC.LoadInsured.Response,
                                     ByRef req As DCSC.LoadInsured.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInsured
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInsuredClaimPayeeId(ByRef res As DCSC.LoadInsuredClaimPayeeId.Response,
                                     ByRef req As DCSC.LoadInsuredClaimPayeeId.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInsuredClaimPayeeId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInsuredInfo(ByRef res As DCSC.LoadInsuredInfo.Response,
                                     ByRef req As DCSC.LoadInsuredInfo.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInsuredInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadInsuredSubmissionQuestions(ByRef res As DCSC.LoadInsuredSubmissionQuestions.Response,
                                     ByRef req As DCSC.LoadInsuredSubmissionQuestions.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadInsuredSubmissionQuestions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLegalName(ByRef res As DCSC.LoadLegalName.Response,
                                     ByRef req As DCSC.LoadLegalName.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLegalName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLevyList(ByRef res As DCSC.LoadLevyList.Response,
                                     ByRef req As DCSC.LoadLevyList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLevyList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadListedClaimantData(ByRef res As DCSC.LoadListedClaimantData.Response,
                                     ByRef req As DCSC.LoadListedClaimantData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadListedClaimantData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadListedClaimantsAndDrivers(ByRef res As DCSC.LoadListedClaimantsAndDrivers.Response,
                                     ByRef req As DCSC.LoadListedClaimantsAndDrivers.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadListedClaimantsAndDrivers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadListedPerson(ByRef res As DCSC.LoadListedPerson.Response,
                                     ByRef req As DCSC.LoadListedPerson.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadListedPerson
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadListedPersonData(ByRef res As DCSC.LoadListedPersonData.Response,
                                     ByRef req As DCSC.LoadListedPersonData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadListedPersonData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLitigation(ByRef res As DCSC.LoadLitigation.Response,
                                     ByRef req As DCSC.LoadLitigation.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLitigation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLitigationList(ByRef res As DCSC.LoadLitigationList.Response,
                                     ByRef req As DCSC.LoadLitigationList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLitigationList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLitigationPersonnelByType(ByRef res As DCSC.LoadLitigationPersonnelByType.Response,
                                     ByRef req As DCSC.LoadLitigationPersonnelByType.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLitigationPersonnelByType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLossNotice(ByRef res As DCSC.LoadLossNotice.Response,
                                     ByRef req As DCSC.LoadLossNotice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLossNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadNotifyUnderwritingDetail(ByRef res As DCSC.LoadNotifyUnderwritingDetail.Response,
                                     ByRef req As DCSC.LoadNotifyUnderwritingDetail.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadNotifyUnderwritingDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadNotifyUnderwritingList(ByRef res As DCSC.LoadNotifyUnderwritingList.Response,
                                     ByRef req As DCSC.LoadNotifyUnderwritingList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadNotifyUnderwritingList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadOfficerClaimPolice(ByRef res As DCSC.LoadOfficerClaimPolice.Response,
                                     ByRef req As DCSC.LoadOfficerClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadOfficerClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadOfficerListClaimPolice(ByRef res As DCSC.LoadOfficerListClaimPolice.Response,
                                     ByRef req As DCSC.LoadOfficerListClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadOfficerListClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadOfficerMaintenanceClaimPolice(ByRef res As DCSC.LoadOfficerMaintenanceClaimPolice.Response,
                                     ByRef req As DCSC.LoadOfficerMaintenanceClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadOfficerMaintenanceClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayee(ByRef res As DCSC.LoadPayee.Response,
                                     ByRef req As DCSC.LoadPayee.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPayee
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayeeList(ByRef res As DCSC.LoadPayeeList.Response,
                                     ByRef req As DCSC.LoadPayeeList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPayeeList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayTypes(ByRef res As DCSC.LoadPayTypes.Response,
                                     ByRef req As DCSC.LoadPayTypes.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPayTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPendingClaimsList(ByRef res As DCSC.LoadPendingClaimsList.Response,
                                     ByRef req As DCSC.LoadPendingClaimsList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPendingClaimsList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function LoadPolicyImageForClaimForms(ByRef res As DCSC.LoadPolicyImageForClaimForms.Response,
        '                             ByRef req As DCSC.LoadPolicyImageForClaimForms.Request,
        '                             Optional ByRef e As Exception = Nothing,
        '                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.ClaimsServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.LoadPolicyImageForClaimForms
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadRandomPolicyImage(ByRef res As DCSC.LoadRandomPolicyImage.Response,
                                     ByRef req As DCSC.LoadRandomPolicyImage.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRandomPolicyImage
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRecentClaimListForUser(ByRef res As DCSC.LoadRecentClaimListForUser.Response,
                                     ByRef req As DCSC.LoadRecentClaimListForUser.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRecentClaimListForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRepairShopDetails(ByRef res As DCSC.LoadRepairShopDetails.Response,
                                     ByRef req As DCSC.LoadRepairShopDetails.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRepairShopDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRepairShopList(ByRef res As DCSC.LoadRepairShopList.Response,
                                     ByRef req As DCSC.LoadRepairShopList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRepairShopList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadScheduledPaymentCycle(ByRef res As DCSC.LoadScheduledPaymentCycle.Response,
                                     ByRef req As DCSC.LoadScheduledPaymentCycle.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadScheduledPaymentCycle
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadScheduledPaymentCycleList(ByRef res As DCSC.LoadScheduledPaymentCycleList.Response,
                                     ByRef req As DCSC.LoadScheduledPaymentCycleList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadScheduledPaymentCycleList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadScheduledPaymentCycleTransactions(ByRef res As DCSC.LoadScheduledPaymentCycleTransactions.Response,
                                     ByRef req As DCSC.LoadScheduledPaymentCycleTransactions.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadScheduledPaymentCycleTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSplitTransactions(ByRef res As DCSC.LoadSplitTransactions.Response,
                                     ByRef req As DCSC.LoadSplitTransactions.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSplitTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadStopPayment(ByRef res As DCSC.LoadStopPayment.Response,
                                     ByRef req As DCSC.LoadStopPayment.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadStopPayment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadStressClaimants(ByRef res As DCSC.LoadStressClaimants.Response,
                                     ByRef req As DCSC.LoadStressClaimants.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadStressClaimants
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadStressClaimControl(ByRef res As DCSC.LoadStressClaimControl.Response,
                                     ByRef req As DCSC.LoadStressClaimControl.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadStressClaimControl
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSubCoverageList(ByRef res As DCSC.LoadSubCoverageList.Response,
                                     ByRef req As DCSC.LoadSubCoverageList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSubCoverageList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSubCoverages(ByRef res As DCSC.LoadSubCoverages.Response,
                                     ByRef req As DCSC.LoadSubCoverages.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSubCoverages
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSubExposureList(ByRef res As DCSC.LoadSubExposureList.Response,
                                     ByRef req As DCSC.LoadSubExposureList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSubExposureList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTabSettings(ByRef res As DCSC.LoadTabSettings.Response,
                                     ByRef req As DCSC.LoadTabSettings.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTabSettings
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTransaction(ByRef res As DCSC.LoadTransaction.Response,
                                     ByRef req As DCSC.LoadTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTransactionList(ByRef res As DCSC.LoadTransactionList.Response,
                                     ByRef req As DCSC.LoadTransactionList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTransactionList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTransactionPayeeList(ByRef res As DCSC.LoadTransactionPayeeList.Response,
                                     ByRef req As DCSC.LoadTransactionPayeeList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTransactionPayeeList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadTransactionTypes(ByRef res As DCSC.LoadTransactionTypes.Response,
                                     ByRef req As DCSC.LoadTransactionTypes.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadTransactionTypes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadWitness(ByRef res As DCSC.LoadWitness.Response,
                                     ByRef req As DCSC.LoadWitness.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadWitness
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadWitnessForLossNotice(ByRef res As DCSC.LoadWitnessForLossNotice.Response,
                                     ByRef req As DCSC.LoadWitnessForLossNotice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadWitnessForLossNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadWitnessList(ByRef res As DCSC.LoadWitnessList.Response,
                                     ByRef req As DCSC.LoadWitnessList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadWitnessList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadWitnessListForLossNotice(ByRef res As DCSC.LoadWitnessListForLossNotice.Response,
                                     ByRef req As DCSC.LoadWitnessListForLossNotice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadWitnessListForLossNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function OpenClaimantRecord(ByRef res As DCSC.OpenClaimantRecord.Response,
                                     ByRef req As DCSC.OpenClaimantRecord.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OpenClaimantRecord
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function OpenClaimRecord(ByRef res As DCSC.OpenClaimRecord.Response,
                                     ByRef req As DCSC.OpenClaimRecord.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OpenClaimRecord
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function OpenFeatureRecord(ByRef res As DCSC.OpenFeatureRecord.Response,
                                     ByRef req As DCSC.OpenFeatureRecord.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.OpenFeatureRecord
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ParsePTTOOString(ByRef res As DCSC.ParsePTTOOString.Response,
                                     ByRef req As DCSC.ParsePTTOOString.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ParsePTTOOString
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PersonnelAccessRestricted(ByRef res As DCSC.PersonnelAccessRestricted.Response,
                                     ByRef req As DCSC.PersonnelAccessRestricted.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.PersonnelAccessRestricted
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function PrintManualForm(ByRef res As DCSC.PrintManualForm.Response,
        '                             ByRef req As DCSC.PrintManualForm.Request,
        '                             Optional ByRef e As Exception = Nothing,
        '                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.ClaimsServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.PrintManualForm
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function ProcessMedicareClaimFileResults(ByRef res As DCSC.ProcessMedicareClaimFileResults.Response,
                                     ByRef req As DCSC.ProcessMedicareClaimFileResults.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessMedicareClaimFileResults
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessMedicareQueryResults(ByRef res As DCSC.ProcessMedicareQueryResults.Response,
                                     ByRef req As DCSC.ProcessMedicareQueryResults.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessMedicareQueryResults
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function QueryPayeeList(ByRef res As DCSC.QueryPayeeList.Response,
                                     ByRef req As DCSC.QueryPayeeList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.QueryPayeeList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RefreshFinancials(ByRef res As DCSC.RefreshFinancials.Response,
                                     ByRef req As DCSC.RefreshFinancials.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RefreshFinancials
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReinstateVoidTransaction(ByRef res As DCSC.ReinstateVoidTransaction.Response,
                                     ByRef req As DCSC.ReinstateVoidTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReinstateVoidTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReissueTransaction(ByRef res As DCSC.ReissueTransaction.Response,
                                     ByRef req As DCSC.ReissueTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReissueTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAdjustedCoverages(ByRef res As DCSC.SaveAdjustedCoverages.Response,
                                     ByRef req As DCSC.SaveAdjustedCoverages.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAdjustedCoverages
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAdjuster(ByRef res As DCSC.SaveAdjuster.Response,
                                     ByRef req As DCSC.SaveAdjuster.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAdjuster
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAppraiser(ByRef res As DCSC.SaveAppraiser.Response,
                                     ByRef req As DCSC.SaveAppraiser.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAppraiser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBuildPTTOO(ByRef res As DCSC.SaveBuildPTTOO.Response,
                                     ByRef req As DCSC.SaveBuildPTTOO.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBuildPTTOO
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCareProvider(ByRef res As DCSC.SaveCareProvider.Response,
                                     ByRef req As DCSC.SaveCareProvider.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCareProvider
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCarrier(ByRef res As DCSC.SaveCarrier.Response,
                                     ByRef req As DCSC.SaveCarrier.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCarrier
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCatastrophe(ByRef res As DCSC.SaveCatastrophe.Response,
                                     ByRef req As DCSC.SaveCatastrophe.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCatastrophe
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimant(ByRef res As DCSC.SaveClaimant.Response,
                                     ByRef req As DCSC.SaveClaimant.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimant
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimantVehicleNum(ByRef res As DCSC.SaveClaimantVehicleNum.Response,
                                     ByRef req As DCSC.SaveClaimantVehicleNum.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimantVehicleNum
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimControlPersonnel(ByRef res As DCSC.SaveClaimControlPersonnel.Response,
                                     ByRef req As DCSC.SaveClaimControlPersonnel.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimControlPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimControlProperties(ByRef res As DCSC.SaveClaimControlProperties.Response,
                                     ByRef req As DCSC.SaveClaimControlProperties.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimControlProperties
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimControlPropertyAppraisal(ByRef res As DCSC.SaveClaimControlPropertyAppraisal.Response,
                                     ByRef req As DCSC.SaveClaimControlPropertyAppraisal.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimControlPropertyAppraisal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimControlScreenData(ByRef res As DCSC.SaveClaimControlScreenData.Response,
                                     ByRef req As DCSC.SaveClaimControlScreenData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimControlScreenData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimControlVehicleAppraisal(ByRef res As DCSC.SaveClaimControlVehicleAppraisal.Response,
                                     ByRef req As DCSC.SaveClaimControlVehicleAppraisal.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimControlVehicleAppraisal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimControlVehicles(ByRef res As DCSC.SaveClaimControlVehicles.Response,
                                     ByRef req As DCSC.SaveClaimControlVehicles.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimControlVehicles
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimExposure(ByRef res As DCSC.SaveClaimExposure.Response,
                                     ByRef req As DCSC.SaveClaimExposure.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimExposure
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimFirm(ByRef res As DCSC.SaveClaimFirm.Response,
                                     ByRef req As DCSC.SaveClaimFirm.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimFirm
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimFirmAttorney(ByRef res As DCSC.SaveClaimFirmAttorney.Response,
                                     ByRef req As DCSC.SaveClaimFirmAttorney.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimFirmAttorney
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimNumberFields(ByRef res As DCSC.SaveClaimNumberFields.Response,
                                     ByRef req As DCSC.SaveClaimNumberFields.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimNumberFields
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimOffice(ByRef res As DCSC.SaveClaimOffice.Response,
                                     ByRef req As DCSC.SaveClaimOffice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimOffice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimPayeeChangeReason(ByRef res As DCSC.SaveClaimPayeeChangeReason.Response,
                                     ByRef req As DCSC.SaveClaimPayeeChangeReason.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimPayeeChangeReason
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimPayeePhrase(ByRef res As DCSC.SaveClaimPayeePhrase.Response,
                                     ByRef req As DCSC.SaveClaimPayeePhrase.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimPayeePhrase
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveClaimPersonnel(ByRef res As DCSC.SaveClaimPersonnel.Response,
                                     ByRef req As DCSC.SaveClaimPersonnel.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveClaimPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDashboardTemplate(ByRef res As DCSC.SaveDashboardTemplate.Response,
                                     ByRef req As DCSC.SaveDashboardTemplate.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDashboardTemplate
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDCPolicy(ByRef res As DCSC.SaveDCPolicy.Response,
                                     ByRef req As DCSC.SaveDCPolicy.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDCPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDeptClaimFire(ByRef res As DCSC.SaveDeptClaimFire.Response,
                                     ByRef req As DCSC.SaveDeptClaimFire.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDeptClaimFire
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveDeptClaimPolice(ByRef res As DCSC.SaveDeptClaimPolice.Response,
                                     ByRef req As DCSC.SaveDeptClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveDeptClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveFeature(ByRef res As DCSC.SaveFeature.Response,
                                     ByRef req As DCSC.SaveFeature.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveFeature
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveFeatureDefault(ByRef res As DCSC.SaveFeatureDefault.Response,
                                     ByRef req As DCSC.SaveFeatureDefault.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveFeatureDefault
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveInitialReserve(ByRef res As DCSC.SaveInitialReserve.Response,
                                     ByRef req As DCSC.SaveInitialReserve.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveInitialReserve
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveInitialTransactions(ByRef res As DCSC.SaveInitialTransactions.Response,
                                     ByRef req As DCSC.SaveInitialTransactions.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveInitialTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveInitialTransactionsAll(ByRef res As DCSC.SaveInitialTransactionsAll.Response,
                                     ByRef req As DCSC.SaveInitialTransactionsAll.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveInitialTransactionsAll
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLevy(ByRef res As DCSC.SaveLevy.Response,
                                     ByRef req As DCSC.SaveLevy.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLevy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLitigation(ByRef res As DCSC.SaveLitigation.Response,
                                     ByRef req As DCSC.SaveLitigation.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLitigation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveNotifyUnderwritingDetail(ByRef res As DCSC.SaveNotifyUnderwritingDetail.Response,
                                     ByRef req As DCSC.SaveNotifyUnderwritingDetail.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveNotifyUnderwritingDetail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveOfficerClaimPolice(ByRef res As DCSC.SaveOfficerClaimPolice.Response,
                                     ByRef req As DCSC.SaveOfficerClaimPolice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveOfficerClaimPolice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePayee(ByRef res As DCSC.SavePayee.Response,
                                     ByRef req As DCSC.SavePayee.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePayee
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveRecentClaimForUser(ByRef res As DCSC.SaveRecentClaimForUser.Response,
                                     ByRef req As DCSC.SaveRecentClaimForUser.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveRecentClaimForUser
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveRepairShop(ByRef res As DCSC.SaveRepairShop.Response,
                                     ByRef req As DCSC.SaveRepairShop.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveRepairShop
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveScheduledPaymentCycle(ByRef res As DCSC.SaveScheduledPaymentCycle.Response,
                                     ByRef req As DCSC.SaveScheduledPaymentCycle.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveScheduledPaymentCycle
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSecondaryTransactions(ByRef res As DCSC.SaveSecondaryTransactions.Response,
                                     ByRef req As DCSC.SaveSecondaryTransactions.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSecondaryTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSecondaryTransactionsAll(ByRef res As DCSC.SaveSecondaryTransactionsAll.Response,
                                     ByRef req As DCSC.SaveSecondaryTransactionsAll.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSecondaryTransactionsAll
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSplitTransactions(ByRef res As DCSC.SaveSplitTransactions.Response,
                                     ByRef req As DCSC.SaveSplitTransactions.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSplitTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveStopPaymentData(ByRef res As DCSC.SaveStopPaymentData.Response,
                                     ByRef req As DCSC.SaveStopPaymentData.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveStopPaymentData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveTransaction(ByRef res As DCSC.SaveTransaction.Response,
                                     ByRef req As DCSC.SaveTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveWitness(ByRef res As DCSC.SaveWitness.Response,
                                     ByRef req As DCSC.SaveWitness.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveWitness
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SubmitInsuredSubmission(ByRef res As DCSC.SubmitInsuredSubmission.Response,
                                     ByRef req As DCSC.SubmitInsuredSubmission.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SubmitInsuredSubmission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SubmitLossNotice(ByRef res As DCSC.SubmitLossNotice.Response,
                                     ByRef req As DCSC.SubmitLossNotice.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SubmitLossNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateClaimSubmission(ByRef res As DCSC.UpdateClaimSubmission.Response,
                                     ByRef req As DCSC.UpdateClaimSubmission.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateClaimSubmission
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateFeature(ByRef res As DCSC.UpdateFeature.Response,
                                     ByRef req As DCSC.UpdateFeature.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateFeature
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdatePayeeSubType(ByRef res As DCSC.UpdatePayeeSubType.Response,
                                     ByRef req As DCSC.UpdatePayeeSubType.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdatePayeeSubType
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateReissue(ByRef res As DCSC.UpdateReissue.Response,
                                     ByRef req As DCSC.UpdateReissue.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateReissue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateSplitTransactionAmount(ByRef res As DCSC.UpdateSplitTransactionAmount.Response,
                                     ByRef req As DCSC.UpdateSplitTransactionAmount.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateSplitTransactionAmount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateStopPaymentStatus(ByRef res As DCSC.UpdateStopPaymentStatus.Response,
                                     ByRef req As DCSC.UpdateStopPaymentStatus.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateStopPaymentStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateTransaction(ByRef res As DCSC.UpdateTransaction.Response,
                                     ByRef req As DCSC.UpdateTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Validate3rdParty(ByRef res As DCSC.Validate3rdParty.Response,
                                     ByRef req As DCSC.Validate3rdParty.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Validate3rdParty
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateBuildPTTOO(ByRef res As DCSC.ValidateBuildPTTOO.Response,
                                     ByRef req As DCSC.ValidateBuildPTTOO.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateBuildPTTOO
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateClaimant(ByRef res As DCSC.ValidateClaimant.Response,
                                     ByRef req As DCSC.ValidateClaimant.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateClaimant
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateClaimControlProperty(ByRef res As DCSC.ValidateClaimControlProperty.Response,
                                     ByRef req As DCSC.ValidateClaimControlProperty.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateClaimControlProperty
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateClaimControlVehicle(ByRef res As DCSC.ValidateClaimControlVehicle.Response,
                                     ByRef req As DCSC.ValidateClaimControlVehicle.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateClaimControlVehicle
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateClaimNumber(ByRef res As DCSC.ValidateClaimNumber.Response,
                                     ByRef req As DCSC.ValidateClaimNumber.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateClaimNumber
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateConstant(ByRef res As DCSC.ValidateConstant.Response,
                                     ByRef req As DCSC.ValidateConstant.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateConstant
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateLossNoticeProperty(ByRef res As DCSC.ValidateLossNoticeProperty.Response,
                                     ByRef req As DCSC.ValidateLossNoticeProperty.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateLossNoticeProperty
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateLossNoticePropertyAppraisal(ByRef res As DCSC.ValidateLossNoticePropertyAppraisal.Response,
                                     ByRef req As DCSC.ValidateLossNoticePropertyAppraisal.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateLossNoticePropertyAppraisal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateLossNoticeVehicle(ByRef res As DCSC.ValidateLossNoticeVehicle.Response,
                                     ByRef req As DCSC.ValidateLossNoticeVehicle.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateLossNoticeVehicle
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateLossNoticeVehicleAppraisal(ByRef res As DCSC.ValidateLossNoticeVehicleAppraisal.Response,
                                     ByRef req As DCSC.ValidateLossNoticeVehicleAppraisal.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateLossNoticeVehicleAppraisal
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateNewClaimOccurrence(ByRef res As DCSC.ValidateNewClaimOccurrence.Response,
                                     ByRef req As DCSC.ValidateNewClaimOccurrence.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateNewClaimOccurrence
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateSequence(ByRef res As DCSC.ValidateSequence.Response,
                                     ByRef req As DCSC.ValidateSequence.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateSequence
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateSortOrder(ByRef res As DCSC.ValidateSortOrder.Response,
                                     ByRef req As DCSC.ValidateSortOrder.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateSortOrder
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ValidateWitnessForLossNotice(ByRef res As DCSC.ValidateWitness.Response,
                                     ByRef req As DCSC.ValidateWitness.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ValidateWitnessForLossNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function VoidTransaction(ByRef res As DCSC.VoidTransaction.Response,
                                     ByRef req As DCSC.VoidTransaction.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.VoidTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ZeroClaimantReserves(ByRef res As DCSC.ZeroClaimantReserves.Response,
                                     ByRef req As DCSC.ZeroClaimantReserves.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ZeroClaimantReserves
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ZeroClaimReserves(ByRef res As DCSC.ZeroClaimReserves.Response,
                                     ByRef req As DCSC.ZeroClaimReserves.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ZeroClaimReserves
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ZeroFeatureReserves(ByRef res As DCSC.ZeroFeatureReserves.Response,
                                     ByRef req As DCSC.ZeroFeatureReserves.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ClaimsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ZeroFeatureReserves
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

