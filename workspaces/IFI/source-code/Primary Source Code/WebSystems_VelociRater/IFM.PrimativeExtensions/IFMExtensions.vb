Imports System.Runtime.CompilerServices
Imports System.Text
Imports IFM.Common.InputValidation

Public Module IFMExtensions

    '******************************************************************************************************************************************************************************
    '******************************************************************************************************************************************************************************
    ' DO NOT ADD ANY ADDITIONAL REFERENCES TO THIS LIBRARY - WE WANT IT TO BE PORTABLE 
    ' IN ORDER FOR IT TO BE PORTABLE DONT BIND IT TO THINGS LIKE DIAMOND EITHER DIRECTLY OR INDIRECTLY BY ADDING REFERENCE TO SOMETHING THAT HAS DIAMOND LIBS AS A DEPENDENCY
    '******************************************************************************************************************************************************************************
    '******************************************************************************************************************************************************************************

#Region "Object"
    '**********************
    ' Use extreme caution in adding extension methods to Object as it will be on absolutely everything that imports the Extensions namespace
    '**********************

    <Extension()>
    Public Function IsNull(obj As Object) As Boolean
        Return obj Is Nothing
    End Function

    <Extension()>
    Public Function IsNotNull(obj As Object) As Boolean
        Return obj IsNot Nothing
    End Function

    ''' <summary>
    ''' Creates the object if it is NULL. And returns the object either way so you can chain it together if you want.
    ''' *** Only use with Types that have a parameterless constructor. ***
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function CreateIfNull(Of T)(ByRef obj As T) As T
        If obj Is Nothing Then
            obj = Activator.CreateInstance(GetType(T))
        End If
        Return obj
    End Function

#End Region

#Region "Numeric"
    ''' <summary>
    ''' System.Int32 extension to test value is greater than zero
    ''' </summary>
    ''' <param name="val"></param>
    ''' <returns>System.Boolean TRUE if value is grater than zero otherwise FALSE</returns>
    ''' <remarks>System.Int32 is assumed to have a value if it is greater than zero</remarks>
    <Extension()>
    Public Function HasValue(val As Integer) As Boolean
        Return (val > 0)
    End Function

    ''' <summary>
    ''' System.Int64 extension to test value is greater than zero
    ''' </summary>
    ''' <param name="val"></param>
    ''' <returns>System.Boolean TRUE if value is grater than zero otherwise FALSE</returns>
    ''' <remarks>System.Int64 is assumed to have a value if it is greater than zero</remarks>
    <Extension()>
    Public Function HasValue(val As Long) As Boolean
        Return (val > 0)
    End Function

    ''' <summary>
    ''' Increments the Int by one returning and returns the incremented Int.
    ''' </summary>
    ''' <param name="aint"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function IncrementByOne(ByRef aInt As Int32) As Int32
        aInt += 1
        Return aInt
    End Function

    <Extension()>
    Public Function TryToFormatAsCurreny(num As Double, Optional showCents As Boolean = True) As String
        Return InputHelpers.TryToFormatAsCurrency(num.ToString(), showCents)
    End Function
#End Region

#Region "DateTime"
    ''' <summary>
    ''' System.DateTime extension to test value is greater than Minimum DateTeim value and less than Maximum DateTime value
    ''' </summary>
    ''' <param name="val"></param>
    ''' <returns>System.Boolean TRUE if value is greater than min. and less than max.</returns>
    <Extension()>
    Public Function HasValue(val As DateTime) As Boolean
        Return (val > DateTime.MinValue AndAlso val < DateTime.MaxValue)
    End Function

#End Region

