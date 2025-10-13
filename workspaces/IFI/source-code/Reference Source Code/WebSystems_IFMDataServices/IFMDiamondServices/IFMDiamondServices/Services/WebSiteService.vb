Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.WebSiteService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.WebSite
    Public Module WebSite
        Public Function GetPolicyTabProgress(ByRef res As DSCM.GetPolicyTabProgress.Response,
                                             ByRef req As DSCM.GetPolicyTabProgress.Request,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebSiteServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyTabProgress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveBrowserInfo(ByRef res As DSCM.SaveBrowserInfo.Response,
                                        ByRef req As DSCM.SaveBrowserInfo.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebSiteServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveBrowserInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function UpdatePolicyTabProgress(ByRef res As DSCM.UpdatePolicyTabProgress.Response,
                                                ByRef req As DSCM.UpdatePolicyTabProgress.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WebSiteServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdatePolicyTabProgress
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace