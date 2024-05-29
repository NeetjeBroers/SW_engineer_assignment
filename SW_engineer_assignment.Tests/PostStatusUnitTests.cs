using System.Text;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using SW_engineer_assignment;
using Xunit;

namespace EquipmentStatusFunctionTests
{
    public class EquipmentStatusFunctionTests
    {
        [Fact]
        public async Task Run_ReturnsOkResult_WhenRequestIsValid()
        {
            // Arrange
            var equipmentStatus = new EquipmentStatus() { Id = "1", Status = "New" };
            var validEquipmentStatusJson = JsonConvert.SerializeObject(equipmentStatus);
            Mock<HttpRequest> mockHttpRequest = CreateMockHttpRequest(validEquipmentStatusJson);
            var mockTableClient = new Mock<TableClient>();
            // Act
            var result = await PostStatus.Run(mockHttpRequest.Object, mockTableClient.Object);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public async Task Run_ReturnsObjectResult_WhenRequestIsSentWithoutStatus()
        {
            // Arrange
            var equipmentStatus = new EquipmentStatusWithoutStatus() { Id = "1" };
            var validEquipmentStatusJson = JsonConvert.SerializeObject(equipmentStatus);
            Mock<HttpRequest> mockHttpRequest = CreateMockHttpRequest(validEquipmentStatusJson);
            var mockTableClient = new Mock<TableClient>();
            // Act
            var result = await PostStatus.Run(mockHttpRequest.Object, mockTableClient.Object);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var errorResultValue = (SW_engineer_assignment.PostStatus.ErrorResult)objectResult.Value;
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Status not found in posted request", errorResultValue.Message);
            Assert.Equal("InvalidRequest", errorResultValue.ErrorCode);
        }

        [Fact]
        public async Task Run_ReturnsObjectResult_WhenRequestIsSentWithoutId()
        {
            // Arrange
            var equipmentStatus = new EquipmentStatusWithoutId() { Status = "New" };
            var validEquipmentStatusJson = JsonConvert.SerializeObject(equipmentStatus);
            Mock<HttpRequest> mockHttpRequest = CreateMockHttpRequest(validEquipmentStatusJson);
            var mockTableClient = new Mock<TableClient>();
            // Act
            var result = await PostStatus.Run(mockHttpRequest.Object, mockTableClient.Object);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var errorResultValue = (SW_engineer_assignment.PostStatus.ErrorResult)objectResult.Value;
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Id not found in posted request", errorResultValue.Message);
            Assert.Equal("InvalidRequest", errorResultValue.ErrorCode);
        }

        private static Mock<HttpRequest> CreateMockHttpRequest(string body)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(item => item.Body).Returns(stream);
            return mockHttpRequest;
        }

        public class EquipmentStatus
        {
            public string? Id { get; set; }
            public string? Status { get; set; }
        }
        public class EquipmentStatusWithoutStatus
        {
            public string? Id { get; set; }
        }
        public class EquipmentStatusWithoutId
        {
            public string? Status { get; set; }
        }
    }
}