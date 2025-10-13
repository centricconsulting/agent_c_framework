Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.DocumentService
Imports DCSP = Diamond.Common.Services.Proxies.DocumentServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.DocumentService
    Public Module DocumentServices
        Public Function GetDocumentCategoryItemList(ByRef res As DSCM.GetDocumentCategoryItemList.Response,
                                                    ByRef req As DSCM.GetDocumentCategoryItemList.Request,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.DocumentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDocumentCategoryItemList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetDocumentCategoryItemListByParentID(ByRef res As DSCM.GetDocumentCategoryItemListByParentID.Response,
                                                              ByRef req As DSCM.GetDocumentCategoryItemListByParentID.Request,
                                                              Optional ByRef e As Exception = Nothing,
                                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.DocumentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDocumentCategoryItemListByParentID
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetDocumentItemByID(ByRef res As DSCM.GetDocumentItemByID.Response,
                                            ByRef req As DSCM.GetDocumentItemByID.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.DocumentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDocumentItemByID
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetDocumentItemListByCategoryID(ByRef res As DSCM.GetDocumentItemListByCategoryID.Response,
                                                        ByRef req As DSCM.GetDocumentItemListByCategoryID.Request,
                                                        Optional ByRef e As Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.DocumentServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetDocumentItemListByCategoryID
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
