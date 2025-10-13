'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.ResidenceInfoService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.ResidenceInfoService
    Public Module ResidenceInfoProxy
        Public Function LoadAll(ByRef res As DSCM.LoadAll.Response,
                                ByRef req As DSCM.LoadAll.Request,
                                Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ResidenceInfoServiceProxy
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