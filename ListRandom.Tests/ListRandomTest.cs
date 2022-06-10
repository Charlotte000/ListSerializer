namespace ListRandom.Tests;

public class ListRandomTest
{
    private static ListRandom list;

    [SetUp]
    public void Setup()
    {
        list = new ListRandom();
        for (int i = 0; i < 10; i++)
        {
            list.Add(new ListNode() { Data = $"item {i}" });
        }
    }

    [Test]
    public void IndexerGetTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(list[0].Data, Is.EqualTo("item 0"));
            Assert.That(list[^2].Data, Is.EqualTo("item 8"));
            Assert.That(list[3].Data, Is.EqualTo("item 3"));
            Assert.Throws<IndexOutOfRangeException>(() => { var node = list[-1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var node = list[666]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var node = list[^666]; });
        });
    }

    [Test]
    public void IndexerSetTest()
    {
        list[0] = new ListNode() { Data = "head" };
        list[^1] = new ListNode() { Data = "tail" };
        list[1] = new ListNode() { Data = "changed item 1" };
        list[8] = new ListNode() { Data = "changed item 8" };

        Assert.Multiple(() =>
        {
            Assert.That(list[0].Data, Is.EqualTo("head"));
            Assert.That(list[^1].Data, Is.EqualTo("tail"));
            Assert.That(list[1].Data, Is.EqualTo("changed item 1"));
            Assert.That(list[8].Data, Is.EqualTo("changed item 8"));
            Assert.Throws<IndexOutOfRangeException>(() => { list[-1] = new ListNode() { Data = "data" }; });
            Assert.Throws<IndexOutOfRangeException>(() => { list[666] = new ListNode() { Data = "data" }; });
            Assert.Throws<IndexOutOfRangeException>(() => { list[^666] = new ListNode() { Data = "data" }; });
        });
    }

    [Test]
    public void SerializeTest1()
    {
        list[0].Random = list[1];

        list[2].Random = list[3];
        list[3].Random = list[2];

        list[4].Random = list[4];

        using Stream stream = new MemoryStream();
        list.Serialize(stream);
        stream.Position = 0;
        var list2 = ListRandom.Deserialize(stream);
        ListRandomTest.Validate(list, list2);
    }

    private static void Validate(ListRandom list1, ListRandom list2)
    {
        var cursor1 = list1.Head;
        var cursor2 = list2.Head;
        while (cursor1 is not null)
        {
            Assert.Multiple(() =>
            {
                Assert.That(cursor2.Data, Is.EqualTo(cursor1.Data));
                Assert.That(cursor2.Random?.Data, Is.EqualTo(cursor1.Random?.Data));
            });

            if (cursor1.Random is not null)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(cursor2.Random.Next.Data, Is.EqualTo(cursor1.Random.Next.Data));
                    Assert.That(cursor2.Random.Previous.Data, Is.EqualTo(cursor1.Random.Previous.Data));
                });
            }

            cursor1 = cursor1.Next;
            cursor2 = cursor2.Next;
        }

        cursor1 = list1.Tail;
        cursor2 = list2.Tail;
        while (cursor1 is not null)
        {
            Assert.Multiple(() =>
            {
                Assert.That(cursor2.Data, Is.EqualTo(cursor1.Data));
                Assert.That(cursor2.Random?.Data, Is.EqualTo(cursor1.Random?.Data));
            });

            cursor1 = cursor1.Previous;
            cursor2 = cursor2.Previous;
        }
    }
}