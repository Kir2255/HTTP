using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gauss
{
    public class TextFileHandler
    {
        public const string MATRIX_FILE = "TextFiles\\matrix.txt";
        public const string RIGHT_PART_FILE = "TextFiles\\right_part.txt";
        public const string RESULT_FILE = "TextFiles\\result.txt";
        public const string SIZE_INFO_FILE = "TextFiles\\size_info.txt";
        public static async Task WriteToFile(string path, string input)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(input);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task ReadArraytFromFile(string path, double[] output)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string result;

                    while ((result = await reader.ReadLineAsync()) != null)
                    {
                        stringBuilder.Append(result + " ");
                    }

                    string[] temp = stringBuilder.ToString().Trim().Split(' ');

                    Parallel.For(0, temp.Length, i => 
                    {
                        output[i] = Double.Parse(temp[i]);
                    });

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task ReadMatrixFromFile(string path, double[][] output)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string result;

                    while ((result = await reader.ReadLineAsync()) != null)
                    {
                        stringBuilder.Append(result + "\n");
                    }

                    string[] temp = stringBuilder.ToString().Trim().Split('\n');
                    string[][] stringArray = new string[temp.Length][];

                    Parallel.For(0, temp.Length, i =>
                    {
                        stringArray[i] = temp[i].Trim().Split(' ');
                    });

                    Parallel.For(0, stringArray.Length, i =>
                    {
                        for (int j = 0; j < stringArray[0].Length; j++)
                        {
                            output[i][j] = Double.Parse(stringArray[i][j]);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task WriteSizeInfoToFile(string path, GaussMethod gauss)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(gauss.rowCount + "x" + gauss.columCount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static GaussMethod ReadSizeInfoFromFile(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string result;

                    while ((result =  reader.ReadLine()) != null)
                    {
                        stringBuilder.Append(result + " ");
                    }

                    string[] temp = stringBuilder.ToString().Trim().Split('x');
                    return new GaussMethod(UInt32.Parse(temp[0]), UInt32.Parse(temp[1]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
