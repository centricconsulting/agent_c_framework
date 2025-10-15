Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store supplement A information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteSupplementA 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _OrganizationCodeLevel1 As String
        Private _OrganizationCodeLevel2 As String
        Private _OrganizationCodeLevel3 As String
        Private _OrganizationCodeLevel4 As String
        Private _SpecialField1 As String
        Private _SpecialField2 As String
        Private _SpecialField3 As String
        Private _SpecialFieldA As String
        Private _SpecialFieldB As String
        Private _SpecialFieldC As String
        Private _SpecialNumericField1 As String

        Public Property OrganizationCodeLevel1 As String
            Get
                Return _OrganizationCodeLevel1
            End Get
            Set(value As String)
                _OrganizationCodeLevel1 = value
            End Set
        End Property
        Public Property OrganizationCodeLevel2 As String
            Get
                Return _OrganizationCodeLevel2
            End Get
            Set(value As String)
                _OrganizationCodeLevel2 = value
            End Set
        End Property
        Public Property OrganizationCodeLevel3 As String
            Get
                Return _OrganizationCodeLevel3
            End Get
            Set(value As String)
                _OrganizationCodeLevel3 = value
            End Set
        End Property
        Public Property OrganizationCodeLevel4 As String
            Get
                Return _OrganizationCodeLevel4
            End Get
            Set(value As String)
                _OrganizationCodeLevel4 = value
            End Set
        End Property
        Public Property SpecialField1 As String
            Get
                Return _SpecialField1
            End Get
            Set(value As String)
                _SpecialField1 = value
            End Set
        End Property
        Public Property SpecialField2 As String
            Get
                Return _SpecialField2
            End Get
            Set(value As String)
                _SpecialField2 = value
            End Set
        End Property
        Public Property SpecialField3 As String
            Get
                Return _SpecialField3
            End Get
            Set(value As String)
                _SpecialField3 = value
            End Set
        End Property
        Public Property SpecialFieldA As String
            Get
                Return _SpecialFieldA
            End Get
            Set(value As String)
                _SpecialFieldA = value
            End Set
        End Property
        Public Property SpecialFieldB As String
            Get
                Return _SpecialFieldB
            End Get
            Set(value As String)
                _SpecialFieldB = value
            End Set
        End Property
        Public Property SpecialFieldC As String
            Get
                Return _SpecialFieldC
            End Get
            Set(value As String)
                _SpecialFieldC = value
            End Set
        End Property
        Public Property SpecialNumericField1 As String
            Get
                Return _SpecialNumericField1
            End Get
            Set(value As String)
                _SpecialNumericField1 = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _OrganizationCodeLevel1 = ""
            _OrganizationCodeLevel2 = ""
            _OrganizationCodeLevel3 = ""
            _OrganizationCodeLevel4 = ""
            _SpecialField1 = ""
            _SpecialField2 = ""
            _SpecialField3 = ""
            _SpecialFieldA = ""
            _SpecialFieldB = ""
            _SpecialFieldC = ""
            _SpecialNumericField1 = ""
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _OrganizationCodeLevel1 IsNot Nothing Then
                        _OrganizationCodeLevel1 = Nothing
                    End If
                    If _OrganizationCodeLevel2 IsNot Nothing Then
                        _OrganizationCodeLevel2 = Nothing
                    End If
                    If _OrganizationCodeLevel3 IsNot Nothing Then
                        _OrganizationCodeLevel3 = Nothing
                    End If
                    If _OrganizationCodeLevel4 IsNot Nothing Then
                        _OrganizationCodeLevel4 = Nothing
                    End If
                    If _SpecialField1 IsNot Nothing Then
                        _SpecialField1 = Nothing
                    End If
                    If _SpecialField2 IsNot Nothing Then
                        _SpecialField2 = Nothing
                    End If
                    If _SpecialField3 IsNot Nothing Then
                        _SpecialField3 = Nothing
                    End If
                    If _SpecialFieldA IsNot Nothing Then
                        _SpecialFieldA = Nothing
                    End If
                    If _SpecialFieldB IsNot Nothing Then
                        _SpecialFieldB = Nothing
                    End If
                    If _SpecialFieldC IsNot Nothing Then
                        _SpecialFieldC = Nothing
                    End If
                    If _SpecialNumericField1 IsNot Nothing Then
                        _SpecialNumericField1 = Nothing
                    End If

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
