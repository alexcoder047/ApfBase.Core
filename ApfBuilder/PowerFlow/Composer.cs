using DataBaseModels.ApfBaseEntities;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.PowerFlow
{
    public abstract class Composer : IComposer
    {
        public abstract void Compose();

        protected string GetValuePrefix(string value, bool canBeUse) =>
            canBeUse ? $"МИН\n({value})" : value.TrimEnd();

        protected string GetDescriptionPrefix(string value, bool canBeUse) =>
            canBeUse ? $"\n{value}" : value.TrimEnd();

        protected string TerminateLine(string text)
        {
            return $"{text.TrimEnd(' ')};\n";
        }

        protected (string Value, string Description) EmergencyResponseCompose(
            string value, string description, 
            IEnumerable<IEmergencyResponse> emergencyResponce, 
            string symbol = " + ")
        {
            string responce = string.Empty;
            emergencyResponce.ForEach(
                (x) => responce += symbol + x.Description
                );

            value +=
                (emergencyResponce.Any() ? responce : "");

            description +=
                (emergencyResponce.Any() ? "+ УВ" : "");

            return (value, description);
        }
    }
}
