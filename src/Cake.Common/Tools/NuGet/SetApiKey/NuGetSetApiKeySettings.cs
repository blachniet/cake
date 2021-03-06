﻿using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Common.Tools.NuGet.SetApiKey
{
    /// <summary>
    /// Contains settings used by <see cref="NuGetSetApiKey"/>.
    /// </summary>
    public sealed class NuGetSetApiKeySettings : ToolSettings
    {
        /// <summary>
        /// Gets or sets the output verbosity.
        /// </summary>
        /// <value>The output verbosity.</value>
        public NuGetVerbosity? Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the NuGet configuration file.
        /// If not specified, the file <c>%AppData%\NuGet\NuGet.config</c> is used as the configuration file.
        /// </summary>
        /// <value>The NuGet configuration file.</value>
        public FilePath ConfigFile { get; set; }
    }
}