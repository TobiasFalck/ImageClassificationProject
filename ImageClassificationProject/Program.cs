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
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageClassificationProject","TensorFlowModel","test.jpg");
        //C: \\Users\\Niklas\\Source\\Repos\\ImageClassificationProject\\ImageClassificationProject\\TensorFlowModel\\test.jpg

        var input = new ModelInput { ImagePath = imagePath };
            var prediction = predictionEngine.Predict(input);

            //fortolker resultatet
            var maxProbability = prediction.Prediction.Max();
            var maxIndex = prediction.Prediction.AsSpan().IndexOf(maxProbability);
            var predictedLabel = labels[maxIndex];

            Console.WriteLine($"Image: {Path.GetFileName(imagePath)}");
            Console.WriteLine($"Predicted Label: {predictedLabel}");
            Console.WriteLine($"Probability: {maxProbability:P2}");

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
