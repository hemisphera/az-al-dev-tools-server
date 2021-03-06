﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnZwDev.VSCodeLangServer.Protocol.MessageProtocol
{

    public abstract class RequestHandler<TParameters, TResult> : AbstractMessageHandler
    {

        public RequestHandler(string name) : base(name)
        {
        }

        public override async Task HandleRawMessage(Message requestMessage, MessageWriter messageWriter)
        {
            var requestContext = new RequestContext<TResult>(requestMessage, messageWriter);
            TParameters typedParams = default(TParameters);
            if (requestMessage.Contents != null)
            {
                // TODO: Catch parse errors!
                typedParams = requestMessage.Contents.ToObject<TParameters>();
            }

            TResult result = await HandleMessage(typedParams, requestContext);

            if (result != null)
                await requestContext.SendResult(result);
        }

        protected virtual async Task<TResult> HandleMessage(TParameters parameters, RequestContext<TResult> context)
        {
            return default(TResult);
        }

    }


}
