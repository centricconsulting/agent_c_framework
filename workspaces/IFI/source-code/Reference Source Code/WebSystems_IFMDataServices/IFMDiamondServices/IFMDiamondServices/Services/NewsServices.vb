Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.NewsService
Imports DCSP = Diamond.Common.Services.Proxies.NewsServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.NewsService
    Public Module NewsService
        Public Function GetNewsItem(ByRef res As DSCM.GetNewsItem.Response,
                                          ByRef req As DSCM.GetNewsItem.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NewsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetNewsItem
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNewsItemList(ByRef res As DSCM.GetNewsItemList.Response,
                                                      ByRef req As DSCM.GetNewsItemList.Request,
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NewsServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetNewsItemList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace