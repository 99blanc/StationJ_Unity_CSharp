using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIManager
{
    public void Init()
    {
        
    }

    private List<T> ParseToList<T>([NotNull] string path)
    {
        using StreamReader reader = new StreamReader(path);
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<T>().ToList();
    }

    /// <summary>
    /// ��ųʸ��� �о���� .csv ������ ������ �Ľ��Ͽ� ��ȯ�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Item"></typeparam>
    /// <param name="path">������ ���� ���</param>
    /// <param name="keySelector">Ű ����</param>
    private Dictionary<Key, Item> ParseToDictionary<Key, Item>([NotNull] string path, Func<Item, Key> keySelector)
    {
        using StreamReader reader = new StreamReader(path);
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<Item>().ToDictionary(keySelector);
    }
}
