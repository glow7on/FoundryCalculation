using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace CloudStorageApp
{
    class Program
    {
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