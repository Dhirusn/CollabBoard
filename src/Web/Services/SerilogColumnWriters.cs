using Serilog.Sinks.PostgreSQL;

namespace CollabBoard.Web.Services;
public static class SerilogColumnWriters
{
    public static IDictionary<string, ColumnWriterBase> Map =>
        new Dictionary<string, ColumnWriterBase>
        {
            { "timestamp", new TimestampColumnWriter(NpgsqlTypes.NpgsqlDbType.TimestampTz) },
            { "level",     new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Text) },
            { "message",   new RenderedMessageColumnWriter(NpgsqlTypes.NpgsqlDbType.Text) },
            { "messagetemplate", new MessageTemplateColumnWriter(NpgsqlTypes.NpgsqlDbType.Text) },
            { "exception", new ExceptionColumnWriter() },
            { "properties", new PropertiesColumnWriter(NpgsqlTypes.NpgsqlDbType.Jsonb) },
            { "logevent",  new LogEventSerializedColumnWriter(NpgsqlTypes.NpgsqlDbType.Text) }
        };
}
