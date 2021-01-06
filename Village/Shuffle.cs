using System.Collections;

public static class Shuffle 
{
    public static T[] ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = RandomNumber.Range(i, array.Length);

            T temporary = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temporary;
        }

        return array;
    }
}
