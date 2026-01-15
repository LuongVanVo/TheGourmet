using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.DTOs.ProductReview;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.ProductReviews.Queries.GetProductReviews;

public class GetProductReviewsHandler : IRequestHandler<GetProductReviewsQuery, List<ProductReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetProductReviewsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProductReviewDto>> Handle(GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.ProductReviews.GetQueryable()
            .Where(pr => pr.ProductId == request.ProductId)
            .OrderByDescending(pr => pr.CreatedAt)
            .ProjectTo<ProductReviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return reviews;
    }
}