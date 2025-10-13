Imports System.Threading.Tasks
Imports IFM.PrimativeExtensions

Public Class DiamondBilling
    Implements IDisposable

    Public Enum cashInSource
        NA = 0
        InsuredsCheck = 1
        Cash = 2
        MoneyOrder = 3
        AgencyCheck = 4
        MortgageCheck = 5
        EFT = 6
        RetailLockbox = 7
        AgencyEft = 8
        IVRCreditCard = 9
        PostDatedCheck = 10
        CreditCard = 11
        PayrollDeduction = 12
        AgencyBill = 13
        Dividend = 14
        ManualLockbox = 15
        'ECheck = 17
        eCheck = 10030 '12/20/11
        RetailLockboxDiscover = 10001
        RetailLockboxMastercard = 10002
        RetailLockboxVisa = 10003
        RetailLockboxAmericanExpress = 10004
        RetailLockboxRemittance = 10005
        WebAmericanExpress = 10006
        WebDiscover = 10007
        WebMastercard = 10008
        WebVisa = 10009
        CashierCheck = 10010
        RemittanceWriteOffInsuredCheck = 10011
        RemittanceWriteOffOther = 10012
        Other = 10013
        DownpaymentWithApp = 10014
        SimplePayEft = 10015
        WebAgencyCCAmericanExpress = 10016
        WebAgencyCCDiscover = 10017
        WebAgencyCCMastercard = 10018
        WebAgencyCCVisa = 10019
        WebAgencyCCWithAppAmericanExpress = 10020
        WebAgencyCCWithAppDiscover = 10021
        WebAgencyCCWithAppMastercard = 10022
        WebAgencyCCWithAppVisa = 10023
        WebAgencyEft = 10024
        WebAgencyEftWithApp = 10025
        WebCCAmericanExpress = 10026
        WebCCDiscover = 10027
        WebCCMastercard = 10028
        WebCCVisa = 10029
    End Enum

    Public Sub New()

    End Sub

    Public Function applyCash(ByVal policyNumber As String, ByVal policyImageNum As Integer, ByVal cashAmount As Decimal, ByVal cashSource As cashInSource, ByVal cashType As DCE.Billing.BillingCashType,
                              ByVal cashReason As DCE.Billing.BillingReason) As Boolean
        Dim isAccountBill As Boolean = False
        Dim BillingAccountId As Integer = 0

        If policyNumber.IsAccountBillNumber() Then
            isAccountBill = True
            BillingAccountId = policyNumber.GetBillingAccountIdFromAccountNumber()
            policyNumber = GetPreferredAccountBillPolicy(policyNumber)
        End If

        Using pno As New PolicyNumberObject(policyNumber, AppSettings("conn"), AppSettings("connDiamond"))
            pno.UseDiamondData = True
            pno.GetAllAgencyInfo = True

            If isAccountBill Then
                pno.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond)
            Else
                BillingAccountId = GetPolicyInfoWhileCheckingIfPolicyIsInAccountBill(pno)
                If BillingAccountId > 0 Then
                    isAccountBill = True
                End If
            End If

            'pno.GetPolicyInfo()
            If pno.hasPolicyInfoError = False Then
                Dim cash As New DCO.Billing.ApplyCash
                With cash
                    .AgencyId = pno.AgencyID 'Int
                    .CashAmount = cashAmount 'Decimal
                    .CheckDate = Date.Today
                    If cashSource = cashInSource.WebAgencyEftWithApp Then .CheckNum = "W/ APP"
                    If cashSource = cashInSource.WebAgencyEftWithApp Or cashSource = cashInSource.WebAgencyEft Then cashSource = cashInSource.AgencyEft
                    .CashInSource = cashSource 'Int - This is where you say EFT/CC/Cash/etc
                    .CashType = cashType 'Int - Payment/Refund/Reversal/etc
                    'TODO: accounting date field (currently missing)
                    .Validated = True

                    If String.IsNullOrEmpty(pno.AgencyInfo.DiamondEFTAccountID) = False AndAlso
                        (cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.AgencyEft Or cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.Eft) Then .EftAccountId = pno.AgencyInfo.DiamondEFTAccountID
                    .ReasonId = cashReason 'Int

                    .AccountPayment = isAccountBill
                    .BillingAccountId = BillingAccountId
                    .PolicyNo = policyNumber
                    .PolicyId = pno.PolicyID
                    .PolicyImageNum = policyImageNum

                    '3/13/2013 - added key to optionally suppress billing receipt on EFT payments
                    If cashSource = cashInSource.AgencyEft AndAlso AppSettings("ApplyCash_SuppressBillingReceiptForEftPayments") IsNot Nothing AndAlso UCase(AppSettings("ApplyCash_SuppressBillingReceiptForEftPayments").ToString) = "YES" Then
                        .SuppressBillingReceipt = True
                    End If
                End With

                If isAccountBill Then
                    Return DoApplyBillingAccountPayment(cash, "")
                Else
                    Return DoApplyCash(cash, "")
                End If
            End If
        End Using
    End Function

    Public Function applyCashWithPolicyInfo(ByVal policyNumber As String, ByVal policyId As Integer, ByVal policyImageNum As Integer, ByVal agencyId As Integer, ByVal cashAmount As Decimal, ByVal cashSource As cashInSource, ByVal cashType As DCE.Billing.BillingCashType,
                               ByVal cashReason As DCE.Billing.BillingReason, Optional ByVal DiamondUserId As Integer = 0, Optional ByVal DiamondLoginName As String = "", Optional ByVal DiamondLoginDomain As String = "", Optional ByVal AgencyEFTAccountId As String = "") As Boolean

        Dim needPolicyLookup As Boolean = False
        Dim needAgentInfo As Boolean = False
        Dim isAccountBill As Boolean = False
        Dim BillingAccountId As Integer = 0

        If policyId = Nothing OrElse policyId = 0 OrElse policyImageNum = Nothing OrElse policyImageNum = 0 OrElse agencyId = Nothing OrElse agencyId = 0 Then
            needPolicyLookup = True
        End If

        If AgencyEFTAccountId = "" AndAlso (cashSource = cashInSource.AgencyEft OrElse cashSource = cashInSource.WebAgencyEft OrElse cashSource = cashInSource.WebAgencyEftWithApp OrElse cashSource = cashInSource.EFT OrElse cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.AgencyEft OrElse cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.Eft) Then
            needPolicyLookup = True
            needAgentInfo = True
        End If

        If policyNumber.IsAccountBillNumber() Then
            isAccountBill = True
            BillingAccountId = policyNumber.GetBillingAccountIdFromAccountNumber()
            policyNumber = GetPreferredAccountBillPolicy(policyNumber)
        End If

        If needPolicyLookup = True Then
            Using polnum As New PolicyNumberObject(policyNumber)
                polnum.UseDiamondData = True

                If needAgentInfo = True Then
                    polnum.GetAllAgencyInfo = True
                End If

                If isAccountBill Then
                    If policyId <> Nothing AndAlso policyId > 0 Then
                        polnum.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond, policyId)
                    Else
                        polnum.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond)
                    End If
                Else
                    BillingAccountId = GetPolicyInfoWhileCheckingIfPolicyIsInAccountBill(polnum, policyId)
                    If BillingAccountId > 0 Then
                        isAccountBill = True
                    End If
                End If

                If polnum.hasPolicyInfo = True Then
                    If policyId = Nothing OrElse policyId = 0 Then
                        policyId = polnum.PolicyID
                        policyImageNum = polnum.DiamondPolicyImageNum
                    ElseIf policyImageNum = Nothing OrElse policyImageNum = 0 Then
                        policyImageNum = polnum.DiamondPolicyImageNum
                    End If

                    If agencyId = Nothing OrElse agencyId = 0 Then
                        agencyId = polnum.AgencyID
                    End If

                    If needAgentInfo = True Then
                        AgencyEFTAccountId = polnum.AgencyInfo.DiamondEFTAccountID
                    End If
                End If
            End Using
        Else
            'check if account bill policy
            BillingAccountId = GetBillingAccountId(policyNumber)
            If BillingAccountId > 0 Then
                isAccountBill = True
                policyNumber = GetPreferredAccountBillPolicy(policyNumber)
            End If
        End If

        Dim cash As New DCO.Billing.ApplyCash
        With cash
            .AgencyId = agencyId
            .CashAmount = cashAmount 'Decimal
            .CheckDate = Date.Today
            If cashSource = cashInSource.WebAgencyEftWithApp Then .CheckNum = "W/ APP"
            If cashSource = cashInSource.WebAgencyEftWithApp Or cashSource = cashInSource.WebAgencyEft Then cashSource = cashInSource.AgencyEft
            .CashInSource = cashSource 'Int - This is where you say EFT/CC/Cash/etc
            .CashType = cashType 'Int - Payment/Refund/Reversal/etc
            'TODO: accounting date field (currently missing)
            .Validated = True
            If DiamondUserId > 0 Then
                .UsersId = DiamondUserId
            End If
            If DiamondLoginName <> "" Then
                .LoginName = DiamondLoginName
            End If
            If DiamondLoginDomain <> "" Then
                .LoginDomain = DiamondLoginDomain
            End If

            If String.IsNullOrEmpty(AgencyEFTAccountId) = False AndAlso
                (cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.AgencyEft Or cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.Eft) Then .EftAccountId = AgencyEFTAccountId

            .ReasonId = cashReason 'Int

            .AccountPayment = isAccountBill
            .BillingAccountId = BillingAccountId
            .PolicyNo = policyNumber
            .PolicyId = policyId
            .PolicyImageNum = policyImageNum

            '3/13/2013 - added key to optionally suppress billing receipt on EFT payments
            If cashSource = cashInSource.AgencyEft AndAlso AppSettings("ApplyCash_SuppressBillingReceiptForEftPayments") IsNot Nothing AndAlso UCase(AppSettings("ApplyCash_SuppressBillingReceiptForEftPayments").ToString) = "YES" Then
                .SuppressBillingReceipt = True
            End If
        End With

        If isAccountBill Then
            Return DoApplyBillingAccountPayment(cash, "")
        Else
            Return DoApplyCash(cash, "")
        End If

        'If isAccountBill Then
        '    Dim request As New Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Request
        '    Dim applyCashResponse As New Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Response
        '    Try
        '        With request.RequestData
        '            .ApplyCash = cash    ' cash As DCO.Billing.ApplyCash
        '        End With

        '        Using billingProxy As New Proxies.BillingServiceProxy
        '            applyCashResponse = billingProxy.ApplyBillingAccountPayment(request)
        '        End Using

        '        Return applyCashResponse.ResponseData.Success
        '    Catch ex As Exception
        '        Dim cashJSON As String = "Nothing"
        '        Dim applyCashResponseJSON As String = "Nothing"
        '        If cash IsNot Nothing Then
        '            cashJSON = Newtonsoft.Json.JsonConvert.SerializeObject(cash)
        '        End If
        '        If applyCashResponse IsNot Nothing Then
        '            applyCashResponseJSON = Newtonsoft.Json.JsonConvert.SerializeObject(applyCashResponse)
        '        End If
        '        IFM.IFMErrorLogging.LogException(ex, "Error ApplyBillingAccountPayment; cash Object - " & cashJSON & "; applyCashResponse Object - " & applyCashResponseJSON)
        '        Using sqlCmd As New SQLexecuteObject(AppSettings("conn"))
        '            sqlCmd.queryOrStoredProc = "Insert into tbl_agentPayment_diamond_Errors (PolicyNumber, ErrorDescription, AccountNumber) values ('" & policyNumber & "', '" & Replace(ex.ToString, "'", "''") & " - Test', '" & BillingAccountId.GetAccountBillNumberFromBillingAccountId() & "')"
        '            sqlCmd.ExecuteStatement()
        '        End Using
        '        Throw
        '    End Try
        'Else
        '    Dim request As New Diamond.Common.Services.Messages.BillingService.ApplyCash.Request
        '    Dim applyCashResponse As New Diamond.Common.Services.Messages.BillingService.ApplyCash.Response
        '    Try
        '        With request.RequestData
        '            .ApplyCash = cash    ' cash As DCO.Billing.ApplyCash
        '        End With

        '        Using billingProxy As New Proxies.BillingServiceProxy
        '            applyCashResponse = billingProxy.ApplyCash(request)
        '        End Using

        '        Return applyCashResponse.ResponseData.Success
        '    Catch ex As Exception
        '        Dim cashJSON As String = "Nothing"
        '        Dim applyCashResponseJSON As String = "Nothing"
        '        If cash IsNot Nothing Then
        '            cashJSON = Newtonsoft.Json.JsonConvert.SerializeObject(cash)
        '        End If
        '        If applyCashResponse IsNot Nothing Then
        '            applyCashResponseJSON = Newtonsoft.Json.JsonConvert.SerializeObject(applyCashResponse)
        '        End If
        '        IFM.IFMErrorLogging.LogException(ex, "Error ApplyCash; cash Object - " & cashJSON & "; applyCashResponse Object - " & applyCashResponseJSON)
        '        Using sqlCmd As New SQLexecuteObject(AppSettings("conn"))
        '            sqlCmd.queryOrStoredProc = "Insert into tbl_agentPayment_diamond_Errors (PolicyNumber, ErrorDescription) values ('" & policyNumber & "', '" & Replace(ex.ToString, "'", "''") & "')"
        '            sqlCmd.ExecuteStatement()
        '        End Using
        '        Throw
        '    End Try
        'End If
    End Function

    Private Function GetPolicyInfoWhileCheckingIfPolicyIsInAccountBill(polnum As PolicyNumberObject, Optional policyId As Integer = 0) As Integer
        Dim TaskA As Task(Of Integer)
        Dim TaskB As Task

        TaskA = Task.Run(Function() GetBillingAccountId(polnum.EnteredPolicy))

        If policyId <> Nothing AndAlso policyId > 0 Then
            TaskB = Task.Run(Sub() polnum.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond, policyId))
        Else
            TaskB = Task.Run(Sub() polnum.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond))
        End If

        Task.WaitAll(TaskA, TaskB)

        Return TaskA.Result()
    End Function

    Private Function GetBillingAccountId(policyNumber As String) As Integer
        Dim returnString As String = ""
        Dim returnVar As Integer = 0
        Using proxy As New IFM.JsonProxyClient.ProxyClient(ConfigurationManager.AppSettings("IFMPolicyInquiry_EndPointBaseUrl"))
            Dim response As System.Net.Http.HttpResponseMessage = Nothing
            If proxy.GetAndDeserializeResponseText($"AccountBill/GetBillingAccountIdByPolicyNumber/{policyNumber}", response, returnString) = True Then
                returnVar = returnString.TryToGetInt32()
            End If
        End Using
        Return returnVar
    End Function

    Private Function GetPreferredAccountBillPolicy(accountNumber As String) As String
        Dim returnVar As String = ""
        Dim endpoint As String

        If accountNumber.IsAccountBillNumber() Then
            endpoint = $"AccountBill/GetPreferredAccountBillPolicyByAccountNumber/{accountNumber}"
        Else
            endpoint = $"AccountBill/GetPreferredAccountBillPolicyByPolicyNumber/{accountNumber}"
        End If

        Using proxy As New IFM.JsonProxyClient.ProxyClient(ConfigurationManager.AppSettings("IFMPolicyInquiry_EndPointBaseUrl"))
            Dim response As System.Net.Http.HttpResponseMessage = Nothing
            If proxy.GetAndDeserializeResponseText(endpoint, response, returnVar) = True Then

            End If
        End Using

        Return returnVar
    End Function

    Public Function applyCashWithPolicyInfoAndErrorMessage(ByVal policyNumber As String, ByVal policyId As Integer, ByVal policyImageNum As Integer, ByVal agencyId As Integer, ByVal cashAmount As Decimal, ByVal cashSource As cashInSource, ByVal cashType As DCE.Billing.BillingCashType,
                               ByVal cashReason As DCE.Billing.BillingReason, Optional ByVal DiamondUserId As Integer = 0, Optional ByVal DiamondLoginName As String = "", Optional ByVal DiamondLoginDomain As String = "", Optional ByVal AgencyEFTAccountId As String = "", Optional ByRef ErrorMessage As String = "") As Boolean

        Dim needPolicyLookup As Boolean = False
        Dim needAgentInfo As Boolean = False
        Dim isAccountBill As Boolean = False
        Dim BillingAccountId As Integer = 0

        If policyId = Nothing OrElse policyId = 0 OrElse policyImageNum = Nothing OrElse policyImageNum = 0 OrElse agencyId = Nothing OrElse agencyId = 0 Then
            needPolicyLookup = True
        End If

        If AgencyEFTAccountId = "" AndAlso (cashSource = cashInSource.AgencyEft OrElse cashSource = cashInSource.WebAgencyEft OrElse cashSource = cashInSource.WebAgencyEftWithApp OrElse cashSource = cashInSource.EFT OrElse cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.AgencyEft OrElse cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.Eft) Then
            needPolicyLookup = True
            needAgentInfo = True
        End If

        If policyNumber.IsAccountBillNumber() Then
            isAccountBill = True
            BillingAccountId = policyNumber.GetBillingAccountIdFromAccountNumber()
            policyNumber = GetPreferredAccountBillPolicy(policyNumber)
        End If

        If needPolicyLookup = True Then
            Using polnum As New PolicyNumberObject(policyNumber)
                polnum.UseDiamondData = True

                If needAgentInfo = True Then
                    polnum.GetAllAgencyInfo = True
                End If

                If isAccountBill Then
                    If policyId <> Nothing AndAlso policyId > 0 Then
                        polnum.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond, policyId)
                    Else
                        polnum.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond)
                    End If
                Else
                    BillingAccountId = GetPolicyInfoWhileCheckingIfPolicyIsInAccountBill(polnum, policyId)
                    If BillingAccountId > 0 Then
                        isAccountBill = True
                    End If
                End If

                If polnum.hasPolicyInfo = True Then
                    If policyId = Nothing OrElse policyId = 0 Then
                        policyId = polnum.PolicyID
                        policyImageNum = polnum.DiamondPolicyImageNum
                    ElseIf policyImageNum = Nothing OrElse policyImageNum = 0 Then
                        policyImageNum = polnum.DiamondPolicyImageNum
                    End If

                    If agencyId = Nothing OrElse agencyId = 0 Then
                        agencyId = polnum.AgencyID
                    End If

                    If needAgentInfo = True Then
                        AgencyEFTAccountId = polnum.AgencyInfo.DiamondEFTAccountID
                    End If
                End If
            End Using
        Else
            'Gotta check if it is an account bill policy but came in as a regular policy number
            BillingAccountId = GetBillingAccountId(policyNumber)
            If BillingAccountId > 0 Then
                isAccountBill = True
                policyNumber = GetPreferredAccountBillPolicy(policyNumber)
            End If
        End If

        'Try
        Dim cash As New DCO.Billing.ApplyCash
        With cash
            .AgencyId = agencyId
            .CashAmount = cashAmount 'Decimal
            .CheckDate = Date.Today
            If cashSource = cashInSource.WebAgencyEftWithApp Then .CheckNum = "W/ APP"
            If cashSource = cashInSource.WebAgencyEftWithApp Or cashSource = cashInSource.WebAgencyEft Then cashSource = cashInSource.AgencyEft
            .CashInSource = cashSource 'Int - This is where you say EFT/CC/Cash/etc
            .CashType = cashType 'Int - Payment/Refund/Reversal/etc
            'TODO: accounting date field (currently missing)
            .Validated = True

            If DiamondUserId > 0 Then
                .UsersId = DiamondUserId
            End If
            If DiamondLoginName <> "" Then
                .LoginName = DiamondLoginName
            End If
            If DiamondLoginDomain <> "" Then
                .LoginDomain = DiamondLoginDomain
            End If

            If String.IsNullOrEmpty(AgencyEFTAccountId) = False AndAlso
                (cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.AgencyEft Or cashSource = Diamond.Common.Enums.Billing.BillingCashInSource.Eft) Then .EftAccountId = AgencyEFTAccountId

            .ReasonId = cashReason 'Int

            .AccountPayment = isAccountBill
            .BillingAccountId = BillingAccountId
            .PolicyNo = policyNumber
            .PolicyId = policyId
            .PolicyImageNum = policyImageNum

            '3/13/2013 - added key to optionally suppress billing receipt on EFT payments
            If cashSource = cashInSource.AgencyEft AndAlso AppSettings("ApplyCash_SuppressBillingReceiptForEftPayments") IsNot Nothing AndAlso UCase(AppSettings("ApplyCash_SuppressBillingReceiptForEftPayments").ToString) = "YES" Then
                .SuppressBillingReceipt = True
            End If
        End With

        If isAccountBill Then
            Return DoApplyBillingAccountPayment(cash, ErrorMessage)
        Else
            Return DoApplyCash(cash, ErrorMessage)
        End If

        '    If isAccountBill Then
        '        Dim request As New Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Request
        '        Dim applyCashResponse As New Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Response

        '        With request.RequestData
        '            .ApplyCash = cash    ' cash As DCO.Billing.ApplyCash
        '        End With

        '        Using billingProxy As New Proxies.BillingServiceProxy
        '            applyCashResponse = billingProxy.ApplyBillingAccountPayment(request)
        '        End Using


        '        If applyCashResponse IsNot Nothing AndAlso applyCashResponse.DiamondValidation.HasAnyItems Then
        '            For Each vItem As DCO.ValidationItem In applyCashResponse.DiamondValidation.ValidationItems
        '                If vItem.ValidationSeverityType = DCE.ValidationSeverityType.Errors Then
        '                    'handle error accordingly
        '                    If ErrorMessage = "" Then
        '                        ErrorMessage = vItem.Message
        '                    Else
        '                        ErrorMessage &= "; " & vItem.Message
        '                    End If
        '                End If
        '            Next
        '        End If

        '        Return applyCashResponse.ResponseData.Success
        '    Else
        '        Dim request As New Diamond.Common.Services.Messages.BillingService.ApplyCash.Request
        '        Dim applyCashResponse As New Diamond.Common.Services.Messages.BillingService.ApplyCash.Response

        '        With request.RequestData
        '            .ApplyCash = cash    ' cash As DCO.Billing.ApplyCash
        '        End With

        '        Using billingProxy As New Proxies.BillingServiceProxy
        '            applyCashResponse = billingProxy.ApplyCash(request)
        '        End Using

        '        If applyCashResponse IsNot Nothing AndAlso applyCashResponse.DiamondValidation.HasAnyItems Then
        '            For Each vItem As DCO.ValidationItem In applyCashResponse.DiamondValidation.ValidationItems
        '                If vItem.ValidationSeverityType = DCE.ValidationSeverityType.Errors Then
        '                    'handle error accordingly
        '                    If ErrorMessage = "" Then
        '                        ErrorMessage = vItem.Message
        '                    Else
        '                        ErrorMessage &= "; " & vItem.Message
        '                    End If
        '                End If
        '            Next
        '        End If

        '        Return applyCashResponse.ResponseData.Success
        '    End If

        'Catch ex As Exception
        '    Using sqlCmd As New SQLexecuteObject(AppSettings("conn"))
        '        sqlCmd.queryOrStoredProc = "Insert into tbl_agentPayment_diamond_Errors (PolicyNumber, ErrorDescription) values ('" & policyNumber & "', '" & Replace(ex.ToString, "'", "''") & "')"
        '        sqlCmd.ExecuteStatement()
        '    End Using
        '    Throw
        'End Try

    End Function

    Private Function DoApplyBillingAccountPayment(cash As DCO.Billing.ApplyCash, ByRef ErrorMessage As String) As Boolean
        Dim request As New Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Request
        Dim applyCashResponse As New Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Response
        Try
            With request.RequestData
                .ApplyCash = cash    ' cash As DCO.Billing.ApplyCash
            End With

            Using billingProxy As New Proxies.BillingServiceProxy
                applyCashResponse = billingProxy.ApplyBillingAccountPayment(request)
            End Using

            If applyCashResponse IsNot Nothing AndAlso applyCashResponse.DiamondValidation.HasAnyItems Then
                For Each vItem As DCO.ValidationItem In applyCashResponse.DiamondValidation.ValidationItems
                    If vItem.ValidationSeverityType = DCE.ValidationSeverityType.Errors Then
                        'handle error accordingly
                        If ErrorMessage = "" Then
                            ErrorMessage = vItem.Message
                        Else
                            ErrorMessage &= "; " & vItem.Message
                        End If
                    End If
                Next
            End If

            Return applyCashResponse.ResponseData.Success

        Catch ex As Exception
            Dim cashJSON As String = "Nothing"
            Dim applyCashResponseJSON As String = "Nothing"
            If cash IsNot Nothing Then
                cashJSON = Newtonsoft.Json.JsonConvert.SerializeObject(cash)
            End If
            If applyCashResponse IsNot Nothing Then
                applyCashResponseJSON = Newtonsoft.Json.JsonConvert.SerializeObject(applyCashResponse)
            End If
            IFM.IFMErrorLogging.LogException(ex, "Error ApplyBillingAccountPayment; cash Object - " & cashJSON & "; applyCashResponse Object - " & applyCashResponseJSON)
            Using sqlCmd As New SQLexecuteObject(AppSettings("conn"))
                sqlCmd.queryOrStoredProc = "Insert into tbl_agentPayment_diamond_Errors (PolicyNumber, ErrorDescription, AccountNumber) values ('" & cash.PolicyNo & "', '" & Replace(ex.ToString, "'", "''") & "', '" & cash.BillingAccountId.GetAccountBillNumberFromBillingAccountId() & "')"
                sqlCmd.ExecuteStatement()
            End Using
            Throw
        End Try
    End Function

    Private Function DoApplyCash(cash As DCO.Billing.ApplyCash, ByRef ErrorMessage As String) As Boolean
        Dim request As New Diamond.Common.Services.Messages.BillingService.ApplyCash.Request
        Dim applyCashResponse As New Diamond.Common.Services.Messages.BillingService.ApplyCash.Response
        Try
            With request.RequestData
                .ApplyCash = cash    ' cash As DCO.Billing.ApplyCash
            End With

            Using billingProxy As New Proxies.BillingServiceProxy
                applyCashResponse = billingProxy.ApplyCash(request)
            End Using

            If applyCashResponse IsNot Nothing AndAlso applyCashResponse.DiamondValidation.HasAnyItems Then
                For Each vItem As DCO.ValidationItem In applyCashResponse.DiamondValidation.ValidationItems
                    If vItem.ValidationSeverityType = DCE.ValidationSeverityType.Errors Then
                        'handle error accordingly
                        If ErrorMessage = "" Then
                            ErrorMessage = vItem.Message
                        Else
                            ErrorMessage &= "; " & vItem.Message
                        End If
                    End If
                Next
            End If

            Return applyCashResponse.ResponseData.Success

        Catch ex As Exception
            Dim cashJSON As String = "Nothing"
            Dim applyCashResponseJSON As String = "Nothing"
            If cash IsNot Nothing Then
                cashJSON = Newtonsoft.Json.JsonConvert.SerializeObject(cash)
            End If
            If applyCashResponse IsNot Nothing Then
                applyCashResponseJSON = Newtonsoft.Json.JsonConvert.SerializeObject(applyCashResponse)
            End If
            IFM.IFMErrorLogging.LogException(ex, "Error ApplyCash; cash Object - " & cashJSON & "; applyCashResponse Object - " & applyCashResponseJSON)
            Using sqlCmd As New SQLexecuteObject(AppSettings("conn"))
                sqlCmd.queryOrStoredProc = "Insert into tbl_agentPayment_diamond_Errors (PolicyNumber, ErrorDescription) values ('" & cash.PolicyNo & "', '" & Replace(ex.ToString, "'", "''") & "')"
                sqlCmd.ExecuteStatement()
            End Using
            Throw
        End Try
    End Function


    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

            End If
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class