using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using DataBaseModels.ApfBaseEntities;
using System;

namespace ApfBuilder.Criteria.Rule
{
    public sealed class PreFaultRule : CriterionRuleBase
    {
        private static readonly PreFaultRule[] _rules = new[]
        {
            #region rules
            new PreFaultRule(
                Cases(CriterionCase.BaseState, CriterionCase.ForcedState),
                pf => true,
                CurrentSecondary.CreateStandard),

            new PreFaultRule(
                Cases(CriterionCase.BaseState, CriterionCase.ForcedState),
                pf => true,
                CurrentSecondary.CreateAOPO),

            new PreFaultRule(
                Cases(CriterionCase.BaseState),
                pf => true,
                VoltageSecondary.Create),

            new PreFaultRule(
                Cases(CriterionCase.BaseState),
                pf => true,
                StaticBaseCaseTPR.Create),

            new PreFaultRule(
                Cases(CriterionCase.BaseState),
                pf => true,
                StaticBaseCaseEPR.CreateStandard),

            new PreFaultRule(
                Cases(CriterionCase.ForcedState),
                pf => true,
                StaticBaseCaseEPR.CreateForcedState)
            #endregion rules
        };

        public override CriterionCase CaseMask { get; }

        public Func<PreFaultConditions, bool> When { get; }

        public Func<PreFaultConditions, ICriterion> Create { get; }

        public static PreFaultRule[] GetRules => _rules;

        private PreFaultRule(CriterionCase caseMask,
            Func<PreFaultConditions, bool> when,
            Func<PreFaultConditions, ICriterion> create)
        {
            CaseMask = caseMask;
            When = when;
            Create = create;
        }
    }
}
