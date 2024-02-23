namespace HttpTriggerFunction.Services.IServices;

public interface IBlobStorageService
{
    Task<Uri> UploadPdfIntoBlobContainer(string name, Stream pdfStream);
}
