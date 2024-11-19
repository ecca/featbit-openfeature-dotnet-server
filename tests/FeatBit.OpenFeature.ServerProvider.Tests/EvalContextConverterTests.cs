using System;
using System.Linq;
using FeatBit.Sdk.Server.Model;
using FluentAssertions;
using OpenFeature.Model;
using Xunit;

namespace FeatBit.OpenFeature.ServerProvider.Tests
{
    public class EvalContextConverterTests
    {
        private readonly EvalContextConverter _converter = new();

        [Fact]
        public void Conversion_Should_Return_Equal()
        {
            var key = Guid.NewGuid();
            var name = "John Doe";

            var evaluationContext = EvaluationContext.Builder()
                .Set("key", key.ToString())
                .Set("name", name)
                .Build();

            var convertedUser = _converter.ToFbUser(evaluationContext);

            var expectedUser = FbUser.Builder(key.ToString())
                .Name(name)
                .Build();

            convertedUser
                .Should()
                .BeEquivalentTo(expectedUser);
        }

        [Fact]
        public void Conversion_CustomAttribute_Added()
        {
            var customAttributeKey = "ca";
            var customAttributeValue = "check";
            
            var evaluationContext = EvaluationContext.Builder()
                .Set("keyId", "26C73595-2C8A-4D47-AED6-8820A31E15C6")
                .Set("name", "name")
                .Set(customAttributeKey, customAttributeValue)
                .Build();

            var fbUser = _converter.ToFbUser(evaluationContext);

            customAttributeValue
                .Should()
                .Be(fbUser.Custom.FirstOrDefault(kvp => kvp.Key == customAttributeKey).Value);
        }
    }
}