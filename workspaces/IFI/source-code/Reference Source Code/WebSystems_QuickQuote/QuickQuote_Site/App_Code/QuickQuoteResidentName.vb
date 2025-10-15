Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store resident name information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteLocation object (<see cref="QuickQuoteLocation"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteResidentName 'added 2/26/2015
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Name As QuickQuoteName
        Private _ResidentNum As String 'for reconciliation

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
            End Set
        End Property
        Public Property ResidentNum As String
            Get
                Return _ResidentNum
            End Get
            Set(value As String)
                _ResidentNum = value
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
            _Name = New QuickQuoteName
            _ResidentNum = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidResidentNum() As Boolean 'added for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ResidentNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                'If Me.RelationshipTypeId <> "" Then
                '    Dim rel As String = ""
                '    rel = "RelationshipTypeId: " & Me.RelationshipTypeId
                '    Dim relType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, Me.RelationshipTypeId)
                '    If relType <> "" Then
                '        rel &= " (" & relType & ")"
                '    End If
                '    str = qqHelper.appendText(str, rel, vbCrLf)
                'End If
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
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    qqHelper.DisposeString(_ResidentNum)

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
