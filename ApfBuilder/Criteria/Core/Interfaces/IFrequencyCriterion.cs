using DataBaseModels.ApfBaseEntities;

namespace ApfBuilder.Criteria.Core.Interfaces
{
    public interface IFrequencyCriterion : IAdditionalCriterion
    {
        (string Value, string Description) FullValue { get; }

        Disturbances Disturbance { get; }
    }
}
