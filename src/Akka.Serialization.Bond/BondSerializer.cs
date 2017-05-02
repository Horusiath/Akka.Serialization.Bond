using System;
using System.Collections.Concurrent;
using System.IO;
using Akka.Actor;
using Bond.IO.Unsafe;
using Akka.Configuration;
using Bond.Protocols;

namespace Akka.Serialization.Bond
{
    using global::Bond;

    /// <summary>
    /// Microsoft Bond serializer compatible with Akka.NET API.
    /// </summary>
    public sealed class BondSerializer : Akka.Serialization.Serializer
    {
        private readonly BondSerializerSettings settings;
        private readonly ConcurrentDictionary<Type, Serializer<FastBinaryWriter<OutputBuffer>>> serializerCache = new ConcurrentDictionary<Type, Serializer<FastBinaryWriter<OutputBuffer>>>();
        private readonly ConcurrentDictionary<Type, Deserializer<FastBinaryReader<InputBuffer>>> deserializerCache = new ConcurrentDictionary<Type, Deserializer<FastBinaryReader<InputBuffer>>>();

        public BondSerializer(Akka.Actor.ExtendedActorSystem system) : this(system, BondSerializerSettings.Default) { }
        public BondSerializer(Akka.Actor.ExtendedActorSystem system, Config config) : this(system, BondSerializerSettings.Create(config)) { }

        public BondSerializer(Akka.Actor.ExtendedActorSystem system, BondSerializerSettings settings) : base(system)
        {
            this.settings = settings;
            this.Identifier = SerializerIdentifierHelper.GetSerializerIdentifierFromConfig(this.GetType(), system);
        }

        public override int Identifier { get; }
        public override bool IncludeManifest => false;
        
        public override byte[] ToBinary(object obj)
        {
            var type = obj.GetType();
            var serializer = serializerCache.GetOrAdd(type, t => new Serializer<FastBinaryWriter<OutputBuffer>>(t));
            var outputBuffer = new OutputBuffer(settings.BufferSize);
            var writer = new FastBinaryWriter<OutputBuffer>(outputBuffer);
            serializer.Serialize(obj, writer);

            return outputBuffer.Data.Array;
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            if (type == null) throw new InvalidOperationException($"{GetType()}.FromBinary requires type to be provided");

            var deserializer = deserializerCache.GetOrAdd(type, t => new Deserializer<FastBinaryReader<InputBuffer>>(t));
            var inputBuffer = new InputBuffer(bytes);
            var reader = new FastBinaryReader<InputBuffer>(inputBuffer);
            var obj = deserializer.Deserialize(reader);
            return obj;
        }
    }
}
