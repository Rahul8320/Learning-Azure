using Microsoft.AspNetCore.Http;

namespace HttpTriggerFunction.Services.IServices;

public interface IBlobStorageService
{
    Task<Uri> UploadPdfIntoBlobContainer(string name, IFormFile pdfFile);
}
