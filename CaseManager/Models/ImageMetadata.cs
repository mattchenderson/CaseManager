﻿using Azure;
using Azure.Data.Tables;
using System;

namespace CaseManager.Models
{
    public class ImageMetadata : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;


        public string Title { get; set; }

        public string Description { get; set; }

        public string ContentUri { get; set; }

    }
}