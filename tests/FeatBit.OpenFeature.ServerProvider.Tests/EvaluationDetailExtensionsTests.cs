using FeatBit.Sdk.Server.Evaluation;
using FluentAssertions;
using OpenFeature.Constant;
using Xunit;

namespace FeatBit.OpenFeature.ServerProvider.Tests
{
    public class EvaluationDetailExtensionsTests
    {
        [Fact]
        public void Evaluation_Handle_Success()
        {
            var evaluationDetail = new EvalDetail<bool>("test-flag", ReasonKind.Fallthrough, "fall through targets and rules", true);
            var resolutionDetail = evaluationDetail.ToResolutionDetails("test-flag");

            resolutionDetail
                .Value
                .Should()
                .BeTrue();

            resolutionDetail
                .Variant
                .Should()
                .Be("True");

            resolutionDetail
                .Reason
                .Should()
                .Be("fall through targets and rules");

            resolutionDetail
                .ErrorType
                .Should()
                .Be(ErrorType.None);
        }

    }
}