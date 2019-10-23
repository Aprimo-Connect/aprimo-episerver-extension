using EPiServer.Data;

namespace Aprimo.Epi.Extensions.Services
{
    public interface IAprimoConnectorSettingsRepository
    {
        AprimoConnectorSettings Load();

        Identity Save(AprimoConnectorSettings settings);
    }
}