﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Naninovel
{
    public class ProjectResources : ScriptableObject
    {
        public IReadOnlyCollection<string> ResourcePaths => resourcePaths;

        [SerializeField] private List<string> resourcePaths = new List<string>();

        private void Awake ()
        {
            LocateAllResources();
        }

        public static ProjectResources Get ()
        {
            return Application.isEditor ? CreateInstance<ProjectResources>() : Resources.Load<ProjectResources>(nameof(ProjectResources));
        }

        public void LocateAllResources ()
        {
            if (!Application.isEditor) return;

            resourcePaths.Clear();
            var dataDir = new System.IO.DirectoryInfo(Application.dataPath);
            var resourcesDirs = dataDir.GetDirectories("*Resources", System.IO.SearchOption.AllDirectories)
                .Where(d => d.FullName.EndsWithFast($"{System.IO.Path.DirectorySeparatorChar}Resources")).ToList();
            foreach (var dir in resourcesDirs)
                WalkResourcesDirectory(dir, resourcePaths);
        }

        private static void WalkResourcesDirectory (System.IO.DirectoryInfo directory, List<string> outPaths)
        {
            var paths = directory.GetFiles().Where(p => !p.FullName.EndsWithFast(".meta"))
                .Select(p => p.FullName.Replace("\\", "/").GetAfterFirst("/Resources/").GetBeforeLast("."));
            outPaths.AddRange(paths);

            var subDirs = directory.GetDirectories();
            foreach (var dirInfo in subDirs)
                WalkResourcesDirectory(dirInfo, outPaths);
        }
    }
}
