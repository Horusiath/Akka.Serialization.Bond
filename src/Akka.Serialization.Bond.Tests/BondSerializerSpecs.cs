﻿using Akka.Actor;
using Akka.Configuration;
using Akka.Util;
using Bond;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Akka.Serialization.Bond.Tests
{
    [Schema]
    public sealed class SmallRecord
    {
        [Id(0)]
        public int X { get; set; }

        [Id(1)]
        public int Y { get; set; }

        public SmallRecord()
        {
        }

        public SmallRecord(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [Schema]
    public sealed class LargeRecord
    {
        [Id(0)]
        public string FirstName { get; set; }

        [Id(1)]
        public string LastName { get; set; }

        [Id(2)]
        public SmallRecord[] Records { get; set; }

        public LargeRecord()
        {
        }

        public LargeRecord(string firstName, string lastName, SmallRecord[] records)
        {
            FirstName = firstName;
            LastName = lastName;
            Records = records;
        }
    }

    public class PlainRecord
    {
        public int Id { get; }
        public string Value { get; }

        public PlainRecord()
        {
        }

        public PlainRecord(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }

    public class BondSerializerSpecs : TestKit.Xunit2.TestKit
    {
        private static readonly Config Config = ConfigurationFactory.ParseString(@"
        akka.actor {
	        serializers.bond = ""Akka.Serialization.Bond.BondSerializer, Akka.Serialization.Bond""

            serialization - bindings {
		        # don't use Bond as a default serializer
		        ""MyNamespace.MyMessage, MyAssembly"" = bond

            }
            serialization-identifiers {
		        # pick some unused number to recognize this identifier
		        ""Akka.Serialization.Bond.BondSerializer, Akka.Serialization.Bond"" = 111 
	        }
            serialization-settings {
		        bond {
			        # Default size of an buffer used as intermediate store for serialized messages.
			        buffer-size = 1024
		        }
	        }
        }");

        public BondSerializerSpecs(ITestOutputHelper output) : base(Config, output: output)
        {
        }

        [Theory]
        [InlineData(BondSerializerSettings.ProtocolType.Simple)]
        [InlineData(BondSerializerSettings.ProtocolType.Fast)]
        [InlineData(BondSerializerSettings.ProtocolType.Compact)]
        public void BondSerializer_must_serialize_small_records_marked_with_Bond_schema_attributes(BondSerializerSettings.ProtocolType protocol)
        {
            var settings = BondSerializerSettings.Default.WithProtocol(protocol);
            var serializer = new BondSerializer((ExtendedActorSystem)Sys, settings);

            Roundtrip(serializer, new SmallRecord(1, 2));
        }

        [Theory]
        [InlineData(BondSerializerSettings.ProtocolType.Simple)]
        [InlineData(BondSerializerSettings.ProtocolType.Fast)]
        [InlineData(BondSerializerSettings.ProtocolType.Compact)]
        public void BondSerializer_must_serialize_large_records_marked_with_Bond_schema_attributes(BondSerializerSettings.ProtocolType protocol)
        {
            var large = new LargeRecord(
                firstName: "John",
                lastName: "Doe",
                records: new []
                {
                    new SmallRecord(1, 2),
                    new SmallRecord(2, 3),
                    new SmallRecord(3, 4),
                    new SmallRecord(4, 5),
                    new SmallRecord(5, 6),
                });

            var settings = BondSerializerSettings.Default.WithProtocol(protocol);
            var serializer = new BondSerializer((ExtendedActorSystem)Sys, settings);
            Roundtrip(serializer, large);
        }
        
        private void Roundtrip<T>(BondSerializer serializer, T value)
        {
            var bytes = serializer.ToBinary(value);
            var deserialized = serializer.FromBinary(bytes, typeof(T));

            deserialized.Should().Equals(value);
        }
    }
}
