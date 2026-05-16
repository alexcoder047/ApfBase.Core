using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using DataBaseModels.ApfBaseEntities;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.PowerFlow
{
    public class PowerFlowSafe : PowerFlowBase
    {
        public override PowerFlowKind Kind => PowerFlowKind.PowerFlowSafe;

        public PowerFlowSafe(IEnumerable<ICriterion> criteria)
            : base(criteria)
        {
            Compose();
        }

        public override void Compose()
        {
            var verificationCriteriaList = new List<VerificationCriterion>();

            foreach (var criterion in Criteria)
            {
                if (criterion is VerificationCriterion vc)
                {
                    verificationCriteriaList.Add(vc);
                    continue;
                }

                switch (criterion)
                {
                    case IBaseCaseCriterion baseCaseCriterion:
                        Value += $"{baseCaseCriterion.Value} " +
                            $"{baseCaseCriterion.Condition?.FormalName}";
                        Description += baseCaseCriterion.Name;

                        Value = TerminateLine(Value);
                        Description = TerminateLine(Description);
                        continue;
                    case Frequency frequencyCriterion:
                        Value += frequencyCriterion.FullValue.Value +
                            (frequencyCriterion.Condition?.FormalName != null
                            ? $" {frequencyCriterion.Condition?.FormalName}"
                            : "");

                        Description +=
                            $"{frequencyCriterion.FullValue.Description}" +
                            (frequencyCriterion?.IrOscExpressions != null ?
                            " - ΔPнк" : "") +
                            (frequencyCriterion.Disturbance?.Number != null ?
                            $", ПАР {frequencyCriterion.Disturbance.Number}"
                            : "");

                        var emergencyCriterion = frequencyCriterion as 
                            IEmergencyResponseCriterion;

                        (Value, Description) = EmergencyResponseCompose(
                            Value, Description, 
                            emergencyCriterion.EmergencyResponse);

                        Value += (frequencyCriterion?.IrOscExpressions != null
                            ? " - ΔPнк" : "");

                        Value = TerminateLine(Value);
                        Description = TerminateLine(Description);
                        continue;
                }

                Value += criterion.Value;

                if (criterion is ISecondaryCriterion secondaryCriterion)
                {
                    Value += $"{secondaryCriterion.Postfix} " +
                        $"{secondaryCriterion.Condition?.FormalName}";
                    Description +=
                        $"{secondaryCriterion.Name}" +
                        (secondaryCriterion is ICurrentCriterion currentSec ?
                        $" {currentSec.Bounding?.Number}" : "");

                    Value = TerminateLine(Value);
                    Description = TerminateLine(Description);
                }

                if (criterion is IEmergencyResponseCriterion complexCriterion)
                {
                    Value += 
                        (complexCriterion.Condition?.FormalName != null ? 
                        $" {complexCriterion.Condition?.FormalName}" : "");

                    Description +=
                        $"{complexCriterion.Name}" +
                        (complexCriterion is ICurrentCriterion currentEmerg ?
                        $" {currentEmerg.Bounding?.Number}" : "") +
                        (complexCriterion.Disturbance?.Number != null ?
                        $", ПАР {complexCriterion.Disturbance.Number}" : "");

                    (Value, Description) = EmergencyResponseCompose(
                        Value, Description, complexCriterion.EmergencyResponse
                        );

                    Value = TerminateLine(Value);
                    Description = TerminateLine(Description);
                }
            }

            if (verificationCriteriaList.Any())
            {
                foreach (var vc in verificationCriteriaList)
                {
                    var er = vc.EmergencyResponse.Where(x => !(x is DAR));
                    var erDar = vc.EmergencyResponse.Where(x => x is DAR);

                    var emergencyValuePlus = string.Empty;
                    var emergencyValueMinus = string.Empty;
                    var emergencyDescription = string.Empty;
                    (emergencyValuePlus, emergencyDescription)
                        = EmergencyResponseCompose(
                            emergencyValuePlus, emergencyDescription, er);
                    (emergencyValueMinus, emergencyDescription)
                        = EmergencyResponseCompose(
                            emergencyValueMinus, emergencyDescription, er, " - ");

                    var emergencyValueDarPlus = string.Empty;
                    var emergencyValueDarMinus = string.Empty;
                    var emergencyDescriptionDar = string.Empty;
                    (emergencyValueDarPlus, emergencyDescription)
                        = EmergencyResponseCompose(
                            emergencyValueDarPlus, emergencyDescription, erDar);
                    (emergencyValueDarMinus, emergencyDescription)
                        = EmergencyResponseCompose(
                            emergencyValueDarMinus, emergencyDescription, erDar, " - ");

                    Value +=
                        $"МАКС\n" +
                        $"({vc.StaticCriterion.Value}" +
                        (vc.Condition?.FormalName != null ?
                        $" {vc.Condition?.FormalName}" : "") +
                        (emergencyValuePlus != string.Empty
                        ? $"{emergencyValuePlus}" : "") + ";\n" +
                        $"{vc.Value} * ({vc.Name}" +
                        (vc.FrequencyCriterion?.Conditions?.FormalName != null ?
                        $" {vc.FrequencyCriterion?.Conditions?.FormalName}" : "") +
                        (emergencyValueMinus != string.Empty
                        ? $"{emergencyValueMinus}" : "") +
                        (emergencyValueDarMinus != string.Empty
                        ? $"{emergencyValueDarMinus}" : "") + ") " +
                        (vc.Condition?.ConditionReplacement != null &&
                        vc.Condition?.ConditionReplacement != string.Empty
                        ? $"{vc.Condition?.ConditionReplacement}" : "") +
                        (emergencyValuePlus != string.Empty
                        ? $"{emergencyValuePlus}" : "") +
                        (emergencyValueDarPlus != string.Empty
                        ? $"{emergencyValueDarPlus}" : "") +
                        (vc?.IrOscExpressions != null ? " - ΔPнк" : "")
                        + ")" + ";\n";
                    Description +=
                        $"\n" +
                        $"{vc.StaticCriterion?.Name}" +
                        (vc.Disturbance?.Number != null
                        ? $", ПАР {vc.Disturbance.Number}" : "") + ";\n" +
                        $"{vc.Value * 100}% {vc.Name}, ПАР " +
                        $"{vc.Disturbance?.Number}" + ";\n";
                }

                Value.TrimEnd(' ', ';', '\n');
                Description.TrimEnd(' ', ';', '\n');
            }

            var isNeedPrefix = Criteria.Skip(1).Any();

            Value = GetValuePrefix(
                Value.TrimEnd(' ', ';', '\n'), isNeedPrefix
            );

            Description = GetDescriptionPrefix(
                Description.TrimEnd(' ', ';', '\n'), isNeedPrefix
            );
        }
    }
}
