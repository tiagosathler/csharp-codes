namespace ContactList.test;

using ContactList.Models;
using ContactList.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;
using System.Text;

public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _httpClient;
    private readonly Mock<IContactService> _mockService;
    private const string URI = "/person";

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

    [Theory(DisplayName = "Testando a rota /GET Person")]
    [InlineData(URI)]
    public async Task TestGetPeople(string uri)
    {
        // arrange
        Person[] peopleMoq = new Person[2];
        peopleMoq[0] = new Person { PersonId = 1, PersonName = "Maria", PersonEmail = "maria@betrybe.com", PersonPhone = "5511999999999" };
        peopleMoq[1] = new Person { PersonId = 2, PersonName = "João", PersonEmail = "joao@betrybe.com", PersonPhone = "5511988888888" };
        _mockService.Setup(s => s.getPersonList()).Returns(peopleMoq);

        // act
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);
        string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
        Person[]? peopleResponse = JsonConvert.DeserializeObject<Person[]>(responseMessage);

        // asserts
        Assert.NotNull(peopleResponse);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal(peopleMoq.Length, peopleResponse.Length);
        Assert.Equivalent(peopleMoq, peopleResponse, true);
    }

    [Theory(DisplayName = "Testando a rota /POST Person")]
    [InlineData(URI)]
    public async Task TestPostPerson(string uri)
    {
        // arrange
        Person personMoq = new() { PersonId = 3, PersonName = "Rebeca", PersonEmail = "Rebeca@betrybe.com", PersonPhone = "5511977777777" };
        object inputOjb = new { personMoq.PersonName, personMoq.PersonEmail, personMoq.PersonPhone };
        HttpContent requestBody = new StringContent(JsonConvert.SerializeObject(inputOjb), Encoding.UTF8, MediaTypeNames.Application.Json);
        _mockService.Setup(s => s.addPerson(It.IsAny<Person>())).Returns(personMoq);

        // act
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(uri, requestBody);
        string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
        Person? personResponse = JsonConvert.DeserializeObject<Person>(responseMessage);

        // asserts
        Assert.NotNull(personResponse);
        Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
        Assert.Equivalent(personMoq, personResponse, true);
    }
}