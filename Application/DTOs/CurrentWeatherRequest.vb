Imports System.ComponentModel.DataAnnotations

Namespace DTOs

    Public Class CurrentWeatherRequest
        <Required>
        Public Property Geolocation As String
    End Class

End Namespace