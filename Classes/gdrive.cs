using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Json;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TuAdelanto.Helpers;

namespace PerrosApp.Classes
{
    public class gdrive
    {
        private string[] Scopes = { DriveService.Scope.Drive };
        DriveService service;

        private readonly AppSettings _appSettings;
        public gdrive(AppSettings appSettings)
        {
            _appSettings = appSettings;

        }

        private void conectar() {
            if (this.service == null) {
                GoogleCredential credencial;

                using (var stream = new FileStream("Auth/Perros-e53b746a265f.json", FileMode.Open, FileAccess.Read))
                {
                    credencial = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credencial,
                    ApplicationName = "Perros"
                });
                this.service = service;
            }
            
        }

        public async Task<string> GetMiniatura(string id) {
            conectar();
            FilesResource.GetRequest req = service.Files.Get(id);
            req.Fields = "hasThumbnail,thumbnailLink";
            Google.Apis.Drive.v3.Data.File file = req.Execute();
            string base64 = "";
            if (file.HasThumbnail.GetValueOrDefault()) {
                
                Stream res = await service.HttpClient.GetStreamAsync(file.ThumbnailLink);
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    res.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                base64 = Convert.ToBase64String(bytes);
            }
  

            return base64;
        }

        public async Task<Stream> GetCompleto(string id)
        {
            conectar();
            Stream stream = new System.IO.MemoryStream();
            IDownloadProgress req = service.Files.Get(id).DownloadWithStatus(stream);
            if (req.Status == DownloadStatus.Failed) {
                throw(new Exception("Ocurrió un problema al obtener la imagen"));
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public async Task<string> SubirArchivo(System.IO.FileStream fileStream, string nombre, string contentType) {
            conectar();
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
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
