﻿using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.Adapter;
using SharedKernel.Application.Cqrs.Queries.Contracts;
using SharedKernel.Application.Cqrs.Queries.Entities;
using SharedKernel.Application.Extensions;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Entities.Paged;
using SharedKernel.Domain.Specifications;
using SharedKernel.Domain.Specifications.Common;
#if NET461 || NETSTANDARD2_1 || NETCOREAPP3_1
using SharedKernel.Infrastructure.Data.EntityFrameworkCore.DbContexts;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Data.EntityFrameworkCore.Queries
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContextBase"></typeparam>
    public sealed class EntityFrameworkCoreQueryProvider<TDbContextBase> where TDbContextBase : DbContext, IDisposable
    {
        private readonly IDbContextFactory<TDbContextBase> _factory;
        private TDbContextBase _lastDbContext;
        private readonly List<TDbContextBase> _dbContexts;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public EntityFrameworkCoreQueryProvider(IDbContextFactory<TDbContextBase> factory)
        {
            _factory = factory;
            _dbContexts = new List<TDbContextBase>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetQuery<TEntity>(bool showDeleted = false) where TEntity : class
        {
            _lastDbContext = _factory.CreateDbContext();
            _dbContexts.Add(_lastDbContext);
            return Set<TEntity>(showDeleted);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Set<TEntity>(bool showDeleted = false) where TEntity : class
        {
            if (_lastDbContext == default)
                throw new Exception("It is required to call the 'GetQuery' method before");

            var query = _lastDbContext.Set<TEntity>().AsNoTracking();

            if (!showDeleted && typeof(IEntityAuditableLogicalRemove).IsAssignableFrom(typeof(TEntity)))
            {
                query = query
                    .Cast<IEntityAuditableLogicalRemove>()
                    .Where(new NotDeletedSpecification<IEntityAuditableLogicalRemove>().SatisfiedBy())
                    .Cast<TEntity>();
            }

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageOptions"></param>
        /// <param name="domainSpecification"></param>
        /// <param name="dtoSpecification"></param>
        /// <param name="selector"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public async Task<IPagedList<TResult>> ToPagedListAsync<T, TResult>(PageOptions pageOptions,
            ISpecification<T> domainSpecification = null, ISpecification<TResult> dtoSpecification = null,
            Expression<Func<T, TResult>> selector = null, CancellationToken cancellationToken = default)
            where T : class where TResult : class
        {
            var dbContext = _factory.CreateDbContext();
            _dbContexts.Add(dbContext);

            var query = dbContext.Set<T>().AsNoTracking();

            var totalBefore = await query.CountAsync(cancellationToken);

            #region Domain Specifications

            if (!pageOptions.ShowDeleted && typeof(IEntityAuditableLogicalRemove).IsAssignableFrom(typeof(T)))
                query = query
                    .Cast<IEntityAuditableLogicalRemove>()
                    .Where(new NotDeletedSpecification<IEntityAuditableLogicalRemove>().SatisfiedBy())
                    .Cast<T>();
            if (pageOptions.ShowDeleted && pageOptions.ShowOnlyDeleted &&
                typeof(IEntityAuditableLogicalRemove).IsAssignableFrom(typeof(T)))
                query = query
                    .Cast<IEntityAuditableLogicalRemove>()
                    .Where(new DeletedSpecification<IEntityAuditableLogicalRemove>().SatisfiedBy())
                    .Cast<T>();
            if (domainSpecification != null)
                query = query.Where(domainSpecification.SatisfiedBy());

            #endregion

            var queryDto = selector == default ? query.ProjectTo<TResult>() : query.Select(selector);

            #region Dto Specifications

            if (pageOptions.FilterProperties != null)
            {
                var propertiesSpec = new PropertiesContainsOrEqualSpecification<TResult>(
                    pageOptions.FilterProperties?.Select(p => new Property(p.Field, p.Value)));

                queryDto = queryDto.Where(propertiesSpec.SatisfiedBy());
            }

            if (!string.IsNullOrWhiteSpace(pageOptions.SearchText))
            {
                var searchTextSpec = new ObjectContainsOrEqualSpecification<TResult>(pageOptions.SearchText);

                queryDto = queryDto.Where(searchTextSpec.SatisfiedBy());
            }

            if (dtoSpecification != null)
                queryDto = queryDto.Where(dtoSpecification.SatisfiedBy());

            #endregion

            var totalAfter = await queryDto.CountAsync(cancellationToken);

            var elements = await queryDto
                .OrderAndPaged(pageOptions)
                .ToListAsync(cancellationToken);


            return new PagedList<TResult>(totalBefore, totalAfter, elements);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (var dbContextBase in _dbContexts)
            {
                dbContextBase.Dispose();

            }
        }
    }
}
