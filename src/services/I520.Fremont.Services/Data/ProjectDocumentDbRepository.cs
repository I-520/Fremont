using I520.Fremont.Services.Models;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace I520.Fremont.Services.Data
{
    public class ProjectDocumentDbRepository : IProjectRepository
    {
        private readonly string _endpointUri;
        private readonly string _accountKey;

        public ProjectDocumentDbRepository(string endpointUri, string accountKey)
        {
            _endpointUri = endpointUri;
            _accountKey = accountKey;
        }

        public List<Project> GetAllProjects()
        {
            using (var client = new DocumentClient(new Uri(_endpointUri), _accountKey))
            {
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

                var query = client.CreateDocumentQuery<Project>(
                    UriFactory.CreateDocumentCollectionUri("Fremont", "Projects"),
                    queryOptions);

                return query.ToList();
            }
        }
    }
}
