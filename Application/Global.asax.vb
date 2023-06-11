Imports System.Web.Http
Imports System.Web
Imports Unity
Imports Unity.Lifetime
Imports Unity.AspNet.WebApi
Imports Application.Services

Public Class WebApiApplication
    Inherits HttpApplication

    Sub Application_Start()

        Dim container = New UnityContainer()
        container.RegisterType(Of WeatherService, WeatherService)(New HierarchicalLifetimeManager())
        GlobalConfiguration.Configuration.DependencyResolver = New UnityDependencyResolver(container)

        GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
    End Sub
End Class