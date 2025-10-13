Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Configuration

Namespace IFM.VR.Common.Helpers.AllLines
    Public Class RequiredEmailHelper
        Private Shared _RequiredEmailSettings As NewFlagItem
        Public Shared ReadOnly Property RequiredEmailSettings() As NewFlagItem
            Get
                If _RequiredEmailSettings Is Nothing Then
                    _RequiredEmailSettings = New NewFlagItem("VR_AllLines_EmailRequired_Settings")
                End If
                Return _RequiredEmailSettings
            End Get
        End Property


        Public Shared Function RequiredEmailEnabled() As Boolean
            Return RequiredEmailSettings.EnabledFlag
        End Function

        Public Shared Function RequiredEmailEffDate() As Date
            Return RequiredEmailSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateRequiredEmail(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
            End Select
        End Sub

        Public Shared Function IsRequiredEmailAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then

                Dim IsCorrectLOB As Boolean = RequiredEmailHelper.RequiredEmailEligibleLOBs(quote)

                RequiredEmailSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RequiredEmailSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
        Public Shared Function RequiredEmailEligibleLOBs(quote As QuickQuoteObject) As Boolean
            Dim RequiredEmailEligible As New List(Of String)
            RequiredEmailEligible = System.Configuration.ConfigurationManager.AppSettings("RequiredEmailEligibleLOBTypes").Split(",").ToList
            If RequiredEmailEligible IsNot Nothing AndAlso RequiredEmailEligible.Contains(quote.LobType.ToString().Trim) Then
                Return True
            End If
            Return False
        End Function
    End Class
End Namespace
