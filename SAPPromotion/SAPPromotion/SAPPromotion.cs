using System.Collections.Generic;

namespace SAPPromotion
    {
    public class SAPPromotion
    {
        public string type { get; set; }
        public string message { get; set; }
        public List<SAPPromotionJsonEntity> payload { get; set; }
        public string status { get; set; }
        public string id { get; set; }
    }
}
