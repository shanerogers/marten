using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Baseline;

using Marten.Services.Includes;
using Marten.Storage;
using Marten.Transforms;
using Marten.Util;
using Npgsql;
using Remotion.Linq;

namespace Marten.Linq
{
    [Obsolete("being replaced for v4")]
    public class MartenQueryable<T>: QueryableBase<T>, IMartenQueryable<T>
    {
        public MartenQueryable(IQueryProvider provider) : base(provider)
        {
        }

        public MartenQueryable(IQueryProvider provider, Expression expression) : base(provider, expression)
        {
        }

        public DocumentStore Store => Executor.Store;

        public ITenant Tenant => Executor.Tenant;

        public MartenQueryExecutor Executor => Provider.As<MartenQueryProvider>().Executor.As<MartenQueryExecutor>();

        public QueryPlan Explain(FetchType fetchType = FetchType.FetchMany, Action<IConfigureExplainExpressions> configureExplain = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TDoc> TransformTo<TDoc>(string transformName)
        {
            return this.Select(x => x.TransformTo<T, TDoc>(transformName));
        }

        public string ToJsonArray()
        {
            throw new NotImplementedException();
        }

        public Task<string> ToJsonArrayAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIncludeJoin> Includes
        {
            get
            {
                var executor = Provider.As<MartenQueryProvider>().Executor.As<MartenQueryExecutor>();
                return executor.Includes;
            }
        }

        public QueryStatistics Statistics
        {
            get
            {
                var executor = Provider.As<MartenQueryProvider>().Executor.As<MartenQueryExecutor>();
                return executor.Statistics;
            }
        }

        public IMartenQueryable<T> Include<TInclude>(Expression<Func<T, object>> idSource, Action<TInclude> callback,
            JoinType joinType = JoinType.Inner)
        {
            var executor = Provider.As<MartenQueryProvider>().Executor.As<MartenQueryExecutor>();
            var tenant = executor.Tenant;

            tenant.EnsureStorageExists(typeof(TInclude));

            var mapping = tenant.MappingFor(typeof(T)).ToQueryableDocument();
            var included = tenant.MappingFor(typeof(TInclude)).ToQueryableDocument();

            var visitor = new FindMembers();
            visitor.Visit(idSource);
            var members = visitor.Members.ToArray();

            var include = mapping.JoinToInclude(joinType, included, members, callback);

            executor.AddInclude(include);

            return this;
        }

        public IMartenQueryable<T> Include<TInclude>(Expression<Func<T, object>> idSource, IList<TInclude> list,
            JoinType joinType = JoinType.Inner)
        {
            return Include<TInclude>(idSource, list.Fill, joinType);
        }

        public IMartenQueryable<T> Include<TInclude, TKey>(Expression<Func<T, object>> idSource,
            IDictionary<TKey, TInclude> dictionary, JoinType joinType = JoinType.Inner)
        {
            var storage = Tenant.StorageFor(typeof(TInclude));

            return Include<TInclude>(idSource, x =>
            {
                if (x == null)
                    return;

                var id = storage.Identity(x).As<TKey>();
                if (!dictionary.ContainsKey(id))
                {
                    dictionary.Add(id, x);
                }
            }, joinType);
        }

        public IMartenQueryable<T> Stats(out QueryStatistics stats)
        {
            stats = new QueryStatistics();
            var executor = Provider.As<MartenQueryProvider>().Executor.As<MartenQueryExecutor>();
            executor.Statistics = stats;

            return this;
        }

        public Task<IReadOnlyList<TResult>> ToListAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
            //return executeAsync(q => q.ToList().As<IQueryHandler<IReadOnlyList<TResult>>>(), token);
        }

        public Task<bool> AnyAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountLongAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> FirstAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> FirstOrDefaultAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> SingleAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> SingleOrDefaultAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> SumAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> MinAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> MaxAsync<TResult>(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<double> AverageAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public QueryModel ToQueryModel()
        {
            return MartenQueryParser.Flyweight.GetParsedQuery(Expression);
        }


        public NpgsqlCommand BuildCommand(FetchType fetchType)
        {
            throw new NotImplementedException();
        }

    }
}
