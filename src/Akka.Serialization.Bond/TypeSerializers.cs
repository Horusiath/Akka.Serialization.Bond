//-----------------------------------------------------------------------
// <copyright file="TypeSerializers.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) 2017 Bartosz Sypytkowski <https://github.com/Horusiath>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Bond;
using Bond.IO.Unsafe;
using Bond.Protocols;

namespace Akka.Serialization.Bond
{
    public interface ITypeSerializer
    {
        byte[] SerializeObject(object value);
        object DeserializeObject(byte[] bytes);
    }

    public abstract class TypeSerializer<TWriter, TReader> : ITypeSerializer
        where TWriter : IProtocolWriter
    {
        private readonly RuntimeSchema schema;
        private readonly Serializer<TWriter> serializer;
        private readonly Deserializer<TReader> deserializer;

        protected TypeSerializer(Type type)
        {
            this.schema = Schema.GetRuntimeSchema(type);
            this.serializer = new Serializer<TWriter>(type);
            this.deserializer = new Deserializer<TReader>(type);
        }

        protected abstract TWriter CreateWriter(OutputBuffer buffer);
        protected abstract TReader CreateReader(InputBuffer buffer);

        public byte[] SerializeObject(object value)
        {
            var buffer = new OutputBuffer();
            var writer = CreateWriter(buffer);

            serializer.Serialize(value, writer);

            return CopyBytes(buffer);
        }

        public object DeserializeObject(byte[] binary)
        {
            var buffer = new InputBuffer(binary);
            var reader = CreateReader(buffer);

            var obj = deserializer.Deserialize(reader);
            return obj;
        }

        private static byte[] CopyBytes(OutputBuffer buffer)
        {
            var segment = buffer.Data;
            var bytes = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, bytes, 0, segment.Count);
            return bytes;
        }
    }

    public sealed class SimpleBinaryTypeSerializer : TypeSerializer<SimpleBinaryWriter<OutputBuffer>, SimpleBinaryReader<InputBuffer>>
    {
        public SimpleBinaryTypeSerializer(Type type) : base(type) { }
        protected override SimpleBinaryWriter<OutputBuffer> CreateWriter(OutputBuffer buffer) => new SimpleBinaryWriter<OutputBuffer>(buffer);
        protected override SimpleBinaryReader<InputBuffer> CreateReader(InputBuffer buffer) => new SimpleBinaryReader<InputBuffer>(buffer);
    }

    public sealed class FastBinaryTypeSerializer : TypeSerializer<FastBinaryWriter<OutputBuffer>, FastBinaryReader<InputBuffer>>
    {
        public FastBinaryTypeSerializer(Type type) : base(type) { }
        protected override FastBinaryWriter<OutputBuffer> CreateWriter(OutputBuffer buffer) => new FastBinaryWriter<OutputBuffer>(buffer);
        protected override FastBinaryReader<InputBuffer> CreateReader(InputBuffer buffer) => new FastBinaryReader<InputBuffer>(buffer);
    }
    
    public sealed class CompactBinaryTypeSerializer : TypeSerializer<CompactBinaryWriter<OutputBuffer>, CompactBinaryReader<InputBuffer>>
    {
        public CompactBinaryTypeSerializer(Type type) : base(type) { }
        protected override CompactBinaryWriter<OutputBuffer> CreateWriter(OutputBuffer buffer) => new CompactBinaryWriter<OutputBuffer>(buffer);
        protected override CompactBinaryReader<InputBuffer> CreateReader(InputBuffer buffer) => new CompactBinaryReader<InputBuffer>(buffer);
    }
}