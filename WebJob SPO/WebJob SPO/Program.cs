using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

namespace WebJob_SPO
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    internal class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
            static async Task Main(string[] args)
            {
                // Подключение к облачному хранилищу
                string storageConnectionString = "Ваша строка подключения";
                CloudStorageAccount storageAccount;
                if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                {
                    // Создание клиента для работы с Blob
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Получение ссылки на контейнер
                    CloudBlobContainer container = blobClient.GetContainerReference("имя-вашего-контейнера");

                    // Создание контейнера, если он не существует
                    await container.CreateIfNotExistsAsync();

                    // Получение ссылки на Blob
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference("имя-файла.txt");

                    // Загрузка файла
                    await blockBlob.UploadFromFileAsync("путь-к-файлу.txt");
                    Console.WriteLine("Файл успешно загружен в облако!");
                }
                else
                {
                    Console.WriteLine("Ошибка: Невозможно подключиться к облачному хранилищу.");
                }
            }
        }
    }
}
}
