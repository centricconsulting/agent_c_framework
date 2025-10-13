Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.ReplacementCost
    Public Module ReplacementCost
        Public Function LoadReportList(LocationCostEstimate As DCO.ReplacementCost.LocationCostEstimate,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ReplacementCost.LocationCostEstimate
            Dim req As New DCSM.ReplacementCostService.CreateCostEstimate.Request
            Dim res As New DCSM.ReplacementCostService.CreateCostEstimate.Response

            With req.RequestData
                .LocationCostEstimate = LocationCostEstimate
            End With

            If (IFMS.ReplacementCost.CreateCostEstimate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LocationCostEstimate
                End If
            End If

            Return Nothing
        End Function

        Public Function Import(LocationCostEstimate As DCO.ReplacementCost.LocationCostEstimate,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ReplacementCost.LocationCostEstimate
            Dim req As New DCSM.ReplacementCostService.Import.Request
            Dim res As New DCSM.ReplacementCostService.Import.Response

            With req.RequestData
                .LocationCostEstimate = LocationCostEstimate
            End With

            If (IFMS.ReplacementCost.Import(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LocationCostEstimate
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadLocationCostEstimate(LocationNum As Integer,
                                                 PolicyId As Integer,
                                                 PolicyImageNum As Integer,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ReplacementCost.LocationCostEstimate
            Dim req As New DCSM.ReplacementCostService.LoadLocationCostEstimate.Request
            Dim res As New DCSM.ReplacementCostService.LoadLocationCostEstimate.Response

            With req.RequestData
                .LocationNum = LocationNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ReplacementCost.LoadLocationCostEstimate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LocationCostEstimate
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveLocationCostEstimate(LocationCostEstimate As DCO.ReplacementCost.LocationCostEstimate,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ReplacementCost.LocationCostEstimate
            Dim req As New DCSM.ReplacementCostService.SaveLocationCostEstimate.Request
            Dim res As New DCSM.ReplacementCostService.SaveLocationCostEstimate.Response

            With req.RequestData
                .LocationCostEstimate = LocationCostEstimate
            End With

            If (IFMS.ReplacementCost.SaveLocationCostEstimate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LocationCostEstimate
                End If
            End If

            Return Nothing
        End Function

        Public Function UpdatePolicyNumber(Estimates As DCO.InsCollection(Of DCO.ReplacementCost.LocationCostEstimate),
                                           LocationNum As Integer,
                                           PolicyId As Integer,
                                           PolicyImageNum As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ReplacementCost.LocationCostEstimate)
            Dim req As New DCSM.ReplacementCostService.UpdatePolicyNumber.Request
            Dim res As New DCSM.ReplacementCostService.UpdatePolicyNumber.Response

            With req.RequestData
                .Estimates = Estimates
                .LocationNum = LocationNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.ReplacementCost.UpdatePolicyNumber(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Estimates
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace