Imports System.Web.Http
Imports System.Net.Http
Imports Unity
Imports Unity.Lifetime
Imports Unity.AspNet.WebApi
Imports Unity.Injection
Imports Application.Services


Public Class WebApiApplication
    Inherits HttpApplication

    Sub Application_Start()
        Dim container = New UnityContainer()
        container.RegisterInstance(New HttpClient())

        Dim weatherApiUrl As String = ConfigurationManager.AppSettings("WeatherApi.Url")
        Dim weatherApiKey As String = ConfigurationManager.AppSettings("WeatherApi.Key")

        Dim geolocationApiUrl As String = ConfigurationManager.AppSettings("GeolocationApi.Url")

        container.RegisterType(Of WeatherService)(New HierarchicalLifetimeManager(),
                                          New InjectionConstructor(New ResolvedParameter(Of HttpClient),
                                                                   weatherApiKey, weatherApiUrl, geolocationApiUrl))


        GlobalConfiguration.Configuration.DependencyResolver = New UnityDependencyResolver(container)
        GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
    End Sub
End Class