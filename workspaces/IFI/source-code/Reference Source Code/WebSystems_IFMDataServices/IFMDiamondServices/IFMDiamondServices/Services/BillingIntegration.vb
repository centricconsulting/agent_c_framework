'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSB = Diamond.Common.Services.Messages.BillingIntegrationService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.BillingIntegration
    Public Module BillingIntegration
        Public Function GetPolicyCancellationInformation(ByRef res As DCSB.GetPolicyCancellationInformation.Response,
                                                         ByRef req As DCSB.GetPolicyCancellationInformation.Request,
                                                         Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingIntegrationServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.GetPolicyCancellationInformation
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PopulateBillingInformation(ByRef res As DCSB.PopulateBillingInformation.Response,
                                                         ByRef req As DCSB.PopulateBillingInformation.Request,
                                                         Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingIntegrationServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PopulateBillingInformation
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PopulatePolicyInformation(ByRef res As DCSB.PopulatePolicyInformation.Response,
                                                         ByRef req As DCSB.PopulatePolicyInformation.Request,
                                                         Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.BillingIntegrationServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PopulatePolicyInformation
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
