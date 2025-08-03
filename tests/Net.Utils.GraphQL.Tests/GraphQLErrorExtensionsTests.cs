using Xunit;
using GraphQL;
using FluentAssertions;
using Net.Utils.GraphQL.Extensions;

namespace Net.Utils.GraphQL.Tests;

public class GraphQLErrorExtensionsTests
{
    public class EnsureNoErrors
    {
        [Fact]
        internal void When_ResponseIsNull_ShouldThrowArgumentNullException()
        {
            GraphQLResponse<string>? response = null;

            var testCode = () => response!.EnsureNoErrors();

            testCode.Should()
               .Throw<ArgumentNullException>()
               .WithParameterName("response");
        }

        [Fact]
        internal void When_NoErrors_ShouldReturnData()
        {
            var response = new GraphQLResponse<string>
            {
                Data = "foo",
                Errors = null
            };

            var result = response.EnsureNoErrors();

            result.Should().Be("foo");
        }

        [Fact]
        internal void When_OnlyIndexingErrorAndFilterEnabled_ShouldReturnData()
        {
            var response = new GraphQLResponse<string>
            {
                Data = "foo",
                Errors = new[]
                {
                    new GraphQLError { Message = "indexing_error" }
                }
            };

            var result = response.EnsureNoErrors(filterIndexingErrors: true);

            result.Should().Be("foo");
            response.Errors.Should().BeNull();
        }

        [Fact]
        internal void When_OnlyIndexingErrorAndFilterDisabled_ShouldThrow()
        {
            var response = new GraphQLResponse<string>
            {
                Data = "foo",
                Errors = new[]
                {
                    new GraphQLError { Message = "indexing_error" }
                }
            };

            var testCode = () => response.EnsureNoErrors(filterIndexingErrors: false);

            testCode.Should()
               .Throw<InvalidOperationException>()
               .WithMessage("indexing_error");
        }

        [Fact]
        internal void When_NonIndexingErrorsExist_ShouldThrowAggregatedMessage()
        {
            var response = new GraphQLResponse<string>
            {
                Data = "foo",
                Errors = new[]
                {
                    new GraphQLError { Message = "some_error" },
                    new GraphQLError { Message = "another_error" }
                }
            };

            var testCode = () => response.EnsureNoErrors();

            testCode.Should()
               .Throw<InvalidOperationException>()
               .WithMessage($"some_error{Environment.NewLine}another_error");
        }

        [Fact]
        internal void When_MixedErrors_FilterRemovesIndexingErrorsAndThrowsForOthers()
        {
            var response = new GraphQLResponse<string>
            {
                Data = "foo",
                Errors = new[]
                {
                    new GraphQLError { Message = "indexing_error" },
                    new GraphQLError { Message = "real_error" }
                }
            };

            var testCode = () => response.EnsureNoErrors();

            testCode.Should()
               .Throw<InvalidOperationException>()
               .WithMessage("real_error");
        }
    }
}