using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Events;

public class LearningRecordedEvent : DomainEvent
{
    public LearningCategory Category { get; }
    public string Key { get; }
    public float Confidence { get; }

    public LearningRecordedEvent(LearningCategory category, string key, float confidence)
    {
        Category = category;
        Key = key;
        Confidence = confidence;
    }
}
