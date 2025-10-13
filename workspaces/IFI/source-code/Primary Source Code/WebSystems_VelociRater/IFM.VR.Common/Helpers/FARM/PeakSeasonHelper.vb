Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Web
Imports System.Globalization

Namespace IFM.VR.Common.Helpers.FARM
    Public Class PeakSeasonHelper


        Public Shared Function CreatePeakSeasonDataTable() As DataTable
            Dim dtPeakSeason As New DataTable
            dtPeakSeason.Columns.Add("IncreasedLimit", GetType(String))
            dtPeakSeason.Columns.Add("EffectiveDate", GetType(String))
            dtPeakSeason.Columns.Add("ExpirationDate", GetType(String))
            dtPeakSeason.Columns.Add("Description", GetType(String))

            Return dtPeakSeason
        End Function


        Public Shared Function SplitPeak(Quote As QuickQuoteObject, propCoverage As List(Of QuickQuoteScheduledPersonalPropertyCoverage), blnktCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage, peakButtonPress As String, rowNumber As Integer, peakSeasonRow As Integer) As List(Of QuickQuotePeakSeason)
            Return SplitPeak_Full(Quote, propCoverage, blnktCoverage, peakButtonPress, rowNumber, peakSeasonRow, QuickQuoteObject.QuickQuoteTransactionType.None)
        End Function
        Public Shared Function SplitPeak_EndoReady(Quote As QuickQuoteObject, propCoverage As List(Of QuickQuoteScheduledPersonalPropertyCoverage), blnktCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage, peakButtonPress As String, rowNumber As Integer, peakSeasonRow As Integer, TransType As QuickQuoteObject.QuickQuoteTransactionType) As List(Of QuickQuotePeakSeason)
            Return SplitPeak_Full(Quote, propCoverage, blnktCoverage, peakButtonPress, rowNumber, peakSeasonRow, TransType)
        End Function

        Public Shared Function SplitPeak_Full(Quote As QuickQuoteObject, propCoverage As List(Of QuickQuoteScheduledPersonalPropertyCoverage), blnktCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage, peakButtonPress As String, rowNumber As Integer, peakSeasonRow As Integer, TransType As QuickQuoteObject.QuickQuoteTransactionType) As List(Of QuickQuotePeakSeason)
            Dim newPeakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            Dim newPeakSeason As QuickQuotePeakSeason
            Dim effectiveDate As DateTime = Quote.EffectiveDate
            Dim effectiveYear As String = effectiveDate.ToString("yyyy", CultureInfo.InvariantCulture)
            Dim effectiveDateFormatted As String = effectiveDate.ToString("MMdd", CultureInfo.InvariantCulture)
            Dim compareStart As Integer = 0
            Dim startDateInt As Integer = 0
            Dim endDateInt As Integer = 0
            Dim effectiveDateInt As Integer = Integer.Parse(effectiveDateFormatted)
            Dim QQHelper As QuickQuoteHelperClass = New QuickQuoteHelperClass
            Dim originalPeaks As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            'Dim effectiveDateInt As Integer = Integer.Parse(Quote.EffectiveDate.Replace("/", "").Substring(0, Quote.EffectiveDate.Replace("/", "").Length - 4))
            Dim peakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)

            If rowNumber = -1 Then
                peakSeasonList = blnktCoverage.PeakSeasons
            Else
                peakSeasonList = propCoverage(rowNumber).PeakSeasons
            End If
            originalPeaks = QQHelper.CloneObject(peakSeasonList)

            If peakSeasonList IsNot Nothing AndAlso peakSeasonList.Count > 0 Then
                Try
                    For dateIdx As Integer = 0 To peakSeasonList.Count - 1 ' Matt A - 9-22-15 added '-1' to avoid out of index error
                        If peakSeasonList(dateIdx).IncreasedLimit <> "" And peakSeasonList(dateIdx).Description <> "" Then
                            If peakSeasonList(dateIdx).EffectiveDate <> "" And peakSeasonList(dateIdx).ExpirationDate <> "" Then
                                If peakSeasonList(dateIdx).EffectiveDate <> "" Then
                                    Dim startPeak As DateTime = peakSeasonList(dateIdx).EffectiveDate
                                    Dim startYear As String = startPeak.ToString("yyyy", CultureInfo.InvariantCulture)
                                    Dim startPeakFormatted As String = startPeak.ToString("MMdd", CultureInfo.InvariantCulture)
                                    Dim adjStartYear As Integer = 0

                                    If Integer.Parse(effectiveYear) <> Integer.Parse(startYear) Then
                                        adjStartYear = Integer.Parse(effectiveYear) - Integer.Parse(startYear)
                                        startPeak = startPeak.AddYears(adjStartYear)
                                    End If

                                    compareStart = DateTime.Compare(effectiveDate, startPeak)
                                    peakSeasonList(dateIdx).EffectiveDate = startPeak
                                    startDateInt = Integer.Parse(startPeakFormatted)
                                End If

                                If peakSeasonList(dateIdx).ExpirationDate <> "" Then
                                    Dim endPeak As DateTime = peakSeasonList(dateIdx).ExpirationDate
                                    Dim endYear As String = endPeak.ToString("yyyy", CultureInfo.InvariantCulture)
                                    Dim endPeakFormatted As String = endPeak.ToString("MMdd", CultureInfo.InvariantCulture)
                                    Dim adjEndYear As Integer = 0

                                    If Integer.Parse(effectiveYear) <> Integer.Parse(endYear) Then
                                        adjEndYear = Integer.Parse(effectiveYear) - Integer.Parse(endYear)
                                        endPeak = endPeak.AddYears(adjEndYear)
                                    End If

                                    peakSeasonList(dateIdx).ExpirationDate = endPeak
                                    endDateInt = Integer.Parse(endPeakFormatted)
                                End If

                                ' Split when end date is before start date
                                If startDateInt > endDateInt AndAlso startDateInt < effectiveDateInt AndAlso endDateInt < effectiveDateInt Then
                                    newPeakSeason = New QuickQuotePeakSeason
                                    newPeakSeason.IncreasedLimit = peakSeasonList(dateIdx).IncreasedLimit
                                    newPeakSeason.EffectiveDate = Quote.EffectiveDate
                                    Dim newExpireDate As DateTime = DateTime.Parse(peakSeasonList(dateIdx).ExpirationDate).AddYears(1)
                                    newPeakSeason.ExpirationDate = newExpireDate


                                    If peakSeasonList(dateIdx).Description.Contains("-" + (dateIdx + 1).ToString()) OrElse IsEndorsementRelated(TransType) Then
                                        newPeakSeason.Description = peakSeasonList(dateIdx).Description
                                    Else
                                        newPeakSeason.Description = peakSeasonList(dateIdx).Description + "-" + (dateIdx + 1).ToString()
                                    End If

                                    newPeakSeasonList.Add(newPeakSeason)

                                    newPeakSeason = New QuickQuotePeakSeason
                                    If peakSeasonList(dateIdx).EffectiveDate <> "" Then
                                        Dim newStartDate As DateTime = DateTime.Parse(peakSeasonList(dateIdx).EffectiveDate).AddYears(1)
                                        newPeakSeason.EffectiveDate = newStartDate
                                    End If

                                    If Quote.EffectiveDate <> "" Then
                                        'Dim newEndDate As DateTime = DateTime.Parse(Quote.EffectiveDate).AddYears(1).AddDays(-1)
                                        Dim newEndDate As DateTime = DateTime.Parse(Quote.EffectiveDate).AddYears(1)
                                        newPeakSeason.ExpirationDate = newEndDate
                                    End If

                                    newPeakSeason.IncreasedLimit = peakSeasonList(dateIdx).IncreasedLimit

                                    If peakSeasonList(dateIdx).Description.Contains("-" + (dateIdx + 1).ToString()) OrElse IsEndorsementRelated(TransType) Then
                                        newPeakSeason.Description = peakSeasonList(dateIdx).Description
                                    Else
                                        newPeakSeason.Description = peakSeasonList(dateIdx).Description + "-" + (dateIdx + 1).ToString()
                                    End If

                                    newPeakSeasonList.Add(newPeakSeason)
                                Else
                                    ' Date next year does NOT split
                                    'If startDateInt < effectiveDateInt AndAlso endDateInt < effectiveDateInt Then
                                    If startDateInt < effectiveDateInt AndAlso endDateInt <= effectiveDateInt Then
                                        newPeakSeason = New QuickQuotePeakSeason
                                        newPeakSeason.IncreasedLimit = peakSeasonList(dateIdx).IncreasedLimit
                                        newPeakSeason.EffectiveDate = DateTime.Parse(peakSeasonList(dateIdx).EffectiveDate).AddYears(1)
                                        newPeakSeason.ExpirationDate = DateTime.Parse(peakSeasonList(dateIdx).ExpirationDate).AddYears(1)

                                        If peakSeasonList(dateIdx).Description.Contains("-" + (dateIdx + 1).ToString()) OrElse IsEndorsementRelated(TransType) Then
                                            newPeakSeason.Description = peakSeasonList(dateIdx).Description
                                        Else
                                            newPeakSeason.Description = peakSeasonList(dateIdx).Description + "-" + (dateIdx + 1).ToString()
                                        End If

                                        newPeakSeasonList.Add(newPeakSeason)
                                    Else
                                        ' Date does NOT split
                                        If startDateInt >= effectiveDateInt And endDateInt <= 1231 And (startDateInt < endDateInt Or endDateInt <= effectiveDateInt) Then
                                            newPeakSeason = New QuickQuotePeakSeason
                                            newPeakSeason.IncreasedLimit = peakSeasonList(dateIdx).IncreasedLimit

                                            ' Checks to see if the date is before the end of the current year
                                            If startDateInt <= 1231 AndAlso startDateInt >= effectiveDateInt Then
                                                newPeakSeason.EffectiveDate = peakSeasonList(dateIdx).EffectiveDate
                                            Else
                                                newPeakSeason.EffectiveDate = DateTime.Parse(peakSeasonList(dateIdx).EffectiveDate).AddYears(1)
                                            End If

                                            'If endDateInt <= 1231 AndAlso endDateInt >= effectiveDateInt AndAlso effectiveYear >= DateTime.Parse(originalPeaks(dateIdx).ExpirationDate).ToString("yyyy", CultureInfo.InvariantCulture) Then
                                            '    newPeakSeason.ExpirationDate = peakSeasonList(dateIdx).ExpirationDate
                                            'Else
                                            '    newPeakSeason.ExpirationDate = DateTime.Parse(peakSeasonList(dateIdx).ExpirationDate).AddYears(1)
                                            'End If

                                            If startDateInt < endDateInt Then
                                                newPeakSeason.ExpirationDate = DateTime.Parse(peakSeasonList(dateIdx).ExpirationDate)
                                            Else
                                                newPeakSeason.ExpirationDate = DateTime.Parse(peakSeasonList(dateIdx).ExpirationDate).AddYears(1)
                                            End If

                                            If peakSeasonList(dateIdx).Description.Contains("-" + (dateIdx + 1).ToString()) OrElse IsEndorsementRelated(TransType) Then
                                                newPeakSeason.Description = peakSeasonList(dateIdx).Description
                                            Else
                                                newPeakSeason.Description = peakSeasonList(dateIdx).Description + "-" + (dateIdx + 1).ToString()
                                            End If

                                            newPeakSeasonList.Add(newPeakSeason)
                                        Else
                                            'If compareStart > 0 Then
                                            newPeakSeason = New QuickQuotePeakSeason
                                            newPeakSeason.IncreasedLimit = peakSeasonList(dateIdx).IncreasedLimit
                                            newPeakSeason.EffectiveDate = Quote.EffectiveDate
                                            newPeakSeason.ExpirationDate = peakSeasonList(dateIdx).ExpirationDate

                                            If peakSeasonList(dateIdx).Description.Contains("-" + (dateIdx + 1).ToString()) OrElse IsEndorsementRelated(TransType) Then
                                                newPeakSeason.Description = peakSeasonList(dateIdx).Description
                                            Else
                                                newPeakSeason.Description = peakSeasonList(dateIdx).Description + "-" + (dateIdx + 1).ToString()
                                            End If

                                            newPeakSeasonList.Add(newPeakSeason)

                                            newPeakSeason = New QuickQuotePeakSeason
                                            If peakSeasonList(dateIdx).EffectiveDate <> "" Then

                                                If startDateInt < endDateInt Then
                                                    newPeakSeason.EffectiveDate = DateTime.Parse(peakSeasonList(dateIdx).EffectiveDate).AddYears(1)
                                                Else
                                                    newPeakSeason.EffectiveDate = DateTime.Parse(peakSeasonList(dateIdx).EffectiveDate)
                                                End If

                                                'Dim newStartDate As DateTime = DateTime.Parse(peakSeasonList(dateIdx).EffectiveDate).AddYears(1)
                                                'newPeakSeason.EffectiveDate = newStartDate
                                            End If

                                            If peakSeasonList(dateIdx).ExpirationDate <> "" Then
                                                'Dim newEndDate As DateTime = DateTime.Parse(Quote.EffectiveDate).AddYears(1).AddDays(-1)
                                                Dim newEndDate As DateTime = DateTime.Parse(Quote.EffectiveDate).AddYears(1)
                                                newPeakSeason.ExpirationDate = newEndDate
                                            End If

                                            newPeakSeason.IncreasedLimit = peakSeasonList(dateIdx).IncreasedLimit

                                            If peakSeasonList(dateIdx).Description.Contains("-" + (dateIdx + 1).ToString()) OrElse IsEndorsementRelated(TransType) Then
                                                newPeakSeason.Description = peakSeasonList(dateIdx).Description
                                            Else
                                                newPeakSeason.Description = peakSeasonList(dateIdx).Description + "-" + (dateIdx + 1).ToString()
                                            End If

                                            newPeakSeasonList.Add(newPeakSeason)
                                            'Else
                                            '    newPeakSeasonList = peakSeasonList
                                            '    Exit For
                                            'End If
                                        End If
                                    End If
                                End If
                            Else
                                newPeakSeasonList = peakSeasonList
                            End If
                        Else
                            If HttpContext.Current.Session("DeleteBlankPeakButton") <> "DeleteBlankPeak" Then
                                newPeakSeasonList.Add(peakSeasonList(dateIdx))
                            End If
                        End If
                    Next
                Catch ex As Exception
#If DEBUG Then
                    Debugger.Break() ' Matt A - 9-22-15
#End If

                End Try
            End If

            If peakButtonPress IsNot Nothing Then
                If peakButtonPress <> "AddNewRecord!ctlBlnkPersProp!0" Then
                    If peakButtonPress = "AddNewRecord!" + propCoverage(rowNumber).CoverageType.ToString() + "!" + peakSeasonRow.ToString() Then
                        newPeakSeasonList.Add(New QuickQuotePeakSeason)
                    End If
                Else
                    newPeakSeasonList.Add(New QuickQuotePeakSeason)
                End If

                peakButtonPress = Nothing
            End If

            Return newPeakSeasonList
            'Return RemovePeakDups(CreatePeakSeasonDataTable, newPeakSeasonList, effectiveDateInt)
        End Function

        Public Shared Function CombinePeak_EndoReady(propCoverage As List(Of QuickQuoteScheduledPersonalPropertyCoverage), blnktCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage, rowNumber As Integer, TransType As QuickQuoteObject.QuickQuoteTransactionType) As List(Of QuickQuotePeakSeason)
            'Return CombinePeak(CreatePeakSeasonDataTable, propCoverage, blnktCoverage, rowNumber)
            Dim newPeakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            Dim newPeakList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            Dim peakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            Dim dtPeakSeason As DataTable = CreatePeakSeasonDataTable()

            If TransType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse TransType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If rowNumber = -1 Then
                    peakSeasonList = blnktCoverage.PeakSeasons
                Else
                    peakSeasonList = propCoverage(rowNumber).PeakSeasons
                End If

                If peakSeasonList IsNot Nothing Then
                    For Each item As QuickQuotePeakSeason In peakSeasonList
                        dtPeakSeason.Rows.Add(item.IncreasedLimit, item.EffectiveDate, item.ExpirationDate, item.Description)
                    Next
                End If

                dtPeakSeason = dtPeakSeason.DefaultView.ToTable(True, "IncreasedLimit", "EffectiveDate", "ExpirationDate", "Description")

                peakSeasonList = New List(Of QuickQuotePeakSeason)
                For Each peakDate As DataRow In dtPeakSeason.Rows
                    Dim newPeakItem As QuickQuotePeakSeason = New QuickQuotePeakSeason
                    newPeakItem.IncreasedLimit = peakDate("IncreasedLimit")
                    newPeakItem.EffectiveDate = peakDate("EffectiveDate")
                    newPeakItem.ExpirationDate = peakDate("ExpirationDate")
                    newPeakItem.Description = peakDate("Description")
                    newPeakList.Add(newPeakItem)
                Next

                peakSeasonList = newPeakList

                Return peakSeasonList
            Else
                Return CombinePeak(dtPeakSeason, propCoverage, blnktCoverage, rowNumber)
            End If
        End Function

        Public Shared Function CombinePeak(dtPeakSeason As DataTable, propCoverage As List(Of QuickQuoteScheduledPersonalPropertyCoverage), blnktCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage, rowNumber As Integer) As List(Of QuickQuotePeakSeason)
            Dim newPeakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            Dim newPeakList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            'Dim peakSeasonList As List(Of QuickQuotePeakSeason) = propCoverage(rowNumber).PeakSeasons
            Dim peakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)

            If rowNumber = -1 Then
                peakSeasonList = blnktCoverage.PeakSeasons
            Else
                peakSeasonList = propCoverage(rowNumber).PeakSeasons
            End If

            'If peakSeasonList IsNot Nothing Then
            '    For Each item As QuickQuotePeakSeason In peakSeasonList
            '        newPeakSeasonList = peakSeasonList.FindAll(Function(p) p.IncreasedLimit = item.IncreasedLimit And p.Description = item.Description)

            '        If newPeakSeasonList.Count > 1 Then
            '            dtPeakSeason.Rows.Add(newPeakSeasonList(0).IncreasedLimit, newPeakSeasonList(1).EffectiveDate, newPeakSeasonList(0).ExpirationDate, newPeakSeasonList(0).Description)
            '        Else
            '            dtPeakSeason.Rows.Add(newPeakSeasonList(0).IncreasedLimit, newPeakSeasonList(0).EffectiveDate, newPeakSeasonList(0).ExpirationDate, newPeakSeasonList(0).Description)
            '        End If
            '    Next
            'End If

            If peakSeasonList IsNot Nothing Then
                For Each item As QuickQuotePeakSeason In peakSeasonList
                    newPeakSeasonList = peakSeasonList.FindAll(Function(p) p.IncreasedLimit = item.IncreasedLimit And p.Description = item.Description)

                    If newPeakSeasonList.Count > 1 Then
                        dtPeakSeason.Rows.Add(newPeakSeasonList(0).IncreasedLimit, newPeakSeasonList(1).EffectiveDate, newPeakSeasonList(0).ExpirationDate, newPeakSeasonList(0).Description)
                    Else
                        dtPeakSeason.Rows.Add(newPeakSeasonList(0).IncreasedLimit, newPeakSeasonList(0).EffectiveDate, newPeakSeasonList(0).ExpirationDate, newPeakSeasonList(0).Description)
                    End If
                Next
            End If

            dtPeakSeason = dtPeakSeason.DefaultView.ToTable(True, "IncreasedLimit", "EffectiveDate", "ExpirationDate", "Description")

            peakSeasonList = New List(Of QuickQuotePeakSeason)
            For Each peakDate As DataRow In dtPeakSeason.Rows
                Dim newPeakItem As QuickQuotePeakSeason = New QuickQuotePeakSeason
                newPeakItem.IncreasedLimit = peakDate("IncreasedLimit")
                newPeakItem.EffectiveDate = peakDate("EffectiveDate")
                newPeakItem.ExpirationDate = peakDate("ExpirationDate")
                newPeakItem.Description = peakDate("Description")
                newPeakList.Add(newPeakItem)
            Next

            peakSeasonList = newPeakList

            Return peakSeasonList
        End Function


        'Not sure what this code is supposed to do.  It does not Remove duplicates.  It does make the dates incorrect for
        'endorsements since it assumes new business.  I think this may have had a purpose at one point that is no longer
        'needed.  It just takes the list, puts it into a table, and then generates the list again.  It had one reference
        'that I removed from the Split full logic above.
        Public Shared Function RemovePeakDups(dtPeakSeason As DataTable, peakSeasonList As List(Of QuickQuotePeakSeason), effectiveDate As Integer) As List(Of QuickQuotePeakSeason)
            Dim newPeakSeasonList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)
            Dim newPeakList As List(Of QuickQuotePeakSeason) = New List(Of QuickQuotePeakSeason)

            If peakSeasonList IsNot Nothing Then
                For Each item As QuickQuotePeakSeason In peakSeasonList
                    Dim startDateFormated As String = ""
                    Dim endDateFormated As String = ""

                    If item.EffectiveDate <> "" Then
                        Dim startDate As DateTime = item.EffectiveDate
                        startDateFormated = startDate.ToString("MM/dd", CultureInfo.InvariantCulture)
                    Else
                        startDateFormated = ""
                    End If

                    If item.ExpirationDate <> "" Then
                        Dim endDate As DateTime = item.ExpirationDate
                        endDateFormated = endDate.ToString("MM/dd", CultureInfo.InvariantCulture)
                    Else
                        endDateFormated = ""
                    End If

                    dtPeakSeason.Rows.Add(item.IncreasedLimit, startDateFormated, endDateFormated, item.Description)
                Next
            End If

            dtPeakSeason = dtPeakSeason.DefaultView.ToTable(True, "IncreasedLimit", "EffectiveDate", "ExpirationDate", "Description")

            peakSeasonList = New List(Of QuickQuotePeakSeason)
            Dim startDateInt As Integer = 0
            Dim endDateInt As Integer = 0

            For Each peakDate As DataRow In dtPeakSeason.Rows
                If peakDate("EffectiveDate") <> "" Then
                    startDateInt = Integer.Parse(peakDate("EffectiveDate").Replace("/", ""))
                Else
                    startDateInt = 0
                End If

                If peakDate("ExpirationDate") <> "" Then
                    endDateInt = Integer.Parse(peakDate("ExpirationDate").Replace("/", ""))
                Else
                    endDateInt = 0
                End If

                Dim newPeakItem As QuickQuotePeakSeason = New QuickQuotePeakSeason
                newPeakItem.IncreasedLimit = peakDate("IncreasedLimit")
                newPeakItem.EffectiveDate = peakDate("EffectiveDate")
                newPeakItem.ExpirationDate = peakDate("ExpirationDate")
                newPeakItem.Description = peakDate("Description")

                ' Checks to see if the date is before the end of the current year
                If startDateInt <= 1231 AndAlso startDateInt >= effectiveDate Then

                Else
                    If newPeakItem.EffectiveDate <> "" Then
                        newPeakItem.EffectiveDate = DateTime.Parse(newPeakItem.EffectiveDate).AddYears(1)
                    End If
                End If

                If endDateInt <= 1231 AndAlso endDateInt >= effectiveDate Then

                Else
                    If newPeakItem.ExpirationDate <> "" Then
                        newPeakItem.ExpirationDate = DateTime.Parse(newPeakItem.ExpirationDate).AddYears(1)
                    End If
                End If
                newPeakList.Add(newPeakItem)
            Next

            peakSeasonList = newPeakList

            Return peakSeasonList
        End Function

        Private Shared Function IsEndorsementRelated(transType As QuickQuoteObject.QuickQuoteTransactionType) As Boolean
            If transType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse transType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Return True
            End If
            Return False
        End Function
    End Class
End Namespace