using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Extension;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.PowerFlow.Factory
{
    public class PowerFlowCollectionFactory : IPowerFlowCollectionFactory
    {
        public IEnumerable<IPowerFlowFactory> PowerFlowFactories { get; }

        private PowerFlowCollectionFactory(IEnumerable<ICriterion> criteria)
        {
            var baseState = criteria.ForCase(CriterionCase.BaseState);
            var forcedState = criteria.ForCase(CriterionCase.ForcedState);
            var additional = criteria.ForCase(CriterionCase.Additional);

            var originalBaseState = baseState.UnwrapAll();
            var originalForcedState = forcedState.UnwrapAll();
            var originalAdditional = additional.UnwrapAll();

            var normal = originalBaseState.Concat(originalAdditional);
            var forced = originalForcedState;

            PowerFlowFactories = new IPowerFlowFactory[]
            {
                new PowerFlowStandardFactory(normal),
                new PowerFlowSafeFactory(normal),
                new PowerFlowEmergencyFactory(normal),
                new PowerFlowForcedStateFactory(forced)
            };
        }

        public static IPowerFlowFactory[] Create(
            IEnumerable<ICriterion> criteria) => 
            new PowerFlowCollectionFactory(criteria)
                .PowerFlowFactories
                .ToArray();
    }
}
