Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Class Location
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Property DwellingTypeId As String
            Get
                Return Me.TypeId
            End Get
            Set
                Me.TypeId = Value
            End Set
        End Property

        Public Property Address As Address
        Public Property NumberOfUnitsId As String

        Shared Sub New()
            ReadXml_ElementsToPreemptivelySkip.Add("LocationTypeId")
        End Sub

        Public Sub New()
            MyBase.New
        End Sub
#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Location)
                Me.Address.InitIfNothing.CopyFrom(.Address)
                Me.NumberOfUnitsId = .NumberOfUnitsId
            End With
        End Sub
        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Location)
                matched = matched AndAlso
                          Me.DwellingTypeId = .DwellingTypeId AndAlso
                          Me.Address.IsMatchFor(.Address)
            End With
            Return matched
        End Function
        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As DCO.InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.Location
            End If

            With DirectCast(diaItem, DCO.Policy.Location)
                .DetailStatusCode = 1
                Me.Address.ConvertToDiamondItem(.Address, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag)
                If Not Integer.TryParse(Me.DwellingTypeId, .DwellingTypeId) Then
                    .DwellingTypeId = -1
                End If
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
                If Not Integer.TryParse(Me.NumberOfUnitsId, .NumberOfUnitsId) Then
                    .NumberOfUnitsId = -1
                End If
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As DCO.InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.Location)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.LocationNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.LocationNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.DwellingTypeId) = diaObj.DwellingTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
#End Region
#Region "IXmlSerialization"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Me.Address)
                        Dim xmlSer As New XmlSerializer(GetType(Address))
                        Me.Address = xmlSer.Deserialize(reader)

                    Case NameOf(Me.DwellingTypeId)
                        Me.DwellingTypeId = reader.ReadElementContentAsString()
                    Case NameOf(Me.NumberOfUnitsId)
                        Me.NumberOfUnitsId = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)
            If Me.Address IsNot Nothing Then
                Dim xmlSer As New XmlSerializer(GetType(Address))
                xmlSer.Serialize(writer, Me.Address)
            End If
            If Not String.IsNullOrWhiteSpace(Me.NumberOfUnitsId) Then
                writer.WriteElementString(NameOf(Me.NumberOfUnitsId), Me.NumberOfUnitsId)
            End If
            writer.WriteElementString(NameOf(Me.DwellingTypeId), Me.DwellingTypeId)
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Address)
        '                    Dim xmlSer As New XmlSerializer(GetType(Address))
        '                    Me.Address = xmlSer.Deserialize(reader.ReadSubtree())

        '                Case NameOf(Me.DwellingTypeId)
        '                    Me.DwellingTypeId = reader.ReadElementContentAsString()
        '                Case Else

        '            End Select
        '            .ReadEndElement()
        '            .MoveToContent()
        '        End While
        '    End With
        'End Sub

        'Public Overrides Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        '    MyBase.WriteXml(writer)
        '    If Me.Address IsNot Nothing Then
        '        Dim xmlSer As New XmlSerializer(GetType(Address))
        '        xmlSer.Serialize(writer, Me.Address)
        '    End If
        '    writer.WriteElementString(NameOf(Me.DwellingTypeId), Me.DwellingTypeId)
        'End Sub
    End Class
#End Region
End Namespace