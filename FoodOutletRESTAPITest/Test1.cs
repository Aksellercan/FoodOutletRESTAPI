using System.Net.Http.Json;
using FoodOutletRESTAPIDatabase.Models;
using System.Net.Http.Headers;

namespace FoodOutletRESTAPITest
{
    [TestClass]
    public sealed class Test1(HttpClient client)
    {
        HttpClient _client = client;

        [TestInitialize]
        public void SetUp()
        {
            // Create an HttpClientHandler that disables SSL validation
            var handler = new HttpClientHandler()
            {
                // Disable SSL validation for testing purposes
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5038");
        }

        [TestMethod]
        public async Task GetAllFoodOutlets_ShouldReturnSuccess()
        {
            // Act: Send a GET request to the API endpoint
            var response = await _client.GetAsync("/api/foodoutlet");

            // Optionally log or check the status code if needed:
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            // Deserialize the response body into a list of FoodOutlets
            var actualOutlets = await response.Content.ReadFromJsonAsync<List<FoodOutlet>>();

            // If no food outlets are found (404 Not Found), you can skip testing the response content
            if (actualOutlets == null)
            {
                Assert.Fail("The response returned no food outlets or an unexpected status.");
                return; // Exit the test if no outlets are found
            }

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Success.");
        }

        [TestMethod]
        public async Task PostReview_ShouldReturnFail()
        {
            // Your food outlet ID
            const int foodOutletId = 8;

            // Review data
            var reviewData = new
            {
                comment = "Test method review",
                score = 5
            };

            // JWT token
            const string jwtToken = "token";

            // Set the authorization header with the Bearer token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Send the POST request with the review data
            var response = await _client.PostAsJsonAsync(
                $"http://localhost:5038/foodoutlets/{foodOutletId}/reviews",
                reviewData
            );

            // Assert the response
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode,
                "Expected status code to be 401 Unauthorized.");
        }

        [TestMethod]
        public async Task AddFoodOutlet_ShouldReturnFail()
        {
            // Setup the outlet data
            var newOutlet = new
            {
                name = "Test Method Outlet",
                location = "New Location"
            };

            // JWT token
            const string jwtToken = "token";

            // Set the authorization header with the Bearer token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act: Send a POST request to add the new food outlet
            var response = await _client.PostAsJsonAsync(
                "http://localhost:5038/foodoutlets/api/foodoutlet",
                newOutlet
            );

            // Assert: Check that the POST request fails (Unauthorized)
            Assert.IsFalse(response.IsSuccessStatusCode, "Request should fail with unauthorized status.");

            // Assert: Check that the response status code is 401 Unauthorized
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode,
                "Expected status code to be 401 Unauthorized.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose of HttpClient after the test
            _client.Dispose();
        }
    }
}