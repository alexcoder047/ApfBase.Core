using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;

namespace ApfBuilder.Criteria.Decorator
{
    public sealed class CaseCriterionDecorator : CriterionDecoratorBase
    {
        private readonly CriterionCase _case;

        public override CriterionCase Case => _case;

        public CaseCriterionDecorator(ICriterion inner, CriterionCase @case)
            : base(inner)
        {
            _case = @case;
        }
    }
}
