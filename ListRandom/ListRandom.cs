namespace ListRandom;

public class ListNode
{
    public ListNode Previous { get; internal set; }

    public ListNode Next { get; internal set; }

    public ListNode Random { get; set; }

    public string Data { get; set; }
}

/// <summary>
/// Doubly linked list with a link to a list item.
/// </summary>
public class ListRandom
{
    public ListNode Head { get; private set; }

    public ListNode Tail { get; private set; }

    public int Count { get; private set; }

    public ListNode this[int index]
    {
        get
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index < this.Count / 2)
            {
                var cursor = this.Head;
                while (index > 0)
                {
                    cursor = cursor.Next;
                    index--;
                }

                return cursor;
            }
            else
            {
                var cursor = this.Tail;
                while (index < this.Count - 1)
                {
                    cursor = cursor.Previous;
                    index++;
                }

                return cursor;
            }
        }

        set
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index < this.Count / 2)
            {
                var cursor = this.Head;
                while (index > 0)
                {
                    cursor = cursor.Next;
                    index--;
                }

                cursor.Data = value.Data;
            }
            else
            {
                var cursor = this.Tail;
                while (index < this.Count - 1)
                {
                    cursor = cursor.Previous;
                    index++;
                }

                cursor.Data = value.Data;
            }
        }
    }

    public ListNode this[Index index]
    {
        get => this[index.GetOffset(this.Count)];

        set => this[index.GetOffset(this.Count)] = value;
    }

    /// <summary>
    /// Deserializes <see cref="ListRandom"/>.
    /// Signature:
    /// <list type="number">
    /// <item>List count</item>
    /// <item>Nodes</item>
    /// <item>Random reference</item>
    /// </list>
    /// </summary>
    /// <param name="s">Input stream.</param>
    /// <returns>Doubly linked list.</returns>
    public static ListRandom Deserialize(Stream s)
    {
        using var streamReader = new StreamReader(s, leaveOpen: true);

        // List count
        var count = int.Parse(streamReader.ReadLine() !);
        var buffer = new List<ListNode>(count);
        var listRandom = new ListRandom();

        // Nodes
        var line = streamReader.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            var node = new ListNode() { Data = line };
            buffer.Add(node);
            listRandom.Add(node);
            line = streamReader.ReadLine();
        }

        // Random reference
        line = streamReader.ReadLine();
        while (line is not null)
        {
            var ids = line.Split('-');
            var index1 = int.Parse(ids[0]);
            var index2 = int.Parse(ids[1]);
            buffer[index1].Random = buffer[index2];
            line = streamReader.ReadLine();
        }

        return listRandom;
    }

    /// <summary>
    /// Serializes <see cref="ListRandom"/>.
    /// Signature:
    /// <list type="number">
    /// <item>List count</item>
    /// <item>Nodes</item>
    /// <item>Random reference</item>
    /// </list>
    /// </summary>
    /// <param name="s">Output stream.</param>
    public void Serialize(Stream s)
    {
        using var streamWriter = new StreamWriter(s, leaveOpen: true);

        // List count
        streamWriter.WriteLine(this.Count);
        var buffer = new List<ListNode>(this.Count);

        // Nodes
        var cursor = this.Head;
        while (cursor is not null)
        {
            buffer.Add(cursor);
            streamWriter.WriteLine(cursor.Data);
            cursor = cursor.Next;
        }

        streamWriter.WriteLine();

        // Random reference
        int id = 0;
        cursor = this.Head;
        while (cursor is not null)
        {
            if (cursor.Random is not null)
            {
                streamWriter.WriteLine($"{id}-{buffer.IndexOf(cursor.Random)}");
            }

            cursor = cursor.Next;
            id++;
        }

        streamWriter.Flush();
    }

    /// <summary>
    /// Adds <paramref name="value"/> to the end of the list.
    /// </summary>
    /// <param name="value">List node.</param>
    public void Add(ListNode value)
    {
        value.Data = value.Data.ReplaceLineEndings(@"\n");

        if (this.Count == 0)
        {
            this.Head = value;
            this.Tail = value;
            this.Count = 1;
            return;
        }

        this.Tail.Next = value;
        value.Previous = this.Tail;
        this.Tail = value;
        this.Count++;
    }
}
