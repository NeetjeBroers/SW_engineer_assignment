using System.Text;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using EquipmentStatusApi;
using Xunit;

namespace EquipmentStatusApiTests
{
    public class PostStatusFunctionTests
    {
        /// <summary>
        /// Test to verify that the function returns an OkObjectResult when a valid request is made.
        /// </summary>
        [Fact]
        public async Task Run_ReturnsOkResult_WhenRequestIsValid()
        {
            // Arrange
            var equipmentStatus = new EquipmentStatus() { Id = "1", Status = "New" };
            var validEquipmentStatusJson = JsonConvert.SerializeObject(equipmentStatus);
            Mock<HttpRequest> mockHttpRequest = CreateMockHttpRequest(validEquipmentStatusJson);
            var mockTableClient = new Mock<TableClient>();
            // Act
            var result = await PostEquipmentStatus.Run(mockHttpRequest.Object, mockTableClient.Object);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        /// <summary>
        /// Test to verify that the function returns a BadRequest result when the request is missing the Status field.
        /// </summary>
        [Fact]
        public async Task Run_ReturnsObjectResult_WhenRequestIsSentWithoutStatus()
        {
            // Arrange
            var equipmentStatus = new EquipmentStatusWithoutStatus() { Id = "1" };
            var validEquipmentStatusJson = JsonConvert.SerializeObject(equipmentStatus);
            Mock<HttpRequest> mockHttpRequest = CreateMockHttpRequest(validEquipmentStatusJson);
            var mockTableClient = new Mock<TableClient>();
            // Act
            var result = await PostEquipmentStatus.Run(mockHttpRequest.Object, mockTableClient.Object);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var errorResultValue = (PostEquipmentStatus.ErrorResult?)objectResult?.Value;
            Assert.Equal(400, objectResult?.StatusCode);
            Assert.Equal("Status not found in posted request", errorResultValue?.Message);
            Assert.Equal("InvalidRequest", errorResultValue?.ErrorCode);
        }

        /// <summary>
        /// Test to verify that the function returns a BadRequest result when the request is missing the Id field.
        /// </summary>
        [Fact]
        public async Task Run_ReturnsObjectResult_WhenRequestIsSentWithoutId()
        {
            // Arrange
            var equipmentStatus = new EquipmentStatusWithoutId() { Status = "New" };
            var validEquipmentStatusJson = JsonConvert.SerializeObject(equipmentStatus);
            Mock<HttpRequest> mockHttpRequest = CreateMockHttpRequest(validEquipmentStatusJson);
            var mockTableClient = new Mock<TableClient>();
            // Act
            var result = await PostEquipmentStatus.Run(mockHttpRequest.Object, mockTableClient.Object);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var errorResultValue = (PostEquipmentStatus.ErrorResult?)objectResult?.Value;
            Assert.Equal(400, objectResult?.StatusCode);
            Assert.Equal("Id not found in posted request", errorResultValue?.Message);
            Assert.Equal("InvalidRequest", errorResultValue?.ErrorCode);
        }

        /// <summary>
        /// Helper method to create a mock HttpRequest with a specified body.
        /// </summary>
        /// <param name="body">The body of the request.</param>
        /// <returns>A Mock of HttpRequest.</returns>
        private static Mock<HttpRequest> CreateMockHttpRequest(string body)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(item => item.Body).Returns(stream);
            return mockHttpRequest;
        }

        /// <summary>
        /// Represents an equipment status with Id and Status fields.
        /// </summary>
        public class EquipmentStatus
        {
            public string? Id { get; set; }
            public string? Status { get; set; }
        }

        /// <summary>
        /// Represents an equipment status without the Status field.
        /// </summary>
        public class EquipmentStatusWithoutStatus
        {
            public string? Id { get; set; }
        }

        /// <summary>
        /// Represents an equipment status without the Id field.
        /// </summary>
        public class EquipmentStatusWithoutId
        {
            public string? Status { get; set; }
        }
    }
}