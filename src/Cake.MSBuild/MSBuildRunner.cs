﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.MSBuild
{
    public sealed class MSBuildRunner
    {
        private readonly IProcessRunner _runner;

        public MSBuildRunner(IProcessRunner runner = null)
        {
            _runner = runner ?? new ProcessRunner();
        }

        public void Run(ICakeContext context, MSBuildSettings settings)
        {
            // Get the MSBuild path.
            var msBuildPath = MSBuildResolver.GetMSBuildPath(context, settings.ToolVersion, settings.PlatformTarget);
            if (!context.FileSystem.GetFile(msBuildPath).Exists)
            {
                var message = string.Format("Could not find MSBuild at {0}", msBuildPath);
                throw new CakeException(message);
            }

            // Start the process.
            var processInfo = GetProcessStartInfo(settings, context, msBuildPath);
            var process = _runner.Start(processInfo);
            if (process == null)
            {
                throw new CakeException("MSBuild process was not started.");
            }

            // Wait for the process to exit.
            process.WaitForExit();

            // Did an error occur?
            if (process.GetExitCode() != 0)
            {
                throw new CakeException("Build failed.");
            }
        }

        private static ProcessStartInfo GetProcessStartInfo(MSBuildSettings settings, ICakeContext context, FilePath msBuildPath)
        {
            var parameters = new List<string>();
            var properties = new List<string>();

            // Got a specific configuration in mind?
            if (!string.IsNullOrWhiteSpace(settings.Configuration))
            {
                // Add the configuration as a property.
                var configuration = settings.Configuration;
                properties.Add(string.Concat("Configuration", "=", configuration));
            }

            // Got any properties?
            if (settings.Properties.Count > 0)
            {
                properties.AddRange(settings.Properties.Select(x => string.Concat(x.Key, "=", x.Value)));
            }
            if (properties.Count > 0)
            {
                parameters.Add(string.Concat("/property:", string.Join(";", properties)));
            }

            // Got any targets?
            if (settings.Targets.Count > 0)
            {
                var targets = string.Join(";", settings.Targets);
                parameters.Add(string.Concat("/target:", targets));
            }
            else
            {
                // Use default target.
                parameters.Add("/target:Build");
            }

            // Add the solution as the last parameter.
            parameters.Add(settings.Solution.FullPath);

            return new ProcessStartInfo(msBuildPath.FullPath)
            {
                WorkingDirectory = context.Environment.WorkingDirectory.FullPath,
                Arguments = string.Join(" ", parameters),
                UseShellExecute = false
            };
        }
    }
}