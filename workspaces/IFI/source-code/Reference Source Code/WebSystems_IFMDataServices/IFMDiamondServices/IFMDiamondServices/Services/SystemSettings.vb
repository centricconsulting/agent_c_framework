'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.SystemSettingsService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.SystemSettings
    Public Module SystemSettingsProxy
        Public Function Delete(ByRef res As DSCM.Delete.Response,
                               ByRef req As DSCM.Delete.Request,
                               Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.SystemSettingsServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Delete
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSystemSettings(ByRef res As DSCM.GetSystemSettings.Response,
                                          ByRef req As DSCM.GetSystemSettings.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.SystemSettingsServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSystemSettings
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Save(ByRef res As DSCM.Save.Response,
                             ByRef req As DSCM.Save.Request,
                             Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.SystemSettingsServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Save
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace