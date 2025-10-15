Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Report
    Public Module Report
        Public Function LoadReportDetails(ClaimantNum As Integer,
                                          ClaimAppraisalNum As Integer,
                                          ClaimControlId As Integer,
                                          ClaimControlProperyNum As Integer,
                                          ClaimControlVehicleNum As Integer,
                                          ClaimFeaturNum As Integer,
                                          ClaimFormsId As Integer,
                                          ClaimTransactionNum As Integer,
                                          FirstCall As Boolean,
                                          LoadForSave As Boolean,
                                          PolicyId As Integer,
                                          PolicyImageNum As Integer,
                                          ReportId As Integer,
                                          ReportName As String,
                                          ReturnPDF As Boolean,
                                          ReturnReportParts As Boolean,
                                          SystemDate As Date,
                                          UserId As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ReportService.LoadReportDetails.ResponseData
            Dim req As New DCSM.ReportService.LoadReportDetails.Request
            Dim res As New DCSM.ReportService.LoadReportDetails.Response

            With req.RequestData
                .ClaimantNum = ClaimantNum
                .ClaimAppraisalNum = ClaimAppraisalNum
                .ClaimControlId = ClaimControlId
                .ClaimControlPropertyNum = ClaimControlProperyNum
                .ClaimControlVehicleNum = ClaimControlVehicleNum
                .ClaimFeatureNum = ClaimFeaturNum
                .ClaimFormsId = ClaimFormsId
                .ClaimTransactionNum = ClaimTransactionNum
                .FirstCall = FirstCall
                .LoadForSave = LoadForSave
                .PolicyId = PolicyId
                .PolicyImageNum = PolicyImageNum
                .ReportId = ReportId
                .ReportName = ReportName
                .ReturnPDF = ReturnPDF
                .ReturnReportParts = ReturnReportParts
                .SystemDate = SystemDate
                .UsersId = UserId

            End With

            If (IFMS.Report.LoadReportDetails(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadReportList(Lobid As Integer,
                                       ReportLevelId As Integer,
                                       UsersId As Integer,
                                       Optional ByRef e As Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ReportListItem)
            Dim req As New DCSM.ReportService.LoadReportList.Request
            Dim res As New DCSM.ReportService.LoadReportList.Response


            With req.RequestData
                .LobId = Lobid
                .ReportLevelId = ReportLevelId
                .UsersId = UsersId
            End With

            If (IFMS.Report.LoadReportList(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ReportListItems
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace