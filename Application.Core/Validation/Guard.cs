using System.Diagnostics.CodeAnalysis;

namespace Application.Core.Validation
{
    public class Guard : IGuardClause
    {
        private Guard()
        {
        }

        [NotNull] public static IGuardClause Against { get; }
    }
}