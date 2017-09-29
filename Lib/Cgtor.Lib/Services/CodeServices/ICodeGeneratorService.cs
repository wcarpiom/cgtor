namespace Cgtor.Lib.Services.CodeServices
{
    public interface ICodeGeneratorService : IGeneratorService
    {
        string DbDataContext { get; set; }
    }
}