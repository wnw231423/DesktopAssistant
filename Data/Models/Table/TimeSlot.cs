namespace Data.Models.Table;

public class TimeSlot
{
    // public int Id { get; set; }
    public int SlotNumber { get; set; } // 课程节次
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
    public TimeSlot(int slotNumber, TimeOnly startTime, TimeOnly endTime)
    {
        SlotNumber = slotNumber;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public TimeSlot() { }
    
    private string DisplayTime => $"{StartTime:HH:mm}-{EndTime:HH:mm}";
    
    private string Name => $"第{SlotNumber}节";
    
    // 便于UI显示的字符串表示
    public override string ToString()
    {
        return $"{Name} ({DisplayTime})";
    }
}