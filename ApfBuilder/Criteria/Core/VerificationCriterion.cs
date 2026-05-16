using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Services;
using DataBaseModels.ApfBaseEntities;
using Exceptions.ApfBuilder;
using Extensions;
using System;
using System.Collections.Generic;
using static ApfBuilder.Criteria.CriterionAttribute;

namespace ApfBuilder.Criteria.Core
{
    [AdditionalAPF]
    public sealed class VerificationCriterion : CriterionBase, IEmergencyResponseCriterion
    {
        public static ICriterion Create(PostFaultConditions postF)
            => new VerificationCriterion(postF);

        public override CriterionType Type => CriterionType.Verification;

        public Static StaticCriterion { get; }

        public double? LimitPowerFlow { get; }

        public int? IrOscExpressions { get; }

        public IEnumerable<IEmergencyResponse> EmergencyResponse { get; }

        public Conditions Condition { get; }

        public Disturbances Disturbance { get; }

        public FrequencyPowerFlow FrequencyCriterion { get; }

        public double? MinValueER { get; }

        public double? MaxValueER { get; }

        private VerificationCriterion(PostFaultConditions postF)
            : base
            (
                  postF.PreFaultConditions
                        ?.BranchGroupVsBranchGroupScheme
                        ?.BranchGroup
                        ?.RoundValue
            )
        {
            try
            {
                FrequencyCriterion = postF?.FrequencyPowerFlow;

                StaticCriterion = (Static)Static.Create(postF);
                LimitPowerFlow = postF?.PreFaultConditions?.LimitPowerFlow;
                IrOscExpressions = postF.PreFaultConditions?.IrOscExpressions;

                Condition = postF?.Conditions;
                Disturbance = postF.Disturbances;
                EmergencyResponse = EmergencyResponseHandler.
                    ProcessHandler(
                        base.RoundValue, this.Type, postF.APNU, postF.DAR);

                Name = postF.FrequencyPowerFlow?.PowerConsumptionName;
                Value = postF.FrequencyPowerFlow?.PowerConsumptionFactor;
                MinValue = (Value * postF.FrequencyPowerFlow?.MinValue)
                    .Round(base.RoundValue);
                MaxValue = (Value * postF.FrequencyPowerFlow?.MaxValue)
                    .Round(base.RoundValue);

                MinValueER = MinValue;
                MaxValueER = MaxValue;
                ComplexMaxValue = MaxValue;
                ComplexMinValue = MinValue;
                foreach (var e in EmergencyResponse)
                {
                    MinValueER += e.MinValue;
                    MaxValueER += e.MaxValue;
                    ComplexMaxValue += e.MaxValue;
                    ComplexMinValue += e.MinValue;
                }
            }
            catch (Exception ex)
            {
                throw new CriterionException(
                    $"Ошибка создания критерия '{Type}'", ex);
            }
        }
    }
}
