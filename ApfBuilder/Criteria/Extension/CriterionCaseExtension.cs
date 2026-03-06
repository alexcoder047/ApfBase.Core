using ApfBuilder.Criteria.Core;

namespace ApfBuilder.Criteria.Extension
{
    public static class CriterionCaseExtension
    {
        public static bool Has(this CriterionCase value, CriterionCase flag) 
            => (value & flag) != 0;
    }
}
