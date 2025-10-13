Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports System.Configuration.ConfigurationManager

Namespace IFM.VR.Common.Helpers.BOP

    Public Class NaicsCodeHelper

        Public Shared Function LookUpNaicsDescription(NaicsCode As String) As String
            If String.IsNullOrEmpty(NaicsCode) Then
                Return String.Empty
            End If
            Using sproc As New SPManager("connDiamondReports", "usp_Get_NAICS_Description")
                sproc.AddStringParameter("@NaicsCode", NaicsCode)
                Dim Data = sproc.ExecuteSPQuery()
                If Data IsNot Nothing AndAlso Data.Rows.Count > 0 AndAlso Data.Rows(0).ItemArray.Length > 0 AndAlso Not String.IsNullOrWhiteSpace(Data.Rows(0).Item(0)) Then
                    Return Data.Rows(0).Item(0).ToString
                Else
                    Return String.Empty
                End If
            End Using
        End Function

        Public Shared Function GetNaicsCodes(SearchTypeid As String, searchString As String) As List(Of NaicsCodeLookupResult)
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Dim spm As New SPManager("connDiamondReports", "usp_Get_NAICS_ClassCodeData")

            Select Case SearchTypeid
                Case "1"  ' Item Number
                    spm.AddStringParameter("@SearchItemNum", searchString.Trim)
                    Exit Select
                Case "2"  ' Description Starts with
                    spm.AddStringParameter("@SearchStartsWithDscr", searchString.Trim)
                    Exit Select
                Case Else  ' Description Contains
                    spm.AddStringParameter("@SearchContainsDscr", searchString.Trim)
                    Exit Select
            End Select

            Dim tbl As DataTable = spm.ExecuteSPQuery()
            Dim results As New List(Of NaicsCodeLookupResult)
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                For Each dr As DataRow In tbl.Rows
                    Dim item As New NaicsCodeLookupResult
                    item.NacisType_Id = qqh.IntegerForString(dr(0).ToString)
                    item.Code = dr(1).ToString
                    item.Description = dr(2).ToString
                    results.Add(item)
                Next
            End If

            Return results
        End Function
    End Class

    Public Class NaicsCodeLookupResult
        Public Property NacisType_Id As Int32
        Public Property Code As String
        Public Property Description As String

    End Class

End Namespace
