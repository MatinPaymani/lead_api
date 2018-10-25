using System.Web.Http;
using System.Web.Http.Results;
using LeadAPI;
using LeadAPI.Controllers;
using NUnit.Framework;
using System.Net;

namespace LeadApiControllerTestFixture
{
    /// <summary>
    /// The controller fixture for testing the LeadApi controller methods and logic
    /// </summary>
    [TestFixture]
    public class LeadApiControllerFixture
    {
        [Test]
        public void PostingLead_ReturnsOK()
        {
            //Arrange
            var lead = new Lead()
            {
                Name = "Bill Gates",
                Email = "bill@microsoft.com",
                Title = "ex-ceo",
                Company = "microsoft",
                CorrelationId = "456781284231",
            };

            var leadController = new LeadApiController();

            //Act
            IHttpActionResult result = leadController.Post(lead);

            //Assert
            Assert.IsTrue(result is NegotiatedContentResult<string> negotiatedContentResult && negotiatedContentResult.Content == "the new lead has added successfully.");
        }

        [Test]
        public void PostingLead_SavesLead()
        {
            //Arrange
            var lead = new Lead()
                       {
                           Name = "Bill Gates",
                           Email = "bill@microsoft.com",
                           Title = "ex-ceo",
                           Company = "microsoft",
                           CorrelationId = "4567812842312",
                       };

            var leadController = new LeadApiController();

            //Act
            IHttpActionResult postResult = leadController.Post(lead);
            IHttpActionResult getResult = leadController.Get(lead.Email, lead.CorrelationId);

            //Assert
            Assert.IsTrue(postResult is NegotiatedContentResult<string> negotiatedContentResult && negotiatedContentResult.Content == "the new lead has added successfully.");
            Assert.IsTrue(getResult is NegotiatedContentResult<Lead> getNegotiatedContentResult && getNegotiatedContentResult.Content.Email == lead.Email && getNegotiatedContentResult.Content.CorrelationId == lead.CorrelationId);

        }

        [Test]
        public void DeletesLead()
        {
            //Arrange
            var lead = new Lead()
            {
                Name = "Bill Gates",
                Email = "bill@microsoft.com",
                Title = "ex-ceo",
                Company = "microsoft",
                CorrelationId = "456781284231",
            };

            var leadController = new LeadApiController();

            //Act
            IHttpActionResult postResult = leadController.Post(lead);
             IHttpActionResult delResult = leadController.Delete(lead.Email, lead.CorrelationId);

            //Assert
            Assert.IsTrue(postResult is NegotiatedContentResult<string> negotiatedContentResult && negotiatedContentResult.Content == "the new lead has added successfully.");
            Assert.IsTrue(delResult is OkNegotiatedContentResult<HttpStatusCode> DelnegotiatedContentResult && DelnegotiatedContentResult.Content == HttpStatusCode.OK);

        }

        [Test]
        public void Detector_UnitTest()
        {
            var leadController = new LeadApiController();

            //Act
            IHttpActionResult test = leadController.GetCaller();
            
            Assert.IsTrue(test is OkNegotiatedContentResult<string> negotiatedContentResult && negotiatedContentResult.Content == "It is Unit test");

        }



        //  Now implement further tests and LeadAPI  logic
        //  ----------------------------------------------
        //  * Fix any obvious errors and explain

        //  * Add email-address validation when attempting to post/save/persist a lead 

        //  * Make sure that correlationIds are associated with the lead when getting a lead by email and correlationId
        //  * Implement logic that allows a lead to be saved with multiple correlation ID's

        //  * Change the logic of the Get method to give a reasonable response if no lead was found
        //  * Implement the delete functionality and support your implementation by unit-tests

        //  * Introduce a logging mechanism in the LeadApiController that logs all requests
        //  * Make sure that the logger will not log entries when the unit-tests are executed

        //  * Looking forward to have a nice discussion about the solution and maybe extend it a bit further during our session
    }
}
