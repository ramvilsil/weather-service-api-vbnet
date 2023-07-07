Namespace DTOs.CurrentWeather

    Public Class ResponsePayloadDTO

        Public Property Geolocation As Geolocation.ResponsePayloadDTO
        Public Property TemperatureC As Double
        Public Property TemperatureF As Double
        Public Property Condition As String
        Public Property Humidity As Integer
        Public Property WindMph As Double
        Public Property WindKph As Double

    End Class

End Namespace