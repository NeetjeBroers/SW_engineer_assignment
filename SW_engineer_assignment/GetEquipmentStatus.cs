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

namespace SW_engineer_assignment
{
    public static class GetEquipmentStatus
    {
        [FunctionName("GetEquipmentStatus")]
        [OpenApiOperation(operationId: "GetEquipmentStatus", tags: new[] { "Get Equipment Status" })]
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
                var equipmentStatusReponse = await GetEquipmentStatusById(equipmentStatusTable, id.ToString());
                return new OkObjectResult(equipmentStatusReponse);
            }
            catch (RequestFailedException exception)
            {
                return new ObjectResult(exception) { StatusCode = exception.Status };
            }
            catch (Exception exception)
            {
                return new ObjectResult(exception) { StatusCode = 500 };
            }
        }
    }
}
