using ApfBuilder.Context;
using ApfBuilder.Criteria;
using ApfBuilder.PowerFlow;
using ApfBuilder.PowerFlow.Factory;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.Services
{
    public class PowerFlowBuilder
    {
        private PowerFlowBuilder() { }

        public static IPowerFlow[] Build(IAPFContext context)
        {
            var builder = new PowerFlowBuilder();

            var powerFlows = builder.GetPowerFlow(context).ToArray();

            return powerFlows;
        }

        private IEnumerable<IPowerFlow> GetPowerFlow(IAPFContext context)
        {
            var criterionFactory = new CriterionFactory(context);

            var criteria = criterionFactory.Criteria;

            var powerFlowFactories = PowerFlowCollectionFactory
                .Create(criteria);

            foreach (var factory in powerFlowFactories)
            {
                yield return factory.PowerFlow;
            }
        }
    }
}
