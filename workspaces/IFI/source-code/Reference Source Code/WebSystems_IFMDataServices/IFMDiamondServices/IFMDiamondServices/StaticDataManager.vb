Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.StaticDataManager
    Public Module StaticDataManager
        Public Function ClearContactManagementData(Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearContactManagementData.Request
            Dim res As New DCSM.StaticDataManager.ClearContactManagementData.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearContactManagementData(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ClearDataFromAllManagers(Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearDataFromAllManagers.Request
            Dim res As New DCSM.StaticDataManager.ClearDataFromAllManagers.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearDataFromAllManagers(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ClearSubmitData(Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearSubmitData.Request
            Dim res As New DCSM.StaticDataManager.ClearSubmitData.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearSubmitData(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ClearSystemData(Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As String()
            Dim req As New DCSM.StaticDataManager.ClearSystemData.Request
            Dim res As New DCSM.StaticDataManager.ClearSystemData.Response

            With req.RequestData
            End With

            If (IFMS.StaticDataManager.ClearSystemData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyNumbers
                End If
            End If

            Return Nothing
        End Function

        Public Function ClearSystemSettingsData(Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearSystemSettingsData.Request
            Dim res As New DCSM.StaticDataManager.ClearSystemSettingsData.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearSystemSettingsData(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ClearUIConfigData(Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearUIConfigData.Request
            Dim res As New DCSM.StaticDataManager.ClearUIConfigData.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearUIConfigData(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ClearUICoverageData(Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearUICoverageData.Request
            Dim res As New DCSM.StaticDataManager.ClearUICoverageData.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearUICoverageData(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ClearVersionData(Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.ClearVersionData.Request
            Dim res As New DCSM.StaticDataManager.ClearVersionData.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.ClearVersionData(res, req, e, dv)

            Return Nothing
        End Function

        Public Function GetContactManagementData(Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.StaticDataManager.ContactManagementData
            Dim req As New DCSM.StaticDataManager.GetContactManagementData.Request
            Dim res As New DCSM.StaticDataManager.GetContactManagementData.Response

            With req.RequestData
            End With

            If (IFMS.StaticDataManager.GetContactManagementData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Data
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSubmitData(LoadAsDataSet As Boolean,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.GetSubmitData.ResponseData
            Dim req As New DCSM.StaticDataManager.GetSubmitData.Request
            Dim res As New DCSM.StaticDataManager.GetSubmitData.Response

            With req.RequestData
                .LoadAsDataSet = LoadAsDataSet
            End With

            If (IFMS.StaticDataManager.GetSubmitData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSystemData(LoadAsDataSet As Boolean,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.GetSystemData.ResponseData
            Dim req As New DCSM.StaticDataManager.GetSystemData.Request
            Dim res As New DCSM.StaticDataManager.GetSystemData.Response

            With req.RequestData
                .LoadAsDataSet = LoadAsDataSet

            End With

            If (IFMS.StaticDataManager.GetSystemData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetSystemSettingsData(LoadAsDataSet As Boolean,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.GetSystemSettingsData.ResponseData
            Dim req As New DCSM.StaticDataManager.GetSystemSettingsData.Request
            Dim res As New DCSM.StaticDataManager.GetSystemSettingsData.Response

            With req.RequestData
                .LoadAsDataSet = LoadAsDataSet
            End With

            If (IFMS.StaticDataManager.GetSystemSettingsData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUIConfigData(LoadAsDataSet As Boolean,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.GetUIConfigData.ResponseData
            Dim req As New DCSM.StaticDataManager.GetUIConfigData.Request
            Dim res As New DCSM.StaticDataManager.GetUIConfigData.Response

            With req.RequestData
                .LoadAsDataSet = LoadAsDataSet
            End With

            If (IFMS.StaticDataManager.GetUIConfigData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetUICoverageData(LoadAsDataSet As Boolean,
                                          VersionId As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.GetUICoverageData.ResponseData
            Dim req As New DCSM.StaticDataManager.GetUICoverageData.Request
            Dim res As New DCSM.StaticDataManager.GetUICoverageData.Response

            With req.RequestData
                .LoadAsDataSet = LoadAsDataSet
                .VersionId = VersionId
            End With

            If (IFMS.StaticDataManager.GetUICoverageData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function GetVersionData(LoadAsDataSet As Boolean,
                                       VersionId As Integer,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.GetVersionData.ResponseData
            Dim req As New DCSM.StaticDataManager.GetVersionData.Request
            Dim res As New DCSM.StaticDataManager.GetVersionData.Response

            With req.RequestData
                .LoadAsDataSet = LoadAsDataSet
                .VersionId = VersionId
            End With

            If (IFMS.StaticDataManager.GetVersionData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadResolverConfig(Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.StaticDataManager.Resolver.Objects.ResolverConfig
            Dim req As New DCSM.StaticDataManager.LoadResolverConfig.Request
            Dim res As New DCSM.StaticDataManager.LoadResolverConfig.Response

            With req.RequestData
            End With

            If (IFMS.StaticDataManager.LoadResolverConfig(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Config
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadSelectedVersionConfig(Config As DCO.StaticDataManager.Config.StaticDataVersionConfig,
                                                  VersionId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.StaticDataManager.LoadSelectedVersionConfig.ResponseData
            Dim req As New DCSM.StaticDataManager.LoadSelectedVersionConfig.Request
            Dim res As New DCSM.StaticDataManager.LoadSelectedVersionConfig.Response

            With req.RequestData
                .Config = Config
                .VersionId = VersionId
            End With

            If (IFMS.StaticDataManager.LoadSelectedVersionConfig(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadVersionConfiguration(Optional ByRef e As Exception = Nothing,
                                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.StaticDataManager.Config.StaticDataVersionConfig)
            Dim req As New DCSM.StaticDataManager.LoadVersionConfiguration.Request
            Dim res As New DCSM.StaticDataManager.LoadVersionConfiguration.Response

            With req.RequestData
            End With

            If (IFMS.StaticDataManager.LoadVersionConfiguration(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Configs
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveSelectedItemsForVersionConfig(SelectedConfig As DCO.StaticDataManager.Config.StaticDataVersionConfig,
                                                          SelectedItems As DCO.InsCollection(Of DCO.StaticDataManager.Config.DataItem),
                                                          Optional ByRef e As Exception = Nothing,
                                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.StaticDataManager.SaveSelectedItemsForVersionConfig.Request
            Dim res As New DCSM.StaticDataManager.SaveSelectedItemsForVersionConfig.Response

            With req.RequestData
                .SelectedConfig = SelectedConfig
                .SelectedItems = SelectedItems
            End With

            If (IFMS.StaticDataManager.SaveSelectedItemsForVersionConfig(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function SupportLoadClaimPersonnel(ClaimPersonnelId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.StaticDataManager.Resolver.Objects.NonStaticSupport.ClaimPersonnel
            Dim req As New DCSM.StaticDataManager.SupportLoadClaimPersonnel.Request
            Dim res As New DCSM.StaticDataManager.SupportLoadClaimPersonnel.Response

            With req.RequestData
                .ClaimPersonnelId = ClaimPersonnelId
            End With

            If (IFMS.StaticDataManager.SupportLoadClaimPersonnel(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ClaimPersonnel
                End If
            End If

            Return Nothing
        End Function

        Public Function ValidateVersionConfiguration(Configs As DCO.InsCollection(Of DCO.StaticDataManager.Config.StaticDataVersionConfig),
                                                     VersionId As Integer,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.StaticDataManager.ValidateVersionConfiguration.Request
            Dim res As New DCSM.StaticDataManager.ValidateVersionConfiguration.Response

            With req.RequestData
                .Configs = Configs
                .VersionId = VersionId
            End With

            If (IFMS.StaticDataManager.ValidateVersionConfiguration(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function WakeUp(Optional ByRef e As Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim req As New DCSM.StaticDataManager.WakeUp.Request
            Dim res As New DCSM.StaticDataManager.WakeUp.Response

            With req.RequestData
            End With

            IFMS.StaticDataManager.WakeUp(res, req, e, dv)

            Return Nothing
        End Function
    End Module
End Namespace