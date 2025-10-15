Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.VersionService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.VersionService
    Public Module VersionServiceProxy
        Public Function CheckSystemVersions(ByRef res As DSCM.CheckSystemVersions.Response,
                                            ByRef req As DSCM.CheckSystemVersions.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VersionServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CheckSystemVersions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetAssemblyInfo(ByRef res As DSCM.GetAssemblyInfo.Response,
                                        ByRef req As DSCM.GetAssemblyInfo.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.VersionServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetAssemblyInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace