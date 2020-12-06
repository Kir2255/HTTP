using Gauss;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static uint num = 1000;
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            GaussMethod gauss = new GaussMethod(num, num);
            gauss.RandomInit(num, num);

            Task<string> rightStringTask = new Task<string>(() => GaussMethod.ArrayToString(gauss.rightPart));
            Task<string> matrixStringTask = new Task<string>(() => GaussMethod.MatrixToString(gauss.matrix));

            Task[] tasks1 = new Task[2] { rightStringTask, matrixStringTask };
            for (int i = 0; i < tasks1.Length; i++)
            {
                tasks1[i].Start();
            }
            Task.WaitAll(tasks1);


            Task first = TextFileHandler.WriteToFile(TextFileHandler.RIGHT_PART_FILE, rightStringTask.Result);
            Task second = TextFileHandler.WriteToFile(TextFileHandler.MATRIX_FILE, matrixStringTask.Result);
            Task third = TextFileHandler.WriteSizeInfoToFile(TextFileHandler.SIZE_INFO_FILE, gauss);

            Task[] tasks2 = new Task[3] { first, second, third };

            for (int i = 0; i < tasks2.Length; i++)
            {
                tasks2[i].GetAwaiter().GetResult();
            }
            Task.WaitAll(tasks2);

            GaussMethod gaussMethod = null;

            Task<GaussMethod> info = new Task<GaussMethod>(() => TextFileHandler.ReadSizeInfoFromFile(TextFileHandler.SIZE_INFO_FILE));
            info.Start();
            info.Wait();

            gaussMethod = info.Result;

            Task fouth = TextFileHandler.ReadArraytFromFile(TextFileHandler.RIGHT_PART_FILE, gaussMethod.rightPart);
            Task fifth = TextFileHandler.ReadMatrixFromFile(TextFileHandler.MATRIX_FILE, gaussMethod.matrix);

            Task[] tasks3 = new Task[2] { fouth, fifth };
            for (int i = 0; i < tasks3.Length; i++)
            {
                tasks3[i].GetAwaiter().GetResult();
            }
            Task.WaitAll(tasks3);

            //Console.WriteLine(GaussMethod.ArrayToString(gaussMethod.rightPart));
            //Console.WriteLine(GaussMethod.MatrixToString(gaussMethod.matrix));

            gaussMethod.SolveMatrix();

            Task sixth = TextFileHandler.WriteToFile(TextFileHandler.RESULT_FILE, GaussMethod.ArrayToString(gaussMethod.result));
            sixth.GetAwaiter().GetResult();

            //Console.WriteLine(gaussMethod.ToString());

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00000000}",
                                                ts.Hours, ts.Minutes, ts.Seconds,
                                                ts.Milliseconds / 10);

            Console.WriteLine("RunTime: " + elapsedTime);

            Console.ReadKey();
        }
    }
}
