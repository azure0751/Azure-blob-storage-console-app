namespace Azure_blob_storage_console_app
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Sas;
    using System;
    using System.Collections.Generic;

    class Program
    {
        public static string connectionString =string.Empty;

        static void Main(string[] args)
        {
            Console.Write("Enter storage account connection string: ");
            connectionString = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("\nAzure Blob Storage Console Application");
                Console.WriteLine("1. Upload Blob");
                Console.WriteLine("2. List Blobs");
                Console.WriteLine("3. Download Blob");
                Console.WriteLine("4. Delete Blob");
                Console.WriteLine("5. Create Container");
                Console.WriteLine("6. Delete Container");
                Console.WriteLine("7. Set Metadata to Container");
                Console.WriteLine("8. Get Metadata of Container");
                Console.WriteLine("9. Generate SAS for Container");
                Console.WriteLine("10. Generate SAS for Blob");
                Console.WriteLine("11. Exit");
                Console.Write("Enter your choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        UploadBlob();
                        break;
                    case "2":
                        ListBlobs();
                        break;
                    case "3":
                        DownloadBlob();
                        break;
                    case "4":
                        DeleteBlob();
                        break;
                    case "5":
                        CreateContainer();
                        break;
                    case "6":
                        DeleteContainer();
                        break;
                    case "7":
                        SetContainerMetadata();
                        break;
                    case "8":
                        GetContainerMetadata();
                        break;
                    case "9":
                        GenerateContainerSas();
                        break;
                    case "10":
                        GenerateBlobSas();
                        break;
                    case "11":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static void UploadBlob()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            Console.Write("Enter file path to upload: ");
            string filePath = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            containerClient.CreateIfNotExists();

            BlobClient blobClient = containerClient.GetBlobClient(System.IO.Path.GetFileName(filePath));
            blobClient.Upload(filePath, overwrite: true);

            Console.WriteLine("File uploaded successfully.");
        }

        static void ListBlobs()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            containerClient.CreateIfNotExists();

            Console.WriteLine("Listing blobs...");

            foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine(blobItem.Name);
            }
        }

        static void DownloadBlob()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            Console.Write("Enter the name of the blob to download: ");
            string blobName = Console.ReadLine();
            Console.Write("Enter the path to save the downloaded file: ");
            string downloadPath = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            blobClient.DownloadTo(downloadPath);

            Console.WriteLine($"Blob {blobName} downloaded successfully.");
        }

        static void DeleteBlob()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            Console.Write("Enter the name of the blob to delete: ");
            string blobName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            blobClient.DeleteIfExists();

            Console.WriteLine($"Blob {blobName} deleted successfully.");
        }

        static void CreateContainer()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            containerClient.CreateIfNotExists();

            Console.WriteLine($"Container '{containerName}' created successfully.");
        }

        static void DeleteContainer()
        {
            Console.Write("Enter container name to delete: ");
            string containerName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            containerClient.DeleteIfExists();

            Console.WriteLine($"Container '{containerName}' deleted successfully.");
        }

        static void SetContainerMetadata()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

            Dictionary<string, string> metadata = new Dictionary<string, string>();
            Console.WriteLine("Enter metadata key-value pairs (enter empty key to finish):");

            while (true)
            {
                Console.Write("Key: ");
                string key = Console.ReadLine();
                if (string.IsNullOrEmpty(key)) break;

                Console.Write("Value: ");
                string value = Console.ReadLine();
                metadata[key] = value;
            }

            containerClient.SetMetadata(metadata);
            Console.WriteLine("Metadata set successfully.");
        }

        static void GetContainerMetadata()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

            BlobContainerProperties properties = containerClient.GetProperties();
            Console.WriteLine("Metadata for container:");
            foreach (var metadataItem in properties.Metadata)
            {
                Console.WriteLine($"{metadataItem.Key}: {metadataItem.Value}");
            }
        }

        static void GenerateContainerSas()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write);

            Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
            Console.WriteLine($"SAS Token for container '{containerName}': {sasUri}");
        }

        static void GenerateBlobSas()
        {
            Console.Write("Enter container name: ");
            string containerName = Console.ReadLine();

            Console.Write("Enter blob name: ");
            string blobName = Console.ReadLine();

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            Console.WriteLine($"SAS Token for blob '{blobName}': {sasUri}");
        }
    }

}