#Region "String"
    ''' <summary>
    ''' System.String extension method to test string is NOT empty, null, or white space
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns>System.Boolean TRUE if string has a value otherwise FALSE</returns>
    <Extension()>
    Public Function HasValue(str As String) As Boolean
        Return (Not [String].IsNullOrWhiteSpace(str) AndAlso Not [String].IsNullOrEmpty(str))
    End Function

    ''' <summary>
    ''' System.String extension method to test string value is a valid System.Int32 value
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns>System.Boolean TRUE is string is a valid Int32 otherwise FALSE</returns>
    <Extension()>
    Public Function HasInt32Value(str As String) As Boolean
        Dim hasValue As Boolean = False
        Dim val As Integer = -1

        If str.HasValue() AndAlso Int32.TryParse(str, val) AndAlso val.HasValue() Then
            hasValue = True
        End If
        Return hasValue
    End Function

    ''' <summary>
    ''' System.String extension method to try and convert a string to an System.Int32
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns>System.Int32 of converted string value or -1 if not able to convert</returns>
    <Extension()>
    Public Function Int32Value(str As String) As Integer
        Dim val As Integer
        If str.HasValue() AndAlso Int32.TryParse(str, val) Then
            Return val
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' System.String extension method to convert a string to title case (every word capitalized)
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns>System.String value in title case</returns>
    <Extension()>
    Public Function Capitalize(str As String) As String
        Return If(str.IsNotNull(), Converter.CapitalizeString(str), str)
    End Function

    '<Extension()>
    'Public Function ToStringArray(str As String) As String()
    '    Return New String() {str}
    'End Function

    <Extension()>
    Public Function Ellipsis(txt As String, maxChars As Int32) As String
        Return InputHelpers.EllipsisText(txt, maxChars)
    End Function

    ''' <summary>
    ''' Tries to parse the text to get a double. It will remove most common symbols like $ or #. Returns zero if it fails.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function TryToGetDouble(txt As String) As Double
        Return InputHelpers.TryToGetDouble(txt)
    End Function

    ''' <summary>
    ''' Tries to parse the text to get a int32. It will remove most common symbols like $ or #. Returns zero if it fails.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function TryToGetInt32(txt As String) As Int32
        Return CInt(InputHelpers.TryToGetDouble(txt))
    End Function

    ''' <summary>
    ''' Tries to parse the text as a double then formats the text. If it fails it will return $0.00 or $0 depending on provided parms.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="showCents"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function TryToFormatAsCurreny(txt As String, Optional showCents As Boolean = True) As String
        Return InputHelpers.TryToFormatAsCurrency(txt, showCents)
    End Function

    ''' <summary>
    ''' Creates a List(of String) from a CSV string.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function CSVtoList(txt As String) As List(Of String)
        Return InputHelpers.CSVtoList(txt)
    End Function

    ''' <summary>
    ''' Wrapper for Microsoft.VisualBasic.Information.IsNumeric()
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsNumeric(txt As String) As Boolean
        Return txt.IsNullEmptyorWhitespace() = False AndAlso Microsoft.VisualBasic.Information.IsNumeric(txt)
    End Function

    ''' <summary>
    ''' Returns empty string is sent string is NUll.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToUpper_NullSafe(txt As String) As String
        Return If(txt.IsNotNull(), txt.ToUpper(), "") ' Matt A - was sending null back if it was null most of the time you want empty string not null 11/14/2016
    End Function

    ''' <summary>
    ''' Returns empty string is sent string is NUll.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToLower_NullSafe(txt As String) As String
        Return If(txt.IsNotNull(), txt.ToLower(), txt)
    End Function

    ''' <summary>
    ''' Returns empty string is sent string is NUll. Returns sent text unchanged if either parameter are NULL.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="find"></param>
    ''' <param name="replaceWith"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function Replace_NullSafe(txt As String, find As String, replaceWith As String) As String
        If (txt.IsNullEmptyorWhitespace()) Then
            Return ""
        Else
            If find.IsNull() = False AndAlso replaceWith.IsNull() = False Then
                Return txt.Replace(find, replaceWith)
            End If
            Return txt
        End If

    End Function


    ''' <summary>
    ''' Returns TRUE is any parms are NULL, EMPTY, or Whitespace
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsNullEmptyorWhitespace(str As String, ParamArray args() As Object) As Boolean
        Dim list As New List(Of String)
        list.Add(str)
        For Each a In args
            list.Add(a)
        Next

        For Each a In list
            If String.IsNullOrEmpty(a) OrElse String.IsNullOrEmpty(a.ToString().Trim()) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Returns TRUE is none of the parms are NULL, EMPTY, or Whitespace
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function NoneAreNullEmptyorWhitespace(str As String, ParamArray args() As Object) As Boolean

        Dim list As New List(Of String)
        list.Add(str)
        For Each a In args
            list.Add(a)
        Next
        For Each a In list
            If String.IsNullOrWhiteSpace(a) Then
                Return False
            End If
        Next
        Return True
    End Function



    ''' <summary>
    ''' Returns True if any equate to anything other than Null/Empty/Whitespace.
    ''' </summary>
    ''' <param name="str"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function OneorMoreIsNotEmpty(str As String, ParamArray args() As Object) As Boolean
        Dim list As New List(Of String)
        list.Add(str)
        For Each a In args
            list.Add(a)
        Next

        For Each a In list
            If String.IsNullOrWhiteSpace(a) = False Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Wrapper for Microsoft.VisualBasic.Information.IsDate()
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsDate(str As String) As Boolean
        Return str.IsNullEmptyorWhitespace() = False AndAlso Microsoft.VisualBasic.Information.IsDate(str)
    End Function

    <Extension()>
    Public Function ReturnEmptyIfDefaultDiamondDate(txt As String) As String
        If txt.IsDate Then
            If (Convert.ToDateTime(txt).ToShortDateString() = Convert.ToDateTime(InputHelpers.DiamondDefaultDate_string).ToShortDateString()) Then
                Return ""
            End If
        End If
        Return txt
    End Function

    ''' <summary>
    ''' Converts a string to a DateTime. If string is invalid you will get an Exception.
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToDateTime(str As String) As DateTime
        Return DateTime.Parse(str)
    End Function

    ''' <summary>
    ''' A wrapper for String.Format()
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function FormatIFM(txt As String, ParamArray args() As Object) As String
        Return String.Format(txt, args)
    End Function

    ''' <summary>
    ''' Return empty if the string equals any of the provided parms otherwise it returns the sent string.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ReturnEmptyIfEqualsAny(txt As String, ParamArray args() As Object) As String
        For Each a In args
            If txt = a.ToString() Then
                Return ""
            End If
        Next
        Return txt
    End Function

    ''' <summary>
    ''' Returns true if the string after being cast to double is zero or less.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ReturnEmptyIfLessThanOrEqualToZero(txt As String) As String
        Return If(txt.TryToGetDouble() <= 0, "", txt)
    End Function

    ''' <summary>
    ''' Returns True if the provided string matches any of the parms. You can use [String.Empty] or "" to test for 'IsNullOrWhiteSpace()'.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function EqualsAny(txt As String, ParamArray args() As String) As Boolean
        For Each a In args
            If String.IsNullOrWhiteSpace(a) Then
                If String.IsNullOrWhiteSpace(txt) Then
                    Return True
                End If
            Else
                If a IsNot Nothing AndAlso a.ToString() = txt Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    <Extension()>
    Public Function EqualsAny(num As Int32, ParamArray args() As Object) As Boolean
        For Each a In args
            If a IsNot Nothing Then
                Dim tInt As Int32 = 0
                If Int32.TryParse(a.ToString(), tInt) Then
                    If num = tInt Then
                        Return True
                    End If
                End If
            End If
        Next
        Return False
    End Function


    ''' <summary>
    ''' Returns True if the provided string does not match any of the parms. You can use [String.Empty] or "" to test for 'IsNullOrWhiteSpace()'.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function NotEqualsAny(txt As String, ParamArray args() As String) As Boolean
        Return Not EqualsAny(txt, args)
    End Function

    <Extension()>
    Public Function NotEqualsAny(num As Int32, ParamArray args() As Object) As Boolean
        Return Not EqualsAny(num, args)
    End Function


    ''' <summary>
    ''' Removes all instances of the provided string(s)
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveAny(txt As String, ParamArray args() As Object) As String
        For Each a In args
            If a IsNot Nothing Then
                txt = txt.Replace(a.ToString(), "")
            End If
        Next
        Return txt
    End Function

    ''' <summary>
    ''' Ensures that a string is equal to or less than the maxLength
    ''' </summary>
    ''' <param name="str"></param>
    ''' <param name="maxlength"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToMaxLength(str As String, maxlength As Int32) As String
        If str.Length > maxlength Then
            Return str.Substring(0, maxlength)
        End If
        Return str
    End Function

    ''' <summary>
    ''' Acts on a string and expects an array of objects(strings). Case insensitive. But doesn't do any automatic trims.
    ''' </summary>
    ''' <param name="str"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function StartsWithAny_NullSafe(str As String, ParamArray args() As Object) As Boolean
        If str.IsNotNull() Then
            If args.IsNotNull() Then
                For Each Text As String In args
                    If Text.IsNotNull() Then
                        If str.ToLower().StartsWith(Text.ToLower()) Then
                            Return True
                        End If
                    End If
                Next
            End If
        End If
        Return False
    End Function


    ''' <summary>
    ''' Acts on a string and expects an array of objects(strings). Case insensitive. But doesn't do any automatic trims.
    ''' </summary>
    ''' <param name="str"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ContainsAny_NullSafe(str As String, ParamArray args() As Object) As Boolean
        If str.IsNotNull() Then
            If args.IsNotNull() Then
                For Each Text As String In args
                    If Text.IsNotNull() Then
                        If str.ToLower().Contains(Text.ToLower()) Then
                            Return True
                        End If
                    End If
                Next
            End If
        End If
        Return False
    End Function


    ''' <summary>
    ''' Returns the first matching text instance in the provided text.
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetTextBetween(text As String, left As String, right As String) As String
        If text.IsNullEmptyorWhitespace() = False AndAlso left.IsNull = False AndAlso left <> String.Empty AndAlso right.IsNull = False AndAlso right <> String.Empty Then
            If text.Contains(left) AndAlso text.Contains(right) Then
                Return text.Substring(text.IndexOf(left) + left.Length, text.IndexOf(right, text.IndexOf(left) + left.Length) - (text.IndexOf(left) + left.Length))
            End If
        End If
        Return String.Empty
    End Function


