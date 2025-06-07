using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace AppBuenisimo.Tests.Helpers
{
    public static class MockDbSetHelper
    {
        public static Mock<DbSet<T>> CreateMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>((s) => ((List<T>)data.ToList()).Add(s));

            return mockSet;
        }
    }

    // Estas clases auxiliares son necesarias para simular operaciones asíncronas de EF6 si las tuvieras.
    // Aunque tu código actual no es asíncrono, es una buena práctica incluirlas.
    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        internal TestDbAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression) { return new TestDbAsyncEnumerable<TEntity>(expression); }
        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression) { return new TestDbAsyncEnumerable<TElement>(expression); }
        public object Execute(System.Linq.Expressions.Expression expression) { return _inner.Execute(expression); }
        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression) { return _inner.Execute<TResult>(expression); }
        public System.Threading.Tasks.Task<object> ExecuteAsync(System.Linq.Expressions.Expression expression, System.Threading.CancellationToken cancellationToken) { return System.Threading.Tasks.Task.FromResult(Execute(expression)); }
        public System.Threading.Tasks.Task<TResult> ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, System.Threading.CancellationToken cancellationToken) { return System.Threading.Tasks.Task.FromResult(Execute<TResult>(expression)); }
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestDbAsyncEnumerable(System.Linq.Expressions.Expression expression) : base(expression) { }
        public IDbAsyncEnumerator<T> GetAsyncEnumerator() { return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator()); }
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() { return GetAsyncEnumerator(); }
        IQueryProvider IQueryable.Provider { get { return new TestDbAsyncQueryProvider<T>(this); } }
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestDbAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
        public void Dispose() { _inner.Dispose(); }
        public System.Threading.Tasks.Task<bool> MoveNextAsync(System.Threading.CancellationToken cancellationToken) { return System.Threading.Tasks.Task.FromResult(_inner.MoveNext()); }
        public T Current { get { return _inner.Current; } }
        object IDbAsyncEnumerator.Current { get { return Current; } }
    }
}