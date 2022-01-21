// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using UniRx.Async;

namespace Naninovel
{
    public class EditorFolderLocator : LocateFoldersRunner
    {
        private readonly IReadOnlyCollection<string> editorResourcePaths;

        public EditorFolderLocator (IResourceProvider provider, string resourcesPath, IReadOnlyCollection<string> editorResourcePaths)
            : base (provider, resourcesPath ?? string.Empty)
        {
            this.editorResourcePaths = editorResourcePaths;
        }

        public override UniTask RunAsync ()
        {
            var locatedFolders = LocateEditorFolders(Path, editorResourcePaths);
            SetResult(locatedFolders);
            return UniTask.CompletedTask;
        }

        public static IReadOnlyCollection<Folder> LocateEditorFolders (string path, IReadOnlyCollection<string> editorResourcePaths)
        {
            return editorResourcePaths.LocateFolderPathsAtFolder(path).Select(p => new Folder(p)).ToArray();
        }
    }
}
