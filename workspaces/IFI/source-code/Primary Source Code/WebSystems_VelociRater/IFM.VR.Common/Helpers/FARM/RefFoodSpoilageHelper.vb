Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class RefFoodSpoilageHelper
        Private Shared _RefFoodSpoilageSettings As NewFlagItem
        Public Shared ReadOnly Property RefFoodSpoilageSettings() As NewFlagItem
            Get
                If _RefFoodSpoilageSettings Is Nothing Then
                    _RefFoodSpoilageSettings = New NewFlagItem("VR_Far_RefFoodSpoilage_Settings")
                End If
                Return _RefFoodSpoilageSettings
            End Get
        End Property

        Const RefFoodSpoilageWarningMsg As String = "N/A"
        Const RefFoodSpoilageRemovedMsg As String = "N/A"
        Public Shared Function RefFoodSpoilageEnabled() As Boolean
            Return RefFoodSpoilageSettings.EnabledFlag
        End Function

        Public Shared Function RefFoodSpoilageEffDate() As Date
            Return RefFoodSpoilageSettings.StartDate
        End Function

        Public Shared Sub UpdateRefFoodSpoilageStove(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Not Needed
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Not Needed
            End Select
        End Sub

        Public Shared Function IsRefFoodSpoilageAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                Dim IsCorrectProgramType As Boolean = SubQuoteFirst.ProgramTypeId = "6"

                RefFoodSpoilageSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RefFoodSpoilageSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

    End Class
End Namespace
