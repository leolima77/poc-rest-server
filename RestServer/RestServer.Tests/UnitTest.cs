using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestServer.Application.Interfaces;
using RestServer.Domain.Entities;
using System.Net;
using System.Text;

[TestClass]
public class SimulateRequestsTests
{
    [TestMethod]
    public async Task SimulateRequests_SuccessfulSimulation()
    {
        // Arrange
        var httpClientMock = new Mock<IHttpClientWrapper>();

        var postResponses = new List<HttpResponseMessage>
        {
            new HttpResponseMessage(HttpStatusCode.Created),
            new HttpResponseMessage(HttpStatusCode.Created)
        };

        var getResponses = new List<string>
        {
            "[{ \"FirstName\": \"John\", \"LastName\": \"Doe\", \"Age\": 30, \"Id\": 1 }]",
            "[{ \"FirstName\": \"Jane\", \"LastName\": \"Doe\", \"Age\": 28, \"Id\": 2 }]"
        };

        for (int i = 0; i < postResponses.Count; i++)
        {
            httpClientMock.Setup(c => c.PostAsync(It.IsAny<Uri>(), It.IsAny<StringContent>()))
                .ReturnsAsync(postResponses[i])
                .Verifiable();

            httpClientMock.Setup(c => c.GetStringAsync(It.IsAny<Uri>()))
                .ReturnsAsync(getResponses[i])
                .Verifiable();
        }

        // Act
        await SimulateRequests(httpClientMock.Object);

        // Assert
        httpClientMock.Verify(c => c.PostAsync(It.IsAny<Uri>(), It.IsAny<StringContent>()), Times.Exactly(postResponses.Count));
        httpClientMock.Verify(c => c.GetStringAsync(It.IsAny<Uri>()), Times.Exactly(getResponses.Count));
    }

    private async Task SimulateRequests(IHttpClientWrapper httpClient)
    {
        var baseUri = new Uri("http://localhost:5000");

        var postRequests = new List<List<Customer>>
        {
            new List<Customer>
            {
                new Customer { FirstName = "Leia", LastName = "Liberty", Age = 20, Id = 1 },
                new Customer { FirstName = "Sadie", LastName = "Ray", Age = 24, Id = 2 }
            },
            new List<Customer>
            {
                new Customer { FirstName = "Jose", LastName = "Harrison", Age = 30, Id = 3 },
                new Customer { FirstName = "Sara", LastName = "Ronan", Age = 28, Id = 4 }
            }
        };

        var getRequests = new List<Task<string>>();

        foreach (var postData in postRequests)
        {
            var json = JsonConvert.SerializeObject(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await httpClient.PostAsync(new Uri(baseUri, "/customer"), content);
        }

        for (int i = 0; i < postRequests.Count; i++)
        {
            getRequests.Add(httpClient.GetStringAsync(new Uri(baseUri, "/customer")));
        }

        var getResponses = await Task.WhenAll(getRequests);

        foreach (var response in getResponses)
        {
            Assert.IsNotNull(response); // Add more specific assertions based on your expected response
        }
    }
}
