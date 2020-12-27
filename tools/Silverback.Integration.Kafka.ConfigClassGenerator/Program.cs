﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.IO;
using System.Reflection;
using Confluent.Kafka;

namespace Silverback.Integration.Kafka.ConfigClassGenerator
{
    internal static class Program
    {
        private static void Main()
        {
            var assembly = Assembly.GetAssembly(typeof(ClientConfig));

            if (assembly == null)
                throw new InvalidOperationException("Couldn't load ClientConfig assembly.");

            var xmlDocumentationPath = Path.Combine(
                Path.GetDirectoryName(assembly.Location)!,
                "Confluent.Kafka.xml");

            WriteBaseClass();

            Console.WriteLine();

            Console.Write(
                new ProxyClassGenerator(
                        typeof(ClientConfig),
                        "ConfluentClientConfigProxy",
                        null,
                        null,
                        xmlDocumentationPath,
                        false)
                    .Generate());

            Console.WriteLine();

            Console.Write(
                new ProxyClassGenerator(
                        typeof(ConsumerConfig),
                        "ConfluentConsumerConfigProxy",
                        "ConfluentClientConfigProxy<Confluent.Kafka.ConsumerConfig>",
                        "Confluent.Kafka.ConsumerConfig",
                        xmlDocumentationPath,
                        false)
                    .Generate());

            Console.WriteLine();

            Console.Write(
                new ProxyClassGenerator(
                        typeof(ProducerConfig),
                        "ConfluentProducerConfigProxy",
                        "ConfluentClientConfigProxy<Confluent.Kafka.ProducerConfig>",
                        "Confluent.Kafka.ProducerConfig",
                        xmlDocumentationPath,
                        false)
                    .Generate());
        }

        private static void WriteBaseClass()
        {
            Console.WriteLine("    /// <summary>");
            Console.WriteLine(
                "    ///     The base class for the types wrapping the the <see cref=\"Confluent.Kafka.ClientConfig\" />.");
            Console.WriteLine("    /// </summary>");
            Console.WriteLine("    [SuppressMessage(\"\", \"SA1649\", Justification = \"Autogenerated all at once\")]");
            Console.WriteLine(
                "    public abstract class ConfluentClientConfigProxyBase : IValidatableEndpointSettings");
            Console.WriteLine("    {");
            Console.WriteLine(
                "        internal static readonly ConfigurationDictionaryEqualityComparer<string, string> ConfluentConfigEqualityComparer = new();");
            Console.WriteLine();
            Console.WriteLine("        /// <inheritdoc cref=\"IValidatableEndpointSettings.Validate\" />");
            Console.WriteLine("        public abstract void Validate();");
            Console.WriteLine("    }");
        }
    }
}
