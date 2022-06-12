using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTraceAnalysis.Cosmos
{
    public class CosmosSqlRepository :  ICosmosSqlRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosSqlRepository(CosmosClient cosmosClient, string databaseId, string containerId)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<T> ReadItemAsync<T>(string documentId, string partitionKey)
        {
            ItemResponse<T> entityResponse = await _container.ReadItemAsync<T>(documentId, new PartitionKey(partitionKey));
            return entityResponse;
        }

        public async Task<List<T>> ReadItemsAsync<T>(string partitionKey)
        {
            string queryByPartition = $"select * from c where c.ComponentName = '{partitionKey}'";
            QueryDefinition queryDefinition = new QueryDefinition(queryByPartition);

            List<T> healthStatusItems = new List<T>();

            using (FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(queryDefinition))
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        healthStatusItems.Add(item);
                    }
                }
            }

            return healthStatusItems;
        }

        public async Task<Tuple<ItemResponse<T>, bool>> CreateItemAsync<T>(T entity, string documentId ="")
        {
            ItemResponse<T> itemResponse = await _container.CreateItemAsync(entity);
            return new Tuple<ItemResponse<T>, bool>(itemResponse, itemResponse.StatusCode == System.Net.HttpStatusCode.Created);
                
        }

        public async Task<Tuple<ItemResponse<T>, bool>> UpsertItemAsync<T>(T entity, string documentId="")
        {
            try
            {
                ItemResponse<T> itemResponse = await _container.UpsertItemAsync<T>(entity);
                return new Tuple<ItemResponse<T>, bool>(itemResponse, 
                    (itemResponse.StatusCode == System.Net.HttpStatusCode.Created ||
                    itemResponse.StatusCode == System.Net.HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Tuple<ItemResponse<T>, bool>> ReplaceItemAsync<T>(T entity, string documentId="")
        {
            ItemResponse<T> itemResponse = await _container.ReplaceItemAsync<T>(entity, documentId);
            return new Tuple<ItemResponse<T>, bool>(itemResponse, itemResponse.StatusCode == System.Net.HttpStatusCode.OK);

        }

        public async Task<Tuple<ItemResponse<T>, bool>> DeleteItemAsync<T>(string documentId, PartitionKey partitionKey)
        {
            ItemResponse<T> itemResponse = await _container.DeleteItemAsync<T>(documentId, partitionKey);
            return new Tuple<ItemResponse<T>, bool>(itemResponse, itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent);
            
        }
    }
}
