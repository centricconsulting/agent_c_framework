Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.DocumentServices
    Public Module DocumentServices
        Public Function GetDocumentCategoryItemList(Optional ByRef e As System.Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.DocumentService.DocumentCategoryItem)
            Dim res As New DCSM.DocumentService.GetDocumentCategoryItemList.Response
            Dim req As New DCSM.DocumentService.GetDocumentCategoryItemList.Request

            With req.RequestData
            End With

            If IFMS.DocumentService.GetDocumentCategoryItemList(res, req, e, dv) Then
                Return res.ResponseData.DocumentCategoryItemList
            End If
            Return Nothing
        End Function

        Public Function GetDocumentCategoryItemListByParentID(CompanyId As Integer,
                                                              ParentCategoryId As Integer,
                                                              Optional ByRef e As System.Exception = Nothing,
                                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.DocumentService.DocumentCategoryItem)
            Dim res As New DCSM.DocumentService.GetDocumentCategoryItemListByParentID.Response
            Dim req As New DCSM.DocumentService.GetDocumentCategoryItemListByParentID.Request

            With req.RequestData
                .CompanyId = CompanyId
                .ParentCategoryId = ParentCategoryId
            End With

            If IFMS.DocumentService.GetDocumentCategoryItemListByParentID(res, req, e, dv) Then
                Return res.ResponseData.DocumentCategoryItemList
            End If
            Return Nothing
        End Function

        Public Function GetDocumentItemByID(DocumentId As Integer,
                                            Optional ByRef e As System.Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.DocumentService.DocumentItem
            Dim res As New DCSM.DocumentService.GetDocumentItemByID.Response
            Dim req As New DCSM.DocumentService.GetDocumentItemByID.Request

            With req.RequestData
                .DocumentId = DocumentId
            End With

            If IFMS.DocumentService.GetDocumentItemByID(res, req, e, dv) Then
                Return res.ResponseData.DocumentItem
            End If
            Return Nothing
        End Function

        Public Function GetDocumentItemListByCategoryID(CategoryId As Integer,
                                                        Optional ByRef e As System.Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.DocumentService.DocumentItemList)
            Dim res As New DCSM.DocumentService.GetDocumentItemListByCategoryID.Response
            Dim req As New DCSM.DocumentService.GetDocumentItemListByCategoryID.Request

            With req.RequestData
                .CategoryID = CategoryId
            End With

            If IFMS.DocumentService.GetDocumentItemListByCategoryID(res, req, e, dv) Then
                Return res.ResponseData.DocumentItemList
            End If
            Return Nothing
        End Function
    End Module
End Namespace