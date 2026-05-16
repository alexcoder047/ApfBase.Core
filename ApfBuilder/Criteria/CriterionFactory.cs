using ApfBuilder.Context;
using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Helper;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Extension;
using ApfBuilder.Services;
using System.Linq;

namespace ApfBuilder.Criteria
{
    public class CriterionFactory : ICriterionFactory
    {
        public ICriterion[] Criteria { get; }

        public CriterionFactory(IAPFContext context)
        {
            var built = CriterionBuilder.Build(context);

            Criteria = GetSelectedCriteria(built);
        }

        private ICriterion[] GetSelectedCriteria(CriterionBuilder built)
        {
            var byCase = built.ByCase;
            var byComplexSelector = built.ByComplexSelector;

            var baseStateCriteria =
                CriterionSelector
                .NotNullDetectSelector(byCase[CriterionCase.BaseState],
                    x => x.Value)
                .Concat(CriterionSelector.MinDetectSelector(
                    byComplexSelector, x => x.Value)
                )
                .ToArray();

            var forcedStateCriteria =
                CriterionSelector
                .NotNullDetectSelector(byCase[CriterionCase.ForcedState],
                    x => x.Value)
                .ToArray();

            var additionalCriteria =
                CriterionSelector
                    .NotNullDetectSelector(byCase[CriterionCase.Additional],
                        x => x.MaxValue)
                    .Where(x =>
                    {
                        var frequency = x.AsInner<Frequency>();
                        var verification = x.AsInner<VerificationCriterion>();
                        
                        return
                            (frequency?.CanUse == true) ||
                                VerificationCriterionHelper
                                    .CanUse(verification);
                    })
                    .ToArray();

            return baseStateCriteria
                .Concat(forcedStateCriteria)
                .Concat(additionalCriteria)
                .DistinctByInner()
                .ToArray();
        }
    }
}
