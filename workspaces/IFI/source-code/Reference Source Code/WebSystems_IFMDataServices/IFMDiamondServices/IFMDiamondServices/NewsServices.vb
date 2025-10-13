Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.NewsServices
    Public Module NewsServices
        Public Function GetNewsItem(ArticalID As Integer,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.NewsItem

            Dim res As New DCSM.NewsService.GetNewsItem.Response
            Dim req As New DCSM.NewsService.GetNewsItem.Request
            With req.RequestData
            .ArticleID = ArticalID
            End With

            If IFMS.NewsService.GetNewsItem(res, req, e, dv) Then
                Return res.ResponseData.NewsItem
            End If
            Return Nothing
        End Function

        Public Function GetNewsItemList(AgencyId As Integer,
                                        ArticleType As Integer,
                                        EffectiveDate As Date,
                                        ExpirateionDate As Date,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.NewsItem)
            Dim res As New DCSM.NewsService.GetNewsItemList.Response
            Dim req As New DCSM.NewsService.GetNewsItemList.Request

            With req.RequestData
                .AgencyId = AgencyId
                .ArticleType = ArticleType
                '.CompanyId = CompanyId
                .EffectivDate = EffectiveDate
                .ExpirationDate = ExpirateionDate
            End With

            If IFMS.NewsService.GetNewsItemList(res, req, e, dv) Then
                Return res.ResponseData.NewsItemList
            End If
            Return Nothing
        End Function
    End Module
End Namespace