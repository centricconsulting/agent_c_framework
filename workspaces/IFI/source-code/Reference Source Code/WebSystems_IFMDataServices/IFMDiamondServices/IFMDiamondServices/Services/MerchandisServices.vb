Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.MerchandiseService
Imports DCSP = Diamond.Common.Services.Proxies.MerchandiseServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.MerchandiseService
    Public Module MerchandiseServices
        Public Function GetMerchandiseCategoryFeaturedList(ByRef res As DSCM.GetMerchandiseCategoryFeaturedList.Response,
                                                           ByRef req As DSCM.GetMerchandiseCategoryFeaturedList.Request,
                                                           Optional ByRef e As Exception = Nothing,
                                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.MerchandiseServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMerchandiseCategoryFeaturedList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetMerchandiseCategoryItemById(ByRef res As DSCM.GetMerchandiseCategoryItemById.Response,
                                                       ByRef req As DSCM.GetMerchandiseCategoryItemById.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.MerchandiseServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMerchandiseCategoryItemById
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetMerchandiseCategoryItemList(ByRef res As DSCM.GetMerchandiseCategoryItemList.Response,
                                                       ByRef req As DSCM.GetMerchandiseCategoryItemList.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.MerchandiseServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMerchandiseCategoryItemList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetMerchandiseProductColorListByProductId(ByRef res As DSCM.GetMerchandiseProductColorListByProductId.Response,
                                                                  ByRef req As DSCM.GetMerchandiseProductColorListByProductId.Request,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.MerchandiseServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMerchandiseProductColorListByProductId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetMerchandiseProductItemListByCategoryId(ByRef res As DSCM.GetMerchandiseProductItemListByCategoryId.Response,
                                                                  ByRef req As DSCM.GetMerchandiseProductItemListByCategoryId.Request,
                                                                  Optional ByRef e As Exception = Nothing,
                                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.MerchandiseServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMerchandiseProductItemListByCategoryId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetMerchandiseProductSizeListByProductId(ByRef res As DSCM.GetMerchandiseProductSizeListByProductId.Response,
                                                                 ByRef req As DSCM.GetMerchandiseProductSizeListByProductId.Request,
                                                                 Optional ByRef e As Exception = Nothing,
                                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.MerchandiseServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetMerchandiseCategoryItemById
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
