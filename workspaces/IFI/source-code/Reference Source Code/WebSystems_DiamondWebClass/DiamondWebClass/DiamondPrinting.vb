Public Class DiamondPrinting
    Implements IDisposable

    Public errMsg As String = ""

    Public Sub New()

    End Sub

    Public Function loginDiamond(ByVal userName As String, ByVal password As String) As Diamond.Common.Services.DiamondSecurityToken
        Dim reqLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Request
        Dim rspLogin As New Diamond.Common.Services.Messages.LoginService.GetDiamTokenForUsernamePassword.Response

        With reqLogin.RequestData
            .LoginName = userName
            .Password = password
        End With

        Using loginProxy As New Diamond.Common.Services.Proxies.LoginServiceProxy
            rspLogin = loginProxy.GetDiamTokenForUsernamePassword(reqLogin)
        End Using

        Return rspLogin.ResponseData.DiamondSecurityToken
    End Function

    Public Function getAllDecs(ByVal policyNumber As String) As DataTable
        Dim policyID As Integer = 0, htDecs As New Hashtable

        Dim dtDecs As New DataTable
        With dtDecs.Columns
            .Add("Title")
            .Add("PrintDate")
            .Add("ProcID")
            .Add("PolicyImage")
        End With

        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
            pno.UseDiamondData = True
            pno.GetPolicyInfo()
            If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
        End Using

        Try
            printRequest.RequestData.PolicyId = policyID
            printRequest.RequestData.PolicyImageNum = -1

            Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        If pf.FormCategoryTypeId = 1 Then
                            If Not htDecs.ContainsValue(pf.PrintProcessId) Then
                                dtDecs.Rows.Add(pf.Description, CDate(pf.AddedDate).ToShortDateString, pf.PrintProcessId, pf.PolicyImageNum)
                                htDecs.Add(pf.PolicyId, pf.PrintProcessId)
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            errMsg = ex.ToString
        End Try

        Return dtDecs
    End Function

    Public Function print_AutoIDCard(ByVal policyNumber As String) As Byte()
        Dim imageNum As Integer = 0, policyID As Integer = 0, decDate As DateTime = #1/1/1800#
        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
            pno.UseDiamondData = True
            pno.GetPolicyInfo()
            If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
        End Using

        Try
            With printRequest.RequestData
                .PolicyId = policyID
                .PolicyImageNum = -1
            End With

            Using printProxy As New Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        'If pf.FormNumber = "ID Card" Then
                        'updated 4/4/2012 for CAP
                        If UCase(pf.FormNumber).Contains("ID CARD") = True OrElse UCase(pf.Description).Contains("ID CARD") = True Then
                            reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                            reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                            imageNum = pf.PolicyImageNum
                            Exit For
                        End If
                    Next

                    With reprintRequest.RequestData
                        .PolicyId = policyID
                        .PolicyImageNum = imageNum
                    End With

                    Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                        reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                    End Using

                    If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                        Return reprintResponse.ResponseData.Data
                    Else
                        If reprintResponse.DiamondValidation.HasErrors Then
                            For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                    errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                End If
                            Next
                        End If
                        Return Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            errMsg = ex.ToString
            Return Nothing
        End Try

        Return Nothing
    End Function
    '4/19/2012 - added new function (w/ printXmlId)
    Public Function print_AutoIDCard_old(ByVal policyNumber As String, ByVal printXmlId As Integer, Optional ByVal policyID As Integer = 0, Optional ByVal imageNum As Integer = 0) As Byte()
        If printXmlId = Nothing OrElse printXmlId = 0 Then
            Return print_AutoIDCard(policyNumber)
        Else
            'Dim imageNum As Integer = 0, policyID As Integer = 0, decDate As DateTime = #1/1/1800#
            Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
            Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

            If policyID = Nothing OrElse policyID = 0 Then
                Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
                    pno.UseDiamondData = True
                    pno.GetPolicyInfo()
                    If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
                End Using
            End If

            Try
                With printRequest.RequestData
                    .PolicyId = policyID
                    If imageNum <> Nothing AndAlso imageNum <> 0 Then
                        .PolicyImageNum = imageNum
                    Else
                        .PolicyImageNum = -1
                    End If
                End With

                Using printProxy As New Proxies.PrintingServiceProxy
                    printResponse = printProxy.LoadPrintHistory(printRequest)
                End Using

                Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
                Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

                If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                    If printResponse.ResponseData.PrintHistory.Count > 0 Then
                        For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                            'If pf.FormNumber = "ID Card" Then
                            'updated 4/4/2012 for CAP
                            If (UCase(pf.FormNumber).Contains("ID CARD") = True OrElse UCase(pf.Description).Contains("ID CARD") = True) AndAlso pf.PrintXmlId = printXmlId Then
                                reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                imageNum = pf.PolicyImageNum
                                Exit For
                            End If
                        Next

                        With reprintRequest.RequestData
                            .PolicyId = policyID
                            .PolicyImageNum = imageNum
                        End With

                        Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                            reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                        End Using

                        If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                            Return reprintResponse.ResponseData.Data
                        Else
                            If reprintResponse.DiamondValidation.HasErrors Then
                                For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                    If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                        errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                    End If
                                Next
                            End If
                            Return Nothing
                        End If
                    End If
                End If
            Catch ex As Exception
                errMsg = ex.ToString
                Return Nothing
            End Try

            Return Nothing
        End If
    End Function
    'new methods 6/16/2015; old one renamed print_AutoIDCard_old
    Public Function print_AutoIDCard(ByVal policyNumber As String, ByVal printXmlId As Integer, Optional ByVal policyID As Integer = 0, Optional ByVal imageNum As Integer = 0) As Byte()
        Return print_AutoIDCard(Nothing, policyNumber, printXmlId, policyID, imageNum)
    End Function
    Enum PrintAllOrOne
        UseDefault = 0
        All = 1
        FirstOne = 2
        LastOne = 3
    End Enum
    Private Function AutoIdCards_Print_AllOrOne_Default() As PrintAllOrOne
        Dim allOrOne As PrintAllOrOne = PrintAllOrOne.All
        If ConfigurationManager.AppSettings("AutoIdCards_Print_AllOrOne") IsNot Nothing AndAlso ConfigurationManager.AppSettings("AutoIdCards_Print_AllOrOne").ToString <> "" Then
            Select Case UCase(ConfigurationManager.AppSettings("AutoIdCards_Print_AllOrOne").ToString)
                Case "FIRSTONE"
                    allOrOne = PrintAllOrOne.FirstOne
                Case "LASTONE"
                    allOrOne = PrintAllOrOne.LastOne
                Case Else
                    allOrOne = PrintAllOrOne.All
            End Select
        End If
        Return allOrOne
    End Function
    Private Function AutoIdCards_Print_IgnorePrintXmlId_WhenPolicyIdAndImageNum() As Boolean
        Dim ignore As Boolean = False

        If ConfigurationManager.AppSettings("AutoIdCards_Print_IgnorePrintXmlId_WhenPolicyIdAndImageNum") IsNot Nothing AndAlso ConfigurationManager.AppSettings("AutoIdCards_Print_IgnorePrintXmlId_WhenPolicyIdAndImageNum").ToString <> "" Then
            Select Case UCase(ConfigurationManager.AppSettings("AutoIdCards_Print_IgnorePrintXmlId_WhenPolicyIdAndImageNum").ToString)
                Case "TRUE", "YES"
                    ignore = True
            End Select
        End If

        Return ignore
    End Function
    Public Function print_AutoIDCard(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal policyNumber As String, ByVal printXmlId As Integer, Optional ByVal policyID As Integer = 0, Optional ByVal imageNum As Integer = 0, Optional ByVal allOrOne As PrintAllOrOne = PrintAllOrOne.UseDefault) As Byte()
        ''If printXmlId = Nothing OrElse printXmlId = 0 Then 'removed IF 6/16/2015 pm so we can ignore printXmlId if needed (and just use policyId and imageNum)
        ''    Return print_AutoIDCard(policyNumber)
        ''Else

        ''added 6/16/2015 pm in case we need to ignore printXmlId... in case endorsements have different printXmlId
        'If policyID > 0 AndAlso imageNum > 0 AndAlso AutoIdCards_Print_IgnorePrintXmlId_WhenPolicyIdAndImageNum() = True Then
        '    printXmlId = 0
        'End If

        'If policyID = Nothing OrElse policyID = 0 Then
        '    Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
        '        pno.UseDiamondData = True
        '        pno.GetPolicyInfo()
        '        If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
        '    End Using
        'End If

        'Dim autoIdCardForms As List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing
        'autoIdCardForms = SpecificPrintFormsForPolicyIdAndImageNumAndPrintXmlId(diaSecurityToken, PrintFormType.AutoIdCards, policyID, imageNum, printXmlId)
        'If autoIdCardForms IsNot Nothing AndAlso autoIdCardForms.Count > 0 Then
        '    If allOrOne = PrintAllOrOne.UseDefault Then
        '        allOrOne = AutoIdCards_Print_AllOrOne_Default()
        '    End If
        '    Select Case allOrOne
        '        Case PrintAllOrOne.FirstOne
        '            GetPrintForm(autoIdCardForms.Item(0))
        '        Case PrintAllOrOne.LastOne
        '            GetPrintForm(autoIdCardForms.Item(autoIdCardForms.Count - 1))
        '        Case Else
        '            Return GetPrintForm(autoIdCardForms)
        '    End Select
        'End If

        'Return Nothing
        ''End If
        'updated 6/17/2015 to use new method
        Dim printXmlIds As List(Of Integer) = Nothing
        If printXmlId > 0 Then
            printXmlIds = New List(Of Integer)
            printXmlIds.Add(printXmlId)
        End If
        Return print_AutoIDCard(diaSecurityToken, policyNumber, printXmlIds, policyID, imageNum, allOrOne)
    End Function
    'added overload 6/17/2015
    Public Function print_AutoIDCard(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal policyNumber As String, ByVal printXmlIds As List(Of Integer), Optional ByVal policyID As Integer = 0, Optional ByVal imageNum As Integer = 0, Optional ByVal allOrOne As PrintAllOrOne = PrintAllOrOne.UseDefault) As Byte()
        'added 6/16/2015 pm in case we need to ignore printXmlId... in case endorsements have different printXmlId
        If policyID > 0 AndAlso imageNum > 0 AndAlso AutoIdCards_Print_IgnorePrintXmlId_WhenPolicyIdAndImageNum() = True Then
            If printXmlIds IsNot Nothing Then
                If printXmlIds.Count > 0 Then
                    For Each id As Integer In printXmlIds
                        id = Nothing
                    Next
                    printXmlIds.Clear()
                End If
                printXmlIds = Nothing
            End If
        End If

        If policyID = Nothing OrElse policyID = 0 Then
            Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
                pno.UseDiamondData = True
                pno.GetPolicyInfo()
                If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
            End Using
        End If

        Dim autoIdCardForms As List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing
        autoIdCardForms = SpecificPrintFormsForPolicyIdAndImageNumAndPrintXmlIds(diaSecurityToken, PrintFormType.AutoIdCards, policyID, imageNum, printXmlIds)
        If autoIdCardForms IsNot Nothing AndAlso autoIdCardForms.Count > 0 Then
            If allOrOne = PrintAllOrOne.UseDefault Then
                allOrOne = AutoIdCards_Print_AllOrOne_Default()
            End If
            Select Case allOrOne
                Case PrintAllOrOne.FirstOne
                    GetPrintForm(autoIdCardForms.Item(0))
                Case PrintAllOrOne.LastOne
                    GetPrintForm(autoIdCardForms.Item(autoIdCardForms.Count - 1))
                Case Else
                    Return GetPrintForm(autoIdCardForms)
            End Select
        End If

        Return Nothing
    End Function

    Public Function printBillingPDF(ByVal policyNumber As String, Optional ByVal printProcID As Integer = 0, Optional ByVal pfCategory As Integer = 0) As Byte()
        Dim imageNum As Integer = 0, policyID As Integer = 0, decDate As DateTime = #1/1/1800#
        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
            pno.UseDiamondData = True
            pno.GetPolicyInfo()
            If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
        End Using

        Try
            printRequest.RequestData.PolicyId = policyID
            printRequest.RequestData.PolicyImageNum = -1

            Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        If pf.PrintProcessId = printProcID Then
                            Select Case pfCategory
                                Case Is = 2
                                    If pf.FormCategoryTypeId = 2 Or pf.Description.ToLower.Contains("invoice") Then
                                        If reprintRequest.RequestData.PrintForms Is Nothing Then reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                        imageNum = pf.PolicyImageNum
                                        Exit For
                                    End If
                                Case Is = 3
                                    If (pf.FormCategoryTypeId = 2 Or pf.FormCategoryTypeId = 4 Or pf.FormCategoryTypeId = 5) Or (pf.Description.ToLower.Contains("advance notice") Or pf.Description.ToLower.Contains("final notice") Or pf.Description.ToLower.Contains("final expiration") Or pf.Description.ToLower.Contains("final cancellation") Or pf.Description.ToLower.Contains("receipt notice") Or pf.Description.ToLower.Contains("pay plan change notice") Or pf.Description.ToLower.Contains("renewal expiration") Or pf.Description.ToLower.Contains("cancel") Or pf.Description.ToLower.Contains("notice") Or pf.Description.ToLower.Contains("declaration")) Then
                                        If reprintRequest.RequestData.PrintForms Is Nothing Then reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                        imageNum = pf.PolicyImageNum
                                        Exit For
                                    End If
                            End Select
                            'If pf.FormCategoryTypeId = 0 And  Then     3/4/7 = notice   2 = invoice
                            'If pf.PrintProcessId = printProcID Then
                            '    If reprintRequest.RequestData.PrintForms Is Nothing Then reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                            '    reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                            '    imageNum = pf.PolicyImageNum
                            '    Exit For
                            'End If
                        End If
                    Next

                    If reprintRequest.RequestData.PrintForms IsNot Nothing Then
                        With reprintRequest.RequestData
                            .PolicyId = policyID
                            .PolicyImageNum = imageNum
                        End With

                        Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                            reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                        End Using

                        If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                            Return reprintResponse.ResponseData.Data
                        Else
                            If reprintResponse.DiamondValidation.HasErrors Then
                                For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                    If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                        errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                    End If
                                Next
                            End If
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            errMsg = ex.ToString
            Return Nothing
        End Try

        Return Nothing
    End Function
    Public Function printBillingPDF_WithPolicyId(ByVal policyNumber As String, ByVal policyID As Integer, Optional ByVal printProcID As Integer = 0, Optional ByVal pfCategory As Integer = 0) As Byte()
        Dim imageNum As Integer = 0, decDate As DateTime = #1/1/1800#
        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Try
            printRequest.RequestData.PolicyId = policyID
            printRequest.RequestData.PolicyImageNum = -1

            Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        If pf.PrintProcessId = printProcID Then
                            Select Case pfCategory
                                Case Is = 2
                                    If pf.FormCategoryTypeId = 2 Or pf.Description.ToLower.Contains("invoice") Then
                                        If reprintRequest.RequestData.PrintForms Is Nothing Then reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                        imageNum = pf.PolicyImageNum
                                        Exit For
                                    End If
                                Case Is = 3
                                    If (pf.FormCategoryTypeId = 2 Or pf.FormCategoryTypeId = 4 Or pf.FormCategoryTypeId = 5) Or (pf.Description.ToLower.Contains("advance notice") Or pf.Description.ToLower.Contains("final notice") Or pf.Description.ToLower.Contains("final expiration") Or pf.Description.ToLower.Contains("final cancellation") Or pf.Description.ToLower.Contains("receipt notice") Or pf.Description.ToLower.Contains("pay plan change notice") Or pf.Description.ToLower.Contains("renewal expiration") Or pf.Description.ToLower.Contains("cancel") Or pf.Description.ToLower.Contains("notice") Or pf.Description.ToLower.Contains("declaration")) Then
                                        If reprintRequest.RequestData.PrintForms Is Nothing Then reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                        imageNum = pf.PolicyImageNum
                                        Exit For
                                    End If
                            End Select
                            'If pf.FormCategoryTypeId = 0 And  Then     3/4/7 = notice   2 = invoice
                            'If pf.PrintProcessId = printProcID Then
                            '    If reprintRequest.RequestData.PrintForms Is Nothing Then reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                            '    reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                            '    imageNum = pf.PolicyImageNum
                            '    Exit For
                            'End If
                        End If
                    Next

                    If reprintRequest.RequestData.PrintForms IsNot Nothing Then
                        With reprintRequest.RequestData
                            .PolicyId = policyID
                            .PolicyImageNum = imageNum
                        End With

                        Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                            reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                        End Using

                        If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                            Return reprintResponse.ResponseData.Data
                        Else
                            If reprintResponse.DiamondValidation.HasErrors Then
                                For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                    If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                        errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                    End If
                                Next
                            End If
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            errMsg = ex.ToString
            Return Nothing
        End Try

        Return Nothing
    End Function

    Public Function TestCall() As String
        Dim imageNum As Integer = 0, policyID As Integer = 0, decDate As DateTime = #1/1/1800#
        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Using pno As New PolicyNumberObject("PPA2003124", "", AppSettings("connDiamond"))
            pno.UseDiamondData = True
            pno.GetPolicyInfo()
            If pno.hasPolicyInfo = True Then policyID = pno.PolicyID
            If policyID = 0 Then Return "Empty"
        End Using

        Try
            printRequest.RequestData.PolicyId = policyID
            printRequest.RequestData.PolicyImageNum = -1
            printRequest.DiamondSecurityToken = loginDiamond("PrintServices", "PrintServices")

            Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        If pf.FormCategoryTypeId = 1 AndAlso pf.AddedDate > decDate Then
                            reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                            reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                            imageNum = pf.PolicyImageNum
                            decDate = pf.AddedDate
                        End If
                    Next

                    reprintRequest.DiamondSecurityToken = loginDiamond("PrintServices", "PrintServices")
                    With reprintRequest.RequestData
                        .PolicyId = policyID
                        .PolicyImageNum = imageNum
                    End With

                    Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                        reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                    End Using

                    If reprintResponse IsNot Nothing Then Return "Response found again" Else Return "No reprint response found"

                    If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                        Return "Success"
                    Else
                        If reprintResponse.DiamondValidation.HasErrors Then
                            For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                    ' errorDetail &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                    errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                End If
                            Next
                        End If
                        Return Nothing
                    End If
                Else
                    Return "No forms found"
                End If
            Else
                Return "No print response"
            End If
        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function

    Public Function printDec(ByVal policyNumber As String, ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, Optional ByVal printXML As Integer = 0) As Byte()
        Dim imageNum As Integer = 0, policyID As Integer = 0, decDate As DateTime = #1/1/1800#
        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Using pno As New PolicyNumberObject(policyNumber, "", AppSettings("connDiamond"))
            pno.UseDiamondData = True
            pno.GetPolicyInfo()
            If pno.hasPolicyInfo = True Then policyID = pno.PolicyID

            'tracking
            'If policyID > 0 Then errorDetail = policyID
        End Using

        Try
            printRequest.RequestData.PolicyId = policyID
            printRequest.RequestData.PolicyImageNum = -1
            'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
            If diaSecurityToken IsNot Nothing Then
                printRequest.DiamondSecurityToken = diaSecurityToken
            End If

            Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        If printXML > 0 Then
                            If pf.FormCategoryTypeId = 1 And pf.PrintXmlId = printXML Then
                                reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                imageNum = pf.PolicyImageNum
                                Exit For
                            ElseIf pf.FormCategoryTypeId = 1 AndAlso (decDate = #1/1/1800# Or pf.AddedDate > decDate) Then
                                reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                imageNum = pf.PolicyImageNum
                                decDate = pf.AddedDate
                            End If
                        Else
                            If pf.FormCategoryTypeId = 1 AndAlso pf.AddedDate > decDate Then
                                reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                                reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                                imageNum = pf.PolicyImageNum
                                decDate = pf.AddedDate
                            End If
                        End If
                    Next

                    'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
                    If diaSecurityToken IsNot Nothing Then
                        reprintRequest.DiamondSecurityToken = diaSecurityToken
                    End If
                    With reprintRequest.RequestData
                        .PolicyId = policyID
                        .PolicyImageNum = imageNum
                    End With

                    Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                        reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                    End Using

                    If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                        Return reprintResponse.ResponseData.Data
                    Else
                        If reprintResponse.DiamondValidation.HasErrors Then
                            For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                    ' errorDetail &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                    errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                End If
                            Next
                        End If
                        Return Nothing
                    End If
                Else
                    ' errorDetail = "No forms found in print history."
                End If
            Else
                'errorDetail = "No response data for load print history."
            End If
        Catch ex As Exception
            errMsg = ex.ToString
            ' errorDetail = ex.ToString
            Return Nothing
        End Try

        Return Nothing
    End Function
    Public Function printDec(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal policyId As Integer, ByVal printXML As Integer, ByVal formDescription As String) As Byte()

        Return printDec_PreferredPrintRecipientId(diaSecurityToken, policyId, printXML, formDescription)

        'Dim imageNum As Integer = 0, decDate As DateTime = #1/1/1800#
        'Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        'Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        'Try
        '    printRequest.RequestData.PolicyId = policyId
        '    printRequest.RequestData.PolicyImageNum = -1
        '    'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
        '    If diaSecurityToken IsNot Nothing Then
        '        printRequest.DiamondSecurityToken = diaSecurityToken
        '    End If

        '    Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
        '        printResponse = printProxy.LoadPrintHistory(printRequest)
        '    End Using

        '    Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
        '    Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

        '    If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
        '        If printResponse.ResponseData.PrintHistory.Count > 0 Then
        '            Dim hasMatchOnPrintXmlId As Boolean = False
        '            Dim isDeclaration As Boolean = False
        '            For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
        '                If pf.FormCategoryTypeId = 1 OrElse (pf.FormCategoryTypeId = 0 AndAlso UCase(pf.Description).Contains("DECLARATION") = True) Then
        '                    isDeclaration = True
        '                Else
        '                    isDeclaration = False
        '                End If
        '                If printXML > 0 Then
        '                    If isDeclaration = True AndAlso pf.PrintXmlId = printXML AndAlso UCase(pf.Description) = UCase(formDescription) Then
        '                        reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
        '                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        '                        imageNum = pf.PolicyImageNum
        '                        hasMatchOnPrintXmlId = True
        '                        Exit For
        '                    ElseIf isDeclaration = True AndAlso pf.PrintXmlId = printXML Then
        '                        reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
        '                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        '                        imageNum = pf.PolicyImageNum
        '                        decDate = pf.AddedDate
        '                        hasMatchOnPrintXmlId = True
        '                    ElseIf isDeclaration = True AndAlso hasMatchOnPrintXmlId = False AndAlso (decDate = #1/1/1800# Or pf.AddedDate > decDate) Then
        '                        reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
        '                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        '                        imageNum = pf.PolicyImageNum
        '                        decDate = pf.AddedDate
        '                    End If
        '                Else
        '                    If isDeclaration = True AndAlso pf.AddedDate > decDate Then
        '                        reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
        '                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        '                        imageNum = pf.PolicyImageNum
        '                        decDate = pf.AddedDate
        '                    End If
        '                End If
        '            Next

        '            'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
        '            If diaSecurityToken IsNot Nothing Then
        '                reprintRequest.DiamondSecurityToken = diaSecurityToken
        '            End If
        '            With reprintRequest.RequestData
        '                .PolicyId = policyId
        '                .PolicyImageNum = imageNum
        '            End With

        '            Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
        '                reprintResponse = reprintProxy.ReprintJob(reprintRequest)
        '            End Using

        '            If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
        '                Return reprintResponse.ResponseData.Data
        '            Else
        '                If reprintResponse.DiamondValidation.HasErrors Then
        '                    For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
        '                        If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
        '                            ' errorDetail &= diaVal.Message & Environment.NewLine & Environment.NewLine
        '                            errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
        '                        End If
        '                    Next
        '                End If
        '                Return Nothing
        '            End If
        '        Else
        '            ' errorDetail = "No forms found in print history."
        '        End If
        '    Else
        '        'errorDetail = "No response data for load print history."
        '    End If
        'Catch ex As Exception
        '    errMsg = ex.ToString
        '    ' errorDetail = ex.ToString
        '    Return Nothing
        'End Try

        'If errMsg = "" Then
        '    errMsg = "Nothing found for policyId " & policyId.ToString & ", printXml " & printXML.ToString & ", and form description '" & formDescription & "'."
        'End If
        'Return Nothing
    End Function

    Public Function printDec_PreferredPrintRecipientId(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal policyId As Integer, ByVal printXML As Integer, ByVal formDescription As String, Optional ByVal PrintRecipientId As Integer = 0) As Byte()
        If PrintRecipientId = 0 Then
            Dim chc As New CommonHelperClass
            PrintRecipientId = chc.ConfigurationAppSettingValueAsInteger("Dec_PrintRecipientId")
        End If

        Dim imageNum As Integer = 0, decDate As DateTime = #1/1/1800#
        Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        Try
            printRequest.RequestData.PolicyId = policyId
            printRequest.RequestData.PolicyImageNum = -1
            'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
            If diaSecurityToken IsNot Nothing Then
                printRequest.DiamondSecurityToken = diaSecurityToken
            End If

            Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                printResponse = printProxy.LoadPrintHistory(printRequest)
            End Using

            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                If printResponse.ResponseData.PrintHistory.Count > 0 Then
                    Dim hasMatchOnPrintXmlId As Boolean = False
                    Dim isDeclaration As Boolean = False
                    Dim pfToUse As Diamond.Common.Objects.Printing.PrintForm = Nothing
                    Dim hasMatchOnFormDesc As Boolean = False

                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory
                        If pf.FormCategoryTypeId = 1 OrElse (pf.FormCategoryTypeId = 0 AndAlso UCase(pf.Description).Contains("DECLARATION") = True) Then
                            isDeclaration = True
                        Else
                            isDeclaration = False
                        End If

                        If printXML > 0 Then
                            If isDeclaration = True AndAlso pf.PrintXmlId = printXML AndAlso UCase(pf.Description) = UCase(formDescription) Then
                                Dim isPrintRecipientIdOK As Boolean = False
                                If PrintRecipientId = 0 OrElse pf.PrintRecipients?.FirstOrDefault.PrintRecipientId = PrintRecipientId Then
                                    isPrintRecipientIdOK = True
                                End If

                                If pfToUse Is Nothing OrElse isPrintRecipientIdOK = True Then
                                    pfToUse = pf
                                    imageNum = pf.PolicyImageNum
                                    hasMatchOnPrintXmlId = True
                                    hasMatchOnFormDesc = True
                                End If

                                If isPrintRecipientIdOK = True Then
                                    Exit For
                                End If

                            ElseIf isDeclaration = True AndAlso hasMatchOnFormDesc = False AndAlso pf.PrintXmlId = printXML Then
                                pfToUse = pf
                                imageNum = pf.PolicyImageNum
                                decDate = pf.AddedDate
                                hasMatchOnPrintXmlId = True

                            ElseIf isDeclaration = True AndAlso hasMatchOnFormDesc = False AndAlso hasMatchOnPrintXmlId = False AndAlso (decDate = #1/1/1800# Or pf.AddedDate > decDate) Then
                                pfToUse = pf
                                imageNum = pf.PolicyImageNum
                                decDate = pf.AddedDate
                            End If
                        Else
                            If isDeclaration = True AndAlso pf.AddedDate > decDate Then
                                pfToUse = pf
                                imageNum = pf.PolicyImageNum
                                decDate = pf.AddedDate
                            End If
                        End If
                    Next

                    If pfToUse IsNot Nothing Then
                        reprintRequest.RequestData.PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                        reprintRequest.RequestData.PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pfToUse})

                    End If


                    'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
                    If diaSecurityToken IsNot Nothing Then
                        reprintRequest.DiamondSecurityToken = diaSecurityToken
                    End If
                    With reprintRequest.RequestData
                        .PolicyId = policyId
                        .PolicyImageNum = imageNum
                    End With

                    Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                        reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                    End Using

                    If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                        Return reprintResponse.ResponseData.Data
                    Else
                        If reprintResponse.DiamondValidation.HasErrors Then
                            For Each diaVal As DCO.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                                If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                    ' errorDetail &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                    errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                                End If
                            Next
                        End If
                        Return Nothing
                    End If
                Else
                    ' errorDetail = "No forms found in print history."
                End If
            Else
                'errorDetail = "No response data for load print history."
            End If
        Catch ex As Exception
            errMsg = ex.ToString
            ' errorDetail = ex.ToString
            Return Nothing
        End Try

        If errMsg = "" Then
            errMsg = "Nothing found for policyId " & policyId.ToString & ", printXml " & printXML.ToString & ", and form description '" & formDescription & "'."
        End If
        Return Nothing
    End Function

    Public Function getPrintFormsForPolicyId(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal policyId As Integer) As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
        'Dim imageNum As Integer = 0, decDate As DateTime = #1/1/1800#
        'Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
        'Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

        'Try
        '    printRequest.RequestData.PolicyId = policyId
        '    printRequest.RequestData.PolicyImageNum = -1
        '    'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
        '    If diaSecurityToken IsNot Nothing Then
        '        printRequest.DiamondSecurityToken = diaSecurityToken
        '    End If

        '    Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
        '        printResponse = printProxy.LoadPrintHistory(printRequest)
        '    End Using

        '    Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
        '    Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

        '    If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
        '        If printResponse.ResponseData.PrintHistory.Count > 0 Then
        '            Return printResponse.ResponseData.PrintHistory
        '            For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory

        '            Next


        '        Else
        '            ' errorDetail = "No forms found in print history."
        '        End If
        '    Else
        '        'errorDetail = "No response data for load print history."
        '    End If
        'Catch ex As Exception
        '    errMsg = ex.ToString
        '    ' errorDetail = ex.ToString
        '    Return Nothing
        'End Try

        'If errMsg = "" Then
        '    errMsg = "Nothing found for policyId " & policyId.ToString & "."
        'End If
        'Return Nothing

        'updated 6/16/2015 to call new method
        Return getPrintFormsForPolicyIdAndImageNum(diaSecurityToken, policyId)
    End Function
    'added 6/16/2015; code originally used in QuickQuote for VR
    Public Function GetPrintForm(ByVal pf As Diamond.Common.Objects.Printing.PrintForm) As Byte()
        Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
        Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

        With reprintRequest.RequestData
            .PolicyId = pf.PolicyId
            .PolicyImageNum = pf.PolicyImageNum

            .PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
            .PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        End With

        Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
            Try
                '5/7/2013 - enclosed in Try/Catch to prevent unhandled exception (so page would still work)
                reprintResponse = reprintProxy.ReprintJob(reprintRequest)
            Catch ex As Exception

            End Try
        End Using

        If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
            Return reprintResponse.ResponseData.Data
        Else
            If reprintResponse.DiamondValidation.HasErrors Then
                For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                    If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                        'errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                    End If
                Next
            End If
            Return Nothing
        End If
    End Function
    Public Function GetPrintForm(ByVal pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)) As Byte()
        If pfs IsNot Nothing AndAlso pfs.Count > 0 Then
            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            With reprintRequest.RequestData
                .PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                For Each pf As Diamond.Common.Objects.Printing.PrintForm In pfs
                    .PolicyId = pf.PolicyId
                    .PolicyImageNum = pf.PolicyImageNum

                    .PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                Next

            End With

            Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                Try
                    '5/7/2013 - enclosed in Try/Catch to prevent unhandled exception (so page would still work)
                    reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                Catch ex As Exception

                End Try
            End Using

            If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                Return reprintResponse.ResponseData.Data
            Else
                If reprintResponse.DiamondValidation.HasErrors Then
                    For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                        If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                            'errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                        End If
                    Next
                End If
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function
    Public Function getPrintFormsForPolicyIdAndImageNum(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal policyId As Integer, Optional ByVal imageNum As Integer = -1) As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
        If policyId > 0 Then
            Dim printRequest As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Request
            Dim printResponse As New Diamond.Common.Services.Messages.PrintingService.LoadPrintHistory.Response

            Try
                If imageNum = 0 Then
                    imageNum = -1
                End If
                printRequest.RequestData.PolicyId = policyId
                printRequest.RequestData.PolicyImageNum = imageNum
                'updated 5/31/2013 to not set security token if nothing; so dec page can send nothing without adding an overload that doesn't have the parameter
                If diaSecurityToken IsNot Nothing Then
                    printRequest.DiamondSecurityToken = diaSecurityToken
                End If

                Using printProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                    printResponse = printProxy.LoadPrintHistory(printRequest)
                End Using

                Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
                Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

                If printResponse.ResponseData IsNot Nothing AndAlso printResponse.ResponseData.PrintHistory IsNot Nothing Then
                    If printResponse.ResponseData.PrintHistory.Count > 0 Then
                        Return printResponse.ResponseData.PrintHistory
                        For Each pf As Diamond.Common.Objects.Printing.PrintForm In printResponse.ResponseData.PrintHistory

                        Next


                    Else
                        ' errorDetail = "No forms found in print history."
                    End If
                Else
                    'errorDetail = "No response data for load print history."
                End If
            Catch ex As Exception
                errMsg = ex.ToString
                ' errorDetail = ex.ToString
                Return Nothing
            End Try
        End If

        If errMsg = "" Then
            errMsg = "Nothing found for policyId " & policyId.ToString & "."
        End If
        Return Nothing
    End Function
    Enum PrintFormType
        All = 1
        Declarations = 2
        AutoIdCards = 3
        Applications = 4
        Worksheets = 5
    End Enum
    Public Function SpecificPrintFormsForPolicyIdAndImageNumAndPrintXmlId(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal type As PrintFormType, ByVal policyId As Integer, Optional ByVal imageNum As Integer = -1, Optional ByVal printXmlId As Integer = 0) As List(Of Diamond.Common.Objects.Printing.PrintForm)
        'Dim specifiedForms As List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing

        'Dim allForms As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing
        'allForms = getPrintFormsForPolicyIdAndImageNum(diaSecurityToken, policyId, imageNum)
        'If allForms IsNot Nothing AndAlso allForms.Count > 0 Then
        '    For Each pf As Diamond.Common.Objects.Printing.PrintForm In allForms
        '        Dim okayToAdd As Boolean = False
        '        If printXmlId <= 0 OrElse (printXmlId > 0 AndAlso pf.PrintXmlId = printXmlId) Then
        '            'Select Case type
        '            '    Case PrintFormType.Declarations
        '            '        If pf.FormCategoryTypeId = 1 OrElse (pf.FormCategoryTypeId = 0 AndAlso UCase(pf.Description).Contains("DECLARATION") = True) Then
        '            '            okayToAdd = True
        '            '        End If
        '            '    Case PrintFormType.AutoIdCards
        '            '        If UCase(pf.FormNumber).Contains("ID CARD") = True OrElse UCase(pf.Description).Contains("ID CARD") = True Then
        '            '            okayToAdd = True
        '            '        End If
        '            '    Case PrintFormType.Applications
        '            '        If UCase(pf.Description).Contains("APPLICATION") Then
        '            '            okayToAdd = True
        '            '        End If
        '            '    Case PrintFormType.Worksheets
        '            '        If UCase(pf.Description).Contains("SHEET") = True Then
        '            '            okayToAdd = True
        '            '        End If
        '            '    Case Else 'All
        '            '        okayToAdd = True
        '            'End Select
        '            'updated 6/17/2015 to use new function
        '            okayToAdd = IsPrintFormType(pf, type)
        '        End If

        '        If okayToAdd = True Then
        '            If specifiedForms Is Nothing Then
        '                specifiedForms = New List(Of Diamond.Common.Objects.Printing.PrintForm)
        '            End If
        '            specifiedForms.Add(pf)
        '        End If
        '    Next
        'End If

        'Return specifiedForms
        'updated 6/17/2015 to use new method
        Dim printXmlIds As List(Of Integer) = Nothing
        If printXmlId > 0 Then
            printXmlIds = New List(Of Integer)
            printXmlIds.Add(printXmlId)
        End If
        Return SpecificPrintFormsForPolicyIdAndImageNumAndPrintXmlIds(diaSecurityToken, type, policyId, imageNum, printXmlIds)
    End Function
    'added 6/17/2015 to separate logic
    Public Function IsPrintFormType(ByVal pf As Diamond.Common.Objects.Printing.PrintForm, ByVal type As PrintFormType) As Boolean
        Dim isType As Boolean = False

        If pf IsNot Nothing Then
            Select Case type
                Case PrintFormType.Declarations
                    If pf.FormCategoryTypeId = 1 OrElse (pf.FormCategoryTypeId = 0 AndAlso UCase(pf.Description).Contains("DECLARATION") = True) Then
                        isType = True
                    End If
                Case PrintFormType.AutoIdCards
                    If UCase(pf.FormNumber).Contains("ID CARD") = True OrElse UCase(pf.Description).Contains("ID CARD") = True Then
                        isType = True
                    End If
                Case PrintFormType.Applications
                    If UCase(pf.Description).Contains("APPLICATION") Then
                        isType = True
                    End If
                Case PrintFormType.Worksheets
                    If UCase(pf.Description).Contains("SHEET") = True Then
                        isType = True
                    End If
                Case Else 'All
                    isType = True
            End Select
        End If

        Return isType
    End Function
    'added overload 6/17/2015
    Public Function SpecificPrintFormsForPolicyIdAndImageNumAndPrintXmlIds(ByVal diaSecurityToken As Diamond.Common.Services.DiamondSecurityToken, ByVal type As PrintFormType, ByVal policyId As Integer, Optional ByVal imageNum As Integer = -1, Optional ByVal printXmlIds As List(Of Integer) = Nothing) As List(Of Diamond.Common.Objects.Printing.PrintForm)
        Dim specifiedForms As List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing

        Dim allForms As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing
        allForms = getPrintFormsForPolicyIdAndImageNum(diaSecurityToken, policyId, imageNum)
        If allForms IsNot Nothing AndAlso allForms.Count > 0 Then
            For Each pf As Diamond.Common.Objects.Printing.PrintForm In allForms
                Dim okayToAdd As Boolean = False
                Dim hasPosInt As Boolean = False
                hasPosInt = ListContainsPositiveInt(printXmlIds)
                If hasPosInt = False OrElse (hasPosInt = True AndAlso printXmlIds.Contains(pf.PrintXmlId) = True) Then
                    okayToAdd = IsPrintFormType(pf, type)
                End If

                If okayToAdd = True Then
                    If specifiedForms Is Nothing Then
                        specifiedForms = New List(Of Diamond.Common.Objects.Printing.PrintForm)
                    End If
                    specifiedForms.Add(pf)
                End If
            Next
        End If

        Return specifiedForms
    End Function
    Private Function ListContainsPositiveInt(ByVal ints As List(Of Integer)) As Boolean
        Dim hasPosInt As Boolean = False

        If ints IsNot Nothing AndAlso ints.Count > 0 Then
            For Each i As Integer In ints
                If i > 0 Then
                    hasPosInt = True
                    Exit For
                End If
            Next
        End If

        Return hasPosInt
    End Function

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
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
