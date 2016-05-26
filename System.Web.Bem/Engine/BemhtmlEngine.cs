﻿using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Web.Bem.BundleMappers;
using System.Web.Bem.Configuration;
using System.Web.Hosting;

namespace System.Web.Bem.Engine
{
    /// <summary>
    /// Template manager: init/cache/find
    /// </summary>
    public class BemhtmlEngine
    {
        private readonly Mapper mapper;
        private readonly ConcurrentDictionary<string, BemhtmlTemplate> cache;

        public BemhtmlEngine()
        {
            var config = BemConfiguration.Load();
            mapper = Mapper.Create(config);
            cache = new ConcurrentDictionary<string, BemhtmlTemplate>();
        }

        /// <summary>
        /// Create and cache bundle .NET wrapper
        /// </summary>
        /// <param name="bundleName"></param>
        public BemhtmlTemplate GetTemplate(string bundleName)
        {
            var path = mapper.Map(bundleName);
            return cache.GetOrAdd(path, InitTemplate);
        }

        private BemhtmlTemplate InitTemplate(string virtualPath)
        {
            var absolutePath = HostingEnvironment.MapPath(virtualPath);
            var content = File.ReadAllText(absolutePath);

            return new BemhtmlTemplate(content);
        }

        public Task<object> Render(string name, object data)
        {
            return GetTemplate(name).Apply(data);
        }
    }
}