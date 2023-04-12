namespace bl_dev_sandbox.BSORv2;

public partial class BSORv2 {
    public ReplayInfo replayInfo = new ReplayInfo();
    public Dictionary<string, byte[]> customData = new Dictionary<string, byte[]>();

    public enum BlockType {
        ReplayInfo = 0,
        CustomData = 10
    }
}