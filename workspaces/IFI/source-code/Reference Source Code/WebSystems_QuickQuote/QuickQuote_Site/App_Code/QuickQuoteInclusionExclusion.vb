Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store inclusion/exclusion information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteInclusionExclusion
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 5/16/2019

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Description As String
        Private _CategoryTypeId As String
        Private _CoverageAppliesToTypeId As String
        Private _CoverageTypeId As String
        Private _LiabilityCoverageTypeId As String
        Private _InclusionExclusionNum As String
        Private _ParentTypeId As String
        Private _ProgramTypeId As String
        Private _TypeId As String
        Private _ScheduledItems As Generic.List(Of QuickQuoteInclusionExclusionScheduledItem)

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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property CategoryTypeId As String
            Get
                Return _CategoryTypeId
            End Get
            Set(value As String)
                _CategoryTypeId = value
            End Set
        End Property
        Public Property CoverageAppliesToTypeId As String
            Get
                Return _CoverageAppliesToTypeId
            End Get
            Set(value As String)
                _CoverageAppliesToTypeId = value
            End Set
        End Property
        Public Property CoverageTypeId As String
            Get
                Return _CoverageTypeId
            End Get
            Set(value As String)
                _CoverageTypeId = value
            End Set
        End Property
        Public Property LiabilityCoverageTypeId As String
            Get
                Return _LiabilityCoverageTypeId
            End Get
            Set(value As String)
                _LiabilityCoverageTypeId = value
            End Set
        End Property
        Public Property InclusionExclusionNum As String
            Get
                Return _InclusionExclusionNum
            End Get
            Set(value As String)
                _InclusionExclusionNum = value
            End Set
        End Property
        Public Property ParentTypeId As String
            Get
                Return _ParentTypeId
            End Get
            Set(value As String)
                _ParentTypeId = value
            End Set
        End Property
        Public Property ProgramTypeId As String
            Get
                Return _ProgramTypeId
            End Get
            Set(value As String)
                _ProgramTypeId = value
            End Set
        End Property
        Public Property TypeId As String
            Get
                Return _TypeId
            End Get
            Set(value As String)
                _TypeId = value
            End Set
        End Property
        Public Property ScheduledItems As Generic.List(Of QuickQuoteInclusionExclusionScheduledItem)
            Get
                SetParentOfListItems(_ScheduledItems, "{663B7C7B-F2AC-4BF6-965A-D30F41A05302}")
                Return _ScheduledItems
            End Get
            Set(value As Generic.List(Of QuickQuoteInclusionExclusionScheduledItem))
                _ScheduledItems = value
                SetParentOfListItems(_ScheduledItems, "{663B7C7B-F2AC-4BF6-965A-D30F41A05302}")
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
            _Description = ""
            _CategoryTypeId = ""
            _CoverageAppliesToTypeId = ""
            _CoverageTypeId = ""
            _LiabilityCoverageTypeId = ""
            _InclusionExclusionNum = ""
            _ParentTypeId = ""
            _ProgramTypeId = ""
            _TypeId = ""
            '_ScheduledItems = New Generic.List(Of QuickQuoteInclusionExclusionScheduledItem)
            _ScheduledItems = Nothing 'added 8/4/2014

            _DetailStatusCode = "" 'added 5/15/2019
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _CategoryTypeId IsNot Nothing Then
                        _CategoryTypeId = Nothing
                    End If
                    If _CoverageAppliesToTypeId IsNot Nothing Then
                        _CoverageAppliesToTypeId = Nothing
                    End If
                    If _CoverageTypeId IsNot Nothing Then
                        _CoverageTypeId = Nothing
                    End If
                    If _LiabilityCoverageTypeId IsNot Nothing Then
                        _LiabilityCoverageTypeId = Nothing
                    End If
                    If _InclusionExclusionNum IsNot Nothing Then
                        _InclusionExclusionNum = Nothing
                    End If
                    If _ParentTypeId IsNot Nothing Then
                        _ParentTypeId = Nothing
                    End If
                    If _ProgramTypeId IsNot Nothing Then
                        _ProgramTypeId = Nothing
                    End If
                    If _TypeId IsNot Nothing Then
                        _TypeId = Nothing
                    End If
                    If _ScheduledItems IsNot Nothing Then
                        If _ScheduledItems.Count > 0 Then
                            For Each si As QuickQuoteInclusionExclusionScheduledItem In _ScheduledItems
                                si.Dispose()
                                si = Nothing
                            Next
                            _ScheduledItems.Clear()
                        End If
                        _ScheduledItems = Nothing
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
