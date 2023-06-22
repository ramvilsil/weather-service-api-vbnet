Imports System.Web.Http
Imports System.Threading.Tasks
Imports Application.Services
Imports Application.Models

Namespace Controllers

    Public Class CurrentWeatherController
        Inherits ApiController

        Private ReadOnly _weatherService As WeatherService

        Public Sub New(weatherService As WeatherService)
            _weatherService = weatherService
        End Sub

        <HttpGet>
        Public Async Function GetCurrentWeatherByPublicGeolocation() As Task(Of IHttpActionResult)
            Dim response = Await _weatherService.GetCurrentWeatherByGeolocationAsync()
            Return Ok(response)
        End Function

        <HttpPost>
        Public Async Function GetCurrentWeatherBySpecificGeolocation(<FromBody> geolocation As Geolocation) As Task(Of IHttpActionResult)
            Dim response = Await _weatherService.GetCurrentWeatherByGeolocationAsync(geolocation)
            Return Ok(response)
        End Function

    End Class

End Namespace