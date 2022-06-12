using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseTraceAnalysis.Cosmos
{
    public interface ICosmosSqlRepository
    {
        Task<Tuple<ItemResponse<T>, bool>> CreateItemAsync<T>(T entity, string documentId="");
        Task<Tuple<ItemResponse<T>, bool>> DeleteItemAsync<T>(string documentId, PartitionKey partitionKey);
        Task<T> ReadItemAsync<T>(string documentId, string partitionKey);
        Task<List<T>> ReadItemsAsync<T>(string partitionKey);
        Task<Tuple<ItemResponse<T>, bool>> ReplaceItemAsync<T>(T entity, string documentId = "");
        Task<Tuple<ItemResponse<T>, bool>> UpsertItemAsync<T>(T entity, string documentId = "");
    }
}