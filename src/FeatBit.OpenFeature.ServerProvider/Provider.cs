using System;
using System.Threading;
using System.Threading.Tasks;
using FeatBit.Sdk.Server;
using FeatBit.Sdk.Server.Options;
using Microsoft.Extensions.Logging;
using OpenFeature;
using OpenFeature.Model;

namespace FeatBit.OpenFeature.ServerProvider
{
    /// <summary>
    /// An OpenFeature <see cref="FeatureProvider"/> which enables the use of the FeatBit Server-Side SDK for .NET
    /// with OpenFeature.
    /// </summary>
    /// <example>
    ///var config = new FbOptionsBuilder("replace-with-your-env-secret")
    ///             .Event(new Uri("replace-with-your-event-url"))
    ///             .Streaming(new Uri("replace-with-your-streaming-url"))
    ///             .Build();
    ///
    ///     var fbClient  = new FbClient(config);
    ///     var provider = new Provider(fbClient);
    ///
    ///     OpenFeature.Api.Instance.SetProvider(provider);
    ///
    ///     var client = OpenFeature.Api.Instance.GetClient();
    /// </example>
    public sealed class Provider : FeatureProvider
    {
        private readonly IFbClient _client;
        private readonly EvalContextConverter _contextConverter;

        /// <summary>
        /// Construct a new instance of the provider.
        /// </summary>
        /// <param name="client">The <see cref="IFbClient"/> instance</param>
        /// <param name="logger">An optional <see cref="ILogger"/></param>
        /// <param name="config">An optional <see cref="FbOptions"/></param>
        public Provider(IFbClient client, ILogger logger = null, FbOptions config = null)
        {
            _client = client;
            _contextConverter = new EvalContextConverter();
        }

        #region FeatureProvider Implementation

        /// <inheritdoc />
        public override Metadata GetMetadata() => new("FeatBit.OpenFeature.ServerProvider");

        /// <inheritdoc />
        public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(
            string flagKey,
            bool defaultValue,
            EvaluationContext context = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_client
                .BoolVariationDetail(flagKey, _contextConverter.ToFbUser(context), defaultValue)
                .ToResolutionDetails(flagKey)
            );
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<string>> ResolveStringValueAsync(
            string flagKey,
            string defaultValue,
            EvaluationContext context = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_client
                .StringVariationDetail(flagKey, _contextConverter.ToFbUser(context), defaultValue)
                .ToResolutionDetails(flagKey));
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(string flagKey, int defaultValue,
            EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_client
                .IntVariationDetail(flagKey, _contextConverter.ToFbUser(context), defaultValue)
                .ToResolutionDetails(flagKey));
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(string flagKey, double defaultValue,
            EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_client
                .DoubleVariationDetail(flagKey, _contextConverter.ToFbUser(context), defaultValue)
                .ToResolutionDetails(flagKey));
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(string flagKey, Value defaultValue,
            EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Task InitializeAsync(EvaluationContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
        
        /// <inheritdoc />
        public override async Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            await _client.CloseAsync();
        }

        #endregion
    }
}