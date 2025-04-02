using MediatR;

namespace ReportHub.Application.Features.DataImports.Queries;

public abstract record ExportBaseQuery : IRequest<Stream>;
