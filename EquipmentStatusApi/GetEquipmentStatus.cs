using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using static Helpers.AzureTableHelper;

namespace EquipmentStatusApi
{
    /// <summary>
    /// Azure Function to get the status of a specific equipment by its ID from Azure Table Storage.
    /// </summary>
    public static class GetEquipmentStatus
    {
        /// <summary>
        /// HTTP GET method to retrieve the status of a specific equipment by its ID.
        /// </summary>
        /// <param name="httpRequest">The HTTP request triggering the function.</param>
        /// <param name="equipmentStatusTable">The Azure Table client connected to the EquipmentStatus table.</param>
        /// <param name="id">The ID of the equipment whose status is being retrieved.</param>
        /// <returns>HTTP response with the equipment status or error information.</returns>
        [FunctionName("GetEquipmentStatus")]
        [OpenApiOperation(operationId: "GetEquipmentStatus", tags: ["Get Equipment Status"])]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "ID of the equipment")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EquipmentStatus), Description = "Successful response with equipment status")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Resource not found")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Internal server error")]

        public static async Task<IActionResult> Run(
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "status/{id}")] HttpRequest httpRequest,
#pragma warning restore IDE0060 // Remove unused parameter
            [Table("EquipmentStatus", Connection = "EquipmentStorage")] TableClient equipmentStatusTable,
            string id)
        {
            try
            {
                // Retrieve the equipment status by ID from the EquipmentStatus table
                var equipmentStatusReponse = await GetEquipmentStatusById(equipmentStatusTable, id.ToString());
                // Return the equipment status as an OK (200) response
                return new OkObjectResult(equipmentStatusReponse);
            }
            catch (RequestFailedException exception)
            {
                // Return a response with the exception details if a RequestFailedException occurs example: 404 Equipment Not Found
                return new ObjectResult(exception) { StatusCode = exception.Status };
            }
            catch (Exception exception)
            {
                // Return a response with the exception details for any other exceptions
                return new ObjectResult(exception) { StatusCode = 500 };
            }
        }
    }
}
