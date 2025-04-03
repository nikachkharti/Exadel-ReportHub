using MediatR;

namespace ReportHub.Application.Features.DataExports.Queries;

public abstract record ExportBaseQuery(string Extension) : IRequest<Stream>;
