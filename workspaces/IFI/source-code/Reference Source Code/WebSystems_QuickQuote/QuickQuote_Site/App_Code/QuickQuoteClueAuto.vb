Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store clue auto information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteClueAuto 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _GroupSearchSec As QuickQuoteGroupSearchSec
        Private _InquirySection As QuickQuoteInquirySection
        'Private _SubjectSection As QuickQuoteSubjectSection
        Private _AutoSubjectIdentifications As List(Of QuickQuoteAutoSubjectIdentification) 'shown as SubjectSection in xml
        Private _VehicleSection As QuickQuoteVehicleSection

        Public Property GroupSearchSec As QuickQuoteGroupSearchSec
            Get
                SetObjectsParent(_GroupSearchSec)
                Return _GroupSearchSec
            End Get
            Set(value As QuickQuoteGroupSearchSec)
                _GroupSearchSec = value
                SetObjectsParent(_GroupSearchSec)
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
        'Public Property SubjectSection As QuickQuoteSubjectSection
        '    Get
        '        Return _SubjectSection
        '    End Get
        '    Set(value As QuickQuoteSubjectSection)
        '        _SubjectSection = value
        '    End Set
        'End Property
        Public Property AutoSubjectIdentifications As List(Of QuickQuoteAutoSubjectIdentification)
            Get
                SetParentOfListItems(_AutoSubjectIdentifications, "{E4B024D9-4279-41D1-9E02-F84734D7D033}")
                Return _AutoSubjectIdentifications
            End Get
            Set(value As List(Of QuickQuoteAutoSubjectIdentification))
                _AutoSubjectIdentifications = value
                SetParentOfListItems(_AutoSubjectIdentifications, "{E4B024D9-4279-41D1-9E02-F84734D7D033}")
            End Set
        End Property
        Public Property VehicleSection As QuickQuoteVehicleSection
            Get
                SetObjectsParent(_VehicleSection)
                Return _VehicleSection
            End Get
            Set(value As QuickQuoteVehicleSection)
                _VehicleSection = value
                SetObjectsParent(_VehicleSection)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _GroupSearchSec = New QuickQuoteGroupSearchSec
            _InquirySection = New QuickQuoteInquirySection
            '_SubjectSection = New QuickQuoteSubjectSection
            '_AutoSubjectIdentifications = New List(Of QuickQuoteAutoSubjectIdentification)
            _AutoSubjectIdentifications = Nothing 'added 8/4/2014
            _VehicleSection = New QuickQuoteVehicleSection
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
                    If _GroupSearchSec IsNot Nothing Then
                        _GroupSearchSec.Dispose()
                        _GroupSearchSec = Nothing
                    End If
                    If _InquirySection IsNot Nothing Then
                        _InquirySection.Dispose()
                        _InquirySection = Nothing
                    End If
                    'If _SubjectSection IsNot Nothing Then
                    '    _SubjectSection.Dispose()
                    '    _SubjectSection = Nothing
                    'End If
                    If _AutoSubjectIdentifications IsNot Nothing Then
                        If _AutoSubjectIdentifications.Count > 0 Then
                            For Each a As QuickQuoteAutoSubjectIdentification In _AutoSubjectIdentifications
                                a.Dispose()
                                a = Nothing
                            Next
                            _AutoSubjectIdentifications.Clear()
                        End If
                        _AutoSubjectIdentifications = Nothing
                    End If
                    If _VehicleSection IsNot Nothing Then
                        _VehicleSection.Dispose()
                        _VehicleSection = Nothing
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
