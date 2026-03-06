using ApfBuilder.Context;
using ApfBuilder.Criteria.Rule;
using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Decorator;
using ApfBuilder.Criteria.Extension;
using System.Collections.Generic;

namespace ApfBuilder.Services
{
    public class CriterionBuilder
    {
        public Dictionary<CriterionCase, List<ICriterion>> ByCase { get; }

        public List<ICriterion[]> ByComplexSelector { get; }

        private CriterionBuilder( 
            Dictionary<CriterionCase, List<ICriterion>> byCase,
            List<ICriterion[]> byComplexSelector)
        {
            ByCase = byCase;
            ByComplexSelector = byComplexSelector;
        }

        public static CriterionBuilder Build(IAPFContext context)
        {
            var prefRules = PreFaultRule.GetRules;
            var postRules = PostFaultRule.GetRules;

            var caseKeys = new HashSet<CriterionCase>();

            foreach (var r in prefRules)
            {
                foreach (var k in EnumerateCases(r.CaseMask))
                {
                    caseKeys.Add(k);
                }
            }

            foreach (var r in postRules)
            {
                foreach (var k in EnumerateCases(r.CaseMask))
                {
                    caseKeys.Add(k);
                }
            }

            var byCase = new Dictionary<CriterionCase, List<ICriterion>>();
            foreach (var k in caseKeys)
            {
                byCase[k] = new List<ICriterion>();
            }
            var byComplexSelector = new List<ICriterion[]>();

            var pref = context.PreF;

            foreach (var r in prefRules)
            {
                if (!r.When(pref)) continue;

                ICriterion c = new CaseCriterionDecorator(
                    r.Create(pref),
                    r.CaseMask);

                foreach (var k in EnumerateCases(r.CaseMask))
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

                    ICriterion c = new CaseCriterionDecorator(
                        r.Create(postF),
                        r.CaseMask);

                    foreach (var k in EnumerateCases(r.CaseMask))
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

        private static IEnumerable<CriterionCase> EnumerateCases(
            CriterionCase mask)
        {
            if (mask.Has(CriterionCase.BaseState))
            {
                yield return CriterionCase.BaseState;
            }
            if (mask.Has(CriterionCase.ForcedState))
            {
                yield return CriterionCase.ForcedState;
            }
            if (mask.Has(CriterionCase.Additional))
            {
                yield return CriterionCase.Additional;
            }
        }
    }
}
