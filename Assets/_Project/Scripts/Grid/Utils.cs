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
    }
}