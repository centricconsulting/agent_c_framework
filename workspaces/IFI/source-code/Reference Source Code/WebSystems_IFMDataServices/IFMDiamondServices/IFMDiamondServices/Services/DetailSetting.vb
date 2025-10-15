'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.DetailSettingService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.DetailSetting
    Public Module DetailSetting
        Public Function GetSettingDescription(ByRef res As DCSM.GetSettingDescription.Response,
                                              ByRef req As DCSM.GetSettingDescription.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSettingDescription
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSettings(ByRef res As DCSM.GetSettings.Response,
                                              ByRef req As DCSM.GetSettings.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSettings
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSettingsByCSL(ByRef res As DCSM.GetSettingsByCSL.Response,
                                              ByRef req As DCSM.GetSettingsByCSL.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSettingsByCSL
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSettingValue(ByRef res As DCSM.GetSettingValue.Response,
                                              ByRef req As DCSM.GetSettingValue.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSettingValue
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetSettingValueByPolicyIDImageAndVersion(ByRef res As DCSM.GetSettingValueByPolicyIDImageAndVersion.Response,
                                              ByRef req As DCSM.GetSettingValueByPolicyIDImageAndVersion.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSettingValueByPolicyIDImageAndVersion
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function IsValidSetting(ByRef res As DCSM.IsValidSetting.Response,
                                              ByRef req As DCSM.IsValidSetting.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsValidSetting
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SettingRetrieveByID(ByRef res As DCSM.SettingRetrieveByID.Response,
                                              ByRef req As DCSM.SettingRetrieveByID.Request,
                                              Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.DetailSettingServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SettingRetrieveByID
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace