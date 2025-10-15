Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.SuppliesService
Imports DCSP = Diamond.Common.Services.Proxies.SuppliesServices
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.SuppliesService
    Public Module SuppliesService
        Public Function GetSuppliesItemList(ByRef res As DSCM.GetSuppliesItemList.Response,
                                     ByRef req As DSCM.GetSuppliesItemList.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.SuppliesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetSuppliesItemList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace