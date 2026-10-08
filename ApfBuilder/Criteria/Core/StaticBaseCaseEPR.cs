using ApfBuilder.Criteria.Core.Interfaces;
using DataBaseModels.ApfBaseEntities;
using Exceptions.ApfBuilder;
using System;
using static ApfBuilder.Criteria.CriterionAttribute;

namespace ApfBuilder.Criteria.Core
{
    [EmergencyAPF]
    public sealed class StaticBaseCaseEPR : CriterionBase, IBaseCaseCriterion
    {
        private const double EprCoeff = 0.92;

        public static ICriterion CreateStandard(
            PreFaultConditions preF)
        {
            return new StaticBaseCaseEPR
                (
                    preF,
                    preF.LimitPowerFlow * EprCoeff
                );
        }

        public static ICriterion CreateForcedState(
            PreFaultConditions preF)
        {
            return new StaticBaseCaseEPR
                (
                    preF,
                    preF.IrOscExpressions != null
                        ? preF.LimitPowerFlow * EprCoeff - 
                            preF.IrOscExpressions * 2
                        : preF.LimitPowerFlow * EprCoeff
                );
        }

        public override CriterionType Type
            => CriterionType.StaticBaseCaseEPR;

        public Conditions Condition { get; }

        private StaticBaseCaseEPR(PreFaultConditions preF, double? value)
            : base
            (
                  preF?.BranchGroupVsBranchGroupScheme
                      ?.BranchGroup
                      ?.RoundValue,
                  value
            )
        {
            try
            {
                Name = "8% P, исходная схема";
                Condition = preF.ConditionsStatic;
            }
            catch (Exception ex)
            {
                throw new CriterionException(
                    $"Ошибка создания критерия '{Type}'", ex);
            }
        }
    }
}
