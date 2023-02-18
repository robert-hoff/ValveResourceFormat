namespace RunVrf.UtilHelpers
{
    class FileVcsCollection
    {

        public FileVcsCollection(ARCHIVE archive)
        {

        }

        /*
         *
         * E.g.
         * GetRelatedFiles(ARCHIVE.dotagame_pcgl, "multiblend_pcgl_30")
         * GetRelatedFiles(ARCHIVE.dotagame_pcgl, "hero_pcgl_30")
         *
         * returns
         * {multiblend_pcgl_30_features.vcs, multiblend_pcgl_30_vs.vcs, multiblend_pcgl_30_ps.vcs}
         * {hero_pcgl_30_features.vcs, hero_pcgl_30_vs.vcs, hero_pcgl_30_ps.vcs, hero_pcgl_30_psrs.vcs}
         *
         * ordered as {features,vs,others}
         *
         *
         */
        public static List<string> GetRelatedFiles(ARCHIVE archive, string vcsCollectionName)
        {
            if (vcsCollectionName.EndsWith(".vcs"))
            {
                // e.g. 'hero_pcgl_30_ps.vcs' is shortened to 'hero_pcgl_30'
                vcsCollectionName = Path.GetFileName(vcsCollectionName);
                vcsCollectionName = vcsCollectionName.Substring(0, vcsCollectionName.LastIndexOf('_'));
            }
            List<string> relatedFiles = new();
            string featuresFile = null;
            foreach (var vcsFile in Directory.GetFiles(FileArchives.GetArchiveDir(archive)))
            {
                if (Path.GetFileName(vcsFile).StartsWith(vcsCollectionName))
                {
                    if (vcsFile.EndsWith("features.vcs"))
                    {
                        featuresFile = Path.GetFileName(vcsFile);
                    }
                    else if (vcsFile.EndsWith("vs.vcs"))
                    {
                        relatedFiles.Insert(0, Path.GetFileName(vcsFile));
                    }
                    else
                    {
                        relatedFiles.Add(Path.GetFileName(vcsFile));
                    }
                }
            }
            if (featuresFile != null)
            {
                relatedFiles.Insert(0, featuresFile);
            }
            return relatedFiles;
        }
    }
}
