//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Yifei.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class YFPLUS_RefreshToken
    {
        public System.Guid ID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> UserID { get; set; }
        public System.DateTime IssuedUtc { get; set; }
        public System.DateTime ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
    }
}
