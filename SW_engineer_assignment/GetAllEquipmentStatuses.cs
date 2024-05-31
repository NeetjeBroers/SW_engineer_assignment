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
    public static class GetAllEquipmentStatuses
    {
        [FunctionName("GetAllEquipmentStatuses")]
        [OpenApiOperation(operationId: "GetAllEquipmentStatuses", tags: ["Get All Statuses"])]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<EquipmentStatus>), Description = "Successfully retrieved equipment statuses")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Internal server error")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "status")] HttpRequest httpRequest,
             [Table("EquipmentStatus", Connection = "EquipmentStorage")] TableClient equipmentStatusTable)
        {
            var statusResults = equipmentStatusTable.QueryAsync<EquipmentStatusTableEntity>();
            var equipmentStatusList = new List<EquipmentStatus>();
            await foreach (var statusResult in statusResults)
            {
                equipmentStatusList.Add(new EquipmentStatus()
                {
                    Id = statusResult.RowKey,
                    Status = statusResult.Status
                });
            }
            var response = JsonConvert.SerializeObject(equipmentStatusList);
            return new OkObjectResult(response);
        }
    }
}
