using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Linq.Expressions;

namespace movielogger.services.tests.helpers;

public static class MockDbSetExtensions
{
    public static DbSet<T> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
    {
        var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();
        
        ((IQueryable<T>)mockSet).Provider.Returns(data.Provider);
        ((IQueryable<T>)mockSet).Expression.Returns(data.Expression);
        ((IQueryable<T>)mockSet).ElementType.Returns(data.ElementType);
        ((IQueryable<T>)mockSet).GetEnumerator().Returns(data.GetEnumerator());
        
        mockSet.AsQueryable().Returns(data);
        
        return mockSet;
    }
} 