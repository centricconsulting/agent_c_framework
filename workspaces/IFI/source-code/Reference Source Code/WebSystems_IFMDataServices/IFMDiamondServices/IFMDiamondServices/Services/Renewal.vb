'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.RenewalService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Renewal
    Public Module Renewal
        Public Function GetCurrentStatus(ByRef res As DCSM.GetCurrentStatus.Response,
                                         ByRef req As DCSM.GetCurrentStatus.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RenewalServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetCurrentStatus
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsAutomaticRenewalsRunning(ByRef res As DCSM.IsAutomaticRenewalsRunning.Response,
                                         ByRef req As DCSM.IsAutomaticRenewalsRunning.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RenewalServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsAutomaticRenewalsRunning
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessAllRenewals(ByRef res As DCSM.ProcessRenewals.Response,
                                         ByRef req As DCSM.ProcessRenewals.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RenewalServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ProcessRenewals
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessRenewalQueueItem(ByRef res As DCSM.ProcessRenewalQueueItem.Response,
                                         ByRef req As DCSM.ProcessRenewalQueueItem.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RenewalServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessRenewalQueueItem
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ProcessRenewals(ByRef res As DCSM.ProcessRenewals.Response,
                                         ByRef req As DCSM.ProcessRenewals.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RenewalServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessRenewals
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ResetStatus(ByRef res As DCSM.ResetStatus.Response,
                                         ByRef req As DCSM.ResetStatus.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RenewalServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ResetStatus
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
