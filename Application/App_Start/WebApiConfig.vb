Imports System.Web.Http
Imports System.Web.Http.Cors

Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        Dim cors = New EnableCorsAttribute("*", "*", "*")
        config.EnableCors(cors)

        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )
    End Sub
End Module