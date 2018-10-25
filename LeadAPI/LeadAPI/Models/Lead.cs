using System.ComponentModel.DataAnnotations;
namespace LeadAPI
{

    public class Lead
    {

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string CorrelationId { get; set; }
    }

}