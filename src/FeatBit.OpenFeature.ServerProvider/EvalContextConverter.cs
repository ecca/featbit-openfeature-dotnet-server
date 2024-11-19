using System.Collections.Immutable;
using FeatBit.Sdk.Server.Model;
using OpenFeature.Model;

namespace FeatBit.OpenFeature.ServerProvider
{
    /// <summary>
    /// Class which converts <see cref="EvaluationContext"/> objects into <see cref="FbUser"/> objects.
    /// </summary>
    internal class EvalContextConverter()
    {
        /// <summary>
        /// Convert an <see cref="EvaluationContext"/> into a <see cref="FbUser"/>.
        /// </summary>
        /// <param name="evaluationContext">The evaluation context to convert</param>
        /// <returns>A converted context</returns>
        public FbUser ToFbUser(EvaluationContext evaluationContext)
        {
            return BuildSingleFbUser(evaluationContext.AsDictionary());
        }

        private FbUser BuildSingleFbUser(IImmutableDictionary<string, Value> attributes)
        {
            attributes.TryGetValue("key", out var keyAttribute);
            attributes.TryGetValue("keyId", out var keyIdAttribute);
            var finalKey = (keyIdAttribute ?? keyAttribute)?.AsString;
            
            if (keyAttribute != null && keyIdAttribute != null)
            {
                // logger.LogWarning("The EvaluationContext contained both a 'keyId' and a 'key' attribute. The 'key'" +
                //           "attribute will be discarded.");
            }

            if (finalKey == null)
            {
                // logger.LogWarning("The EvaluationContext must contain either a 'keyId' or a 'key' and the type" +
                //            "must be a string.");
            }
            
            var fbUserBuilder = FbUser.Builder(finalKey);
            foreach (var kvp in attributes)
            {
                ProcessValue(kvp.Key, kvp.Value, fbUserBuilder);
            }

            return fbUserBuilder.Build();
        }
        
        /// <summary>
        /// Extract a value and add it to a context builder.
        /// </summary>
        /// <param name="key">The key to add to the context if the value can be extracted</param>
        /// <param name="value">The value to extract</param>
        /// <param name="builder">The context builder to add the value to</param>
        private static void ProcessValue(string key, Value value, IFbUserBuilder builder)
        {
            var fbValue = value.AsString;

            switch (key)
            {
                case "keyId":
                case "key":
                    break;
                case "name":
                    builder.Name(fbValue);
                    break;
                default:
                    builder.Custom(key, fbValue);
                    break;
            }
        }
    }
}