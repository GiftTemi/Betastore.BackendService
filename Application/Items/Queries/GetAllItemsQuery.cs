using Application.Context;
using Application.Interfaces;
using Application.Users.Queries;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Items.Queries
{
    public class GetAllItemsQuery : IRequest<Result>
    {
        public string UserId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Search { get; set; }
        public List<string> Filter { get; set; }
    }


    public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllItemsQuery> _logger;

        public GetAllItemsQueryHandler(IApplicationDbContext dbContext, IIdentityService identityService, IMapper mapper, ILogger<GetAllItemsQuery> logger)
        {
            _context = dbContext;
            _identityService = identityService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            // Verify user using IIdentityService
            var (userResult, user) = await _identityService.GetUserById(request.UserId);
            if (!userResult.Succeeded || user == null)
                return Result.Failure("User not found");

            try
            {
                var items = _context.Items
                     .Where(item =>
                         (string.IsNullOrEmpty(request.Search) || item.Description != null && item.Description.Contains(request.Search)) &&
                         (request.Filter == null || !request.Filter.Any() || item.StatusDesc != null && request.Filter.Contains(item.StatusDesc)))
                     .Skip(request.Skip)
                     .Take(request.Take)
                     .ToList();

                // Map items to ItemDto
                var itemDtos = _mapper.Map<List<ItemDto>>(items);

                return Result.Success(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backend Service at {DateTime.Now} - Error retrieving items - response parameters: {ex?.Message ?? ex?.InnerException?.Message}");
                return Result.Failure($"Error retrieving items: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }

}
