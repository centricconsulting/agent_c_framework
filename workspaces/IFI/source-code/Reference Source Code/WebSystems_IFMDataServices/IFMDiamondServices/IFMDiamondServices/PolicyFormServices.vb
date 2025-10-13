Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.PolicyFormServices
    Public Module PolicyFormServices
        Public Function AddFormAndPrintByCategory(FormCategoryTypeId As Integer(),
                                                  PolicyId As Integer,
                                                  PolicyImage As DCO.Policy.Image,
                                                  PolicyImageNum As Integer,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.PolicyFormService.AddFormAndPrintByCategory.Request
            Dim res As New DCSM.PolicyFormService.AddFormAndPrintByCategory.Response

            With req.RequestData
                .FormCategoryTypeId = FormCategoryTypeId
                .PolicyId = PolicyId
                .policyImage = PolicyImage
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.PolicyFormServices.AddFormAndPrintByCategory(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.printGUID
                End If
            End If

            Return Nothing
        End Function

        Public Function AddFormsAndPrintByFormVersion(Level As DCE.Forms.FormLevel,
                                                      PolicyId As Integer,
                                                      PolicyImage As DCO.Policy.Image,
                                                      PolicyImageNum As Integer,
                                                      PrintItems As DCO.InsCollection(Of DCSM.PolicyFormService.AddFormsAndPrintByFormVersion.PrintItems),
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As String
            Dim req As New DCSM.PolicyFormService.AddFormsAndPrintByFormVersion.Request
            Dim res As New DCSM.PolicyFormService.AddFormsAndPrintByFormVersion.Response

            With req.RequestData
                .level = Level
                .PolicyId = PolicyId
                .policyImage = PolicyImage
                .PolicyImageNum = PolicyImageNum
                .PrintItems = PrintItems
            End With

            If (IFMS.PolicyFormServices.AddFormsAndPrintByFormVersion(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.printGUID
                End If
            End If

            Return Nothing
        End Function

        Public Function AddManualPolicyForm(FormVersionId As Integer(),
                                            PolicyFormRisk As DCO.Policy.PolicyForms.PolicyFormRisk,
                                            PolicyId As Integer,
                                            PolicyImageNum As Integer,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim req As New DCSM.PolicyFormService.AddManualPolicyForm.Request
            Dim res As New DCSM.PolicyFormService.AddManualPolicyForm.Response

            With req.RequestData
                .FormVersionIds = FormVersionId
                .PolicyFormRisk = PolicyFormRisk
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.PolicyFormServices.AddManualPolicyForm(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyFormNum
                End If
            End If

            Return Nothing
        End Function

        Public Function DeleteManualPolicyForm(PolicyFormNum As Integer,
                                    PolicyId As Integer,
                                    PolicyImageNum As Integer,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.PolicyFormService.DeleteManualPolicyForm.Request
            Dim res As New DCSM.PolicyFormService.DeleteManualPolicyForm.Response

            With req.RequestData
                .PolicyFormNum = PolicyFormNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.PolicyFormServices.DeleteManualPolicyForm(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Successful
                End If
            End If

            Return Nothing
        End Function

        Public Function GetFormVersionData(CompanyStateLobId As Integer,
                                           EffDate As Date,
                                           PolicyId As Integer,
                                           PolicyImageNumber As Integer,
                                           TransactionTypeId As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DC.StaticDataManager.Objects.SystemData.FormVersion)
            Dim req As New DCSM.PolicyFormService.GetFormVersionData.Request
            Dim res As New DCSM.PolicyFormService.GetFormVersionData.Response

            With req.RequestData
                .CompanyStateLobId = CompanyStateLobId
                .EffDate = EffDate
                .PolicyId = PolicyId
                .PolicyImageNumber = PolicyImageNumber
                .TransactionTypeId = TransactionTypeId
            End With

            If (IFMS.PolicyFormServices.GetFormVersionData(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.FormVersions
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadAdditionalinfo(FormVersionId As Integer,
                                           PolicyFormNum As Integer,
                                           PolicyId As Integer,
                                           PolicyImageNum As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.PolicyForms.PolicyFormAdditionalInfo)
            Dim req As New DCSM.PolicyFormService.LoadAdditionalinfo.Request
            Dim res As New DCSM.PolicyFormService.LoadAdditionalinfo.Response

            With req.RequestData
                .FormVersionId = FormVersionId
                .PolicyFormNum = PolicyFormNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.PolicyFormServices.LoadAdditionalinfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyFormAdditionalInfo
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadForView(EffectiveDate As Date,
                                    LoadClaims As Boolean,
                                    PolicyId As Integer,
                                    PolicyImageNum As Integer,
                                    RenewalVersion As Integer,
                                    TransTypeId As DCE.TransType,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.PolicyForms.PolicyFormView)
            Dim req As New DCSM.PolicyFormService.LoadForView.Request
            Dim res As New DCSM.PolicyFormService.LoadForView.Response

            With req.RequestData
                .EffectiveDate = EffectiveDate
                .LoadClaims = LoadClaims
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .RenewalVersion = RenewalVersion
                .TransTypeId = TransTypeId
            End With

            If (IFMS.PolicyFormServices.LoadForView(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyForms
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadRisks(PolicyId As Integer,
                                  PolicyImageNum As Integer,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.PolicyForms.PolicyFormRisk)
            Dim req As New DCSM.PolicyFormService.LoadRisks.Request
            Dim res As New DCSM.PolicyFormService.LoadRisks.Response

            With req.RequestData
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.PolicyFormServices.LoadRisks(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.PolicyFormRisks
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveAdditionalInfo(PolicyFormAdditionalInfo As DCO.InsCollection(Of DCO.Policy.PolicyForms.PolicyFormAdditionalInfo),
                                           PolicyFormNum As Integer,
                                           PolicyId As Integer,
                                           PolicyImageNum As Integer,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.PolicyFormService.SaveAdditionalInfo.Request
            Dim res As New DCSM.PolicyFormService.SaveAdditionalInfo.Response

            With req.RequestData
                .PolicyFormNum = PolicyFormNum
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
            End With

            If (IFMS.PolicyFormServices.SaveAdditionalInfo(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Successful
                End If
            End If

            Return Nothing
        End Function

        Public Function TestConfigurableFormsBook(BookXml As String,
                                                  FormsData As DCO.Policy.PolicyForms.FormsDataObject,
                                                  Image As DCO.Policy.Image,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.PolicyFormService.TestConfigurableFormsBook.ResponseData
            Dim req As New DCSM.PolicyFormService.TestConfigurableFormsBook.Request
            Dim res As New DCSM.PolicyFormService.TestConfigurableFormsBook.Response

            With req.RequestData
                .BookXml = BookXml
                .FormsData = FormsData
                .Image = Image
            End With

            If (IFMS.PolicyFormServices.TestConfigurableFormsBook(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace