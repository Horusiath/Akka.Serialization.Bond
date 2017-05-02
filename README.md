## Akka.Serialization.Bond

A serialization bindings for Akka.NET and [Microsoft Bond](https://microsoft.github.io/bond/manual/bond_cs.html) framework.

### Configuration

HOCON config:

```hocon
akka.actor {
	serializers.bond = "Akka.Serialization.Bond.BondSerializer, Akka.Serialization.Bond"
	serialization-bindings {
		# don't use Bond as a default serializer
		"MyNamespace.MyMessage, MyAssembly" = bond
	}
	serialization-identifiers {
		# pick some unused number to recognize this identifier
		"Akka.Serialization.Bond.BondSerializer, Akka.Serialization.Bond" = 111 
	}
	serialization-settings {
		bond {
			# Default size of an buffer used as intermediate store for serialized messages.
			buffer-size = 1024
		}
	}
}
```