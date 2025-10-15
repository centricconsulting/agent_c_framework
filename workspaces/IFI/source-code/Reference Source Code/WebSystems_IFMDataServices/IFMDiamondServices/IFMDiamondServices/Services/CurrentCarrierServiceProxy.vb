'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.CurrentCarrierService
Imports DCSP = Diamond.Common.Services.Proxies.CurrentCarrierServiceProxy
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.CurrentCarrierService
    Public Module CurrentCarrierServiceProxy
        Public Function CreateCurrentCarrier(ByRef res As DSCM.CreateCurrentCarrier.Response,
                                             ByRef req As DSCM.CreateCurrentCarrier.Request,
                                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.CurrentCarrierServiceProxy
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