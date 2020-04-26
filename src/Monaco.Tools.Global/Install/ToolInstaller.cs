namespace Monaco.Tools.Install
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hosting;
    using Microsoft.Extensions.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    internal class ToolInstaller : IToolInstaller
    {
        private const string Framework = "netcoreapp3.1";

        private readonly ILogger<ToolInstaller> _logger;
        private readonly ApplicationContext _context;
        private readonly IApplicationHost _host;

        public ToolInstaller(ILogger<ToolInstaller> logger, ApplicationContext context, IApplicationHost host)
        {
            _logger = logger;
            _context = context;
            _host = host;
        }

        public async Task Install(string name, string version, string source,
            CancellationToken cancellationToken = default)
        {
            await _host.StartWebServer(cancellationToken);
            return;

            var settings = Settings.LoadDefaultSettings(root: _context.WorkingDirectory);
            var sourceRepositoryProvider = new SourceRepositoryProvider(settings, Repository.Provider.GetCoreV3());

            using var cacheContext = new SourceCacheContext();

            var repositories = sourceRepositoryProvider.GetRepositories();
            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

            await GetPackageDependencies(
                new PackageIdentity(name, NuGetVersion.Parse(version)),
                NuGetFramework.ParseFolder("net46"), cacheContext, NullLogger.Instance, repositories, availablePackages);

            foreach (var package in availablePackages)
            {
                _logger.LogDebug(package.ToString());
            }
        }

        private async Task GetPackageDependencies(PackageIdentity package,
            NuGetFramework framework,
            SourceCacheContext cacheContext,
            NuGet.Common.ILogger logger,
            IEnumerable<SourceRepository> repositories,
            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Any(x => x.Id == package.Id)) return;

            foreach (var sourceRepository in repositories)
            {
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None);

                if (dependencyInfo == null) continue;

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        framework, cacheContext, logger, repositories, availablePackages);
                }
            }
        }


        public async Task Install2(string name, string version, string source,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var logger = NullLogger.Instance;

                var package = new PackageIdentity(name, NuGetVersion.Parse(version));
                var settings = Settings.LoadDefaultSettings(root: _context.WorkingDirectory);
                var provider = new SourceRepositoryProvider(settings, Repository.Provider.GetCoreV3());
                var framework = NuGetFramework.ParseFolder(Framework);

                using (var cacheContext = new SourceCacheContext())
                {
                    foreach (var sourceRepository in provider.GetRepositories())
                    {
                        var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>(cancellationToken);
                        var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                            package, framework, cacheContext, logger, CancellationToken.None);

                        if (dependencyInfo != null)
                        {
                            Console.WriteLine(dependencyInfo);
                            return;
                        }
                    }
                }


                var cache = new SourceCacheContext();
                // var packageSource = new PackageSource(source);
                var repository = Repository.Factory.GetCoreV3(source);
                var resource = await repository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);

                var versions = await resource.GetAllVersionsAsync(
                    name,
                    cache,
                    logger,
                    cancellationToken);

                foreach (var v in versions)
                {
                    _logger.LogDebug($"Found version {v}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            await Task.CompletedTask;
        }
    }
}
