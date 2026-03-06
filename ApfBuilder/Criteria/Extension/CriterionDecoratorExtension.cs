using ApfBuilder.Criteria.Core.Interfaces;
using ApfBuilder.Criteria.Decorator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApfBuilder.Criteria.Extension
{
    public static class CriterionDecoratorExtension
    {
        public static ICriterion Unwrap(this ICriterion c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            while (c is ICriterionDecorator d) c = d.Inner;

            return c;
        }

        public static IEnumerable<ICriterion> UnwrapAll(
            this IEnumerable<ICriterion> src) => src.Select(c => c.Unwrap()
            );

        public static Type UnwrapType(this ICriterion c)
            => c.Unwrap().GetType();

        public static bool HasAttribute<TAttr>(
            this ICriterion c, bool inherit = true) where TAttr : Attribute
        {
            var t = c.UnwrapType();

            return Attribute.IsDefined(t, typeof(TAttr), inherit);
        }
    }
}
