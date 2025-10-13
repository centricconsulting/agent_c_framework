Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store clue property information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteClueProperty 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _FormerSection As QuickQuoteFormerSection
        Private _InquirySection As QuickQuoteInquirySection
        Private _MailingSection As QuickQuoteMailingSection
        Private _MortgageSection As QuickQuoteMortgageSection
        Private _PolicySection As QuickQuotePolicySection
        Private _RiskSection As QuickQuoteRiskSection
        'Private _SubjectSection As QuickQuoteSubjectSection
        Private _PropSubjectIdentifications As List(Of QuickQuotePropSubjectIdentification) 'shown as SubjectSection in xml

        Public Property FormerSection As QuickQuoteFormerSection
            Get
                SetObjectsParent(_FormerSection)
                Return _FormerSection
            End Get
            Set(value As QuickQuoteFormerSection)
                _FormerSection = value
                SetObjectsParent(_FormerSection)
            End Set
        End Property
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
        Public Property MailingSection As QuickQuoteMailingSection
            Get
                SetObjectsParent(_MailingSection)
                Return _MailingSection
            End Get
            Set(value As QuickQuoteMailingSection)
                _MailingSection = value
                SetObjectsParent(_MailingSection)
            End Set
        End Property
        Public Property MortgageSection As QuickQuoteMortgageSection
            Get
                SetObjectsParent(_MortgageSection)
                Return _MortgageSection
            End Get
            Set(value As QuickQuoteMortgageSection)
                _MortgageSection = value
                SetObjectsParent(_MortgageSection)
            End Set
        End Property
        Public Property PolicySection As QuickQuotePolicySection
            Get
                SetObjectsParent(_PolicySection)
                Return _PolicySection
            End Get
            Set(value As QuickQuotePolicySection)
                _PolicySection = value
                SetObjectsParent(_PolicySection)
            End Set
        End Property
        Public Property RiskSection As QuickQuoteRiskSection
            Get
                SetObjectsParent(_RiskSection)
                Return _RiskSection
            End Get
            Set(value As QuickQuoteRiskSection)
                _RiskSection = value
                SetObjectsParent(_RiskSection)
            End Set
        End Property
        'Public Property SubjectSection As QuickQuoteSubjectSection
        '    Get
        '        Return _SubjectSection
        '    End Get
        '    Set(value As QuickQuoteSubjectSection)
        '        _SubjectSection = value
        '    End Set
        'End Property
        Public Property PropSubjectIdentifications As List(Of QuickQuotePropSubjectIdentification)
            Get
                SetParentOfListItems(_PropSubjectIdentifications, "{E4B024D9-4279-41D1-9E02-F84734D7D034}")
                Return _PropSubjectIdentifications
            End Get
            Set(value As List(Of QuickQuotePropSubjectIdentification))
                _PropSubjectIdentifications = value
                SetParentOfListItems(_PropSubjectIdentifications, "{E4B024D9-4279-41D1-9E02-F84734D7D034}")
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _FormerSection = New QuickQuoteFormerSection
            _InquirySection = New QuickQuoteInquirySection
            _MailingSection = New QuickQuoteMailingSection
            _MortgageSection = New QuickQuoteMortgageSection
            _PolicySection = New QuickQuotePolicySection
            _RiskSection = New QuickQuoteRiskSection
            '_SubjectSection = New QuickQuoteSubjectSection
            '_PropSubjectIdentifications = New List(Of QuickQuotePropSubjectIdentification)
            _PropSubjectIdentifications = Nothing 'added 8/4/2014
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
                    If _FormerSection IsNot Nothing Then
                        _FormerSection.Dispose()
                        _FormerSection = Nothing
                    End If
                    If _InquirySection IsNot Nothing Then
                        _InquirySection.Dispose()
                        _InquirySection = Nothing
                    End If
                    If _MailingSection IsNot Nothing Then
                        _MailingSection.Dispose()
                        _MailingSection = Nothing
                    End If
                    If _MortgageSection IsNot Nothing Then
                        _MortgageSection.Dispose()
                        _MortgageSection = Nothing
                    End If
                    If _PolicySection IsNot Nothing Then
                        _PolicySection.Dispose()
                        _PolicySection = Nothing
                    End If
                    If _RiskSection IsNot Nothing Then
                        _RiskSection.Dispose()
                        _RiskSection = Nothing
                    End If
                    'If _SubjectSection IsNot Nothing Then
                    '    _SubjectSection.Dispose()
                    '    _SubjectSection = Nothing
                    'End If
                    If _PropSubjectIdentifications IsNot Nothing Then
                        If _PropSubjectIdentifications.Count > 0 Then
                            For Each p As QuickQuotePropSubjectIdentification In _PropSubjectIdentifications
                                p.Dispose()
                                p = Nothing
                            Next
                            _PropSubjectIdentifications.Clear()
                        End If
                        _PropSubjectIdentifications = Nothing
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
