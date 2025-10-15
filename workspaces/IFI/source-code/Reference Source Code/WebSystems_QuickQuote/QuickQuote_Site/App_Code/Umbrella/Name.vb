Imports System.Xml
Imports System.Xml.Serialization
Imports Diamond.Common
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Class Name
        Inherits BasePolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Property NameId As String
        Public Property FirstName As String
        Public Property LastName As String
        Public Property SexId As String
        Public Property BirthDate As String
        Public Property DLN As String
        Public Property DLDate As String
        Public Property MaritalStatusId As String
        Public Property DLStateId As String
        Public Property DLCountryId As String
        Public Property NameAddressSourceId As String
        Public Property TypeId As String 'default to 1 for personal names
        Public Property TaxTypeId As String
        Public Property TaxNumber As String

#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, setItemNumbers, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Name)
                Me.NameId = .NameId
                Me.FirstName = .FirstName
                Me.LastName = .LastName
                Me.SexId = .SexId
                Me.BirthDate = .BirthDate
                Me.DLN = .DLN
                Me.DLDate = .DLDate
                Me.MaritalStatusId = .MaritalStatusId
                Me.DLStateId = .DLStateId
                Me.NameAddressSourceId = .NameAddressSourceId
                Me.DLCountryId = .DLCountryId
                Me.TaxTypeId = .TaxTypeId
                Me.TaxNumber = .TaxNumber
            End With
        End Sub

        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Name)
                matched = matched AndAlso
                          _strCompare.Equals(Me.FirstName, .FirstName) AndAlso
                          _strCompare.Equals(Me.LastName, .LastName)
            End With
            Return matched
        End Function
        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As DCO.InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Name
            End If

            With DirectCast(diaItem, DCO.Name)
                .DetailStatusCode = 1
                .TypeId = _qqHelper.IntegerForString(Me.TypeId)

                If Date.TryParse(Me.BirthDate, .BirthDate) = False Then
                    .BirthDate = Nothing
                End If
                If Date.TryParse(Me.DLDate, .DLDate) = False Then
                    .BirthDate = Nothing
                End If
                .DLN = Me.DLN
                If Integer.TryParse(Me.DLStateId, .DLStateId) = False Then
                    .DLStateId = Enums.State.BLANK
                End If
                .FirstName = Me.FirstName.ToUpper()
                .LastName = Me.LastName.ToUpper()
                Integer.TryParse(Me.MaritalStatusId, .MaritalStatusId)
                If Integer.TryParse(Me.NameAddressSourceId, .NameAddressSourceId) = False Then
                    .NameAddressSourceId = Enums.NameAddressSource.Unassigned
                End If
                Integer.TryParse(Me.SexId, .SexId)
                If Integer.TryParse(Me.TaxTypeId, .TaxTypeId) = False Then
                    .TaxTypeId = Enums.TaxType.BLANK
                End If
                .TaxNumber = Me.TaxNumber
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As DCO.InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Name)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.NameNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.NameNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.TypeId AndAlso
                       Me.FirstName.ToUpper() = diaObj.FirstName.ToUpper() AndAlso
                       Me.LastName.ToUpper() = diaObj.LastName.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
