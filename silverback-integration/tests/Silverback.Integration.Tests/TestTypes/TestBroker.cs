﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Silverback.Messaging;
using Silverback.Messaging.Broker;
using Silverback.Messaging.Serialization;

namespace Silverback.Tests.TestTypes
{
    public class TestBroker : Broker
    {
        public TestBroker(IMessageSerializer serializer) : base(serializer, NullLoggerFactory.Instance)
        {
        }

        public List<SentMessage> SentMessages { get; } = new List<SentMessage>();

        protected override Producer InstantiateProducer(IEndpoint endpoint) => new TestProducer(this, endpoint);

        protected override Consumer InstantiateConsumer(IEndpoint endpoint) => new TestConsumer(this, endpoint);

        protected override void Connect(IEnumerable<IConsumer> consumers)
        {
            consumers.Cast<TestConsumer>().ToList().ForEach(c => c.IsReady = true);
        }

        protected override void Disconnect(IEnumerable<IConsumer> consumers)
        {
        }

        public class SentMessage
        {
            public SentMessage(byte[] message, IEndpoint endpoint)
            {
                Message = message;
                Endpoint = endpoint;
            }

            public byte[] Message { get; }
            public IEndpoint Endpoint { get; }
        }
    }
}