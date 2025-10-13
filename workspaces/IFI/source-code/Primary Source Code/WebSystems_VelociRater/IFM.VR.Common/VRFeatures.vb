
Imports System.Web

Namespace IFM.VR.Common
    ''' <summary>
    ''' Used to allow devs to experiment with new features while preserving existing functionality. Once a feature is mainstream you should remove it from this class.
    ''' </summary>
    Public Class VRFeatures

        Public Shared ReadOnly Property AllowStaffToPullFromDiamond As Boolean
            Get
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context.Session("allowPullFromDiamond") IsNot Nothing Then
                    Dim result As Boolean = False
                    Boolean.TryParse(context.Session("allowPullFromDiamond"), result)
                    Return result
                End If
                Return False
            End Get
        End Property

        Public Shared ReadOnly Property AllowVrToUpdateFromDiamond As Boolean
            Get
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Session("allowUpdateFromDiamond") IsNot Nothing Then
                    Dim result As Boolean = False
                    Boolean.TryParse(context.Session("allowUpdateFromDiamond"), result)
                    Return result
                End If

                Return System.Configuration.ConfigurationManager.AppSettings("AllowVRToUpdateFromDiamond") IsNot Nothing AndAlso CBool(System.Configuration.ConfigurationManager.AppSettings("AllowVRToUpdateFromDiamond"))
            End Get
        End Property
        'added 11/10/2020 (Interoperability); removed 11/20/2020... will just use QQ method
        'Public Shared ReadOnly Property AllowVRToUpdateFromDiamond_Interoperability As Boolean
        '    Get
        '        Dim context As HttpContext = System.Web.HttpContext.Current
        '        If context IsNot Nothing AndAlso context.Session("allowUpdateFromDiamond_Interoperability") IsNot Nothing Then
        '            Dim result As Boolean = False
        '            Boolean.TryParse(context.Session("allowUpdateFromDiamond_Interoperability"), result)
        '            Return result
        '        End If

        '        Return System.Configuration.ConfigurationManager.AppSettings("AllowVRToUpdateFromDiamond_Interoperability") IsNot Nothing AndAlso CBool(System.Configuration.ConfigurationManager.AppSettings("AllowVRToUpdateFromDiamond_Interoperability"))
        '    End Get
        'End Property


    End Class
End Namespace

