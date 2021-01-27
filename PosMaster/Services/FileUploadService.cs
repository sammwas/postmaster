using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PosMaster.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<FormUploadReturnData> UploadAsync(Guid clientId, IFormFile file, string currentPath = "")
        {
            var res = new FormUploadReturnData();
            try
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var folder = Path.Combine("uploads", $"{clientId}");
                if (file == null)
                {
                    res.Message = "No file provided";
                    return res;
                }
                var fileName = file.FileName;
                var ext = Path.GetExtension(fileName);
                if (!AllowedExtensions().Contains(ext.ToLower()))
                {
                    res.Message = $"Extension {ext} is not Allowed";
                    return res;
                }

                var size = file.Length;
                if (size > 1 * 1000000)
                {
                    res.Message = $"Ensure file size is below 9Mbs";
                    return res;
                }

                var filePath = Path.Combine(webRootPath, folder);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(filePath, uniqueFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(currentPath))
                {
                    try
                    {
                        var cFile = Path.Combine(webRootPath, currentPath);
                        File.Delete(cFile);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine($"Failed to delete file at {currentPath}");
                    }
                }

                var fileUrl = Path.Combine(folder, uniqueFileName);

                return new FormUploadReturnData
                {
                    Success = true,
                    Message = "Uploaded",
                    Size = size,
                    Extension = ext,
                    PathUrl = fileUrl,
                    Name = fileName
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                res.Message = "File upload Error Occured. Try again later";
                return res;
            }
        }

        public static List<string> AllowedExtensions()
        {
            return new List<string> { ".png", ".jpg", ".jpeg" };
        }
    }
}
