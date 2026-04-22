using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace AgriMarket.Tests.Helpers;

public static class ControllerContextFactory
{
    private static IServiceProvider BuildServices(bool includeAuth = false)
    {
        var services = new Mock<IServiceProvider>();

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/test");
        var urlHelperFactory = new Mock<IUrlHelperFactory>();
        urlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);
        services.Setup(s => s.GetService(typeof(IUrlHelperFactory))).Returns(urlHelperFactory.Object);

        var tempData = new Mock<ITempDataDictionary>();
        var tempDataFactory = new Mock<ITempDataDictionaryFactory>();
        tempDataFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(tempData.Object);
        services.Setup(s => s.GetService(typeof(ITempDataDictionaryFactory))).Returns(tempDataFactory.Object);

        if (includeAuth)
        {
            var authService = new Mock<IAuthenticationService>();
            authService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string?>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties?>()))
                .Returns(Task.CompletedTask);
            services.Setup(s => s.GetService(typeof(IAuthenticationService))).Returns(authService.Object);
        }

        return services.Object;
    }

    public static ControllerContext WithAuthenticatedUser(Guid userId, string role = "Farmer")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal,
            RequestServices = BuildServices()
        };
        return new ControllerContext { HttpContext = httpContext };
    }

    public static ControllerContext WithSignInSupport()
    {
        var httpContext = new DefaultHttpContext { RequestServices = BuildServices(includeAuth: true) };
        return new ControllerContext { HttpContext = httpContext };
    }
}
