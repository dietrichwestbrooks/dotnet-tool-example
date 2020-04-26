namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using AspNetCore.Mvc.ApplicationParts;
    using McMaster.NETCore.Plugins;

    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddPlugins<TPlugin>(this IMvcBuilder builder, Action<TPlugin> configurator = null)
            where TPlugin : class
        {
            var assemblyFile =
                @"C:\Aristocrat\Projects\Monaco.Tool\src\Monaco.Tool.Emdi\bin\Debug\netcoreapp3.1\Monaco.Tool.Emdi.dll";

            // builder.AddPluginFromAssemblyFile(@"C:\Aristocrat\Projects\Monaco.Tool\src\Monaco.Tool.Emdi\bin\Debug\netcoreapp3.1\Monaco.Tool.Emdi.dll");
            
            return builder.AddPlugin(assemblyFile, configurator);
        }

        public static IMvcBuilder AddPlugin<TPlugin>(this IMvcBuilder builder, string assemblyFile, Action<TPlugin> configurator = null)
            where TPlugin : class
        {
            var plugin = PluginLoader.CreateFromAssemblyFile(
                assemblyFile,
                config =>
                    config.PreferSharedTypes = true);

            return builder.AddPlugin(plugin, configurator);
        }

        public static IMvcBuilder AddPlugin<TPlugin>(this IMvcBuilder builder, PluginLoader pluginLoader, Action<TPlugin> configurator = null)
            where TPlugin : class
        {
            var pluginAssembly = pluginLoader.LoadDefaultAssembly();

            foreach (var type in pluginAssembly.GetTypes().Where(x => typeof(TPlugin).IsAssignableFrom(x)))
            {
                var plugin = (TPlugin)Activator.CreateInstance(type);
                configurator?.Invoke(plugin);
                builder.Services.AddSingleton(plugin);
            }

            // This loads MVC application parts from plugin assemblies
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginAssembly);
            foreach (var part in partFactory.GetApplicationParts(pluginAssembly))
            {
                builder.PartManager.ApplicationParts.Add(part);
            }

            // This piece finds and loads related parts, such as MvcAppPlugin1.Views.dll.
            var relatedAssembliesAttrs = pluginAssembly.GetCustomAttributes<RelatedAssemblyAttribute>();
            foreach (var attr in relatedAssembliesAttrs)
            {
                var assembly = pluginLoader.LoadAssembly(attr.AssemblyFileName);
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                foreach (var part in partFactory.GetApplicationParts(assembly))
                {
                    builder.PartManager.ApplicationParts.Add(part);
                }
            }

            return builder;
        }
    }
}
