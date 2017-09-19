using System;
using System.Collections.Concurrent;
using Akka.Configuration;

namespace Akka.Serialization.Bond
{
    /// <summary>
    /// Microsoft Bond serializer compatible with Akka.NET API.
    /// </summary>
    public sealed class BondSerializer : Akka.Serialization.Serializer
    {
        private readonly BondSerializerSettings settings;
        private readonly ConcurrentDictionary<Type, ITypeSerializer> cache = new ConcurrentDictionary<Type, ITypeSerializer>();
        private readonly Func<Type, ITypeSerializer> typeSerializerFactory;

        public BondSerializer(Akka.Actor.ExtendedActorSystem system) : this(system, BondSerializerSettings.Default) { }
        public BondSerializer(Akka.Actor.ExtendedActorSystem system, Config config) : this(system, BondSerializerSettings.Create(config)) { }

        public BondSerializer(Akka.Actor.ExtendedActorSystem system, BondSerializerSettings settings) : base(system)
        {
            this.settings = settings;
            this.typeSerializerFactory = ConstructTypeSerializerFactory(settings);
        }

        public override int Identifier { get; } = 151;
        public override bool IncludeManifest => false;
        
        public override byte[] ToBinary(object obj)
        {
            var type = obj.GetType();
            var serializer = cache.GetOrAdd(type, typeSerializerFactory);
            return serializer.SerializeObject(obj);
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            if (type == null) throw new InvalidOperationException($"{nameof(BondSerializer)}.FromBinary requires type to be provided");

            var deserializer = cache.GetOrAdd(type, typeSerializerFactory);
            return deserializer.DeserializeObject(bytes);
        }

        private static Func<Type, ITypeSerializer> ConstructTypeSerializerFactory(BondSerializerSettings settings)
        {
            switch (settings.Protocol)
            {
                case BondSerializerSettings.ProtocolType.Simple: return type => new SimpleBinaryTypeSerializer(type);
                case BondSerializerSettings.ProtocolType.Fast: return type => new FastBinaryTypeSerializer(type);
                case BondSerializerSettings.ProtocolType.Compact: return type => new CompactBinaryTypeSerializer(type);
                default: throw new NotSupportedException($"Protocol type of {settings.Protocol} is not supported by {nameof(BondSerializer)}");
            }
        }
    }
}
