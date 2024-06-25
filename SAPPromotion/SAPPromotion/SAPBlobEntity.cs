﻿using Microsoft.Azure.Storage.Blob;
using System;

namespace SAPPromotion
    {
    public class SAPBlobEntity
    {
        public CloudBlockBlob Blob { get; set; }
        public string BlobName { get; set; }

        public string FileName { get; set; }

        public string Status { get; set; }

        public string FileData { get; set; }

        public DateTime FileCreatedDate { get; set; }
    }
}
