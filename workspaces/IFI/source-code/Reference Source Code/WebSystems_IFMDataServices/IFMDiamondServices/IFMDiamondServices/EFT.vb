Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.EFT
    Public Module EFT
        Public Function ImportRecords(EFTGroupID As Integer,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.EFTService.BeginEFTFileProcessing.Response
            Dim req As New DCSM.EFTService.BeginEFTFileProcessing.Request

            With req.RequestData
                .EFTGroupID = EFTGroupID
            End With

            If IFMS.EFT.BeginEFTFileProcessing(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function CreateEFTFile(EFTGroupID As Integer,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.EFTService.CreateEFTFile.Response
            Dim req As New DCSM.EFTService.CreateEFTFile.Request

            With req.RequestData
                .EFTGroupID = EFTGroupID
            End With

            IFMS.EFT.CreateEFTFile(res, req, e, dv)

            Return Nothing
        End Function

        Public Function DeleteEFTGroup(EFTGroupID As Integer,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.EFTService.DeleteEFTGroup.Response
            Dim req As New DCSM.EFTService.DeleteEFTGroup.Request

            With req.RequestData
                .EFTGroupId = EFTGroupID
            End With

            If IFMS.EFT.DeleteEFTGroup(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function LoadAllEFTGroups(Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.EFT.EftGroup)
            Dim res As New DCSM.EFTService.LoadAllEFTGroups.Response
            Dim req As New DCSM.EFTService.LoadAllEFTGroups.Request

            With req.RequestData
            End With

            If IFMS.EFT.LoadAllEFTGroups(res, req, e, dv) Then
                Return res.ResponseData.EFTGroups
            End If
            Return Nothing
        End Function

        Public Function LoadEFTGroup(EFTGroupId As Integer,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.EFT.EftGroup
            Dim res As New DCSM.EFTService.LoadEFTGroup.Response
            Dim req As New DCSM.EFTService.LoadEFTGroup.Request

            With req.RequestData
                .EFTGroupId = EFTGroupId
            End With

            If IFMS.EFT.LoadEFTGroup(res, req, e, dv) Then
                Return res.ResponseData.EFTGroup
            End If
            Return Nothing
        End Function

        Public Function LoadSetupInfo(EFTGroupId As Integer,
                                      EFTGroupTypeId As Byte,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Billing.EftBankAccountView)
            Dim res As New DCSM.EFTService.LoadSetupInfo.Response
            Dim req As New DCSM.EFTService.LoadSetupInfo.Request

            With req.RequestData
                .EFTGroupId = EFTGroupId
                .EFTGroupTypeId = EFTGroupTypeId
            End With

            If IFMS.EFT.LoadSetupInfo(res, req, e, dv) Then
                Return res.ResponseData.EFTBankAccountViews
            End If
            Return Nothing
        End Function

        Public Function ProcessEFTReturnFile(EFTReturnFileContents As String,
                                             EFTReturnFileName As String,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.EFTService.ProcessEFTReturnFile.Response
            Dim req As New DCSM.EFTService.ProcessEFTReturnFile.Request

            With req.RequestData
                .EFTReturnFileContents = EFTReturnFileContents
                .EftReturnFileName = EFTReturnFileName
            End With

            IFMS.EFT.ProcessEFTReturnFile(res, req, E, dv)

            Return Nothing
        End Function

        Public Function SaveEFTGroupInfo(EFTGroup As DCO.EFT.EftGroup,
                                         EFTGroupCompanyLinks As DCO.InsCollection(Of DCO.EFT.EftGroupCompany),
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.EFTService.SaveEFTGroupInfo.Response
            Dim req As New DCSM.EFTService.SaveEFTGroupInfo.Request

            With req.RequestData
                .EFTGroup = EFTGroup
                .EFTGroupCompanyLinks = EFTGroupCompanyLinks
            End With

            If IFMS.EFT.SaveEFTGroupInfo(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function SavePolicyEftInfo(EFTAccountPolicy As DCO.EFT.EftAccountPolicy,
                                          RequireEFT As Boolean,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.EFTService.SavePolicyEftInfo.Response
            Dim req As New DCSM.EFTService.SavePolicyEftInfo.Request

            With req.RequestData
                .EFTAccountPolicy = EFTAccountPolicy
                .RequireEFT = RequireEFT
            End With

            IFMS.EFT.SavePolicyEftInfo(res, req, e, dv)

            Return Nothing
        End Function

        Public Function ValidateRoutingNumberAgainstList(RoutingNumber As String,
                                                         Optional ByRef e As System.Exception = Nothing,
                                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.EFTService.ValidateRoutingNumberAgainstList.Response
            Dim req As New DCSM.EFTService.ValidateRoutingNumberAgainstList.Request

            With req.RequestData
                .RoutingNumber = RoutingNumber
            End With

            If IFMS.EFT.ValidateRoutingNumberAgainstList(res, req, e, dv) Then
                Return res.ResponseData.IsValid
            End If
            Return Nothing
        End Function
    End Module
End Namespace