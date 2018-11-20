﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Silverback.Messaging;
using Silverback.Messaging.Configuration;
using Silverback.Messaging.ErrorHandling;
using Silverback.Messaging.Messages;
using Silverback.Tests.TestTypes;
using Silverback.Tests.TestTypes.Domain;

namespace Silverback.Tests.Messaging.ErrorHandling
{
    [TestFixture]
    public class ErrorPolicyChainTests
    {
        private readonly ErrorPolicyBuilder _errorPolicyBuilder = new ErrorPolicyBuilder(new ServiceCollection().BuildServiceProvider(), NullLoggerFactory.Instance);

        [Test]
        public void ChainingTest()
        {
            var testPolicies = new[]
            {
                new TestErrorPolicy(),
                new TestErrorPolicy(),
                new TestErrorPolicy(),
                new TestErrorPolicy(),
                new TestErrorPolicy()
            };

            var chain = _errorPolicyBuilder.Chain(
                _ => testPolicies[0],
                _ => testPolicies[1],
                _ => testPolicies[2],
                _ => testPolicies[3],
                _ => testPolicies[4]);
            
            chain.TryHandleMessage(
                Envelope.Create(new TestEventOne()),
                _ => throw new Exception("retry, please"));

            Assert.That(testPolicies.Count(x => x.Applied), Is.EqualTo(1));
        }

        [Test]
        public void ChainingTest2()
        {
            var testPolicy = new TestErrorPolicy();

            var chain = _errorPolicyBuilder.Chain(
                builder => builder.Retry(1),
                builder => builder.Retry(1),
                builder => builder.Retry(1),
                builder => builder.Retry(1),
                _ => testPolicy);

            chain.TryHandleMessage(
                Envelope.Create(new TestEventOne()),
                _ => throw new Exception("retry, please"));

            Assert.That(testPolicy.Applied);
        }

        [Test]
        public void ChainingTest3()
        {
            var tryCount = 0;

            var testPolicy = new TestErrorPolicy();
            var chain = _errorPolicyBuilder.Chain(
                builder => builder.Retry(3),
                _ => testPolicy);

            chain.TryHandleMessage(
                Envelope.Create(new TestEventOne()),
                _ =>
                {
                    tryCount++;
                    throw new Exception("retry, please");
                });

            Assert.That(testPolicy.Applied, Is.True);
            Assert.That(tryCount, Is.EqualTo(4));
        }

        [Test]
        public void ChainingTest4()
        {
            var tryCount = 0;

            var chain = _errorPolicyBuilder.Chain(
                builder => builder.Retry(3),
                builder => builder.Skip());

            chain.TryHandleMessage(
                Envelope.Create(new TestEventOne()),
                _ =>
                {
                    tryCount++;
                    throw new Exception("retry, please");
                });

            Assert.That(tryCount, Is.EqualTo(4));
        }

        [Test]
        public void ChainingTest5()
        {
            var tryCount = 0;

            var chain = _errorPolicyBuilder.Chain(
                builder => builder.Retry(3).ApplyTo<InvalidOperationException>(),
                builder => builder.Skip());

            chain.TryHandleMessage(
                Envelope.Create(new TestEventOne()),
                _ =>
                {
                    tryCount++;
                    if (tryCount < 2)
                        throw new InvalidOperationException();
                    throw new Exception("retry, please");
                });

            Assert.That(tryCount, Is.EqualTo(2));
        }
    }
}