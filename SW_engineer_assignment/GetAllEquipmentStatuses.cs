using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Helpers.AzureTableHelper;

namespace EquipmentStatusApi
{
    /// <summary>
    /// Azure Function to get all equipment statuses from Azure Table Storage.
    /// </summary>
    public static class GetAllEquipmentStatuses
    {
        /// <summary>
        /// HTTP GET method to retrieve all equipment statuses.
        /// </summary>
        /// <param name="httpRequest">The HTTP request triggering the function.</param>
        /// <param name="equipmentStatusTable">The Azure Table client connected to the EquipmentStatus table.</param>
        /// <returns>HTTP response with a list of all equipment statuses.</returns>
        [FunctionName("GetAllEquipmentStatuses")]
        [OpenApiOperation(operationId: "GetAllEquipmentStatuses", tags: ["Get All Statuses"])]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<EquipmentStatus>), Description = "Successfully retrieved equipment statuses")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Internal server error")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "status")] HttpRequest httpRequest,
             [Table("EquipmentStatus", Connection = "EquipmentStorage")] TableClient equipmentStatusTable)
        {
            // Query the EquipmentStatus table asynchronously
            var statusResults = equipmentStatusTable.QueryAsync<EquipmentStatusTableEntity>();
            var equipmentStatusList = new List<EquipmentStatus>();
            // Iterate through the query results and convert them to EquipmentStatus objects
            await foreach (var statusResult in statusResults)
            {
                equipmentStatusList.Add(new EquipmentStatus()
                {
                    Id = statusResult.RowKey,
                    Status = statusResult.Status
                });
            }
            // Serialize the list of equipment statuses to JSON
            var response = JsonConvert.SerializeObject(equipmentStatusList);
            // Return the serialized list as an OK (200) response
            return new OkObjectResult(response);
        }
    }
}
