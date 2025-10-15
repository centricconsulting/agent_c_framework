Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Imports IFM.DiamondServices.Enums
Imports DCSB = Diamond.Common.Services.Messages.BillingService
Namespace Services.Billing
    Public Module Billing
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="PolicyImage"></param>
        ''' <param name="Amount"></param>
        ''' <param name="Source"></param>
        ''' <param name="Type"></param>
        ''' <param name="Reason"></param>
        ''' <param name="AgencyEFTAccountID"></param>
        ''' <param name="CheckNum"></param>
        ''' <param name="DiamondUserId"></param>
        ''' <param name="DiamondLoginName"></param>
        ''' <param name="DiamondLoginDomain"></param>
        ''' <param name="SuppressBillingReceiptForEftPayments"></param>
        ''' <param name="CheckDate"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ApplyCash(PolicyImage As DCO.Policy.Image,
                                  Amount As Decimal,
                                  Source As BillingCashInSource,
                                  Type As DC.Enums.Billing.BillingCashType,
                                  Reason As DC.Enums.Billing.BillingReason,
                                  AgencyEFTAccountID As String,
                                  CheckNum As String,
                                  Optional ByVal DiamondUserId As Integer = Nothing,
                                  Optional ByVal DiamondLoginName As String = Nothing,
                                  Optional ByVal DiamondLoginDomain As String = Nothing,
                                  Optional SuppressBillingReceiptForEftPayments As Boolean = False,
                                  Optional ByVal CheckDate As Date = Nothing,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean

            Dim req As New DCSM.BillingService.ApplyCash.Request
            Dim res As New DCSM.BillingService.ApplyCash.Response
            Dim cash As New DCO.Billing.ApplyCash
            If CheckDate = Nothing Then
                CheckDate = Date.Today
            End If
            With cash
                .AgencyId = PolicyImage.AgencyId
                .CashAmount = Amount
                .CheckDate = CheckDate
                .CashInSource = Source
                .CheckNum = CheckNum

                If Source = BillingCashInSource.WebAgencyEftWithApp Or
                    Source = BillingCashInSource.WebAgencyEft Then
                    Source = BillingCashInSource.AgencyEft
                End If
                .CashType = Type
                .Validated = True
                .PolicyNo = PolicyImage.PolicyNumber
                .EftAccountId = AgencyEFTAccountID
                .PolicyId = PolicyImage.PolicyId
                .PolicyImageNum = PolicyImage.PolicyImageNum
                .ReasonId = Reason
                .SuppressAgencyEftReceipt = SuppressBillingReceiptForEftPayments

                .UsersId = DiamondUserId
                .LoginName = DiamondLoginName
                .LoginDomain = DiamondLoginDomain
            End With

            req.RequestData.ApplyCash = cash

            If IFMS.Billing.ApplyCash(res, req, e, dv) Then
                If res.ResponseData IsNot Nothing Then
                    Return res.ResponseData.Success
                End If
            End If
            Return False
        End Function

        Public Function CreateMultpleShortPreviewInvoices(img As DCO.Policy.Image,
                                                          Optional ByRef e As System.Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Billing.PayPlanPreview)
            Dim req As New DC.Services.Messages.BillingService.CreateMultipleShortPreviewInvoices.Request
            Dim res As New DC.Services.Messages.BillingService.CreateMultipleShortPreviewInvoices.Response
            req.RequestData.PolicyImage = img
            If IFMS.Billing.CreateMultipleShortPreviewInvoices(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.PayPlanPreviews
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Load should be used when the policy is in the Diamond System and in-force.
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="LoadAccountDetail"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <param name="dv"></param>
        ''' <returns>Boolean on success</returns>
        ''' <remarks>Untested</remarks>
        Public Function Load(PolicyID As Integer,
                             Optional LoadAccountDetail As Boolean = True,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.Objects.Billing.Data

            Dim req As New DC.Services.Messages.BillingService.Load.Request
            Dim res As New DC.Services.Messages.BillingService.Load.Response
            req.RequestData.PolicyId = PolicyID
            req.RequestData.LoadAccountDetail = LoadAccountDetail

            If IFMS.Billing.Load(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.BillingData
                End If
            End If
            Return Nothing
        End Function


        ''' <summary>
        ''' Load preview should be used when the policy is in the Diamond System and is in a pending state.
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="BillingPayPlanID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <param name="dv"></param>
        ''' <returns>Boolean on success</returns>
        ''' <remarks>Untested</remarks>
        Public Function LoadPreview(PolicyID As Integer,
                                    PolicyImageNum As String,
                                    BillingPayPlanID As Integer,
                                    BillingTransactionTypeID As DC.Enums.Billing.BillingTransactionType,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.Objects.Billing.Data
            Dim p As New DC.Services.Proxies.BillingServiceProxy
            Dim req As New DC.Services.Messages.BillingService.LoadPreview.Request
            Dim res As New DC.Services.Messages.BillingService.LoadPreview.Response
            With req.RequestData
                .PolicyId = PolicyID
                .PolicyImageNum = PolicyImageNum
                .BillingPayPlanId = BillingPayPlanID
                .BillingTransactionTypeId = BillingTransactionTypeID
                If BillingTransactionTypeID = DC.Enums.Billing.BillingTransactionType.PayPlanChange Or
                    BillingTransactionTypeID = DC.Enums.Billing.BillingTransactionType.RecalculateFutureInstallments Or
                    BillingTransactionTypeID = DC.Enums.Billing.BillingTransactionType.RecalculateInstallsAfterDivRefund Then
                    .PayPlanChangeTransaction = True
                End If
            End With

            If IFMS.Billing.LoadPreview(res, req, e, dv) Then
                If res.ResponseData.BillingData IsNot Nothing Then
                    Return res.ResponseData.BillingData
                End If
            End If
            Return Nothing
        End Function

        Public Function GetAccountBalance(PolicyID As Integer,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.Objects.Billing.AccountBalance

            Dim req As New DCSB.GetAccountBalance.Request
            Dim res As New DCSB.GetAccountBalance.Response
            req.RequestData.PolicyId = PolicyID

            If IFMS.Billing.GetAccountBalance(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.AccountBalance
                End If
            End If
            Return Nothing
        End Function

        Public Function GetAccountInfo(PolicyID As Integer,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DC.Objects.Billing.AccountInfo)

            Dim req As New DCSB.GetAccountInfo.Request
            Dim res As New DCSB.GetAccountInfo.Response
            req.RequestData.PolicyId = PolicyID

            If IFMS.Billing.GetAccountInfo(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.AccountInfos
                End If
            End If
            Return Nothing
        End Function


        Public Function LoadBillingAccountsByClient(ClientId As Integer,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.Objects.Billing.BillingAccountInfo

            Dim req As New DCSB.LoadBillingAccountsByClient.Request
            Dim res As New DCSB.LoadBillingAccountsByClient.Response
            req.RequestData.ClientId = ClientId

            If IFMS.Billing.LoadBillingAccountsByClient(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.BillingAccountInfo
                End If
            End If
            Return Nothing
        End Function

        Public Function GetBillingSummary(PolicyID As Integer,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.Objects.Billing.BillingSummary

            Dim req As New DCSB.GetBillingSummary.Request
            Dim res As New DCSB.GetBillingSummary.Response
            req.RequestData.PolicyId = PolicyID

            If IFMS.Billing.GetBillingSummary(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.BillingSummary
                End If
            End If
            Return Nothing
        End Function


        Public Function IssueTransaction(transactionData As DCO.Billing.TransactionData,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean

            Dim req As New DCSB.IssueTransaction.Request
            Dim res As New DCSB.IssueTransaction.Response
            req.RequestData.TransactionData = New DCO.InsCollection(Of DCO.Billing.TransactionData)
            req.RequestData.TransactionData.Add(transactionData)

            If IFMS.Billing.IssueTransaction(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.Success
                End If
            End If
            Return False
        End Function

        Public Function SavePolicyCreditCardInfo(PolicyId As Int32,
                                                 cc As DCO.Billing.CreditCard,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean

            Dim req As New DCSB.SavePolicyCreditCardInfo.Request
            Dim res As New DCSB.SavePolicyCreditCardInfo.Response
            req.RequestData.PolicyId = PolicyId
            req.RequestData.CreditCard = cc
            req.RequestData.RequireCreditCard = False

            If IFMS.Billing.SavePolicyCreditCardInfo(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.Success
                End If
            End If
            Return False
        End Function


        Public Function GetPolicyEftInfo(PolicyId As Int32,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.EFT.Eft

            Dim req As New DCSB.GetPolicyEftInfo.Request
            Dim res As New DCSB.GetPolicyEftInfo.Response
            req.RequestData.PolicyId = PolicyId


            If IFMS.Billing.GetPolicyEftInfo(res, req, e, dv) Then
                Return res.ResponseData.Eft
                'If res.ResponseData.Success Then seems to alway return false ????
                '    Return res.ResponseData?.Eft
                'End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Saves the EFT information to diamond. Successful if it returns an EftAccountId greater than 0.
        ''' </summary>
        ''' <param name="eftInfo"></param>
        ''' <param name="e"></param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        Public Function SavePolicyEftInfo(eftInfo As DCO.EFT.Eft,
                            Optional ByRef e As System.Exception = Nothing,
                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Int32

            Dim req As New DCSB.SavePolicyEftInfo.Request
            Dim res As New DCSB.SavePolicyEftInfo.Response
            req.RequestData.Eft = eftInfo


            If IFMS.Billing.SavePolicyEftInfo(res, req, e, dv) Then
                If res.ResponseData.Success Then
                    Return res.ResponseData.EftAccountId
                End If
            End If
            Return 0
        End Function



    End Module
End Namespace

