Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store scheduled text information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteCoverage object (<see cref="QuickQuoteCoverage"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteScheduledText 'added 3/27/2015 for CRM
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _CoverageCodeId As String
        Private _CoverageNum As String
        Private _Description As String
        Private _ScheduledTextNum As String
        Private _UICoverageScheduledCoverageParentTypeId As String 'static data

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property CoverageCodeId As String
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
            End Set
        End Property
        Public Property CoverageNum As String
            Get
                Return _CoverageNum
            End Get
            Set(value As String)
                _CoverageNum = value
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
        Public Property ScheduledTextNum As String
            Get
                Return _ScheduledTextNum
            End Get
            Set(value As String)
                _ScheduledTextNum = value
            End Set
        End Property
        Public Property UICoverageScheduledCoverageParentTypeId As String 'static data
            Get
                Return _UICoverageScheduledCoverageParentTypeId
            End Get
            Set(value As String)
                _UICoverageScheduledCoverageParentTypeId = value
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
            _CoverageCodeId = ""
            _CoverageNum = ""
            _Description = ""
            _ScheduledTextNum = ""
            _UICoverageScheduledCoverageParentTypeId = "" 'static data

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidScheduledTextNum() As Boolean 'added 3/30/2015 for possible reconciliation
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ScheduledTextNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    str = qqHelper.appendText(str, "CoverageCodeId: " & Me.CoverageCodeId, vbCrLf)
                End If
                If Me.Description <> "" Then
                    str = qqHelper.appendText(str, "Description: " & Me.Description, vbCrLf)
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
                    qqHelper.DisposeString(_CoverageCodeId)
                    qqHelper.DisposeString(_CoverageNum)
                    qqHelper.DisposeString(_Description)
                    qqHelper.DisposeString(_ScheduledTextNum)
                    qqHelper.DisposeString(_UICoverageScheduledCoverageParentTypeId) 'static data

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
