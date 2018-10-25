using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Net.Mail;
using System;
using System.Linq;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;
using System.Net.Http;

namespace LeadAPI.Controllers
{
    /// <summary>
    /// Main controller for managing leads
    /// </summary>
    [RoutePrefix("api/LeadApi")] //contactapi
    public class LeadApiController : ApiController
    {
      
        //Saves a lead with name, email etc. Correlation ID can be an ID that is associated 
        //with this lead e.g. a whitepaper id that was downloaded by the lead

        //api/LeadApi/bill@microsoft.com/456781284231
        [HttpGet,Route("{email}/{correlationId}")]
        public IHttpActionResult Get(string email, string correlationId)
        {
          var lead = LeadRepository.Get(email, correlationId);

            if (lead != null)
                return Content(HttpStatusCode.OK, lead);
            else
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("the lead is not existed.");
                throw new HttpResponseException(message);
            }
        }

        //api/LeadApi/GetAll   
        [HttpGet,Route("GetAll")]
        public IHttpActionResult GetAll()
        {
               if (LeadRepository.DictionaryIsEmpty())
                    throw new HttpResponseException(HttpStatusCode.Gone);

                return Content(HttpStatusCode.OK, LeadRepository.GetAll());
           
        }

       //api/LeadApi/bill@microsoft.com/
        [HttpDelete,Route("{email}/{correlationId?}")]
        public IHttpActionResult Delete(string email, string correlationId="")
        {
            if (LeadRepository.DictionaryIsEmpty())
                throw new HttpResponseException(HttpStatusCode.Gone);

            if (LeadRepository.Delete(email, correlationId))
                return Ok(HttpStatusCode.OK);

            else
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("Oooops! the lead is not member of the dictinary.");
                throw new HttpResponseException(message);
            }
        }

        //api/LeadApi
        [HttpPost,Route("")]
        public IHttpActionResult Post([FromBody]Lead lead)
        {

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (LeadRepository.Save(lead))
                    return Content(HttpStatusCode.Created,"the new lead has added successfully.");

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Ambiguous);
                message.Content = new StringContent("Ooops! This lead is already existed in the dictionary.");
                throw new HttpResponseException(message);
        }


        [Route("GetCaller")]
        public IHttpActionResult GetCaller()
        {
                return Ok(LeadMiddlewares.IsInUnitTest()?"It is Unit test":"It is not Unit test");           
        }
    }

    public static class LeadRepository
    {
        //The in-memory-thread-unsafe repository
        private static readonly Dictionary<string, List<Lead>> leads = new Dictionary<string, List<Lead>>();
        
      
        public static bool Save(Lead lead)
        {
            if (!leads.ContainsKey(lead.Email))
            {
                //email no exists then add
                leads.Add(lead.Email, new List<Lead> { lead });
                LeadLog.writeTofile(lead);
                return true;
            }
            else
            {
                if (LeadRepository.Get(lead.Email, lead.CorrelationId) == null)
                {
                    //Email with this Id isn't exist then add
                    var listLead = leads.Where(x => x.Key == lead.Email).Select(x => x.Value).SingleOrDefault();
                    listLead.Add(lead);
                    leads[lead.Email] = listLead;
                    LeadLog.writeTofile(lead);
                    return true;
                }
                else
                {
                    //the email with this correlationId has already existed.
                    return false;
                }
            }

        }
        public static Lead Get(string email, string correlationId)
        {
            var query = leads.Where(x => x.Key == email).Select(x => x.Value).SingleOrDefault();
            if (query != null)
                return query.Where(y => y.CorrelationId == correlationId).SingleOrDefault();
            return null;

        }
        public static List<List<Lead>> GetAll()
        {
                return leads.Values.ToList();
        }
        public static bool Delete(string email, string correlationId)
        {
            if (string.IsNullOrEmpty(correlationId))
            {
                if (leads.Remove(email))
                    return true;
                return false;
            }
            else
            {
                var listLead = leads.Where(x => x.Key == email).Select(x => x.Value).SingleOrDefault();
                if (listLead != null)
                {
                    if (listLead.Remove(listLead.Where(x => x.CorrelationId == correlationId).SingleOrDefault()))
                        return true;
                }
               
                return false;
            }
        }
        public static bool DictionaryIsEmpty()
        {
            return leads.Count() == 0 ? true : false;
        }
        
    }

    public static class LeadMiddlewares
    {
        public static bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static bool IsInUnitTest()
        {
            string testAssemblyName = "UnitTest";
            return  AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.FullName.Contains(testAssemblyName));
        }
    }

    public static class LeadLog
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void ConfigureLogging()
        {
            var log4NetConfigFilePath = " C:\\Users\\matin\\Desktop\\Charlie Tango\\LeadAPI\\LeadAPI\\log4net.config";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(log4NetConfigFilePath));

        }
        public static void writeTofile(Lead lead)
        {
            ConfigureLogging();
            if (!LeadMiddlewares.IsInUnitTest())
            {
                log.Warn("directly from API !! From Post Method  " + lead.Email + "The CorrelatinId is:  " + lead.CorrelationId);
            }
            
        }
       

    }

}

