using MessagePack;
using MessagePack.Formatters;

namespace Wahren.GraphicalEditor.Models;

[MessagePackFormatter(typeof(Formatter))]
public record struct InitialSettings(bool SaveWorkingDirectory, string? WorkingDirectory)
{
    public sealed class Formatter : IMessagePackFormatter<InitialSettings>
    {
        public static void Deserialize(ref MessagePackReader reader, out InitialSettings value)
        {
            var count = reader.ReadArrayHeader();
            if (count == 0)
            {
                value = new(true, null);
                return;
            }

            var saveWorkingDirectory = reader.ReadBoolean();
            if (count == 1)
            {
                value = new(saveWorkingDirectory, null);
                return;
            }

            value = new(saveWorkingDirectory, reader.TryReadNil() ? null : reader.ReadString());
            for (int i = 2; i < count; i++)
            {
                reader.Skip();
            }
        }

        public InitialSettings Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            Deserialize(ref reader, out var value);
            return value;
        }

        public static void Serialize(ref MessagePackWriter writer, ref InitialSettings value)
        {
            writer.WriteArrayHeader(2);
            writer.Write(value.SaveWorkingDirectory);
            writer.Write(value.WorkingDirectory);
        }

        public void Serialize(ref MessagePackWriter writer, InitialSettings value, MessagePackSerializerOptions options)
        {
            Serialize(ref writer, ref value);
        }
    }
}
