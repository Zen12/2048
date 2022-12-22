using UnityEngine.Assertions;

namespace _Project.Scripts.Grid
{
    public static class ArrayUtils
    {
        public static void Transpose(this int[,] array)
        {
            
            var x = array.GetLength(0);
            var y = array.GetLength(1);
            
            Assert.AreEqual(x, y, "Width and height if array should be equal");
            
            var copy = (int[,]) array.Clone();

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    array[i, j] = copy[j, i];
                }
            }
        }

        public static void Equal(int[,] expected, int[,] actual)
        {
            Assert.AreEqual(expected.GetLength(0), actual.GetLength(0));
            Assert.AreEqual(expected.GetLength(1), actual.GetLength(1));

            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    Assert.AreEqual(expected[i, j], actual[i, j]);
                }
            }
        }

    }
}