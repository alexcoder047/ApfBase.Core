using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace ApfBuilder.Criteria.Extension
{
    public static class CriterionEnumerableExtension
    {
        public static IEnumerable<ICriterion> ForCase(
            this IEnumerable<ICriterion> source, CriterionCase @case)
                => source.Where(c => c.Case.Has(@case)
                );

        public static IEnumerable<ICriterion> DistinctByInner(
            this IEnumerable<ICriterion> source)
        {
            var seen = new HashSet<ICriterion>(
                ReferenceEqualityComparer<ICriterion>.Instance);

            foreach (var criterion in source)
            {
                var inner = criterion.Unwrap();

                if (seen.Add(inner)) yield return criterion;
            }
        }
    }
}
