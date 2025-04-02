using MediatR;

namespace ReportHub.Application.Features.DataImports.Queries;

public abstract record ImportBaseQuery<T>(Stream Stream, string FileExtension) : IRequest<IEnumerable<T>> where T : class;
