using LudumDare54.Core;
using LudumDare54.Graphics;
using Newtonsoft.Json;

namespace LudumDare54.Editor;

public class MemoryRepositoryCreator : RepositoryFactory, WriteableRepositoryFactory, LoadableRepositoryFactory {
    public async Task<Repository> GetRepository() => _repository ??= await Load();

    private Repository? _repository;

    public String Preset { get => _preset; set { _preset = value; _repository = null; Preferences.Default.Set("preset", value); } }
    private String _preset;

    public MemoryRepositoryCreator() {
        Preset = Preferences.Default.Get("preset", "data.json");
    }

    private async Task<Repository> Load() {
        try {
            var strData = default(String);

            var filePath = Preset;
            if (!Preset.Contains(':') && !Preset.StartsWith('/') && !Preset.StartsWith("..")) {
                var overridePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LudumDare54", Preset);
                if (File.Exists(overridePath)) {
                    filePath = overridePath;
                }
            }
            
            if (filePath.Contains(':') || filePath.StartsWith('/')) {
                strData = File.ReadAllText(filePath);
            }
            else {
                var stream = await FileSystem.OpenAppPackageFileAsync("wwwroot/_content/LudumDare54.Graphics/config/" + filePath);
                var reader = new StreamReader(stream);
                strData = await reader.ReadToEndAsync();
            }
            var repository = JsonConvert.DeserializeObject<Repository>(strData, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Objects
            });
            Preferences.Default.Set("preset", filePath);
            return repository;
        }
        catch(Exception e) {
            return new Repository();
        }
    }

    public void Save() {
        var strData = JsonConvert.SerializeObject(_repository, new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Objects
        });
        var filePath = Preset;
        if (!Preset.Contains(':') && !Preset.StartsWith('/') && !Preset.StartsWith("..")) {
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LudumDare54", Preset);
        }

        var dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllText(filePath, strData);
    }

    public async Task SelectFile() {
        var file = await FilePicker.PickAsync();
        if (file is not null) {
            Preset = file.FullPath;
        }
    }
}