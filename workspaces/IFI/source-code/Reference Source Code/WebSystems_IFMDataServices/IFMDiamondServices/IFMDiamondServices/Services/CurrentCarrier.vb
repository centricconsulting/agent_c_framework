'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSC = Diamond.Common.Services.Messages.CurrentCarrierService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.CurrentCarrier
    Public Module CurrentCarrier
        Public Function CreateCurrentCarrier(ByRef res As DCSC.CreateCurrentCarrier.Response,
                               ByRef req As DCSC.CreateCurrentCarrier.Request,
                               Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.CurrentCarrierServiceProxy.CurrentCarrierServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateCurrentCarrier
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
