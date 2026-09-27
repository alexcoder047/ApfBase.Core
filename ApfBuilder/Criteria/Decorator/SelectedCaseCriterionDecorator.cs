using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;

namespace ApfBuilder.Criteria.Decorator
{
    public sealed class SelectedCaseCriterionDecorator
        : CriterionDecoratorBase
    {
        private readonly CriterionCase _selectedCase;

        public override CriterionCase SelectedCase => _selectedCase;

        public SelectedCaseCriterionDecorator(
            ICriterion inner,
            CriterionCase selectedCase)
            : base(inner)
        {
            _selectedCase = selectedCase;
        }
    }
}
