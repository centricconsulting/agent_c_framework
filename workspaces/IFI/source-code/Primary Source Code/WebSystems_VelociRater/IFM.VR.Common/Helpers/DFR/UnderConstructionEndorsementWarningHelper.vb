Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.DFR
    Public Class UnderConstructionEndorsementWarningHelper
        Private Shared _UCWarning As NewFlagItem
        Public Shared ReadOnly Property UCWarning() As NewFlagItem
            Get
                If _UCWarning Is Nothing Then
                    _UCWarning = New NewFlagItem("VR_DFR_UnderConstruction_EndorsementWarning")
                End If
                Return _UCWarning
            End Get
        End Property

        Const UCWarningMsg As String = "Not Used."
        Public Shared Sub UpdateUCWarningExclusion(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Added Coverage Possibility
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Coverage Possibility
            End Select
        End Sub

        Public Shared Function isUCWarningAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UCWarning, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
