using System;

namespace ApfBuilder.Criteria.Core
{
    [Flags]
    public enum CriterionCase
    {
        None = 0,
        BaseState = 1 << 0,
        ForcedState = 1 << 1,
        Additional = 1 << 2
    }
}
