'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.RuleEngineService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.RuleEngineService
    Public Module RuleEngingProxy
        Public Function EvaluatePolicyRules(ByRef res As DSCM.EvaluatePolicyRules.Response,
                                            ByRef req As DSCM.EvaluatePolicyRules.Request,
                                            Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.RuleEngineServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.EvaluatePolicyRules
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace