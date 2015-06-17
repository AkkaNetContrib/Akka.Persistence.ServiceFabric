﻿//-----------------------------------------------------------------------
// <copyright file="InternalExtensions.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) Microsoft Corporation. All rights reserved. 
//     Copyright (C) Nethouse Örebro AB <http://akka.nethouse.se>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Akka.Persistence.ServiceFabric
{
    internal static class InternalExtensions
    {
        public static string QualifiedTypeName(this Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

    }
}