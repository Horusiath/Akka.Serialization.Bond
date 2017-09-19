//-----------------------------------------------------------------------
// <copyright file="BondTypeAliasConverter.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) 2017 Bartosz Sypytkowski <https://github.com/Horusiath>
// </copyright>
//-----------------------------------------------------------------------

using System.Threading;
using Akka.Actor;
using Akka.Util;

namespace Akka.Serialization.Bond
{
    public static class BondTypeAliasConverter
    {
        public static ISurrogate Convert(ISurrogated surrogated, ISurrogate surrogate)
        {
            var system = BondSerializer.LocalSystem.Value;
            return surrogated.ToSurrogate(system);
        }

        public static ISurrogated Convert(ISurrogate surrogate, ISurrogated surrogated)
        {
            var system = BondSerializer.LocalSystem.Value;
            return surrogate.FromSurrogate(system);
        }
    }
}