#End Region

#Region "Collections"
    ''' <summary>
    ''' Returns True if the collection is not NULL and has any items.
    ''' </summary>
    ''' <param name="coll"></param>
    ''' <returns>System.Boolean TRUE if instance not NULL and has more than one item otherwise FALSE</returns>
    <Extension()>
    Public Function IsLoaded(Of T)(coll As IEnumerable(Of T)) As Boolean
        Return (coll IsNot Nothing AndAlso coll.Any() AndAlso coll(0) IsNot Nothing)
    End Function

    ''' <summary>
    ''' Return True if the collection is not null and has an item at the index that isnot NULL.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="coll"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function HasItemAtIndex(Of T)(coll As IEnumerable(Of T), index As Int32) As Boolean
        Return (coll IsNot Nothing AndAlso coll.Count > index AndAlso coll(index) IsNot Nothing)
    End Function

    ''' <summary>
    ''' Returns the item at the index provided. If the collection is NULL or the index does not exist you will get NULL.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="coll"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetItemAtIndex(Of T)(coll As IEnumerable(Of T), index As Int32) As T
        If coll.HasItemAtIndex(index) Then
            Return coll(index)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Creates the list if needed, then creates new item of proper type, then returns new item.
    ''' MUST BE A TYPE THAT HAS A PARAMETERLESS CONSTRUCTOR
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="coll"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function AddNew(Of T)(ByRef coll As List(Of T)) As T
        If coll Is Nothing Then
            coll = New List(Of T)
        End If
        coll.Add(Activator.CreateInstance(GetType(T))) 'If you get an error here it is because the type you are creating doesn't have a parameterless constructor

        Return coll.LastOrDefault()
    End Function

    ''' <summary>
    ''' Returns the count of the list as 0 if NULL otherwise it returns the actual count.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="coll"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function CountEvenIfNull(Of T)(coll As IEnumerable(Of T)) As Int32
        If coll IsNot Nothing Then
            Return coll.Count
        End If
        Return 0
    End Function

    <Extension()>
    Public Function ListToCSV(Of T)(list As IEnumerable(Of T)) As String
        Dim txt As New StringBuilder()
        If list IsNot Nothing Then
            For Each i In list
                txt.Append(i.ToString().Trim() + ",")
            Next
        End If

        Return txt.ToString().TrimEnd(",")
    End Function

    ''' <summary>
    ''' Returns True if at least one item is in both collections. Basically a wrapper for [ return collection1.Union(collection2).Any() ]
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="coll"></param>
    ''' <param name="contains"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ContainsAny(Of T)(coll As IEnumerable(Of T), contains As IEnumerable(Of T)) As Boolean
        If coll IsNot Nothing AndAlso contains IsNot Nothing Then
            Return coll.Union(contains).Any()
        End If
        Return False
    End Function


    ''' <summary>
    ''' Make the list2 have as many items as list1.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="U"></typeparam>
    ''' <param name="list1"></param>
    ''' <param name="list2"></param>
    <Extension()>
    Public Sub MatchLengthOf(Of T, U)(ByRef list2 As List(Of U), list1 As List(Of T))
        If list1 IsNot Nothing Then
            If list2 Is Nothing Then
                list2 = Activator.CreateInstance(GetType(List(Of U)))
            End If
            While list2.Count < list1.Count
                list2.Add(Activator.CreateInstance(GetType(U)))
            End While
            'now list2 has atleast as many as list1 but may have more
            While list2.Count > list1.Count
                list2.RemoveAt(list2.Count - 1)
            End While
        Else
            list2 = Nothing
        End If
    End Sub


    ''' <summary>
    ''' Creates the destination list if it is null.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="listFrom"></param>
    ''' <param name="listTo"></param>
    <Extension()>
    Public Sub AppendItemsTo(Of T)(listFrom As IEnumerable(Of T), ByRef listTo As List(Of T))
        If listTo.IsNull Then
            listTo = Activator.CreateInstance(GetType(List(Of T)))
        End If
        If listFrom.IsNotNull AndAlso listFrom.Any() Then
            listTo.AddRange(listFrom)
        End If

    End Sub

    ''' <summary>
    ''' Creates the list you are adding an item to if the list is null.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list"></param>
    ''' <param name="item"></param>
    <Extension()>
    Public Sub AddItem(Of T)(ByRef list As List(Of T), item As T)
        If list.IsNull Then
            list = Activator.CreateInstance(GetType(List(Of T)))
        End If
        If item.IsNotNull Then '99.9999% you don't want to add null to a list
            list.Add(item)
        End If
    End Sub



#End Region

#Region "Sql"

#Region "Reader"

    ''' <summary>
    ''' Returns string.empty if field is DBNull
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As String
        Return If(reader.IsDBNull(index), "", reader.GetString(index).ToString())
    End Function


    ''' <summary>
    ''' Returns string.empty if field is DBNull
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As String
        Return If(IsDBNull(reader(columnName)), "", reader(columnName).ToString())
    End Function



    ''' <summary>
    ''' Returns 0 if field is DBNull
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIntIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As Int32
        Return If(reader.IsDBNull(index), 0, reader.GetInt32(index))
    End Function



    ''' <summary>
    ''' Returns 0 if field is DBNull
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIntIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As Int32
        Return If(IsDBNull(reader(columnName)), 0, TryToGetInt32(reader(columnName)))
    End Function



    ''' <summary>
    ''' Returns the Int.ToString() if not DBNull otherwise it return empty string.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIntAsStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As String
        Return If(reader.IsDBNull(index), String.Empty, reader.GetInt32(index).ToString())
    End Function


    ''' <summary>
    ''' Returns the Int.ToString() if not DBNull otherwise it return empty string.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIntAsStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As String
        Return If(IsDBNull(reader(columnName)), "", reader(columnName))
    End Function




    ''' <summary>
    ''' Returns 0 if field is DBNull
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDecimalIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As Decimal
        Return If(reader.IsDBNull(index), 0, reader.GetDecimal(index).ToString())
    End Function



    ''' <summary>
    ''' Returns 0 if field is DBNull
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDecimalIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As Decimal
        Return If(IsDBNull(reader(columnName)), 0.0, CDec(reader(columnName)))
    End Function




    ''' <summary>
    ''' Returns the Decimal.ToString() if not DBNull otherwise it return empty string.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDecimalAsStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As String
        Return If(reader.IsDBNull(index), "", reader.GetDecimal(index).ToString())
    End Function


    ''' <summary>
    ''' Returns the Decimal.ToString() if not DBNull otherwise it return empty string.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDecimalAsStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As String
        Return If(IsDBNull(reader(columnName)), "", reader(columnName).ToString())
    End Function




    ''' <summary>
    ''' Returns datetime.MinValue if field is DBNull.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDateTimeIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As DateTime
        Return If(reader.IsDBNull(index), DateTime.MinValue, reader.GetDateTime(index))
    End Function



    ''' <summary>
    ''' Returns datetime.MinValue if field is DBNull.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDateTimeIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As DateTime
        Return If(IsDBNull(reader(columnName)), DateTime.MinValue, CDate(reader(columnName).ToString()))
    End Function



    ''' <summary>
    ''' Returns Datetime.ToShortDateString() if not DBNull otherwise it returns empty string.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDateTimeAsShortDateStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, index As Int32) As String
        Return If(reader.IsDBNull(index), "", reader.GetDateTime(index).ToShortDateString())
    End Function


    ''' <summary>
    ''' Returns Datetime.ToShortDateString() if not DBNull otherwise it returns empty string.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="columnName"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDateTimeAsShortDateStringIgnoreDBNull(reader As System.Data.SqlClient.SqlDataReader, columnName As String) As String
        Return If(IsDBNull(reader(columnName)), "", reader(columnName).ToString())
    End Function

#End Region

#End Region





End Module