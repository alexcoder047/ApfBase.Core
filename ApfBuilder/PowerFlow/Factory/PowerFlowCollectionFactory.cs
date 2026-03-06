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

            var originalBaseState = baseState.UnwrapAll();
            var originalForcedState = forcedState.UnwrapAll();

            PowerFlowFactories = new IPowerFlowFactory[]
            {
                new PowerFlowStandardFactory(originalBaseState),
                new PowerFlowSafeFactory(originalBaseState),
                new PowerFlowEmergencyFactory(originalBaseState),
                new PowerFlowForcedStateFactory(originalForcedState)
            };
        }

        public static IPowerFlowFactory[] Create(
            IEnumerable<ICriterion> criteria) => 
            new PowerFlowCollectionFactory(criteria)
                .PowerFlowFactories
                .ToArray();
    }
}
