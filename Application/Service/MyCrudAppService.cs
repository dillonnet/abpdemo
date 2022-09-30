using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;

namespace Application.Service;

public abstract class MyCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput> : ApplicationService
    where TEntity : class, IEntity<TKey>
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
{
    protected IRepository<TEntity, TKey> Repository { get; }
    protected virtual string GetPolicyName { get; set; }
    protected virtual string GetListPolicyName { get; set; }
    protected virtual string CreatePolicyName { get; set; }

    protected virtual string UpdatePolicyName { get; set; }

    protected virtual string DeletePolicyName { get; set; }


    protected MyCrudAppService(IRepository<TEntity, TKey> repository)
    {
        Repository = repository;
    }
    
    
    public virtual async Task<TGetOutputDto> GetAsync(TKey id)
    {
        await CheckGetPolicyAsync();

        var entity = await GetEntityByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException();

        return await MapToGetOutputDtoAsync(entity);
    }
    
    public virtual async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
    {
        await CheckGetListPolicyAsync();

        var query = await CreateFilteredQueryAsync(input);
        var totalCount = await AsyncExecuter.CountAsync(query);

        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);

        var entities = await AsyncExecuter.ToListAsync(query);
        var entityDtos = await MapToGetListOutputDtosAsync(entities);

        return new PagedResultDto<TGetListOutputDto>(
            totalCount,
            entityDtos
        );
    }
    
    public virtual async Task<TGetOutputDto> CreateAsync(TCreateInput input)
    {
        await CheckCreatePolicyAsync();
        await CheckCreateValidateAsync(input);
        
        var entity =  MapToEntity(input);
        
        await Repository.InsertAsync(entity, autoSave: true);

        return await MapToGetOutputDtoAsync(entity);
    }
    
   
    
    public virtual async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
    {
        await CheckUpdatePolicyAsync();
        await CheckUpdateValidateAsync(id, input);

        var entity = await GetEntityByIdAsync(id); 
        MapToEntity(input, entity);
        await Repository.UpdateAsync(entity, autoSave: true);

        return await MapToGetOutputDtoAsync(entity);
    }
    
    public virtual async Task DeleteAsync(TKey id)
    {
        await CheckDeletePolicyAsync();
        await CheckDeleteValidateAsync(id);

        await DeleteByIdAsync(id);
    }

    #region Policy

    protected virtual async Task CheckGetPolicyAsync()
    {
        await CheckPolicyAsync(GetPolicyName);
    }
    
    protected virtual async Task CheckGetListPolicyAsync()
    {
        await CheckPolicyAsync(GetListPolicyName);
    }
    
    protected virtual async Task CheckCreatePolicyAsync()
    {
        await CheckPolicyAsync(CreatePolicyName);
    }
    
    protected virtual async Task CheckUpdatePolicyAsync()
    {
        await CheckPolicyAsync(UpdatePolicyName);
    }
    
    protected virtual async Task CheckDeletePolicyAsync()
    {
        await CheckPolicyAsync(DeletePolicyName);
    }
    
    #endregion

    #region Validate
    protected virtual Task CheckCreateValidateAsync(TCreateInput input)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task CheckUpdateValidateAsync(TKey id, TUpdateInput input)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task CheckDeleteValidateAsync(TKey id)
    {
        return Task.CompletedTask;
    }

    #endregion
    
    protected async Task<TEntity> GetEntityByIdAsync(TKey id)
    {
        return await Repository.GetAsync(id);
    }
    
    protected async Task DeleteByIdAsync(TKey id)
    {
        await Repository.DeleteAsync(id);
    }
    
    protected virtual async Task<IQueryable<TEntity>> CreateFilteredQueryAsync(TGetListInput input)
    {
        return await Repository.GetQueryableAsync();
    }
    
    protected virtual async Task CheckPolicyAsync([CanBeNull] string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return;
        }

        await AuthorizationService.CheckAsync(policyName);
    }

    #region Map
    protected virtual Task<TGetOutputDto> MapToGetOutputDtoAsync(TEntity entity)
    {
        return Task.FromResult(ObjectMapper.Map<TEntity, TGetOutputDto>(entity));
    }
    
    protected virtual Task<List<TGetListOutputDto>> MapToGetListOutputDtosAsync(List<TEntity> entities)
    {
        return Task.FromResult(ObjectMapper.Map<List<TEntity>, List<TGetListOutputDto>>(entities));
    }
    
    protected virtual TEntity MapToEntity(TCreateInput createInput)
    {
        var entity = ObjectMapper.Map<TCreateInput, TEntity>(createInput);
        SetIdForGuids(entity);
        return entity;
    }
    
    protected virtual void MapToEntity(TUpdateInput updateInput, TEntity entity)
    {
        ObjectMapper.Map(updateInput, entity);
    }
    #endregion
   
    
    protected virtual void SetIdForGuids(TEntity entity)
    {
        if (entity is IEntity<Guid> entityWithGuidId && entityWithGuidId.Id == Guid.Empty)
        {
            EntityHelper.TrySetId(
                entityWithGuidId,
                () => GuidGenerator.Create(),
                true
            );
        }
    }
    
    protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TGetListInput input)
    {
        if (input is ISortedResultRequest sortInput)
        {
            if (!sortInput.Sorting.IsNullOrWhiteSpace())
            {
                return query.OrderBy(sortInput.Sorting);
            }
        }

        if (input is ILimitedResultRequest)
        {
            return ApplyDefaultSorting(query);
        }

        return query;
    }
    
    protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, TGetListInput input)
    {
        if (input is IPagedResultRequest pagedInput)
        {
            return query.PageBy(pagedInput);
        }

        if (input is ILimitedResultRequest limitedInput)
        {
            return query.Take(limitedInput.MaxResultCount);
        }

        return query;
    }
    
    protected IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
    {
        if (typeof(TEntity).IsAssignableTo<ICreationAuditedObject>())
        {
            return query.OrderByDescending(e => ((ICreationAuditedObject)e).CreationTime);
        }
        else
        {
            return query.OrderByDescending(e => e.Id);
        }
    }
}