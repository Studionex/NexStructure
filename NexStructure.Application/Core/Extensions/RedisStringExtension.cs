using System.Linq.Expressions;

namespace NexStructure.Application.Core.Extensions;

public static class RPath
{
    public static string Of<T>(Expression<Func<T, object>> expression)
    {
        if (expression.Body is MemberExpression member)
            return "$." + member.Member.Name;

        if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
            return "$." + unaryMember.Member.Name;

        throw new ArgumentException("Invalid expression");
    }
}