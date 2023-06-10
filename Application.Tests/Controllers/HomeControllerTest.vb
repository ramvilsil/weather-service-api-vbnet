Imports System.Web.Mvc
Imports Application
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class HomeControllerTest
    <TestMethod()> Public Sub Index()
        'Arrange
        Dim controller As New WeatherController()

        'Act
        Dim result As ViewResult = DirectCast(controller.Index(), ViewResult)

        'Assert
        Assert.IsNotNull(result)
        Dim viewData As ViewDataDictionary = result.ViewData
        Assert.AreEqual("Home Page", viewData("Title"))
    End Sub
End Class
