Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.VersionService
    Public Module VersionService
        Public Function CheckSystemVersions(BaseUIVersion As System.Version,
                                            CompanyUIVersion As System.Version,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.VersionService.CheckSystemVersions.ResponseData
            Dim req As New DCSM.VersionService.CheckSystemVersions.Request
            Dim res As New DCSM.VersionService.CheckSystemVersions.Response

            With req.RequestData
                .BaseUIVersion = BaseUIVersion
                .CompanyUIVersion = CompanyUIVersion
            End With

            If (IFMS.VersionService.CheckSystemVersions(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetAssemblyInfo(AssemblyName As String,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.AssemblyVersionInfo
            Dim req As New DCSM.VersionService.GetAssemblyInfo.Request
            Dim res As New DCSM.VersionService.GetAssemblyInfo.Response

            With req.RequestData
                .AssemblyName = AssemblyName
            End With

            If (IFMS.VersionService.GetAssemblyInfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.VersionInformation
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace