Imports IFM.VR.Common.QuoteSearch.QuotingStats
Imports System.Linq
Imports System.Xml.Serialization
Imports System.Globalization

Namespace IFM.VR.Common.QuoteSearch
    Public Class QuotingStats

        Public Enum SliceTimeFrame ' keep all exactly 1 apart smallest to largest
            daily = 0
            weekly = 1
            monthly = 2
            yearly = 3
            forever = yearly + 1
        End Enum

        Public Shared Function GetStats(startDate As DateTime, endDate As DateTime, agencyId As Int32, lobList As String, isStaff As Boolean) As TimeSliceDataSet
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Dim slice As New TimeSliceDataSet()
            slice.TimeFrameType = SliceTimeFrame.forever
            slice.StartDate = startDate
            slice.EndDate = endDate

            If String.IsNullOrWhiteSpace(lobList) Then
                Dim lobs As String = ""
                For Each i In IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                    lobs += i.Value.ToString() + ","
                Next
                lobList = lobs.Trim(",")
            End If
            Dim results = IFM.VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(0, "", agencyId, "", "", "", lobList, "", isStaff, True, 0, False, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId")))

            For Each d In results
                If IFM.Common.InputValidation.CommonValidations.IsDateInRange(d.CreatedTimestamp, startDate, endDate) Then
                    Dim r As New QuoteStat()
                    r.LobId = d.LobId
                    r.LobName = d.LobName
                    r.ClientName = d.ClientName
                    r.AgencyCode = d.AgencyCode
                    r.Premium = d.Premium
                    r.LastModifiedDate = CDate(d.LastModified.ToShortDateString())
                    r.Status = d.FriendlyStatus
                    r.StatusId = d.StatusId
                    r.QuoteNumber = d.QuoteNumber
                    r.QuoteId = d.QuoteId
                    r.LastModifiedBy = d.LastModifiedByUsername
                    r.PolicyNumber = d.PolicyNumber
                    r.QuoteCreatedDate = CDate(d.CreatedTimestamp.ToShortDateString())
                    r.QuoteCredtedByUserId = d.CreateByUserid
                    slice.Data.Add(r)
                End If
            Next

            slice.AnalyzeData()

            Return slice
        End Function

        Friend Shared Function GetDataSetsBySlice(data As TimeSliceDataSet, sliceframe As SliceTimeFrame) As List(Of TimeSliceDataSet)

            'Note : This will only find time slices where data exists - you need to go back later and fill in any spaces

            Dim GetStartDateOfWeek As Func(Of DateTime, DateTime) = Function(dt As DateTime)
                                                                        Dim firstDayOfWeek As DateTime = dt
                                                                        While dt.DayOfWeek <> DayOfWeek.Monday
                                                                            dt = dt.AddDays(-1)
                                                                        End While
                                                                        Return dt
                                                                    End Function

            Dim GetEndDateOfWeek As Func(Of DateTime, DateTime) = Function(dt As DateTime)
                                                                      Dim firstDayOfWeek As DateTime = dt
                                                                      While dt.DayOfWeek <> DayOfWeek.Sunday
                                                                          dt = dt.AddDays(1)
                                                                      End While
                                                                      Return dt
                                                                  End Function

            Dim results As List(Of TimeSliceDataSet) = Nothing
            Select Case sliceframe
                Case SliceTimeFrame.daily
                    Dim daysWithdata = From d As QuoteStat In data.Data
                                       Group d By d.QuoteCreatedDate Into Group
                                       Select n = New TimeSliceDataSet With
                                              {
                                                  .Data = Group.ToList(),
                                                  .TimeFrameType = sliceframe,
                                                  .StartDate = If(Group.Any(), Group(0).QuoteCreatedDate, DateTime.MinValue),
                                                  .EndDate = If(Group.Any(), Group(0).QuoteCreatedDate, DateTime.MinValue)
                                              }
                                       Order By n.StartDate Descending
                    results = daysWithdata.ToList()

                Case SliceTimeFrame.weekly
                    Dim monthsWithdata = From d As QuoteStat In data.Data
                                         Let weekNum As Int32 = GetWeekNumber(d.QuoteCreatedDate)
                                         Group d By d.QuoteCreatedDate.Year, weekNum Into Group
                                         Select n = New TimeSliceDataSet With
                                            {
                                                .Data = Group.ToList(),
                                                .TimeFrameType = sliceframe,
                                                .StartDate = If(Group.Any(), GetStartDateOfWeek(Group(0).QuoteCreatedDate), DateTime.MinValue),
                                                .EndDate = If(Group.Any(), GetEndDateOfWeek(Group(0).QuoteCreatedDate), DateTime.MinValue)
                                            }
                                         Order By n.StartDate Descending
                    results = monthsWithdata.ToList()

                Case SliceTimeFrame.monthly
                    Dim monthsWithdata = From d As QuoteStat In data.Data
                                         Group d By d.QuoteCreatedDate.Year, d.QuoteCreatedDate.Month Into Group
                                         Select n = New TimeSliceDataSet With
                                                {
                                                    .Data = Group.ToList(),
                                                    .TimeFrameType = sliceframe,
                                                    .StartDate = If(Group.Any(), DateTime.Parse(String.Format("{0}/1/{1}", Group(0).QuoteCreatedDate.Month, Group(0).QuoteCreatedDate.Year)), DateTime.MinValue),
                                                    .EndDate = If(Group.Any(), .StartDate.AddMonths(1).AddDays(-1), DateTime.MinValue)
                                                }
                                         Order By n.StartDate Descending
                    results = monthsWithdata.ToList()

                Case SliceTimeFrame.yearly
                    Dim monthsWithdata = From d As QuoteStat In data.Data
                                         Group d By d.QuoteCreatedDate.Year Into Group
                                         Select n = New TimeSliceDataSet With
                                                {
                                                    .Data = Group.ToList(),
                                                    .TimeFrameType = sliceframe,
                                                    .StartDate = If(Group.Any(), DateTime.Parse(String.Format("1/1/{0}", Group(0).QuoteCreatedDate.Year)), DateTime.MinValue),
                                                    .EndDate = If(Group.Any(), .StartDate.AddYears(1).AddDays(-1), DateTime.MinValue)
                                                }
                                         Order By n.StartDate Descending
                    results = monthsWithdata.ToList()

            End Select

            'Note : This will only find time slices where data exists - you need to go back and fill in any spaces

            Return results
        End Function

        Public Shared Function GetWeekNumber(dt As DateTime) As Integer
            Return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
        End Function

    End Class

    Public Class TimeSliceDataSet
        Public Property TimeFrameType As SliceTimeFrame
        Public Property StartDate As DateTime
        Public ReadOnly Property StartDate_short As String
            Get
                Return StartDate.ToShortDateString()
            End Get
        End Property
        Public Property EndDate As DateTime
        Public ReadOnly Property EndDate_short As String
            Get
                Return EndDate.ToShortDateString()
            End Get
        End Property

        Public Property Stats As DataSetStats
        Friend Property Data As New List(Of QuoteStat)  'don't want this serialized to json
        Public Property SubSlices As New List(Of TimeSliceDataSet)

        Public Sub AnalyzeData(Optional buildSubSlices As Boolean = True)
            Me.Stats = New DataSetStats()
            BuildStats(Me.Stats, Me.Data, buildSubSlices)
            If buildSubSlices Then
                Me.BuildSubSlices()
            End If
        End Sub

        Private Sub BuildSubSlices()
            If Me.TimeFrameType <> SliceTimeFrame.daily Then
                SubSlices = QuotingStats.GetDataSetsBySlice(Me, Me.TimeFrameType - 1)
            End If
            For Each s In SubSlices
                s.AnalyzeData()
            Next

        End Sub

        Private Sub BuildStats(stats As DataSetStats, data As List(Of QuoteStat), Optional buildByLobs As Boolean = True)

            Dim DaysBetweenStartandFinzalized As New List(Of Double)
            For Each d In data
                stats.NewQuoteCount += 1
                If stats.AgencyCodes.Contains(d.AgencyCode) = False Then
                    stats.AgencyCodes.Add(d.AgencyCode)
                End If

                stats.Clientnames.Add(d.ClientName)

                If stats.QuoteCountByUser_Created.ContainsKey(d.QuoteCredtedByUserId) Then
                    stats.QuoteCountByUser_Created(d.QuoteCredtedByUserId) += 1
                Else
                    stats.QuoteCountByUser_Created.Add(d.QuoteCredtedByUserId, 1)
                End If

                If stats.QuoteCountByUser_LastModified.ContainsKey(d.LastModifiedBy) Then
                    stats.QuoteCountByUser_LastModified(d.LastModifiedBy) += 1
                Else
                    stats.QuoteCountByUser_LastModified.Add(d.LastModifiedBy, 1)
                End If

                If stats.QuoteCountByLob.ContainsKey(d.LobName) Then
                    stats.QuoteCountByLob(d.LobName) += 1
                Else
                    stats.QuoteCountByLob.Add(d.LobName, 1)
                End If

                stats.TotalQuotedPremium += d.Premium

                Select Case d.StatusId
                    Case 1, 2, 3, 4, 5, 6 ' quotes
                        stats.QuoteCountInQuoteStatus += 1
                    Case 7, 8, 9, 10 ' apps
                        stats.QuoteCountInAppStatus += 1
                    Case 13 '  Routed to UW
                        stats.QuoteCountInRoutedStatus += 1
                    Case 14 ' Finalized
                        stats.QuoteCountInFinalizedStatus += 1
                        stats.TotalFinalizedPremium += d.Premium
                        DaysBetweenStartandFinzalized.Add((d.LastModifiedDate - d.QuoteCreatedDate).TotalDays)
                End Select

                If buildByLobs Then '  need this to prevent forever loops
                    'get datasets by lobid
                    Dim byLob = From dd As QuoteStat In data
                                Group dd By dd.LobId Into Group
                                Select n = New TimeSliceDataSet With
                                       {
                                           .Data = Group.ToList(),
                                           .TimeFrameType = Me.TimeFrameType,
                                           .StartDate = Me.StartDate,
                                           .EndDate = Me.EndDate
                                       }
                                Order By n.StartDate Descending
                    stats.StatsByLob = byLob.ToList()
                    For Each st In stats.StatsByLob
                        st.AnalyzeData(False) ' don't want to build any sub slices from this slice
                    Next

                    'get datasets by create by user
                End If
            Next
            If DaysBetweenStartandFinzalized.Any() Then
                stats.DaysFromStartToFinalized_Avg = DaysBetweenStartandFinzalized.Average()
            End If

        End Sub

        Public Overrides Function ToString() As String
            If Data IsNot Nothing Then
                Return String.Format("{0} to {1} ({2})", StartDate.ToShortDateString(), EndDate.ToShortDateString(), Data.Count)
            End If
            Return String.Format("{0} to {1} (data null)", StartDate.ToShortDateString(), EndDate.ToShortDateString())
        End Function

    End Class

    Public Class DataSetStats
        Public Property NewQuoteCount As Int32

        'Dictionaries look like crap in json hide these and expose as an array of keyValuepairs
        Friend Property QuoteCountByUser_Created As New Dictionary(Of String, Int32) 'don't want this serialized to json
        Public ReadOnly Property QuoteCount_ByUserCreated As List(Of KeyValuePair(Of String, Int32))
            Get
                Dim data As New List(Of KeyValuePair(Of String, Int32))
                If QuoteCountByUser_Created IsNot Nothing Then
                    For Each pair In From f In QuoteCountByUser_Created Select f Order By f.Value Descending
                        data.Add(New KeyValuePair(Of String, Int32)(pair.Key, pair.Value))
                    Next
                End If
                Return data
            End Get
        End Property
        Friend Property QuoteCountByUser_LastModified As New Dictionary(Of String, Int32) 'don't want this serialized to json
        Public ReadOnly Property QuoteCount_ByUserLastModified As List(Of KeyValuePair(Of String, Int32))
            Get
                Dim data As New List(Of KeyValuePair(Of String, Int32))
                If QuoteCountByUser_Created IsNot Nothing Then
                    For Each pair In From f In QuoteCountByUser_LastModified Select f Order By f.Value Descending
                        data.Add(New KeyValuePair(Of String, Int32)(pair.Key, pair.Value))
                    Next
                End If
                Return data
            End Get
        End Property
        Friend Property QuoteCountByLob As New Dictionary(Of String, Int32) 'don't want this serialized to json
        Public ReadOnly Property QuoteCount_ByLob As List(Of KeyValuePair(Of String, Int32))
            Get
                Dim data As New List(Of KeyValuePair(Of String, Int32))
                If QuoteCountByUser_Created IsNot Nothing Then
                    For Each pair In From f In QuoteCountByLob Select f Order By f.Value Descending
                        data.Add(New KeyValuePair(Of String, Int32)(pair.Key, pair.Value))
                    Next
                End If
                Return data
            End Get
        End Property

        Public Property StatsByLob As List(Of TimeSliceDataSet)

        Public Property TotalQuotedPremium As Double
        Public Property TotalFinalizedPremium As Double

        Public Property QuoteCountInQuoteStatus As Int32
        Public ReadOnly Property PercentOfQuotesAsQuote As Double
            Get
                Return CDbl(String.Format("{0:N2}", (QuoteCountInQuoteStatus / NewQuoteCount) * 100.0))
            End Get
        End Property

        Public Property QuoteCountInAppStatus As Int32
        Public ReadOnly Property PercentOfQuotesAsApp As Double
            Get
                Return CDbl(String.Format("{0:N2}", (QuoteCountInAppStatus / NewQuoteCount) * 100.0))
            End Get
        End Property

        Public Property QuoteCountInRoutedStatus As Int32
        Public ReadOnly Property PercentOfQuotesRoutedToUW As Double
            Get
                Return CDbl(String.Format("{0:N2}", (QuoteCountInRoutedStatus / NewQuoteCount) * 100.0))
            End Get
        End Property

        Public Property QuoteCountInFinalizedStatus As Int32
        Public ReadOnly Property PercentOfQuotesFinalized As Double
            Get
                Return CDbl(String.Format("{0:N2}", (QuoteCountInFinalizedStatus / NewQuoteCount) * 100.0))
            End Get
        End Property

        Public Property AgencyCodes As New List(Of String)
        Friend Property Clientnames As New List(Of String) 'don't want this serialized to json
        Public ReadOnly Property ClientCount As Int32
            Get
                Return Clientnames.Count
            End Get
        End Property
        Public ReadOnly Property UniqueClientNamesCount As Int32
            Get
                Return Clientnames.Distinct().Count
            End Get
        End Property
        Public Property DaysFromStartToFinalized_Avg As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} quotes in sample", NewQuoteCount)
        End Function
    End Class

    Public Class QuoteStat
        Public QuoteNumber As String
        Public ClientName As String
        Public QuoteId As Int32
        Public Premium As Double
        Public LobId As Int32
        Public LobName As String
        Public AgencyCode As String
        Public LastModifiedBy As String
        Public QuoteCreatedDate As DateTime
        Public QuoteCredtedByUserId As Int32
        Public LastModifiedDate As DateTime
        Public Status As String
        Public StatusId As Int32
        Public PolicyNumber As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} {2}", QuoteCreatedDate.ToShortDateString(), LobName, Status)
        End Function
    End Class

End Namespace