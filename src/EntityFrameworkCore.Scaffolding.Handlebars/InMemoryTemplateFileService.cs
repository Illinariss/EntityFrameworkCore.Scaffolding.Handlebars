﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntityFrameworkCore.Scaffolding.Handlebars.Helpers;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Provides files to the template service from an in-memory store.
    /// </summary>
    public class InMemoryTemplateFileService : InMemoryFileService, ITemplateFileService
    {
        /// <summary>
        /// Allows files to be stored for later retrieval. Used for testing purposes.
        /// </summary>
        /// <param name="files">Files used by the template service.</param>
        /// <returns>Array of file paths.</returns>
        public virtual string[] InputFiles(params InputFile[] files)
        {
            var filePaths = new List<string>();

            foreach (var file in files)
            {
                if (!NameToContentMap.TryGetValue(file.Directory, out var filesMap))
                {
                    filesMap = new Dictionary<string, string>();
                    NameToContentMap[file.Directory] = filesMap;
                }

                filesMap[file.File] = file.Contents;

                var path = Path.Combine(file.Directory, file.File);
                filePaths.Add(path);
            }

            return filePaths.ToArray();
        }

        /// <summary>
        /// Retries all files from a relative directory.
        /// </summary>
        /// <param name="relativeDirectory">Relative directory name.</param>
        /// <returns>File names.</returns>
        public string[] RetrieveAllFileNames(string relativeDirectory)
        {
            if (!NameToContentMap.TryGetValue(relativeDirectory, out var filesMap))
            {
                throw new DirectoryNotFoundException("Could not find directory " + relativeDirectory);
            }
            return filesMap.Select(x=> x.Key).ToArray();
        }

        /// <summary>
        /// Finds all partial templates
        /// </summary>
        /// <param name="result">Dictionary containing template info</param>
        /// <param name="relativeDirectory">Relative Directory.</param>
        /// <returns></returns>
        public virtual Dictionary<string, TemplateFileInfo> FindAllPartialTemplates(Dictionary<string, TemplateFileInfo> result, string relativeDirectory)
        {
            foreach (var file in RetrieveAllFileNames(relativeDirectory))
            {
                result.Add(file, new TemplateFileInfo()
                {
                    RelativeDirectory = relativeDirectory,
                    FileName = file + Constants.TemplateExtension
                });
            }
            return result;
        }

        /// <summary>
        /// Retrieve template file contents from the file system. 
        /// If template is not present, copy it locally.
        /// </summary>
        /// <param name="relativeDirectory">Relative directory name.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="altRelativeDirectory">Alternative relative directory. Used for testing purposes.</param>
        /// <returns>File contents.</returns>
        public virtual string RetrieveTemplateFileContents(string relativeDirectory, string fileName, string altRelativeDirectory = null)
        {
            return RetrieveFileContents(relativeDirectory, fileName);
        }
    }
}
