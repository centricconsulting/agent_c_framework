Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store acreage information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteLocation object (<see cref="QuickQuoteLocation"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteAcreage 'added 2/25/2015 for Farm
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Acreage As String
        Private _AcreageNum As String 'for reconciliation
        Private _County As String
        Private _Description As String
        Private _LocationAcreageTypeId As String 'static data
        Private _Range As String
        Private _Section As String
        Private _StateId As String 'static data; may default to 16 (IN)
        Private _TownshipCodeTypeId As String 'static data
        Private _Twp As String

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property Acreage As String
            Get
                Return _Acreage
            End Get
            Set(value As String)
                _Acreage = value
            End Set
        End Property
        Public Property AcreageNum As String 'for reconciliation
            Get
                Return _AcreageNum
            End Get
            Set(value As String)
                _AcreageNum = value
            End Set
        End Property
        Public Property County As String
            Get
                Return _County
            End Get
            Set(value As String)
                _County = value
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
        Public Property LocationAcreageTypeId As String 'static data
            Get
                Return _LocationAcreageTypeId
            End Get
            Set(value As String)
                _LocationAcreageTypeId = value
            End Set
        End Property
        Public Property Range As String
            Get
                Return _Range
            End Get
            Set(value As String)
                _Range = value
            End Set
        End Property
        Public Property Section As String
            Get
                Return _Section
            End Get
            Set(value As String)
                _Section = value
            End Set
        End Property
        Public Property StateId As String 'static data; may default to 16 (IN)
            Get
                Return _StateId
            End Get
            Set(value As String)
                _StateId = value
            End Set
        End Property
        Public Property TownshipCodeTypeId As String 'static data
            Get
                Return _TownshipCodeTypeId
            End Get
            Set(value As String)
                _TownshipCodeTypeId = value
            End Set
        End Property
        Public Property Twp As String
            Get
                Return _Twp
            End Get
            Set(value As String)
                _Twp = value
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
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Acreage = ""
            _AcreageNum = "" 'for reconciliation
            _County = ""
            _Description = ""
            _LocationAcreageTypeId = "" 'static data
            _Range = ""
            _Section = ""
            _StateId = "" 'static data; may default to 16 (IN)
            _TownshipCodeTypeId = "" 'static data
            _Twp = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidAcreageNum() As Boolean 'added for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_AcreageNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.LocationAcreageTypeId <> "" Then
                    Dim a As String = ""
                    a = "LocationAcreageTypeId: " & Me.LocationAcreageTypeId
                    Dim aType As String = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAcreage, QuickQuoteHelperClass.QuickQuotePropertyName.LocationAcreageTypeId, Me.LocationAcreageTypeId)
                    If aType <> "" Then
                        a &= " (" & aType & ")"
                    End If
                    str = qqHelper.appendText(str, a, vbCrLf)
                End If
                If Me.Description <> "" Then
                    str = qqHelper.appendText(str, "Description: " & Me.Description, vbCrLf)
                End If
                If Me.Acreage <> "" Then
                    str = qqHelper.appendText(str, "Acreage: " & Me.Acreage, vbCrLf)
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
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_Acreage)
                    qqHelper.DisposeString(_AcreageNum) 'for reconciliation
                    qqHelper.DisposeString(_County)
                    qqHelper.DisposeString(_Description)
                    qqHelper.DisposeString(_LocationAcreageTypeId) 'static data
                    qqHelper.DisposeString(_Range)
                    qqHelper.DisposeString(_Section)
                    qqHelper.DisposeString(_StateId) 'static data; may default to 16 (IN)
                    qqHelper.DisposeString(_TownshipCodeTypeId) 'static data
                    qqHelper.DisposeString(_Twp)

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    MyBase.Dispose()
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
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
