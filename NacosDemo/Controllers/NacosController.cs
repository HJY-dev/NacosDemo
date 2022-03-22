using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using Nacos.V2.Naming;
using NacosDemo.NacosConfig;
using System;
using System.Threading.Tasks;

namespace NacosDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NacosController : ControllerBase
    {
        private readonly ILogger<NacosController> _logger;
        private readonly IConfiguration _configuration;
        private readonly INacosNamingService _nacosNamingService;
        private readonly INacosConfigService _nacosConfigService;

        public NacosController(
            ILogger<NacosController> logger,
            IConfiguration configuration,
            INacosNamingService nacosNamingService,
            INacosConfigService nacosConfigService
        )
        {
            _logger = logger;
            _configuration = configuration;
            _nacosNamingService = nacosNamingService;
            _nacosConfigService = nacosConfigService;
        }


        /// <summary>
        /// 获取Nacos 状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("getStatus")]
        public async Task<ApiJsonResult<string>> GetStatus()
        {
            var data = new ApiJsonResult<string>();
            var instances = await _nacosNamingService.GetAllInstances(JsonConfigSettings.NacosServiceName);
            if (instances == null || instances.Count == 0)
            {
                data.Code = ApiResultCode.Success;
                data.Data = "DOWN";
                return data;
            }
            // 获取当前程序IP
            var currentIp = IpHelper.GetCurrentIp(null);
            bool isUp = false;
            instances.ForEach(item =>
            {
                if (item.Ip == currentIp)
                    isUp = true;
            });
            // var baseUrl = await _nacosNamingService.GetServerStatus();
            if (isUp)
            {
                data.Code = ApiResultCode.Success;
                data.Data = "UP";
                return data;
            }
            else
            {
                data.Code = ApiResultCode.Success;
                data.Data = "DOWN";
                return data;
            }
        }


        #region 服务相关

        /// <summary>
        /// 服务上线
        /// </summary>
        /// <returns></returns>
        [HttpGet("register")]
        public async Task<ApiJsonResult<string>> Register()
        {
            var data = new ApiJsonResult<string>();
            var instance = new Nacos.V2.Naming.Dtos.Instance()
            {
                ServiceName = JsonConfigSettings.NacosServiceName,
                ClusterName = Nacos.V2.Common.Constants.DEFAULT_CLUSTER_NAME,
                Ip = IpHelper.GetCurrentIp(null),
                Port = JsonConfigSettings.NacosPort,
                Enabled = true,
                Weight = 100,
                Metadata = JsonConfigSettings.NacosMetadata
            };
            await _nacosNamingService.RegisterInstance(JsonConfigSettings.NacosServiceName, Nacos.V2.Common.Constants.DEFAULT_GROUP, instance);
            data.Code = ApiResultCode.Success;
            data.Data = "SUCCESS";
            return data;
        }

        /// <summary>
        /// 服务下线
        /// </summary>
        /// <returns></returns>
        [HttpGet("deregister")]
        public async Task<ApiJsonResult<string>> Deregister()
        {
            var data = new ApiJsonResult<string>();
            await _nacosNamingService.DeregisterInstance(JsonConfigSettings.NacosServiceName, Nacos.V2.Common.Constants.DEFAULT_GROUP, IpHelper.GetCurrentIp(null), JsonConfigSettings.NacosPort);
            data.Code = ApiResultCode.Success;
            data.Data = "SUCCESS";
            return data;
        }

        /// <summary>
        /// 服务订阅
        /// </summary>
        /// <returns></returns>
        [HttpGet("subscribe")]
        public async Task<ApiJsonResult<string>> Subscribe()
        {
            var data = new ApiJsonResult<string>();

            NamingServiceEventListener listener = new NamingServiceEventListener();
            await _nacosNamingService.Subscribe(JsonConfigSettings.NacosServiceName, Nacos.V2.Common.Constants.DEFAULT_GROUP,listener);

            data.Code = ApiResultCode.Success;
            data.Data = "SUCCESS";
            return data;
        }

        /// <summary>
        /// 取消服务订阅
        /// </summary>
        /// <returns></returns>
        [HttpGet("unsubscribe")]
        public async Task<ApiJsonResult<string>> Unsubscribe()
        {
            var data = new ApiJsonResult<string>();

            NamingServiceEventListener listener = new NamingServiceEventListener();
            await _nacosNamingService.Unsubscribe(JsonConfigSettings.NacosServiceName, Nacos.V2.Common.Constants.DEFAULT_GROUP, listener);

            data.Code = ApiResultCode.Success;
            data.Data = "SUCCESS";
            return data;
        }

        #endregion


        #region 配置相关

        /// <summary>
        /// 发布配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("publishConfig")]
        public async Task PublishConfig()
        {
            var dataId = "demo-dateid";
            var group = "demo-group";
            var val = "test-value-" + DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            await Task.Delay(500);
            var flag = await _nacosConfigService.PublishConfig(dataId, group, val);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("getConfig")]
        public async Task GetConfig()
        {
            var dataId = "demo-dateid";
            var group = "demo-group";

            await Task.Delay(500);
            var config = await _nacosConfigService.GetConfig(dataId, group, 5000L);
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("removeConfig")]
        public async Task RemoveConfig()
        {
            var dataId = "demo-dateid";
            var group = "demo-group";

            await Task.Delay(500);
            var flag = await _nacosConfigService.RemoveConfig(dataId, group);
        }

        /// <summary>
        /// 监听配置变更
        /// </summary>
        /// <returns></returns>
        [HttpGet("listenConfig")]
        public async Task ListenConfig()
        {
            var dataId = "demo-dateid";
            var group = "demo-group";
            NacosConfigListener listener = new NacosConfigListener ();
            await _nacosConfigService.AddListener(dataId, group, listener);
        }


        #endregion

    }

}
