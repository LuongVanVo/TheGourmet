using AutoMapper;
using MassTransit.Contracts;
using MediatR;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrdersWithPagination;

public class GetOrdersWithPaginationHandler : IRequestHandler<GetOrdersWithPaginationQuery, PaginatedList<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetOrdersWithPaginationHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async  Task<PaginatedList<OrderDto>> Handle(GetOrdersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paginatedOrders = await _unitOfWork.Orders.GetOrdersWithPaginationAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.FromDate,
            request.ToDate,
            cancellationToken
        );

        // map to dto
        var ordersDto = _mapper.Map<List<OrderDto>>(paginatedOrders.Items);

        return new PaginatedList<OrderDto>(
            ordersDto,
            paginatedOrders.TotalCount,
            paginatedOrders.PageNumber,
            paginatedOrders.TotalCount
        );
    }
}