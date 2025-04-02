using MediatR;

namespace ReportHub.Application.Features.DataExports.Queries;

public abstract record ExportBaseQuery : IRequest<Stream>;
