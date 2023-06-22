Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Dynamic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Application.Models

Namespace Services

    Public Class WeatherService

        Private ReadOnly _weatherApiKey As String
        Private ReadOnly _weatherApiUrl As String
        Private ReadOnly _geolocationApiUrl As String

        Private ReadOnly _httpClient As HttpClient

        Public Sub New(
            httpClient As HttpClient,
            weatherApiKey As String,
            weatherApiUrl As String,
            geolocationApiUrl As String
        )
            _httpClient = httpClient
            _weatherApiKey = weatherApiKey
            _weatherApiUrl = weatherApiUrl & _weatherApiKey
            _geolocationApiUrl = geolocationApiUrl
        End Sub

        Public Async Function GetCurrentWeatherByGeolocationAsync(Optional geolocation As Object = Nothing) As Task(Of Object)
            If geolocation Is Nothing Then
                Dim result = Await GetGeolocationByPublicIpAddressAsync()
                If TypeOf result Is Geolocation Then
                    geolocation = CType(result, Geolocation)
                Else
                    Return result
                End If
            End If

            Dim geolocationQuery As String

            Select Case True
                Case geolocation.Latitude IsNot Nothing AndAlso geolocation.Longitude IsNot Nothing
                    geolocationQuery = $"{geolocation.Latitude},{geolocation.Longitude}"
                Case geolocation.City IsNot Nothing
                    geolocationQuery = $"{geolocation.City}"
                Case geolocation.Region IsNot Nothing
                    geolocationQuery = $"{geolocation.Region}"
                Case geolocation.Country IsNot Nothing
                    geolocationQuery = $"{geolocation.Country}"
                Case Else
                    Return New With {.Error = "No valid geolocation provided"}
            End Select

            Dim response As HttpResponseMessage = Await _httpClient.GetAsync($"{_weatherApiUrl}&q={geolocationQuery}")

            Dim content As String = Await response.Content.ReadAsStringAsync()

            If Not response.IsSuccessStatusCode Then
                Dim statusCode = response.StatusCode
                Dim message = JObject.Parse(content)
                Return New With {.Error = New With {.StatusCode = statusCode, .Message = message}}
            End If

            Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(content)

            Debug.WriteLine(data)

            Dim currentWeather As CurrentWeather = Nothing

            Try
                currentWeather = New CurrentWeather With
                {
                    .Geolocation = $"{data.location.name}, {data.location.region}, {data.location.country}",
                    .TemperatureC = data.current.temp_c,
                    .TemperatureF = data.current.temp_f,
                    .Condition = data.current.condition.text,
                    .Humidity = data.current.humidity,
                    .WindMph = data.current.wind_mph,
                    .WindKph = data.current.wind_kph
                }
            Catch ex As Exception
                Return New With {.Error = "Invalid API Response"}
            End Try

            Return currentWeather

        End Function

        Private Function GetClientPublicIpAddress() As String
            Dim request = HttpContext.Current.Request
            Dim userHostAddress = request.UserHostAddress

            Try
                Dim hostIpAddress = IPAddress.Parse(userHostAddress)
                If hostIpAddress.AddressFamily = System.Net.Sockets.AddressFamily.InterNetworkV6 Then
                    Dim ipv4Address As IPAddress = hostIpAddress.MapToIPv4()
                    Return ipv4Address.ToString()
                Else
                    Return hostIpAddress.ToString()
                End If
            Catch
                Return userHostAddress
            End Try
        End Function

        Public Async Function GetGeolocationByPublicIpAddressAsync(Optional publicIpAddress As String = Nothing) As Task(Of Object)
            If publicIpAddress Is Nothing Then
                Debug.WriteLine($"No public ip address passed")
                Try
                    publicIpAddress = GetClientPublicIpAddress()
                Catch ex As Exception
                    Return New With {.Error = "Client Public IP Address could not be fetched"}
                End Try
            End If

            Dim response As HttpResponseMessage = Await _httpClient.GetAsync($"{_geolocationApiUrl}{publicIpAddress}")
            Dim content As String = Await response.Content.ReadAsStringAsync()

            If Not response.IsSuccessStatusCode Then
                Dim statusCode = response.StatusCode
                Dim message = JObject.Parse(content)
                Return New With {.Error = New With {.StatusCode = statusCode, .Message = message}}
            End If

            Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(content)

            Debug.WriteLine(data)

            Dim geolocation As Geolocation = Nothing
            Try
                geolocation = New Geolocation With {
                .City = data.city,
                .Region = data.regionName,
                .Country = data.country,
                .Latitude = data.lat,
                .Longitude = data.lon
            }
            Catch ex As Exception
                Return New With {.Error = "Invalid API Response"}
            End Try

            Return geolocation
        End Function


    End Class

End Namespace