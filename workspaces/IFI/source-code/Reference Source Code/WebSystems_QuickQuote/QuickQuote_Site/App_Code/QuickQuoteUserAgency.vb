Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store user agency information
    ''' </summary>
    ''' <remarks>used when building drop-down lists for available agencies</remarks>
    <Serializable()> _
    Public Class QuickQuoteUserAgency '8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        Implements IDisposable

        ''' <summary>
        ''' signifies how the agency is linked to the user
        ''' </summary>
        ''' <remarks></remarks>
        Enum QuickQuoteUserAgencyRelationshipType
            None = 0
            Primary = 1
            Secondary = 2
        End Enum

        Private _DiamondAgencyId As String
        Private _DiamondAgencyCode As String
        Private _Name As String
        Private _Code6000 As String
        Private _ShortAgencyCode As String
        Private _CloseDate As String
        Private _UserAgencyRelationshipTypeId As String
        Private _UserAgencyRelationshipType As QuickQuoteUserAgencyRelationshipType

        Public Property DiamondAgencyId As String
            Get
                Return _DiamondAgencyId
            End Get
            Set(value As String)
                _DiamondAgencyId = value
            End Set
        End Property
        Public Property DiamondAgencyCode As String
            Get
                Return _DiamondAgencyCode
            End Get
            Set(value As String)
                _DiamondAgencyCode = value
                If Len(_DiamondAgencyCode) = 9 AndAlso IsNumeric(Left(_DiamondAgencyCode, 4)) = True AndAlso IsNumeric(Right(_DiamondAgencyCode, 4)) = True AndAlso Mid(_DiamondAgencyCode, 5, 1) = "-" Then
                    _Code6000 = Left(_DiamondAgencyCode, 4)
                    _ShortAgencyCode = Right(_DiamondAgencyCode, 4)
                End If
            End Set
        End Property
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property
        Public ReadOnly Property Code6000 As String
            Get
                Return _Code6000
            End Get
        End Property
        Public ReadOnly Property ShortAgencyCode As String
            Get
                Return _ShortAgencyCode
            End Get
        End Property
        Public Property CloseDate As String
            Get
                Return _CloseDate
            End Get
            Set(value As String)
                _CloseDate = value
            End Set
        End Property
        Public Property UserAgencyRelationshipTypeId As String
            Get
                Return _UserAgencyRelationshipTypeId
            End Get
            Set(value As String)
                _UserAgencyRelationshipTypeId = value
                If IsNumeric(_UserAgencyRelationshipTypeId) = True Then
                    Select Case _UserAgencyRelationshipTypeId
                        Case "1"
                            _UserAgencyRelationshipType = QuickQuoteUserAgencyRelationshipType.Primary
                        Case "2"
                            _UserAgencyRelationshipType = QuickQuoteUserAgencyRelationshipType.Secondary
                        Case Else
                            _UserAgencyRelationshipType = QuickQuoteUserAgencyRelationshipType.None
                    End Select
                End If
            End Set
        End Property
        Public ReadOnly Property UserAgencyRelationshipType As QuickQuoteUserAgencyRelationshipType
            Get
                Return _UserAgencyRelationshipType
            End Get
        End Property

        Public Sub New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _DiamondAgencyId = ""
            _DiamondAgencyCode = ""
            _Name = ""
            _Code6000 = ""
            _ShortAgencyCode = ""
            _CloseDate = ""
            _UserAgencyRelationshipTypeId = ""
            _UserAgencyRelationshipType = QuickQuoteUserAgencyRelationshipType.None
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _DiamondAgencyId IsNot Nothing Then
                        _DiamondAgencyId = Nothing
                    End If
                    If _DiamondAgencyCode IsNot Nothing Then
                        _DiamondAgencyCode = Nothing
                    End If
                    If _Name IsNot Nothing Then
                        _Name = Nothing
                    End If
                    If _Code6000 IsNot Nothing Then
                        _Code6000 = Nothing
                    End If
                    If _ShortAgencyCode IsNot Nothing Then
                        _ShortAgencyCode = Nothing
                    End If
                    If _CloseDate IsNot Nothing Then
                        _CloseDate = Nothing
                    End If
                    If _UserAgencyRelationshipTypeId IsNot Nothing Then
                        _UserAgencyRelationshipTypeId = Nothing
                    End If
                    If _UserAgencyRelationshipType <> Nothing Then
                        _UserAgencyRelationshipType = Nothing
                    End If
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
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
