Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects.Umbrella
    Public MustInherit Class BasePolicyItem
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IXmlSerializable
        Implements IReconcilable

        Protected _qqHelper As New QuickQuoteHelperClass
        Protected _strCompare As StringComparer = StringComparer.CurrentCultureIgnoreCase

        Public Property PolicyItemNumber As String Implements IReconcilable.ItemNumber
        Public ReadOnly Property HasValidPolicyItemNumber As Boolean Implements IReconcilable.HasValidItemNumber
            Get
                Return _qqHelper.IsValidQuickQuoteIdOrNum(PolicyItemNumber)
            End Get
        End Property
        Public Property PolicyId As String
        Public Property PolicyImageNum As String
        Public Property DetailStatusCode As String

#Region "IXmlSerializable"

        ''' <summary>
        ''' Can be used in Xml deserialization to pre-emptively skip processing for specified elements
        ''' If their normal processing causes an issue. Default skipping functionality is impemented within ReadXml of BasePolicyItem class.
        ''' Derived classes that override/shadow this method must reimplment the pre-emptive skipping functionality.
        ''' Use class constructor in derived classes to add items to this. Comparisons are case sensitive.
        ''' </summary>
        Protected Shared ReadOnly Property ReadXml_ElementsToPreemptivelySkip As New List(Of String)

        Public Overridable Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml

            If reader.IsEmptyElement Then Return

            With reader
                .Read()
                While .NodeType <> XmlNodeType.EndElement AndAlso .NodeType <> XmlNodeType.None
                    If ReadXml_ElementsToPreemptivelySkip.Contains(reader.Name) Then
                        .Skip()
                        Continue While
                    End If

                    'Select Case .NodeType
                    '   Case XmlNodeType.Element
                    If Not .IsEmptyElement Then
                        If ReadXml_NextProperty(reader) Then

                        ElseIf String.IsNullOrWhiteSpace(.Name) = False Then
                            
                            .Skip() 'skip is better than using ReadXYZ methods to skip an undesirable element
                                    'because we don't need the content and it is less error prone
                            '.ReadOuterXml()
                            'UnmatchedElements.Item(.Name) = .ReadOuterXml()                        
                        End If
                    Else
                        .Skip()
                    End If

                End While
            End With
            If reader.NodeType = XmlNodeType.EndElement Then
                reader.ReadEndElement()
            End If
        End Sub

        Protected Overridable Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found As Boolean = True
            Select Case reader.Name
                Case $"{Me.GetType().Name}Num"
                    If reader.ReadToDescendant("InternalValue") Then
                        Me.PolicyItemNumber = reader.ReadElementContentAsString()
                    End If
                    reader.ReadEndElement()

                Case NameOf(Me.DetailStatusCode)
                    Me.DetailStatusCode = reader.ReadElementContentAsString()
                Case NameOf(Me.PolicyImageNum)
                    Me.PolicyImageNum = reader.ReadElementContentAsString()
                Case NameOf(Me.PolicyId)
                    Me.PolicyId = reader.ReadElementContentAsString()
                Case Else
                    found = False
            End Select
            Return found
        End Function
        Protected UnmatchedElements As New Dictionary(Of String, String)
        Public Overridable Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
            'writer.WriteElementString(NameOf(Me.DetailStatusCode), Me.DetailStatusCode)
            'writer.WriteElementString(NameOf(Me.PolicyImageNum), Me.PolicyImageNum)
            'writer.WriteElementString(NameOf(Me.PolicyId), Me.PolicyId)

            'writer.WriteStartElement($"{Me.GetType()}Num")
            'writer.WriteElementString("InternalValue", Me.PolicyItemNumber)
            'writer.WriteEndElement()
            WriteXml_Internal(writer)

            'For Each elementXml In UnmatchedElements.Values
            '    writer.WriteRaw(elementXml)
            'Next
        End Sub
        Protected Overridable Sub WriteXml_Internal(writer As XmlWriter)
            writer.WriteElementString(NameOf(Me.DetailStatusCode), Me.DetailStatusCode)
            ' writer.WriteElementString(NameOf(Me.PolicyImageNum), Me.PolicyImageNum)
            ' writer.WriteElementString(NameOf(Me.PolicyId), Me.PolicyId)

            If Not String.IsNullOrWhiteSpace(Me.PolicyItemNumber) AndAlso
               Not String.IsNullOrWhiteSpace(Me.GetTopLevelQuoteObject()?.QuoteNumber) Then
                writer.WriteStartElement($"{Me.GetType().Name}Num")
                writer.WriteElementString("InternalValue", Me.PolicyItemNumber)
                writer.WriteEndElement()
            End If
        End Sub
        Public Overridable Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

#End Region

#Region "IReconcilable"
        Public Overridable Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            EnsureSameType(src)

            With src.AsInstance(Of BasePolicyItem)
                Me.DetailStatusCode = .DetailStatusCode
                Me.PolicyId = .PolicyId
                Me.PolicyImageNum = .PolicyImageNum
                If setItemNumbers AndAlso Not Me.HasValidPolicyItemNumber Then
                    Me.PolicyItemNumber = .PolicyItemNumber
                End If
            End With
        End Sub

        Protected Overridable Function EnsureSameType(src As IReconcilable, Optional throwException As Boolean = True) As Boolean Implements IReconcilable.EnsureSameType
            'skip redundant calls within the same call stack


            If Me.GetType().Equals(src?.GetType()) = False Then
                If throwException Then
                    Throw New ArgumentException("Cannot copy from object of a different type", NameOf(src))
                Else
                    Return False
                End If
            Else
                Return True
            End If
        End Function

        Public Overridable Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            EnsureSameType(src)

            Dim retval As Boolean = False

            With src.AsInstance(Of BasePolicyItem)

                If canUseItemNumberForPolicyItemReconcilliation AndAlso
                   retval AndAlso
                   Me.HasValidPolicyItemNumber AndAlso
                   .HasValidPolicyItemNumber Then
                    retval = (Me.PolicyItemNumber = .PolicyItemNumber)
                Else
                    retval = (Me.PolicyId = .PolicyId)
                    retval = retval AndAlso (Me.PolicyImageNum = .PolicyImageNum)
                End If
            End With

            Return retval
        End Function

        Public Overridable Function MatchesByItemNumber(src As IReconcilable) As Boolean Implements IReconcilable.MatchesByItemNumber
            EnsureSameType(src)

            Return Me.HasValidPolicyItemNumber AndAlso
                   src.HasValidItemNumber AndAlso
                   Me.PolicyItemNumber = src.ItemNumber
        End Function

        Public MustOverride Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.ConvertToDiamondItem

        Public MustOverride Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchForDiamondItem

        Public Overridable Sub ParseThroughCollectionsAndSetFlags() Implements IReconcilable.ParseThroughCollectionsAndSetFlags
            'nothing to do here, but subclasses may override if needed
        End Sub

#End Region
    End Class
End Namespace