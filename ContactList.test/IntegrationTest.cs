namespace ContactList.test;

using AutoBogus;
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
    private readonly HttpClient _httpClient;
    private readonly Mock<IContactService> _mockService;
    private const string URI = "/person";

    public IntegrationTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        _mockService = new();

        _httpClient = webApplicationFactory
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
        Person[] peopleFake = AutoFaker.Generate<Person>(3).ToArray();
        _mockService.Setup(s => s.getPersonList()).Returns(peopleFake);

        // act
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);
        string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
        Person[]? peopleResponse = JsonConvert.DeserializeObject<Person[]>(responseMessage);

        // asserts
        Assert.NotNull(peopleResponse);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal(peopleFake.Length, peopleResponse.Length);
        Assert.Equivalent(peopleFake, peopleResponse, true);
    }

    [Theory(DisplayName = "Testando a rota /POST Person")]
    [InlineData(URI)]
    public async Task TestPostPerson(string uri)
    {
        // arrange
        Person personMoq = AutoFaker.Generate<Person>();
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