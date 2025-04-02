using MediatR;

namespace ReportHub.Application.Features.DataImports.Queries;

public abstract record ImportBaseQuery(Stream Stream, string FileExtension) : IRequest<bool>;
