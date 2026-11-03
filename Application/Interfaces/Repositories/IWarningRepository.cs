using Application.Common;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IWarningRepository:IGenericRepository<Warning>
    {
        Task<PagedResult<Warning>> GetWarningsByAlertAsync(Alert warning, int pageNumber, int pageSize);

        Task<PagedResult<Warning>> GetWarningsByLabAsync(int LabID, int pageNumber, int pageSize);

        Task<PagedResult<Warning>> GetAllWithDetailsAsync(int pageNumber, int pageSize);


    }
}
