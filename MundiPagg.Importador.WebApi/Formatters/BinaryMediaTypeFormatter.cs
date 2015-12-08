using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MundiPagg.Importador.Core.Extensions;

namespace MundiPagg.Importador.WebApi.Formatters
{
    public class FileMediaFormatter<T> : MediaTypeFormatter
    {
        public FileMediaFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(FileUpload<T>);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (!content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var parts = await content.ReadAsMultipartAsync();
            var fileContent = parts.Contents.First(x => SupportedMediaTypes.Contains(x.Headers.ContentType));

            var dataString = "";
            foreach (var part in parts.Contents.Where(x => x.Headers.ContentDisposition.DispositionType == "form-data"
                                                        && x.Headers.ContentDisposition.Name == "\"data\""))
            {
                var data = await part.ReadAsStringAsync();
                dataString = data;
            }

            string fileName = fileContent.Headers.ContentDisposition.FileName;
            string mediaType = fileContent.Headers.ContentType.MediaType;

            using (var stream = await fileContent.ReadAsStreamAsync())
            {
                byte[] imagebuffer = stream.ReadFully();
                return new FileUpload<T>(imagebuffer, mediaType, fileName, dataString);
            }
        }
    }
    public class FileUpload<T>
    {
        private readonly string _RawValue;
        public T Value { get; set; }
        public string FileName { get; set; }
        public string MediaType { get; set; }
        public byte[] Buffer { get; set; }

        public FileUpload(byte[] buffer, string mediaType, string fileName, string value)
        {
            Buffer = buffer;
            MediaType = mediaType;
            FileName = fileName == null ? null : fileName.Replace("\"", "");
            _RawValue = value;

            Value = JsonConvert.DeserializeObject<T>(_RawValue);
        }

        public void Save(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var newPath = Path.Combine(path, FileName);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            File.WriteAllBytes(newPath, Buffer);

            var property = Value.GetType().GetProperty("FileName");
            property.SetValue(Value, FileName, null);
        }
    }


    public class ImageMediaFormatter : MediaTypeFormatter
    {
        public ImageMediaFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpeg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(ImageMedia);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent httpContent, IFormatterLogger formatterLogger)
        {
            if (!httpContent.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var parts = await httpContent.ReadAsMultipartAsync();
            var content = parts.Contents.First(x => SupportedMediaTypes.Contains(x.Headers.ContentType));

            string fileName = content.Headers.ContentDisposition.FileName;
            string mediaType = content.Headers.ContentType.MediaType;

            using (var stream = await content.ReadAsStreamAsync())
            {
                byte[] imagebuffer = stream.ReadFully();
                return new ImageMedia(fileName, mediaType, imagebuffer);
            }
        }
    }
    public class ImageMedia
    {
        public string FileName { get; set; }
        public string MediaType { get; private set; }
        public byte[] Buffer { get; private set; }

        public ImageMedia(string fileName, string mediaType, byte[] imagebuffer)
        {
            FileName = fileName;
            MediaType = mediaType;
            Buffer = imagebuffer;
        }

        public void Save(string path)
        {
            Save(path, FileName);
        }
        public void Save(string path, string filename)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var newPath = Path.Combine(path, filename);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            File.WriteAllBytes(newPath, Buffer);
        }
    }
}
