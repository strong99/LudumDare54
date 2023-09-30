using LudumDare54.Core;
using LudumDare54.Graphics;
using Newtonsoft.Json;

namespace LudumDare54.Web;

public class MemoryRepositoryCreator : RepositoryFactory {
    public async Task<Repository> GetRepository() => _repository ??= await Load();

    private Repository? _repository;

    private HttpClient _httpClient;

    public MemoryRepositoryCreator(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    private async Task<Repository> Load() {
        try {
            var strData = await _httpClient.GetStringAsync("_content/LudumDare54.Graphics/config/data.json");
            var repository = JsonConvert.DeserializeObject<Repository>(strData, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return repository ?? throw new Exception("Unable load the repository from file");
        }
        catch (Exception e) {
            throw new Exception("Unable to locate the repository", e);
        }
    }
}