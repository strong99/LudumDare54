using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace LudumDare54.Editor;

public class CustomFilesBlazorWebView : BlazorWebView {
    public override IFileProvider CreateFileProvider(String contentRootDir) {
        var lPhysicalFiles = new CustomPhysicalFileProvider();
        return new CompositeFileProvider(lPhysicalFiles, base.CreateFileProvider(contentRootDir));
    }
}

public class CustomPhysicalFileProvider : IFileProvider, IDisposable {
    private String _activationPath = "localsystem/";
    public void Dispose() {

    }

    public IDirectoryContents GetDirectoryContents(String subpath) {
        return NotFoundDirectoryContents.Singleton;
    }

    public IFileInfo GetFileInfo(String subpath) {
        if (!subpath.StartsWith(_activationPath)) {
            return new NotFoundFileInfo(subpath);
        }

        var encodedpath = subpath.Substring(_activationPath.Length);
        var fullPath = Uri.UnescapeDataString(encodedpath);

        var fileInfo = new FileInfo(fullPath);
        return new PhysicalFileInfo(fileInfo);
    }

    public IChangeToken Watch(String filter) {
        return NullChangeToken.Singleton;
    }
}