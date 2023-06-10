Imports System.Net
Imports System.Web.Http
Imports System.Threading.Tasks
Imports Application.Services
Imports Application.Models

Public Class MainController
    Inherits ApiController

    Private ReadOnly _weatherService As WeatherService

    Public Sub New(weatherService As WeatherService)
        _weatherService = weatherService
    End Sub

    'Async Function GetGeolocation() As Task(Of String)
    '    Dim geolocation = Await _weatherService.GetGeolocationByPublicIpAddressAsync()

    '    Return $"{geolocation.City}, {geolocation.Region}, {geolocation.Country}"
    'End Function

    Async Function GetWeather() As Task(Of String)
        Dim weather = Await _weatherService.GetCurrentWeatherByGeolocationAsync()

        Return $"{weather.Geolocation}, {weather.TemperatureF}, {weather.Condition}"
    End Function

End Class