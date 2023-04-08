
using System.ComponentModel.DataAnnotations;

namespace Entities.Enums
{
    /// <summary>
    /// ENUM 
    /// </summary>
    /// <remarks>
    ///<ul>  
    ///<li>1 = Success</li>
    ///<li>2 = Pending</li>
    ///<li>3 = Failed</li>
    ///<li>4 = Token Expired</li>
    ///<li>5 = Insufficient Balance</li>
    ///<li>6 = Order Placed</li>
    ///<li>7 = Order Canceled</li>
    ///<li>8 = An Error Occured</li>
    ///<li>9 = Order Delivered</li>
    ///  </ul>
    /// </remarks>
    public enum ResponseStatus
    {
        All = 0,
        Success = 1,
        Pending = 2,
        Failed = 3,
        [Display(Name = "Token Expired")]
        TokenExpired = 4,
        [Display(Name = "Insufficient Balance")]
        InsufficientBalance = 5,
        [Display(Name = "Order Placed")]
        OrderPlaced = 6,
        [Display(Name = "Order Canceled")]
        OrderCanceled = 7,
        [Display(Name = "An Error Occured")]
        Error = 8,
        [Display(Name = "Order Delivered")]
        OrderDelivered = 9,
		[Display(Name = "SWAPPED")]
		SWAPPED = 10,
		[Display(Name = "SHIPPING ADDRESS MISSING")]
		SHIPPING_ADDRESS_MISSING = 11,
	}
}
