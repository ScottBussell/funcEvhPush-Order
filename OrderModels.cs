using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace funcEvhPush_Order
{
    internal class OrderModel
    {
        // properties for order

        /// <summary>
        /// Identifier for the source of this order
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// (Unique) Identifier for the order specific to the source
        /// </summary>
        public string sourceOrderId { get; set; }






        // added for SNS event messages
        public string messageId { get; set; }
        public string message { get; set; }

        // added to get debugging info
        public string[] GetLoggingInfo
        {
            get
            {
                List<string> result = new List<string>();
                {
                    string.Format("Source: {0}", this.source);
                    string.Format("OrderID: {0}", this.sourceOrderId);
                };

                // if it's a SNS message, add message ID
                if (!string.IsNullOrEmpty(this.messageId))
                {
                    result.Add(string.Format("SNS MessageID: {0}", this.messageId));
                }

                //if (orderItems != null)
                //    if (orderItems.Length > 0)
                //        foreach (var oi in orderItems)
                //            result.AddRange(oi.GetLogIdentifiers);

                return result.ToArray();
            }
        }
    }
}
