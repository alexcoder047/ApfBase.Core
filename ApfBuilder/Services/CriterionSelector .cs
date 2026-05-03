using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.Services
{
    public class CriterionSelector
    {
        public static IEnumerable<ICriterion> SimpleSelector(
            IEnumerable<ICriterion> criteriaList, 
                Func<ICriterion, double?> compare) 
                    => criteriaList.Where(c => compare(c) != null);

        public static IEnumerable<ICriterion> ComplexSelector(
            IEnumerable<ICriterion[]> 
            criteriaList)
        {
            foreach (var criteria in criteriaList)
            {
                var correctCriteria = criteria.Where(
                    c => c.Value != null).ToList();

                if (!correctCriteria.Any()) continue;

                var minCriterion = correctCriteria.Min(c => c.Value);

                yield return minCriterion;
            }
        }
    }
}
