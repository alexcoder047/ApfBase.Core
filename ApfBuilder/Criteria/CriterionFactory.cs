using ApfBuilder.Context;
using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Extension;
using ApfBuilder.Services;
using System.Linq;

namespace ApfBuilder.Criteria
{
    public class CriterionFactory : ICriterionFactory
    {
        private readonly IAPFContext _context;

        public ICriterion[] Criteria { get; }

        public CriterionFactory(IAPFContext context)
        {
            _context = context;

            var built = CriterionBuilder.Build(_context);

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
                .UsageSelector(byCase[CriterionCase.Additional],
                    x => x.AsInner<IAdditionalCriterion>()?.CanUse);

            Criteria = baseStateCriteria
                .Concat(forcedStateCriteria)
                .Concat(additionalCriteria)
                .DistinctByInner()
                .ToArray();
        }
    }
}
