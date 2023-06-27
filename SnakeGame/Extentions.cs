namespace SnakeGame
{
    public static class Extention
    {
        /// <summary>
        /// Gives the opposit of the current direction
        /// </summary>
        /// <param name="direction"> Direction </param>
        /// <returns> Opposit to the <paramref name="direction"/> direcrion </returns>
        public static Direction Opposit(this Direction direction)
        {
            return (Direction)(-(int)direction);
        }

        /// <summary>
        /// Compares two int arrays
        /// </summary>
        /// <param name="array1"/>
        /// <param name="array2"/>
        /// <returns> Are they equals </returns>
        public static bool ListEquals(this int[] array1, int[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }
    }
}