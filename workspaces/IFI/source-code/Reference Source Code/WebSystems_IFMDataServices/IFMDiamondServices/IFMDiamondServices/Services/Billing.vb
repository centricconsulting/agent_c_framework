Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSB = Diamond.Common.Services.Messages.BillingService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Billing
    Public Module Billing
        Public Function ApplyCash(cash As DCO.Billing.ApplyCash,
                                  ByRef response As DCS.Messages.ResponseBase,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCS.Proxies.BillingServiceProxy
            Dim m As pMethod = AddressOf p.ApplyCash
            Dim req As New DCS.Messages.BillingService.ApplyCash.Request
            req.RequestData.ApplyCash = cash
            response = RunDiamondService(m, req)
            If response IsNot Nothing Then
                Return True
            End If
            Return False
        End Function
        Public Function AddLegalNotice(ByRef res As DCSB.AddLegalNotice.Response,
                                       ByRef req As DCSB.AddLegalNotice.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddLegalNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function AddReminderNotice(ByRef res As DCSB.AddReminderNotice.Response,
                                       ByRef req As DCSB.AddReminderNotice.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddReminderNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ApplyBillingAccountAdjustment(ByRef res As DCSB.ApplyBillingAccountAdjustment.Response,
                                       ByRef req As DCSB.ApplyBillingAccountAdjustment.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ApplyBillingAccountAdjustment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ApplyBillingAccountPayment(ByRef res As DCSB.ApplyBillingAccountPayment.Response,
                                       ByRef req As DCSB.ApplyBillingAccountPayment.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ApplyBillingAccountPayment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ApplyCash(ByRef res As DCSB.ApplyCash.Response,
                                       ByRef req As DCSB.ApplyCash.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ApplyCash
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ApplyCharge(ByRef res As DCSB.ApplyCharge.Response,
                                       ByRef req As DCSB.ApplyCharge.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ApplyCharge
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ApplyCredit(ByRef res As DCSB.ApplyCredit.Response,
                                       ByRef req As DCSB.ApplyCredit.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ApplyCredit
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckForValidPercents(ByRef res As DCSB.CheckForValidPercents.Response,
                                       ByRef req As DCSB.CheckForValidPercents.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckForValidPercents
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateMultipleShortPreviewInvoices(ByRef res As DCSB.CreateMultipleShortPreviewInvoices.Response,
                                       ByRef req As DCSB.CreateMultipleShortPreviewInvoices.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateMultipleShortPreviewInvoices
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCash(ByRef res As DCSB.DeleteCash.Response,
                                       ByRef req As DCSB.DeleteCash.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCash
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteChargeCredit(ByRef res As DCSB.DeleteChargeCredit.Response,
                                       ByRef req As DCSB.DeleteChargeCredit.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteChargeCredit
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteLegalNotice(ByRef res As DCSB.DeleteLegalNotice.Response,
                                       ByRef req As DCSB.DeleteLegalNotice.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteLegalNotice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function EditBillingInvoice(ByRef res As DCSB.EditBillingInvoice.Response,
                                       ByRef req As DCSB.EditBillingInvoice.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.EditBillingInvoice
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAccountBalance(ByRef res As DCSB.GetAccountBalance.Response,
                                       ByRef req As DCSB.GetAccountBalance.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAccountBalance
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAccountInfo(ByRef res As DCSB.GetAccountInfo.Response,
                                       ByRef req As DCSB.GetAccountInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAccountInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyBillActivity(ByRef res As DCSB.GetAgencyBillActivity.Response,
                                       ByRef req As DCSB.GetAgencyBillActivity.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyBillActivity
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyStatementCommissionInfo(ByRef res As DCSB.GetAgencyStatementCommissionInfo.Response,
                                       ByRef req As DCSB.GetAgencyStatementCommissionInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyStatementCommissionInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAutomaticRefundCheckPayeeInfo(ByRef res As DCSB.GetAutomaticRefundCheckPayeeInfo.Response,
                                       ByRef req As DCSB.GetAutomaticRefundCheckPayeeInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAutomaticRefundCheckPayeeInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingAccountCashInfo(ByRef res As DCSB.GetBillingAccountCashInfo.Response,
                                       ByRef req As DCSB.GetBillingAccountCashInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingAccountCashInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingAccountRebalanceInfo(ByRef res As DCSB.GetBillingAccountRebalanceInfo.Response,
                                       ByRef req As DCSB.GetBillingAccountRebalanceInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingAccountRebalanceInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingBusinessData(ByRef res As DCSB.GetBillingBusinessData.Response,
                                       ByRef req As DCSB.GetBillingBusinessData.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingBusinessData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingDataFromBusinessData(ByRef res As DCSB.GetBillingDataFromBusinessData.Response,
                                       ByRef req As DCSB.GetBillingDataFromBusinessData.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingDataFromBusinessData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingHistory(ByRef res As DCSB.GetBillingHistory.Response,
                                       ByRef req As DCSB.GetBillingHistory.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingPayPlanInstallment(ByRef res As DCSB.GetBillingPayPlanInstallment.Response,
                                       ByRef req As DCSB.GetBillingPayPlanInstallment.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingPayPlanInstallment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetBillingSummary(ByRef res As DCSB.GetBillingSummary.Response,
                                       ByRef req As DCSB.GetBillingSummary.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetBillingSummary
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCashInfo(ByRef res As DCSB.GetCashInfo.Response,
                                       ByRef req As DCSB.GetCashInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetCashInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetChargeInfo(ByRef res As DCSB.GetChargeInfo.Response,
                                       ByRef req As DCSB.GetChargeInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetChargeInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCheckPayeeInfo(ByRef res As DCSB.GetCheckPayeeInfo.Response,
                                       ByRef req As DCSB.GetCheckPayeeInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetCheckPayeeInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCommissionInfo(ByRef res As DCSB.GetCommissionInfo.Response,
                                       ByRef req As DCSB.GetCommissionInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetCommissionInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetFutureCash(ByRef res As DCSB.GetFutureCash.Response,
                                       ByRef req As DCSB.GetFutureCash.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetFutureCash
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetMiscellaneousChargeCashDetails(ByRef res As DCSB.GetMiscellaneousChargeCashDetails.Response,
                                       ByRef req As DCSB.GetMiscellaneousChargeCashDetails.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMiscellaneousChargeCashDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPolicyCreditCardInfo(ByRef res As DCSB.GetPolicyCreditCardInfo.Response,
                                       ByRef req As DCSB.GetPolicyCreditCardInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPolicyCreditCardInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPolicyEftInfo(ByRef res As DCSB.GetPolicyEftInfo.Response,
                                       ByRef req As DCSB.GetPolicyEftInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPolicyEftInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetReceiptInfo(ByRef res As DCSB.GetReceiptInfo.Response,
                                       ByRef req As DCSB.GetReceiptInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetReceiptInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetRenewalOfferInfo(ByRef res As DCSB.GetRenewalOfferInfo.Response,
                                       ByRef req As DCSB.GetRenewalOfferInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetRenewalOfferInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSingleBillingPayPlan(ByRef res As DCSB.GetSingleBillingPayPlan.Response,
                                       ByRef req As DCSB.GetSingleBillingPayPlan.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetSingleBillingPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function HasPolicyHadCashApplied(ByRef res As DCSB.HasPolicyHadCashApplied.Response,
                                       ByRef req As DCSB.HasPolicyHadCashApplied.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.HasPolicyHadCashApplied
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsDueDateChangeAllowed(ByRef res As DCSB.IsDueDateChangeAllowed.Response,
                                       ByRef req As DCSB.IsDueDateChangeAllowed.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsDueDateChangeAllowed
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsPayPlanChangeAllowed(ByRef res As DCSB.IsPayPlanChangeAllowed.Response,
                                       ByRef req As DCSB.IsPayPlanChangeAllowed.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IsPayPlanChangeAllowed
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IssueTransaction(ByRef res As DCSB.IssueTransaction.Response,
                                       ByRef req As DCSB.IssueTransaction.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.IssueTransaction
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Load(ByRef res As DCSB.Load.Response,
                                       ByRef req As DCSB.Load.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.Load
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBillingAccountsByClient(ByRef res As DCSB.LoadBillingAccountsByClient.Response,
                                       ByRef req As DCSB.LoadBillingAccountsByClient.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillingAccountsByClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBillingPayPlan(ByRef res As DCSB.LoadBillingPayPlan.Response,
                                       ByRef req As DCSB.LoadBillingPayPlan.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillingPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function LoadBillingPayPlanBillMethodVersion(ByRef res As DCSB.LoadBillingPayPlanBillMethodVersion.Response,
        '                               ByRef req As DCSB.LoadBillingPayPlanBillMethodVersion.Request,
        '                               Optional ByRef e As Exception = Nothing,
        '                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.BillingServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.LoadBillingPayPlanBillMethodVersion
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function LoadBillingPayPlanInstallment(ByRef res As DCSB.LoadBillingPayPlanInstallment.Response,
                                       ByRef req As DCSB.LoadBillingPayPlanInstallment.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillingPayPlanInstallment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBillingSummary(ByRef res As DCSB.LoadBillingSummary.Response,
                                       ByRef req As DCSB.LoadBillingSummary.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillingSummary
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBillTo(ByRef res As DCSB.LoadBillTo.Response,
                                       ByRef req As DCSB.LoadBillTo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillTo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadMortgagees(ByRef res As DCSB.LoadMortgagees.Response,
                                       ByRef req As DCSB.LoadMortgagees.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadMortgagees
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayrollDeductionInfo(ByRef res As DCSB.LoadPayrollDeductionInfo.Response,
                                       ByRef req As DCSB.LoadPayrollDeductionInfo.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPayrollDeductionInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPoliciesByClient(ByRef res As DCSB.LoadPoliciesByClient.Response,
                                                 ByRef req As DCSB.LoadPoliciesByClient.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPoliciesByClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPolicyAgencyBillTo(ByRef res As DCSB.LoadPolicyAgencyBillTo.Response,
                                                 ByRef req As DCSB.LoadPolicyAgencyBillTo.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPolicyAgencyBillTo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPreview(ByRef res As DCSB.LoadPreview.Response,
                                                 ByRef req As DCSB.LoadPreview.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPreview
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleBillingPayPlan(ByRef res As DCSB.LoadSingleBillingPayPlan.Response,
                                                 ByRef req As DCSB.LoadSingleBillingPayPlan.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleBillingPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSingleBillingPayPlanInstallment(ByRef res As DCSB.LoadSingleBillingPayPlanInstallment.Response,
                                                 ByRef req As DCSB.LoadSingleBillingPayPlanInstallment.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSingleBillingPayPlanInstallment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessRefundCheck(ByRef res As DCSB.ProcessRefundCheck.Response,
                                                 ByRef req As DCSB.ProcessRefundCheck.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessRefundCheck
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RecreateInstallments(ByRef res As DCSB.RecreateInstallments.Response,
                                                 ByRef req As DCSB.RecreateInstallments.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.RecreateInstallments
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReversalCashAdjustment(ByRef res As DCSB.ReversalCashAdjustment.Response,
                                                 ByRef req As DCSB.ReversalCashAdjustment.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReversalCashAdjustment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReversalChargesCreditsAdjustment(ByRef res As DCSB.ReversalChargesCreditsAdjustment.Response,
                                                 ByRef req As DCSB.ReversalChargesCreditsAdjustment.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReversalChargesCreditsAdjustment
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBillingInfo(ByRef res As DCSB.SaveBillingInfo.Response,
                                                 ByRef req As DCSB.SaveBillingInfo.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBillingInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function SaveBillingPayPlanBillMethodVersion(ByRef res As DCSB.SaveBillingPayPlanBillMethodVersion.Response,
        '                                         ByRef req As DCSB.SaveBillingPayPlanBillMethodVersion.Request,
        '                                         Optional ByRef e As Exception = Nothing,
        '                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.BillingServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.SaveBillingPayPlanBillMethodVersion
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function SaveBillMethodVersion(ByRef res As DCSB.SaveBillMethodVersion.Response,
                                                 ByRef req As DCSB.SaveBillMethodVersion.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBillMethodVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveBillTo(ByRef res As DCSB.SaveBillTo.Response,
                                                 ByRef req As DCSB.SaveBillTo.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveBillTo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SavePayrollDeductionInfo(ByRef res As DCSB.SavePayrollDeductionInfo.Response,
                                                 ByRef req As DCSB.SavePayrollDeductionInfo.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePayrollDeductionInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePolicyCreditCardInfo(ByRef res As DCSB.SavePolicyCreditCardInfo.Response,
                                                 ByRef req As DCSB.SavePolicyCreditCardInfo.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePolicyCreditCardInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePolicyEftInfo(ByRef res As DCSB.SavePolicyEftInfo.Response,
                                                 ByRef req As DCSB.SavePolicyEftInfo.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePolicyEftInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateAccountPayPlan(ByRef res As DCSB.UpdateAccountPayPlan.Response,
                                                 ByRef req As DCSB.UpdateAccountPayPlan.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateAccountPayPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateBillingRefundOptionPerClient(ByRef res As DCSB.UpdateBillingRefundOptionPerClient.Response,
                                                 ByRef req As DCSB.UpdateBillingRefundOptionPerClient.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateBillingRefundOptionPerClient
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateMultipleEFTAccounts(ByRef res As DCSB.UpdateMultipleEFTAccounts.Response,
                                                 ByRef req As DCSB.UpdateMultipleEFTAccounts.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateMultipleEFTAccounts
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        'Public Function ValidateDuplicateAccountPayments(ByRef res As DCSB.ValidateDuplicateAccountPayments.Response,
        '                                         ByRef req As DCSB.ValidateDuplicateAccountPayments.Request,
        '                                         Optional ByRef e As Exception = Nothing,
        '                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.BillingServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.ValidateDuplicateAccountPayments
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        'Public Function ValidateDuplicatePayments(ByRef res As DCSB.ValidateDuplicatePayments.Response,
        '                                         ByRef req As DCSB.ValidateDuplicatePayments.Request,
        '                                         Optional ByRef e As Exception = Nothing,
        '                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
        '    Dim p As New DCSP.BillingServiceProxy
        '    Dim m As Services.Common.pMethod = AddressOf p.ValidateDuplicatePayments
        '    res = RunDiamondService(m, req, e, dv)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        Public Function VerifyBilling(ByRef res As DCSB.VerifyBilling.Response,
                                                 ByRef req As DCSB.VerifyBilling.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.VerifyBilling
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function VoidRefundCheck(ByRef res As DCSB.VoidRefundCheck.Response,
                                                 ByRef req As DCSB.VoidRefundCheck.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.VoidRefundCheck
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
