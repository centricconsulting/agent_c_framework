Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond

Namespace Services.AgencyAdministration

    Public Module AgencyAdministration
        Public Function GetAgencyWorkflowInfoByCode(agencyCode As String,
                                                    Optional workFlowQueueType As DCE.Workflow.WorkflowQueueType = DCE.Workflow.WorkflowQueueType.General,
                                                    Optional ByRef e As System.Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim res As New DCSM.AgencyAdministrationService.GetAgencyWorkflowInfoByCode.Response
            Dim req As New DCSM.AgencyAdministrationService.GetAgencyWorkflowInfoByCode.Request

            With req.RequestData
                .AgencyCode = agencyCode
                '.Originator = ""
                .WorkflowQueueType = workFlowQueueType
            End With

            If IFMS.AgencyAdministration.GetAgencyWorkflowInfoByCode(res, req, e, dv) Then
                If res.ResponseData IsNot Nothing Then
                    Return res.ResponseData.WorkflowQueueId
                End If
            End If
            Return Nothing
        End Function
    End Module

End Namespace

