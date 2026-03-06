using ApfBuilder.Criteria.Core.Interfaces;

namespace ApfBuilder.Criteria.Decorator
{
    public interface ICriterionDecorator : ICriterion
    {
        ICriterion Inner { get; }
    }
}
