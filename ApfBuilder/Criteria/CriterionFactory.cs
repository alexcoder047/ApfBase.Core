using ApfBuilder.Context;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Services;
using System.Linq;

namespace ApfBuilder.Criteria
{
    public class CriterionFactory : ICriterionFactory
    {
        private readonly IAPFContext _context;

        private readonly CriterionSelector _selector;

        public ICriterion[] BaseStateCriteria { get; }

        public ICriterion[] ForcedStateCriteria { get; }

        public ICriterion[] AdditionalCriteria { get; }

        public CriterionFactory(IAPFContext context)
        {
            _context = context;
            _selector = new CriterionSelector();

            var built = CriterionRule.BuildCriteria(_context);

            var byCase = built.ByCase;
            var byComplexSelector = built.ByComplexSelector;

            BaseStateCriteria =
                _selector.GetSimpleSelector(byCase[CriterionCase.BaseState])
                .Concat(_selector.GetComplexSelector(byComplexSelector))
                .ToArray();

            ForcedStateCriteria =
                _selector.GetSimpleSelector(byCase[CriterionCase.ForcedState])
                .ToArray();

            AdditionalCriteria =
                _selector.GetSimpleSelector(byCase[CriterionCase.Additional])
                .ToArray();
        }
    }
}
