using AutoMapper;
using MediatR;
using TheGourmet.Application.DTOs.OrderCancelReason;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Orders.Queries.GetCancelReasons;

public class GetCancelReasonsHandle : IRequestHandler<GetCancelReasonsQuery, List<OrderCancelReasonDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetCancelReasonsHandle(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<OrderCancelReasonDto>> Handle(GetCancelReasonsQuery request, CancellationToken cancellationToken)
    {
        var reasons = await _unitOfWork.OrderCancelReasons.GetAllAsync();
        var result = _mapper.Map<List<OrderCancelReasonDto>>(reasons);
        return result;
    }
}