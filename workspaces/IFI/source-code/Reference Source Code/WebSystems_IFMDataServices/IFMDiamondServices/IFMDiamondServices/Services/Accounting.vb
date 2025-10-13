'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSA = Diamond.Common.Services.Messages.AccountingService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.Accounting
    Friend Module Accounting
        Public Function ApplySuspenseCash(ByRef res As DCSA.ApplySuspenseCash.Response,
                                          ByRef req As DCSA.ApplySuspenseCash.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Using p As New DCSP.AccountingServiceProxy
                Dim m As Services.Common.pMethod = AddressOf p.ApplySuspenseCash
                res = RunDiamondService(m, req, e)
                If res IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Using
        End Function
        Public Function BeginLockboxFileImport(ByRef res As DCSA.BeginLockboxFileImport.Response,
                                               ByRef req As DCSA.BeginLockboxFileImport.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Using p As New DCSP.AccountingServiceProxy
                Dim m As Services.Common.pMethod = AddressOf p.BeginLockboxFileImport
                res = RunDiamondService(m, req, e)
                If res IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Using

        End Function
        Public Function BeginPostPolicyPaymentsProcess(ByRef res As DCSA.BeginPostPolicyPaymentsProcess.Response,
                                                       ByRef req As DCSA.BeginPostPolicyPaymentsProcess.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Using p As New DCSP.AccountingServiceProxy
                Dim m As Services.Common.pMethod = AddressOf p.BeginPostPolicyPaymentsProcess
                res = RunDiamondService(m, req, e)
                If res IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Using
        End Function
        Public Function CreateGLFileXML(ByRef res As DCSA.CreateGLFileXML.Response,
                                        ByRef req As DCSA.CreateGLFileXML.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateGLFileXML
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyPayments(ByRef res As DCSA.DeleteAgencyPayments.Response,
                                             ByRef req As DCSA.DeleteAgencyPayments.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyPayments
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAgencyReceipts(ByRef res As DCSA.DeleteAgencyReceipts.Response,
                                             ByRef req As DCSA.DeleteAgencyReceipts.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteAgencyReceipts
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteBatchWaiveRecords(ByRef res As DCSA.DeleteBatchWaiveRecords.Response,
                                                ByRef req As DCSA.DeleteBatchWaiveRecords.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteBatchWaiveRecords
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteLockboxBatch(ByRef res As DCSA.DeleteLockboxBatch.Response,
                                           ByRef req As DCSA.DeleteLockboxBatch.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteLockboxBatch
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePendingSuspensePayment(ByRef res As DCSA.DeletePendingSuspensePayment.Response,
                                                     ByRef req As DCSA.DeletePendingSuspensePayment.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePendingSuspensePayment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePolicyPayment(ByRef res As DCSA.DeletePolicyPayment.Response,
                                            ByRef req As DCSA.DeletePolicyPayment.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeletePolicyPayment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ImportLockboxPayment(ByRef res As DCSA.ImportLockboxPayment.Response,
                                             ByRef req As DCSA.ImportLockboxPayment.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ImportLockboxPayment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAccountingBatchWaiveRecords(ByRef res As DCSA.LoadAccountingBatchWaiveRecords.Response,
                                                        ByRef req As DCSA.LoadAccountingBatchWaiveRecords.Request,
                                                        Optional ByRef e As Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAccountingBatchWaiveRecords
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyActivity(ByRef res As DCSA.LoadAgencyActivity.Response,
                                           ByRef req As DCSA.LoadAgencyActivity.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyActivity
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencybasedOnCompanyStateLobId(ByRef res As DCSA.LoadAgencybasedOnCompanyStateLobId.Response,
                                                           ByRef req As DCSA.LoadAgencybasedOnCompanyStateLobId.Request,
                                                           Optional ByRef e As Exception = Nothing,
                                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencybasedOnCompanyStateLobId
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyBillActivity(ByRef res As DCSA.LoadAgencyBillActivity.Response,
                                               ByRef req As DCSA.LoadAgencyBillActivity.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyBillActivity
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyBillPaymentDetail(ByRef res As DCSA.LoadAgencyBillPaymentDetail.Response,
                                                    ByRef req As DCSA.LoadAgencyBillPaymentDetail.Request,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyBillPaymentDetail
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyCode(ByRef res As DCSA.LoadAgencyCode.Response,
                                       ByRef req As DCSA.LoadAgencyCode.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyCode
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyPayments(ByRef res As DCSA.LoadAgencyPayments.Response,
                                           ByRef req As DCSA.LoadAgencyPayments.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyPayments
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyReceiptItemInfo(ByRef res As DCSA.LoadAgencyReceiptItemInfo.Response,
                                                  ByRef req As DCSA.LoadAgencyReceiptItemInfo.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyReceiptItemInfo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyReceipts(ByRef res As DCSA.LoadAgencyReceipts.Response,
                                           ByRef req As DCSA.LoadAgencyReceipts.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyReceipts
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBatchWaiveQueue(ByRef res As DCSA.LoadBatchWaiveQueue.Response,
                                            ByRef req As DCSA.LoadBatchWaiveQueue.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBatchWaiveQueue
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadBillMethod(ByRef res As DCSA.LoadBillMethod.Response,
                                       ByRef req As DCSA.LoadBillMethod.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadBillMethod
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCashEntryDetailInfo(ByRef res As DCSA.LoadCashEntryDetailInfo.Response,
                                                ByRef req As DCSA.LoadCashEntryDetailInfo.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCashEntryDetailInfo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompanyStateLob(ByRef res As DCSA.LoadCompanyStateLob.Response,
                                            ByRef req As DCSA.LoadCompanyStateLob.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompanyStateLob
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadForSuspenseAccountAdjustment(ByRef res As DCSA.LoadForSuspenseAccountAdjustment.Response,
                                                         ByRef req As DCSA.LoadForSuspenseAccountAdjustment.Request,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadForSuspenseAccountAdjustment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLockboxInfo(ByRef res As DCSA.LoadLockboxInfo.Response,
                                        ByRef req As DCSA.LoadLockboxInfo.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLockboxInfo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPolicyPaymentItemInfo(ByRef res As DCSA.LoadPolicyPaymentItemInfo.Response,
                                                  ByRef req As DCSA.LoadPolicyPaymentItemInfo.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPolicyPaymentItemInfo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPolicyPayments(ByRef res As DCSA.LoadPolicyPayments.Response,
                                           ByRef req As DCSA.LoadPolicyPayments.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPolicyPayments
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSuspenseAccountCash(ByRef res As DCSA.LoadSuspenseAccountCash.Response,
                                                ByRef req As DCSA.LoadSuspenseAccountCash.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSuspenseAccountCash
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSuspenseAccounts(ByRef res As DCSA.LoadSuspenseAccounts.Response,
                                             ByRef req As DCSA.LoadSuspenseAccounts.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSuspenseAccounts
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PostAgencyPayments(ByRef res As DCSA.PostAgencyPayments.Response,
                                           ByRef req As DCSA.PostAgencyPayments.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.PostAgencyPayments
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PostAgencyReceipts(ByRef res As DCSA.PostAgencyReceipts.Response,
                                           ByRef req As DCSA.PostAgencyReceipts.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.PostAgencyReceipts
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessAgencyBillBatchPaymentAdjustment(ByRef res As DCSA.ProcessAgencyBillBatchPaymentAdjustment.Response,
                                                                ByRef req As DCSA.ProcessAgencyBillBatchPaymentAdjustment.Request,
                                                                Optional ByRef e As Exception = Nothing,
                                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessAgencyBillBatchPaymentAdjustment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyActivityAdjustment(ByRef res As DCSA.SaveAgencyActivityAdjustment.Response,
                                                     ByRef req As DCSA.SaveAgencyActivityAdjustment.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyActivityAdjustment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyPayments(ByRef res As DCSA.SaveAgencyPayments.Response,
                                           ByRef req As DCSA.SaveAgencyPayments.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyPayments
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyReceipts(ByRef res As DCSA.SaveAgencyReceipts.Response,
                                           ByRef req As DCSA.SaveAgencyReceipts.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyReceipts
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyStatementAdjustment(ByRef res As DCSA.SaveAgencyStatementAdjustment.Response,
                                                      ByRef req As DCSA.SaveAgencyStatementAdjustment.Request,
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyStatementAdjustment
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyStatementReceipts(ByRef res As DCSA.SaveAgencyStatementReceipts.Response,
                                                    ByRef req As DCSA.SaveAgencyStatementReceipts.Request,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyStatementReceipts
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveLockboxData(ByRef res As DCSA.SaveLockboxData.Response,
                                        ByRef req As DCSA.SaveLockboxData.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveLockboxData
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePolicyPayments(ByRef res As DCSA.SavePolicyPayments.Response,
                                           ByRef req As DCSA.SavePolicyPayments.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePolicyPayments
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function WaiveBatchRecords(ByRef res As DCSA.WaiveBatchRecords.Response,
                                          ByRef req As DCSA.WaiveBatchRecords.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AccountingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.WaiveBatchRecords
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
