'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSC = Diamond.Common.Services.Messages.CheckService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Check
    Public Module Check
        Public Function BuildQueue(ByRef res As DCSC.BuildQueue.Response,
                                   ByRef req As DCSC.BuildQueue.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.BuildQueue
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ChangePayee(ByRef res As DCSC.ChangePayee.Response,
                                   ByRef req As DCSC.ChangePayee.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ChangePayee
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CheckJobName(ByRef res As DCSC.CheckJobName.Response,
                                   ByRef req As DCSC.CheckJobName.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CheckJobName
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ClearChecks(ByRef res As DCSC.ClearChecks.Response,
                                   ByRef req As DCSC.ClearChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ClearChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateAgencyChecks(ByRef res As DCSC.CreateAgencyChecks.Response,
                                   ByRef req As DCSC.CreateAgencyChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateAgencyChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateManualMunicipalCheck(ByRef res As DCSC.CreateManualMunicipalCheck.Response,
                                   ByRef req As DCSC.CreateManualMunicipalCheck.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateManualMunicipalCheck
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function CreateMunicipalChecks(ByRef res As DCSC.CreateMunicipalChecks.Response,
                                   ByRef req As DCSC.CreateMunicipalChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateMunicipalChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Delete(ByRef res As DCSC.Delete.Response,
                                   ByRef req As DCSC.Delete.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Delete
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteCheckRule(ByRef res As DCSC.DeleteCheckRule.Response,
                                   ByRef req As DCSC.DeleteCheckRule.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteCheckRule
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteJobs(ByRef res As DCSC.DeleteJobs.Response,
                                   ByRef req As DCSC.DeleteJobs.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteJobs
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteJobTasks(ByRef res As DCSC.DeleteJobTasks.Response,
                                   ByRef req As DCSC.DeleteJobTasks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteJobTasks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteMunicipalCheckPayee(ByRef res As DCSC.DeleteMunicipalCheckPayee.Response,
                                   ByRef req As DCSC.DeleteMunicipalCheckPayee.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteMunicipalCheckPayee
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeletePayee(ByRef res As DCSC.DeletePayee.Response,
                                   ByRef req As DCSC.DeletePayee.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeletePayee
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExportCheckData(ByRef res As DCSC.ExportCheckData.Response,
                                   ByRef req As DCSC.ExportCheckData.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExportCheckData
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExportVoidCheckDataXML(ByRef res As DCSC.ExportVoidCheckDataXML.Response,
                                   ByRef req As DCSC.ExportVoidCheckDataXML.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExportVoidCheckDataXML
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GenerateChecks(ByRef res As DCSC.GenerateChecks.Response,
                                   ByRef req As DCSC.GenerateChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GenerateChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetAllCheckExport(ByRef res As DCSC.GetAllCheckExport.Response,
                                   ByRef req As DCSC.GetAllCheckExport.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetAllCheckExport
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetCheckRemark(ByRef res As DCSC.GetCheckRemark.Response,
                                   ByRef req As DCSC.GetCheckRemark.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetCheckRemark
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetExportFile(ByRef res As DCSC.GetExportFile.Response,
                                   ByRef req As DCSC.GetExportFile.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetExportFile
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetMailToInformation(ByRef res As DCSC.GetMailToInformation.Response,
                                   ByRef req As DCSC.GetMailToInformation.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetMailToInformation
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetMunicipalCheckActivity(ByRef res As DCSC.GetMunicipalCheckActivity.Response,
                                   ByRef req As DCSC.GetMunicipalCheckActivity.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetMunicipalCheckActivity
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetMunicipalCheckPayeeNameAddress(ByRef res As DCSC.GetMunicipalCheckPayeeNameAddress.Response,
                                   ByRef req As DCSC.GetMunicipalCheckPayeeNameAddress.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetMunicipalCheckPayeeNameAddress
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetMunicipalCheckPayees(ByRef res As DCSC.GetMunicipalCheckPayees.Response,
                                   ByRef req As DCSC.GetMunicipalCheckPayees.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetMunicipalCheckPayees
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function GetNextCheckNumber(ByRef res As DCSC.GetNextCheckNumber.Response,
                                   ByRef req As DCSC.GetNextCheckNumber.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetNextCheckNumber
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ImportCheckDataXML(ByRef res As DCSC.ImportCheckDataXML.Response,
                                   ByRef req As DCSC.ImportCheckDataXML.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ImportCheckDataXML
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ImportPositivePayCheckDataXML(ByRef res As DCSC.ImportPositivePayCheckDataXML.Response,
                                   ByRef req As DCSC.ImportPositivePayCheckDataXML.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ImportPositivePayCheckDataXML
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Issue(ByRef res As DCSC.Issue.Response,
                                   ByRef req As DCSC.Issue.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Issue
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAccountsChecks(ByRef res As DCSC.LoadAccountsChecks.Response,
                                   ByRef req As DCSC.LoadAccountsChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAccountsChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAccountsShape(ByRef res As DCSC.LoadAccountsShape.Response,
                                   ByRef req As DCSC.LoadAccountsShape.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAccountsShape
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAllCheckRules(ByRef res As DCSC.LoadAllCheckRules.Response,
                                   ByRef req As DCSC.LoadAllCheckRules.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAllCheckRules
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadChecks(ByRef res As DCSC.LoadChecks.Response,
                                   ByRef req As DCSC.LoadChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadChecksForJob(ByRef res As DCSC.LoadChecksForJob.Response,
                                   ByRef req As DCSC.LoadChecksForJob.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadChecksForJob
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCompany(ByRef res As DCSC.LoadCompany.Response,
                                   ByRef req As DCSC.LoadCompany.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadCompany
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadCSL(ByRef res As DCSC.LoadCSL.Response,
                                   ByRef req As DCSC.LoadCSL.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadCSL
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadJobs(ByRef res As DCSC.LoadJobs.Response,
                                   ByRef req As DCSC.LoadJobs.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadJobs
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadJobTasks(ByRef res As DCSC.LoadJobTasks.Response,
                                   ByRef req As DCSC.LoadJobTasks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadJobTasks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadPayeeInfo(ByRef res As DCSC.LoadPayeeInfo.Response,
                                   ByRef req As DCSC.LoadPayeeInfo.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadPayeeInfo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadProcessingSteps(ByRef res As DCSC.LoadProcessingSteps.Response,
                                   ByRef req As DCSC.LoadProcessingSteps.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadProcessingSteps
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSpecificCheck(ByRef res As DCSC.LoadSpecificCheck.Response,
                                   ByRef req As DCSC.LoadSpecificCheck.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadSpecificCheck
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadSuspenseAccount(ByRef res As DCSC.LoadSuspenseAccount.Response,
                                   ByRef req As DCSC.LoadSuspenseAccount.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadSuspenseAccount
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadVoidCheckItems(ByRef res As DCSC.LoadVoidCheckitems.Response,
                                   ByRef req As DCSC.LoadVoidCheckitems.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadVoidCheckItems
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LookupPayTo(ByRef res As DCSC.LookupPayTo.Response,
                                   ByRef req As DCSC.LookupPayTo.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LookupPayTo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LookupPolicyPayTo(ByRef res As DCSC.LookupPolicyPayTo.Response,
                                   ByRef req As DCSC.LookupPolicyPayTo.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LookupPolicyPayTo
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Modify(ByRef res As DCSC.Modify.Response,
                                   ByRef req As DCSC.Modify.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Modify
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PositivePayExport(ByRef res As DCSC.PositivePayExport.Response,
                                   ByRef req As DCSC.PositivePayExport.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PositivePayExport
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PostChecks(ByRef res As DCSC.PostChecks.Response,
                                   ByRef req As DCSC.PostChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PostChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PrintChecks(ByRef res As DCSC.PrintChecks.Response,
                                   ByRef req As DCSC.PrintChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PrintChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RemoveChecks(ByRef res As DCSC.RemoveChecks.Response,
                                   ByRef req As DCSC.RemoveChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RemoveChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RenumberChecks(ByRef res As DCSC.RenumberChecks.Response,
                                   ByRef req As DCSC.RenumberChecks.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RenumberChecks
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCheckChanges(ByRef res As DCSC.SaveCheckChanges.Response,
                                   ByRef req As DCSC.SaveCheckChanges.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveCheckChanges
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCheckRemark(ByRef res As DCSC.SaveCheckRemark.Response,
                                   ByRef req As DCSC.SaveCheckRemark.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveCheckRemark
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCheckRule(ByRef res As DCSC.SaveCheckRule.Response,
                                   ByRef req As DCSC.SaveCheckRule.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveCheckRule
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveCSL(ByRef res As DCSC.SaveCSL.Response,
                                   ByRef req As DCSC.SaveCSL.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveCSL
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveMailToNameAddress(ByRef res As DCSC.SaveMailToNameAddress.Response,
                                   ByRef req As DCSC.SaveMailToNameAddress.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveMailToNameAddress
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveMunicipalCheckActivity(ByRef res As DCSC.SaveMunicipalCheckActivity.Response,
                                   ByRef req As DCSC.SaveMunicipalCheckActivity.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveMunicipalCheckActivity
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveMunicipalCheckPayeeNameAddress(ByRef res As DCSC.SaveMunicipalCheckPayeeNameAddress.Response,
                                   ByRef req As DCSC.SaveMunicipalCheckPayeeNameAddress.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveMunicipalCheckPayeeNameAddress
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SavePayee(ByRef res As DCSC.SavePayee.Response,
                                   ByRef req As DCSC.SavePayee.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SavePayee
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveProcessingSteps(ByRef res As DCSC.SaveProcessingSteps.Response,
                                   ByRef req As DCSC.SaveProcessingSteps.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveProcessingSteps
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveSA(ByRef res As DCSC.SaveSA.Response,
                                   ByRef req As DCSC.SaveSA.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveSA
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function UpdateCheckStatus(ByRef res As DCSC.UpdateCheckStatus.Response,
                                   ByRef req As DCSC.UpdateCheckStatus.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.UpdateCheckStatus
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Void(ByRef res As DCSC.Void.Response,
                                   ByRef req As DCSC.Void.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Void
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function VoidCheck(ByRef res As DCSC.VoidCheck.Response,
                                   ByRef req As DCSC.VoidCheck.Request,
                                   Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.CheckServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.VoidCheck
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
      End Module
End Namespace
