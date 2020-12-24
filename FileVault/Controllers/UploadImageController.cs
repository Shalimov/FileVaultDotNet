using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FileVault.Controllers
{
    [ApiController]
    [Route("")]
    public class UploadImageController : ControllerBase
    {
        private const string BucketKey = "file-vault-debug-test-w2cjebtqm333";
        private readonly Random _rand = new();
        private readonly short[] _modes = {200, 300, 400, 500, 600, 700, 800, 900, 1000};
        private readonly ILogger<UploadImageController> _logger;
        private readonly AmazonS3Client _s3Client;

        public UploadImageController(ILogger<UploadImageController> logger)
        {
            _s3Client = new AmazonS3Client(RegionEndpoint.USWest2);
            _logger = logger;
        }

        [HttpPost("/v1/images/upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var filename = Path.GetFileNameWithoutExtension(file.FileName);

            var transfer = new TransferUtility(_s3Client);

            var original = await Image.LoadAsync(file.OpenReadStream());

            await Task.WhenAll(_modes.Select(async mode =>
            {
                await using var memoryStream = new MemoryStream();
                await original.Clone(x => x.Resize(mode, mode)).SaveAsPngAsync(memoryStream);

                var rndId = _rand.Next(1, 10000);

                await transfer.UploadAsync(memoryStream, BucketKey, $"{filename}_x{mode}_{rndId}.png");
            }));

            return Ok();
        }
    }
}