using System;
using Akka.Configuration;

namespace Akka.Serialization.Bond
{
    public class BondSerializerSettings 
    {
        public static readonly BondSerializerSettings Default = new BondSerializerSettings(
            bufferSize: 1024);

        public static BondSerializerSettings Create(Config config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config), "No HOCON config was supplied for BondSerializerSettings");

            return new BondSerializerSettings(
                bufferSize: config.GetInt("buffer-size", 1024));
        }

        /// <summary>
        /// Default size of a buffer used to serialize messages to.
        /// </summary>
        public int BufferSize { get; }

        public BondSerializerSettings(int bufferSize)
        {
            BufferSize = bufferSize;
        }
    }
}