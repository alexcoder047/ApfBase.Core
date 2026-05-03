using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using DataBaseModels.ApfBaseEntities;
using System;

namespace ApfBuilder.Criteria.Rule
{
    public sealed class PostFaultRule : CriterionRuleBase
    {
        private static readonly PostFaultRule[] _rules = new[]
        {
            #region rules
            new PostFaultRule(
                Cases(CriterionCase.BaseState, CriterionCase.ForcedState),
                pf => true,
                true,
                Current.CreateStandard),

            new PostFaultRule(
                Cases(CriterionCase.BaseState, CriterionCase.ForcedState),
                pf => true,
                true,
                Current.CreateAOPO),

            new PostFaultRule(
                Cases(CriterionCase.BaseState),
                pf => true,
                true,
                Static.Create),

            new PostFaultRule(
                Cases(CriterionCase.BaseState),
                pf => true,
                true,
                Dynamic.Create),

            new PostFaultRule(
                Cases(CriterionCase.BaseState),
                pf => true,
                true,
                Voltage.Create),

            new PostFaultRule(
                Cases(CriterionCase.Additional),
                pf => true,
                false,
                Frequency.Create)
            #endregion rules
        };

        public override CriterionCase CaseMask { get; }

        public Func<PostFaultConditions, bool> When { get; }

        public Func<PostFaultConditions, ICriterion> Create { get; }

        public bool CanUseComplexSelector { get; }

        public static PostFaultRule[] GetRules => _rules;

        private PostFaultRule(CriterionCase caseMask,
            Func<PostFaultConditions, bool> when,
            bool canUseComplexSelector,
            Func<PostFaultConditions, ICriterion> create)
        {
            CaseMask = caseMask;
            When = when;
            CanUseComplexSelector = canUseComplexSelector;
            Create = create;
        }
    }
}
