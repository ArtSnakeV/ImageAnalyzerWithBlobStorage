using AzureHomework2t1.Models;
using AzureHomework2t1.Services.Abstract;
using AzureHomework2t1.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace AzureHomework2t1.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IBlobService _blobService;
        private readonly string _containerName;
        private readonly string _visionKey;
        private readonly string _visionEndpoint;
        // 
        private readonly ApplicationDbContext _dbContext;

        public ImagesController(IBlobService blobService, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _blobService = blobService;
            _dbContext = dbContext;
            _containerName = configuration["AzureBlobStorage:ImageContainerName"]!;
            _visionKey = configuration["ComputerVision:AzureVisionKey"]!;
            _visionEndpoint = configuration["ComputerVision:AzureVisionEndpoint"]!;

        }

        public IActionResult Index()
        {
            // Displaying all images from the storage
            var images = _dbContext.ImageRecords.ToList();
            return View(images);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not selected.");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                var url = await _blobService.UploadBlobAsync(fileName, stream);

                ///////////////////////////
                /// Computer Vision part
                var connectionString = _visionKey;
                string endpoint = _visionEndpoint;


                
                ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_visionKey);
                ComputerVisionClient visionClient = new ComputerVisionClient(credentials)
                {
                    Endpoint = _visionEndpoint
                };

                IList<VisualFeatureTypes?> visualFeatures = Enum
                    .GetValues(typeof(VisualFeatureTypes))
                    .OfType<VisualFeatureTypes?>()
                    .ToList();
                ImageAnalysis imageAnalysis = await visionClient.AnalyzeImageAsync(
                    url: url,
                    visualFeatures: visualFeatures);

                var analysisResultsStrings = new List<string>();
                if (imageAnalysis.Description is not null)
                {
                    foreach (var caption in imageAnalysis.Description.Captions)
                    {
                        analysisResultsStrings.Add($"Text: {caption.Text} / Confidense: {caption.Confidence} ");
                    }
                }
                analysisResultsStrings.Add($"-----------Categories-----------");
                foreach (Category category in imageAnalysis.Categories)
                {
                    analysisResultsStrings.Add($"Name: {category.Name}");
                    analysisResultsStrings.Add($"Score: {category.Score}");
                }

                TempData["ImageItself"] = url;
                TempData["ImageData"] = string.Join("<br/>", analysisResultsStrings);


                ///////////////////////////

                // Save record in database
                var imageRecord = new ImageRecord
                {
                    FileName = fileName,
                    Url = url,
                    Description = JsonConvert.SerializeObject(imageAnalysis)
                };
                _dbContext.ImageRecords.Add(imageRecord);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
        }
    }
}
