using DataBaseModels.ApfBaseEntities;

namespace ApfBuilder.Criteria.Core.Interfaces
{
    public interface IFrequencyCriterion : ICriterion
    {
        bool? CanUse { get; }

        (string Value, string Description) FullValue { get; }

        Disturbances Disturbance { get; }
    }
}
