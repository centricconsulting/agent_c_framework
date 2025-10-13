Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCS = Diamond.Common.Services.Messages.WorkflowService
Imports DCSP = Diamond.Common.Services.Proxies.Workflow
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.Workflow
    Public Module Workflow
        Public Function ApproveUnderwritingTask(ByRef res As DSCS.ApproveUnderwritingTask.Response,
                                                ByRef req As DSCS.ApproveUnderwritingTask.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ApproveUnderwritingTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanDeleteTask(ByRef res As DSCS.CanDeleteTask.Response,
                                      ByRef req As DSCS.CanDeleteTask.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CanDeleteTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanUserDeleteTask(ByRef res As DSCS.CanUserDeleteTask.Response,
                                          ByRef req As DSCS.CanUserDeleteTask.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CanUserDeleteTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanViewOthersWorkflow(ByRef res As DSCS.CanViewOthersWorkflow.Response,
                                              ByRef req As DSCS.CanViewOthersWorkflow.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CanViewOthersWorkflow
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanViewWorkflow(ByRef res As DSCS.CanViewWorkflow.Response,
                                        ByRef req As DSCS.CanViewWorkflow.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CanViewWorkflow
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CountPolicyTasks(ByRef res As DSCS.CountPolicyTasks.Response,
                                         ByRef req As DSCS.CountPolicyTasks.Request,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CountPolicyTasks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CreateWorkflowForClaims(ByRef res As DSCS.CreateWorkflowForClaims.Response,
                                                ByRef req As DSCS.CreateWorkflowForClaims.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateWorkflowForClaims
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CreateWorkflowForPolicy(ByRef res As DSCS.CreateWorkflowForPolicy.Response,
                                                ByRef req As DSCS.CreateWorkflowForPolicy.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateWorkflowForPolicy
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteClaimTask(ByRef res As DSCS.DeleteClaimTask.Response,
                                        ByRef req As DSCS.DeleteClaimTask.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteClaimTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteTask(ByRef res As DSCS.DeleteTask.Response,
                                   ByRef req As DSCS.DeleteTask.Request,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteToDoListTask(ByRef res As DSCS.DeleteToDoListTask.Response,
                                           ByRef req As DSCS.DeleteToDoListTask.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteToDoListTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteWorkflowForClaims(ByRef res As DSCS.DeleteWorkflowForClaims.Response,
                                                ByRef req As DSCS.DeleteWorkflowForClaims.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteWorkflowForClaims
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetValidTransferQueues(ByRef res As DSCS.GetValidTransferQueues.Response,
                                               ByRef req As DSCS.GetValidTransferQueues.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetValidTransferQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetValidTransferUsers(ByRef res As DSCS.GetValidTransferUsers.Response,
                                              ByRef req As DSCS.GetValidTransferUsers.Request,
                                              Optional ByRef e As Exception = Nothing,
                                              Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetValidTransferUsers
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetWorkflowId(ByRef res As DSCS.GetWorkflowId.Response,
                                      ByRef req As DSCS.GetWorkflowId.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetWorkflowId
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsTaskDeleted(ByRef res As DSCS.IsTaskDeleted.Response,
                                      ByRef req As DSCS.IsTaskDeleted.Request,
                                      Optional ByRef e As Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.IsTaskDeleted
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadClaimTasks(ByRef res As DSCS.LoadClaimTasks.Response,
                                       ByRef req As DSCS.LoadClaimTasks.Request,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadClaimTasks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadOpenTasksUserSettings(ByRef res As DSCS.LoadOpenTasksUserSettings.Response,
                                                  ByRef req As DSCS.LoadOpenTasksUserSettings.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadOpenTasksUserSettings
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyOrClaimInformation(ByRef res As DSCS.LoadPolicyOrClaimInformation.Response,
                                                     ByRef req As DSCS.LoadPolicyOrClaimInformation.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyOrClaimInformation
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyTasks(ByRef res As DSCS.LoadPolicyTasks.Response,
                                        ByRef req As DSCS.LoadPolicyTasks.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyTasks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadPolicyTasksAtThisLevel(ByRef res As DSCS.LoadPolicyTasksAtThisLevel.Response,
                                                   ByRef req As DSCS.LoadPolicyTasksAtThisLevel.Request,
                                                   Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPolicyTasksAtThisLevel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadTaskDetails(ByRef res As DSCS.LoadTaskDetails.Response,
                                        ByRef req As DSCS.LoadTaskDetails.Request,
                                        Optional ByRef e As Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadTaskDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListAgencyList(ByRef res As DSCS.LoadToDoListAgencyList.Response,
                                               ByRef req As DSCS.LoadToDoListAgencyList.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListAgencyList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListMyTasksList(ByRef res As DSCS.LoadToDoListMyTasksList.Response,
                                                ByRef req As DSCS.LoadToDoListMyTasksList.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListMyTasksList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListPendingCancellations(ByRef res As DSCS.LoadToDoListPendingCancellations.Response,
                                                         ByRef req As DSCS.LoadToDoListPendingCancellations.Request,
                                                         Optional ByRef e As Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListPendingCancellations
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListPendingTransactions(ByRef res As DSCS.LoadToDoListPendingTransactions.Response,
                                                        ByRef req As DSCS.LoadToDoListPendingTransactions.Request,
                                                        Optional ByRef e As Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListPendingTransactions
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListPrintErrors(ByRef res As DSCS.LoadToDoListPrintErrors.Response,
                                                ByRef req As DSCS.LoadToDoListPrintErrors.Request,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListPrintErrors
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListPrintItems(ByRef res As DSCS.LoadToDoListPrintItems.Response,
                                               ByRef req As DSCS.LoadToDoListPrintItems.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListPrintItems
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListQuotes(ByRef res As DSCS.LoadToDoListQuotes.Response,
                                           ByRef req As DSCS.LoadToDoListQuotes.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListQuotes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListRescissionItems(ByRef res As DSCS.LoadToDoListRescissionItems.Response,
                                                    ByRef req As DSCS.LoadToDoListRescissionItems.Request,
                                                    Optional ByRef e As Exception = Nothing,
                                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListRescissionItems
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListTransferredTasks(ByRef res As DSCS.LoadToDoListTransferredTasks.Response,
                                                     ByRef req As DSCS.LoadToDoListTransferredTasks.Request,
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListTransferredTasks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadToDoListUnderwriterReview(ByRef res As DSCS.LoadToDoListUnderwriterReview.Response,
                                                      ByRef req As DSCS.LoadToDoListUnderwriterReview.Request,
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadToDoListUnderwriterReview
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadWorkflowQueues(ByRef res As DSCS.LoadWorkflowQueues.Response,
                                           ByRef req As DSCS.LoadWorkflowQueues.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadWorkflowQueues
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadWorkflowTasks(ByRef res As DSCS.LoadWorkflowTasks.Response,
                                          ByRef req As DSCS.LoadWorkflowTasks.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadWorkflowTasks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function RecurTask(ByRef res As DSCS.RecurTask.Response,
                                  ByRef req As DSCS.RecurTask.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RecurTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveOpenTasksUserSettings(ByRef res As DSCS.SaveOpenTasksUserSettings.Response,
                                                  ByRef req As DSCS.SaveOpenTasksUserSettings.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveOpenTasksUserSettings
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveTask(ByRef res As DSCS.SaveTask.Response,
                                 ByRef req As DSCS.SaveTask.Request,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SendToAgencyQueue(ByRef res As DSCS.SendToAgencyQueue.Response,
                                          ByRef req As DSCS.SendToAgencyQueue.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SendToAgencyQueue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SetUrgent(ByRef res As DSCS.SetUrgent.Response,
                                  ByRef req As DSCS.SetUrgent.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SetUrgent
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TransferPolicyTask(ByRef res As DSCS.TransferPolicyTask.Response,
                                           ByRef req As DSCS.TransferPolicyTask.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TransferPolicyTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TransferTask(ByRef res As DSCS.TransferTask.Response,
                                     ByRef req As DSCS.TransferTask.Request,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TransferTask
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TransferTaskToAgencyQueue(ByRef res As DSCS.TransferTaskToAgencyQueue.Response,
                                                  ByRef req As DSCS.TransferTaskToAgencyQueue.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.WorkflowServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TransferTaskToAgencyQueue
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace