Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports System.Configuration

Namespace IFM.VR.Common.Helpers.CL
    Public Class HotelMotelRemovedRisks
        Private Shared _HotelMotelRemovedRisksSettings As NewFlagItem
        Public Shared ReadOnly Property HotelMotelRemovedRisksSettings() As NewFlagItem
            Get
                If _HotelMotelRemovedRisksSettings Is Nothing Then
                    _HotelMotelRemovedRisksSettings = New NewFlagItem("VR_CL_HotelMotelRemovedRisks_Settings")
                End If
                Return _HotelMotelRemovedRisksSettings
            End Get
        End Property

        Const HotelMotelRemovedRisksWarningMsg As String = ""
        Public Shared Function HotelMotelRemovedRisksEnabled() As Boolean
            Return HotelMotelRemovedRisksSettings.EnabledFlag
        End Function

        Public Shared Function HotelMotelRemovedRisksEffDate() As Date
            Return HotelMotelRemovedRisksSettings.GetStartDateOrDefault("7/1/2024")
        End Function

        Public Shared Sub UpdateHotelMotelRemovedRisks(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD

                Case Helper.EnumsHelper.CrossDirectionEnum.BACK

            End Select
        End Sub

        Public Shared Function IsHotelMotelRemovedRisksAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then

               Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _ 
                OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability _
                OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation _
                OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty _
                OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP
               
                HotelMotelRemovedRisksSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, HotelMotelRemovedRisksSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
       
    End Class

End Namespace
