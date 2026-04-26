using ApfBuilder.Context;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Core;
using ApfBuilder.Services;
using System.Linq;
using System;
using ApfBuilder.Criteria.Extension;

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
                .SimpleSelector(byCase[CriterionCase.BaseState])
                .Concat(CriterionSelector.ComplexSelector(byComplexSelector))
                .ToArray();

            var forcedStateCriteria =
                CriterionSelector
                .SimpleSelector(byCase[CriterionCase.ForcedState])
                .ToArray();

            ICriterion[] additionalCriteria = Array.Empty<ICriterion>();
            //var additionalCriteria = byCase[CriterionCase.Additional].ToArray();

            Criteria = baseStateCriteria
                .Concat(forcedStateCriteria)
                .Concat(additionalCriteria)
                .DistinctByInner()
                .ToArray();
        }
    }
}
