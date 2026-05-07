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
    public sealed class Frequency : CriterionBase, IFrequencyCriterion, IEmergencyResponseCriterion
    {
        public static ICriterion Create(PostFaultConditions postF)
             => new Frequency(postF);

        public override CriterionType Type => CriterionType.Frequency;

        public bool? CanUse { get; }

        public (string Value, string Description) FullValue { get; }

        public IEnumerable<IEmergencyResponse> EmergencyResponse { get; }

        public Conditions Condition { get; }

        public Disturbances Disturbance { get; }

        public double? MinValueER { get; }

        public double? MaxValueER { get; }

        private Frequency(PostFaultConditions postF) 
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
                Condition = postF.Conditions;
                Disturbance = postF.Disturbances;
                EmergencyResponse = EmergencyResponseHandler.
                    ProcessHandler(
                        base.RoundValue, this.Type, postF.APNU, postF.DAR);

                CanUse = postF.FrequencyPowerFlow?.Normal == true;

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

                FullValue =
                (
                    $"{postF.FrequencyPowerFlow?.FrequencyFormalNameProxy}" +
                    (postF?.PreFaultConditions?.IrOscExpressions != null ?
                    " - ΔPнк" : ""),
                    $"{postF.FrequencyPowerFlow?.PowerConsumptionFactor * 100}" +
                    $"% {Name}" +
                    (postF?.PreFaultConditions?.IrOscExpressions != null ?
                    " - ΔPнк" : "")
                );
            }
            catch (Exception ex)
            {
                throw new CriterionException(
                    $"Ошибка создания критерия '{Type}'", ex);
            }
        }
    }
}
