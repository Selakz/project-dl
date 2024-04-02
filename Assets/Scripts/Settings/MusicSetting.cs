public class MusicSetting
{
    public enum LayoutType { Empty, Score, Combo, FLIndicator, JudgeSum, Accuracy }

    public float speed = 1.0f;
    public float audioDeviation = 0f;
    public float visionDeviation = 0f;
    public LayoutType[] layouts = new LayoutType[5]{
        LayoutType.Score,
        LayoutType.JudgeSum,
        LayoutType.Empty,
        LayoutType.FLIndicator,
        LayoutType.Combo
    };
}
