namespace ApfBuilder.Criteria.Core.Helper
{
    public static class VerificationCriterionHelper
    {
        public static bool CanUse(VerificationCriterion verification)
        {
            if (verification == null) return false;

            if (HasEmergencyLimitLessThanOscExpressions(verification)) return true;

            if (HasEmergencyLimitLessThanLimitPowerFlowPart(verification)) return true;

            if (HasConditionMaxValueGreaterThanLimitPowerFlowPart(verification)) return true;

            return false;
        }

        private static bool HasEmergencyLimitLessThanOscExpressions(
            VerificationCriterion verification)
        {
            return verification.StaticCriterion != null &&
                   verification.StaticCriterion.Value.HasValue &&
                   verification.IrOscExpressions.HasValue &&
                   (verification.Condition?.ConditionReplacement == null ||
                   verification.Condition?.ConditionReplacement == string.Empty) &&
                   verification.FrequencyCriterion != null &&
                   ((verification.StaticCriterion.Value +
                     verification.IrOscExpressions) / 0.92) <
                   verification.IrOscExpressions * 3;
        }

        private static bool HasEmergencyLimitLessThanLimitPowerFlowPart(
            VerificationCriterion verification)
        {
            return verification.StaticCriterion != null &&
                   verification.StaticCriterion.Value.HasValue &&
                   verification.IrOscExpressions.HasValue && 
                   (verification.Condition?.ConditionReplacement == null ||
                   verification.Condition?.ConditionReplacement == string.Empty) &&
                   verification.FrequencyCriterion != null &&
                   ((verification.StaticCriterion.Value +
                     verification.IrOscExpressions) / 0.92) <
                   verification.LimitPowerFlow * 0.3;
        }

        private static bool HasConditionMaxValueGreaterThanLimitPowerFlowPart(
            VerificationCriterion verification)
        {
            return verification.StaticCriterion != null &&
                   verification.StaticCriterion.Value.HasValue &&
                   (verification.Condition?.ConditionReplacement != null ||
                   verification.Condition?.ConditionReplacement != string.Empty) &&
                   verification.FrequencyCriterion != null &&
                   verification.Condition?.MaxValue != null &&
                   verification.Condition.MaxValue <
                   verification.LimitPowerFlow * 0.5;
        }
    }
}
