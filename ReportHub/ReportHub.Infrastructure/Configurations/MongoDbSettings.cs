﻿namespace ReportHub.Infrastructure.Configurations
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ClientsCollectionName { get; set; }
    }
}
