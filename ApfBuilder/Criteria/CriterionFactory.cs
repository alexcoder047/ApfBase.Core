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
                .SimpleSelector(byCase[CriterionCase.BaseState], x => x.Value)
                .Concat(CriterionSelector.ComplexSelector(byComplexSelector))
                .ToArray();

            var forcedStateCriteria =
                CriterionSelector
                .SimpleSelector(byCase[CriterionCase.ForcedState], 
                    x => x.Value)
                .ToArray();

            var additionalCriteria =
                CriterionSelector
                .SimpleSelector(byCase[CriterionCase.Additional], 
                    x => x.MaxValue);

            Criteria = baseStateCriteria
                .Concat(forcedStateCriteria)
                .Concat(additionalCriteria)
                .DistinctByInner()
                .ToArray();
        }
    }
}
