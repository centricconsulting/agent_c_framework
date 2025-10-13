Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.PrintingService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.Printing
    Public Module Printing
        Public Function FilterPrintHistory(ByRef res As DCSM.FilterPrintHistory.Response,
                                           ByRef req As DCSM.FilterPrintHistory.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.FilterPrintHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPolicyPrintDistribution(ByRef res As DCSM.GetPolicyPrintDistribution.Response,
                                           ByRef req As DCSM.GetPolicyPrintDistribution.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPolicyPrintDistribution
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPrintReportingData(ByRef res As DCSM.GetPrintReportingData.Response,
                                           ByRef req As DCSM.GetPrintReportingData.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPrintReportingData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEliosHistory(ByRef res As DCSM.LoadEliosHistory.Response,
                                           ByRef req As DCSM.LoadEliosHistory.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEliosHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadEmailQueueHistory(ByRef res As DCSM.LoadEmailQueueHistory.Response,
                                           ByRef req As DCSM.LoadEmailQueueHistory.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadEmailQueueHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPolicyPrintDistributionHistory(ByRef res As DCSM.LoadPolicyPrintDistributionHistory.Response,
                                           ByRef req As DCSM.LoadPolicyPrintDistributionHistory.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadPolicyPrintDistributionHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPrintHistory(ByRef res As DCSM.LoadPrintHistory.Response,
                                           ByRef req As DCSM.LoadPrintHistory.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPrintHistory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessUnprocessedPrinting(ByRef res As DCSM.ProcessUnprocessedPrinting.Response,
                                           ByRef req As DCSM.ProcessUnprocessedPrinting.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessUnprocessedPrinting
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReceiveEvent(ByRef res As DCSM.ReceiveEvent.Response,
                                           ByRef req As DCSM.ReceiveEvent.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReceiveEvent
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReprintJob(ByRef res As DCSM.ReprintJob.Response,
                                           ByRef req As DCSM.ReprintJob.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ReprintJob
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ReprintPolicyPackage(ByRef res As DCSM.ReprintPolicyPackage.Response,
                                           ByRef req As DCSM.ReprintPolicyPackage.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReprintPolicyPackage
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ResendEmail(ByRef res As DCSM.ResendEmail.Response,
                                           ByRef req As DCSM.ResendEmail.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ResendEmail
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveNoticesAndQuestionnaires(ByRef res As DCSM.SaveNoticesAndQuestionnaires.Response,
                                           ByRef req As DCSM.SaveNoticesAndQuestionnaires.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveNoticesAndQuestionnaires
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePolicyPrintDistribution(ByRef res As DCSM.SavePolicyPrintDistribution.Response,
                                           ByRef req As DCSM.SavePolicyPrintDistribution.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePolicyPrintDistribution
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePrintControl(ByRef res As DCSM.SavePrintControl.Response,
                                           ByRef req As DCSM.SavePrintControl.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SavePrintControl
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdatePrintExportStatus(ByRef res As DCSM.UpdatePrintExportStatus.Response,
                                           ByRef req As DCSM.UpdatePrintExportStatus.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdatePrintExportStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateViewedStatus(ByRef res As DCSM.UpdateViewedStatus.Response,
                                           ByRef req As DCSM.UpdateViewedStatus.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PrintingServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.UpdateViewedStatus
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
