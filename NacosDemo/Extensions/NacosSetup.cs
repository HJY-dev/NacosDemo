using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nacos.V2.DependencyInjection;
using NacosDemo.NacosConfig;
using System;

namespace NacosDemo.Extensions
{
    /// <summary>
    /// Nacos
    /// </summary>
    public static class NacosSetup
    {
        public static void AddNacosSetup(this IServiceCollection services, IConfiguration Configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 从当前配置取文件去注册naocs
            services.AddNacosV2Config(x =>
            {
                x.ServerAddresses = JsonConfigSettings.NacosServerAddresses;
                x.EndPoint = "";
                x.Namespace = JsonConfigSettings.NacosNamespace;
                x.DefaultTimeOut = JsonConfigSettings.NacosDefaultTimeOut;
                x.ListenInterval = JsonConfigSettings.ListenInterval;
                // swich to use http or rpc
                x.ConfigUseRpc = false;
            });
            services.AddNacosV2Naming(x =>
            {
                x.ServerAddresses = JsonConfigSettings.NacosServerAddresses;
                x.EndPoint = "";
                x.Namespace = JsonConfigSettings.NacosNamespace;
                x.DefaultTimeOut = JsonConfigSettings.NacosDefaultTimeOut;
                x.ListenInterval = JsonConfigSettings.ListenInterval;
                // swich to use http or rpc
                x.NamingUseRpc = false;
            });
            services.AddHostedService<NacosListenNamingTask>(); //增加服务注入，删除事件
                                                                // 监听nacos中的配置中心 如果有新配置变更 执行相关逻辑
            services.AddHostedService<NacosListenConfigurationTask>();//增加配置文件监听事件

            services.AddSingleton<IConfigurationManager>(new ConfigurationManager((ConfigurationRoot)Configuration));
            services.AddSingleton(Configuration);

        }
    }
}
