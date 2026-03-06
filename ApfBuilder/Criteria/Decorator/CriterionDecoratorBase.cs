using ApfBuilder.Criteria.Core;
using ApfBuilder.Criteria.Core.Interfaces;
using System;

namespace ApfBuilder.Criteria.Decorator
{
    public abstract class CriterionDecoratorBase : ICriterionDecorator
    {
        public ICriterion Inner { get; }

        public virtual string Name => Inner.Name;

        public virtual double? Value => Inner.Value;

        public virtual double? MinValue => Inner.MinValue;

        public virtual double? MaxValue => Inner.MaxValue;

        public virtual double? ComplexMaxValue => Inner.ComplexMaxValue;

        public virtual double? ComplexMinValue => Inner.ComplexMinValue;

        public virtual int? RoundValue => Inner.RoundValue;

        public virtual CriterionType Type => Inner.Type;

        public virtual CriterionCase Case => Inner.Case;

        protected CriterionDecoratorBase(ICriterion inner) 
            => Inner = inner ?? 
                throw new ArgumentNullException(
                    nameof(inner)
                    );
    }
}
