static int[] SortBySwap(int[] arrayToSort)
{
    for (int i = 0; i < arrayToSort.Length; i++)
    {
        if (i + 1 < arrayToSort.Length)
        {
            if (arrayToSort[i] > arrayToSort[i + 1])
            {
                var temp = arrayToSort[i];
                arrayToSort[i] = arrayToSort[i + 1];
                arrayToSort[i + 1] = temp;
                i = -1; // Restart the loop
            }
        }
    }
    return arrayToSort;
}
///////////////////////////////////////////////////////////
static int[] Sort222(int[] arrayToSort)
{
    return arrayToSort;
}
///////////////////////////////////////////////////////////
var unsortedArray = new int[20];
for (int i = 0; i < unsortedArray.Length; i++)
{
    int nextNum;
    do nextNum = Random.Shared.Next(1, 100);
    while (unsortedArray.Contains(nextNum));
    unsortedArray[i] = nextNum;
}
foreach (int i in unsortedArray) Console.Write(i + " ");
Console.WriteLine("\r\n");

var sortedArray = SortBySwap(unsortedArray);

foreach (int i in sortedArray) Console.Write(i + " ");
Console.WriteLine("\r\n");

var sortedArray2 = Sort222(unsortedArray);

foreach (int i in sortedArray2) Console.Write(i + " ");
Console.WriteLine("\r\n");