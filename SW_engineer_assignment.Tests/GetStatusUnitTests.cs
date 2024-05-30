using System.Text;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using SW_engineer_assignment;
using Xunit;
using static Helpers.AzureTableHelper;

namespace EquipmentStatusFunctionTests
{
    public class GetStatusFunctionTests
    {
        [Fact]
        public async Task Run_ReturnsOkResult_WhenEquipmentStatusIsFound()
        {
            // Arrange
            var equipmentId = "1";
            var Id = "";
            var equipmentStatusTableEntity = new EquipmentStatusTableEntity
            {
                PartitionKey = "",
                RowKey = equipmentId,
                Status = "Active",
                Timestamp = DateTimeOffset.Now
            };
            var mockTableClient = new Mock<TableClient>();
            var response = Response.FromValue(equipmentStatusTableEntity, Mock.Of<Response>());
            mockTableClient.Setup(item => item.GetEntityAsync<EquipmentStatusTableEntity>(Id, equipmentId, null, default))
                           .ReturnsAsync(response);
            var mockHttpRequest = CreateMockHttpRequest(equipmentId);
            // Act
            var result = await GetStatus.Run(mockHttpRequest.Object, mockTableClient.Object, equipmentId);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedStatus = Assert.IsType<EquipmentStatus>(okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.Equal(equipmentId, returnedStatus.Id);
            Assert.Equal("Active", returnedStatus.Status);            
        }
      
        private Mock<HttpRequest> CreateMockHttpRequest(string id)
        {
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(req => req.Method).Returns("GET");
            mockHttpRequest.Setup(req => req.Path).Returns(new PathString($"/api/status/{id}"));
            //mockHttpRequest.Setup(req => req.Host).Returns(new HostString("localhost", 7024));
            mockHttpRequest.Setup(req => req.Scheme).Returns("http");
            return mockHttpRequest;
        }
    }
}