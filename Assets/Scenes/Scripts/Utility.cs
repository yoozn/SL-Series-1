using System.Collections;
using System.Collections.Generic;

//Static so easily accessible by all other classes.
//Not using any unity name spaces, pretty much raw c#, with general functionality
public static class Utility
{
    //Method with return type general array 'T[]'. Named ShuffleArray, with input value of <T>. Parameters are T[] array, and a int seed for random control
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        //Generate a new random instance with set seed.
        System.Random prng = new System.Random(seed);

        // loop over array length, except for the last array index, since that one doesn't need to be shuffled.
        for (int i = 0; i < array.Length - 1; i++)
        {
            //Generate a random index, from values unshuffled (don't change already shuffled values that are behind index)
            int randomIndex = prng.Next(i, array.Length);
            //Swap the randomIndex, and the index the loop is currently on.
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        //Return the shuffled array
        return array;
    }
}
