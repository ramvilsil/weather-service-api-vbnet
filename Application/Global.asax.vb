Imports System.Web.Http
Imports Unity
Imports Unity.Lifetime
Imports Unity.AspNet.WebApi
Imports Application.Services

Public Class WebApiApplication
    Inherits System.Web.HttpApplication

    Sub Application_Start()

        Dim container = New UnityContainer()
        container.RegisterType(Of WeatherService, WeatherService)(New HierarchicalLifetimeManager())
        GlobalConfiguration.Configuration.DependencyResolver = New UnityDependencyResolver(container)

        GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
    End Sub
End Class