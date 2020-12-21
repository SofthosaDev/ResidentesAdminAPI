using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuAdelanto.Helpers;


using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace PerrosApp.Classes
{
    public class gdrive
    {
        private readonly AppSettings _appSettings;
        public gdrive(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }


        public string SubirArchivo(System.IO.FileStream fileStream, string nombre, string contentType) {

            DriveService  service = new DriveService(new BaseClientService.Initializer()
            {
                ApplicationName = "My First Project",
                ApiKey = _appSettings.GDriveKey,
            });

            var fileMetadata = new File()
            {
                Name = nombre
            };

            FilesResource.CreateMediaUpload req = service.Files.Create(fileMetadata, fileStream, contentType);
            req.Fields = "id";
            req.Upload();
            var file = req.ResponseBody;
            return file.Id;

        }
    }
}
