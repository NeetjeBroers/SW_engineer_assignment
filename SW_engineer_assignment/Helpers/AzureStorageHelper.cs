using Azure;
using Azure.Data.Tables;
using System;
using System.Threading.Tasks;

namespace Helpers
{
    public static class AzureTableHelper
    {
        internal static async Task<Response> UpsertEquipmentStatus(TableClient equipmentTableClient, EquipmentStatusTableEntity equipmentStatusTableEntity)
        {
            return await equipmentTableClient.UpsertEntityAsync(equipmentStatusTableEntity);
        }

        public class EquipmentStatusTableEntity : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Status { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
        }

        public class EquipmentStatus
        {
            public string Id { get; set; }
            public string Status { get; set; }
        }
    }
}
