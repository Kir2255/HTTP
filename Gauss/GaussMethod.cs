using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gauss
{
    [Serializable]
    public class GaussMethod
    {
        public uint rowCount;
        public uint columCount;
        public double[][] matrix { get; set; }
        public double[] rightPart { get; set; }
        public double[] result { get; set; }

        public GaussMethod(uint rows, uint columns)
        {
            rightPart = new double[rows];
            result = new double[rows];
            matrix = new double[rows][];

            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new double[columns];
            }

            rowCount = rows;
            columCount = columns;

            //обнулим массив
            for (int i = 0; i < rows; i++)
            {
                result[i] = 0;
                rightPart[i] = 0;

                for (int j = 0; j < columns; j++)
                {
                    matrix[i][j] = 0;
                }
            }
        }

        private void SortRows(int sortIndex)
        {

            double maxElement = matrix[sortIndex][sortIndex];
            int maxElementIndex = sortIndex;

            for (int i = sortIndex + 1; i < rowCount; i++)
            {
                if (matrix[i][sortIndex] > maxElement)
                {
                    maxElement = matrix[i][sortIndex];
                    maxElementIndex = i;
                }
            }

            //теперь найден максимальный элемент ставим его на верхнее место
            if (maxElementIndex > sortIndex)//если это не первый элемент
            {
                double temp;

                temp = rightPart[maxElementIndex];
                rightPart[maxElementIndex] = rightPart[sortIndex];
                rightPart[sortIndex] = temp;

                for (int i = 0; i < columCount; i++)
                {
                    temp = matrix[maxElementIndex][i];
                    matrix[maxElementIndex][i] = matrix[sortIndex][i];
                    matrix[sortIndex][i] = temp;
                }
            }
        }

        public int SolveMatrix()
        {
            if (rowCount != columCount)
            {
                return 2; //нет решения
            }

            Parallel.For(0, (int)rowCount, i =>
            {
                SortRows(i);

                for (int j = i + 1; j < rowCount; j++)
                {
                    if (matrix[i][i] != 0) //если главный элемент не 0, то производим вычисления
                    {
                        double multElement = matrix[j][i] / matrix[i][i];

                        for (int k = i; k < columCount; k++)
                        {
                            matrix[j][k] -= matrix[i][k] * multElement;
                        }

                        rightPart[j] -= rightPart[i] * multElement;
                    }
                }
            });


            //ищем решение
            for (int i = (int)(rowCount - 1); i >= 0; i--)
            {
                result[i] = rightPart[i];

                for (int j = (int)(rowCount - 1); j > i; j--)
                {
                    result[i] -= matrix[i][j] * result[j];
                }

                if (matrix[i][i] == 0)
                {
                    if (rightPart[i] == 0)
                        return 2; //множество решений
                    else
                        return 1; //нет решения
                }

                result[i] /= matrix[i][i];

            }
            return 0;
        }

        public override string ToString()
        {
            StringBuilder answer = new StringBuilder();
            for (int i = 0; i < rowCount; i++)
            {
                answer.Append("\r\n");
                for (int j = 0; j < columCount; j++)
                {
                    answer.Append(matrix[i][j].ToString("F03") + " * x" + (j + 1) + "\t");
                }

                answer.Append(" = " + rightPart[i].ToString("F03"));
            }

            answer.Append("\n");

            for (int i = 0; i < rowCount; i++)
            {
                answer.Append("\nx" + (i + 1) + " = " + result[i].ToString("F03"));
            }

            return answer.ToString();
        }

        private static double[] GetRandomRight(uint num)
        {
            double[] result = new double[num];

            Random rnd = new Random();

            for (int i = 0; i < num; i++)
            {
                result[i] = rnd.NextDouble() * 100;
            }

            return result;
        }

        private static double[][] GetRandomMatrix(uint rows, uint cols)
        {
            double[][] result = new double[rows][];

            Random rnd = new Random();

            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = rnd.NextDouble() * 100;
                }
            }

            return result;
        }

        public void RandomInit(uint rows, uint cols)
        {
            Task<double[]> task1 = new Task<double[]>(() => GetRandomRight(rows));
            Task<double[][]> task2 = new Task<double[][]>(() => GetRandomMatrix(rows, cols));

            Task[] tasks = new Task[2] { task1, task2 };
            foreach (Task task in tasks)
            {
                task.Start();
            }
            Task.WaitAll(tasks);

            this.rightPart = task1.Result;
            this.matrix = task2.Result;
        }

        public static string MatrixToString(double[][] matrix)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[0].Length; j++)
                {
                    stringBuilder.Append(matrix[i][j].ToString() + " ");
                }
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        public static string ArrayToString(double[] right)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < right.Length; i++)
            {
                stringBuilder.Append(right[i].ToString() + "\n");
            }

            return stringBuilder.ToString();
        }


    }
}
