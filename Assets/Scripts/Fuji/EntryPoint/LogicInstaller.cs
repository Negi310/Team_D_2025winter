using UnityEngine;

public class LogicInstaller
{
    public ConversationModel ConversationModel { get; }
    public ChunkPoolManager ChunkPoolManager { get; }

    public LogicInstaller()
    {
        ConversationModel = new ConversationModel();
        ChunkPoolManager = new ChunkPoolManager(new GameObject("PoolRoot").transform);
    }
}
