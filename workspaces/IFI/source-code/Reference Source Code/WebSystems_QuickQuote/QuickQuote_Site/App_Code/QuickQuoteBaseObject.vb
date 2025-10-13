Imports System.Web
Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' base object to store info common to most object
    ''' </summary>
    ''' <remarks>most objects will inherit from this object</remarks>
    <Serializable()>
    Public Class QuickQuoteBaseObject 'added 7/31/2014
        Implements IDisposable

        Private _UniqueIdentifier As String
        Private _IsClone As Boolean 'added 10/21/2021

        Public ReadOnly Property UniqueIdentifier As String
            Get
                Return _UniqueIdentifier
            End Get
        End Property
        Public ReadOnly Property IsClone As Boolean 'added 10/21/2021
            Get
                Return _IsClone
            End Get
        End Property

        Public Sub New()
            SetBaseObjectDefaults()
        End Sub

        Private _dictionaryOfListCountersToTrackParent As Dictionary(Of String, Integer)

        Protected Friend Sub SetParentOfListItems(Of A)(ByVal thisList As List(Of A), ByVal myGuid As String)
            If GetType(A).BaseType.Name Like "QuickQuoteBaseGenericObject*" Then
                Dim counter As Integer = GetCounterFromDictionary(myGuid)
                If thisList IsNot Nothing AndAlso thisList.Count > 0 Then
                    If thisList.Count >= counter Then
                        If String.IsNullOrWhiteSpace(Me.UniqueIdentifier) = True Then
                            Me.GenerateNewUniqueIdentifierGuid() 'Assuming this should never actually fire... Just incase...
                        End If
                        For Each item As Object In thisList
                            If item IsNot Nothing AndAlso (item.Parent Is Nothing OrElse (item.Parent IsNot Nothing AndAlso item.Parent.UniqueIdentifier <> Me.UniqueIdentifier)) Then
                                item.SetParent = Me
                            End If
                        Next
                    End If
                    counter = thisList.Count
                Else
                    counter = 0
                End If
                _dictionaryOfListCountersToTrackParent(myGuid) = counter
            End If
        End Sub

        Private Function GetCounterFromDictionary(ByVal myGuid As String) As Integer
            If _dictionaryOfListCountersToTrackParent Is Nothing Then
                _dictionaryOfListCountersToTrackParent = New Dictionary(Of String, Integer)
                _dictionaryOfListCountersToTrackParent.Add(myGuid, 0)
            Else
                If _dictionaryOfListCountersToTrackParent.Count = 0 OrElse _dictionaryOfListCountersToTrackParent.ContainsKey(myGuid) = False Then
                    _dictionaryOfListCountersToTrackParent.Add(myGuid, 0)
                End If
            End If

            Return _dictionaryOfListCountersToTrackParent(myGuid)
        End Function

        Private Sub SetBaseObjectDefaults() '8/4/2014 note: needs to have separate method name than what's used on child old constructors
            '_UniqueIdentifier = ""
            '_UniqueIdentifier = Guid.NewGuid.ToString
            'updated 8/5/2014 to use new method
            GenerateNewUniqueIdentifierGuid()
            _IsClone = False 'added 10/21/2021
        End Sub
        Public Sub GenerateNewUniqueIdentifierGuid() 'added 8/5/2014 so it can be called whenever
            _UniqueIdentifier = Guid.NewGuid.ToString
        End Sub

        Protected Friend Sub SetObjectsParent(myObject As Object)
            If myObject IsNot Nothing Then '8/3/2018 note: should possibly check base type to make sure it's QuickQuoteBaseGenericObject, but may require changing signature to (Of A)
                Dim myEnumTest As IEnumerable = TryCast(myObject, IEnumerable)
                If myEnumTest Is Nothing Then
                    myObject.SetParent = Me
                Else
                    'uh oh!
                    Dim a As String = ""
                End If
            End If
        End Sub
        Protected Friend Sub Set_IsClone(ByVal isIt As Boolean) 'added 10/21/2021
            _IsClone = isIt
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _UniqueIdentifier IsNot Nothing Then
                        _UniqueIdentifier = Nothing
                    End If
                    _IsClone = Nothing 'added 10/21/2021
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
        'updated 8/4/2014 for QuickQuoteBaseObject inheritance
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    <Serializable>
    Public Class QuickQuoteBaseGenericObject(Of T)
        Inherits QuickQuoteBaseObject

        <Script.Serialization.ScriptIgnore>
        <NonSerialized()>
        Private _parent As T = Nothing

        <Script.Serialization.ScriptIgnore>
        <NonSerialized()>
        Private _quoteObject As QuickQuoteObject = Nothing
        'Private _parentsUniqueIdentifier As String = ""

        Protected Event ConnectedToQuoteObject()

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Private ReadOnly Property LinkedToQuoteObject As Boolean
            Get
                If _quoteObject Is Nothing Then
                    If GetQuoteObject() IsNot Nothing Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return True
                End If
            End Get
        End Property

        Private Function ReturnParent(myObject As Object) As Object
            Dim returnVar As Object
            Try
                If (myObject.GetType).BaseType.Name Like "QuickQuoteBaseGenericObject*" Then
                    returnVar = myObject.Parent
                    'returnVar = CallByName(myObject, "Parent", CallType.Get)
                Else
                    returnVar = Nothing
                End If
            Catch ex As Exception
                returnVar = Nothing
            End Try

            Return returnVar
        End Function

        'Public Property ParentsUniqueIdentifier As String
        '    Get
        '        Return _parentsUniqueIdentifier
        '    End Get
        '    Set(value As String)
        '        _parentsUniqueIdentifier = value
        '    End Set
        'End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property Parent As T
            Get
                Return _parent
            End Get
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public WriteOnly Property SetParent As T
            Set(value As T)
                If value IsNot Nothing Then
                    If _parent Is Nothing OrElse (_parent IsNot Nothing AndAlso DirectCast(_parent, Object).UniqueIdentifier <> DirectCast(value, Object).UniqueIdentifier) Then
                        _parent = value
                        _quoteObject = Nothing
                        If LinkedToQuoteObject = True Then
                            RaiseEvent ConnectedToQuoteObject()
                        End If
                    End If
                Else
                    _parent = Nothing
                    _quoteObject = Nothing
                End If
            End Set
        End Property

        Public Function GetQuoteObject() As QuickQuoteObject
            If _quoteObject IsNot Nothing Then
                Return _quoteObject
            Else
                'Dim returnVar As Object = Nothing 'removed 8/3/2018
                Dim thisParent As Object = Nothing
                Dim endLoop As Boolean = False

                If Parent IsNot Nothing Then
                    thisParent = Parent
                    Do While endLoop = False
                        If thisParent IsNot Nothing Then
                            If thisParent.GetType = GetType(QuickQuoteObject) Then
                                endLoop = True
                                _quoteObject = thisParent
                                Exit Do 'added 8/9/2018
                            Else
                                thisParent = ReturnParent(thisParent)
                            End If
                        Else
                            endLoop = True
                            Exit Do 'added 8/3/2018
                        End If
                    Loop
                End If

                'returnVar = thisParent 'removed 8/3/2018

                'Return returnVar
                'updated 8/3/2018
                Return _quoteObject
            End If
        End Function
        '8/3/2018 - created new method to get top level QQO
        Public Function GetTopLevelParentQuoteObject() As QuickQuoteObject
            Dim topParentQQO As QuickQuoteObject = Nothing

            topParentQQO = GetQuoteObject()
            If topParentQQO IsNot Nothing Then
                Dim endLoop As Boolean = False
                Dim parentQQO As QuickQuoteObject = Nothing
                Do While endLoop = False
                    parentQQO = topParentQQO.GetQuoteObject()
                    If parentQQO IsNot Nothing Then
                        topParentQQO = parentQQO
                    Else
                        endLoop = True
                        Exit Do
                    End If
                Loop
            End If

            Return topParentQQO
        End Function
        Public Function GetTopLevelQuoteObject() As QuickQuoteObject
            Dim topQQO As QuickQuoteObject = GetTopLevelParentQuoteObject()

            If topQQO Is Nothing AndAlso Me.GetType() = GetType(QuickQuoteObject) Then
                topQQO = CType(CType(Me, Object), QuickQuoteObject)
            End If

            Return topQQO
        End Function
    End Class
End Namespace
