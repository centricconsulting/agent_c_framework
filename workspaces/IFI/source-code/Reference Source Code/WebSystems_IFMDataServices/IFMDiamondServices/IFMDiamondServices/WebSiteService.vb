Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.WebSiteService
    Public Module WebSiteService
        Public Function GetPolicyTabProgress(PolicyNumber As String,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.WebSiteService.GetPolicyTabProgress.ResponseData
            Dim req As New DCSM.WebSiteService.GetPolicyTabProgress.Request
            Dim res As New DCSM.WebSiteService.GetPolicyTabProgress.Response

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If (IFMS.WebSite.GetPolicyTabProgress(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveBrowserInfo(BrowserName As String,
                                        BrowserVersion As String,
                                        ClientPlantform As String,
                                        IPAddress As String,
                                        IsMobileDevice As Boolean,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.WebSiteService.SaveBrowserInfo.Request
            Dim res As New DCSM.WebSiteService.SaveBrowserInfo.Response

            With req.RequestData
                .BrowserName = BrowserName
                .BrowserVersion = BrowserVersion
                .ClientPlatform = ClientPlantform
                .IPAddress = IPAddress
                .IsMobileDevice = IsMobileDevice
            End With

            IFMS.WebSite.SaveBrowserInfo(res, req, e, dv)

            Return Nothing
        End Function

        Public Function UpdatePolicyTabProgress(PolicyNumber As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WebSiteService.UpdatePolicyTabProgress.Request
            Dim res As New DCSM.WebSiteService.UpdatePolicyTabProgress.Response

            With req.RequestData
                .PolicyNumber = PolicyNumber
            End With

            If (IFMS.WebSite.UpdatePolicyTabProgress(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.SuccessFlag
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace