namespace Data.Models.Table;

public static class TableLayout
{
    // public int Id { get; set; }
    // public static int Interval = 45;

    public static DateTime First = new DateTime(2025, 2, 16);
    public const int TotalWeek = 20;

    public static readonly List<TimeSlot> TimeSlots =
    [
        new TimeSlot(1, new TimeOnly(8, 0), new TimeOnly(8, 50)),
        new TimeSlot(2, new TimeOnly(8, 55), new TimeOnly(9, 35)),
        new TimeSlot(3, new TimeOnly(9, 50), new TimeOnly(10, 35)),
        new TimeSlot(4, new TimeOnly(10, 40), new TimeOnly(11, 25)),
        new TimeSlot(5, new TimeOnly(11, 30), new TimeOnly(12, 15)),
        new TimeSlot(6, new TimeOnly(14, 5), new TimeOnly(14, 50)),
        new TimeSlot(7, new TimeOnly(14, 55), new TimeOnly(15, 40)),
        new TimeSlot(8, new TimeOnly(15, 45), new TimeOnly(16, 30)),
        new TimeSlot(9, new TimeOnly(16, 45), new TimeOnly(17, 30)),
        new TimeSlot(10, new TimeOnly(17, 40), new TimeOnly(18, 25)),
        new TimeSlot(11, new TimeOnly(18, 30), new TimeOnly(19, 15)),
        new TimeSlot(12, new TimeOnly(19, 20), new TimeOnly(20, 5)),
        new TimeSlot(13, new TimeOnly(20, 10), new TimeOnly(20, 55))
    ];
}