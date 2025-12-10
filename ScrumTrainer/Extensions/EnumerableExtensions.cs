namespace ScrumTrainer.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> TakeShuffled<T>(
        this IEnumerable<T> source,
        int numElementsToTake,
        Random? random = null)
    {
        if (source == null || numElementsToTake < 0)
            return default!;

        random ??= new Random();

        var list = source.ToList();
        int n = list.Count;

        if (numElementsToTake > n)
            numElementsToTake = n; // We can't take more elements than the source's length

        // Partial Fisher–Yates
        for (int i = 0; i < numElementsToTake; i++)
        {
            int j = random.Next(i, n);
            (list[i], list[j]) = (list[j], list[i]); // Swap
        }

        return list.Take(numElementsToTake);
    }
}
