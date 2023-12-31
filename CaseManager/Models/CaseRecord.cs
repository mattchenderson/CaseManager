﻿using Azure;
using Azure.Data.Tables;
using System;

namespace CaseManager.Models
{
    public class CaseRecord : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;


        public string CustomerDescription { get; set; }

        public int? EstimateInHours { get; set; }    
   
    }
}