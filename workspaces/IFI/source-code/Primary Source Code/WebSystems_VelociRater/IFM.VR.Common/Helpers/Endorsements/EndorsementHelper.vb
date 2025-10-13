Namespace IFM.VR.Common.Helpers.Endorsements
    Public Class EndorsementHelper
        Public Shared Function EndorsementDaysBack() As Integer
            Dim days As Integer = 0

            Dim strDays As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_Endorsement_Past_CreateDateMin")
            If String.IsNullOrWhiteSpace(strDays) = False AndAlso IsNumeric(strDays) = True Then
                days = CInt(strDays)
            Else
                days = -30
            End If

            Return days
        End Function
        Public Shared Function EndorsementDaysForward() As Integer
            Dim days As Integer = 0

            Dim strDays As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_Endorsement_Future_CreateDateMax")
            If String.IsNullOrWhiteSpace(strDays) = False AndAlso IsNumeric(strDays) = True Then
                days = CInt(strDays)
            Else
                days = 25
            End If

            Return days
        End Function

        'Added 2/23/2022 for Task 64829 MLW
        Public Shared Function AdditionalResRentedToOthers_Task64829FeatureFlag() As Boolean
            'Flag to not validate required fields on home endorsement for optional coverage Additional Residence Rented to Others
            If System.Configuration.ConfigurationManager.AppSettings("Task64829_HOMEnd_AdditionalResRentedToOthers_DoNotValidateReqdFields") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings("Task64829_HOMEnd_AdditionalResRentedToOthers_DoNotValidateReqdFields").ToString()) Then
                Dim chc = New CommonHelperClass
                Return chc.ConfigurationAppSettingValueAsBoolean("Task64829_HOMEnd_AdditionalResRentedToOthers_DoNotValidateReqdFields")
                'Return System.Configuration.ConfigurationManager.AppSettings("Task64829_HOMEnd_AdditionalResRentedToOthers_DoNotValidateReqdFields")
            Else
                Return False
            End If
        End Function

        'Added 2/24/2022 for Task 62956 MLW
        Public Shared Function PermittedIncidentalOther_Task62956FeatureFlag() As Boolean
            'Flag to not validate required fields on home endorsement for optional coverage Permitted Incidental Occupancies Other Residence (HO 2443)
            If System.Configuration.ConfigurationManager.AppSettings("Task62956_HOMEnd_PermittedIncidentalOther_DoNotValidateReqdFields") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings("Task62956_HOMEnd_PermittedIncidentalOther_DoNotValidateReqdFields").ToString()) Then
                Dim chc = New CommonHelperClass
                Return chc.ConfigurationAppSettingValueAsBoolean("Task62956_HOMEnd_PermittedIncidentalOther_DoNotValidateReqdFields")
            Else
                Return False
            End If
        End Function

        'Added 2/24/2022 for Task 62956 MLW
        Public Shared Function OtherInsuredLoc_Task62956FeatureFlag() As Boolean
            'Flag to not validate required fields on home endorsement for optional coverage Other Insured Location Occupied by Insured
            If System.Configuration.ConfigurationManager.AppSettings("Task62956_HOMEnd_OtherInsuredLoc_DoNotValidateReqdFields") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings("Task62956_HOMEnd_OtherInsuredLoc_DoNotValidateReqdFields").ToString()) Then
                Dim chc = New CommonHelperClass
                Return chc.ConfigurationAppSettingValueAsBoolean("Task62956_HOMEnd_OtherInsuredLoc_DoNotValidateReqdFields")
            Else
                Return False
            End If
        End Function
    End Class
End Namespace
