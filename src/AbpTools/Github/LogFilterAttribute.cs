using System;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace AbpTools.Github
{
    internal class LogFilter : ApiActionFilterAttribute
    {
        /// <summary>
        /// before request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnBeginRequestAsync(ApiActionContext context)
        {
            var request = context.RequestMessage;
            var dateTime = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine("{0} {1} {2}", dateTime, request.Method, request.RequestUri);

            context.Tags.Set("BeginTime", DateTime.Now);
            return base.OnBeginRequestAsync(context);
        }

        /// <summary>
        /// after request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnEndRequestAsync(ApiActionContext context)
        {
            if (context.Exception != null)
            {
                return;
            }

            var request = context.RequestMessage;
            var dateTime = DateTime.Now.ToString("HH:mm:ss.fff");
            var timeSpan = DateTime.Now.Subtract(context.Tags["BeginTime"].As<DateTime>());
            Console.WriteLine("{0} {1} {2} Finish，Time Cost:{3}", dateTime, request.Method, request.RequestUri.AbsolutePath, timeSpan);

            //output
            var result = await context.ResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}