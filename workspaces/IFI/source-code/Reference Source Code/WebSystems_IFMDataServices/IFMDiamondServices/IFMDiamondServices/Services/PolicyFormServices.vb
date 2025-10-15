Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.PolicyFormService
Imports DCSP = Diamond.Common.Services.Proxies.PolicyFormServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.PolicyFormServices
    Public Module PolicyFormServices
        Public Function AddFormAndPrintByCategory(ByRef res As DSCM.AddFormAndPrintByCategory.Response,
                                                  ByRef req As DSCM.AddFormAndPrintByCategory.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddFormAndPrintByCategory
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function AddFormsAndPrintByFormVersion(ByRef res As DSCM.AddFormsAndPrintByFormVersion.Response,
                                                      ByRef req As DSCM.AddFormsAndPrintByFormVersion.Request,
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddFormsAndPrintByFormVersion
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function AddManualPolicyForm(ByRef res As DSCM.AddManualPolicyForm.Response,
                                            ByRef req As DSCM.AddManualPolicyForm.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AddManualPolicyForm
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteManualPolicyForm(ByRef res As DSCM.DeleteManualPolicyForm.Response,
                                               ByRef req As DSCM.DeleteManualPolicyForm.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteManualPolicyForm
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetFormVersionData(ByRef res As DSCM.GetFormVersionData.Response,
                                           ByRef req As DSCM.GetFormVersionData.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetFormVersionData
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadAdditionalinfo(ByRef res As DSCM.LoadAdditionalinfo.Response,
                                           ByRef req As DSCM.LoadAdditionalinfo.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadAdditionalinfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadForView(ByRef res As DSCM.LoadForView.Response,
                                    ByRef req As DSCM.LoadForView.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadForView
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadRisks(ByRef res As DSCM.LoadRisks.Response,
                                  ByRef req As DSCM.LoadRisks.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRisks
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveAdditionalInfo(ByRef res As DSCM.SaveAdditionalInfo.Response,
                                           ByRef req As DSCM.SaveAdditionalInfo.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveAdditionalinfo
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function TestConfigurableFormsBook(ByRef res As DSCM.TestConfigurableFormsBook.Response,
                                                  ByRef req As DSCM.TestConfigurableFormsBook.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.PolicyFormServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.TestConfigurableFormsBook
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
