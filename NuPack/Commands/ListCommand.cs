﻿namespace NuGet.Commands {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using NuGet.Common;

    [Export(typeof(ICommand))]
    [Command(typeof(NuGetResources), "list", "ListCommandDescription", AltName = "l",
        UsageSummaryResourceName = "ListCommandUsageSummary", UsageDescriptionResourceName = "ListCommandUsageDescription")]
    public class ListCommand : ICommand {
        private const string _defaultFeedUrl = "http://go.microsoft.com/fwlink/?LinkID=204820";

        [Option(typeof(NuGetResources), "ListCommandSourceDescription", AltName = "s")]
        public string Source { get; set; }

        [Option(typeof(NuGetResources), "ListCommandVerboseListDescription", AltName = "v")]
        public bool Verbose { get; set; }

        public List<string> Arguments { get; set; }

        public IConsole Console { get; private set; }

        public IPackageRepositoryFactory RepositoryFactory { get; private set; }

        [ImportingConstructor]
        public ListCommand(IPackageRepositoryFactory packageRepositoryFactory, IConsole console) {
            if (console == null) {
                throw new ArgumentNullException("console");
            }

            if (packageRepositoryFactory == null) {
                throw new ArgumentNullException("packageRepositoryFactory");
            }

            Console = console;
            RepositoryFactory = packageRepositoryFactory;
        }

        public IQueryable<IPackage> GetPackages() {
            var feedUrl = _defaultFeedUrl;
            if (!String.IsNullOrEmpty(Source)) {
                feedUrl = Source;
            }

            var packageRepository = RepositoryFactory.CreateRepository(new PackageSource("feed", feedUrl));

            if (Arguments != null && Arguments.Any()) {
                return packageRepository.GetPackages().Find(Arguments.ToArray());
            }

            return packageRepository.GetPackages();
        }

        public void Execute() {

            IEnumerable<IPackage> packages = GetPackages();

            if (packages != null && packages.Any()) {
                if (Verbose) {
                    /***********************************************
                     * Package-Name
                     *  1.0.0.2010
                     *  This is the package Description
                     * 
                     * Package-Name-Two
                     *  2.0.0.2010
                     *  This is the second package Description
                     ***********************************************/
                    foreach (var p in packages) {
                        Console.PrintJustified(0, p.Id);
                        Console.PrintJustified(1, p.Version.ToString());
                        Console.PrintJustified(1, p.Description);
                        Console.WriteLine();
                    }
                }
                else {
                    /***********************************************
                     * Package-Name 1.0.0.2010
                     * Package-Name-Two 2.0.0.2010
                     ***********************************************/
                    foreach (var p in packages) {
                        Console.PrintJustified(0, p.GetFullName());
                    }
                }
            }
            else {
                Console.WriteLine(NuGetResources.ListCommandNoPackages);
            }
        }
    }
}
