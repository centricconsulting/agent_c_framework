Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CGL
    Public Class CommRiskGradeHelper

        Public Shared Function GetRiskCodes(riskGradeSearchTypeid As String, searchString As String, versionID As String) As List(Of RiskGradeLookupResult)

            Dim results As New List(Of RiskGradeLookupResult)


            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    conn.Open()
                    cmd.CommandText = "usp_RiskGrade_Search"
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@riskgradesearchtype_id", riskGradeSearchTypeid)
                    cmd.Parameters.AddWithValue("@searchstring", searchString)
                    cmd.Parameters.AddWithValue("@version_id", versionID)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim r As New RiskGradeLookupResult()
                                r.RiskGradeLookupId = reader.GetInt32(0)
                                r.GlClasscode = reader.GetString(1)
                                r.Description = reader.GetString(2)
                                r.PropertyGrade = reader.GetString(3)
                                r.GlGrade = reader.GetString(4)
                                r.AutoGrade = reader.GetString(5)
                                r.WcGrade = reader.GetString(6)
                                results.Add(r)
                            End While
                        End If
                    End Using
                End Using
            End Using

            Return results
        End Function

    End Class

    Public Class RiskGradeLookupResult
        Public Property RiskGradeLookupId As Int32
        Public Property GlClasscode As String
        Public Property Description As String

        ' These can be 1,2,3,P
        Public Property PropertyGrade As String
        Public Property GlGrade As String
        Public Property AutoGrade As String
        Public Property WcGrade As String

        Public ReadOnly Property IsUnaccepablePropertyGrade As Boolean
            Get
                Return PropertyGrade.ToUpper().Trim().EqualsAny("3", "P")
            End Get
        End Property

        Public ReadOnly Property IsUnaccepableGlGrade As Boolean
            Get
                Return GlGrade.ToUpper().Trim().EqualsAny("3", "P")
            End Get
        End Property

        Public ReadOnly Property IsUnacceptableCppRiskGrade As Boolean
            Get
                Return IsUnaccepablePropertyGrade Or IsUnaccepableGlGrade
            End Get
        End Property

        Public ReadOnly Property IsUnAcceptableAutoGrade As Boolean
            Get
                Return AutoGrade.Trim().ToUpper().EqualsAny("3", "P")
            End Get
        End Property

        Public ReadOnly Property IsUnacceptableWcGrade As Boolean
            Get
                Return WcGrade.Trim().ToUpper().EqualsAny("3", "P")
            End Get
        End Property


        'If 3 or P it is not an acceptable risk
    End Class

End Namespace

