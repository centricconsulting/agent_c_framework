Imports System.Configuration
Imports System.Data.SqlClient

Namespace IFM.VR.Common.Helpers

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>   Manager for Stored Proc Usage. </summary>
    '''
    ''' <remarks>   Chhaw, 06/17/2020. </remarks>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    Public Class SPManager
        Implements IDisposable

        ''' <summary>   Options for controlling the SQL. </summary>
        Private _SqlParameters As List(Of SqlParameter)
        ''' <summary>   The commandtext. </summary>
        Private _Commandtext As String
        ''' <summary>   The con string. </summary>
        Private _ConStr As String

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Constructor. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="Connection"> The connection string as a web.config Key value. EX: "conn". </param>
        ''' <param name="CommandText_SP"> The stored proc name. EX:
        '''     "usp_BOPCLASSNEW_ClassCode_Classification_Search". </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub New(Connection As String, CommandText_SP As String)
            _ConStr = Connection
            _Commandtext = CommandText_SP
            _SqlParameters = New List(Of SqlParameter)()
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Constructor. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="Connection"> The connection string as a web.config Key value. EX: "conn". </param>
        ''' <param name="CommandText_SP"> The stored proc name. EX:
        '''     "usp_BOPCLASSNEW_ClassCode_Classification_Search". </param>
        ''' <param name="SqlParameters"> List of SqlParameter items as Options for controlling the SQL. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub New(Connection As String, CommandText_SP As String, SqlParameters As List(Of SqlParameter))
            _ConStr = Connection
            _Commandtext = CommandText_SP
            If SqlParameters IsNot Nothing And SqlParameters.Count > 0 Then
                _SqlParameters = SqlParameters
            Else
                _SqlParameters = New List(Of SqlParameter)()
            End If

        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Gets or sets options for controlling the operation. </summary>
        '''
        ''' <value> The parameters. </value>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Property Parameters() As List(Of SqlParameter)
            Get
                Return _SqlParameters
            End Get
            Set
                Value = _SqlParameters
            End Set
        End Property

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Gets or sets the command text. </summary>
        '''
        ''' <value> The command text. </value>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Property CommandText() As String
            Get
                Return _Commandtext
            End Get
            Set
                Value = _Commandtext
            End Set
        End Property

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Gets or sets the connection. </summary>
        '''
        ''' <value> The connection. </value>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Property Conn() As String
            Get
                Return _ConStr
            End Get
            Set

                Value = _ConStr
            End Set
        End Property

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Executes a Stored Proc query operation. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <returns>   A DataTable. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Function ExecuteSPQuery() As DataTable
            Dim tbl As New DataTable()
            If String.IsNullOrWhiteSpace(Conn) = False AndAlso String.IsNullOrWhiteSpace(CommandText) = False Then
                Using con As New SqlConnection(ConfigurationManager.AppSettings(_ConStr))
                    Using com As New SqlCommand(_Commandtext, con)
                        Dim da As New SqlDataAdapter()

                        com.CommandType = CommandType.StoredProcedure
                        If Parameters.Count > 0 Then
                            com.Parameters.AddRange(_SqlParameters.ToArray())
                        End If

                        con.Open()
                        da = New SqlDataAdapter()
                        da.SelectCommand = com
                        da.Fill(tbl)

                        If tbl Is Nothing OrElse tbl.Rows.Count < 0 Then tbl = New DataTable()

                        Return tbl
                        con.Close()
                    End Using
                End Using
            End If
            Return tbl
        End Function

        'Strings/Chars

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a string parameter to 'value'. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddStringParameter(key As String, value As String)
            Dim param = New SqlParameter(key, SqlDbType.VarChar)
            param.Value = value
            Parameters.Add(param)
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds an unicode string parameter to 'value'. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddUnicodeStringParameter(key As String, value As String)
            Dim param = New SqlParameter(key, SqlDbType.NVarChar)
            param.Value = value
            Parameters.Add(param)
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a character parameter to 'value'. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddCharParameter(key As String, value As Char)
            Dim param = New SqlParameter(key, SqlDbType.Char)
            param.Value = value
            Parameters.Add(param)
        End Sub

        'DateTime

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a date time parameter to 'value'. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddDateTimeParameter(key As String, value As DateTime)
            Dim param = New SqlParameter(key, SqlDbType.DateTime)
            param.Value = value
            Parameters.Add(param)
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a date time 2 parameter. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        ''' <param name="scale">    (Optional) The scale. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddDateTime2Parameter(key As String, value As DateTime, Optional scale As Integer = 3)
            Dim param = New SqlParameter(key, SqlDbType.DateTime2)
            param.Value = value
            param.Scale = scale
            Parameters.Add(param)
        End Sub

        'Numeric

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a decimal parameter. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        ''' <param name="precision">    The precision. </param>
        ''' <param name="scale">    The scale. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddDecimalParameter(key As String, value As Decimal, precision As Integer, scale As Integer)
            Dim param = New SqlParameter(key, SqlDbType.Decimal)
            param.Value = value
            param.Scale = scale
            param.Precision = precision
            Parameters.Add(param)
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a Money based decimal parameter. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        ''' <param name="precision">    (Optional) The precision. </param>
        ''' <param name="scale">    (Optional) The scale. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddMoneyParameter(key As String, value As Decimal, Optional precision As Integer = 38, Optional scale As Integer = 2)
            Dim param = New SqlParameter(key, SqlDbType.Decimal)
            param.Value = value
            param.Scale = scale
            param.Precision = precision
            Parameters.Add(param)
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds an integer paramater to 'value'. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddIntegerParamater(key As String, value As Integer)
            Dim param = New SqlParameter(key, SqlDbType.Int)
            param.Value = value
            Parameters.Add(param)
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds an bit paramater to 'value'. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddBitParamater(key As String, value As Integer)
            Dim param = New SqlParameter(key, SqlDbType.Bit)
            param.Value = value
            Parameters.Add(param)
        End Sub

        'Generic Mulipurpose

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a generic parameter. </summary>
        '''
        ''' <remarks>   Chhaw, 06/17/2020. </remarks>
        '''
        ''' <param name="key">  The key. </param>
        ''' <param name="value">    The value. </param>
        ''' <param name="sqlType">  Type of the SQL. </param>
        ''' <param name="precision">    (Optional) The precision. </param>
        ''' <param name="scale">    (Optional) The scale. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Sub AddGenericParameter(key As String, value As Object, sqlType As SqlDbType, Optional precision As Integer = 0, Optional scale As Integer = 0)
            Dim param = New SqlParameter(key, sqlType)
            param.Value = value
            param.Scale = scale
            param.Precision = precision
            Parameters.Add(param)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    If _SqlParameters IsNot Nothing Then
                        _SqlParameters = Nothing
                    End If
                    If _Commandtext IsNot Nothing Then
                        _Commandtext = Nothing
                    End If
                    If _ConStr IsNot Nothing Then
                        _ConStr = Nothing
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region




    End Class
End Namespace


