Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CL
    Public Class YearBuiltValidationHelper
        Private Shared _UseYearBuiltValidationSettings As NewFlagItem
        Public Shared ReadOnly Property UseYearBuiltValidationSettings() As NewFlagItem
            Get
                If _UseYearBuiltValidationSettings Is Nothing Then
                    _UseYearBuiltValidationSettings = New NewFlagItem("VR_CL_UseYearBuiltValidation_Settings")
                End If
                Return _UseYearBuiltValidationSettings
            End Get
        End Property

        Public Shared Function UseYearBuiltValidationEnabled() As Boolean
            Return UseYearBuiltValidationSettings.EnabledFlag
        End Function

        Public Shared Function UseYearBuiltValidationEffDate() As Date
            Return UseYearBuiltValidationSettings.GetStartDateOrDefault("8/15/2024")
        End Function

        Public Shared Sub UpdateUseYearBuiltValidation(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD

                Case Helper.EnumsHelper.CrossDirectionEnum.BACK

            End Select
        End Sub

        Public Shared Function IsYearBuiltPriorTo1900Available(ByRef Quote As QuickQuoteObject)
            If Quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(Quote, UseYearBuiltValidationSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Public Shared Function ValidYearBuilt(ByVal testYear As String, ByRef quote As QuickQuoteObject) As Boolean
            If Not IsNumeric(testYear) Then Return False
            Dim yr As Integer = CInt(testYear)
            If yr > DateTime.Now.Year Then Return False
            If Not IsYearBuiltPriorTo1900Available(quote) AndAlso yr < 1900 Then Return False
            Return True
        End Function
    End Class
End Namespace
