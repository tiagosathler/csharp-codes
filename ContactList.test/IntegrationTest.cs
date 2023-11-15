namespace ContactList.test;

using ContactList.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _httpClient;
    private readonly Mock<IContactService> _mockService;

    public IntegrationTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _mockService = new();

        _httpClient = _webApplicationFactory
            .WithWebHostBuilder(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IContactService));

                    if (serviceDescriptor != null)
                    {
                        services.Remove(serviceDescriptor);
                    }

                    services.AddSingleton(_mockService.Object);
                });
            })
            .CreateClient();
    }

    [Fact]
    public void Test1()
    {
    }
}