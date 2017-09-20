//-----------------------------------------------------------------------
// <copyright file="BondSerializer.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) 2017 Bartosz Sypytkowski <https://github.com/Horusiath>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;

namespace Akka.Serialization.Bond
{
    /// <summary>
    /// Microsoft Bond serializer compatible with Akka.NET API.
    /// </summary>
    public sealed class BondSerializer : Akka.Serialization.Serializer
    {
        internal static AsyncLocal<ActorSystem> LocalSystem = new AsyncLocal<ActorSystem>();

        private readonly ConcurrentDictionary<Type, ITypeSerializer> cache = new ConcurrentDictionary<Type, ITypeSerializer>();
        private readonly Func<Type, ITypeSerializer> typeSerializerFactory;

        public BondSerializer(Akka.Actor.ExtendedActorSystem system) : this(system, BondSerializerSettings.Default) { }
        public BondSerializer(Akka.Actor.ExtendedActorSystem system, Config config) : this(system, BondSerializerSettings.Create(config)) { }

        public BondSerializer(Akka.Actor.ExtendedActorSystem system, BondSerializerSettings settings) : base(system)
        {
            this.Settings = settings;
            this.typeSerializerFactory = ConstructTypeSerializerFactory(settings);

            LocalSystem.Value = system;
        }

        public BondSerializerSettings Settings { get; }
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
                case BondSerializerSettings.ProtocolType.Simple: return type => new SimpleBinaryTypeSerializer(type, settings);
                case BondSerializerSettings.ProtocolType.Fast: return type => new FastBinaryTypeSerializer(type, settings);
                case BondSerializerSettings.ProtocolType.Compact: return type => new CompactBinaryTypeSerializer(type, settings);
                default: throw new NotSupportedException($"Protocol type of {settings.Protocol} is not supported by {nameof(BondSerializer)}");
            }
        }
    }
}
