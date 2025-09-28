using Infrastructure.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ImageClassificationProject
{

    public class TensorFlowImageClassifier
    {
        public enum ImageLabels
        {
            cardboard = 0,
            glass = 1,
            metal = 2,
            plastic = 3,
        }

        private static string _localPath = "C:\\Users\\Niklas\\source\\repos\\ImageClassificationProject\\ImageClassificationProject";
        private static string _baseDir = string.Empty;

        static void Main(string[] args)
        {
            var trainList = new List<ImageData>();
            var validateList = new List<ImageData>();

            _baseDir = Path.Combine(_localPath, "data");

            var testImage = Path.Combine(_baseDir, "unknown", "test_002.jpg");


            foreach (var materialTypeName in Enum.GetValues(typeof(ImageLabels)))
            {
                foreach (var trainFilePath in Directory.GetFiles(Path.Combine(_baseDir, materialTypeName.ToString(), "train")))
                {
                    trainList.Add(new ImageData() { ImagePath = trainFilePath, Label = ((int)materialTypeName).ToString() });
                }
            }

            foreach (var materialTypeName in Enum.GetValues(typeof(ImageLabels)))
            {
                foreach (var validateFilePath in Directory.GetFiles(Path.Combine(_baseDir, materialTypeName.ToString(), "model_test")))
                {
                    validateList.Add(new ImageData() { ImagePath = validateFilePath, Label = ((int)materialTypeName).ToString() });
                }
            }


            var ml = new MLContext(seed: 1);

            var trainData = ml.Data.LoadFromEnumerable(trainList);
            var validData = ml.Data.LoadFromEnumerable(validateList);


            Action<InputData, OutputData> convertVecType = (input, output) => output.Image = ConvertToVarVectorByte(input.Image);

            var pipeline = ml.Transforms.Conversion.MapValueToKey("LabelAsKey", nameof(ImageData.Label))
                  .Append(ml.Transforms.LoadImages(
                            outputColumnName: "Image",
                            imageFolder: "",
                            inputColumnName: nameof(ImageData.ImagePath)))
                  .Append(ml.Transforms.ResizeImages(
                            outputColumnName: "Image",
                            imageWidth: 224,
                            imageHeight: 224,
                            inputColumnName: "Image"))
                  .Append(ml.Transforms.ExtractPixels(
                      outputColumnName: "Image"))
                  .Append(ml.Transforms.CustomMapping<InputData, OutputData>(convertVecType, "convertVecType")
                  .Append(ml.MulticlassClassification.Trainers.ImageClassification(
                            featureColumnName: "Image",
                            labelColumnName: "LabelAsKey", //System.ArgumentOutOfRangeException: 'Column 'LabelAsKey' not found (Parameter 'name')' ?!
                            validationSet: validData))
                  .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"))
                  );

            var model = pipeline.Fit(trainData);


            var engine = ml.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);


            var pred = engine.Predict(new ImageData { ImagePath = testImage });


            int bestIdx = -1;
            float bestScore = float.NegativeInfinity;

            if (pred.Score != null && pred.Score.Length > 0)
            {
                bestIdx = 0; bestScore = pred.Score[0];

                for (int i = 1; i < pred.Score.Length; i++)
                {
                    if (pred.Score[i] > bestScore)
                    {
                        bestIdx = i; bestScore = pred.Score[i];
                    }
                }
            }

            Console.WriteLine("\n=== Klassifikation (ren ML.NET) ===");
            Console.WriteLine($"Billede: {testImage}");
            Console.WriteLine($"Label:   {pred.PredictedLabel}");
            Console.WriteLine($"Score:   {bestScore:P2}");
        }


        static VBuffer<byte> ConvertToVarVectorByte(VBuffer<float> input)
        {
            VBuffer<byte> result = default;
            var editor = VBufferEditor.Create(ref result, input.Length, input.GetValues().Length);

            var values = input.GetValues();
            if (input.IsDense)
            {
                for (int i = 0; i < values.Length; i++)
                    editor.Values[i] = (byte)values[i];
            }
            else
            {
                var indices = input.GetIndices();
                for (int i = 0; i < values.Length; i++)
                {
                    editor.Values[i] = (byte)values[i];
                    editor.Indices[i] = indices[i];
                }
            }

            return editor.Commit();
        }
    }
}
