using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Transactions.Queries.GetTransactionById;

/// <summary>
/// Query to get a transaction by ID
/// </summary>
public class GetTransactionByIdQuery : IRequest<GetTransactionByIdQueryResponse>
{
    /// <summary>
    /// Transaction ID
    /// </summary>
    public int Id { get; set; }

    public GetTransactionByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Response for GetTransactionByIdQuery
/// </summary>
public class GetTransactionByIdQueryResponse
{
    /// <summary>
    /// Transaction data
    /// </summary>
    public TransactionDto? Transaction { get; set; }

    /// <summary>
    /// Indicates if the transaction was found
    /// </summary>
    public bool IsFound { get; set; }

    /// <summary>
    /// Indicates if the query was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if query failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Handler for GetTransactionByIdQuery
/// </summary>
public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, GetTransactionByIdQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTransactionByIdQueryHandler> _logger;

    public GetTransactionByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetTransactionByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetTransactionByIdQueryResponse> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting transaction with ID: {TransactionId}", request.Id);

            // Get transaction by ID
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.Id, cancellationToken);
            
            // If transaction not found, return early
            if (transaction == null)
            {
                _logger.LogWarning("Transaction with ID {TransactionId} not found", request.Id);
                
                return new GetTransactionByIdQueryResponse
                {
                    IsFound = false,
                    Success = true
                };
            }

            // Get transaction with navigation properties
            var transactionWithIncludes = await _unitOfWork.Transactions.GetAsync(
                filter: t => t.Id == request.Id,
                includeProperties: "Product,Warehouse,User",
                cancellationToken: cancellationToken);

            transaction = transactionWithIncludes.FirstOrDefault();

            if (transaction == null)
            {
                _logger.LogWarning("Transaction with ID {TransactionId} not found", request.Id);
                
                return new GetTransactionByIdQueryResponse
                {
                    IsFound = false,
                    Success = true
                };
            }

            var transactionDto = _mapper.Map<TransactionDto>(transaction);

            _logger.LogInformation("Successfully retrieved transaction with ID: {TransactionId}", request.Id);

            return new GetTransactionByIdQueryResponse
            {
                Transaction = transactionDto,
                IsFound = true,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting transaction with ID: {TransactionId}", request.Id);

            return new GetTransactionByIdQueryResponse
            {
                IsFound = false,
                Success = false,
                ErrorMessage = "An error occurred while retrieving the transaction."
            };
        }
    }
}
