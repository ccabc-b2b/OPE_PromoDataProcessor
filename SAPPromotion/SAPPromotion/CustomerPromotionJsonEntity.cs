using System.Collections.Generic;

namespace SAPPromotion
    {
    public class CustomerPromotionJsonEntity
    {
        public string type { get; set; }
        public string message { get; set; }
        public List<Payload> payload { get; set; }
        public string status { get; set; }
        public string id { get; set; }

    }
}
