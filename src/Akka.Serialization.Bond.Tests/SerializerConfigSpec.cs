using Akka.Actor;
using Akka.Configuration;
using FluentAssertions;
using Xunit;

namespace Akka.Serialization.Bond.Tests
{

    public interface IBond { }

    public class SerializerConfigSpec 
    {
        private const string Common = @"
            akka.actor {
                serializers.bond = ""Akka.Serialization.Bond.BondSerializer, Akka.Serialization.Bond""
                serialization-bindings {
		            ""Akka.Serialization.Bond.Tests.IBond, Akka.Serialization.Bond.Tests"" = bond
                }
            }";

        [Theory]
        [InlineData("simple", BondSerializerSettings.ProtocolType.Simple)]
        [InlineData("fast", BondSerializerSettings.ProtocolType.Fast)]
        [InlineData("compact", BondSerializerSettings.ProtocolType.Compact)]
        public void BondSerializerSettings_must_apply_correct_protocol_from_HOCON(string protocol, BondSerializerSettings.ProtocolType expected)
        {
            var conf = ConfigurationFactory.ParseString(Common).WithFallback($"akka.actor.serialization-settings.bond {{ protocol = {protocol} }}");
            using (var system = ActorSystem.Create(nameof(SerializerConfigSpec), conf))
            {
                var serializer = (BondSerializer)system.Serialization.FindSerializerForType(typeof(IBond));
                var settings = serializer.Settings;

                settings.Protocol.Should().Be(expected);
            }
        }
    }
}