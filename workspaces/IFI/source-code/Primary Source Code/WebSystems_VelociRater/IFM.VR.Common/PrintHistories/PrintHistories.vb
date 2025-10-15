Namespace IFM.VR.Common.PrintHistories

    Public Class PrintHistories
        '6-5-14 Matt - Using Dons Code to get the byte stream of the pdf
        Public Enum PrintType
            All = 1
            JustWorksheet = 2
            JustApplication = 3
            Search = 4
            AllExceptDeclarations = 5
        End Enum

        Public Enum PrintFormDescriptionEvaluationType
            UseDefaultForPrintType = 1
            IgnoreFormDescription = 2
            OnlyUniqueFormDescriptions = 3
        End Enum

        Public Shared Function GetPrintHistories(policyId As String, loginName As String, loginPassword As String, printType As PrintType, Optional searchParm As String = "", Optional ByVal formDescriptionEvaluationType As PrintFormDescriptionEvaluationType = PrintFormDescriptionEvaluationType.UseDefaultForPrintType) As Byte()
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Dim formBytes As Byte() = Nothing
            Dim PFS As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing

            If policyId <> "" Then
                Select Case printType
                    Case PrintHistories.PrintType.JustWorksheet
                        PFS = GetDiamondPrintHistory(policyId, loginName, loginPassword, PrintType.JustWorksheet)
                    Case PrintHistories.PrintType.JustApplication
                        PFS = GetDiamondPrintHistory(policyId, loginName, loginPassword, PrintType.JustApplication)
                    Case PrintHistories.PrintType.Search
                        PFS = GetDiamondPrintHistory(policyId, loginName, loginPassword, PrintType.Search, searchParm)
                    Case Else
                        PFS = GetDiamondPrintHistory(policyId, loginName, loginPassword, ) ' pull all
                End Select

                If PFS IsNot Nothing Then
                    formBytes = GetPrintForm(PFS)
                End If

                Return formBytes
            End If
            Return Nothing
        End Function

        Public Shared Function GetDiamondPrintHistory(policyId As Int32, loginName As String, loginPassword As String, Optional ByVal pType As PrintType = PrintType.All, Optional ByVal searchText As String = "", Optional ByVal formDescriptionEvaluationType As PrintFormDescriptionEvaluationType = PrintFormDescriptionEvaluationType.UseDefaultForPrintType) As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
            Using dia As New DiamondWebClass.DiamondPrinting
                Dim forms As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing
                'updated 3/12/2013 to not send new credentials to Diamond

                If loginName <> "" AndAlso loginPassword <> "" Then
                    '5/7/2013 - doesn't need Try/Catch to prevent unhandled exception (since method in DiamondPrinting class already has it)
                    forms = dia.getPrintFormsForPolicyId(dia.loginDiamond(loginName, loginPassword), CInt(policyId))
                End If
                'updated 6/20/2014 for PrintFormDescriptionEvaluationType
                Dim onlyPullUniqueFormDescriptions As Boolean = False
                If formDescriptionEvaluationType <> Nothing Then
                    Select Case formDescriptionEvaluationType
                        Case PrintFormDescriptionEvaluationType.OnlyUniqueFormDescriptions
                            onlyPullUniqueFormDescriptions = True
                        Case PrintFormDescriptionEvaluationType.UseDefaultForPrintType
                            If pType <> Nothing Then
                                Select Case pType
                                    Case PrintType.JustApplication, PrintType.JustWorksheet
                                        onlyPullUniqueFormDescriptions = True
                                    Case Else 'Search, All
                                        'already defaulted to False above
                                End Select
                            End If
                        Case Else 'IgnoreFormDescription
                            'already defaulted to False above
                    End Select
                End If

                If forms IsNot Nothing Then
                    Dim pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing
                    Dim formDescriptions As New List(Of String) 'added 6/20/2014 for PrintFormDescriptionEvaluationType

                    For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                        Dim okayToUse As Boolean = False
                        If pType = PrintType.JustWorksheet AndAlso UCase(pf.Description).Contains("SHEET") = True Then
                            okayToUse = True
                        ElseIf pType = PrintType.JustApplication AndAlso UCase(pf.Description).Contains("APPLICATION") Then
                            okayToUse = True
                        ElseIf pType = PrintType.Search AndAlso UCase(pf.Description).Contains(UCase(searchText)) Then
                            okayToUse = True
                        ElseIf pType = PrintType.All Then
                            okayToUse = True
                        ElseIf pType = PrintType.AllExceptDeclarations AndAlso Not UCase(pf.Description).Contains("DECLARATION") Then
                            okayToUse = True
                        End If

                        If okayToUse = True AndAlso (onlyPullUniqueFormDescriptions = False OrElse formDescriptions.Contains(pf.Description) = False) Then
                            formDescriptions.Add(pf.Description) 'added 6/20/2014
                            If pfs Is Nothing Then
                                pfs = New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
                            End If

                            pfs.Add(pf)
                        End If
                    Next

                    Return pfs
                    'If pfs IsNot Nothing Then
                    '    Return GetPrintForm(pfs)
                    'End If
                End If
            End Using

            Return Nothing
        End Function

        Private Shared Function GetPrintForm(ByVal pf As Diamond.Common.Objects.Printing.PrintForm) As Byte()
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
#If DEBUG Then
                    Debugger.Break()
#End If
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

        Public Shared Function GetPrintForm(ByVal pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)) As Byte()
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
                        reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                    Catch ex As Exception
#If DEBUG Then
                        Debugger.Break()
#End If
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

        Public Shared Function GetPrintForm(ByVal pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm), ByRef errMsg As String) As Byte()
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
                        reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                    Catch ex As Exception
#If DEBUG Then
                        Debugger.Break()
#End If
                    End Try
                End Using

                If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                    Return reprintResponse.ResponseData.Data
                Else
                    If reprintResponse.DiamondValidation.HasErrors Then
                        For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                            If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                                errMsg &= diaVal.Message & " \n"
                            End If
                        Next
                    End If
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class

End Namespace