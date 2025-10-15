Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.StaticDataManager
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.StaticDataManager
    Public Module StaticDataManagerProxy
        Public Function ClearContactManagementData(ByRef res As DSCM.ClearContactManagementData.Response,
                                                   ByRef req As DSCM.ClearContactManagementData.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearContactManagementData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearDataFromAllManagers(ByRef res As DSCM.ClearDataFromAllManagers.Response,
                                                 ByRef req As DSCM.ClearDataFromAllManagers.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearDataFromAllManagers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearSubmitData(ByRef res As DSCM.ClearSubmitData.Response,
                                        ByRef req As DSCM.ClearSubmitData.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearSubmitData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearSystemData(ByRef res As DSCM.ClearSystemData.Response,
                                        ByRef req As DSCM.ClearSystemData.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearSystemData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearSystemSettingsData(ByRef res As DSCM.ClearSystemSettingsData.Response,
                                                ByRef req As DSCM.ClearSystemSettingsData.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearSystemSettingsData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearUIConfigData(ByRef res As DSCM.ClearUIConfigData.Response,
                                          ByRef req As DSCM.ClearUIConfigData.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearUIConfigData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearUICoverageData(ByRef res As DSCM.ClearUICoverageData.Response,
                                            ByRef req As DSCM.ClearUICoverageData.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearUICoverageData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ClearVersionData(ByRef res As DSCM.ClearVersionData.Response,
                                         ByRef req As DSCM.ClearVersionData.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearVersionData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetContactManagementData(ByRef res As DSCM.GetContactManagementData.Response,
                                                 ByRef req As DSCM.GetContactManagementData.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetContactManagementData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSubmitData(ByRef res As DSCM.GetSubmitData.Response,
                                      ByRef req As DSCM.GetSubmitData.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSubmitData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSystemData(ByRef res As DSCM.GetSystemData.Response,
                                      ByRef req As DSCM.GetSystemData.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSystemData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetSystemSettingsData(ByRef res As DSCM.GetSystemSettingsData.Response,
                                              ByRef req As DSCM.GetSystemSettingsData.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetSystemSettingsData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUIConfigData(ByRef res As DSCM.GetUIConfigData.Response,
                                        ByRef req As DSCM.GetUIConfigData.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetUIConfigData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetUICoverageData(ByRef res As DSCM.GetUICoverageData.Response,
                                          ByRef req As DSCM.GetUICoverageData.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetUICoverageData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetVersionData(ByRef res As DSCM.GetVersionData.Response,
                                       ByRef req As DSCM.GetVersionData.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetVersionData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadResolverConfig(ByRef res As DSCM.LoadResolverConfig.Response,
                                           ByRef req As DSCM.LoadResolverConfig.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadResolverConfig
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadSelectedVersionConfig(ByRef res As DSCM.LoadSelectedVersionConfig.Response,
                                                  ByRef req As DSCM.LoadSelectedVersionConfig.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadSelectedVersionConfig
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadVersionConfiguration(ByRef res As DSCM.LoadVersionConfiguration.Response,
                                                 ByRef req As DSCM.LoadVersionConfiguration.Request,
                                                 Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadVersionConfiguration
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveSelectedItemsForVersionConfig(ByRef res As DSCM.SaveSelectedItemsForVersionConfig.Response,
                                                          ByRef req As DSCM.SaveSelectedItemsForVersionConfig.Request,
                                                          Optional ByRef e As Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveSelectedItemsForVersionConfig
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SupportLoadClaimPersonnel(ByRef res As DSCM.SupportLoadClaimPersonnel.Response,
                                                  ByRef req As DSCM.SupportLoadClaimPersonnel.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SupportLoadClaimPersonnel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ValidateVersionConfiguration(ByRef res As DSCM.ValidateVersionConfiguration.Response,
                                                     ByRef req As DSCM.ValidateVersionConfiguration.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ValidateVersionConfiguration
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function WakeUp(ByRef res As DSCM.WakeUp.Response,
                               ByRef req As DSCM.WakeUp.Request,
                               Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.StaticDataManagerServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.WakeUp
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace