namespace GrpcPort.Contracts.Actions;

/// <summary>Значения StreamEnvelope.Type для протокола Actions поверх gRPC-порта.</summary>
public static class StreamActionTypes
{
    public const string Ping = "ping";
    public const string Pong = "pong";
    public const string Data = "data";
    public const string Publish = "publish";
    public const string Unpublish = "unpublish";
    public const string Request = "request";
    public const string Response = "response";
    public const string OnPublish = "on-publish";
    public const string OnUnpublish = "on-unpublish";
    public const string Catalog = "catalog";
}
