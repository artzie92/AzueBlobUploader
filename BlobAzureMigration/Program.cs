using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;

namespace BlobAzureMigration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }

    internal class Bootstrapper
    {
        private readonly string storageAccountConnection = CloudConfigurationManager.GetSetting("StorageConnectionString");
        private readonly string directoryPath = ConfigurationManager.AppSettings.Get("DirectoryPath");
        private readonly string containerName = ConfigurationManager.AppSettings.Get("ContainerName");

        public void Run()
        {
            var files = GetFiles();

            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine("Start uploading file number: {0} / {1}", i + 1, files.Length);
                    UploadFileToAzureBlob(files[i]);
                    Console.WriteLine("Done");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        public void UploadFileToAzureBlob(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            using (var stream = File.OpenRead(filePath))
            {
                var storageAccount = CloudStorageAccount.Parse(storageAccountConnection);
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference(containerName);
                CloudBlockBlob blockblob = container.GetBlockBlobReference(fileName);
                blockblob.UploadFromStream(stream);
            }
        }

        private string[] GetFiles()
        {
            var files = Directory.GetFiles(directoryPath);
            return files;
        }
    }
}