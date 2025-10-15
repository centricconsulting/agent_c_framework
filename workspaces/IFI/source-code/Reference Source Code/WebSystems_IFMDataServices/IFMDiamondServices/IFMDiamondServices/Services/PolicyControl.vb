'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.PolicyControlService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.PolicyControl
    'wont use it
    Public Module PolicyControl
        Public Function GetIdFromPolicyNumber(ByRef res As DCSM.GetIdFromPolicyNumber.Response,
                                          ByRef req As DCSM.GetIdFromPolicyNumber.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.PolicyControlServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetIdFromPolicyNumber
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SetFirstWrittenDate(ByRef res As DCSM.SetFirstWrittenDate.Response,
                                          ByRef req As DCSM.SetFirstWrittenDate.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.PolicyControlServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SetFirstWrittenDate
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
