using Microsoft.EntityFrameworkCore;
using Moq;

namespace APIGestionFacturas.Tests
{
    public static class MockDbContext
    {
        public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));

            dbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));

            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((d) => sourceList.Add(d));
            dbSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>(d => sourceList.Remove(d));

            dbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] r) =>
            {
                return new ValueTask<T>(sourceList.FirstOrDefault(b => (int)b.GetType().GetProperty("Id")!.GetValue(b) == (int)r[0]));
            });

            return dbSet.Object;
        }
    }
}
