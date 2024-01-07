using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CMS.Web.Utils
{
    public class AttachmentHelper
    {
        public static async Task<FileStream> handleUpload(IFormFile file, string saveLocation)
        {
            var filePath = Path.Combine(saveLocation, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var attachmentStream = new FileStream(filePath, FileMode.Open);
            return attachmentStream;

        }
        public static byte[] ReadStream(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public static void removeFile(string fileName, string saveLocation)
        {
            var filePath = Path.Combine(saveLocation, fileName);
            File.Delete(filePath);
        }
    }
    
}
