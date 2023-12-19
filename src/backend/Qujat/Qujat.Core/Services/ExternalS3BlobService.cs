using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Qujat.Core.Services
{
    public class ExternalS3BlobServiceConfig
    {
        public string S3StorageBaseUri { get; set; }
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }


    public class ExternalS3BlobService
    {
        private readonly AmazonS3Client _client;
        private readonly ExternalS3BlobServiceConfig _config;

        public ExternalS3BlobService(
            ExternalS3BlobServiceConfig config)
        {
            _config = config;
            _client = new AmazonS3Client(
                new BasicAWSCredentials(_config.AccessKey, _config.SecretKey),
                new AmazonS3Config
                {
                    ServiceURL = _config.S3StorageBaseUri
                });
        }

        public async Task<Uri> UploadBlob(
            Stream inputStream,
            string fileNameWithExtension,
            CancellationToken ct)
        {
            var inputExtension = Path.GetExtension(fileNameWithExtension);
            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(inputExtension);

            using var memoryStream = new MemoryStream();
            await inputStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;

            var key = fileNameWithExtension;
            var putRequest = new PutObjectRequest
            {
                BucketName = _config.BucketName,
                Key = key,
                InputStream = memoryStream,
                ContentType = mimeType,
            };

            var response = await _client.PutObjectAsync(putRequest, ct);

            if (response.HttpStatusCode != HttpStatusCode.OK &&
                response.HttpStatusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException(
                    "Received error response from ps.kz/s3 storage while trying to upload file, " +
                    $"response content {response}");
            }

            var generatedUri = $"https://{_config.BucketName}." +
                $"{_config.S3StorageBaseUri.Replace(@"https://", string.Empty)}/" +
                $"{key}";

            return new Uri(generatedUri);
        }
    }
}
