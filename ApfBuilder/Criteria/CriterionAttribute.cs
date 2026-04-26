using System;

namespace ApfBuilder.Criteria
{
    public class CriterionAttribute
    {
        public class FirstAPF : Attribute { }

        public class SecondaryAPF : Attribute { }

        public class EmergencyAPF : Attribute { }

        public class AdditionalAPF : Attribute { }

        public class CriterionPriority : Attribute
        {
            public int Priority { get; }

            public CriterionPriority(int priority)
            {
                Priority = priority;
            }
        }
    }
}
