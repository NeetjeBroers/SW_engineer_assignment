using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Azure.Data.Tables;
using static Helpers.AzureTableHelper;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;

namespace SW_engineer_assignment
{
    public static class PostEquipmentStatus
    {
        [FunctionName("PostEquipmentStatus")]
        [OpenApiOperation(operationId: "PostEquipmentStatus", tags: new[] { "Post Equipment Status" })]
        [OpenApiRequestBody("application/json", typeof(EquipmentStatus), Description = "Equipment status to be updated or created")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EquipmentStatus), Description = "Successfully updated or created equipment status")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Invalid input")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "status")] HttpRequest httpRequest,
            [Table("EquipmentStatus", Connection = "EquipmentStorage")] TableClient equipmentTable)
        {
            try
            {
                var equipmentStatus = await GetEquipmentStatusFromBody(httpRequest.Body);
                var errorResult = ValidateRequestFields(equipmentStatus);
                if (errorResult.Status != 200)
                {
                    return new ObjectResult(errorResult) { StatusCode = errorResult.Status };
                }
                var insertOrUpdateEquipmentResponse = UpsertEquipmentStatus(equipmentTable, EquipmentStatusToEquipmentStatusTableEntity(equipmentStatus));
                if (insertOrUpdateEquipmentResponse.Status == TaskStatus.Faulted)
                {
                    throw insertOrUpdateEquipmentResponse.Exception;
                }
                return new OkObjectResult(equipmentStatus);
            }
            catch (Exception exception)
            {
                return new ObjectResult(exception) { StatusCode = 500 };
            }
        }

        private static ErrorResult ValidateRequestFields(EquipmentStatus equipmentStatus)
        {
            if (equipmentStatus == null)
            {
                return new ErrorResult { Status = 400, ErrorCode = "InvalidRequest", Message = "No request body not found" };
            }
            if (string.IsNullOrEmpty(equipmentStatus.Id))
            {
                return new ErrorResult { Status = 400, ErrorCode = "InvalidRequest", Message = "Id not found in posted request" };
            }
            if (string.IsNullOrEmpty(equipmentStatus.Status))
            {
                return new ErrorResult { Status = 400, ErrorCode = "InvalidRequest", Message = "Status not found in posted request" };
            }
            return new ErrorResult { Status = 200 };
        }

        private static EquipmentStatusTableEntity EquipmentStatusToEquipmentStatusTableEntity(EquipmentStatus equipmentStatus)
        {
            return new EquipmentStatusTableEntity
            {
                PartitionKey = "",
                RowKey = equipmentStatus.Id,
                Status = equipmentStatus.Status
            };
        }

        private static async Task<EquipmentStatus> GetEquipmentStatusFromBody(Stream body)
        {
            var requestBody = await new StreamReader(body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<EquipmentStatus>(requestBody);
        }

        public class ErrorResult
        {
            public int Status { get; set; }
            public string ErrorCode { get; set; }
            public string Message { get; set; }
        }
    }
}