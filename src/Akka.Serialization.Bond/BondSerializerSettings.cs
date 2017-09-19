using System;
using Akka.Configuration;

namespace Akka.Serialization.Bond
{
    public class BondSerializerSettings
    {
        public enum ProtocolType
        {
            Simple = 0,
            Fast = 1,
            Compact = 2
        }

        public static readonly BondSerializerSettings Default = new BondSerializerSettings(
            bufferSize: 1024,
            protocol: ProtocolType.Simple);

        public static BondSerializerSettings Create(Config config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config), "No HOCON config was supplied for BondSerializerSettings");

            var protocol = ParseProtocolType(config.GetString("protocol", "simple"));

            return new BondSerializerSettings(
                bufferSize: config.GetInt("buffer-size", 1024),
                protocol: protocol);
        }

        private static ProtocolType ParseProtocolType(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "simple": return ProtocolType.Simple;
                case "fast": return ProtocolType.Fast;
                case "compact": return ProtocolType.Compact;
                default: throw new ArgumentException($"{nameof(BondSerializerSettings)} couldn't recognize protocol type '{value}'. Allowed options are: simple | fast | compact.");
            }
        }

        /// <summary>
        /// Default size of a buffer used to serialize messages to.
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Protocol type used by Bond. Currently only binary protocols are supported.
        /// </summary>
        public ProtocolType Protocol { get; }

        public BondSerializerSettings(int bufferSize, ProtocolType protocol)
        {
            BufferSize = bufferSize;
            Protocol = protocol;
        }

        public BondSerializerSettings WithBufferSize(int bufferSize) => new BondSerializerSettings(bufferSize, Protocol);
        public BondSerializerSettings WithProtocol(ProtocolType protocol) => new BondSerializerSettings(BufferSize, protocol);
    }
}