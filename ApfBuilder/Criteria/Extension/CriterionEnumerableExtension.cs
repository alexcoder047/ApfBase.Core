using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Decorator;
using Shared;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.Criteria.Extension
{
    public static class CriterionEnumerableExtension
    {
        public static IEnumerable<ICriterion> ForSelectedCase(
            this IEnumerable<ICriterion> source,
            CriterionCase @case)
            => source.Where(c => c.SelectedCase.Has(@case));

        public static IEnumerable<ICriterion> WithSelectedCase(
            this IEnumerable<ICriterion> source,
            CriterionCase selectedCase)
        {
            return source.Select(x =>
                new SelectedCaseCriterionDecorator(
                    x,
                    selectedCase));
        }

        public static IEnumerable<ICriterion> MergeSelectedCases(
            this IEnumerable<ICriterion> source)
        {
            return source
                .GroupBy(
                    x => x.Unwrap(),
                    ReferenceEqualityComparer<ICriterion>.Instance)
                .Select(group =>
                {
                    var selectedCase = group
                        .Select(x => x.SelectedCase)
                        .Aggregate(
                            CriterionCase.None,
                            (result, current) => result | current);

                    var criterion = group.First();

                    return (ICriterion)
                        new SelectedCaseCriterionDecorator(
                            criterion,
                            selectedCase);
                });
        }
    }
}
