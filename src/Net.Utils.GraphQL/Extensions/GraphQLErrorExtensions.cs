using System;
using GraphQL;
using System.Linq;

namespace Net.Utils.GraphQL.Extensions
{
    /// <summary>
    /// Extension methods for working with <see cref="GraphQLResponse{T}"/>.
    /// </summary>
    public static class GraphQLErrorExtensions
    {
        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the response contains errors.
        /// Optionally filters out "indexing_error" messages before evaluating.
        /// </summary>
        /// <param name="response">The GraphQL response.</param>
        /// <param name="filterIndexingErrors">Whether to ignore errors with message "indexing_error".</param>
        /// <typeparam name="TData">Type of the response data.</typeparam>
        /// <returns>The data from the response.</returns>
        /// <exception cref="InvalidOperationException">Thrown when errors are present after filtering.</exception>
        public static TData EnsureNoErrors<TData>(this GraphQLResponse<TData> response, bool filterIndexingErrors = true)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (filterIndexingErrors && response.Errors != null)
            {
                var filtered = response.Errors
                    .Where(e => !string.Equals(e.Message, "indexing_error", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                response.Errors = filtered.Length > 0 ? filtered : null;
            }

            if (response.Errors != null && response.Errors.Length > 0)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, response.Errors.Select(e => e.Message)));
            }

            return response.Data;
        }
    }
}
