Imports Microsoft.VisualBasic
Imports DCSA = Diamond.Common.Services.Messages.AgencyAdministrationService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.AgencyAdministration

    Friend Module AgencyAdministration
        Public Function BeginDownloadAgencyListProcess(ByRef res As DCSA.BeginDownloadAgencyListProcess.Response,
                                                       ByRef req As DCSA.BeginDownloadAgencyListProcess.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.BeginDownloadAgencyListProcess
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function BeginDownloadPolicyListProcess(ByRef res As DCSA.BeginDownloadPolicyListProcess.Response,
                                                       ByRef req As DCSA.BeginDownloadPolicyListProcess.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.BeginDownloadPolicyListProcess
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckPlanExistedByAgencyIDPlanName(ByRef res As DCSA.CheckPlanExistedByAgencyIDPlanName.Response,
                                                       ByRef req As DCSA.CheckPlanExistedByAgencyIDPlanName.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CheckPlanExistedByAgencyIDPlanName
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCoveragePlan(ByRef res As DCSA.DeleteCoveragePlan.Response,
                                                       ByRef req As DCSA.DeleteCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCoveragePlanDetails(ByRef res As DCSA.DeleteCoveragePlanDetails.Response,
                                                       ByRef req As DCSA.DeleteCoveragePlanDetails.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteCoveragePlanDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DownloadOnDemand(ByRef res As DCSA.DownloadOnDemand.Response,
                                                       ByRef req As DCSA.DownloadOnDemand.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DownloadOnDemand
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyCodeByAgencyID(ByRef res As DCSA.GetAgencyCodeByAgencyID.Response,
                                                       ByRef req As DCSA.GetAgencyCodeByAgencyID.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyCodeByAgencyID
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyForDownload(ByRef res As DCSA.GetAgencyForDownload.Response,
                                                       ByRef req As DCSA.GetAgencyForDownload.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyForDownload
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyIDByPolicyIDAndImageNum(ByRef res As DCSA.GetAgencyIDByPolicyIDAndImageNum.Response,
                                                       ByRef req As DCSA.GetAgencyIDByPolicyIDAndImageNum.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyIDByPolicyIDAndImageNum
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAgencyWorkflowInfoByCode(ByRef res As DCSA.GetAgencyWorkflowInfoByCode.Response,
                                                       ByRef req As DCSA.GetAgencyWorkflowInfoByCode.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetAgencyWorkflowInfoByCode
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCoverageCodeLimits(ByRef res As DCSA.GetCoverageCodeLimits.Response,
                                                       ByRef req As DCSA.GetCoverageCodeLimits.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetCoverageCodeLimits
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCoverages(ByRef res As DCSA.GetCoverages.Response,
                                                       ByRef req As DCSA.GetCoverages.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetCoverages
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetLobConnectionTypeIdForVersionId(ByRef res As DCSA.GetLobConnectionTypeIdForVersionId.Response,
                                                       ByRef req As DCSA.GetLobConnectionTypeIdForVersionId.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetLobConnectionTypeIdForVersionId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetOtherDefaults(ByRef res As DCSA.GetOtherDefaults.Response,
                                                       ByRef req As DCSA.GetOtherDefaults.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetOtherDefaults
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetOtherDefaultsOptions(ByRef res As DCSA.GetOtherDefaultsOptions.Response,
                                                       ByRef req As DCSA.GetOtherDefaultsOptions.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetOtherDefaultsOptions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetPolicyFromAgency(ByRef res As DCSA.GetPolicyFromAgency.Response,
                                                       ByRef req As DCSA.GetPolicyFromAgency.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetPolicyFromAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyDefaultData(ByRef res As DCSA.LoadAgencyDefaultData.Response,
                                                       ByRef req As DCSA.LoadAgencyDefaultData.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyDefaultData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAgencyStaticData(ByRef res As DCSA.LoadAgencyStaticData.Response,
                                                       ByRef req As DCSA.LoadAgencyStaticData.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAgencyStaticData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllCoveragePlansForAnAgency(ByRef res As DCSA.LoadAllCoveragePlansForAnAgency.Response,
                                                       ByRef req As DCSA.LoadAllCoveragePlansForAnAgency.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllCoveragePlansForAnAgency
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllCoveragePlansForAnAgencyAndVersion(ByRef res As DCSA.LoadAllCoveragePlansForAnAgencyAndVersion.Response,
                                                       ByRef req As DCSA.LoadAllCoveragePlansForAnAgencyAndVersion.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAllCoveragePlansForAnAgencyAndVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompanyStateLobIDsByVersion(ByRef res As DCSA.LoadCompanyStateLobIDsByVersion.Response,
                                                       ByRef req As DCSA.LoadCompanyStateLobIDsByVersion.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCompanyStateLobIDsByVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCopyPlanAgencyInfo(ByRef res As DCSA.LoadCopyPlanAgencyInfo.Response,
                                                       ByRef req As DCSA.LoadCopyPlanAgencyInfo.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCopyPlanAgencyInfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCoveragePlanDetails(ByRef res As DCSA.LoadCoveragePlanDetails.Response,
                                                       ByRef req As DCSA.LoadCoveragePlanDetails.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCoveragePlanDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCSL(ByRef res As DCSA.LoadCSL.Response,
                                                       ByRef req As DCSA.LoadCSL.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadCSL
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDefaultAutomobileCoveragePlan(ByRef res As DCSA.LoadDefaultAutomobileCoveragePlan.Response,
                                                       ByRef req As DCSA.LoadDefaultAutomobileCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDefaultAutomobileCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDefaultDwellingCoveragePlan(ByRef res As DCSA.LoadDefaultDwellingCoveragePlan.Response,
                                                       ByRef req As DCSA.LoadDefaultDwellingCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDefaultDwellingCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDefaultHomeownerCoveragePlan(ByRef res As DCSA.LoadDefaultHomeownerCoveragePlan.Response,
                                                       ByRef req As DCSA.LoadDefaultHomeownerCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDefaultHomeownerCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadDefaultWatercraftCoveragePlan(ByRef res As DCSA.LoadDefaultWatercraftCoveragePlan.Response,
                                                       ByRef req As DCSA.LoadDefaultWatercraftCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadDefaultWatercraftCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadLatestVersionByCSL(ByRef res As DCSA.LoadLatestVersionByCSL.Response,
                                                       ByRef req As DCSA.LoadLatestVersionByCSL.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadLatestVersionByCSL
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSpecificPlan(ByRef res As DCSA.LoadSpecificPlan.Response,
                                                       ByRef req As DCSA.LoadSpecificPlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadSpecificPlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadVersionsByCSL(ByRef res As DCSA.LoadVersionsByCSL.Response,
                                                       ByRef req As DCSA.LoadVersionsByCSL.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadVersionsByCSL
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAgencyData(ByRef res As DCSA.SaveAgencyData.Response,
                                                       ByRef req As DCSA.SaveAgencyData.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAgencyData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCoveragePlan(ByRef res As DCSA.SaveCoveragePlan.Response,
                                                       ByRef req As DCSA.SaveCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCoveragePlanDetails(ByRef res As DCSA.SaveCoveragePlanDetails.Response,
                                                       ByRef req As DCSA.SaveCoveragePlanDetails.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveCoveragePlanDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSpecificCoveragePlan(ByRef res As DCSA.SaveSpecificCoveragePlan.Response,
                                                       ByRef req As DCSA.SaveSpecificCoveragePlan.Request,
                                                       Optional ByRef e As Exception = Nothing,
                                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AgencyAdministrationServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveSpecificCoveragePlan
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

