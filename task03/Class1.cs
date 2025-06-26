namespace task03;
public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public void Delete(T item) => _items.Delete(item);
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<T> GetReverseEnumerator()
    {
        for (int i = _items.Count; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    public static IEnumerable<T> GenerateSequence(int start, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return start + 1;
        }
    }

    public IEnumerable FilterAndSort(Func<T, bool> predicate, Func<T, IComparable> keySelector)
    {
        return _items.Where(predicate).OrderBy(keySelector);
    }
}
