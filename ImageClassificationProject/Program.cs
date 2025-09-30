using Infrastructure.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Diagnostics.Contracts;
using Tensorflow.Operations.Initializers;

namespace ConsoleImageClassification
{
    //alle filer, altsaa test billedet, labels.txt, saved_model.pb, variables.index og variables.data:
    //Tjek deres properties og vaer sikker paa der staar "Copy to Output Directory: Copy if newer"
    internal class Program
    {
        //stien til modellens MAPPE, sammenhold det med solution explorer saa skal det nok give mening.
        private static readonly string ModelPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"TensorFlowModel","model.savedmodel");
        //stien til labels
        private static readonly string LabelsPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"TensorFlowModel", "labels.txt");
        //input noden paa jeres savedmodel, find den vha. netron.app (hjemmeside)
        private const string ModelInputName = "serving_default_sequential_3_input";
        //outputnoden p[ jeres savedmodel, find paa samme maade
        private const string ModelOutputName = "StatefulPartitionedCall";



        static void Main(string[] args)
        {
            //bruge noget som hedder MLContext - hvad er det? Who knows!
            var mlContext = new MLContext();

            // Define the paths to the four test folders containing images for each material category.
            // Each folder is expected to be located under the "data" directory in the application's base directory.
            // The folders are named according to the material type: "cardboard", "glass", "metal", and "plastic".
            // Each material folder contains a "model_test" subfolder where the test images are stored.
            var testFolders = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data","cardboard", "model_test"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data","glass", "model_test"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data","metal", "model_test"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data","plastic", "model_test")
                };

            // Collect all image files (e.g., jpg, png) from all test folders
            var allImagePaths = testFolders
                .SelectMany(folder => Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                   file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                   file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                .ToList();
            // Create ModelInput objects for each image
            var allInputs = allImagePaths.Select(path => new ModelInput { ImagePath = path }).ToList();

            //definer image processing pipeline - hvad er det? Who the fuck knows, jeg
            //faar hjaelp af AI!
            var pipeline = mlContext.Transforms.LoadImages(outputColumnName: ModelInputName, imageFolder: "", inputColumnName: nameof(ModelInput.ImagePath))
            .Append(mlContext.Transforms.ResizeImages(outputColumnName: ModelInputName, imageWidth: 224, imageHeight: 224, inputColumnName: ModelInputName))
            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: ModelInputName, interleavePixelColors: true, offsetImage: 127.5f, scaleImage: 1 / 127.5f))
            .Append(mlContext.Model.LoadTensorFlowModel(ModelPath)
            .ScoreTensorFlowModel(
                outputColumnNames: new[] { ModelOutputName },
                inputColumnNames: new[] { ModelInputName },
                addBatchDimensionInput: true));

            //lav en prediction engine - do i have to repeat myself???
            var emptyData = mlContext.Data.LoadFromEnumerable(new List<ModelInput>());
            var model = pipeline.Fit(emptyData);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

            // Load all ModelInput objects into IDataView
            var inputData = mlContext.Data.LoadFromEnumerable(allInputs);

            // Fit the pipeline (if needed, usually for transformers, not for TensorFlow models)
            model = pipeline.Fit(inputData);

            // Transform the data (run all images through the pipeline)
            var predictions = model.Transform(inputData);

            // Extract predictions
            var predictionResults = mlContext.Data.CreateEnumerable<ModelOutput>(predictions, reuseRowObject: false).ToList();

            // Load labels
            var labels = File.ReadAllLines(LabelsPath);

            // Print results for each image
            for (int i = 0; i < allInputs.Count; i++)
            {
                var prediction = predictionResults[i];
                var maxProbability = prediction.Prediction.Max();
                var maxIndex = prediction.Prediction.AsSpan().IndexOf(maxProbability);
                var predictedLabel = labels[maxIndex];

                Console.WriteLine($"Image: {Path.GetFileName(allInputs[i].ImagePath)}");
                Console.WriteLine($"Predicted Label: {predictedLabel}");
                Console.WriteLine($"Probability: {maxProbability:P2}");
                Console.WriteLine();
                CreateCsvFile();

            }

            Console.ReadLine();

            


        }

        private static void CreateCsvFile(bool includeHeader = false)
        {
            string folderPath = @"C:\Users\Niklas\Source\Repos\ImageClassificationProject\ImageClassificationProject\data";
            string fileName = "mlStats.csv";
            string fullPath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);

            if (!File.Exists(fullPath))
            {
                string content = includeHeader ? "ImageName,PredictedLabel,Probability\n" : string.Empty;
                File.WriteAllText(fullPath, content);

                Console.WriteLine($"CSV file created at: {fullPath}");
            }
            else
            {
                Console.WriteLine($"CSV file already exists: {fullPath}");
            }
        }


    }

    //stien til billedfilen paa computeren
    public class ModelInput
   {
       public string ImagePath { get; set; }
    }

    public class ModelOutput
    {
        //StatefulPartitionedCall var navnet i min tensorflowmodel,
        //gaar ud fra det er det samme for alle savedmodel fra teachablemachine
        [ColumnName("StatefulPartitionedCall")]
        public float[] Prediction { get; set; }
    }


}