#End Region
#Region "IXmlSerializable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True
                Select Case reader.Name
                    'Case NameOf(Me.NameId)
                    '    If reader.ReadToDescendant("InternalValue") Then
                    '        Me.NameId = reader.ReadElementContentAsString()
                    '    End If
                    '    reader.ReadEndElement()
                    Case NameOf(Me.FirstName)
                        Me.FirstName = reader.ReadElementContentAsString()
                    Case NameOf(Me.LastName)
                        Me.LastName = reader.ReadElementContentAsString()
                    Case NameOf(Me.SexId)
                        Me.SexId = reader.ReadElementContentAsString()
                    Case NameOf(Me.BirthDate)
                        If reader.ReadToDescendant("DateTime") Then
                            Me.BirthDate = reader.ReadElementContentAsString()
                        End If
                        reader.ReadEndElement()

                    Case NameOf(Me.DLN)
                        Me.DLN = reader.ReadElementContentAsString()
                    Case NameOf(Me.DLDate)
                        If reader.ReadToDescendant("DateTime") Then
                            Me.DLDate = reader.ReadElementContentAsString()
                        End If
                        reader.ReadEndElement()

                    Case NameOf(Me.MaritalStatusId)
                        Me.MaritalStatusId = reader.ReadElementContentAsString()
                    Case NameOf(Me.DLStateId)
                        Me.DLStateId = reader.ReadElementContentAsString()
                    Case NameOf(Me.DLCountryId)
                        Me.DLCountryId = reader.ReadElementContentAsString()
                    Case NameOf(Me.NameAddressSourceId)
                        Me.NameAddressSourceId = reader.ReadElementContentAsString()
                    Case NameOf(Me.TypeId)
                        Me.TypeId = reader.ReadElementContentAsString()
                    Case NameOf(Me.TaxTypeId)
                        Me.TaxTypeId = reader.ReadElementContentAsString()
                    Case NameOf(Me.TaxNumber)
                        Me.TaxNumber = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function

        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)

            'If Not String.IsNullOrWhiteSpace(Me.NameId) Then
            '    writer.WriteStartElement(NameOf(Me.NameId))
            '    writer.WriteElementString("InternalValue", Me.NameId)
            '    writer.WriteEndElement()
            'End If

            writer.WriteElementString(NameOf(Me.FirstName), Me.FirstName)
            writer.WriteElementString(NameOf(Me.LastName), Me.LastName)
            writer.WriteElementString(NameOf(Me.SexId), Me.SexId)

            If Not String.IsNullOrWhiteSpace(Me.BirthDate) Then
                writer.WriteStartElement(NameOf(Me.BirthDate))
                writer.WriteElementString("DateTime", Me.BirthDate)
                writer.WriteEndElement()
            End If

            writer.WriteElementString(NameOf(Me.DLN), Me.DLN)

            If Not String.IsNullOrWhiteSpace(Me.DLDate) Then
                writer.WriteStartElement(NameOf(Me.DLDate))
                writer.WriteElementString("DateTime", Me.DLDate)
                writer.WriteEndElement()
            End If

            writer.WriteElementString(NameOf(Me.MaritalStatusId), Me.MaritalStatusId)
            writer.WriteElementString(NameOf(Me.DLStateId), Me.DLStateId)
            writer.WriteElementString(NameOf(Me.DLCountryId), Me.DLCountryId)
            writer.WriteElementString(NameOf(Me.NameAddressSourceId), Me.NameAddressSourceId)
            writer.WriteElementString(NameOf(Me.TypeId), Me.TypeId)
            writer.WriteElementString(NameOf(Me.TaxTypeId), Me.TaxTypeId)
            writer.WriteElementString(NameOf(Me.TaxNumber), Me.TaxNumber)
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.NameId)
        '                    If reader.ReadToDescendant("InternalValue") Then
        '                        Me.NameId = reader.ReadElementContentAsString()
        '                        .ReadEndElement()
        '                    End If
        '                Case NameOf(Me.FirstName)
        '                    Me.FirstName = reader.ReadElementContentAsString()
        '                Case NameOf(Me.LastName)
        '                    Me.LastName = reader.ReadElementContentAsString()
        '                Case NameOf(Me.SexId)
        '                    Me.SexId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.BirthDate)
        '                    If reader.ReadToDescendant("DateTime") Then
        '                        Me.BirthDate = reader.ReadElementContentAsString()
        '                        .ReadEndElement()
        '                    End If
        '                Case NameOf(Me.DLN)
        '                    Me.DLN = reader.ReadElementContentAsString()
        '                Case NameOf(Me.DLDate)
        '                    If reader.ReadToDescendant("DateTime") Then
        '                        Me.DLDate = reader.ReadElementContentAsString()
        '                        .ReadEndElement()
        '                    End If
        '                Case NameOf(Me.MaritalStatusId)
        '                    Me.MaritalStatusId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.DLStateId)
        '                    Me.DLStateId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.DLCountryId)
        '                    Me.DLCountryId = reader.ReadElementContentAsString()
        '                Case NameOf(Me.NameAddressSourceId)
        '                    Me.NameAddressSourceId = reader.ReadElementContentAsString()
        '                Case Else

        '            End Select
        '            .ReadEndElement()
        '            .MoveToContent()
        '        End While
        '    End With
        'End Sub

        'Public Overrides Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        '    writer.WriteStartElement(NameOf(Me.NameId))
        '    writer.WriteElementString("InternalValue", Me.NameId)
        '    writer.WriteEndElement()

        '    writer.WriteElementString(NameOf(Me.FirstName), Me.FirstName)
        '    writer.WriteElementString(NameOf(Me.LastName), Me.LastName)
        '    writer.WriteElementString(NameOf(Me.SexId), Me.SexId)

        '    writer.WriteStartElement(NameOf(Me.BirthDate))
        '    writer.WriteElementString("DateTime", Me.BirthDate)
        '    writer.WriteEndElement()

        '    writer.WriteElementString(NameOf(Me.DLN), Me.DLN)

        '    writer.WriteStartElement(NameOf(Me.DLDate))
        '    writer.WriteElementString("DateTime", Me.DLDate)
        '    writer.WriteEndElement()

        '    writer.WriteElementString(NameOf(Me.MaritalStatusId), Me.MaritalStatusId)
        '    writer.WriteElementString(NameOf(Me.DLStateId), Me.DLStateId)
        '    writer.WriteElementString(NameOf(Me.DLCountryId), Me.DLCountryId)
        '    writer.WriteElementString(NameOf(Me.NameAddressSourceId), Me.NameAddressSourceId)
        'End Sub
#End Region
    End Class
End Namespace
