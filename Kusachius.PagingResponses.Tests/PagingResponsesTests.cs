using System;
using System.Linq;
using Xunit;

namespace Kusachius.PagingResponses.Tests;

public class PagingInfoTests
{
    [Theory]
    [InlineData(1, 20, 0)]
    [InlineData(2, 20, 20)]
    [InlineData(3, 5, 10)]
    public void RecordsToSkip_ReturnsOffsetForPage(int currentPage, int pageSize, int expected)
    {
        var pagingInfo = new PagingInfo<object>
        {
            CurrentPage = currentPage,
            PageSize = pageSize
        };

        Assert.Equal(expected, pagingInfo.RecordsToSkip());
    }
}

public class LtStringCleanerTests
{
    [Fact]
    public void Clean_ReplacesLithuanianCharactersAndNormalizesCase()
    {
        var cleaner = new LtStringCleaner();

        var result = cleaner.Clean("ĄžuolĖlis ŠČĘĮŲŪŽ");

        Assert.Equal("azuolelis sceiuuz", result);
    }

    [Fact]
    public void Clean_ReturnsNullForNullInput()
    {
        var cleaner = new LtStringCleaner();

        Assert.Null(cleaner.Clean(null));
    }
}

public class ValueObjectValidationTests
{
    [Fact]
    public void Sorting_UsesAscendingDirectionWhenNotSpecified()
    {
        var sorting = new Sorting("Name");

        Assert.Equal("Name", sorting.Column);
        Assert.Equal("asc", sorting.Direction);
    }

    [Fact]
    public void Condition_RejectsUnsupportedOperations()
    {
        Assert.Throws<Exception>(() => new Condition("Name", "contains", "value"));
    }
}

public class KEnumerableTests
{
    [Fact]
    public void CreateResult_AppliesPagingAndIncludesMetadata()
    {
        var criteria = new Criteria<int>(new TestCriteriaMapper(), new LtStringCleaner());
        criteria.SetPageSize(2);
        criteria.PagingInfo.CurrentPage = 2;

        var result = KEnumerable<int>.CreateResult(Enumerable.Range(1, 5).AsQueryable(), criteria);

        Assert.Equal(new[] { 3, 4 }, result.Data);
        Assert.Equal(2, result.PerPage);
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(5, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
    }

    private sealed class TestCriteriaMapper : ICriteriaMapper
    {
        public string ToDbSortColumn(string column) => column;

        public string ToDbWhereColumn(string column) => column;
    }
}
