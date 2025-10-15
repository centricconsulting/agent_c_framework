Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Workflow
    Public Module Workflow
        Public Function ApproveUnderwritingTask(PolicyId As Integer,
                                                PolicyImageNum As Integer,
                                                UserId As Integer,
                                                WorkflowId As Collection,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.WorkflowService.ApproveUnderwritingTask.ResponseData
            Dim req As New DCSM.WorkflowService.ApproveUnderwritingTask.Request
            Dim res As New DCSM.WorkflowService.ApproveUnderwritingTask.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .UserId = UserId
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.ApproveUnderwritingTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function CanDeleteTask(WorkflowId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.CanDeleteTask.Request
            Dim res As New DCSM.WorkflowService.CanDeleteTask.Response

            With req.RequestData
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.CanDeleteTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.AllowDeletion
                End If
            End If

            Return Nothing
        End Function

        Public Function CanUserDeleteTask(WorkflowId As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.CanUserDeleteTask.Request
            Dim res As New DCSM.WorkflowService.CanUserDeleteTask.Response

            With req.RequestData
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.CanUserDeleteTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UserCanDeleteTask
                End If
            End If

            Return Nothing
        End Function

        Public Function CanViewOthersWorkflow(Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.CanViewOthersWorkflow.Request
            Dim res As New DCSM.WorkflowService.CanViewOthersWorkflow.Response

            If (IFMS.Workflow.CanViewOthersWorkflow(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.AuthorityGranted
                End If
            End If

            Return Nothing
        End Function

        Public Function CanViewWorkflow(Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.CanViewWorkflow.Request
            Dim res As New DCSM.WorkflowService.CanViewWorkflow.Response

            If (IFMS.Workflow.CanViewWorkflow(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.AuthorityGranted
                End If
            End If

            Return Nothing
        End Function

        Public Function CountPolicyTasks(PolicyId As Integer,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.WorkflowService.CountPolicyTasks.Request
            Dim res As New DCSM.WorkflowService.CountPolicyTasks.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Workflow.CountPolicyTasks(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.TaskCount
                End If
            End If

            Return Nothing
        End Function

        Public Function CreateWorkflowForClaims(ClaimantNum As Integer,
                                                ClaimControlId As Integer,
                                                ClaimFeatureNum As Integer,
                                                ClaimNumber As String,
                                                ClaimTransactionNum As Integer,
                                                Workflows As DCO.InsCollection(Of DCO.Workflow.Workflow),
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Workflow.Workflow)
            Dim req As New DCSM.WorkflowService.CreateWorkflowForClaims.Request
            Dim res As New DCSM.WorkflowService.CreateWorkflowForClaims.Response

            With req.RequestData
                .ClaimantNum = ClaimantNum
                .ClaimControlId = ClaimControlId
                .ClaimFeatureNum = ClaimFeatureNum
                .ClaimNumber = ClaimNumber
                .ClaimTransactionNum = ClaimTransactionNum
                .Workflows = Workflows
            End With

            If (IFMS.Workflow.CreateWorkflowForClaims(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Workflows
                End If
            End If

            Return Nothing
        End Function

        Public Function CreateWorkflowForPolicy(DiamondValidation As DCO.DiamondValidation,
                                                IsAutomaticTransaction As Boolean,
                                                NoteText As String,
                                                NoteTitle As String,
                                                PolicyId As Integer,
                                                PolicyImageNum As Integer,
                                                Workflow As DCO.Workflow.Workflow,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Workflow.Workflow
            Dim req As New DCSM.WorkflowService.CreateWorkflowForPolicy.Request
            Dim res As New DCSM.WorkflowService.CreateWorkflowForPolicy.Response

            With req.RequestData
                .DiamondValidation = DiamondValidation
                .IsAutomaticTransaction = IsAutomaticTransaction
                .NoteText = NoteText
                .NoteTitle = NoteTitle
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .Workflow = Workflow
            End With

            If (IFMS.Workflow.CreateWorkflowForPolicy(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Workflow
                End If
            End If

            Return Nothing
        End Function

        Public Function DeleteClaimTask(ClaimantNum As Integer,
                                        ClaimControlId As Integer,
                                        ClaimFeature As Integer,
                                        ClaimTransactionNum As Integer,
                                        WorkflowType As DCE.Workflow.WorkflowType,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.DeleteClaimTask.Request
            Dim res As New DCSM.WorkflowService.DeleteClaimTask.Response

            With req.RequestData
                .ClaimantNum = ClaimantNum
                .ClaimControlId = ClaimControlId
                .ClaimFeature = ClaimFeature
                .ClaimTransactionNum = ClaimTransactionNum
                .WorkflowType = WorkflowType
            End With

            If (IFMS.Workflow.DeleteClaimTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function DeleteTask(DenyOnly As Boolean,
                                   OverrideAuthority As Boolean,
                                   RecordDeletionAudit As Boolean,
                                   WorkflowId As Integer,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.DeleteTask.Request
            Dim res As New DCSM.WorkflowService.DeleteTask.Response

            With req.RequestData
                .DenyOnly = DenyOnly
                .OverrideAuthority = OverrideAuthority
                .RecordDeletionAudit = RecordDeletionAudit
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.DeleteTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function DeleteToDoListTask(WorkflowId As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.DeleteToDoListTask.Request
            Dim res As New DCSM.WorkflowService.DeleteToDoListTask.Response

            With req.RequestData
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.DeleteToDoListTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.TaskDeleted
                End If
            End If

            Return Nothing
        End Function

        Public Function DeleteWorkflowForClaims(ClaimantNum As Integer,
                                                ClaimControlId As Integer,
                                                ClaimFeatureNum As Integer,
                                                ClaimTransactionNum As Integer,
                                                WorkflowType As DCE.Workflow.WorkflowType,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.DeleteWorkflowForClaims.Request
            Dim res As New DCSM.WorkflowService.DeleteWorkflowForClaims.Response

            With req.RequestData
                .ClaimantNum = ClaimantNum
                .ClaimControlId = ClaimControlId
                .ClaimFeatureNum = ClaimFeatureNum
                .ClaimTransactionNum = ClaimTransactionNum
            End With

            If (IFMS.Workflow.DeleteWorkflowForClaims(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function GetValidTransferUsers(AgencyId As Integer,
                                               UserId As Integer,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.User)
            Dim req As New DCSM.WorkflowService.GetValidTransferUsers.Request
            Dim res As New DCSM.WorkflowService.GetValidTransferUsers.Response

            With req.RequestData
                .AgencyId = AgencyId
                .UsersId = UserId
            End With

            If (IFMS.Workflow.GetValidTransferUsers(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Users
                End If
            End If

            Return Nothing
        End Function

        Public Function GetWorkflowId(PolicyId As Integer,
                                      PolicyImageNum As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.WorkflowService.GetWorkflowId.ResponseData
            Dim req As New DCSM.WorkflowService.GetWorkflowId.Request
            Dim res As New DCSM.WorkflowService.GetWorkflowId.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Workflow.GetWorkflowId(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function IsTaskDeleted(WorkflowId As Integer,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.IsTaskDeleted.Request
            Dim res As New DCSM.WorkflowService.IsTaskDeleted.Response

            With req.RequestData
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.IsTaskDeleted(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.IsDeleted
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadClaimTasks(ClaimNumber As String,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Workflow.Workflow)
            Dim req As New DCSM.WorkflowService.LoadClaimTasks.Request
            Dim res As New DCSM.WorkflowService.LoadClaimTasks.Response

            With req.RequestData
                .ClaimNumber = ClaimNumber
            End With

            If (IFMS.Workflow.LoadClaimTasks(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ClaimsTasks
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadOpenTasksUserSettings(UserId As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Workflow.OpenTasksScreen.UserSettings
            Dim req As New DCSM.WorkflowService.LoadOpenTasksUserSettings.Request
            Dim res As New DCSM.WorkflowService.LoadOpenTasksUserSettings.Response

            With req.RequestData
                .UserId = UserId
            End With

            If (IFMS.Workflow.LoadOpenTasksUserSettings(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.UserSettings
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadPolicyOrClaimInformation(ClaimControlId As Integer,
                                                     PolicyId As Integer,
                                                     PolicyImageNum As Integer,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.WorkflowService.LoadPolicyOrClaimInformation.ResponseData
            Dim req As New DCSM.WorkflowService.LoadPolicyOrClaimInformation.Request
            Dim res As New DCSM.WorkflowService.LoadPolicyOrClaimInformation.Response

            With req.RequestData
                .ClaimControlId = ClaimControlId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.Workflow.LoadPolicyOrClaimInformation(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadPolicyTasks(PolicyId As Integer,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Workflow.Workflow)
            Dim req As New DCSM.WorkflowService.LoadPolicyTasks.Request
            Dim res As New DCSM.WorkflowService.LoadPolicyTasks.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Workflow.LoadPolicyTasks(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyTasks
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadPolicyTasksAtThisLevel(PolicyId As Integer,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Workflow.Workflow)
            Dim req As New DCSM.WorkflowService.LoadPolicyTasksAtThisLevel.Request
            Dim res As New DCSM.WorkflowService.LoadPolicyTasksAtThisLevel.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Workflow.LoadPolicyTasksAtThisLevel(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyTasks
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadTaskDetails(WorkflowGroupId As DCE.Workflow.WorkflowGroup,
                                        WorkflowId As Integer,
                                        WorkflowTypeId As Integer,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Workflow.Workflow
            Dim req As New DCSM.WorkflowService.LoadTaskDetails.Request
            Dim res As New DCSM.WorkflowService.LoadTaskDetails.Response

            With req.RequestData
                .WorkflowGroupId = WorkflowGroupId
                .WorkflowId = WorkflowId
                .WorkflowTypeId = WorkflowTypeId
            End With

            If (IFMS.Workflow.LoadTaskDetails(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Workflow
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListAgencyList(AgencyId As Integer,
                                               WorkflowQueueTypeId As Integer,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListAgencyList.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListAgencyList.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListAgencyList.Response

            With req.RequestData
                .AgencyId = AgencyId
                .WorkflowQueueTypeId = WorkflowQueueTypeId
            End With

            If (IFMS.Workflow.LoadToDoListAgencyList(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListMyTasksList(Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListMyTasksList.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListMyTasksList.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListMyTasksList.Response

            If (IFMS.Workflow.LoadToDoListMyTasksList(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListPendingCancellations(Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListPendingCancellations.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListPendingCancellations.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListPendingCancellations.Response

            If (IFMS.Workflow.LoadToDoListPendingCancellations(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListPendingTransactions(Optional ByRef e As Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListPendingTransactions.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListPendingTransactions.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListPendingTransactions.Response

            If (IFMS.Workflow.LoadToDoListPendingTransactions(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListPrintErrors(Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListPrintErrors.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListPrintErrors.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListPrintErrors.Response

            If (IFMS.Workflow.LoadToDoListPrintErrors(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListPrintItems(Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListPrintItems.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListPrintItems.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListPrintItems.Response

            If (IFMS.Workflow.LoadToDoListPrintItems(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListQuotes(Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListQuotes.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListQuotes.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListQuotes.Response

            If (IFMS.Workflow.LoadToDoListQuotes(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListRescissionItems(Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListRescissionItems.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListRescissionItems.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListRescissionItems.Response

            If (IFMS.Workflow.LoadToDoListRescissionItems(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListTransferredTasks(Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListTransferredTasks.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListTransferredTasks.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListTransferredTasks.Response

            If (IFMS.Workflow.LoadToDoListTransferredTasks(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadToDoListUnderwriterReview(Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.WorkflowService.LoadToDoListUnderwriterReview.DataItem)
            Dim req As New DCSM.WorkflowService.LoadToDoListUnderwriterReview.Request
            Dim res As New DCSM.WorkflowService.LoadToDoListUnderwriterReview.Response

            If (IFMS.Workflow.LoadToDoListUnderwriterReview(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.DataItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadWorkflowQueues(CheckAuthority As Boolean,
                                           UsersId As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Workflow.WorkflowQueue)
            Dim req As New DCSM.WorkflowService.LoadWorkflowQueues.Request
            Dim res As New DCSM.WorkflowService.LoadWorkflowQueues.Response

            With req.RequestData
                .CheckAuthority = CheckAuthority
                .UsersId = UsersId
            End With

            If (IFMS.Workflow.LoadWorkflowQueues(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WorkflowQueues
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadWorkflowTasks(AgencyUsersOnly As Boolean,
                                          EndDate As Date,
                                          FilterType As DCE.Workflow.FilterType,
                                          FilterValue As String,
                                          OriginatedByUsersId As Integer,
                                          QuotesOnly As Boolean,
                                          StartDate As Date,
                                          TransferredTask As Boolean,
                                          UsersId As Integer,
                                          WorkflowQueueId As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Workflow.Workflow)
            Dim req As New DCSM.WorkflowService.LoadWorkflowTasks.Request
            Dim res As New DCSM.WorkflowService.LoadWorkflowTasks.Response

            With req.RequestData
                .AgencyUsersOnly = AgencyUsersOnly
                .EndDate = EndDate
                .FilterType = FilterType
                .FilterValue = FilterValue
                .OriginatedByUsersId = OriginatedByUsersId
                .QuotesOnly = QuotesOnly
                .StartDate = StartDate
                .TransferredTask = TransferredTask
                .UsersId = UsersId
                .WorkflowQueueId = WorkflowQueueId
            End With

            If (IFMS.Workflow.LoadWorkflowTasks(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WorkflowCollection
                End If
            End If

            Return Nothing
        End Function

        Public Function RecurTask(Workflow As DCO.Workflow.Workflow,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.RecurTask.Request
            Dim res As New DCSM.WorkflowService.RecurTask.Response

            With req.RequestData
                .Workflow = Workflow
            End With

            Return IFMS.Workflow.RecurTask(res, req, e, dv)

        End Function

        Public Function SaveOpenTasksUserSettings(UserSettings As DCO.Workflow.OpenTasksScreen.UserSettings,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.SaveOpenTasksUserSettings.Request
            Dim res As New DCSM.WorkflowService.SaveOpenTasksUserSettings.Response

            With req.RequestData
                .UserSettings = UserSettings
            End With

            If (IFMS.Workflow.SaveOpenTasksUserSettings(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveTask(AlreadyValidated As Boolean,
                                 IsAutomaticTransaction As Boolean,
                                 Workflow As DCO.Workflow.Workflow,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.WorkflowService.SaveTask.Request
            Dim res As New DCSM.WorkflowService.SaveTask.Response

            With req.RequestData
                .AlreadyValidated = AlreadyValidated
                .IsAutomaticTransaction = IsAutomaticTransaction
                .Workflow = Workflow
            End With

            If (IFMS.Workflow.SaveTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WorkflowId
                End If
            End If

            Return Nothing
        End Function

        Public Function SendToAgencyQueue(PolicyId As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.SendToAgencyQueue.Request
            Dim res As New DCSM.WorkflowService.SendToAgencyQueue.Response

            With req.RequestData
                .PolicyId = PolicyId
            End With

            If (IFMS.Workflow.SendToAgencyQueue(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function SetUrgent(WorkflowId As Integer,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.WorkflowService.SetUrgent.Request
            Dim res As New DCSM.WorkflowService.SetUrgent.Response

            With req.RequestData
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.SetUrgent(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Success
                End If
            End If

            Return Nothing
        End Function

        Public Function TransferPolicyTask(Acquire As Boolean,
                                           CurrentUsersId As Integer,
                                           IsAutomaticTransaction As Boolean,
                                           Mandatory As Boolean,
                                           NewUsersId As Integer,
                                           PolicyId As Integer,
                                           PolicyImageNum As Integer,
                                           Remarks As String,
                                           Urgent As Boolean,
                                           WorkflowGroup As DCE.Workflow.WorkflowGroup,
                                           WorkflowId As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.WorkflowService.TransferPolicyTask.Request
            Dim res As New DCSM.WorkflowService.TransferPolicyTask.Response

            With req.RequestData
                .Acquire = Acquire
                .CurrentUsersId = CurrentUsersId
                .IsAutomaticTransaction = IsAutomaticTransaction
                .Mandatory = Mandatory
                .NewUsersId = NewUsersId
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .Remarks = Remarks
                .Urgent = Urgent
                .WorkflowGroup = WorkflowGroup
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.TransferPolicyTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WorkflowId
                End If
            End If

            Return Nothing
        End Function

        Public Function TransferTask(CurrentUsersId As Integer,
                                     WorkflowTransferTypeId As DCE.Workflow.WorkflowTransferType,
                                     Mandatory As Boolean,
                                     NewUsersId As Integer,
                                     Remarks As String,
                                     Urgent As Boolean,
                                     WorkflowId As Integer,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.WorkflowService.TransferTask.Request
            Dim res As New DCSM.WorkflowService.TransferTask.Response

            With req.RequestData
                .CurrentUsersId = CurrentUsersId
                .WorkflowTransferTypeId = WorkflowTransferTypeId
                .Mandatory = Mandatory
                .NewUsersId = NewUsersId
                .Remarks = Remarks
                .Urgent = Urgent
                .WorkflowId = WorkflowId
            End With

            If (IFMS.Workflow.TransferTask(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WorkflowId
                End If
            End If

            Return Nothing
        End Function

        Public Function TransferTaskToAgencyQueue(AgencyId As Integer,
                                                  CurrentUsersId As Integer,
                                                  Mandatory As Boolean,
                                                  NewUsersId As Integer,
                                                  PolciyId As Integer,
                                                  PolicyImageNum As Integer,
                                                  Remarks As String,
                                                  Urgent As Boolean,
                                                  WorkflowId As Integer,
                                                  WorkflowQueueTypeId As DCE.Workflow.WorkflowQueueType,
                                                  Optional IsAutomaticTransaction As Boolean = False,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.WorkflowService.TransferTaskToAgencyQueue.Request
            Dim res As New DCSM.WorkflowService.TransferTaskToAgencyQueue.Response

            With req.RequestData
                .AgencyId = AgencyId
                .CurrentUsersId = CurrentUsersId
                .IsAutomaticTransaction = IsAutomaticTransaction
                .Mandatory = Mandatory
                .NewUsersId = NewUsersId
                .PolicyId = PolciyId
                .PolicyImageNum = PolicyImageNum
                .Remarks = Remarks
                .Urgent = Urgent
                .WorkflowId = WorkflowId
                .WorkflowQueueTypeId = WorkflowQueueTypeId
            End With

            If (IFMS.Workflow.TransferTaskToAgencyQueue(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.WorkflowId
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace
