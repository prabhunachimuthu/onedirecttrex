using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Repository;
using System.Net;
using OneDirect.Helper;
using OneDirect.Response;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageInterface lIMessageRepository;
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public MessageController(OneDirectContext context, ILogger<ProtocolController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIMessageRepository = new MessageRepository(context);
            lIProtocolRepository = new ProtocolRepository(context);
            lIPainRepository = new PainRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string _result = string.Empty;
            ProtocolList _protocolList = new ProtocolList();
            _protocolList.Protocol = new List<Protocol>();
            try
            {
                List<Protocol> lProtocollist = lIProtocolRepository.getMobileProtocol(id);
                if (lProtocollist != null && lProtocollist.Count > 0)
                {
                    _protocolList.Protocol = lProtocollist;
                    _protocolList.result = "success";
                    _result = Newtonsoft.Json.JsonConvert.SerializeObject(_protocolList);
                }
                else
                {
                    _protocolList.result = "success";
                    _result = Newtonsoft.Json.JsonConvert.SerializeObject(_protocolList);
                }
            }
            catch (Exception ex)
            {
                _protocolList.result = "failed";
                _result = Newtonsoft.Json.JsonConvert.SerializeObject(_protocolList);
            }
            return _result;
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Messages pMessage)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Message Post Start");

                if (pMessage != null)
                {

                    //Messages lMessage = lIMessageRepository.getMessage(pMessage.SenderId, pMessage.ReceiverId);
                    if (pMessage.ReadStatus != 2)
                    {
                        pMessage.MessageId = Guid.NewGuid().ToString();
                        pMessage.DateCreated = DateTime.UtcNow;
                        pMessage.DateModified = DateTime.UtcNow;
                        lIMessageRepository.InsertMessage(pMessage);
                    }
                    else
                    {
                        List<Messages> MessageList = lIMessageRepository.getBySenderIdAndReceiverId(pMessage.ReceiverId, pMessage.SenderId);
                        MessageList = MessageList.Where(x => x.ReadStatus == 1).ToList();
                        if (MessageList != null && MessageList.Count > 0)
                        {
                            foreach (Messages message in MessageList)
                            {
                                message.ReadStatus = 2;
                                message.DateModified = DateTime.UtcNow;
                                lIMessageRepository.UpdateMessage(message);
                            }
                        }
                        pMessage.MessageId = Guid.NewGuid().ToString();
                        pMessage.DateCreated = DateTime.UtcNow;
                        pMessage.DateModified = DateTime.UtcNow;
                        lIMessageRepository.InsertMessage(pMessage);
                    }


                    return Json(new { Status = (int)HttpStatusCode.OK });
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "Not Inserted";
                    response.Add("ErrorResponse", error);
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, response });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Get Rx Error: " + ex);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = ex.ToString();
                response.Add("ErrorResponse", error);
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, response });
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        [HttpGet]
        [Route("ViewMessages")]
        public IActionResult ViewMessages(string id)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            List<Messages> lMessageList = new List<Messages>();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    lMessageList = lIMessageRepository.getMessagebyMessageId(id);
                    string res = JsonConvert.SerializeObject(lMessageList);
                    JsonResult result = Json(new { Status = (int)HttpStatusCode.OK, MessageList = lMessageList });
                    return result;
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "Not found the message details";
                    response.Add("ErrorResponse", error);
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, response });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Get Rx Error: " + ex);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = ex.ToString();
                response.Add("ErrorResponse", error);
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, response });
            }
        }
    }
}
