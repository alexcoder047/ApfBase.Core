using ApfBuilder.Context;
using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Services;
using DataBaseModels.ApfBaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.Criteria
{
    public enum CriterionCase
    {
        BaseState,
        ForcedState,
        Additional
    }

    public class CriterionRule
    {
        private sealed class PreFaultRule
        {
            private static readonly PreFaultRule[] _rules = new[]
            {
                #region rules
                new PreFaultRule(
                    new[]
                    {
                        CriterionCase.BaseState,
                        CriterionCase.ForcedState
                    },
                    pf => true,
                    CurrentSecondary.CreateStandard),

                new PreFaultRule(
                    new[]
                    {
                        CriterionCase.BaseState,
                        CriterionCase.ForcedState
                    },
                    pf => true,
                    CurrentSecondary.CreateAOPO),

                new PreFaultRule(
                    new[] 
                    { 
                        CriterionCase.BaseState 
                    },
                    pf => true,
                    VoltageSecondary.Create),

                new PreFaultRule(
                    new[] 
                    { 
                        CriterionCase.BaseState 
                    },
                    pf => true,
                    StaticBaseCaseTPR.Create),

                new PreFaultRule(
                    new[] 
                    { 
                        CriterionCase.BaseState 
                    },
                    pf => true,
                    StaticBaseCaseEPR.CreateStandard),

                new PreFaultRule(
                    new[] 
                    { 
                        CriterionCase.ForcedState 
                    },
                    pf => true,
                    StaticBaseCaseEPR.CreateForcedState)
                #endregion rules
            };

            public IReadOnlyList<CriterionCase> Keys { get; }

            public Func<PreFaultConditions, bool> When { get; }

            public Func<PreFaultConditions, ICriterion> Create { get; }

            public static PreFaultRule[] GetRules => _rules;

            private PreFaultRule(IEnumerable<CriterionCase> keys,
                Func<PreFaultConditions, bool> when,
                Func<PreFaultConditions, ICriterion> create)
            {
                Keys = keys.ToArray();
                When = when;
                Create = create;
            }
        }

        private sealed class PostFaultRule
        {
            private static readonly PostFaultRule[] _rules = new[]
            {
                #region rules
                new PostFaultRule(
                    new[]
                    {
                        CriterionCase.BaseState,
                        CriterionCase.ForcedState
                    },
                    pf => true,
                    true,
                    Current.CreateStandard),

                new PostFaultRule(
                    new[]
                    {
                        CriterionCase.BaseState,
                        CriterionCase.ForcedState
                    },
                    pf => true,
                    true,
                    Current.CreateAOPO),

                new PostFaultRule(
                    new[] 
                    { 
                        CriterionCase.BaseState 
                    },
                    pf => true,
                    true,
                    Static.Create),

                new PostFaultRule(
                    new[] 
                    { 
                        CriterionCase.BaseState 
                    },
                    pf => true,
                    true,
                    Dynamic.Create),

                new PostFaultRule(
                    new[] 
                    {
                        CriterionCase.BaseState 
                    },
                    pf => true,
                    true,
                    Voltage.Create),

                new PostFaultRule(
                    new[] 
                    { 
                        CriterionCase.Additional 
                    },
                    pf => true,
                    false,
                    Frequency.Create)
                #endregion rules
            };

            public IReadOnlyList<CriterionCase> Keys { get; }

            public Func<PostFaultConditions, bool> When { get; }

            public Func<PostFaultConditions, ICriterion> Create { get; }

            public bool CanUseComplexSelector { get; }

            public static PostFaultRule[] GetRules => _rules;

            private PostFaultRule(IEnumerable<CriterionCase> keys,
                Func<PostFaultConditions, bool> when,
                bool canUseComplexSelector,
                Func<PostFaultConditions, ICriterion> create)
            {
                Keys = keys.ToArray();
                When = when;
                CanUseComplexSelector = canUseComplexSelector;
                Create = create;
            }
        }

        public static CriterionBuilder BuildCriteria(IAPFContext context)
        {
            var prefRules = PreFaultRule.GetRules;
            var postRules = PostFaultRule.GetRules;

            var allKeys = prefRules.SelectMany(r => r.Keys)
                .Concat(postRules.SelectMany(r => r.Keys))
                .Distinct();

            var byCase = allKeys.ToDictionary(
                k => k, _ => new List<ICriterion>());
            var byComplexSelector = new List<ICriterion[]>();

            var pref = context.PreF;

            foreach (var r in prefRules)
            {
                if (!r.When(pref)) continue;

                var c = r.Create(pref);
                foreach (var k in r.Keys)
                {
                    byCase[k].Add(c);
                }
            }

            foreach (var postF in pref.PostFaultConditions)
            {
                if (!(postF.Using ?? false)) continue;

                List<ICriterion> complexForThisPost = null;

                foreach (var r in postRules)
                {
                    if (!r.When(postF)) continue;

                    var c = r.Create(postF);

                    foreach (var k in r.Keys)
                    {
                        if (k == CriterionCase.BaseState && 
                            r.CanUseComplexSelector) continue;

                        byCase[k].Add(c);
                    }

                    if (r.CanUseComplexSelector)
                    {
                        (complexForThisPost ?? 
                            (complexForThisPost = new List<ICriterion>())
                                ).Add(c);
                    }
                }

                if (complexForThisPost != null && 
                    complexForThisPost.Count > 0)
                {
                    byComplexSelector.Add(complexForThisPost.ToArray());
                }
            }

            return new CriterionBuilder(byCase, byComplexSelector);
        }
    }
}
