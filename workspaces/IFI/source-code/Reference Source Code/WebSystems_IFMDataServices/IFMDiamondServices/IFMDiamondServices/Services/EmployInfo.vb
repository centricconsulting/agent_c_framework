'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.EmployInfoService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.EmployInfo
    Public Module EmployInfo
        Public Function LoadAll(ByRef res As DCSM.LoadAll.Response,
                                ByRef req As DCSM.LoadAll.Request,
                                Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.EmployInfoServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAll
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

