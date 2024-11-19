using System;
using FeatBit.Sdk.Server.Evaluation;
using OpenFeature.Constant;
using OpenFeature.Model;

namespace FeatBit.OpenFeature.ServerProvider
{
    /// <summary>
    /// Class containing extension methods used in the conversion of <see cref="EvalDetail{TValue}"/> into
    /// <see cref="ResolutionDetails{T}"/>.
    /// </summary>
    internal static class EvaluationDetailExtensions
    {
        
        /// <summary>
        /// Convert an <see cref="EvalDetail{T}"/> to a <see cref="ResolutionDetails{T}"/>.
        /// </summary>
        /// <param name="detail">The detail to convert</param>
        /// <param name="flagKey">The flag key that was evaluated</param>
        /// <typeparam name="T">The type of the flag evaluation</typeparam>
        /// <returns>The <see cref="EvalDetail{T}"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the kind is not valid</exception>
        public static ResolutionDetails<T> ToResolutionDetails<T>(this EvalDetail<T> detail, string flagKey)
        {
            if (detail.Kind != ReasonKind.Error)
            {
                return new ResolutionDetails<T>(flagKey, detail.Value, ErrorType.None,
                    reason: detail.Reason, variant: detail.Value?.ToString());
            }

            var errorType = ErrorType.General;
            switch (detail.Kind)
            {
                case ReasonKind.ClientNotReady:
                    errorType = ErrorType.ProviderNotReady;
                    break;
                case ReasonKind.WrongType:
                    errorType = ErrorType.TypeMismatch;
                    break;
                case ReasonKind.Off:
                case ReasonKind.Fallthrough:
                case ReasonKind.TargetMatch:
                case ReasonKind.RuleMatch:
                case ReasonKind.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(detail.Kind.ToString());
            }

            return new ResolutionDetails<T>(flagKey, detail.Value, errorType,
                reason: detail.Reason, variant: detail.Value?.ToString());
        }
    }
}