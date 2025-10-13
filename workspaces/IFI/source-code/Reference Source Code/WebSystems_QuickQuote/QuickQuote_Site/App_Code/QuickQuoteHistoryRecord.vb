Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store QuickQuote history information
    ''' </summary>
    ''' <remarks>used when creating Diamond note that encompasses all VelociRater activity</remarks>
    <Serializable()> _
    Public Class QuickQuoteHistoryRecord 'added 7/10/2013; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        Implements IDisposable

        ''' <summary>
        ''' valid types that signify the origin of a quote's quote xml (applicable once app gap has been started)
        ''' </summary>
        ''' <remarks>rate means the xml was new to that QuoteXml record; reuse means that it was copied from a previous QuoteXml record</remarks>
        Public Enum SecondaryUserQuoteType 'added 7/10/2013 PM
            None = 0
            Rate = 1
            Reuse = 2
        End Enum

        Dim _quoteXmlId As Integer
        Dim _quoteInserted As String
        Dim _initialUserId As Integer
        Dim _initialUserName As String
        Dim _quoteUserId As Integer
        Dim _quoteUserName As String
        Dim _appGapUserId As Integer
        Dim _appGapUserName As String
        Dim _quoteState As String
        Dim _appGapState As String
        Dim _xmlStatus As String
        Dim _ratedQuoteSuccess As Boolean
        Dim _ratedAppGapSuccess As Boolean
        Dim _xmlUpdated As String

        Dim _currUserId As Integer
        Dim _currUserName As String

        Dim _quoteUpdatedUserId As String
        Dim _quoteUpdatedUserName As String
        Dim _quoteStatus As String
        Dim _quoteUpdated As String

        Dim _ratedQuoteNumber As String

        Dim _secondaryUserId As Integer
        Dim _secondaryUserName As String

        Dim _xmlInserted As String

        Dim _quoteXmlLength As Integer
        Dim _ratedQuoteXmlLength As Integer
        Dim _appGapXmlLength As Integer
        Dim _ratedAppGapXmlLength As Integer

        Dim _secondaryUserOriginalQuoteXmlId As Integer

        Dim _secondaryQuoteType As SecondaryUserQuoteType 'added 7/10/2013 PM

        'added 7/10/2013 PM
        Dim _hasAnyUserChange As Boolean
        Dim _hasLastUpdatedUserChange As Boolean
        Dim _hasSecondaryUserChange As Boolean
        Dim _hasDifferentSecondaryUser As Boolean 'added 7/11/2013
        Dim _keptSameUserButDroppedDifferentSecondaryUser As Boolean 'added 7/11/2013

        Dim _wasPreviouslyRated As Boolean 'added 7/12/2013

        Dim _hasChangeInRatedQuoteXml As Boolean 'added 7/15/2013

        Public Property quoteXmlId As Integer
            Get
                Return _quoteXmlId
            End Get
            Set(value As Integer)
                _quoteXmlId = value
            End Set
        End Property
        Public Property quoteInserted As String
            Get
                Return _quoteInserted
            End Get
            Set(value As String)
                _quoteInserted = value
            End Set
        End Property
        Public Property initialUserId As Integer
            Get
                Return _initialUserId
            End Get
            Set(value As Integer)
                _initialUserId = value
            End Set
        End Property
        Public Property initialUserName As String
            Get
                Return _initialUserName
            End Get
            Set(value As String)
                _initialUserName = value
            End Set
        End Property
        Public Property quoteUserId As Integer
            Get
                Return _quoteUserId
            End Get
            Set(value As Integer)
                _quoteUserId = value
            End Set
        End Property
        Public Property quoteUserName As String
            Get
                Return _quoteUserName
            End Get
            Set(value As String)
                _quoteUserName = value
            End Set
        End Property
        Public Property appGapUserId As Integer
            Get
                Return _appGapUserId
            End Get
            Set(value As Integer)
                _appGapUserId = value
            End Set
        End Property
        Public Property appGapUserName As String
            Get
                Return _appGapUserName
            End Get
            Set(value As String)
                _appGapUserName = value
            End Set
        End Property
        Public Property quoteState As String
            Get
                Return _quoteState
            End Get
            Set(value As String)
                _quoteState = value
            End Set
        End Property
        Public Property appGapState As String
            Get
                Return _appGapState
            End Get
            Set(value As String)
                _appGapState = value
            End Set
        End Property
        Public Property xmlStatus As String
            Get
                Return _xmlStatus
            End Get
            Set(value As String)
                _xmlStatus = value
            End Set
        End Property
        Public Property ratedQuoteSuccess As Boolean
            Get
                Return _ratedQuoteSuccess
            End Get
            Set(value As Boolean)
                _ratedQuoteSuccess = value
            End Set
        End Property
        Public Property ratedAppGapSuccess As Boolean
            Get
                Return _ratedAppGapSuccess
            End Get
            Set(value As Boolean)
                _ratedAppGapSuccess = value
            End Set
        End Property
        Public Property xmlUpdated As String
            Get
                Return _xmlUpdated
            End Get
            Set(value As String)
                _xmlUpdated = value
            End Set
        End Property

        Public Property currUserId As Integer
            Get
                Return _currUserId
            End Get
            Set(value As Integer)
                _currUserId = value
            End Set
        End Property
        Public Property currUserName As String
            Get
                Return _currUserName
            End Get
            Set(value As String)
                _currUserName = value
            End Set
        End Property

        Public Property quoteUpdatedUserId As String
            Get
                Return _quoteUpdatedUserId
            End Get
            Set(value As String)
                _quoteUpdatedUserId = value
            End Set
        End Property
        Public Property quoteUpdatedUserName As String
            Get
                Return _quoteUpdatedUserName
            End Get
            Set(value As String)
                _quoteUpdatedUserName = value
            End Set
        End Property
        Public Property quoteStatus As String
            Get
                Return _quoteStatus
            End Get
            Set(value As String)
                _quoteStatus = value
            End Set
        End Property
        Public Property quoteUpdated As String
            Get
                Return _quoteUpdated
            End Get
            Set(value As String)
                _quoteUpdated = value
            End Set
        End Property

        Public Property ratedQuoteNumber As String
            Get
                Return _ratedQuoteNumber
            End Get
            Set(value As String)
                _ratedQuoteNumber = value
            End Set
        End Property

        Public Property secondaryUserId As Integer
            Get
                Return _secondaryUserId
            End Get
            Set(value As Integer)
                _secondaryUserId = value
            End Set
        End Property
        Public Property secondaryUserName As String
            Get
                Return _secondaryUserName
            End Get
            Set(value As String)
                _secondaryUserName = value
            End Set
        End Property

        Public Property xmlInserted As String
            Get
                Return _xmlInserted
            End Get
            Set(value As String)
                _xmlInserted = value
            End Set
        End Property

        Public Property quoteXmlLength As Integer
            Get
                Return _quoteXmlLength
            End Get
            Set(value As Integer)
                _quoteXmlLength = value
            End Set
        End Property
        Public Property ratedQuoteXmlLength As Integer
            Get
                Return _ratedQuoteXmlLength
            End Get
            Set(value As Integer)
                _ratedQuoteXmlLength = value
            End Set
        End Property
        Public Property appGapXmlLength As Integer
            Get
                Return _appGapXmlLength
            End Get
            Set(value As Integer)
                _appGapXmlLength = value
            End Set
        End Property
        Public Property ratedAppGapXmlLength As Integer
            Get
                Return _ratedAppGapXmlLength
            End Get
            Set(value As Integer)
                _ratedAppGapXmlLength = value
            End Set
        End Property

        Public Property secondaryUserOriginalQuoteXmlId As Integer
            Get
                Return _secondaryUserOriginalQuoteXmlId
            End Get
            Set(value As Integer)
                _secondaryUserOriginalQuoteXmlId = value
            End Set
        End Property

        Public Property secondaryQuoteType() As SecondaryUserQuoteType 'added 7/10/2013 PM
            Get
                Return _secondaryQuoteType
            End Get
            Set(ByVal value As SecondaryUserQuoteType)
                _secondaryQuoteType = value
            End Set
        End Property

        'added 7/10/2013 PM
        Public Property hasAnyUserChange() As Boolean
            Get
                Return _hasAnyUserChange
            End Get
            Set(ByVal value As Boolean)
                _hasAnyUserChange = value
            End Set
        End Property
        Public Property hasLastUpdatedUserChange() As Boolean
            Get
                Return _hasLastUpdatedUserChange
            End Get
            Set(ByVal value As Boolean)
                _hasLastUpdatedUserChange = value
            End Set
        End Property
        Public Property hasSecondaryUserChange() As Boolean
            Get
                Return _hasSecondaryUserChange
            End Get
            Set(ByVal value As Boolean)
                _hasSecondaryUserChange = value
            End Set
        End Property
        Public Property hasDifferentSecondaryUser As Boolean 'added 7/11/2013
            Get
                Return _hasDifferentSecondaryUser
            End Get
            Set(value As Boolean)
                _hasDifferentSecondaryUser = value
            End Set
        End Property
        Public Property keptSameUserButDroppedDifferentSecondaryUser As Boolean 'added 7/11/2013
            Get
                Return _keptSameUserButDroppedDifferentSecondaryUser
            End Get
            Set(value As Boolean)
                _keptSameUserButDroppedDifferentSecondaryUser = value
            End Set
        End Property

        Public Property wasPreviouslyRated As Boolean 'added 7/12/2013
            Get
                Return _wasPreviouslyRated
            End Get
            Set(value As Boolean)
                _wasPreviouslyRated = value
            End Set
        End Property

        Public Property hasChangeInRatedQuoteXml As Boolean 'added 7/15/2013
            Get
                Return _hasChangeInRatedQuoteXml
            End Get
            Set(value As Boolean)
                _hasChangeInRatedQuoteXml = value
            End Set
        End Property

        Public Sub New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _quoteXmlId = 0
            _quoteInserted = ""
            _initialUserId = 0
            _initialUserName = ""
            _quoteUserId = 0
            _quoteUserName = ""
            _appGapUserId = 0
            _appGapUserName = ""
            _quoteState = ""
            _appGapState = ""
            _xmlStatus = ""
            _ratedQuoteSuccess = False
            _ratedAppGapSuccess = False
            _xmlUpdated = ""

            _currUserId = 0
            _currUserName = ""

            _quoteUpdatedUserId = ""
            _quoteUpdatedUserName = ""
            _quoteStatus = ""
            _quoteUpdated = ""

            _ratedQuoteNumber = ""

            _secondaryUserId = 0
            _secondaryUserName = ""

            _xmlInserted = ""

            _quoteXmlLength = 0
            _ratedQuoteXmlLength = 0
            _appGapXmlLength = 0
            _ratedAppGapXmlLength = 0

            _secondaryUserOriginalQuoteXmlId = 0

            _secondaryQuoteType = SecondaryUserQuoteType.None 'Nothing 'added 7/10/2013 PM

            'added 7/10/2013 PM
            _hasAnyUserChange = False
            _hasLastUpdatedUserChange = False
            _hasSecondaryUserChange = False
            _hasDifferentSecondaryUser = False 'added 7/11/2013
            _keptSameUserButDroppedDifferentSecondaryUser = False 'added 7/11/2013

            _wasPreviouslyRated = False 'added 7/12/2013

            _hasChangeInRatedQuoteXml = False 'added 7/15/2013
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _quoteXmlId <> Nothing Then
                        _quoteXmlId = Nothing
                    End If
                    If _quoteInserted IsNot Nothing Then
                        _quoteInserted = Nothing
                    End If
                    If _initialUserId <> Nothing Then
                        _initialUserId = Nothing
                    End If
                    If _initialUserName IsNot Nothing Then
                        _initialUserName = Nothing
                    End If
                    If _quoteUserId <> Nothing Then
                        _quoteUserId = Nothing
                    End If
                    If _quoteUserName IsNot Nothing Then
                        _quoteUserName = Nothing
                    End If
                    If _appGapUserId <> Nothing Then
                        _appGapUserId = Nothing
                    End If
                    If _appGapUserName IsNot Nothing Then
                        _appGapUserName = Nothing
                    End If
                    If _quoteState IsNot Nothing Then
                        _quoteState = Nothing
                    End If
                    If _appGapState IsNot Nothing Then
                        _appGapState = Nothing
                    End If
                    If _xmlStatus IsNot Nothing Then
                        _xmlStatus = Nothing
                    End If
                    If _ratedQuoteSuccess <> Nothing Then
                        _ratedQuoteSuccess = Nothing
                    End If
                    If _ratedAppGapSuccess <> Nothing Then
                        _ratedAppGapSuccess = Nothing
                    End If
                    If _xmlUpdated IsNot Nothing Then
                        _xmlUpdated = Nothing
                    End If

                    If _currUserId <> Nothing Then
                        _currUserId = Nothing
                    End If
                    If _currUserName IsNot Nothing Then
                        _currUserName = Nothing
                    End If

                    If _quoteUpdatedUserId IsNot Nothing Then
                        _quoteUpdatedUserId = Nothing
                    End If
                    If _quoteUpdatedUserName IsNot Nothing Then
                        _quoteUpdatedUserName = Nothing
                    End If
                    If _quoteStatus IsNot Nothing Then
                        _quoteStatus = Nothing
                    End If
                    If _quoteUpdated IsNot Nothing Then
                        _quoteUpdated = Nothing
                    End If

                    If _ratedQuoteNumber IsNot Nothing Then
                        _ratedQuoteNumber = Nothing
                    End If

                    If _secondaryUserId <> Nothing Then
                        _secondaryUserId = Nothing
                    End If
                    If _secondaryUserName IsNot Nothing Then
                        _secondaryUserName = Nothing
                    End If

                    If _xmlInserted IsNot Nothing Then
                        _xmlInserted = Nothing
                    End If

                    If _quoteXmlLength <> Nothing Then
                        _quoteXmlLength = Nothing
                    End If
                    If _ratedQuoteXmlLength <> Nothing Then
                        _ratedQuoteXmlLength = Nothing
                    End If
                    If _appGapXmlLength <> Nothing Then
                        _appGapXmlLength = Nothing
                    End If
                    If _ratedAppGapXmlLength <> Nothing Then
                        _ratedAppGapXmlLength = Nothing
                    End If

                    If _secondaryUserOriginalQuoteXmlId <> Nothing Then
                        _secondaryUserOriginalQuoteXmlId = Nothing
                    End If

                    If _secondaryQuoteType <> Nothing Then 'added 7/10/2013 PM
                        _secondaryQuoteType = Nothing
                    End If

                    'added 7/10/2013 PM
                    If _hasAnyUserChange <> Nothing Then
                        _hasAnyUserChange = Nothing
                    End If
                    If _hasLastUpdatedUserChange <> Nothing Then
                        _hasLastUpdatedUserChange = Nothing
                    End If
                    If _hasSecondaryUserChange <> Nothing Then
                        _hasSecondaryUserChange = Nothing
                    End If
                    If _hasDifferentSecondaryUser <> Nothing Then 'added 7/11/2013
                        _hasDifferentSecondaryUser = Nothing
                    End If
                    If _keptSameUserButDroppedDifferentSecondaryUser <> Nothing Then 'added 7/11/2013
                        _keptSameUserButDroppedDifferentSecondaryUser = Nothing
                    End If

                    If _wasPreviouslyRated <> Nothing Then 'added 7/12/2013
                        _wasPreviouslyRated = Nothing
                    End If

                    If _hasChangeInRatedQuoteXml <> Nothing Then 'added 7/15/2013
                        _hasChangeInRatedQuoteXml = Nothing
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
