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

namespace EquipmentStatusApi
{
    /// <summary>
    /// Azure Function to create or update the status of an equipment in Azure Table Storage.
    /// </summary>
    public static class PostEquipmentStatus
    {
        /// <summary>
        /// HTTP POST method to create or update the status of an equipment.
        /// </summary>
        /// <param name="httpRequest">The HTTP request triggering the function.</param>
        /// <param name="equipmentTable">The Azure Table client connected to the EquipmentStatus table.</param>
        /// <returns>HTTP response with the created or updated equipment status or error information.</returns>
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
                // Deserialize the request body to get the equipment status
                var equipmentStatus = await GetEquipmentStatusFromBody(httpRequest.Body);
                // Validate the deserialized equipment status object
                var errorResult = ValidateRequestFields(equipmentStatus);
                if (errorResult.Status != 200)
                {
                    return new ObjectResult(errorResult) { StatusCode = errorResult.Status };
                }
                // Insert or update the equipment status in the table
                var insertOrUpdateEquipmentResponse = UpsertEquipmentStatus(equipmentTable, EquipmentStatusToEquipmentStatusTableEntity(equipmentStatus));
                // Return error when insert or update fails
                if (insertOrUpdateEquipmentResponse.Status == TaskStatus.Faulted)
                {
                    throw insertOrUpdateEquipmentResponse.Exception;
                }
                // Return the created or updated equipment status
                return new OkObjectResult(equipmentStatus);
            }
            catch (Exception exception)
            {
                // Return a 500 Internal Server Error response if an exception occurs
                return new ObjectResult(exception) { StatusCode = 500 };
            }
        }

        /// <summary>
        /// Validates the fields of the provided EquipmentStatus object.
        /// </summary>
        /// <param name="equipmentStatus">The equipment status to validate.</param>
        /// <returns>An ErrorResult object indicating the result of the validation.</returns>
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

        /// <summary>
        /// Converts an EquipmentStatus object to an EquipmentStatusTableEntity object for storage in the Azure Table.
        /// </summary>
        /// <param name="equipmentStatus">The equipment status to convert.</param>
        /// <returns>An EquipmentStatusTableEntity object.</returns>
        private static EquipmentStatusTableEntity EquipmentStatusToEquipmentStatusTableEntity(EquipmentStatus equipmentStatus)
        {
            return new EquipmentStatusTableEntity
            {
                PartitionKey = "",
                RowKey = equipmentStatus.Id,
                Status = equipmentStatus.Status
            };
        }

        /// <summary>
        /// Reads and deserializes the equipment status from the request body.
        /// </summary>
        /// <param name="body">The request body stream.</param>
        /// <returns>The deserialized EquipmentStatus object.</returns>
        private static async Task<EquipmentStatus> GetEquipmentStatusFromBody(Stream body)
        {
            var requestBody = await new StreamReader(body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<EquipmentStatus>(requestBody);
        }

        /// <summary>
        /// Class representing an error result.
        /// </summary>
        public class ErrorResult
        {
            public int Status { get; set; }
            public string ErrorCode { get; set; }
            public string Message { get; set; }
        }
    }
}
