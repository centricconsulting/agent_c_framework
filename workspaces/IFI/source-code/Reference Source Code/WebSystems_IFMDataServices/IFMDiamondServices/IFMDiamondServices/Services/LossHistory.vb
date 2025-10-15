'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.LossHistoryService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.LossHistory
    Public Module LossHistory
        Public Function UpdateLossHistoriesOnRenewal(ByRef res As DCSM.UpdateLossHistoriesOnRenewal.Response,
                                          ByRef req As DCSM.UpdateLossHistoriesOnRenewal.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.LossHistoryServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdateLossHistoriesOnRenewal
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
