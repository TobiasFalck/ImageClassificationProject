using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageClassificationProject
{
    internal class Program
    {
        private static readonly string ModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TensorFlowModel", "model.savedmodel");
        private static readonly string LabelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TensorFlowModel", "labels.txt");
        private const string ModelInputName = "serving_default_sequential_3_input";
        private const string ModelOutputName = "StatefulPartitionedCall";

        static void Main(string[] args)
        {
            var mlContext = new MLContext();

            var pipeline = mlContext.Transforms
                .LoadImages(outputColumnName: ModelInputName, imageFolder: "", inputColumnName: nameof(ModelInput.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: ModelInputName, imageWidth: 224, imageHeight: 224, inputColumnName: ModelInputName))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: ModelInputName, interleavePixelColors: true, offsetImage: 127.5f, scaleImage: 1 / 127.5f))
                .Append(mlContext.Model.LoadTensorFlowModel(ModelPath)
                    .ScoreTensorFlowModel(
                        outputColumnNames: new[] { ModelOutputName },
                        inputColumnNames: new[] { ModelInputName },
                        addBatchDimensionInput: true));

            var emptyData = mlContext.Data.LoadFromEnumerable(new List<ModelInput>());
            var model = pipeline.Fit(emptyData);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

            var labels = File.ReadAllLines(LabelsPath);
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg");

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Image not found at: {imagePath}");
                return;
            }

            var input = new ModelInput { ImagePath = imagePath };
            var prediction = predictionEngine.Predict(input);

            var maxProbability = prediction.Prediction.Max();
            var maxIndex = Array.IndexOf(prediction.Prediction, maxProbability);
            var predictedLabel = labels[maxIndex];

            Console.WriteLine($"Image: {Path.GetFileName(imagePath)}");
            Console.WriteLine($"Predicted Label: {predictedLabel}");
            Console.WriteLine($"Probability: {maxProbability:P2}");
            Console.ReadLine();
        }
    }

    public class ModelInput
    {
        public string ImagePath { get; set; }
    }

    public class ModelOutput
    {
        [ColumnName("StatefulPartitionedCall")]
        public float[] Prediction { get; set; }
    }
}
