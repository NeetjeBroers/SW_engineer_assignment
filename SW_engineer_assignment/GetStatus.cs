using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static Helpers.AzureTableHelper;

namespace SW_engineer_assignment
{
    public static class GetStatus
    {       
        [FunctionName("GetStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "status/{id}")] HttpRequest httpRequest,            
            [Table("EquipmentStatus", Connection = "EquipmentStorage")] TableClient equipmentStatusTable,
            ExecutionContext context,
            string id,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function {context.FunctionName} processed a request.");
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
