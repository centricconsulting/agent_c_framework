
Imports System.Configuration
Imports System.Data.SqlClient

Namespace IFM.VR.Common.Helpers.BOP
    Public Class QueryHelper

        Public Function GetClassDescFromClassification(dia_classificationtype_id As Integer) As String
            Dim Data = GetDataByClassification(dia_classificationtype_id)
            Return Data.Rows(0).Item("ClassDesc").ToString
        End Function

        Public Function GetBOPClassIdFromClassification(dia_classificationtype_id As Integer) As String
            Dim Data = GetDataByClassification(dia_classificationtype_id)
            Return Data.Rows(0).Item("BOPClass_Id").ToString
        End Function

        Public Function GetProgramNameFromClassification(dia_classificationtype_id As Integer) As String
            Dim Data = GetDataByClassification(dia_classificationtype_id)
            Select Case Data.Rows(0).Item("ProgramType").ToString().ToUpper() ' Just let it fail
                Case "AP"
                    Return "Apartments"
                    Exit Select
                Case "CT"
                    Return "Contractors"
                    Exit Select
                Case "CTO"
                    Return "Contractors - Office"
                    Exit Select
                Case "CTS"
                    Return "Contractors - Service"
                    Exit Select
                Case "MO"
                    Return "MO???"
                    Exit Select
                Case "OF"
                    Return "Office"
                    Exit Select
                Case "RE"
                    Return "Retail"
                    Exit Select
                Case "RS"
                    Return "Restaurant"
                    Exit Select
                Case "SE"
                    Return "Service"
                    Exit Select
                Case "WH"
                    Return "Warehouse"
                    Exit Select
                Case Else
                    Return "UnKnown"
                    Exit Select
            End Select

        End Function

        Public Function GetProgramNameByClassCode(classcode As String) As String
            Using sproc As New SPManager("ConnQQ", "usp_get_BopClassNewData")
                sproc.AddStringParameter("@ClassCode", classcode.TrimStart("0"c))
                Dim Data = sproc.ExecuteSPQuery()
                Select Case Data.Rows(0).Item("ProgramType").ToString().ToUpper() ' Just let it fail
                    Case "AP"
                        Return "Apartments"
                        Exit Select
                    Case "CT"
                        Return "Contractors"
                        Exit Select
                    Case "CTO"
                        Return "Contractors - Office"
                        Exit Select
                    Case "CTS"
                        Return "Contractors - Service"
                        Exit Select
                    Case "MO"
                        Return "MO???"
                        Exit Select
                    Case "OF"
                        Return "Office"
                        Exit Select
                    Case "RE"
                        Return "Retail"
                        Exit Select
                    Case "RS"
                        Return "Restaurant"
                        Exit Select
                    Case "SE"
                        Return "Service"
                        Exit Select
                    Case "WH"
                        Return "Warehouse"
                        Exit Select
                    Case Else
                        Return "UnKnown"
                        Exit Select
                End Select
            End Using
        End Function

        Public Function GetProgramNameByBOPClassId(BOPClassId As Integer) As String
            Dim Data = GetDataByBOPClassId(BOPClassId)
            Select Case Data.Rows(0).Item("ProgramType").ToString().ToUpper() ' Just let it fail
                Case "AP"
                    Return "Apartments"
                    Exit Select
                Case "CT"
                    Return "Contractors"
                    Exit Select
                Case "CTO"
                    Return "Contractors - Office"
                    Exit Select
                Case "CTS"
                    Return "Contractors - Service"
                    Exit Select
                Case "MO"
                    Return "MO???"
                    Exit Select
                Case "OF"
                    Return "Office"
                    Exit Select
                Case "RE"
                    Return "Retail"
                    Exit Select
                Case "RS"
                    Return "Restaurant"
                    Exit Select
                Case "SE"
                    Return "Service"
                    Exit Select
                Case "WH"
                    Return "Warehouse"
                    Exit Select
                Case Else
                    Return "UnKnown"
                    Exit Select
            End Select
        End Function

        Public Function GetClassCodeByBOPClassId(BOPClassId As Integer) As String
            Dim Data = GetDataByBOPClassId(BOPClassId)
            Return Data.Rows(0).Item("ClassCode").ToString
        End Function

        Public Function GetClassificationtypeByBOPClassId(BOPClassId As Integer) As String
            Dim Data = GetDataByBOPClassId(BOPClassId)
            Return Data.Rows(0).Item("Dia_classificationtype_id").ToString
        End Function

        Public Function GetFootnoteByBOPClassId(BOPClassId As Integer) As String
            Dim Data = GetDataByBOPClassId(BOPClassId)
            Return Data.Rows(0).Item("Footnote").ToString
        End Function

        Public Function GetClassDescByProgramType(ProgramType As String) As DataTable
            Using sproc As New SPManager("ConnQQ", "usp_get_BopClassNewData")
                sproc.AddStringParameter("@ProgramType", ProgramType)
                sproc.AddBitParamater("@UsedInNewBOP", 1)
                Dim Data = sproc.ExecuteSPQuery()
                Dim SortedTable As DataView = Data.DefaultView
                SortedTable.Sort = "ClassDesc asc"
                Return SortedTable.ToTable()
            End Using
        End Function

        'Consolidated Calls
        Public Function GetDataByClassification(dia_classificationtype_id As Integer) As DataTable
            Using sproc As New SPManager("ConnQQ", "usp_get_BopClassNewData")
                sproc.AddIntegerParamater("@dia_classificationtype_id", dia_classificationtype_id)
                sproc.AddBitParamater("@UsedInNewBOP", 1)
                Return sproc.ExecuteSPQuery()
            End Using
        End Function

        Public Function GetDataByBOPClassId(BOPClassId As Integer) As DataTable
            Using sproc As New SPManager("ConnQQ", "usp_get_BopClassNewData")
                sproc.AddStringParameter("@BOPClassId", BOPClassId)
                sproc.AddBitParamater("@UsedInNewBOP", 1)
                Return sproc.ExecuteSPQuery()
            End Using
        End Function

    End Class
End Namespace


