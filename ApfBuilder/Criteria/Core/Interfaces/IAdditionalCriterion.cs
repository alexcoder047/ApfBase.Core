namespace ApfBuilder.Criteria.Core.Interfaces
{
    public interface IAdditionalCriterion : ICriterion
    {
        bool? CanUse { get; }
    }
}
