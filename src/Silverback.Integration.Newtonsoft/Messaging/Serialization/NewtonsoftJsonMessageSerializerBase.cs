﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Silverback.Messaging.Messages;

namespace Silverback.Messaging.Serialization
{
    /// <summary>
    ///     The base class for <see cref="NewtonsoftJsonMessageSerializer" /> and
    ///     <see cref="NewtonsoftJsonMessageSerializer{TMessage}" />.
    /// </summary>
    public abstract class NewtonsoftJsonMessageSerializerBase : IMessageSerializer
    {
        /// <summary>
        ///     Gets or sets the message encoding. The default is UTF8.
        /// </summary>
        [DefaultValue("UTF8")]
        public MessageEncoding Encoding { get; set; } = MessageEncoding.UTF8;

        /// <summary>
        ///     Gets or sets the settings to be applied to the Json.NET serializer.
        /// </summary>
        public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };

        /// <inheritdoc cref="IMessageSerializer.Serialize" />
        [SuppressMessage("", "SA1011", Justification = Justifications.NullableTypesSpacingFalsePositive)]
        public abstract byte[]? Serialize(
            object? message,
            MessageHeaderCollection messageHeaders,
            MessageSerializationContext context);

        /// <inheritdoc cref="IMessageSerializer.Deserialize" />
        [SuppressMessage("", "SA1011", Justification = Justifications.NullableTypesSpacingFalsePositive)]
        public abstract (object?, Type) Deserialize(
            byte[]? message,
            MessageHeaderCollection messageHeaders,
            MessageSerializationContext context);

        /// <inheritdoc cref="IMessageSerializer.SerializeAsync" />
        [SuppressMessage("", "SA1011", Justification = Justifications.NullableTypesSpacingFalsePositive)]
        public virtual Task<byte[]?> SerializeAsync(
            object? message,
            MessageHeaderCollection messageHeaders,
            MessageSerializationContext context) =>
            Task.FromResult(Serialize(message, messageHeaders, context));

        /// <inheritdoc cref="IMessageSerializer.DeserializeAsync" />
        [SuppressMessage("", "SA1011", Justification = Justifications.NullableTypesSpacingFalsePositive)]
        public virtual Task<(object?, Type)> DeserializeAsync(
            byte[]? message,
            MessageHeaderCollection messageHeaders,
            MessageSerializationContext context) =>
            Task.FromResult(Deserialize(message, messageHeaders, context));

        /// <summary>
        ///     Maps the <see cref="MessageEncoding" /> to the <see cref="System.Text.Encoding" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Text.Encoding" /> that matches the current <see cref="MessageEncoding" />.
        /// </returns>
        protected Encoding GetSystemEncoding() =>
            Encoding switch
            {
                MessageEncoding.Default => System.Text.Encoding.Default,
                MessageEncoding.ASCII => System.Text.Encoding.ASCII,
                MessageEncoding.UTF8 => System.Text.Encoding.UTF8,
                MessageEncoding.UTF32 => System.Text.Encoding.UTF32,
                MessageEncoding.Unicode => System.Text.Encoding.Unicode,
                _ => throw new InvalidOperationException("Unhandled encoding.")
            };
    }
}