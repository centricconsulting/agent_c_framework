Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class PrivatePowerPolesHelper
        Private Shared _PrivatePowerPolesSettings As NewFlagItem
        Public Shared ReadOnly Property PrivatePowerPolesSettings() As NewFlagItem
            Get
                If _PrivatePowerPolesSettings Is Nothing Then
                    _PrivatePowerPolesSettings = New NewFlagItem("VR_Far_PrivatePowerPoles_Settings")
                End If
                Return _PrivatePowerPolesSettings
            End Get
        End Property

        Const PrivatePowerPolesWarningMsg As String = "N/A"
        Const PrivatePowerPolesRemovedMsg As String = "N/A"
        Public Shared Function PrivatePowerPolesEnabled() As Boolean
            Return PrivatePowerPolesSettings.EnabledFlag
        End Function

        Public Shared Function PrivatePowerPolesEffDate() As Date
            Return PrivatePowerPolesSettings.StartDate
        End Function

        Public Shared Sub UpdatePrivatePowerPolesStove(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Not Needed
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Not Needed
            End Select
        End Sub

        Public Shared Function IsPrivatePowerPolesAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                Dim IsCorrectProgramType As Boolean = SubQuoteFirst.ProgramTypeId <> "8" AndAlso SubQuoteFirst.ProgramTypeId <> "7"
                PrivatePowerPolesSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, PrivatePowerPolesSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

    End Class
End Namespace
