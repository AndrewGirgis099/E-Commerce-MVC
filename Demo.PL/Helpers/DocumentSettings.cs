using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Demo.PL.Helpers
{
    public class DocumentSettings
    {
        public static string UploadFile(IFormFile file , string folderName)
        {
            //1. Get Located Folder Path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", folderName);

            //2. get fileName and make it unique
            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            //3. get file path  [folderPath +  FileName]
            string filePath = Path.Combine(folderPath, fileName);

            //4. save file as stream
            using var fileStream = new FileStream(filePath, FileMode.Create);

            file.CopyTo(fileStream);
            return fileName;


        }

        public static void Delete (string fileName , string folderName)
        {
            //1. get file path
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", folderName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
