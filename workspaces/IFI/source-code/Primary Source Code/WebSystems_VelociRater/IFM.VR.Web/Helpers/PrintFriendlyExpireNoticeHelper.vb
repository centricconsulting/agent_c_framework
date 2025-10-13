Imports PublicQuotingLib.Models
Imports QuickQuote.CommonObjects
Imports System.Globalization
Imports IFM.PrimativeExtensions

Namespace Helpers
    Public Class PrintFriendlyExpireNoticeHelper
        Private Shared ReadOnly Property DaysAgoForForcedQuoteArchivingOrCopy As Integer
            Get
                Dim configDays As Integer = 0
                Dim defaultDays As Integer = 91
                configDays = System.Configuration.ConfigurationManager.AppSettings("VR_NewCo_DaysAgoForForcedQuoteArchivingOrCopy")
                If configDays > 0 Then
                    Return configDays
                Else
                    Return defaultDays
                End If
            End Get
        End Property

        Public Shared Sub SetExpireDate(quote As QuickQuoteObject, container As HtmlGenericControl, statementControl As Label)

            If (quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal _
                OrElse quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal _
                OrElse quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal _
                OrElse quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal _
                OrElse quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm) Then
                Dim OriginDateString = quote.Database_QuickQuote_Inserted
                Dim OriginDate As Date
                Dim ExpireDate As Date
                Dim DayOfWeek As String
                Dim UmbrellaString As String = String.Empty
                If quote.LobType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then
                    UmbrellaString = " Your umbrella quote requires re-rating if any of the underlying policy's coverage or premium has been adjusted."
                End If
                If Date.TryParse(OriginDateString, OriginDate) Then
                    ExpireDate = OriginDate.AddDays(DaysAgoForForcedQuoteArchivingOrCopy - 1)
                    DayOfWeek = ExpireDate.ToString("D", CultureInfo.CreateSpecificCulture("en-US"))
                    Dim ExpireString = "This quotation is valid for 90 days, until " & DayOfWeek _
                                    & "." & UmbrellaString
                    statementControl.Text = ExpireString
                    container.Visible = True
                End If
            End If
        End Sub

    End Class
End Namespace
