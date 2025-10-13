Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Namespace QuickQuote.CommonObjects.Umbrella
    Public MustInherit Class TrackedPolicyItem
        Inherits LinkedPolicyItem
        Implements IXmlSerializable, IReconcilable

#Region "protected/private"
        Private _PremiumFullterm As String
        Private _EffectiveDate As String
#End Region

        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set
                _EffectiveDate = Value
                _qqHelper.ConvertToShortDate(_EffectiveDate)
            End Set
        End Property

        Public Property AddedDate As String
        Public Property PCAdded_Date As String
        Public Property NumberOfItems As String 'int                
        Public Property Description As String

        Public Property PremiumFullterm As String
            Get
                Return _PremiumFullterm
            End Get
            Set(value As String)
                _PremiumFullterm = _qqHelper.DiamondAmountFormat(value)
            End Set
        End Property
        Public Property TypeId As String

#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of TrackedPolicyItem)
                'nothing to do here ¯\_(ツ)_/¯
                Me.PremiumFullterm = .PremiumFullterm
                Me.EffectiveDate = EffectiveDate
                Me.AddedDate = .AddedDate
                Me.PCAdded_Date = .PCAdded_Date
                Me.Description = Me.Description
                Me.NumberOfItems = .NumberOfItems
                Me.TypeId = .TypeId
            End With
        End Sub

        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)
            Return matched AndAlso (Me.TypeId = src.AsInstance(Of TrackedPolicyItem).TypeId)
        End Function
#End Region
#Region "IXmlSerializable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)

            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Me.AddedDate)
                        Me.AddedDate = reader.ReadElementContentAsString()
                    Case NameOf(Me.PCAdded_Date)
                        Me.PCAdded_Date = reader.ReadElementContentAsString()
                    Case NameOf(Me.NumberOfItems)
                        Me.NumberOfItems = reader.ReadElementContentAsString()
                    Case NameOf(Me.EffectiveDate)
                        If reader.ReadToDescendant("DateTime") Then
                            Me.EffectiveDate = reader.ReadElementContentAsString()
                        End If
                        reader.ReadEndElement()

                    Case NameOf(Me.Description)
                        Me.Description = reader.ReadElementContentAsString()
                    Case $"{Me.GetType().Name}{NameOf(Me.TypeId)}"
                        Me.TypeId = reader.ReadElementContentAsString()
                    Case NameOf(Me.PremiumFullterm)
                        Me.PremiumFullterm = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If

            Return found
        End Function

        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)

            If Not String.IsNullOrWhiteSpace(Me.EffectiveDate) Then
                writer.WriteStartElement(NameOf(Me.EffectiveDate))
                writer.WriteElementString("DateTime", _qqHelper.DiamondDateFormat(Me.EffectiveDate))
                writer.WriteEndElement()
            End If

            'writer.WriteElementString(NameOf(Me.PremiumFullterm), Me.PremiumFullterm)
            writer.WriteElementString(NameOf(Me.AddedDate), _qqHelper.DiamondDateFormat(Me.AddedDate))
            writer.WriteElementString(NameOf(Me.PCAdded_Date), _qqHelper.DiamondDateFormat(Me.PCAdded_Date))
            writer.WriteElementString(NameOf(Me.NumberOfItems), Me.NumberOfItems)
            writer.WriteElementString(NameOf(Me.Description), Me.Description)
            writer.WriteElementString($"{Me.GetType().Name}{NameOf(Me.TypeId)}", Me.TypeId)
        End Sub
        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.AddedDate)
        '                    Me.AddedDate = reader.ReadElementContentAsString()
        '                Case NameOf(Me.PCAdded_Date)
        '                    Me.PCAdded_Date = reader.ReadElementContentAsString()
        '                Case NameOf(Me.NumberOfItems)
        '                    Me.NumberOfItems = reader.ReadElementContentAsString()
        '                Case NameOf(Me.EffectiveDate)
        '                    If reader.ReadToDescendant("DateTime") Then
        '                        Me.EffectiveDate = reader.ReadElementContentAsString()
        '                        .ReadEndElement()
        '                    End If
        '                Case NameOf(Me.Description)
        '                    Me.Description = reader.ReadElementContentAsString()
        '                Case $"{Me.GetType()}{NameOf(Me.TypeId)}"
        '                    Me.TypeId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.PremiumFullterm)
        '                    Me.PremiumFullterm = reader.ReadElementContentAsString()
        '                Case Else

        '            End Select
        '            .ReadEndElement()
        '            .MoveToContent()
        '        End While
        '    End With
        'End Sub

        'Public Overrides Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        '    writer.WriteStartElement(NameOf(Me.EffectiveDate))
        '    writer.WriteElementString("DateTime", Me.EffectiveDate)
        '    writer.WriteEndElement()

        '    writer.WriteElementString(NameOf(Me.PremiumFullterm), Me.PremiumFullterm)
        '    writer.WriteElementString(NameOf(Me.AddedDate), Me.AddedDate)
        '    writer.WriteElementString(NameOf(Me.PCAdded_Date), Me.PCAdded_Date)
        '    writer.WriteElementString(NameOf(Me.NumberOfItems), Me.NumberOfItems)
        '    writer.WriteElementString(NameOf(Me.Description), Me.Description)
        '    writer.WriteElementString($"{Me.GetType()}{NameOf(Me.TypeId)}", Me.TypeId)
        'End Sub
#End Region

    End Class

End Namespace