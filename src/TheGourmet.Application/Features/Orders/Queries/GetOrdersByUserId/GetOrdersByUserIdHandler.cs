using AutoMapper;
using MediatR;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrdersByUserId;

public class GetOrdersByUserIdHandler : IRequestHandler<GetOrdersByUserIdQuery, List<OrderDto>> 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetOrdersByUserIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(request.UserId);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}