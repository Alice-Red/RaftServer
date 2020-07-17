namespace RaftServer
{
    /// <summary>
    /// リクエストタイプを定義します
    /// </summary>
    public enum RequestType
    {
        Err,
        Get,
        Put,
        Post,
        Head,
        Options,
        Delete,
    }
}