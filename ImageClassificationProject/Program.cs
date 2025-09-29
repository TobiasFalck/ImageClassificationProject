using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Diagnostics.Contracts;
using Tensorflow.Operations.Initializers;

namespace UsingTensorFlowModel
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

            // Find all image files in the folder (adjust the extensions as needed)
            var imageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "cardboard", "model_test");
            var imageFiles = Directory.GetFiles(imageDirectory, "*.*")
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Fill the ImageList with ModelInput objects
            var ImageList = imageFiles.Select(f => new ModelInput { ImagePath = f }).ToList();

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

            //load labels og lav forudsigelsen!
            var labels = File.ReadAllLines(LabelsPath);
            //flg. skal vaere dit eget testbillede og det skal vaere tilfoejet til projektet
            //var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"data", "cardboard", "model_test");


            //var input = new ModelInput { ImagePath = imagePath };
            foreach (var input in ImageList)
            {
                var prediction = predictionEngine.Predict(input);

                var maxProbability = prediction.Prediction.Max();
                var maxIndex = prediction.Prediction.AsSpan().IndexOf(maxProbability);
                var predictedLabel = labels[maxIndex];

                Console.WriteLine($"Image: {Path.GetFileName(input.ImagePath)}");
                Console.WriteLine($"Predicted Label: {predictedLabel}");
                Console.WriteLine($"Accuracy: {maxProbability:P2}");
                Console.WriteLine(); // Blank line for readability
            }

            Console.ReadLine();
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
