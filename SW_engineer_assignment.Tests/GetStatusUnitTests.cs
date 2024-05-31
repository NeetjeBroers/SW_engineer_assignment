using System.Net;
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
            var Id = "1";
            var partitionKey = "";
            var equipmentStatusTableEntity = new EquipmentStatusTableEntity
            {
                PartitionKey = "",
                RowKey = Id,
                Status = "Active",
                Timestamp = DateTimeOffset.Now
            };
            var mockTableClient = new Mock<TableClient>();
            var response = Response.FromValue(equipmentStatusTableEntity, Mock.Of<Response>());
            mockTableClient.Setup(item => item.GetEntityAsync<EquipmentStatusTableEntity>(partitionKey, Id, null, default))
                           .ReturnsAsync(response);
            var mockHttpRequest = CreateMockHttpRequest(Id);
            // Act
            var result = await GetStatus.Run(mockHttpRequest.Object, mockTableClient.Object, Id);
            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedEquipmentStatus = Assert.IsType<EquipmentStatus>(okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.Equal(Id, returnedEquipmentStatus.Id);
            Assert.Equal("Active", returnedEquipmentStatus.Status);
        }

        [Fact]
        public async Task Run_ReturnsNotFoundResult_WhenEquipmentStatusIsNotFound()
        {
            // Arrange
            var Id = "1";
            var partitionKey = "";
            var equipmentStatusTableEntity = new EquipmentStatusTableEntity
            {
                PartitionKey = "",
                RowKey = Id,
                Status = "Active",
                Timestamp = DateTimeOffset.Now
            };
            var mockTableClient = new Mock<TableClient>();         
            var responseBody = "{\r\n    \"Status\": 404,\r\n    \"ErrorCode\": \"ResourceNotFound\",\r\n    \"ClassName\": \"Azure.RequestFailedException\",\r\n    \"Message\": \"The specified resource does not exist.\\nRequestId:525d0c23-f002-0027-7a3a-b36d86000000\\nTime:2024-05-31T09:09:13.5169208Z\\r\\nStatus: 404 (Not Found)\\r\\nErrorCode: ResourceNotFound\\r\\n\\r\\nContent:\\r\\n{\\\"odata.error\\\":{\\\"code\\\":\\\"ResourceNotFound\\\",\\\"message\\\":{\\\"lang\\\":\\\"en-US\\\",\\\"value\\\":\\\"The specified resource does not exist.\\\\nRequestId:525d0c23-f002-0027-7a3a-b36d86000000\\\\nTime:2024-05-31T09:09:13.5169208Z\\\"}}}\\r\\n\\r\\nHeaders:\\r\\nCache-Control: no-cache\\r\\nTransfer-Encoding: chunked\\r\\nServer: Windows-Azure-Table/1.0 Microsoft-HTTPAPI/2.0\\r\\nx-ms-request-id: 525d0c23-f002-0027-7a3a-b36d86000000\\r\\nx-ms-client-request-id: 225ac987-5dd2-408c-a708-af0a34b41127\\r\\nx-ms-version: REDACTED\\r\\nX-Content-Type-Options: REDACTED\\r\\nDate: Fri, 31 May 2024 09:09:13 GMT\\r\\nContent-Type: application/json;odata=minimalmetadata;streaming=true;charset=utf-8\\r\\n\",\r\n    \"Data\": null,\r\n    \"InnerException\": null,\r\n    \"HelpURL\": null,\r\n    \"StackTraceString\": \"   at Azure.Core.HttpPipelineExtensions.ProcessMessageAsync(HttpPipeline pipeline, HttpMessage message, RequestContext requestContext, CancellationToken cancellationToken)\\r\\n   at Azure.Data.Tables.TableRestClient.QueryEntityWithPartitionAndRowKeyAsync(String table, String partitionKey, String rowKey, Nullable`1 timeout, String format, String select, String filter, RequestContext context)\\r\\n   at Azure.Data.Tables.TableClient.GetEntityInternalAsync[T](Boolean async, String partitionKey, String rowKey, Boolean noThrow, IEnumerable`1 select, CancellationToken cancellationToken)\\r\\n   at Azure.Data.Tables.TableClient.GetEntityAsync[T](String partitionKey, String rowKey, IEnumerable`1 select, CancellationToken cancellationToken)\\r\\n   at Helpers.AzureTableHelper.GetEquipmentStatusById(TableClient equipmentStatusTableClient, String id) in C:\\\\Users\\\\NBROERS\\\\source\\\\repos\\\\SW_engineer_assignment\\\\SW_engineer_assignment\\\\Helpers\\\\AzureStorageHelper.cs:line 17\\r\\n   at SW_engineer_assignment.GetStatus.Run(HttpRequest httpRequest, TableClient equipmentStatusTable, String id) in C:\\\\Users\\\\NBROERS\\\\source\\\\repos\\\\SW_engineer_assignment\\\\SW_engineer_assignment\\\\GetStatus.cs:line 26\",\r\n    \"RemoteStackTraceString\": null,\r\n    \"RemoteStackIndex\": 0,\r\n    \"ExceptionMethod\": null,\r\n    \"HResult\": -2146233088,\r\n    \"Source\": \"Azure.Data.Tables\",\r\n    \"WatsonBuckets\": null\r\n}";
            mockTableClient.Setup(item => item.GetEntityAsync<EquipmentStatusTableEntity>(partitionKey, Id, null, default))
                         .ThrowsAsync(new RequestFailedException(404, responseBody));
            var mockHttpRequest = CreateMockHttpRequest(Id);
            // Act
            var result = await GetStatus.Run(mockHttpRequest.Object, mockTableClient.Object, Id);
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
            Assert.Contains(responseBody, objectResult.Value.ToString());
        }

        private static Mock<HttpRequest> CreateMockHttpRequest(string id)
        {
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(req => req.Method).Returns("GET");
            mockHttpRequest.Setup(req => req.Path).Returns(new PathString($"/api/status/{id}"));
            mockHttpRequest.Setup(req => req.Scheme).Returns("http");
            return mockHttpRequest;
        }
    }
}