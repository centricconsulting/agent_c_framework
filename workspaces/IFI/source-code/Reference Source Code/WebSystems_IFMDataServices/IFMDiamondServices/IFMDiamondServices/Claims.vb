Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Claims
    Public Module Claims
        Public Function LoadClaimsListForPolicyNumber(
                                          PolicyNumber As String,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Claims.List.ClaimListData)
            Dim res As New DCSM.ClaimsService.LoadClaimsListForPolicyNumber.Response
            Dim req As New DCSM.ClaimsService.LoadClaimsListForPolicyNumber.Request

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With
            'TODO: Check returns for possible nullreference exceptions. IE: res.ResponseData is Nothing
            If IFMS.Claims.Claims.LoadClaimsListForPolicyNumber(res, req, e, dv) Then
                Return res.ResponseData.ClaimsList
            End If
            Return Nothing
        End Function


        Public Function LoadActivity(
                                         ClaimControlId As Int32,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.ClaimsService.LoadActivity.DataItem)
            Dim res As New DCSM.ClaimsService.LoadActivity.Response
            Dim req As New DCSM.ClaimsService.LoadActivity.Request

            With req.RequestData
                .ClaimControlId = ClaimControlId
            End With
            'TODO: Check returns for possible nullreference exceptions. IE: res.ResponseData is Nothing
            If IFMS.Claims.Claims.LoadActivity(res, req, e, dv) Then
                Return res.ResponseData.DataItems
            End If
            Return Nothing
        End Function

        Public Function LoadAdjuster(
                                        CarrierAdjusterId As Int32,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Claims.Claimant.CarrierAdjuster
            Dim res As New DCSM.ClaimsService.LoadAdjuster.Response
            Dim req As New DCSM.ClaimsService.LoadAdjuster.Request

            With req.RequestData
                .CarrierAdjusterId = CarrierAdjusterId
            End With
            'TODO: Check returns for possible nullreference exceptions. IE: res.ResponseData is Nothing
            If IFMS.Claims.Claims.LoadAdjuster(res, req, e, dv) Then
                Return res.ResponseData.CarrierAdjuster
            End If
            Return Nothing
        End Function

        Public Function LoadClaimControlPersonnel(
                                        ClaimControlId As Int32,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Claims.PersonnelData
            Dim res As New DCSM.ClaimsService.LoadClaimControlPersonnel.Response
            Dim req As New DCSM.ClaimsService.LoadClaimControlPersonnel.Request

            With req.RequestData
                .ClaimControlId = ClaimControlId
            End With
            'TODO: Check returns for possible nullreference exceptions. IE: res.ResponseData is Nothing
            If IFMS.Claims.Claims.LoadClaimControlPersonnel(res, req, e, dv) Then
                Return res.ResponseData.PersonnelData
            End If
            Return Nothing
        End Function

        Public Function LoadFeatureList(
                                        ClaimControlId As Int32,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.ClaimsService.LoadFeatureList.FeatureListItem)
            Dim res As New DCSM.ClaimsService.LoadFeatureList.Response
            Dim req As New DCSM.ClaimsService.LoadFeatureList.Request

            With req.RequestData
                .ClaimControlId = ClaimControlId
            End With
            'TODO: Check returns for possible nullreference exceptions. IE: res.ResponseData is Nothing
            If IFMS.Claims.Claims.LoadFeatureList(res, req, e, dv) Then
                Return res.ResponseData.FeatureCollection
            End If
            Return Nothing
        End Function



    End Module
End Namespace

