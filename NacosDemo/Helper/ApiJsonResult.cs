﻿namespace NacosDemo
{
    public class ApiJsonResult<T>
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public ApiResultCode Code { get; set; }
        /// <summary>
        /// 错误信息 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 具体数据
        /// </summary>
        public T Data { get; set; }
    }
    public enum ApiResultCode
    {
        //失败 
        Failed,
        //成功
        Success,
        //数据为空
        Empty,
    }
}
