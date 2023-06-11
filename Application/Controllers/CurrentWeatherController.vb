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
            Return Ok(
                Await _weatherService.GetCurrentWeatherByGeolocationAsync()
            )
        End Function

        <HttpPost>
        Public Async Function GetCurrentWeatherBySpecificGeolocation(<FromBody> geolocation As Geolocation) As Task(Of IHttpActionResult)
            Return Ok(
                Await _weatherService.GetCurrentWeatherByGeolocationAsync(geolocation)
            )
        End Function

    End Class

End Namespace