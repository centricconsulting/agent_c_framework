Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Printing
    Public Module Printing
        Public Function FilterPrintHistory(AllImages As Boolean,
                                           Description As String,
                                           FormCategoryId As Integer,
                                           PolicyId As Integer,
                                           PolicyImageNum As Integer,
                                           Optional ByRef e As Exception = Nothing) As DCO.InsCollection(Of DCO.Printing.PrintForm)
            Dim req As New DCSM.PrintingService.FilterPrintHistory.Request
            Dim res As New DCSM.PrintingService.FilterPrintHistory.Response

            With req.RequestData
                .AllImages = AllImages
                .Description = Description
                .FormCategoryId = FormCategoryId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Printing.FilterPrintHistory(res, req, e)) Then
                If (res.ResponseData.PrintHistory IsNot Nothing) Then
                    Return res.ResponseData.PrintHistory
                End If
            End If

            Return Nothing
        End Function

        Public Function GetPolicyPrintDistribution(PolicyId As Integer,
                                                   Optional ByRef e As Exception = Nothing) As DCO.Printing.PolicyPrintDistribution
            Dim req As New DCSM.PrintingService.GetPolicyPrintDistribution.Request
            Dim res As New DCSM.PrintingService.GetPolicyPrintDistribution.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Printing.GetPolicyPrintDistribution(res, req, e)) Then
                If (res.ResponseData.PrintDistribution IsNot Nothing) Then
                    Return res.ResponseData.PrintDistribution
                End If
            End If

            Return Nothing
        End Function

        Public Function GetPrintReportingData(ReportFromDate As Date,
                                              ReportId As Integer,
                                              ReportToDate As Date,
                                              Optional ByRef e As Exception = Nothing) As System.Data.DataSet
            Dim req As New DCSM.PrintingService.GetPrintReportingData.Request
            Dim res As New DCSM.PrintingService.GetPrintReportingData.Response

            With req.RequestData
                .ReportFromDate = ReportFromDate
                .ReportId = ReportId
                .ReportToDate = ReportToDate
            End With

            If (IFMS.Printing.GetPrintReportingData(res, req, e)) Then
                If (res.ResponseData.Data IsNot Nothing) Then
                    Return res.ResponseData.Data
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadEliosHistory(PolicyId As Integer,
                                         PolicyImageNum As Integer,
                                         Optional ByRef e As Exception = Nothing) As DCO.InsCollection(Of DCO.Printing.EliosProcess)
            Dim req As New DCSM.PrintingService.LoadEliosHistory.Request
            Dim res As New DCSM.PrintingService.LoadEliosHistory.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Printing.LoadEliosHistory(res, req, e)) Then
                If (res.ResponseData.EliosHistory IsNot Nothing) Then
                    Return res.ResponseData.EliosHistory
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadEmailQueueHistory(PolicyId As Integer,
                                              PolicyImageNum As Integer,
                                              Optional ByRef e As Exception = Nothing) As DCO.InsCollection(Of DCO.Printing.EmailQueue)
            Dim req As New DCSM.PrintingService.LoadEmailQueueHistory.Request
            Dim res As New DCSM.PrintingService.LoadEmailQueueHistory.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Printing.LoadEmailQueueHistory(res, req, e)) Then
                If (res.ResponseData.EmailQueueHistory IsNot Nothing) Then
                    Return res.ResponseData.EmailQueueHistory
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadPolicyPrintDistributionHistory(PolicyId As Integer,
                                                           Optional ByRef e As Exception = Nothing) As DCO.InsCollection(Of DCO.Printing.PolicyPrintDistribution)
            Dim req As New DCSM.PrintingService.LoadPolicyPrintDistributionHistory.Request
            Dim res As New DCSM.PrintingService.LoadPolicyPrintDistributionHistory.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Printing.LoadPolicyPrintDistributionHistory(res, req, e)) Then
                If (res.ResponseData.PrintDistributionHistory IsNot Nothing) Then
                    Return res.ResponseData.PrintDistributionHistory
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadPrintHistory(PolicyId As Integer,
                                         PolicyImageNum As Integer,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Printing.PrintForm)
            Dim req As New DCSM.PrintingService.LoadPrintHistory.Request
            Dim res As New DCSM.PrintingService.LoadPrintHistory.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Printing.LoadPrintHistory(res, req, e, dv)) Then
                If (res.ResponseData.PrintHistory IsNot Nothing) Then
                    Return res.ResponseData.PrintHistory
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessUnprocessedPrinting(PolicyId As Integer,
                                                   PolicyImageNum As Integer,
                                                   ProcessAll As Boolean,
                                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim req As New DCSM.PrintingService.ProcessUnprocessedPrinting.Request
            Dim res As New DCSM.PrintingService.ProcessUnprocessedPrinting.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .ProcessAll = ProcessAll
            End With

            IFMS.Printing.ProcessUnprocessedPrinting(res, req, e)

            Return res.ResponseData.Success
        End Function

        Public Function ReceiveEvent(PrintEvent As DCO.Printing.PrintEvent,
                                     Optional ByRef e As Exception = Nothing) As String
            Dim req As New DCSM.PrintingService.ReceiveEvent.Request
            Dim res As New DCSM.PrintingService.ReceiveEvent.Response

            With req.RequestData
                .PrintEvent = PrintEvent
            End With

            If (IFMS.Printing.ReceiveEvent(res, req, e)) Then
                If (res.ResponseData.printGUID IsNot Nothing) Then
                    Return res.ResponseData.printGUID
                End If
            End If

            Return Nothing
        End Function

        Public Function ReprintJob(PolicyId As Integer,
                                   PolicyImageNum As Integer,
                                   PrintForms As DCO.InsCollection(Of DCO.Printing.PrintForm),
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Byte()
            Dim req As New DCSM.PrintingService.ReprintJob.Request
            Dim res As New DCSM.PrintingService.ReprintJob.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .PrintForms = PrintForms
            End With

            If (IFMS.Printing.ReprintJob(res, req, e, dv)) Then
                If (res.ResponseData.Data IsNot Nothing) Then
                    Return res.ResponseData.Data
                End If
            End If

            Return Nothing
        End Function

        Public Function ReprintPolicyPackage(PolicyId As Integer,
                                             TransDate As Date,
                                             Optional ByRef e As Exception = Nothing) As Byte()
            Dim req As New DCSM.PrintingService.ReprintPolicyPackage.Request
            Dim res As New DCSM.PrintingService.ReprintPolicyPackage.Response

            With req.RequestData
                .PolicyId = PolicyId
                .TransDate = TransDate
            End With

            If (IFMS.Printing.ReprintPolicyPackage(res, req, e)) Then
                If (res.ResponseData.Data IsNot Nothing) Then
                    Return res.ResponseData.Data
                End If
            End If

            Return Nothing
        End Function

        Public Function ResendEmail(PrintForms As DCO.InsCollection(Of DCO.Printing.PrintForm),
                                    Optional ByRef e As Exception = Nothing) As Boolean
            Dim req As New DCSM.PrintingService.ResendEmail.Request
            Dim res As New DCSM.PrintingService.ResendEmail.Response

            With req.RequestData
                .PrintForms = PrintForms
            End With

            Return IFMS.Printing.ResendEmail(res, req, e)

        End Function

        Public Function SaveNoticesAndQuestionnaires(NoticesAndQuestionnaires As DCO.InsCollection(Of DCO.Policy.NoticeAndQuestionnaire),
                                                     PolicyId As Integer,
                                                     PolicyImageNum As Integer,
                                                     Optional ByRef e As Exception = Nothing) As DCO.InsCollection(Of DCO.Policy.NoticeAndQuestionnaire)
            Dim req As New DCSM.PrintingService.SaveNoticesAndQuestionnaires.Request
            Dim res As New DCSM.PrintingService.SaveNoticesAndQuestionnaires.Response

            With req.RequestData
                .NoticesAndQuestionnaires = NoticesAndQuestionnaires
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Printing.SaveNoticesAndQuestionnaires(res, req, e)) Then
                If (res.ResponseData.NoticesAndQuestionnaires IsNot Nothing) Then
                    Return res.ResponseData.NoticesAndQuestionnaires
                End If
            End If

            Return Nothing
        End Function

        'Public Function SavePolicyPrintDistribution(EmailAddress As String,
        '                                            PolicyId As Integer,
        '                                            PrintDistributionTypeId As Integer,
        '                                            Optional ByRef e As Exception = Nothing) As Boolean
        '    Dim req As New DCSM.PrintingService.SavePolicyPrintDistribution.Request
        '    Dim res As New DCSM.PrintingService.SavePolicyPrintDistribution.Response

        '    With req.RequestData
        '        .EmailAddress = EmailAddress
        '        .PolicyId = PolicyId
        '        .PrintDistributionTypeId = PrintDistributionTypeId
        '    End With

        '    If (IFMS.Printing.SavePolicyPrintDistribution(res, req, e)) Then
        '        Return res.ResponseData.Result
        '    End If

        '    Return res.ResponseData.Result
        'End Function

        Public Function SavePrintControl(PolicyId As Integer,
                                         PolicyImageNum As Integer,
                                         PrintingControl As DCO.Policy.PrintingControl,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim req As New DCSM.PrintingService.SavePrintControl.Request
            Dim res As New DCSM.PrintingService.SavePrintControl.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .PrintingControl = PrintingControl
            End With

            If (IFMS.Printing.SavePrintControl(res, req, e)) Then
                Return res.ResponseData.Success
            End If

            Return res.ResponseData.Success
        End Function

        Public Function UpdatePrintExportStatus(CompanySpecificExportId As Integer,
                                                ExportStatusId As Integer,
                                                PrintGUID As Integer,
                                                Optional ByRef e As Exception = Nothing) As String
            Dim req As New DCSM.PrintingService.UpdatePrintExportStatus.Request
            Dim res As New DCSM.PrintingService.UpdatePrintExportStatus.Response

            With req.RequestData
                .CompanySpecificExportId = CompanySpecificExportId
                .ExportStatusId = ExportStatusId
                .PrintGUID = PrintGUID
            End With

            If (IFMS.Printing.UpdatePrintExportStatus(res, req, e)) Then
                If (res.ResponseData.xml IsNot Nothing) Then
                    Return res.ResponseData.xml
                End If
            End If

            Return Nothing
        End Function

        Public Function UpdateViewedStatus(PrintGUID As String,
                                           ViewedAgency As Integer,
                                           ViewedAI As Integer,
                                           ViewedInsured As Integer,
                                           Optional ByRef e As Exception = Nothing) As Boolean
            Dim req As New DCSM.PrintingService.UpdateViewedStatus.Request
            Dim res As New DCSM.PrintingService.UpdateViewedStatus.Response

            With req.RequestData
                .PrintGUID = PrintGUID
                .ViewedAgency = ViewedAgency
                .ViewedAI = ViewedAI
                .ViewedInsured = ViewedInsured
            End With

            If (IFMS.Printing.UpdateViewedStatus(res, req, e)) Then
                Return res.ResponseData.Success
            End If

            Return res.ResponseData.Success
        End Function

    End Module
End Namespace