using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using static Helpers.AzureTableHelper;
using System.Collections.Generic;
using System.Transactions;

namespace SW_engineer_assignment
{
    public static class GetAllStatus
    {
        [FunctionName("GetAllStatus")]
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
