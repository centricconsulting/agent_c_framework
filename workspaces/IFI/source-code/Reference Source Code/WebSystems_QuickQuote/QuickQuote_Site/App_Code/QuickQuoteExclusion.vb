Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 4/27/2014

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store exclusion information
    ''' </summary>
    ''' <remarks>currently used as list object under Location object (<see cref="QuickQuoteLocation"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteExclusion 'added 8/1/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 4/27/2014

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Description As String
        Private _ExclusionTypeId As String 'may need matching ExclusionType variable/property
        Private _ExclusionNum As String 'added 4/23/2014 for reconciliation

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ExclusionType table (-1=[blank or empty string], 0=N/A; 1=Exclusion, 2=Exclusion Cov G, 3=Restriction, 4=Comment)</remarks>
        Public Property ExclusionTypeId As String '-1=[blank or empty string]; 0=N/A; 1=Exclusion; 2=Exclusion Cov G; 3=Restriction; 4=Comment
            Get
                Return _ExclusionTypeId
            End Get
            Set(value As String)
                _ExclusionTypeId = value
            End Set
        End Property
        Public Property ExclusionNum As String 'added 4/23/2014 for reconciliation
            Get
                Return _ExclusionNum
            End Get
            Set(value As String)
                _ExclusionNum = value
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _Description = ""
            _ExclusionTypeId = ""
            _ExclusionNum = "" 'added 4/23/2014 for reconciliation

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidExclusionNum() As Boolean 'added 4/23/2014 for reconciliation purposes
            'If _ExclusionNum <> "" AndAlso IsNumeric(_ExclusionNum) = True AndAlso CInt(_ExclusionNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ExclusionNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.ExclusionTypeId <> "" Then
                    Dim e As String = ""
                    e = "ExclusionTypeId: " & Me.ExclusionTypeId
                    Dim eType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteExclusion, QuickQuoteHelperClass.QuickQuotePropertyName.ExclusionTypeId, Me.ExclusionTypeId)
                    If eType <> "" Then
                        e &= " (" & eType & ")"
                    End If
                    str = qqHelper.appendText(str, e, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _ExclusionTypeId IsNot Nothing Then
                        _ExclusionTypeId = Nothing
                    End If
                    If _ExclusionNum IsNot Nothing Then 'added 4/23/2014 for reconciliation
                        _ExclusionNum = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    MyBase.Dispose() 'added 8/4/2014
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
