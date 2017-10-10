using Encog.ML.Data;
using Encog.Neural.Thermal;
using Encog.Neural.NeuralData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Specific;
using Encog.Persist;
using Encog.Neural.Networks;

namespace RNA
{
    class Program
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static List<BiPolarMLData> patterns = new List<BiPolarMLData>();

        static void Main(string[] args)
        {
            train();
            //useNetwork();
            //var pattern = getPattern("RNA.Letters.01.png");
            //var test = getPattern("RNA.Letters.01.png");          


        }

        private static void train()
        {
            loadPatterns();
            BiPolarMLData test = getPattern("RNA.Test.01.png");
            HopfieldNetwork network = new HopfieldNetwork(patterns.First().Count);

            foreach (var pattern in patterns)
            {
                network.AddPattern(pattern);
            }

            network.CurrentState = test;
            int cycles = network.RunUntilStable(10000);
            BiPolarMLData result = network.CurrentState;
            createImage(result);
            //EncogDirectoryPersistence.SaveObject(new FileInfo("network"), network);
        }

        private static void useNetwork()
        {
            var test = getPattern("RNA.Letters.01.png");
            BasicNetwork network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo("network"));
            
            //var result = network.Compute(test);
            //createImage(result as BiPolarMLData);
        }

        private static void loadPatterns()
        {
            var letters = assembly.GetManifestResourceNames();

            for(int i=0; i<letters.Length; i++)
            {
                if(letters[i].Contains("Letters"))
                {
                    var pattern = getPattern(letters[i]);
                    patterns.Add(pattern);
                }
            }
        }

        private static BiPolarMLData getPattern(string filename)
        {
            BiPolarMLData pattern;
            int item = 0;

            using (Stream stream = assembly.GetManifestResourceStream(filename))
            using (Bitmap image = new Bitmap(stream))
            {
                pattern = new BiPolarMLData(image.Width * image.Height);

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color color = image.GetPixel(x, y);

                        if (color.ToArgb() != Color.White.ToArgb())
                            pattern.SetBoolean(item++, true);
                        else
                            pattern.SetBoolean(item++, false);
                    }
                }
            }

            return pattern;
        }

        private static void createImage(BiPolarMLData pattern)
        {
            int item = 0;
            using (Bitmap image = new Bitmap(79, 61))
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        if (pattern.Data[item++] == 1)
                            image.SetPixel(x, y, Color.Black);
                        else
                            image.SetPixel(x, y, Color.White);
                    }
                }

                image.Save("result.png");
            }
        }

        private static byte[] toArrayByte(double[] data)
        {
            byte[] rawData = new byte[data.Length];

            for(int i=0; i<rawData.Length; i++)
            {
                if (data[i] == 1)
                    rawData[i] = (byte)Color.Black.ToArgb();
                else
                    rawData[i] = (byte)Color.White.ToArgb();
            }

            return rawData;
        }
    }
}
