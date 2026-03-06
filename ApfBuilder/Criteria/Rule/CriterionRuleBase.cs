using ApfBuilder.Criteria.Core;

namespace ApfBuilder.Criteria.Rule
{
    public abstract class CriterionRuleBase
    {
        public abstract CriterionCase CaseMask { get; }

        protected static CriterionCase Cases(params CriterionCase[] cases)
        {
            CriterionCase n = CriterionCase.None;

            foreach (var c in cases)
            {
                n |= c;
            }
            
            return n;
        }
    }
}
