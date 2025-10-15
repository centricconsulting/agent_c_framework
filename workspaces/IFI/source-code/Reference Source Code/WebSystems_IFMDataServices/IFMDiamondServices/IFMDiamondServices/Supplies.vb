Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Supplies
    Public Module Supplies
        Public Function GetSuppliesItemList(Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.SuppliesService.SuppliesItem)
            Dim req As New DCSM.SuppliesService.GetSuppliesItemList.Request
            Dim res As New DCSM.SuppliesService.GetSuppliesItemList.Response

            With req.RequestData
            End With

            If IFMS.SuppliesService.GetSuppliesItemList(res, req, e, dv) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.SuppliesItemList
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace