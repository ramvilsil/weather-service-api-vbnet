﻿Imports System.Web.Http
Imports System.Threading.Tasks
Imports Application.Services
Imports Application.Models

Namespace Controllers

    Public Class GeolocationController
        Inherits ApiController

        Private ReadOnly _weatherService As WeatherService

        Public Sub New(weatherService As WeatherService)
            _weatherService = weatherService
        End Sub

        <HttpGet>
        Public Async Function GetGeolocationByPublicIpAddress() As Task(Of IHttpActionResult)
            Dim response = Await _weatherService.GetGeolocationByPublicIpAddressAsync()
            Return Ok(response)
        End Function

        <HttpPost>
        Public Async Function GetGeolocationBySpecificIpAddress(<FromBody> ipAddress As String) As Task(Of IHttpActionResult)
            Dim response = Await _weatherService.GetGeolocationByPublicIpAddressAsync(ipAddress)
            Return Ok(response)
        End Function

    End Class

End Namespace