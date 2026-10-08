using ApfBuilder.Criteria.Core.Interfaces;
using DataBaseModels.ApfBaseEntities;
using Exceptions.ApfBuilder;
using System;
using static ApfBuilder.Criteria.CriterionAttribute;

namespace ApfBuilder.Criteria.Core
{
    [FirstAPF]
    public sealed class StaticBaseCaseTPR : CriterionBase, IBaseCaseCriterion
    {
        public static ICriterion Create(PreFaultConditions preF)
             => new StaticBaseCaseTPR(preF);

        public override CriterionType Type
            => CriterionType.StaticBaseCaseTPR;

        public Conditions Condition { get; }

        private StaticBaseCaseTPR(PreFaultConditions preF)
            : base
            (
                  preF?.BranchGroupVsBranchGroupScheme
                      ?.BranchGroup
                      ?.RoundValue,
                  preF.LimitPowerFlow * (1 - preF?.BranchGroupVsBranchGroupScheme
                                            ?.BranchGroup
                                            ?.SafetyFactor / 100) - preF.IrOscExpressions
                    ?? preF.LimitPowerFlow * (1 - preF?.BranchGroupVsBranchGroupScheme
                                            ?.BranchGroup
                                            ?.SafetyFactor / 100),
                  preF.ConditionsStatic
            )
        {
            try
            {
                var sf = preF?.BranchGroupVsBranchGroupScheme
                               ?.BranchGroup
                               ?.SafetyFactor;

                Name = $"{sf}% P, исходная схема";
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
