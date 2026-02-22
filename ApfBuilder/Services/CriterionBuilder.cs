using ApfBuilder.Criteria;
using ApfBuilder.Criteria.Core.Interfaces;
using System.Collections.Generic;

namespace ApfBuilder.Services
{
    public class CriterionBuilder
    {
        public Dictionary<CriterionCase, List<ICriterion>> ByCase { get; }

        public List<ICriterion[]> ByComplexSelector { get; }

        public CriterionBuilder(Dictionary<CriterionCase,
            List<ICriterion>> byCase,
            List<ICriterion[]> byComplexSelector)
        {
            ByCase = byCase;
            ByComplexSelector = byComplexSelector;
        }
    }
}
