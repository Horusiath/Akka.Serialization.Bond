//-----------------------------------------------------------------------
// <copyright file="Messages.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) 2017 Bartosz Sypytkowski <https://github.com/Horusiath>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Akka.Actor;

namespace Akka.Serialization.Bond.Tests
{
    public class ContainerMessage<T>
    {
        public ContainerMessage(T contents)
        {
            Contents = contents;
        }
        public T Contents { get; private set; }
    }

    public class ImmutableMessage
    {
        public string Foo { get; private set; }
        public string Bar { get; private set; }

        public ImmutableMessage()
        {

        }

        protected bool Equals(ImmutableMessage other)
        {
            return string.Equals(Bar, other.Bar) && string.Equals(Foo, other.Foo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ImmutableMessage)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Bar != null ? Bar.GetHashCode() : 0) * 397) ^ (Foo != null ? Foo.GetHashCode() : 0);
            }
        }

        public ImmutableMessage(Tuple<string, string> nonConventionalArg)
        {
            Foo = nonConventionalArg.Item1;
            Bar = nonConventionalArg.Item2;
        }
    }
    public class ImmutableMessageWithPrivateCtor
    {
        public string Foo { get; private set; }
        public string Bar { get; private set; }

        protected ImmutableMessageWithPrivateCtor()
        {
        }

        protected bool Equals(ImmutableMessageWithPrivateCtor other)
        {
            return String.Equals(Bar, other.Bar) && String.Equals(Foo, other.Foo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ImmutableMessageWithPrivateCtor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Bar != null ? Bar.GetHashCode() : 0) * 397) ^ (Foo != null ? Foo.GetHashCode() : 0);
            }
        }

        public ImmutableMessageWithPrivateCtor(Tuple<string, string> nonConventionalArg)
        {
            Foo = nonConventionalArg.Item1;
            Bar = nonConventionalArg.Item2;
        }
    }

    public class SomeMessage
    {
        public IActorRef ActorRef { get; set; }
    }

    public class UntypedContainerMessage : IEquatable<UntypedContainerMessage>
    {
        public bool Equals(UntypedContainerMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Contents, other.Contents);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UntypedContainerMessage)obj);
        }

        public override int GetHashCode() => (Contents != null ? Contents.GetHashCode() : 0);

        public static bool operator ==(UntypedContainerMessage left, UntypedContainerMessage right) => Equals(left, right);

        public static bool operator !=(UntypedContainerMessage left, UntypedContainerMessage right) => !Equals(left, right);

        public object Contents { get; set; }

        public override string ToString() => $"<UntypedContainerMessage {Contents}>";
    }
}