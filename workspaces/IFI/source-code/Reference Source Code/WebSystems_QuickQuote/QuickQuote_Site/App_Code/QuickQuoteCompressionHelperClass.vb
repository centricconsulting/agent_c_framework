Imports Microsoft.VisualBasic
'Imports QuickQuote.CommonObjects
'Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports System.Xml
Imports System.IO
Imports System.IO.Compression

Namespace QuickQuote.CommonMethods
    ''' <summary>
    ''' class used for common compression methods
    ''' </summary>
    ''' <remarks>currently used for storing/retrieving quote xml byte arrays</remarks>
    <Serializable()> _
    Public Class QuickQuoteCompressionHelperClass 'added 9/10/2015

        'Dim qqHelper As New QuickQuoteHelperClass 'can't be here since QuickQuoteHelperClass instantiates an instance of this class (causes StackOverflowException because of circular reference)

        Public Function BytesFromString(ByVal str As String, Optional ByVal compressed As Boolean = True) As Byte()
            Dim bytes As Byte() = Nothing
            Dim zippedBytes As Byte() = Nothing

            If String.IsNullOrEmpty(str) = False Then
                bytes = System.Text.Encoding.ASCII.GetBytes(str)

                If compressed = True Then
                    'Using mem As New MemoryStream
                    '    'Using zip As New GZipStream(mem, CompressionMode.Compress, True) 'overload for LeaveOpen (boolean)
                    '    Using zip As New GZipStream(mem, CompressionMode.Compress) 'overload for LeaveOpen (boolean)
                    '        zip.Write(bytes, 0, bytes.Length)
                    '        zip.Close() 'added 9/4/2015; needed orelse zippedBytes will just reflect the 1st 10 positions (header)
                    '        zippedBytes = mem.ToArray
                    '    End Using
                    'End Using
                    '9/4/2015 - moved above logic to CompressedBytes(); Compress() also works... commented below
                    zippedBytes = CompressedBytes(bytes, False) 'defaulting optional verifyDecompressed param to False (we already know since we just created it above)
                    'zippedBytes = Compress(bytes)
                End If
            End If

            If zippedBytes IsNot Nothing Then 'maybe check zippedBytes.Length to make sure it's smaller than bytes before returning
                Return zippedBytes
            Else
                Return bytes
            End If
        End Function
        Public Function StringFromBytes(ByVal bytes As Byte()) As String
            Dim str As String = ""

            If bytes IsNot Nothing Then
                If IsCompressed(bytes) = True Then
                    'Using mem As New MemoryStream
                    '    mem.Write(bytes, 0, bytes.Length)
                    '    mem.Position = 0 'updated 9/4/2015 from 1 to 0
                    '    'Using zip As New GZipStream(mem, CompressionMode.Decompress, True) 'overload for LeaveOpen (boolean)
                    '    Using zip As New GZipStream(mem, CompressionMode.Decompress) 'overload for LeaveOpen (boolean)
                    '        Using output As New MemoryStream
                    '            Dim buff As Byte() = New Byte(63) {}
                    '            Dim read As Integer = -1
                    '            read = zip.Read(buff, 0, buff.Length) 'System.IO.InvalidDataException: The magic number in GZip header is not correct. Make sure you are passing in a GZip stream. ... fixed after changing mem.Position from 1 to 0
                    '            While read > 0
                    '                output.Write(buff, 0, read)
                    '                read = zip.Read(buff, 0, buff.Length)
                    '            End While
                    '            zip.Close()
                    '            bytes = output.ToArray()
                    '        End Using
                    '    End Using
                    'End Using
                    '9/4/2015 - moved above logic to DecompressedBytes(); Decompress() also works... commented below
                    'DecompressedBytes(bytes, False) 'defaulting optional verifyCompressed param to False (already checked above)
                    'updated (fixed) 8/5/2016 to actually set bytes to function result since it wasn't doing anything before since it's not a Sub that passes bytes ByRef
                    bytes = DecompressedBytes(bytes, False) 'defaulting optional verifyCompressed param to False (already checked above)
                    'bytes = Decompress(bytes)
                End If
                str = System.Text.Encoding.ASCII.GetString(bytes)
            End If

            Return str
        End Function
        'added CompressedBytes and DecompressedBytes on 9/4/2015
        Public Function CompressedBytes(ByVal bytes As Byte(), Optional ByVal verifyDecompressed As Boolean = True) As Byte()
            Dim compressed As Byte() = Nothing

            If bytes IsNot Nothing Then
                Dim okayToCompress As Boolean = True
                If verifyDecompressed = True Then
                    okayToCompress = Not IsCompressed(bytes)
                End If
                If okayToCompress = True Then
                    Using mem As New MemoryStream
                        'Using zip As New GZipStream(mem, CompressionMode.Compress, True) 'overload for LeaveOpen (boolean)
                        Using zip As New GZipStream(mem, CompressionMode.Compress) 'overload for LeaveOpen (boolean)
                            zip.Write(bytes, 0, bytes.Length)
                            zip.Close() 'added 9/4/2015; needed orelse zippedBytes will just reflect the 1st 10 positions (header)
                            compressed = mem.ToArray
                        End Using
                    End Using
                End If
            End If

            If compressed IsNot Nothing Then
                Return compressed
            Else
                Return bytes
            End If

        End Function
        Public Function DecompressedBytes(ByVal bytes As Byte(), Optional ByVal verifyCompressed As Boolean = True) As Byte()
            Dim decompressed As Byte() = Nothing

            If bytes IsNot Nothing Then
                Dim okayToDecompress As Boolean = True
                If verifyCompressed = True Then
                    okayToDecompress = IsCompressed(bytes)
                End If
                If okayToDecompress = True Then
                    Using mem As New MemoryStream
                        mem.Write(bytes, 0, bytes.Length)
                        mem.Position = 0 'updated 9/4/2015 from 1 to 0
                        'Using zip As New GZipStream(mem, CompressionMode.Decompress, True) 'overload for LeaveOpen (boolean)
                        Using zip As New GZipStream(mem, CompressionMode.Decompress) 'overload for LeaveOpen (boolean)
                            Using output As New MemoryStream
                                Dim buff As Byte() = New Byte(63) {}
                                Dim read As Integer = -1
                                read = zip.Read(buff, 0, buff.Length) 'System.IO.InvalidDataException: The magic number in GZip header is not correct. Make sure you are passing in a GZip stream. ... fixed after changing mem.Position from 1 to 0
                                While read > 0
                                    output.Write(buff, 0, read)
                                    read = zip.Read(buff, 0, buff.Length)
                                End While
                                zip.Close()
                                decompressed = output.ToArray()
                            End Using
                        End Using
                    End Using
                End If
            End If

            If decompressed IsNot Nothing Then
                Return decompressed
            Else
                Return bytes
            End If

        End Function
        Public Function IsCompressed(ByVal bytes As Byte()) As Boolean
            'dim GZipHeaderBytes as Byte() = {0x1f, 0x8b, 8, 0, 0, 0, 0, 0, 4, 0}
            'dim GZipLevel10HeaderBytes as Byte() = {0x1f, 0x8b, 8, 0, 0, 0, 0, 0, 2, 0}
            Dim GZipHeaderBytes As Byte() = {31, 139, 8, 0, 0, 0, 0, 0, 4, 0}
            Dim GZipLevel10HeaderBytes As Byte() = {31, 139, 8, 0, 0, 0, 0, 0, 2, 0}
            Dim isIt As Boolean = False

            If bytes IsNot Nothing AndAlso bytes.Length > 10 Then 'maybe allow for bytes.Length >= 10 as long as the compressed pattern would always need to be removed before converting back to string
                isIt = True 'default to True and change to False once there's not a match to one of the 10 expected header positions
                For i As Integer = 0 To 9 'check all 10 positions
                    If bytes(i) = GZipHeaderBytes(i) OrElse bytes(i) = GZipLevel10HeaderBytes(i) Then
                        'still okay
                    Else
                        isIt = False
                        Exit For
                    End If
                Next
            End If

            Return isIt
        End Function
        'Public Function ShouldCompressQuickQuoteXmls() As Boolean 'moved to QuickQuoteHelperClass to avoid circular reference
        '    Dim shouldCompress As Boolean = False

        '    If ConfigurationManager.AppSettings("QuickQuote_ShouldCompressQuoteXmls") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_ShouldCompressQuoteXmls").ToString <> "" Then
        '        If UCase(ConfigurationManager.AppSettings("QuickQuote_ShouldCompressQuoteXmls").ToString) = "YES" OrElse qqHelper.BitToBoolean(ConfigurationManager.AppSettings("QuickQuote_ShouldCompressQuoteXmls").ToString) = True Then
        '            shouldCompress = True
        '        End If
        '    End If

        '    Return shouldCompress
        'End Function
    End Class
End Namespace