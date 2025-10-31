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
            var response = await _client.GetAsync("/api/foodoutlet"); // Replace with your API endpoint

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

            // Expected list of food outlets
            var expectedOutlets = new List<FoodOutlet>
            {
                new FoodOutlet {Id = 8, Name = "Kebab King 4", Location = "poznan"},
                new FoodOutlet {Id = 10, Name = "Rover", Location = "mars"},
                new FoodOutlet {Id = 11, Name = "Pizza Hut", Location = "US New York"},
                new FoodOutlet {Id = 12, Name = "KFC", Location = "US Kentucky"},
                new FoodOutlet {Id = 14, Name = "Stary Browar MC", Location = "poznan"},
                new FoodOutlet {Id = 15, Name = "Cafe", Location = "Poznan PL"},
                new FoodOutlet {Id = 16, Name = "Cafe the sequel", Location = "Poznan PL"},
                new FoodOutlet {Id = 18, Name = "nirvana", Location = "Seattle USA"},
                new FoodOutlet {Id = 19, Name = "Serve the Servants", Location = "Seattle USA"},
                //new FoodOutlet {Id = 20, Name = "spring", Location = "Seattle USA"},
                new FoodOutlet {Id = 21, Name = "Blur", Location = "England London"},
                new FoodOutlet {Id = 23, Name = "sametin yeri", Location = "342 Stary Rynek Poznan"}
            };

            // Assert: Check if actual and expected lists are equal
            Assert.AreEqual(expectedOutlets.Count, actualOutlets.Count, "The number of outlets doesn't match.");

            for (int i = 0; i < expectedOutlets.Count; i++)
            {
                //Assert.AreEqual(expectedOutlets[i].Id, actualOutlets[i].Id);
                //Assert.AreEqual(expectedOutlets[i].Name, actualOutlets[i].Name);
                //Assert.AreEqual(expectedOutlets[i].Location, actualOutlets[i].Location);
                Assert.IsTrue(string.Equals(expectedOutlets[i].Name, actualOutlets[i].Name, StringComparison.OrdinalIgnoreCase));
                Assert.IsTrue(string.Equals(expectedOutlets[i].Location, actualOutlets[i].Location, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(expectedOutlets[i].Id, actualOutlets[i].Id);


            }
        }


        [TestMethod]
        public async Task PostReview_ShouldReturnSuccess()
        {
            const int foodOutletId = 8;

            // Review data
            var reviewData = new
            {
                comment = "Test method review",
                score = 5
            };

            // JWT token
            const string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiNCIsImV4cCI6MTczODM1MDY0NCwiaXNzIjoiWW91cklzc3VlciIsImF1ZCI6IllvdXJBdWRpZW5jZSJ9.FX4CcHB4-kA0QLqLqAovvpQEdYfxLNYOTOdnxHoIeWw";

            // Set the authorization header with the Bearer token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Send the POST request with the review data
            var response = await _client.PostAsJsonAsync(
                $"http://localhost:5038/foodoutlets/{foodOutletId}/reviews",
                reviewData
            );

            // Assert the response
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to post review.");
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
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to post review.");
        }

        [TestMethod]
        public async Task AddFoodOutlet_ShouldReturnSuccessAndGetAllOutlets()
        {
            // Setup the outlet data
            var newOutlet = new
            {
                name = "Test Method Outlet",
                location = "New Location"
            };

            // JWT token
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiNCIsImV4cCI6MTczODE2NzUxMCwiaXNzIjoiWW91cklzc3VlciIsImF1ZCI6IllvdXJBdWRpZW5jZSJ9.pmsnTfSlRQF-q3kpuEJ_y2dTfUeE_xmcUEAzM2B3OCg";

            // Set the authorization header with the Bearer token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Act: Send a POST request to add the new food outlet
            var response = await _client.PostAsJsonAsync(
                "http://localhost:5038/foodoutlets/api/foodoutlet",
                newOutlet
            );

            // Assert that the POST request was successful
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to add food outlet.");

            // Act: Send a GET request to retrieve all food outlets
            var getResponse = await _client.GetAsync("http://localhost:5038/api/foodoutlet");

            // Assert that the GET request was successful
            Assert.IsTrue(getResponse.IsSuccessStatusCode, "Failed to get food outlets.");

            // Deserialize the response body into a list of food outlets
            var actualOutlets = await getResponse.Content.ReadFromJsonAsync<List<FoodOutlet>>();

            var expectedOutlet = new FoodOutlet
            {
                Name = newOutlet.name,
                Location = newOutlet.location
            };

            // Assert: Check if the new outlet was added
            Assert.IsTrue(actualOutlets != null && actualOutlets.Any(outlet =>
                string.Equals(outlet.Name, expectedOutlet.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(outlet.Location, expectedOutlet.Location, StringComparison.OrdinalIgnoreCase)
            ), "New outlet was not added correctly.");
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
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "Expected status code to be 401 Unauthorized.");
        }

        [TestMethod]
        public async Task GetReviews_ShouldReturnReviews()
        {
            // Act: Send a GET request to retrieve all reviews
            var response = await _client.GetAsync("http://localhost:5038/reviews");

            // Assert that the GET request was successful
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to get reviews.");

            // Deserialize the response body into a list of reviews
            var reviews = await response.Content.ReadFromJsonAsync<List<Review>>();

            var expectedReview1 = new Review
            {
                FoodOutlet = new FoodOutlet { Id = 8, Name = "Kebab King 4", Location = "somewhere" },
                Id = 10,
                Comment = "Postman Reviews i have an idea!",
                Score = 3,
                CreatedAt = DateTime.Parse("2025-01-28T01:33:40")
            };

            var expectedReview2 = new Review
            {
                FoodOutlet = new FoodOutlet { Id = 8, Name = "Kebab King 4", Location = "somewhere"},
                Id = 12,
                Comment = "testing sending review from the spa",
                Score = 4,
                CreatedAt = DateTime.Parse("2025-01-28T18:45:26")
            };

            // Assert: Check if at least one review matches the expected structure
            var review1Matches = reviews != null && reviews.Any(r =>
                r.Id == expectedReview1.Id &&
                r.FoodOutlet != null &&
                r.FoodOutlet.Id == expectedReview1.FoodOutlet.Id &&
                r.FoodOutlet.Name == expectedReview1.FoodOutlet.Name &&
                r.Comment == expectedReview1.Comment &&
                r.Score == expectedReview1.Score &&
                r.CreatedAt == expectedReview1.CreatedAt);

            var review2Matches = reviews != null && reviews.Any(r =>
                r.Id == expectedReview2.Id &&
                r.FoodOutlet != null &&
                r.FoodOutlet.Id == expectedReview2.FoodOutlet.Id &&
                r.FoodOutlet.Name == expectedReview2.FoodOutlet.Name &&
                r.Comment == expectedReview2.Comment &&
                r.Score == expectedReview2.Score &&
                r.CreatedAt == expectedReview2.CreatedAt);

            Assert.IsTrue(review1Matches, "Review 1 was not added correctly.");
            Assert.IsTrue(review2Matches, "Review 2 was not added correctly.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose of HttpClient after the test
            _client.Dispose();
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
