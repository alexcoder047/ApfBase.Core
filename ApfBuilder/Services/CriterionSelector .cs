using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.Services
{
    public class CriterionSelector
    {
        public static IEnumerable<ICriterion> NotNullDetectSelector(
            IEnumerable<ICriterion> criteriaList, 
                Func<ICriterion, double?> compare) 
                    => criteriaList.Where(c => compare(c) != null);

        public static IEnumerable<ICriterion> MinDetectSelector(
            IEnumerable<ICriterion[]> criteriaList, 
                Func<ICriterion, double?> compare)
        {
            foreach (var criteria in criteriaList)
            {
                var correctCriteria = criteria.Where(
                    c => compare(c) != null).ToList();

                if (!correctCriteria.Any()) continue;

                var minCriterion = correctCriteria.Min(c => compare(c));

                yield return minCriterion;
            }
        }

        public static IEnumerable<ICriterion> UsageSelector(
            IEnumerable<ICriterion> criteriaList,
                Func<ICriterion, bool?> compare) 
                    => criteriaList.Where(c => compare(c) == true);
    }
}
