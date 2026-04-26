using ApfBuilder.Criteria.Extension;
using ApfBuilder.Criteria.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using static ApfBuilder.Criteria.CriterionAttribute;

namespace ApfBuilder.PowerFlow.Factory
{
    public class PowerFlowStandardFactory : IPowerFlowFactory
    {
        public IPowerFlow PowerFlow { get; }

        public PowerFlowStandardFactory(IEnumerable<ICriterion> criteria)
        {
            var filteredCriteria = Filter(criteria).ToList();

            PowerFlow = new PowerFlowStandard(filteredCriteria);
        }

        private IEnumerable<ICriterion> Filter(
            IEnumerable<ICriterion> criteria)
        {
            var baseCriteria = criteria
                .Where(x => x.HasAttribute<FirstAPF>()
                );

            var minValueOfMaxValueBase = baseCriteria.Min(
                c => c.MaxValue);

            foreach (var criterion in baseCriteria?.ToList())
            {
                if (criterion?.MinValue <= minValueOfMaxValueBase?.MaxValue)
                {
                    yield return criterion;
                }
            }

            var secondCriteria = criteria
                .Where(x => x.HasAttribute<SecondaryAPF>()
                );

            foreach (var secondCriterion in secondCriteria)
            {
                if (secondCriterion?.MaxValue <
                    minValueOfMaxValueBase?.MaxValue)
                {
                    yield return secondCriterion;
                }
            }

            var additionalCriteria = criteria
                .Where(x => x.HasAttribute<AdditionalAPF>()
                );

            foreach (var additionalCriterion in additionalCriteria)
            {
                yield return additionalCriterion;
            }
        }
    }
}
