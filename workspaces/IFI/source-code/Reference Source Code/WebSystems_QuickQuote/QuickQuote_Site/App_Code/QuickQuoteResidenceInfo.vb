Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store residence information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteResidenceInfo 'added 8/5/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _CurrentResidenceTypeId As String 'may need matching CurrentResidenceType
        Private _Owned As Boolean
        Private _ResidenceInfoDetails As List(Of QuickQuoteResidenceInfoDetail)
        Private _YearsAtCurrentAddress As String
        'added 8/7/2013 for PPA (originally created for HOM)
        Private _ExpirationDate As String
        Private _HomeDiscount As Boolean
        Private _PolicyNum As String

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
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's CurrentResidenceType table (0=N/A, 1=Owned home or condo, 2=Owned mobile or manufactured home, 3=Rented single family dwelling, 4=Apartment, 5=Unknown)</remarks>
        Public Property CurrentResidenceTypeId As String '0=N/A; 1=Owned home or condo; 2=Owned mobile or manufactured home; 3=Rented single family dwelling; 4=Apartment; 5=Unknown
            Get
                Return _CurrentResidenceTypeId
            End Get
            Set(value As String)
                _CurrentResidenceTypeId = value
            End Set
        End Property
        Public Property Owned As Boolean
            Get
                Return _Owned
            End Get
            Set(value As Boolean)
                _Owned = value
            End Set
        End Property
        Public Property ResidenceInfoDetails As List(Of QuickQuoteResidenceInfoDetail)
            Get
                SetParentOfListItems(_ResidenceInfoDetails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05601}")
                Return _ResidenceInfoDetails
            End Get
            Set(value As List(Of QuickQuoteResidenceInfoDetail))
                _ResidenceInfoDetails = value
                SetParentOfListItems(_ResidenceInfoDetails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05601}")
            End Set
        End Property
        Public Property YearsAtCurrentAddress As String
            Get
                Return _YearsAtCurrentAddress
            End Get
            Set(value As String)
                _YearsAtCurrentAddress = value
            End Set
        End Property
        Public Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
            Set(value As String)
                _ExpirationDate = value
                qqHelper.ConvertToShortDate(_ExpirationDate)
            End Set
        End Property
        Public Property HomeDiscount As Boolean
            Get
                Return _HomeDiscount
            End Get
            Set(value As Boolean)
                _HomeDiscount = value
            End Set
        End Property
        Public Property PolicyNum As String
            Get
                Return _PolicyNum
            End Get
            Set(value As String)
                _PolicyNum = value
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
            _CurrentResidenceTypeId = ""
            _Owned = False
            '_ResidenceInfoDetails = New List(Of QuickQuoteResidenceInfoDetail)
            _ResidenceInfoDetails = Nothing 'added 8/4/2014
            _YearsAtCurrentAddress = ""
            _ExpirationDate = ""
            _HomeDiscount = False
            _PolicyNum = ""

            _DetailStatusCode = "" 'added 5/15/2019

        End Sub

        Public Function HasData() As Boolean
            Dim hasIt As Boolean = False

            If qqHelper.IsPositiveIntegerString(_CurrentResidenceTypeId) = True OrElse _Owned = True OrElse (_ResidenceInfoDetails IsNot Nothing AndAlso _ResidenceInfoDetails.Count > 0) OrElse qqHelper.IsPositiveIntegerString(_YearsAtCurrentAddress) = True OrElse qqHelper.IsValidDateString(_ExpirationDate, mustBeGreaterThanDefaultDate:=True) = True OrElse _HomeDiscount = True Then
                hasIt = True
            End If

            Return hasIt
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
                    If _CurrentResidenceTypeId IsNot Nothing Then
                        _CurrentResidenceTypeId = Nothing
                    End If
                    If _Owned <> Nothing Then
                        _Owned = Nothing
                    End If
                    If _ResidenceInfoDetails IsNot Nothing Then
                        If _ResidenceInfoDetails.Count > 0 Then
                            For Each d As QuickQuoteResidenceInfoDetail In _ResidenceInfoDetails
                                d.Dispose()
                                d = Nothing
                            Next
                            _ResidenceInfoDetails.Clear()
                        End If
                        _ResidenceInfoDetails = Nothing
                    End If
                    If _YearsAtCurrentAddress IsNot Nothing Then
                        _YearsAtCurrentAddress = Nothing
                    End If
                    If _ExpirationDate IsNot Nothing Then
                        _ExpirationDate = Nothing
                    End If
                    If _HomeDiscount <> Nothing Then
                        _HomeDiscount = Nothing
                    End If
                    If _PolicyNum IsNot Nothing Then
                        _PolicyNum = Nothing
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
