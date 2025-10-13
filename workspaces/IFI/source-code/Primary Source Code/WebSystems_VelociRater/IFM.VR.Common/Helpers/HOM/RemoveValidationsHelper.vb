Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls


Namespace IFM.VR.Common.Helpers.HOM

    Public Class RemoveValidationsHelper

        Private Shared _RemoveValidationsSettings As NewFlagItem
        Public Shared ReadOnly Property RemoveValidationsSettings() As NewFlagItem
            Get
                If _RemoveValidationsSettings Is Nothing Then
                    _RemoveValidationsSettings = New NewFlagItem("VR_HOM_RemoveValidations_Settings")
                End If
                Return _RemoveValidationsSettings
            End Get
        End Property

         Public Shared Function RemoveValidationsEnabled() As Boolean
            Return RemoveValidationsSettings.EnabledFlag
        End Function

        Public Shared Function RemoveValidationsEffDate() As Date
            Return RemoveValidationsSettings.GetStartDateOrDefault("9/1/2024")
        End Function

         Public Shared Sub UpdateRemoveValidations(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
            End Select
        End Sub

        Public Shared Function IsRemoveValidationsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RemoveValidationsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
