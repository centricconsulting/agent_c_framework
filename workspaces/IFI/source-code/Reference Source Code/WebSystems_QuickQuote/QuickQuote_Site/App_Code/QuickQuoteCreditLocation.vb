Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store credit location information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteCreditLocation 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _InquirySection As QuickQuoteInquirySection
        Private _SubjectSection As QuickQuoteSubjectSection

        Public Property InquirySection As QuickQuoteInquirySection
            Get
                SetObjectsParent(_InquirySection)
                Return _InquirySection
            End Get
            Set(value As QuickQuoteInquirySection)
                _InquirySection = value
                SetObjectsParent(_InquirySection)
            End Set
        End Property
        Public Property SubjectSection As QuickQuoteSubjectSection
            Get
                SetObjectsParent(_SubjectSection)
                Return _SubjectSection
            End Get
            Set(value As QuickQuoteSubjectSection)
                _SubjectSection = value
                SetObjectsParent(_SubjectSection)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _InquirySection = New QuickQuoteInquirySection
            _SubjectSection = New QuickQuoteSubjectSection
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
                    If _InquirySection IsNot Nothing Then
                        _InquirySection.Dispose()
                        _InquirySection = Nothing
                    End If
                    If _SubjectSection IsNot Nothing Then
                        _SubjectSection.Dispose()
                        _SubjectSection = Nothing
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
