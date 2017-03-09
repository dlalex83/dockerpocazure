using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IConnectionMultiplexer redisConnection;
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var myIp = "";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://api.ipify.org/?format=json");
                var content = await response.Content.ReadAsStringAsync();
                myIp = JToken.Parse(content).SelectToken("ip").Value<string>();
            }

            InitRedisConnection();
            var redisOk = redisConnection.IsConnected.ToString();

            return Json(new {redisConnected = redisOk, publicIp = myIp});

        }

        private void InitRedisConnection()
        {
            ConfigurationOptions config = ConfigurationOptions.Parse("cache:6379");

            DnsEndPoint addressEndpoint = config.EndPoints.First() as DnsEndPoint;
            int port = addressEndpoint.Port;

            bool isIp = IsIpAddress(addressEndpoint.Host);
            if (!isIp)
            {
                //Please Don't use this line in blocking context. Please remove ".Result"
                //Just for test purposes
                IPHostEntry ip = Dns.GetHostEntryAsync(addressEndpoint.Host).Result;
                config.EndPoints.Remove(addressEndpoint);
                config.EndPoints.Add(ip.AddressList.First(), port);
            }

            redisConnection = ConnectionMultiplexer.Connect(config);
        }

        bool IsIpAddress(string host)
        {
            string ipPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            return Regex.IsMatch(host, ipPattern);
        }

    }
}
